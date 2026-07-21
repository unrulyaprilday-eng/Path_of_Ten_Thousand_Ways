using System;
using System.Collections.Generic;
using System.Linq;

using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    // Runtime compatibility values are deliberately supplied by the caller so a
    // loaded run cannot silently follow a different content or map algorithm.
    public sealed class DemoJourneyRunSessionOptions
    {
        public string RunId { get; set; } = string.Empty;
        public string ConfigSchemaVersion { get; set; } = string.Empty;
        public string ContentVersion { get; set; } = string.Empty;
        public string MapAlgorithmVersion { get; set; } = string.Empty;
        public string RegionId { get; set; } = string.Empty;
        public DemoRunBuildSnapshot Build { get; set; } = new DemoRunBuildSnapshot();
        public DemoRunRealmSnapshot Realm { get; set; } = new DemoRunRealmSnapshot();
        public int MaxHealth { get; set; } = 72;
        public int CurrentHealth { get; set; } = 72;
    }

    // Node results are collected before the checkpoint is committed. Callers do
    // not receive the mutable staged save, so a result cannot change run identity
    // or graph membership outside the atomic transaction.
    public sealed class DemoJourneyNodeOutcome
    {
        private readonly List<string> experienceFlagIds = new List<string>();
        private readonly List<string> consumedUniqueContentIds = new List<string>();
        private readonly List<string> pendingMetaDiscoveryIds = new List<string>();

        public IReadOnlyList<string> ExperienceFlagIds => experienceFlagIds;
        public IReadOnlyList<string> ConsumedUniqueContentIds => consumedUniqueContentIds;
        public IReadOnlyList<string> PendingMetaDiscoveryIds => pendingMetaDiscoveryIds;
        public DemoRunBuildSnapshot Build { get; set; }
        public DemoRunRealmSnapshot Realm { get; set; }
        public bool GrantMinerSpiritLife { get; set; }
        public int BattlesWonDelta { get; set; }
        public int MiniBossesDefeatedDelta { get; set; }
        public int MaxSwordCount { get; set; }
        public int HighestBurstDamage { get; set; }
        public float ElapsedSecondsDelta { get; set; }
        public int? MaxHealth { get; set; }
        public int? CurrentHealth { get; set; }

        public void AddExperienceFlag(string id) { AddUnique(experienceFlagIds, id); }
        public void ConsumeUniqueContent(string id) { AddUnique(consumedUniqueContentIds, id); }
        public void AddPendingMetaDiscovery(string id) { AddUnique(pendingMetaDiscoveryIds, id); }

        private static void AddUnique(ICollection<string> destination, string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && !destination.Contains(id))
            {
                destination.Add(id);
            }
        }
    }

    public sealed class DemoJourneyRunSession
    {
        private readonly DemoJourneyGraph graph;
        private readonly IDemoRunSaveStore store;
        private readonly bool recoveredFromPrevious;
        private DemoRunSaveV2 checkpoint;

        private DemoJourneyRunSession(
            DemoJourneyGraph journeyGraph,
            IDemoRunSaveStore saveStore,
            DemoRunSaveV2 stableCheckpoint,
            bool usedPreviousCheckpoint)
        {
            graph = journeyGraph;
            store = saveStore;
            checkpoint = stableCheckpoint;
            recoveredFromPrevious = usedPreviousCheckpoint;
        }

        public DemoJourneyGraph Graph => graph;
        public IDemoRunSaveStore Store => store;
        public string RunId => checkpoint.RunId;
        public int RootSeed => checkpoint.RootSeed;
        public string CurrentNodeId => checkpoint.CurrentNodeId;
        public string FlowPhaseId => checkpoint.FlowPhaseId;
        public DemoRunSaveV2 Snapshot => checkpoint.DeepClone();

        public static bool TryCreateNew(
            DemoJourneyGraph journeyGraph,
            IDemoRunSaveStore saveStore,
            DemoJourneyRunSessionOptions options,
            int rootSeed,
            out DemoJourneyRunSession session,
            out string error)
        {
            session = null;
            if (journeyGraph == null || saveStore == null || options == null)
            {
                error = "A journey graph, save store and session options are required.";
                return false;
            }

            if (!ValidateOptions(options, out error))
            {
                return false;
            }

            if (journeyGraph.Seed != rootSeed)
            {
                error = "The journey graph seed must match the run root seed.";
                return false;
            }

            if (!journeyGraph.Validate(out IReadOnlyList<string> graphErrors))
            {
                error = "Journey graph validation failed: " + string.Join(" ", graphErrors);
                return false;
            }

            DemoJourneyNode start;
            if (!journeyGraph.TryGetNode(journeyGraph.StartNodeId, out start))
            {
                error = "The journey graph start node is missing.";
                return false;
            }

            DemoRunSaveV2 initial = new DemoRunSaveV2
            {
                ConfigSchemaVersion = options.ConfigSchemaVersion,
                ContentVersion = options.ContentVersion,
                MapAlgorithmVersion = options.MapAlgorithmVersion,
                RunId = options.RunId,
                RootSeed = rootSeed,
                CheckpointSequence = 0,
                FlowPhaseId = DemoRunFlowPhaseId.JourneyMap,
                RegionId = options.RegionId,
                ActIndex = start.ActIndex,
                CurrentNodeId = start.NodeId,
                LastCommittedNodeId = string.Empty,
                ResolvedGraphNodeSnapshotIds = GetGraphNodeIds(journeyGraph),
                CompletedNodeIds = new List<string>(),
                ReachableNodeIds = journeyGraph.GetReachableNodeIds(new string[0]).ToList(),
                Build = CloneBuild(options.Build),
                Realm = CloneRealm(options.Realm),
                MaxHealth = options.MaxHealth,
                CurrentHealth = options.CurrentHealth
            };

            if (!ValidateCheckpoint(initial, journeyGraph, options, out error))
            {
                return false;
            }

            if (!WriteAndVerify(saveStore, initial, out DemoRunSaveV2 persisted, out error))
            {
                return false;
            }

            if (!ValidateCheckpoint(persisted, journeyGraph, options, out error))
            {
                return false;
            }

            session = new DemoJourneyRunSession(journeyGraph, saveStore, persisted, false);
            return true;
        }

        public static bool TryRestore(
            DemoJourneyGraph journeyGraph,
            IDemoRunSaveStore saveStore,
            string configSchemaVersion,
            string contentVersion,
            string mapAlgorithmVersion,
            string regionId,
            out DemoJourneyRunSession session,
            out bool recoveredPrevious,
            out string error)
        {
            session = null;
            recoveredPrevious = false;
            error = string.Empty;
            if (journeyGraph == null || saveStore == null)
            {
                error = "A journey graph and save store are required for restore.";
                return false;
            }

            if (!journeyGraph.Validate(out IReadOnlyList<string> graphErrors))
            {
                error = "Journey graph validation failed: " + string.Join(" ", graphErrors);
                return false;
            }

            DemoJourneyRunSessionOptions compatibility = new DemoJourneyRunSessionOptions
            {
                ConfigSchemaVersion = configSchemaVersion ?? string.Empty,
                ContentVersion = contentVersion ?? string.Empty,
                MapAlgorithmVersion = mapAlgorithmVersion ?? string.Empty,
                RegionId = regionId ?? string.Empty
            };
            if (!ValidateCompatibility(compatibility, out error))
            {
                return false;
            }

            DemoRunSaveV2 candidate;
            string latestError;
            if (saveStore.TryLoadLatest(out candidate, out latestError))
            {
                if (ValidateCheckpoint(candidate, journeyGraph, compatibility, out error))
                {
                    session = new DemoJourneyRunSession(journeyGraph, saveStore, candidate, false);
                    return true;
                }

                latestError = error;
            }

            string previousError;
            if (saveStore.TryLoadPrevious(out candidate, out previousError))
            {
                if (ValidateCheckpoint(candidate, journeyGraph, compatibility, out error))
                {
                    recoveredPrevious = true;
                    session = new DemoJourneyRunSession(journeyGraph, saveStore, candidate, true);
                    return true;
                }

                previousError = error;
            }

            error = "Latest restore failed: " + latestError + " Previous restore failed: " + previousError;
            return false;
        }

        public bool TrySelectReachableNode(
            string nodeId,
            out DemoRunSaveV2 selected,
            out string error)
        {
            selected = null;
            DemoJourneyNode node;
            if (!graph.TryGetNode(nodeId, out node))
            {
                error = "Unknown journey node: " + (nodeId ?? string.Empty) + ".";
                return false;
            }

            if (node.IsCombat)
            {
                error = "Combat nodes require TrySelectEncounter so the encounter identity and seed are persisted.";
                return false;
            }

            return TrySelectInternal(node, string.Empty, null, out selected, out error);
        }

        public bool TrySelectEncounter(
            string nodeId,
            string encounterId,
            out DemoRunSaveV2 selected,
            out string error)
        {
            selected = null;
            DemoJourneyNode node;
            if (!graph.TryGetNode(nodeId, out node) || !node.IsCombat)
            {
                error = "A valid combat node is required for encounter selection.";
                return false;
            }

            int encounterSeed = ComputeEncounterSeed(checkpoint.RootSeed, nodeId, encounterId);
            return TrySelectInternal(node, encounterId, encounterSeed, out selected, out error);
        }

        public bool TrySelectEncounter(
            string nodeId,
            string encounterId,
            int encounterSeed,
            out DemoRunSaveV2 selected,
            out string error)
        {
            selected = null;
            DemoJourneyNode node;
            if (!graph.TryGetNode(nodeId, out node) || !node.IsCombat)
            {
                error = "A valid combat node is required for encounter selection.";
                return false;
            }

            int expectedSeed = ComputeEncounterSeed(checkpoint.RootSeed, nodeId, encounterId);
            if (encounterSeed != expectedSeed)
            {
                error = "Encounter seed does not match the run seed, node and encounter identity.";
                return false;
            }

            return TrySelectInternal(node, encounterId, encounterSeed, out selected, out error);
        }

        public bool TryCompleteCurrentNode(
            DemoJourneyNodeOutcome outcome,
            out DemoRunSaveV2 committed,
            out string error)
        {
            committed = null;
            if (outcome == null)
            {
                outcome = new DemoJourneyNodeOutcome();
            }

            if (checkpoint.FlowPhaseId == DemoRunFlowPhaseId.JourneyMap
                || checkpoint.FlowPhaseId == DemoRunFlowPhaseId.RunResult)
            {
                error = "The current node is not awaiting a node outcome.";
                return false;
            }

            DemoJourneyNode current;
            if (!graph.TryGetNode(checkpoint.CurrentNodeId, out current))
            {
                error = "The current checkpoint node is missing from the journey graph.";
                return false;
            }

            if (checkpoint.CompletedNodeIds.Contains(current.NodeId))
            {
                error = "The current node has already been settled and cannot be settled twice.";
                return false;
            }

            if (!checkpoint.ReachableNodeIds.Contains(current.NodeId))
            {
                error = "The current node is not in the checkpoint frontier.";
                return false;
            }

            if (!ValidateOutcome(outcome, out error))
            {
                return false;
            }

            if (!EnsureStoreHead(out error))
            {
                return false;
            }

            IReadOnlyList<string> nextReachable = graph.GetReachableNodeIds(
                checkpoint.CompletedNodeIds.Concat(new[] { current.NodeId }));
            string nextPhase = current.Type == DemoJourneyNodeType.Boss
                ? DemoRunFlowPhaseId.RunResult
                : DemoRunFlowPhaseId.JourneyMap;
            DemoNodeOutcomeTransaction transaction = new DemoNodeOutcomeTransaction(checkpoint);
            if (!transaction.TryStageCompletion(
                    current.NodeId,
                    current.NodeId,
                    nextPhase,
                    nextReachable,
                    out error))
            {
                return false;
            }

            ApplyOutcome(transaction.Staged, outcome);
            if (!HasStableIdentity(transaction.Staged, checkpoint))
            {
                error = "A node outcome changed stable run or graph identity.";
                return false;
            }

            if (!ValidateCheckpoint(transaction.Staged, graph, CompatibilityFrom(checkpoint), out error))
            {
                return false;
            }

            if (!transaction.TryCommit(store, out committed, out error))
            {
                return false;
            }

            if (!ValidateCheckpoint(committed, graph, CompatibilityFrom(committed), out error))
            {
                return false;
            }

            checkpoint = committed;
            committed = checkpoint.DeepClone();
            return true;
        }

        public static int ComputeEncounterSeed(int rootSeed, string nodeId, string encounterId)
        {
            unchecked
            {
                uint hash = 2166136261u ^ (uint)rootSeed;
                hash = Mix(hash, nodeId);
                hash = Mix(hash, encounterId);
                int result = (int)(hash & 0x7fffffff);
                return result == 0 ? 1 : result;
            }
        }

        private bool TrySelectInternal(
            DemoJourneyNode node,
            string encounterId,
            int? encounterSeed,
            out DemoRunSaveV2 selected,
            out string error)
        {
            selected = null;
            if (checkpoint.FlowPhaseId != DemoRunFlowPhaseId.JourneyMap)
            {
                if (string.Equals(checkpoint.CurrentNodeId, node.NodeId, StringComparison.Ordinal)
                    && string.Equals(checkpoint.PendingEncounterId, encounterId ?? string.Empty, StringComparison.Ordinal)
                    && checkpoint.PendingEncounterSeed == encounterSeed)
                {
                    if (!EnsureStoreHead(out error))
                    {
                        return false;
                    }

                    selected = checkpoint.DeepClone();
                    error = string.Empty;
                    return true;
                }

                error = "A new node can only be selected from the journey map phase.";
                return false;
            }

            if (!checkpoint.ReachableNodeIds.Contains(node.NodeId)
                || checkpoint.CompletedNodeIds.Contains(node.NodeId))
            {
                error = "The selected node is not currently reachable.";
                return false;
            }

            if (!EnsureStoreHead(out error))
            {
                return false;
            }

            if (node.IsCombat != !string.IsNullOrEmpty(encounterId))
            {
                error = node.IsCombat
                    ? "Combat nodes require an encounter identity."
                    : "Non-combat nodes cannot carry an encounter identity.";
                return false;
            }

            DemoRunSaveV2 candidate = checkpoint.DeepClone();
            candidate.CurrentNodeId = node.NodeId;
            candidate.ActIndex = node.ActIndex;
            candidate.FlowPhaseId = node.IsCombat
                ? DemoRunFlowPhaseId.EncounterIntro
                : node.Type == DemoJourneyNodeType.Breakthrough
                    ? DemoRunFlowPhaseId.Breakthrough
                    : DemoRunFlowPhaseId.NodeScene;
            candidate.PendingEncounterId = encounterId ?? string.Empty;
            candidate.PendingEncounterSeed = encounterSeed;
            candidate.CheckpointSequence = checkpoint.CheckpointSequence + 1;

            if (!ValidateCheckpoint(candidate, graph, CompatibilityFrom(checkpoint), out error))
            {
                return false;
            }

            if (!WriteAndVerify(store, candidate, out selected, out error))
            {
                return false;
            }

            checkpoint = selected;
            selected = checkpoint.DeepClone();
            return true;
        }

        private bool EnsureStoreHead(out string error)
        {
            if (store.TryLoadLatest(out DemoRunSaveV2 latest, out string latestError))
            {
                if (SameCheckpointIdentity(latest, checkpoint))
                {
                    error = string.Empty;
                    return true;
                }

                if (recoveredFromPrevious
                    && string.Equals(latest.RunId, checkpoint.RunId, StringComparison.Ordinal)
                    && store.TryLoadPrevious(out DemoRunSaveV2 recovered, out string recoveredError)
                    && SameCheckpointIdentity(recovered, checkpoint))
                {
                    error = string.Empty;
                    return true;
                }

                error = "The latest checkpoint changed outside this run session.";
                return false;
            }

            if (store.TryLoadPrevious(out DemoRunSaveV2 previous, out string previousError)
                && SameCheckpointIdentity(previous, checkpoint))
            {
                error = string.Empty;
                return true;
            }

            error = "The save store head cannot be matched to this run session. Latest: "
                + latestError + " Previous: " + previousError;
            return false;
        }

        private static bool WriteAndVerify(
            IDemoRunSaveStore saveStore,
            DemoRunSaveV2 candidate,
            out DemoRunSaveV2 persisted,
            out string error)
        {
            persisted = null;
            if (candidate.Validate().Count > 0)
            {
                error = string.Join(" ", candidate.Validate());
                return false;
            }

            if (!saveStore.TryWriteCheckpoint(candidate, out error)
                || !saveStore.TryLoadLatest(out persisted, out error))
            {
                return false;
            }

            if (!SameCheckpointIdentity(persisted, candidate))
            {
                error = "The persisted checkpoint identity changed during verification.";
                persisted = null;
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool ValidateCheckpoint(
            DemoRunSaveV2 save,
            DemoJourneyGraph journeyGraph,
            DemoJourneyRunSessionOptions compatibility,
            out string error)
        {
            if (save == null)
            {
                error = "The checkpoint is empty.";
                return false;
            }

            IReadOnlyList<string> saveErrors = save.Validate();
            if (saveErrors.Count > 0)
            {
                error = string.Join(" ", saveErrors);
                return false;
            }

            if (save.RootSeed != journeyGraph.Seed
                || !string.Equals(save.ConfigSchemaVersion, compatibility.ConfigSchemaVersion, StringComparison.Ordinal)
                || !string.Equals(save.ContentVersion, compatibility.ContentVersion, StringComparison.Ordinal)
                || !string.Equals(save.MapAlgorithmVersion, compatibility.MapAlgorithmVersion, StringComparison.Ordinal)
                || !string.Equals(save.RegionId, compatibility.RegionId, StringComparison.Ordinal))
            {
                error = "Checkpoint seed, region or configuration version does not match the active journey.";
                return false;
            }

            HashSet<string> expectedGraphIds = new HashSet<string>(GetGraphNodeIds(journeyGraph), StringComparer.Ordinal);
            HashSet<string> savedGraphIds = new HashSet<string>(save.ResolvedGraphNodeSnapshotIds, StringComparer.Ordinal);
            if (!expectedGraphIds.SetEquals(savedGraphIds))
            {
                error = "Checkpoint graph snapshot does not match the active journey graph.";
                return false;
            }

            DemoJourneyNode current;
            if (!journeyGraph.TryGetNode(save.CurrentNodeId, out current) || current.ActIndex != save.ActIndex)
            {
                error = "Checkpoint current node or act does not match the active journey graph.";
                return false;
            }

            IReadOnlyList<string> expectedReachable = journeyGraph.GetReachableNodeIds(save.CompletedNodeIds);
            if (!new HashSet<string>(expectedReachable, StringComparer.Ordinal)
                .SetEquals(save.ReachableNodeIds))
            {
                error = "Checkpoint reachable frontier does not match completed nodes.";
                return false;
            }

            if (save.Statistics.NodesCompleted != save.CompletedNodeIds.Count)
            {
                error = "Checkpoint node completion count does not match completed node IDs.";
                return false;
            }

            bool hasEncounter = !string.IsNullOrEmpty(save.PendingEncounterId);
            if (hasEncounter)
            {
                if (!current.IsCombat
                    || save.FlowPhaseId != DemoRunFlowPhaseId.EncounterIntro
                    || save.PendingEncounterSeed != ComputeEncounterSeed(save.RootSeed, current.NodeId, save.PendingEncounterId))
                {
                    error = "Checkpoint encounter identity is invalid for the current node.";
                    return false;
                }
            }
            else if (save.FlowPhaseId == DemoRunFlowPhaseId.EncounterIntro)
            {
                error = "Encounter intro checkpoints must contain an encounter identity and seed.";
                return false;
            }

            if (save.FlowPhaseId == DemoRunFlowPhaseId.JourneyMap)
            {
                bool initialStart = save.CompletedNodeIds.Count == 0 && current.NodeId == journeyGraph.StartNodeId;
                if (!initialStart && !save.CompletedNodeIds.Contains(current.NodeId))
                {
                    error = "Journey map checkpoints must anchor on the start or last completed node.";
                    return false;
                }
            }
            else if (save.FlowPhaseId != DemoRunFlowPhaseId.RunResult
                && save.CompletedNodeIds.Contains(current.NodeId))
            {
                error = "An active node checkpoint cannot reopen a completed node.";
                return false;
            }

            if (save.FlowPhaseId != DemoRunFlowPhaseId.JourneyMap
                && save.FlowPhaseId != DemoRunFlowPhaseId.RunResult
                && !save.ReachableNodeIds.Contains(current.NodeId))
            {
                error = "An active node checkpoint must point to the current reachable frontier.";
                return false;
            }

            if (!string.IsNullOrEmpty(save.LastCommittedNodeId)
                && !save.CompletedNodeIds.Contains(save.LastCommittedNodeId))
            {
                error = "Last committed node is not present in completed node IDs.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static void ApplyOutcome(DemoRunSaveV2 target, DemoJourneyNodeOutcome outcome)
        {
            foreach (string id in outcome.ExperienceFlagIds) AddUnique(target.ExperienceFlagIds, id);
            foreach (string id in outcome.ConsumedUniqueContentIds) AddUnique(target.ConsumedUniqueContentIds, id);
            foreach (string id in outcome.PendingMetaDiscoveryIds) AddUnique(target.PendingMetaDiscoveryIds, id);
            if (outcome.Build != null) target.Build = CloneBuild(outcome.Build);
            if (outcome.Realm != null) target.Realm = CloneRealm(outcome.Realm);
            if (outcome.GrantMinerSpiritLife) target.MinerSpiritLife.Granted = true;
            target.Statistics.BattlesWon += outcome.BattlesWonDelta;
            target.Statistics.MiniBossesDefeated += outcome.MiniBossesDefeatedDelta;
            target.Statistics.MaxSwordCount = Math.Max(target.Statistics.MaxSwordCount, outcome.MaxSwordCount);
            target.Statistics.HighestBurstDamage = Math.Max(target.Statistics.HighestBurstDamage, outcome.HighestBurstDamage);
            target.Statistics.ElapsedSeconds += outcome.ElapsedSecondsDelta;
            if (outcome.MaxHealth.HasValue)
            {
                target.MaxHealth = Math.Max(1, outcome.MaxHealth.Value);
            }
            if (outcome.CurrentHealth.HasValue)
            {
                target.CurrentHealth = Math.Max(0, Math.Min(target.MaxHealth, outcome.CurrentHealth.Value));
            }
        }

        private static bool ValidateOutcome(DemoJourneyNodeOutcome outcome, out string error)
        {
            if (outcome.BattlesWonDelta < 0 || outcome.MiniBossesDefeatedDelta < 0
                || outcome.MaxSwordCount < 0 || outcome.HighestBurstDamage < 0
                || outcome.ElapsedSecondsDelta < 0f || float.IsNaN(outcome.ElapsedSecondsDelta)
                || float.IsInfinity(outcome.ElapsedSecondsDelta)
                || (outcome.MaxHealth.HasValue && outcome.MaxHealth.Value <= 0)
                || (outcome.CurrentHealth.HasValue && outcome.CurrentHealth.Value < 0)
                || (outcome.MaxHealth.HasValue && outcome.CurrentHealth.HasValue
                    && outcome.CurrentHealth.Value > outcome.MaxHealth.Value))
            {
                error = "Node outcome statistics cannot be negative or non-finite.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool ValidateOptions(DemoJourneyRunSessionOptions options, out string error)
        {
            if (string.IsNullOrWhiteSpace(options.RunId)
                || string.IsNullOrWhiteSpace(options.ConfigSchemaVersion)
                || string.IsNullOrWhiteSpace(options.ContentVersion)
                || string.IsNullOrWhiteSpace(options.MapAlgorithmVersion)
                || string.IsNullOrWhiteSpace(options.RegionId)
                || options.MaxHealth <= 0
                || options.CurrentHealth < 0
                || options.CurrentHealth > options.MaxHealth)
            {
                error = "Run ID, region and all compatibility versions are required.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static bool ValidateCompatibility(DemoJourneyRunSessionOptions options, out string error)
        {
            if (string.IsNullOrWhiteSpace(options.ConfigSchemaVersion)
                || string.IsNullOrWhiteSpace(options.ContentVersion)
                || string.IsNullOrWhiteSpace(options.MapAlgorithmVersion)
                || string.IsNullOrWhiteSpace(options.RegionId))
            {
                error = "Region and all compatibility versions are required for restore.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        private static DemoJourneyRunSessionOptions CompatibilityFrom(DemoRunSaveV2 save)
        {
            return new DemoJourneyRunSessionOptions
            {
                ConfigSchemaVersion = save.ConfigSchemaVersion,
                ContentVersion = save.ContentVersion,
                MapAlgorithmVersion = save.MapAlgorithmVersion,
                RegionId = save.RegionId
            };
        }

        private static bool HasStableIdentity(DemoRunSaveV2 left, DemoRunSaveV2 right)
        {
            return left.RootSeed == right.RootSeed
                && string.Equals(left.RunId, right.RunId, StringComparison.Ordinal)
                && string.Equals(left.ConfigSchemaVersion, right.ConfigSchemaVersion, StringComparison.Ordinal)
                && string.Equals(left.ContentVersion, right.ContentVersion, StringComparison.Ordinal)
                && string.Equals(left.MapAlgorithmVersion, right.MapAlgorithmVersion, StringComparison.Ordinal)
                && string.Equals(left.RegionId, right.RegionId, StringComparison.Ordinal)
                && new HashSet<string>(left.ResolvedGraphNodeSnapshotIds, StringComparer.Ordinal)
                    .SetEquals(right.ResolvedGraphNodeSnapshotIds);
        }

        private static bool SameCheckpointIdentity(DemoRunSaveV2 left, DemoRunSaveV2 right)
        {
            return left != null && right != null
                && string.Equals(left.RunId, right.RunId, StringComparison.Ordinal)
                && left.CheckpointSequence == right.CheckpointSequence
                && string.Equals(left.CurrentNodeId, right.CurrentNodeId, StringComparison.Ordinal)
                && string.Equals(left.FlowPhaseId, right.FlowPhaseId, StringComparison.Ordinal)
                && string.Equals(left.PendingEncounterId, right.PendingEncounterId, StringComparison.Ordinal)
                && left.PendingEncounterSeed == right.PendingEncounterSeed;
        }

        private static List<string> GetGraphNodeIds(DemoJourneyGraph journeyGraph)
        {
            return journeyGraph.Nodes.Select(node => node.NodeId).Distinct(StringComparer.Ordinal).ToList();
        }

        private static uint Mix(uint hash, string value)
        {
            if (value == null) value = string.Empty;
            for (int i = 0; i < value.Length; i++)
            {
                hash ^= value[i];
                hash *= 16777619u;
            }
            hash ^= 0xffu;
            hash *= 16777619u;
            return hash;
        }

        private static void AddUnique(ICollection<string> destination, string id)
        {
            if (!string.IsNullOrWhiteSpace(id) && !destination.Contains(id)) destination.Add(id);
        }

        private static DemoRunBuildSnapshot CloneBuild(DemoRunBuildSnapshot source)
        {
            source = source ?? new DemoRunBuildSnapshot();
            return new DemoRunBuildSnapshot
            {
                StartingPracticePackageId = source.StartingPracticePackageId ?? string.Empty,
                MindMethodId = source.MindMethodId ?? string.Empty,
                MindMethodLevel = source.MindMethodLevel,
                InnateArtifactId = source.InnateArtifactId ?? string.Empty,
                InnateArtifactRefinementStage = source.InnateArtifactRefinementStage,
                TechniqueIds = source.TechniqueIds == null ? new List<string>() : source.TechniqueIds.ToList(),
                AcquiredArtifactIds = source.AcquiredArtifactIds == null ? new List<string>() : source.AcquiredArtifactIds.ToList()
            };
        }

        private static DemoRunRealmSnapshot CloneRealm(DemoRunRealmSnapshot source)
        {
            source = source ?? new DemoRunRealmSnapshot();
            return new DemoRunRealmSnapshot
            {
                RealmId = source.RealmId ?? string.Empty,
                Stage = source.Stage,
                FoundationRuleId = source.FoundationRuleId ?? string.Empty,
                BreakthroughSourceIds = source.BreakthroughSourceIds == null ? new List<string>() : source.BreakthroughSourceIds.ToList()
            };
        }
    }
}
