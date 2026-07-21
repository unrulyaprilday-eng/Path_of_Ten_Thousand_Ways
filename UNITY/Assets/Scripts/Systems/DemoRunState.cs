using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    public enum DemoFlowPhase
    {
        Home,
        OpeningTrace,
        OpeningRoot,
        OpeningVessel,
        OpeningRegion,
        OpeningStory,
        RegionChoice,
        JourneyMap,
        NodeScene,
        Breakthrough,
        EncounterIntro,
        Battle,
        BattleOutcome,
        RewardChoice,
        RouteChoice,
        Training,
        Preparation,
        BossGate,
        RunResult
    }

    public sealed class DemoFlowContext
    {
        public string NodeId { get; }
        public string NodeName { get; }
        public DemoNodeType? NodeType { get; }
        public int Layer { get; }
        public string EncounterId { get; }
        public string RouteId { get; }
        public string RouteName { get; }
        public string RouteRisk { get; }
        public bool? BattleVictory { get; }
        public string TraceId { get; }
        public string RootId { get; }
        public string RootName { get; }
        public string VesselId { get; }
        public string VesselName { get; }
        public string RegionId { get; }
        public string RegionName { get; }

        public DemoFlowContext(
            string nodeId = null,
            string nodeName = null,
            DemoNodeType? nodeType = null,
            int layer = 0,
            string encounterId = null,
            string routeId = null,
            string routeName = null,
            string routeRisk = null,
            bool? battleVictory = null,
            string traceId = null,
            string rootId = null,
            string rootName = null,
            string vesselId = null,
            string vesselName = null,
            string regionId = null,
            string regionName = null)
        {
            NodeId = nodeId ?? string.Empty;
            NodeName = nodeName ?? string.Empty;
            NodeType = nodeType;
            Layer = Math.Max(0, layer);
            EncounterId = encounterId ?? string.Empty;
            RouteId = routeId ?? string.Empty;
            RouteName = routeName ?? string.Empty;
            RouteRisk = routeRisk ?? string.Empty;
            BattleVictory = battleVictory;
            TraceId = traceId ?? string.Empty;
            RootId = rootId ?? string.Empty;
            RootName = rootName ?? string.Empty;
            VesselId = vesselId ?? string.Empty;
            VesselName = vesselName ?? string.Empty;
            RegionId = regionId ?? string.Empty;
            RegionName = regionName ?? string.Empty;
        }

        public static DemoFlowContext FromMap(DemoMapRun map, bool? battleVictory = null)
        {
            DemoMapNode node = map?.CurrentNode;
            DemoMapRouteRecord route = map?.CurrentRoute;
            return new DemoFlowContext(
                node?.NodeId,
                node?.Name,
                node?.Type,
                node?.Layer ?? 0,
                node?.EncounterId,
                route?.RouteId,
                route?.Name,
                route?.Risk,
                battleVictory);
        }

        public static DemoFlowContext FromRun(DemoRunState run, bool? battleVictory = null)
        {
            DemoFlowContext mapContext = FromMap(run?.Map, battleVictory);
            DemoRunState.DemoOpeningSelection selection = run?.OpeningSelection;
            return new DemoFlowContext(
                mapContext.NodeId,
                mapContext.NodeName,
                mapContext.NodeType,
                mapContext.Layer,
                mapContext.EncounterId,
                mapContext.RouteId,
                mapContext.RouteName,
                mapContext.RouteRisk,
                mapContext.BattleVictory,
                run?.EquippedTraceId,
                selection?.Root?.Id,
                selection?.Root?.Name,
                selection?.Vessel?.Id,
                selection?.Vessel?.Name,
                selection?.FirstRegion?.Id,
                selection?.FirstRegion?.Name);
        }
    }

    public sealed class DemoFlowSnapshot
    {
        public long Sequence { get; }
        public DemoFlowPhase PreviousPhase { get; }
        public DemoFlowPhase Phase { get; }
        public DemoFlowContext Context { get; }

        internal DemoFlowSnapshot(
            long sequence,
            DemoFlowPhase previousPhase,
            DemoFlowPhase phase,
            DemoFlowContext context)
        {
            Sequence = sequence;
            PreviousPhase = previousPhase;
            Phase = phase;
            Context = context ?? new DemoFlowContext();
        }
    }

    public sealed class DemoFlowState
    {
        private DemoFlowPhase previousPhase;
        private DemoFlowPhase phase;
        private DemoFlowContext context = new DemoFlowContext();
        private long sequence;

        public DemoFlowPhase PreviousPhase => previousPhase;
        public DemoFlowPhase Phase => phase;
        public DemoFlowContext Context => context;
        public long Sequence => sequence;
        public DemoFlowSnapshot Snapshot => Capture();

        public DemoFlowState()
        {
            Reset();
        }

        public void Reset(DemoFlowPhase initialPhase = DemoFlowPhase.Home)
        {
            previousPhase = initialPhase;
            phase = initialPhase;
            context = new DemoFlowContext();
            sequence = 0;
        }

        public DemoFlowSnapshot TransitionTo(DemoFlowPhase nextPhase, DemoFlowContext nextContext = null)
        {
            previousPhase = phase;
            phase = nextPhase;
            context = nextContext ?? new DemoFlowContext();
            sequence++;
            return Capture();
        }

        public DemoFlowSnapshot TransitionToCurrentNode(
            DemoFlowPhase nextPhase,
            DemoMapRun map,
            bool? battleVictory = null)
        {
            return TransitionTo(nextPhase, DemoFlowContext.FromMap(map, battleVictory));
        }

        public DemoFlowSnapshot TransitionToRun(
            DemoFlowPhase nextPhase,
            DemoRunState run,
            bool? battleVictory = null)
        {
            return TransitionTo(nextPhase, DemoFlowContext.FromRun(run, battleVictory));
        }

        public DemoFlowSnapshot Capture()
        {
            return new DemoFlowSnapshot(sequence, previousPhase, phase, context);
        }
    }

    public sealed class DemoRunState
    {
        public sealed class DemoOpeningSelection
        {
            public DemoRootDefinition Root;
            public DemoJourneyVesselDefinition Vessel;
            public DemoStartingPracticePackageDefinition StartingPracticePackage;

            // Legacy mirror kept while the existing controller and UI migrate to Vessel.
            public DemoJourneyLineDefinition JourneyLine;
            public DemoRegionDefinition FirstRegion;
        }

        public DemoMapRun Map { get; } = new DemoMapRun();
        public DemoFlowState Flow { get; } = new DemoFlowState();
        public List<DemoCard> Deck { get; } = new List<DemoCard>();
        public int MaxHealth;
        public int CurrentHealth;
        public int BonusEnergy;
        public int BonusPermanentSwords;
        public readonly List<string> Relics = new List<string>();
        public readonly List<DemoArtifactType> Artifacts = new List<DemoArtifactType>();
        public DemoGongfaType MainGongfa = DemoGongfaType.None;
        public DemoGongfaType SupportGongfa = DemoGongfaType.None;
        public DemoGongfaType DivineGongfa = DemoGongfaType.None;
        public DemoOpeningSelection OpeningSelection { get; } = new DemoOpeningSelection();
        public DemoCorePracticeState CorePractice { get; } = new DemoCorePracticeState();
        public DemoInnateArtifactState InnateArtifact { get; } = new DemoInnateArtifactState();
        public DemoRealmState Realm { get; } = new DemoRealmState();
        public DemoStoryState Story { get; } = new DemoStoryState();
        public List<DemoTechniqueState> Techniques { get; } = new List<DemoTechniqueState>();

        public int BattlesWon;
        public int MaxSwordCount;
        public int HighestBurstDamage;
        public int ConsecutiveRewardsWithoutFocus;
        public int OpeningRewardRerolls;
        public string EquippedTraceId;
        public float ElapsedSeconds { get; private set; }

        public DemoRunState()
        {
            ResetForNewRun();
        }

        public void ResetForNewRun(
            string equippedTraceId = null,
            int openingRewardRerolls = 0)
        {
            Map.Reset();
            Flow.Reset();
            Deck.Clear();
            Relics.Clear();
            Artifacts.Clear();

            MaxHealth = DemoConfigRepository.GetIntConstant("battle", "player_base_max_health", 72);
            CurrentHealth = MaxHealth;
            BonusEnergy = 0;
            BonusPermanentSwords = 0;
            MainGongfa = DemoGongfaType.None;
            SupportGongfa = DemoGongfaType.None;
            DivineGongfa = DemoGongfaType.None;

            OpeningSelection.Root = null;
            OpeningSelection.Vessel = null;
            OpeningSelection.StartingPracticePackage = null;
            OpeningSelection.JourneyLine = null;
            OpeningSelection.FirstRegion = null;

            CorePractice.DefinitionId = string.Empty;
            CorePractice.Level = 1;
            InnateArtifact.DefinitionId = string.Empty;
            InnateArtifact.RefinementStage = 1;
            InnateArtifact.BearerDefinitionId = string.Empty;
            Realm.RealmId = "realm_qi_refining";
            Realm.Stage = 1;
            Realm.FoundationRuleId = string.Empty;
            Realm.BreakthroughSourceIds.Clear();
            Story.Restore(null, null, null);
            Techniques.Clear();

            BattlesWon = 0;
            MaxSwordCount = 0;
            HighestBurstDamage = 0;
            ConsecutiveRewardsWithoutFocus = 0;
            EquippedTraceId = equippedTraceId;
            OpeningRewardRerolls = Math.Max(0, openingRewardRerolls);
            ElapsedSeconds = 0f;

            Deck.AddRange(DemoCardLibrary.CreateStarterDeck());
        }

        public void AddCard(DemoCard card)
        {
            if (card == null)
            {
                return;
            }

            Deck.Add(card.Clone());
        }

        public void SetRoot(DemoRootDefinition root)
        {
            OpeningSelection.Root = root;
            OpeningSelection.Vessel = null;
            OpeningSelection.StartingPracticePackage = null;
            OpeningSelection.JourneyLine = null;
            OpeningSelection.FirstRegion = null;
        }

        public void SetVessel(DemoJourneyVesselDefinition vessel)
        {
            OpeningSelection.Vessel = vessel;
            OpeningSelection.StartingPracticePackage = ResolveStartingPracticePackage(vessel);
            OpeningSelection.JourneyLine = vessel == null ? null : ToLegacyJourneyLine(vessel);
            OpeningSelection.FirstRegion = null;
            LoadVesselStarterDeck(vessel?.StarterPoolId);
            ApplyStartingPracticePackage(OpeningSelection.StartingPracticePackage);
        }

        public void SetJourneyLine(DemoJourneyLineDefinition line)
        {
            OpeningSelection.JourneyLine = line;
            OpeningSelection.FirstRegion = null;

            if (line != null && DemoConfigRepository.TryGetJourneyVessel(line.Id, out DemoJourneyVesselDefinition configured))
            {
                OpeningSelection.Vessel = configured;
                OpeningSelection.StartingPracticePackage = ResolveStartingPracticePackage(configured);
            }
            else
            {
                OpeningSelection.Vessel = line == null ? null : ToJourneyVessel(line);
                OpeningSelection.StartingPracticePackage = ResolveStartingPracticePackage(OpeningSelection.Vessel);
            }

            LoadVesselStarterDeck(line?.StarterPoolId);
            ApplyStartingPracticePackage(OpeningSelection.StartingPracticePackage);
        }

        public bool LearnTechnique(string techniqueId, string sourceNodeId)
        {
            if (string.IsNullOrWhiteSpace(techniqueId))
            {
                return false;
            }

            DemoTechniqueState existing = Techniques.FirstOrDefault(item =>
                string.Equals(item.DefinitionId, techniqueId, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Level++;
                return false;
            }

            if (!DemoConfigRepository.TryGetTechnique(techniqueId, out _)
                || !DemoConfigRepository.TryCreateCard(techniqueId, out DemoCard card))
            {
                return false;
            }

            Techniques.Add(new DemoTechniqueState
            {
                DefinitionId = techniqueId,
                Level = 1,
                SourceNodeId = sourceNodeId ?? string.Empty
            });
            if (Deck.Count < 9 && !Deck.Any(item => string.Equals(item.Id, techniqueId, StringComparison.OrdinalIgnoreCase)))
            {
                Deck.Add(card);
            }
            return true;
        }

        public void RestoreJourneyState(DemoRunSaveV2 snapshot)
        {
            if (snapshot == null)
            {
                return;
            }

            snapshot.Normalize();
            MaxHealth = snapshot.MaxHealth;
            CurrentHealth = snapshot.CurrentHealth;
            BattlesWon = snapshot.Statistics.BattlesWon;
            MaxSwordCount = snapshot.Statistics.MaxSwordCount;
            HighestBurstDamage = snapshot.Statistics.HighestBurstDamage;

            CorePractice.DefinitionId = snapshot.Build.MindMethodId;
            CorePractice.Level = Math.Max(1, snapshot.Build.MindMethodLevel);
            InnateArtifact.DefinitionId = snapshot.Build.InnateArtifactId;
            InnateArtifact.RefinementStage = Math.Max(1, snapshot.Build.InnateArtifactRefinementStage);
            Realm.RealmId = snapshot.Realm.RealmId;
            Realm.Stage = Math.Max(1, snapshot.Realm.Stage);
            Realm.FoundationRuleId = snapshot.Realm.FoundationRuleId;
            Realm.BreakthroughSourceIds.Clear();
            Realm.BreakthroughSourceIds.AddRange(snapshot.Realm.BreakthroughSourceIds);
            Story.Restore(snapshot.ExperienceFlagIds, snapshot.ConsumedUniqueContentIds, snapshot.PendingMetaDiscoveryIds);

            Techniques.Clear();
            Deck.Clear();
            foreach (string techniqueId in snapshot.Build.TechniqueIds)
            {
                if (DemoConfigRepository.TryCreateCard(techniqueId, out DemoCard card))
                {
                    Techniques.Add(new DemoTechniqueState
                    {
                        DefinitionId = techniqueId,
                        Level = 1,
                        SourceNodeId = "restored_checkpoint"
                    });
                    Deck.Add(card);
                }
            }
        }

        private void ApplyStartingPracticePackage(DemoStartingPracticePackageDefinition package)
        {
            Techniques.Clear();
            if (package == null)
            {
                return;
            }

            CorePractice.DefinitionId = package.CorePracticeId ?? string.Empty;
            CorePractice.Level = 1;
            InnateArtifact.DefinitionId = package.InnateArtifactId ?? string.Empty;
            InnateArtifact.RefinementStage = 1;
            InnateArtifact.BearerDefinitionId = package.BearerDefinitionId ?? string.Empty;
            foreach (string techniqueId in package.ActiveTechniqueIds ?? new List<string>())
            {
                Techniques.Add(new DemoTechniqueState
                {
                    DefinitionId = techniqueId,
                    Level = 1,
                    SourceNodeId = package.SourceStoryId ?? string.Empty
                });
            }
            Story.AddExperience(package.SourceStoryId);
        }

        private static DemoStartingPracticePackageDefinition ResolveStartingPracticePackage(DemoJourneyVesselDefinition vessel)
        {
            if (vessel == null || string.IsNullOrEmpty(vessel.StartingPracticePackageId))
            {
                return null;
            }

            return DemoConfigRepository.TryGetStartingPracticePackage(
                vessel.StartingPracticePackageId,
                out DemoStartingPracticePackageDefinition package)
                ? package
                : null;
        }

        public bool TrySetFirstRegion(DemoRegionDefinition region)
        {
            if (region == null || !region.IsAvailable)
            {
                return false;
            }

            OpeningSelection.FirstRegion = region;
            return true;
        }

        public void SetFirstRegion(DemoRegionDefinition region)
        {
            TrySetFirstRegion(region);
        }

        public void AddRelic(string relicName)
        {
            if (!HasRelic(relicName))
            {
                Relics.Add(relicName);
            }

            if (relicName == "剑骨")
            {
                BonusPermanentSwords += 1;
            }
        }

        public bool HasRelic(string relicName)
        {
            return Relics.Contains(relicName);
        }

        public bool HasArtifact(DemoArtifactType type)
        {
            return Artifacts.Contains(type);
        }

        public void AddArtifact(DemoArtifactType type)
        {
            if (!Artifacts.Contains(type))
            {
                Artifacts.Add(type);
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + Math.Max(0, amount));
        }

        public void UpgradeEnergy()
        {
            BonusEnergy += 1;
        }

        public bool HasGongfa(DemoGongfaType type)
        {
            return MainGongfa == type || SupportGongfa == type || DivineGongfa == type;
        }

        public void LearnGongfa(DemoGongfaType type)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(type);
            switch (definition.Slot)
            {
                case DemoGongfaSlot.Main:
                    MainGongfa = type;
                    break;
                case DemoGongfaSlot.Support:
                    SupportGongfa = type;
                    break;
                case DemoGongfaSlot.Divine:
                    DivineGongfa = type;
                    break;
            }
        }

        public DemoSwordStyle GetBuildStyle()
        {
            if (MainGongfa != DemoGongfaType.None)
            {
                return DemoGongfaLibrary.Get(MainGongfa).Style;
            }

            string baseStyle = OpeningSelection.Vessel?.BaseStyle
                ?? OpeningSelection.JourneyLine?.BaseStyle;
            if (Enum.TryParse(baseStyle, true, out DemoSwordStyle vesselStyle))
            {
                return vesselStyle;
            }

            int wanjian = Deck.Count(card => card.Style == DemoSwordStyle.Wanjian);
            int thunder = Deck.Count(card => card.Style == DemoSwordStyle.Thunder);
            int blood = Deck.Count(card => card.Style == DemoSwordStyle.Blood);

            if (wanjian >= thunder && wanjian >= blood)
            {
                return DemoSwordStyle.Wanjian;
            }

            if (thunder >= blood)
            {
                return DemoSwordStyle.Thunder;
            }

            return DemoSwordStyle.Blood;
        }

        public bool HasBuildComponent(string componentId)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                return false;
            }

            if (Deck.Any(card => string.Equals(card.Id, componentId, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            switch (componentId)
            {
                case "gongfa_sword_control_art":
                    return HasGongfa(DemoGongfaType.SwordControlArt);
                case "gongfa_wanjian_return":
                    return HasGongfa(DemoGongfaType.WanjianReturn);
                case "artifact_sword_box":
                    return HasArtifact(DemoArtifactType.SwordBox);
                default:
                    return false;
            }
        }

        public HashSet<string> GetBuildComponentIds()
        {
            HashSet<string> ids = new HashSet<string>(
                Deck.Where(card => card != null).Select(card => card.Id),
                StringComparer.OrdinalIgnoreCase);

            if (HasGongfa(DemoGongfaType.SwordControlArt))
            {
                ids.Add("gongfa_sword_control_art");
            }

            if (HasGongfa(DemoGongfaType.WanjianReturn))
            {
                ids.Add("gongfa_wanjian_return");
            }

            if (HasArtifact(DemoArtifactType.SwordBox))
            {
                ids.Add("artifact_sword_box");
            }

            return ids;
        }

        public void RecordBattleVictory(int swordCount, int burstDamage)
        {
            BattlesWon++;
            RecordSwordCount(swordCount);
            RecordBurstDamage(burstDamage);
        }

        public void RecordSwordCount(int swordCount)
        {
            MaxSwordCount = Math.Max(MaxSwordCount, swordCount);
        }

        public void RecordBurstDamage(int damage)
        {
            HighestBurstDamage = Math.Max(HighestBurstDamage, damage);
        }

        public void RecordRewardSelection(bool selectedFocusComponent)
        {
            ConsecutiveRewardsWithoutFocus = selectedFocusComponent
                ? 0
                : ConsecutiveRewardsWithoutFocus + 1;
        }

        public void AdvanceElapsedTime(float seconds)
        {
            if (seconds > 0f && !float.IsNaN(seconds) && !float.IsInfinity(seconds))
            {
                ElapsedSeconds += seconds;
            }
        }

        public DemoRunSummary CreateSummary(bool victory, bool defeatedBoss, int reachedLayer)
        {
            DemoRunSummary summary = new DemoRunSummary
            {
                Victory = victory,
                DefeatedBoss = defeatedBoss,
                ReachedLayer = Math.Max(0, reachedLayer),
                BattlesWon = BattlesWon,
                MaxSwordCount = MaxSwordCount,
                HighestBurstDamage = HighestBurstDamage,
                DurationSeconds = ElapsedSeconds,
                MainGongfaId = GetGongfaId(MainGongfa),
                MainGongfaName = MainGongfa == DemoGongfaType.None
                    ? "未定主修"
                    : DemoGongfaLibrary.Get(MainGongfa).Name,
                CoreArtifactId = Artifacts.Count == 0 ? string.Empty : GetArtifactId(Artifacts[0]),
                CoreArtifactName = Artifacts.Count == 0
                    ? "未获核心法器"
                    : DemoArtifactLibrary.Get(Artifacts[0]).Name
            };

            DemoMapNodeRecord failedNode = Map.FailedNode;
            if (failedNode != null)
            {
                summary.FailureNodeId = failedNode.NodeId;
                summary.FailureNodeName = failedNode.Name;
                summary.FailureNodeType = failedNode.Type.ToString();
            }

            foreach (DemoMapRouteRecord route in Map.SelectedRoutes)
            {
                DemoRunRouteSummary routeSummary = new DemoRunRouteSummary
                {
                    RouteId = route.RouteId,
                    RouteName = route.Name,
                    Layer = route.Layer,
                    Risk = route.Risk
                };

                foreach (DemoMapNodeRecord node in route.Nodes)
                {
                    routeSummary.NodeSequence.Add(new DemoRunNodeSummary
                    {
                        NodeId = node.NodeId,
                        NodeName = node.Name,
                        NodeType = node.Type.ToString(),
                        Layer = node.Layer,
                        Completed = node.IsCompleted,
                        Succeeded = node.Succeeded
                    });
                }

                summary.RouteHistory.Add(routeSummary);
            }

            foreach (DemoMapNodeRecord node in Map.CompletedNodes)
            {
                summary.CompletedNodeHistory.Add(new DemoRunNodeSummary
                {
                    NodeId = node.NodeId,
                    NodeName = node.Name,
                    NodeType = node.Type.ToString(),
                    Layer = node.Layer,
                    Completed = node.IsCompleted,
                    Succeeded = node.Succeeded
                });
            }

            foreach (string componentId in GetBuildComponentIds())
            {
                string displayName = GetBuildComponentDisplayName(componentId);
                summary.CoreComponents.Add(componentId);
                summary.CoreComponentDetails.Add(new DemoRunComponentSummary
                {
                    Id = componentId,
                    DisplayName = displayName
                });
            }

            summary.Normalize();
            return summary;
        }

        public bool ConsumeOpeningRewardReroll()
        {
            if (OpeningRewardRerolls <= 0)
            {
                return false;
            }

            OpeningRewardRerolls--;
            return true;
        }

        public IEnumerable<DemoGongfaType> GetLearnedGongfas()
        {
            if (MainGongfa != DemoGongfaType.None)
            {
                yield return MainGongfa;
            }

            if (SupportGongfa != DemoGongfaType.None)
            {
                yield return SupportGongfa;
            }

            if (DivineGongfa != DemoGongfaType.None)
            {
                yield return DivineGongfa;
            }
        }

        private void LoadVesselStarterDeck(string starterPoolId)
        {
            Deck.Clear();
            if (!string.IsNullOrEmpty(starterPoolId)
                && DemoConfigRepository.TryCreateDeckFromPool(starterPoolId, out List<DemoCard> configuredDeck))
            {
                Deck.AddRange(configuredDeck);
                return;
            }

            Deck.AddRange(DemoCardLibrary.CreateStarterDeck());
        }

        private string GetBuildComponentDisplayName(string componentId)
        {
            DemoCard card = Deck.FirstOrDefault(candidate => candidate != null
                && string.Equals(candidate.Id, componentId, StringComparison.OrdinalIgnoreCase));
            if (card != null)
            {
                return card.Name;
            }

            switch (componentId)
            {
                case "gongfa_sword_control_art":
                    return DemoGongfaLibrary.Get(DemoGongfaType.SwordControlArt).Name;
                case "gongfa_wanjian_return":
                    return DemoGongfaLibrary.Get(DemoGongfaType.WanjianReturn).Name;
                case "artifact_sword_box":
                    return DemoArtifactLibrary.Get(DemoArtifactType.SwordBox).Name;
                default:
                    return componentId ?? string.Empty;
            }
        }

        private static string GetGongfaId(DemoGongfaType type)
        {
            switch (type)
            {
                case DemoGongfaType.SwordControlArt:
                    return "gongfa_sword_control_art";
                case DemoGongfaType.ThunderScripture:
                    return "gongfa_thunder_scripture";
                case DemoGongfaType.BloodFiendCanon:
                    return "gongfa_blood_fiend_canon";
                case DemoGongfaType.SwordHeartResonance:
                    return "gongfa_sword_heart_resonance";
                case DemoGongfaType.LightningMeridians:
                    return "gongfa_lightning_meridians";
                case DemoGongfaType.BloodRefiningBody:
                    return "gongfa_blood_refining_body";
                case DemoGongfaType.WanjianReturn:
                    return "gongfa_wanjian_return";
                case DemoGongfaType.HeavenlyThunderEdict:
                    return "gongfa_heavenly_thunder_edict";
                case DemoGongfaType.BloodPrisonExecution:
                    return "gongfa_blood_prison_execution";
                default:
                    return string.Empty;
            }
        }

        private static string GetArtifactId(DemoArtifactType type)
        {
            switch (type)
            {
                case DemoArtifactType.SwordBox:
                    return "artifact_sword_box";
                case DemoArtifactType.HaotianMirror:
                    return "artifact_haotian_mirror";
                case DemoArtifactType.PurpleGourd:
                    return "artifact_purple_gourd";
                case DemoArtifactType.ThunderSeal:
                    return "artifact_thunder_seal";
                default:
                    return type.ToString();
            }
        }

        private static DemoJourneyVesselDefinition ToJourneyVessel(DemoJourneyLineDefinition line)
        {
            return new DemoJourneyVesselDefinition
            {
                Id = line.Id,
                RootId = line.RootId,
                Name = string.IsNullOrEmpty(line.CarryItemName) ? line.Title : line.CarryItemName,
                OriginText = line.OriginText,
                VesselType = line.VesselType,
                StarterPoolId = line.StarterPoolId,
                StartingPracticePackageId = line.StartingPracticePackageId,
                BaseStyle = line.BaseStyle,
                StartingEffectText = line.CarryItemEffect,
                FirstRegionId = line.FirstRegionId,
                RegionCandidateIds = line.RegionCandidateIds != null ? new List<string>(line.RegionCandidateIds) : new List<string>(),
                RiskLevel = line.RiskLevel,
                SummaryTags = line.SummaryTags != null ? new List<string>(line.SummaryTags) : new List<string>(),
                IsAvailable = line.IsAvailable
            };
        }

        private static DemoJourneyLineDefinition ToLegacyJourneyLine(DemoJourneyVesselDefinition vessel)
        {
            return new DemoJourneyLineDefinition
            {
                Id = vessel.Id,
                RootId = vessel.RootId,
                Title = vessel.Name,
                OriginText = vessel.OriginText,
                CarryItemName = vessel.Name,
                CarryItemEffect = vessel.StartingEffectText,
                VesselType = vessel.VesselType,
                StarterPoolId = vessel.StarterPoolId,
                StartingPracticePackageId = vessel.StartingPracticePackageId,
                BaseStyle = vessel.BaseStyle,
                IsAvailable = vessel.IsAvailable,
                FirstRegionId = vessel.FirstRegionId,
                RegionCandidateIds = vessel.RegionCandidateIds != null ? new List<string>(vessel.RegionCandidateIds) : new List<string>(),
                RiskLevel = vessel.RiskLevel,
                SummaryTags = vessel.SummaryTags != null ? new List<string>(vessel.SummaryTags) : new List<string>()
            };
        }
    }
}
