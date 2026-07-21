using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace PathOfTenThousandWays.Demo.Systems
{
    public static class DemoRunFlowPhaseId
    {
        public const string Home = "home";
        public const string OpeningStory = "opening_story";
        public const string RegionChoice = "region_choice";
        public const string JourneyMap = "journey_map";
        public const string NodeScene = "node_scene";
        public const string EncounterIntro = "encounter_intro";
        public const string BattleOutcome = "battle_outcome";
        public const string Breakthrough = "breakthrough";
        public const string RunResult = "run_result";

        private static readonly HashSet<string> ValidIds = new HashSet<string>(StringComparer.Ordinal)
        {
            Home,
            OpeningStory,
            RegionChoice,
            JourneyMap,
            NodeScene,
            EncounterIntro,
            BattleOutcome,
            Breakthrough,
            RunResult
        };

        public static bool IsValid(string phaseId)
        {
            return !string.IsNullOrEmpty(phaseId) && ValidIds.Contains(phaseId);
        }
    }

    [DataContract]
    public sealed class DemoRunBuildSnapshot
    {
        [DataMember(Order = 1)] public string StartingPracticePackageId { get; set; } = string.Empty;
        [DataMember(Order = 2)] public string MindMethodId { get; set; } = string.Empty;
        [DataMember(Order = 3)] public int MindMethodLevel { get; set; }
        [DataMember(Order = 4)] public string InnateArtifactId { get; set; } = string.Empty;
        [DataMember(Order = 5)] public int InnateArtifactRefinementStage { get; set; }
        [DataMember(Order = 6)] public List<string> TechniqueIds { get; set; } = new List<string>();
        [DataMember(Order = 7)] public List<string> AcquiredArtifactIds { get; set; } = new List<string>();

        internal void Normalize()
        {
            StartingPracticePackageId = StartingPracticePackageId ?? string.Empty;
            MindMethodId = MindMethodId ?? string.Empty;
            InnateArtifactId = InnateArtifactId ?? string.Empty;
            TechniqueIds = TechniqueIds ?? new List<string>();
            AcquiredArtifactIds = AcquiredArtifactIds ?? new List<string>();
        }
    }

    [DataContract]
    public sealed class DemoRunRealmSnapshot
    {
        [DataMember(Order = 1)] public string RealmId { get; set; } = string.Empty;
        [DataMember(Order = 2)] public int Stage { get; set; }
        [DataMember(Order = 3)] public string FoundationRuleId { get; set; } = string.Empty;
        [DataMember(Order = 4)] public List<string> BreakthroughSourceIds { get; set; } = new List<string>();

        internal void Normalize()
        {
            RealmId = RealmId ?? string.Empty;
            FoundationRuleId = FoundationRuleId ?? string.Empty;
            BreakthroughSourceIds = BreakthroughSourceIds ?? new List<string>();
        }
    }

    [DataContract]
    public sealed class DemoMinerSpiritLifeSnapshot
    {
        [DataMember(Order = 1)] public bool Granted { get; set; }
        [DataMember(Order = 2)] public bool Consumed { get; set; }
    }

    [DataContract]
    public sealed class DemoRunStatisticsSnapshot
    {
        [DataMember(Order = 1)] public int BattlesWon { get; set; }
        [DataMember(Order = 2)] public int MiniBossesDefeated { get; set; }
        [DataMember(Order = 3)] public int NodesCompleted { get; set; }
        [DataMember(Order = 4)] public int MaxSwordCount { get; set; }
        [DataMember(Order = 5)] public int HighestBurstDamage { get; set; }
        [DataMember(Order = 6)] public int MinerSpiritRetriesUsed { get; set; }
        [DataMember(Order = 7)] public float ElapsedSeconds { get; set; }
    }

    [DataContract]
    public sealed class DemoRunSaveV2
    {
        public const int CurrentSaveVersion = 2;

        [DataMember(Order = 1)] public int SaveVersion { get; set; } = CurrentSaveVersion;
        [DataMember(Order = 2)] public string ConfigSchemaVersion { get; set; } = string.Empty;
        [DataMember(Order = 3)] public string ContentVersion { get; set; } = string.Empty;
        [DataMember(Order = 4)] public string MapAlgorithmVersion { get; set; } = string.Empty;
        [DataMember(Order = 5)] public string RunId { get; set; } = string.Empty;
        [DataMember(Order = 6)] public int RootSeed { get; set; }
        [DataMember(Order = 7)] public int CheckpointSequence { get; set; }
        [DataMember(Order = 8)] public string FlowPhaseId { get; set; } = DemoRunFlowPhaseId.OpeningStory;
        [DataMember(Order = 9)] public string RegionId { get; set; } = string.Empty;
        [DataMember(Order = 10)] public int ActIndex { get; set; }
        [DataMember(Order = 11)] public string CurrentNodeId { get; set; } = string.Empty;
        [DataMember(Order = 12)] public string LastCommittedNodeId { get; set; } = string.Empty;
        [DataMember(Order = 13)] public List<string> ResolvedGraphNodeSnapshotIds { get; set; } = new List<string>();
        [DataMember(Order = 14)] public List<string> CompletedNodeIds { get; set; } = new List<string>();
        [DataMember(Order = 15)] public List<string> ReachableNodeIds { get; set; } = new List<string>();
        [DataMember(Order = 16)] public DemoRunBuildSnapshot Build { get; set; } = new DemoRunBuildSnapshot();
        [DataMember(Order = 17)] public DemoRunRealmSnapshot Realm { get; set; } = new DemoRunRealmSnapshot();
        [DataMember(Order = 18)] public List<string> ExperienceFlagIds { get; set; } = new List<string>();
        [DataMember(Order = 19)] public List<string> ConsumedUniqueContentIds { get; set; } = new List<string>();
        [DataMember(Order = 20)] public List<string> PendingMetaDiscoveryIds { get; set; } = new List<string>();
        [DataMember(Order = 21)] public DemoMinerSpiritLifeSnapshot MinerSpiritLife { get; set; } = new DemoMinerSpiritLifeSnapshot();
        [DataMember(Order = 22)] public string PendingEncounterId { get; set; } = string.Empty;
        [DataMember(Order = 23)] public int? PendingEncounterSeed { get; set; }
        [DataMember(Order = 24)] public DemoRunStatisticsSnapshot Statistics { get; set; } = new DemoRunStatisticsSnapshot();
        [DataMember(Order = 25)] public int MaxHealth { get; set; } = 72;
        [DataMember(Order = 26)] public int CurrentHealth { get; set; } = 72;

        public string MetaCommitIdempotencyKey => RunId;

        public void Normalize()
        {
            ConfigSchemaVersion = ConfigSchemaVersion ?? string.Empty;
            ContentVersion = ContentVersion ?? string.Empty;
            MapAlgorithmVersion = MapAlgorithmVersion ?? string.Empty;
            RunId = RunId ?? string.Empty;
            FlowPhaseId = FlowPhaseId ?? string.Empty;
            RegionId = RegionId ?? string.Empty;
            CurrentNodeId = CurrentNodeId ?? string.Empty;
            LastCommittedNodeId = LastCommittedNodeId ?? string.Empty;
            ResolvedGraphNodeSnapshotIds = ResolvedGraphNodeSnapshotIds ?? new List<string>();
            CompletedNodeIds = CompletedNodeIds ?? new List<string>();
            ReachableNodeIds = ReachableNodeIds ?? new List<string>();
            Build = Build ?? new DemoRunBuildSnapshot();
            Realm = Realm ?? new DemoRunRealmSnapshot();
            ExperienceFlagIds = ExperienceFlagIds ?? new List<string>();
            ConsumedUniqueContentIds = ConsumedUniqueContentIds ?? new List<string>();
            PendingMetaDiscoveryIds = PendingMetaDiscoveryIds ?? new List<string>();
            MinerSpiritLife = MinerSpiritLife ?? new DemoMinerSpiritLifeSnapshot();
            PendingEncounterId = PendingEncounterId ?? string.Empty;
            Statistics = Statistics ?? new DemoRunStatisticsSnapshot();
            Build.Normalize();
            Realm.Normalize();
        }

        public IReadOnlyList<string> Validate()
        {
            Normalize();
            List<string> errors = new List<string>();

            if (SaveVersion != CurrentSaveVersion)
            {
                errors.Add("Unsupported save_version: " + SaveVersion + ".");
            }

            RequireValue(errors, ConfigSchemaVersion, "config_schema_version");
            RequireValue(errors, ContentVersion, "content_version");
            RequireValue(errors, MapAlgorithmVersion, "map_algorithm_version");
            RequireValue(errors, RunId, "run_id");
            RequireValue(errors, RegionId, "region_id");
            RequireValue(errors, CurrentNodeId, "current_node_id");

            if (!DemoRunFlowPhaseId.IsValid(FlowPhaseId))
            {
                errors.Add("Unknown flow_phase_id: " + FlowPhaseId + ".");
            }

            if (ActIndex < 0 || CheckpointSequence < 0)
            {
                errors.Add("Act index and checkpoint sequence cannot be negative.");
            }

            ValidateStableIds(errors, ResolvedGraphNodeSnapshotIds, "resolved_graph_node_snapshot_ids");
            ValidateStableIds(errors, CompletedNodeIds, "completed_node_ids");
            ValidateStableIds(errors, ReachableNodeIds, "reachable_node_ids");
            ValidateStableIds(errors, ExperienceFlagIds, "experience_flag_ids");
            ValidateStableIds(errors, ConsumedUniqueContentIds, "consumed_unique_content_ids");
            ValidateStableIds(errors, PendingMetaDiscoveryIds, "pending_meta_discovery_ids");
            ValidateStableIds(errors, Build.TechniqueIds, "technique_ids");
            ValidateStableIds(errors, Build.AcquiredArtifactIds, "acquired_artifact_ids");
            ValidateStableIds(errors, Realm.BreakthroughSourceIds, "breakthrough_source_ids");

            HashSet<string> graphIds = new HashSet<string>(ResolvedGraphNodeSnapshotIds, StringComparer.Ordinal);
            if (graphIds.Count == 0)
            {
                errors.Add("The resolved graph snapshot cannot be empty.");
            }
            else
            {
                RequireGraphMember(errors, graphIds, CurrentNodeId, "current_node_id");
                RequireGraphMembers(errors, graphIds, CompletedNodeIds, "completed_node_ids");
                RequireGraphMembers(errors, graphIds, ReachableNodeIds, "reachable_node_ids");
                if (!string.IsNullOrEmpty(LastCommittedNodeId))
                {
                    RequireGraphMember(errors, graphIds, LastCommittedNodeId, "last_committed_node_id");
                }
            }

            if (MinerSpiritLife.Consumed && !MinerSpiritLife.Granted)
            {
                errors.Add("Miner spirit life cannot be consumed before it is granted.");
            }

            bool hasPendingEncounterId = !string.IsNullOrEmpty(PendingEncounterId);
            if (hasPendingEncounterId != PendingEncounterSeed.HasValue)
            {
                errors.Add("pending_encounter_id and pending_encounter_seed must be written together.");
            }

            if (Build.MindMethodLevel < 0 || Build.InnateArtifactRefinementStage < 0 || Realm.Stage < 0)
            {
                errors.Add("Build and realm progression values cannot be negative.");
            }

            if (MaxHealth <= 0 || CurrentHealth < 0 || CurrentHealth > MaxHealth)
            {
                errors.Add("Run health must remain within zero and max_health.");
            }

            if (Statistics.BattlesWon < 0
                || Statistics.MiniBossesDefeated < 0
                || Statistics.NodesCompleted < 0
                || Statistics.MaxSwordCount < 0
                || Statistics.HighestBurstDamage < 0
                || Statistics.MinerSpiritRetriesUsed < 0
                || Statistics.ElapsedSeconds < 0f
                || float.IsNaN(Statistics.ElapsedSeconds)
                || float.IsInfinity(Statistics.ElapsedSeconds))
            {
                errors.Add("Run statistics cannot contain negative or non-finite values.");
            }

            return errors;
        }

        public DemoRunSaveV2 DeepClone()
        {
            string payload;
            string serializeError;
            DemoRunSaveV2 clone;
            string deserializeError = string.Empty;
            if (!DemoRunSaveCodec.TrySerialize(this, out payload, out serializeError)
                || !DemoRunSaveCodec.TryDeserialize(payload, out clone, out deserializeError))
            {
                throw new InvalidOperationException("Unable to clone run save: " + serializeError + deserializeError);
            }

            return clone;
        }

        private static void RequireValue(ICollection<string> errors, string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(fieldName + " is required.");
            }
        }

        private static void ValidateStableIds(ICollection<string> errors, IEnumerable<string> values, string fieldName)
        {
            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (string value in values)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    errors.Add(fieldName + " contains an empty ID.");
                }
                else if (!seen.Add(value))
                {
                    errors.Add(fieldName + " contains duplicate ID: " + value + ".");
                }
            }
        }

        private static void RequireGraphMember(
            ICollection<string> errors,
            ISet<string> graphIds,
            string nodeId,
            string fieldName)
        {
            if (!string.IsNullOrEmpty(nodeId) && !graphIds.Contains(nodeId))
            {
                errors.Add(fieldName + " references a node outside the resolved graph: " + nodeId + ".");
            }
        }

        private static void RequireGraphMembers(
            ICollection<string> errors,
            ISet<string> graphIds,
            IEnumerable<string> nodeIds,
            string fieldName)
        {
            foreach (string nodeId in nodeIds)
            {
                RequireGraphMember(errors, graphIds, nodeId, fieldName);
            }
        }
    }

    public interface IDemoRunSaveStore
    {
        bool TryWriteCheckpoint(DemoRunSaveV2 checkpoint, out string error);
        bool TryLoadLatest(out DemoRunSaveV2 checkpoint, out string error);
        bool TryLoadPrevious(out DemoRunSaveV2 checkpoint, out string error);
        bool TryLoadLatestOrPrevious(out DemoRunSaveV2 checkpoint, out bool recoveredPrevious, out string error);
        bool ArchiveIncompatible(string reason, out string error);
    }

    public sealed class DemoArchivedRunSave
    {
        public string SlotName { get; }
        public string Reason { get; }
        public string SerializedPayload { get; }

        internal DemoArchivedRunSave(string slotName, string reason, string serializedPayload)
        {
            SlotName = slotName ?? string.Empty;
            Reason = reason ?? string.Empty;
            SerializedPayload = serializedPayload ?? string.Empty;
        }
    }

    public sealed class DemoMemoryRunSaveStore : IDemoRunSaveStore
    {
        private string latestPayload;
        private string previousPayload;
        private readonly List<DemoArchivedRunSave> archived = new List<DemoArchivedRunSave>();

        public IReadOnlyList<DemoArchivedRunSave> Archived => archived;

        public bool TryWriteCheckpoint(DemoRunSaveV2 checkpoint, out string error)
        {
            if (!DemoRunSaveCodec.TrySerialize(checkpoint, out string candidatePayload, out error))
            {
                return false;
            }

            // The candidate must survive a full deserialize and validation pass before slots rotate.
            if (!DemoRunSaveCodec.TryDeserialize(candidatePayload, out DemoRunSaveV2 verified, out error))
            {
                return false;
            }

            if (!string.Equals(verified.RunId, checkpoint.RunId, StringComparison.Ordinal)
                || verified.CheckpointSequence != checkpoint.CheckpointSequence)
            {
                error = "Serialized checkpoint identity changed during verification.";
                return false;
            }

            previousPayload = latestPayload;
            latestPayload = candidatePayload;
            error = string.Empty;
            return true;
        }

        public bool TryLoadLatest(out DemoRunSaveV2 checkpoint, out string error)
        {
            return TryLoadSlot(latestPayload, "latest", out checkpoint, out error);
        }

        public bool TryLoadPrevious(out DemoRunSaveV2 checkpoint, out string error)
        {
            return TryLoadSlot(previousPayload, "previous", out checkpoint, out error);
        }

        public bool TryLoadLatestOrPrevious(out DemoRunSaveV2 checkpoint, out bool recoveredPrevious, out string error)
        {
            if (TryLoadLatest(out checkpoint, out string latestError))
            {
                recoveredPrevious = false;
                error = string.Empty;
                return true;
            }

            if (TryLoadPrevious(out checkpoint, out string previousError))
            {
                recoveredPrevious = true;
                error = latestError;
                return true;
            }

            recoveredPrevious = false;
            error = "Latest failed: " + latestError + " Previous failed: " + previousError;
            return false;
        }

        public bool ArchiveIncompatible(string reason, out string error)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                error = "An archive reason is required.";
                return false;
            }

            if (string.IsNullOrEmpty(latestPayload) && string.IsNullOrEmpty(previousPayload))
            {
                error = "There is no run save to archive.";
                return false;
            }

            if (!string.IsNullOrEmpty(latestPayload))
            {
                archived.Add(new DemoArchivedRunSave("latest", reason, latestPayload));
            }

            if (!string.IsNullOrEmpty(previousPayload))
            {
                archived.Add(new DemoArchivedRunSave("previous", reason, previousPayload));
            }

            latestPayload = null;
            previousPayload = null;
            error = string.Empty;
            return true;
        }

        internal void ReplaceLatestPayloadForTesting(string payload)
        {
            latestPayload = payload;
        }

        private static bool TryLoadSlot(
            string payload,
            string slotName,
            out DemoRunSaveV2 checkpoint,
            out string error)
        {
            if (string.IsNullOrEmpty(payload))
            {
                checkpoint = null;
                error = "The " + slotName + " checkpoint is empty.";
                return false;
            }

            return DemoRunSaveCodec.TryDeserialize(payload, out checkpoint, out error);
        }
    }

    public sealed class DemoFileRunSaveStore : IDemoRunSaveStore
    {
        private static readonly UTF8Encoding Utf8WithoutBom = new UTF8Encoding(false);

        private readonly string saveDirectory;
        private readonly string latestPath;
        private readonly string previousPath;
        private readonly string archiveDirectory;

        public string LatestPath => latestPath;
        public string PreviousPath => previousPath;

        public DemoFileRunSaveStore(string saveDirectory, string slotName = "demo_run_v2")
        {
            if (string.IsNullOrWhiteSpace(saveDirectory))
            {
                throw new ArgumentException("A save directory is required.", nameof(saveDirectory));
            }

            if (string.IsNullOrWhiteSpace(slotName)
                || slotName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                throw new ArgumentException("The save slot name is invalid.", nameof(slotName));
            }

            this.saveDirectory = Path.GetFullPath(saveDirectory);
            latestPath = Path.Combine(this.saveDirectory, slotName + ".latest.json");
            previousPath = Path.Combine(this.saveDirectory, slotName + ".previous.json");
            archiveDirectory = Path.Combine(this.saveDirectory, "Archive");
        }

        public bool TryWriteCheckpoint(DemoRunSaveV2 checkpoint, out string error)
        {
            if (!DemoRunSaveCodec.TrySerialize(checkpoint, out string candidatePayload, out error))
            {
                return false;
            }

            string temporaryPath = latestPath + ".tmp";
            try
            {
                Directory.CreateDirectory(saveDirectory);
                File.WriteAllText(temporaryPath, candidatePayload, Utf8WithoutBom);

                string persistedCandidate = File.ReadAllText(temporaryPath, Encoding.UTF8);
                if (!DemoRunSaveCodec.TryDeserialize(
                        persistedCandidate,
                        out DemoRunSaveV2 verified,
                        out error))
                {
                    return false;
                }

                if (!string.Equals(verified.RunId, checkpoint.RunId, StringComparison.Ordinal)
                    || verified.CheckpointSequence != checkpoint.CheckpointSequence)
                {
                    error = "Serialized checkpoint identity changed during disk verification.";
                    return false;
                }

                if (File.Exists(latestPath))
                {
                    File.Replace(temporaryPath, latestPath, previousPath, true);
                }
                else
                {
                    File.Move(temporaryPath, latestPath);
                }

                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "Run checkpoint disk write failed: " + exception.Message;
                return false;
            }
            finally
            {
                TryDeleteTemporaryFile(temporaryPath);
            }
        }

        public bool TryLoadLatest(out DemoRunSaveV2 checkpoint, out string error)
        {
            return TryLoadFile(latestPath, "latest", out checkpoint, out error);
        }

        public bool TryLoadPrevious(out DemoRunSaveV2 checkpoint, out string error)
        {
            return TryLoadFile(previousPath, "previous", out checkpoint, out error);
        }

        public bool TryLoadLatestOrPrevious(out DemoRunSaveV2 checkpoint, out bool recoveredPrevious, out string error)
        {
            if (TryLoadLatest(out checkpoint, out string latestError))
            {
                recoveredPrevious = false;
                error = string.Empty;
                return true;
            }

            if (TryLoadPrevious(out checkpoint, out string previousError))
            {
                recoveredPrevious = true;
                error = latestError;
                return true;
            }

            recoveredPrevious = false;
            error = "Latest failed: " + latestError + " Previous failed: " + previousError;
            return false;
        }

        public bool ArchiveIncompatible(string reason, out string error)
        {
            if (string.IsNullOrWhiteSpace(reason))
            {
                error = "An archive reason is required.";
                return false;
            }

            List<string> occupiedPaths = new List<string>();
            if (File.Exists(latestPath)) occupiedPaths.Add(latestPath);
            if (File.Exists(previousPath)) occupiedPaths.Add(previousPath);
            if (occupiedPaths.Count == 0)
            {
                error = "There is no run save to archive.";
                return false;
            }

            try
            {
                Directory.CreateDirectory(archiveDirectory);
                string archiveId = DateTime.UtcNow.ToString("yyyyMMddTHHmmssfffffffZ")
                    + "_" + Guid.NewGuid().ToString("N");
                for (int i = 0; i < occupiedPaths.Count; i++)
                {
                    string sourcePath = occupiedPaths[i];
                    string destinationPath = Path.Combine(
                        archiveDirectory,
                        Path.GetFileName(sourcePath) + "." + archiveId + ".archived");
                    File.Copy(sourcePath, destinationPath, false);
                }

                File.WriteAllText(
                    Path.Combine(archiveDirectory, archiveId + ".reason.txt"),
                    reason,
                    Utf8WithoutBom);
                for (int i = 0; i < occupiedPaths.Count; i++)
                {
                    File.Delete(occupiedPaths[i]);
                }

                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "Run checkpoint archive failed: " + exception.Message;
                return false;
            }
        }

        private static bool TryLoadFile(
            string path,
            string slotName,
            out DemoRunSaveV2 checkpoint,
            out string error)
        {
            checkpoint = null;
            if (!File.Exists(path))
            {
                error = "The " + slotName + " checkpoint is empty.";
                return false;
            }

            try
            {
                string payload = File.ReadAllText(path, Encoding.UTF8);
                return DemoRunSaveCodec.TryDeserialize(payload, out checkpoint, out error);
            }
            catch (Exception exception)
            {
                error = "The " + slotName + " checkpoint could not be read: " + exception.Message;
                return false;
            }
        }

        private static void TryDeleteTemporaryFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // A stale candidate is ignored and overwritten on the next save attempt.
            }
        }
    }

    public sealed class DemoNodeOutcomeTransaction
    {
        private readonly DemoRunSaveV2 original;
        private readonly DemoRunSaveV2 staged;
        private bool hasStagedCompletion;

        public DemoRunSaveV2 Staged => staged;
        public bool IsCommitted { get; private set; }

        public DemoNodeOutcomeTransaction(DemoRunSaveV2 stableCheckpoint)
        {
            if (stableCheckpoint == null)
            {
                throw new ArgumentNullException(nameof(stableCheckpoint));
            }

            original = stableCheckpoint;
            staged = stableCheckpoint.DeepClone();
        }

        public bool TryStageCompletion(
            string completedNodeId,
            string nextNodeId,
            string nextFlowPhaseId,
            IEnumerable<string> reachableNodeIds,
            out string error)
        {
            if (IsCommitted)
            {
                error = "A committed node transaction cannot be changed.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(completedNodeId)
                || string.IsNullOrWhiteSpace(nextNodeId)
                || !DemoRunFlowPhaseId.IsValid(nextFlowPhaseId))
            {
                error = "Node completion requires stable completed/next IDs and a valid next phase.";
                return false;
            }

            HashSet<string> graph = new HashSet<string>(staged.ResolvedGraphNodeSnapshotIds, StringComparer.Ordinal);
            if (!graph.Contains(completedNodeId) || !graph.Contains(nextNodeId))
            {
                error = "Node completion references an ID outside the resolved graph.";
                return false;
            }

            AddUnique(staged.CompletedNodeIds, completedNodeId);
            staged.LastCommittedNodeId = completedNodeId;
            staged.CurrentNodeId = nextNodeId;
            staged.FlowPhaseId = nextFlowPhaseId;
            staged.ReachableNodeIds = reachableNodeIds == null
                ? new List<string>()
                : reachableNodeIds.Distinct(StringComparer.Ordinal).ToList();
            staged.PendingEncounterId = string.Empty;
            staged.PendingEncounterSeed = null;
            staged.Statistics.NodesCompleted++;
            hasStagedCompletion = true;
            error = string.Empty;
            return true;
        }

        public void AddExperienceFlag(string experienceFlagId)
        {
            AddUnique(staged.ExperienceFlagIds, experienceFlagId);
        }

        public void AddPendingMetaDiscovery(string discoveryId)
        {
            AddUnique(staged.PendingMetaDiscoveryIds, discoveryId);
        }

        public bool TryCommit(IDemoRunSaveStore store, out DemoRunSaveV2 committed, out string error)
        {
            committed = null;
            if (store == null)
            {
                error = "A run save store is required.";
                return false;
            }

            if (IsCommitted || !hasStagedCompletion)
            {
                error = IsCommitted
                    ? "The node transaction has already committed."
                    : "The node transaction has no staged completion.";
                return false;
            }

            if (!string.Equals(original.RunId, staged.RunId, StringComparison.Ordinal))
            {
                error = "A node transaction cannot change run_id.";
                return false;
            }

            staged.CheckpointSequence = original.CheckpointSequence + 1;
            IReadOnlyList<string> validationErrors = staged.Validate();
            if (validationErrors.Count > 0)
            {
                error = string.Join(" ", validationErrors);
                return false;
            }

            if (!store.TryWriteCheckpoint(staged, out error)
                || !store.TryLoadLatest(out DemoRunSaveV2 verified, out error))
            {
                return false;
            }

            if (!string.Equals(verified.RunId, original.RunId, StringComparison.Ordinal)
                || verified.CheckpointSequence != staged.CheckpointSequence)
            {
                error = "The persisted node checkpoint did not retain its transaction identity.";
                return false;
            }

            IsCommitted = true;
            committed = verified;
            error = string.Empty;
            return true;
        }

        private static void AddUnique(ICollection<string> destination, string value)
        {
            if (destination != null && !string.IsNullOrWhiteSpace(value) && !destination.Contains(value))
            {
                destination.Add(value);
            }
        }
    }

    public static class DemoMinerSpiritLifeRetry
    {
        public static bool TryPersistConsumptionBeforeRetry(
            DemoRunSaveV2 stablePreBattleCheckpoint,
            IDemoRunSaveStore store,
            out DemoRunSaveV2 retryCheckpoint,
            out string error)
        {
            retryCheckpoint = null;
            if (stablePreBattleCheckpoint == null || store == null)
            {
                error = "A stable pre-battle checkpoint and run save store are required.";
                return false;
            }

            stablePreBattleCheckpoint.Normalize();
            if (!stablePreBattleCheckpoint.MinerSpiritLife.Granted
                || stablePreBattleCheckpoint.MinerSpiritLife.Consumed)
            {
                error = "Miner spirit life is unavailable or already consumed.";
                return false;
            }

            if (string.IsNullOrEmpty(stablePreBattleCheckpoint.PendingEncounterId)
                || !stablePreBattleCheckpoint.PendingEncounterSeed.HasValue)
            {
                error = "A deterministic pending encounter is required before consuming miner spirit life.";
                return false;
            }

            string encounterId = stablePreBattleCheckpoint.PendingEncounterId;
            int encounterSeed = stablePreBattleCheckpoint.PendingEncounterSeed.Value;
            DemoRunSaveV2 candidate = stablePreBattleCheckpoint.DeepClone();
            candidate.MinerSpiritLife.Consumed = true;
            candidate.Statistics.MinerSpiritRetriesUsed++;
            candidate.CheckpointSequence = stablePreBattleCheckpoint.CheckpointSequence + 1;
            candidate.FlowPhaseId = DemoRunFlowPhaseId.EncounterIntro;

            if (!store.TryWriteCheckpoint(candidate, out error)
                || !store.TryLoadLatest(out DemoRunSaveV2 persisted, out error))
            {
                return false;
            }

            if (!persisted.MinerSpiritLife.Consumed
                || !string.Equals(persisted.RunId, stablePreBattleCheckpoint.RunId, StringComparison.Ordinal)
                || !string.Equals(persisted.PendingEncounterId, encounterId, StringComparison.Ordinal)
                || persisted.PendingEncounterSeed != encounterSeed)
            {
                error = "Miner spirit life consumption was not durably persisted with the same encounter seed.";
                return false;
            }

            retryCheckpoint = persisted;
            error = string.Empty;
            return true;
        }
    }

    internal static class DemoRunSaveCodec
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(
            typeof(DemoRunSaveV2),
            new DataContractJsonSerializerSettings { UseSimpleDictionaryFormat = true });

        public static bool TrySerialize(DemoRunSaveV2 checkpoint, out string payload, out string error)
        {
            payload = string.Empty;
            if (checkpoint == null)
            {
                error = "Run checkpoint cannot be null.";
                return false;
            }

            IReadOnlyList<string> validationErrors = checkpoint.Validate();
            if (validationErrors.Count > 0)
            {
                error = string.Join(" ", validationErrors);
                return false;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    Serializer.WriteObject(stream, checkpoint);
                    payload = Encoding.UTF8.GetString(stream.ToArray());
                }

                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "Run checkpoint serialization failed: " + exception.Message;
                return false;
            }
        }

        public static bool TryDeserialize(string payload, out DemoRunSaveV2 checkpoint, out string error)
        {
            checkpoint = null;
            if (string.IsNullOrWhiteSpace(payload))
            {
                error = "Serialized run checkpoint is empty.";
                return false;
            }

            try
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(payload)))
                {
                    checkpoint = Serializer.ReadObject(stream) as DemoRunSaveV2;
                }

                if (checkpoint == null)
                {
                    error = "Serialized payload did not contain a run checkpoint.";
                    return false;
                }

                IReadOnlyList<string> validationErrors = checkpoint.Validate();
                if (validationErrors.Count > 0)
                {
                    error = string.Join(" ", validationErrors);
                    checkpoint = null;
                    return false;
                }

                error = string.Empty;
                return true;
            }
            catch (Exception exception)
            {
                error = "Run checkpoint deserialization failed: " + exception.Message;
                return false;
            }
        }
    }
}
