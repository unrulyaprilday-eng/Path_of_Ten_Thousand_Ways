using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.CompileCheck
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                VerifyOpeningVesselFlow();
                VerifyStartingPracticePackageContract();
                VerifyJourneyContentConfig();
                VerifyBattleLayoutContract();
                VerifyDeterministicJourneyGraph();
                VerifyRunSaveContract();
                VerifyJourneyRunSession();
                VerifyMultiTargetCombatContracts();
                VerifyRealtimeBattleLoop();
                VerifyRewardProgression();
                VerifyThreeLayerRoutesAndIds();
                VerifyConfiguredCombatPacing();
                VerifyMetaProgression();
                Console.WriteLine("P0 realtime vertical slice smoke check passed.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception);
                return 1;
            }
        }

        private static void VerifyStartingPracticePackageContract()
        {
            DemoStartingPracticePackageDefinition sword = DemoStartingPracticePackageDefinition.SwordDemo();
            DemoCorePracticeDefinition breathing = new DemoCorePracticeDefinition
            {
                Id = "practice_branch_breathing",
                Name = "旁支吐纳法",
                PracticeType = DemoCorePracticeType.MindMethod,
                GrantedTechniqueId = "technique_breathing_recovery",
                SourceStoryId = "opening_story_branch_ancestral_vault"
            };
            DemoTechniqueDefinition heartFormula = new DemoTechniqueDefinition
            {
                Id = "technique_breathing_recovery",
                Name = "吐纳归息",
                Kind = DemoTechniqueKind.HeartFormula,
                SourceStoryId = breathing.SourceStoryId
            };
            DemoTechniqueDefinition swordArt = new DemoTechniqueDefinition
            {
                Id = "technique_incomplete_sword_scroll",
                Name = "残卷御剑式",
                Kind = DemoTechniqueKind.SwordArt,
                SourceStoryId = sword.SourceStoryId
            };
            DemoBearerDefinition ground = new DemoBearerDefinition
            {
                Id = "bearer_old_mine_ground",
                Name = "旧矿断台",
                Mode = DemoBearerMode.Ground,
                IsRequired = true
            };
            Dictionary<string, DemoTechniqueDefinition> techniques = new Dictionary<string, DemoTechniqueDefinition>
            {
                [heartFormula.Id] = heartFormula,
                [swordArt.Id] = swordArt
            };

            Require(sword.Validate(breathing, techniques, ground).Count == 0, "Sword opening package must validate.");
            Require(
                DemoConfigRepository.TryGetStartingPracticePackage(sword.Id, out DemoStartingPracticePackageDefinition configuredSword),
                "Configured sword opening package must load from game_config.json.");
            Require(
                DemoConfigRepository.TryGetCorePractice(configuredSword.CorePracticeId, out DemoCorePracticeDefinition configuredPractice),
                "Configured sword opening package must resolve its core practice.");
            Require(
                DemoConfigRepository.TryGetBearer(configuredSword.BearerDefinitionId, out DemoBearerDefinition configuredBearer),
                "Configured sword opening package must resolve its bearer.");
            Dictionary<string, DemoTechniqueDefinition> configuredTechniques = new Dictionary<string, DemoTechniqueDefinition>();
            foreach (string techniqueId in configuredSword.ActiveTechniqueIds)
            {
                Require(
                    DemoConfigRepository.TryGetTechnique(techniqueId, out DemoTechniqueDefinition configuredTechnique),
                    "Configured sword opening package has an unknown technique: " + techniqueId);
                configuredTechniques[techniqueId] = configuredTechnique;
            }
            Require(
                configuredSword.Validate(configuredPractice, configuredTechniques, configuredBearer).Count == 0,
                "Configured sword opening package must pass cross-reference validation.");

            DemoStartingPracticePackageDefinition bodyPackage = new DemoStartingPracticePackageDefinition
            {
                Id = "package_body_fist_demo",
                RootId = "root_body",
                SourceStoryId = "opening_story_body_training",
                CorePracticeId = "practice_body_tempering",
                PrimaryTechniqueId = "technique_beginner_fist",
                BearerDefinitionId = "bearer_old_mine_ground",
                IsAvailable = true,
                ActiveTechniqueIds = new List<string> { "technique_body_breath", "technique_beginner_fist" }
            };
            DemoCorePracticeDefinition bodyPractice = new DemoCorePracticeDefinition
            {
                Id = "practice_body_tempering",
                Name = "炼体术",
                PracticeType = DemoCorePracticeType.BodyCultivation,
                GrantedTechniqueId = "technique_body_breath",
                SourceStoryId = bodyPackage.SourceStoryId
            };
            techniques["technique_body_breath"] = new DemoTechniqueDefinition
            {
                Id = "technique_body_breath",
                Name = "淬体换息",
                Kind = DemoTechniqueKind.BodyArt,
                SourceStoryId = bodyPackage.SourceStoryId
            };
            techniques["technique_beginner_fist"] = new DemoTechniqueDefinition
            {
                Id = "technique_beginner_fist",
                Name = "初阶拳法",
                Kind = DemoTechniqueKind.FistArt,
                SourceStoryId = bodyPackage.SourceStoryId
            };

            Require(bodyPackage.Validate(bodyPractice, techniques, ground).Count == 0, "Body and fist opening package must validate.");
        }

        private static void VerifyBattleLayoutContract()
        {
            Require(DemoBattleLayoutContract.IsStable(), "Battle layout contract must keep stage anchors separated.");
            Require(
                DemoBattleLayoutContract.PlayerSafeArea.Contains(DemoBattleLayoutContract.PlayerAnchor(false)),
                "Player anchor must remain inside the player safe area.");
            Require(
                DemoBattleLayoutContract.EnemySafeArea.Contains(DemoBattleLayoutContract.EnemyAnchor(DemoBattleVisualTier.FinalBoss)),
                "Final boss anchor must remain inside the enemy safe area.");
            Require(
                DemoBattleLayoutContract.CommandSurface.MaxY < DemoBattleLayoutContract.EnergyGrowthTrack.MinY,
                "Energy growth must sit above the command surface.");
        }

        private static void VerifyJourneyContentConfig()
        {
            Require(
                DemoConfigRepository.TryGetInnateArtifact(
                    "artifact_broken_sword_embryo",
                    out DemoInnateArtifactConfig innateArtifact),
                "The configured innate sword embryo must load.");
            Require(innateArtifact.Stages.Count >= 3 && innateArtifact.BaseCooldown > 0f,
                "The innate sword embryo must have staged cooldown-driven growth.");
            Require(
                DemoConfigRepository.TryGetMindMethod(
                    "practice_branch_breathing",
                    out DemoMindMethodConfig mindMethod),
                "The opening mind method must load.");
            Require(mindMethod.Levels.Count >= 3
                && mindMethod.GrantedTechniqueId == "technique_breathing_recovery",
                "The opening mind method must carry its recovery technique and level rules.");
            Require(
                DemoConfigRepository.TryGetRealmBreakthrough(
                    "breakthrough_qi_to_foundation",
                    out DemoRealmBreakthroughConfig breakthrough),
                "The story breakthrough must load.");
            Require(breakthrough.TechniquePowerMultiplier > 1f
                && breakthrough.EnergyRegenMultiplier > 1f,
                "Foundation establishment must improve both technique power and energy flow.");

            string[] foundationRules =
            {
                "foundation_stable",
                "foundation_sword_bone",
                "foundation_clear_spirit",
                "foundation_thunder_meridian",
                "foundation_baleful_contract"
            };
            foreach (string ruleId in foundationRules)
            {
                Require(DemoConfigRepository.TryGetFoundationRule(ruleId, out _),
                    "Missing foundation rule: " + ruleId);
            }

            string[] encounterGroups =
            {
                "encounter_act1_patrol",
                "encounter_act1_elite",
                "encounter_act1_miniboss",
                "encounter_act2_wraiths",
                "encounter_act2_elite",
                "encounter_act2_miniboss",
                "encounter_act3_furnace",
                "encounter_act3_elite",
                "encounter_act3_boss"
            };
            foreach (string encounterId in encounterGroups)
            {
                Require(
                    DemoConfigRepository.TryGetEncounterGroup(
                        encounterId,
                        out DemoEncounterGroupConfig group)
                    && group.Members.Count >= 2
                    && group.Members.Count <= 3,
                    "Encounter group must contain 2-3 configured targets: " + encounterId);
            }

            Require(
                DemoConfigRepository.TryGetMapTemplate(
                    "map_old_mine_three_act",
                    out DemoMapTemplateConfig mapTemplate)
                && mapTemplate.ActCount == 3
                && mapTemplate.StandardDepthCount == 8,
                "The old mine map template must define three eight-depth acts.");
            Require(
                DemoConfigRepository.TryGetEvent(
                    "event_miner_spirit_first",
                    out DemoEventConfig minerSpirit)
                && minerSpirit.Choices.Count == 4,
                "The first miner spirit event must expose four concrete fates.");
            Require(
                DemoConfigRepository.TryGetStoryFlag(
                    "experience_miner_spirit_helped",
                    out _),
                "The miner spirit outcome must resolve to a configured story flag.");
        }

        private static void VerifyMultiTargetCombatContracts()
        {
            DemoCombatTarget first = new DemoCombatTarget(
                "enemy_a",
                "old_mine_beast",
                "front",
                0,
                true,
                true,
                2,
                20);
            first.Intent = new DemoIntentState
            {
                BehaviorId = "heavy_bite",
                RemainingSeconds = 0.8f,
                IsPending = true,
                IsKnown = true,
                ThreatPriority = 3
            };

            DemoCombatTarget second = new DemoCombatTarget(
                "enemy_b",
                "mine_wraith",
                "middle",
                1,
                true,
                false,
                9,
                20);
            second.Intent = new DemoIntentState
            {
                BehaviorId = "curse",
                RemainingSeconds = 0.3f,
                IsPending = true,
                IsKnown = true,
                ThreatPriority = 1
            };

            DemoCombatTarget third = new DemoCombatTarget(
                "enemy_c",
                "mine_wraith",
                "rear",
                2,
                true,
                false,
                1,
                20);
            third.Intent = new DemoIntentState
            {
                BehaviorId = "guard",
                RemainingSeconds = 2f,
                IsPending = false,
                IsKnown = true,
                ThreatPriority = 0
            };

            DemoTargetResolver resolver = new DemoTargetResolver(new[] { third, first, second });
            DemoTargetResolver oneTargetResolver = new DemoTargetResolver(new[] { first });
            DemoTargetResolver twoTargetResolver = new DemoTargetResolver(new[] { second, first });
            Require(oneTargetResolver.QueryTargets().Count == 1, "Target resolver must support one-target encounters.");
            Require(twoTargetResolver.QueryTargets().Count == 2, "Target resolver must support two-target encounters.");
            Require(resolver.QueryTargets().Count == 3, "Target resolver must support three-target encounters.");
            Require(
                resolver.QueryTargets().Select(target => target.CombatantId).SequenceEqual(new[] { "enemy_a", "enemy_b", "enemy_c" }),
                "Target queries must use stable battlefield order independent of insertion order.");
            Require(resolver.ResolveAutoTarget().CombatantId == "enemy_b", "Auto target must prioritize the soonest pending intent.");
            first.Intent.RemainingSeconds = 0.1f;
            Require(resolver.ResolveAutoTarget().CombatantId == "enemy_a", "Automatic target must re-evaluate changing intent urgency.");
            first.Intent.RemainingSeconds = 0.8f;

            Require(resolver.LockTarget("enemy_c"), "A live lockable target must be accepted immediately.");
            Require(resolver.LockedTarget.CombatantId == "enemy_c", "Manual lock must override automatic threat ordering.");
            third.MarkDead();
            Require(resolver.LockedTarget.CombatantId == "enemy_b", "A dead locked target must transfer automatically.");
            Require(!resolver.LockTarget("missing"), "Unknown target lock must be rejected.");
            Require(resolver.LockedTarget.CombatantId == "enemy_b", "Rejected lock must retain a valid automatic target.");

            IReadOnlyList<DemoDamageRequest> area = resolver.CreateAreaDamageRequests(
                "player",
                4,
                DemoDamageType.Sword,
                "wanjian_test");
            Require(area.Count == 2, "AOE must create one request for each live lockable target.");
            Require(area.All(request => request.IsAreaEffect && request.TargetCombatantId != ""), "AOE requests must remain single-target and addressable.");
            DemoDamageResult damage = DemoDamageResult.Apply(area[0], resolver.QueryTargets().First());
            Require(damage.HealthDamage == 4 && !damage.WasKilled, "Single-target damage result must report applied health damage.");

            DemoChainContext chain = new DemoChainContext();
            Require(chain.TryVisit("enemy_b"), "Chain must visit its first target.");
            Require(!chain.TryVisit("enemy_b"), "Chain must reject a repeated target and prevent a回跳.");
            Require(chain.TryVisit("enemy_a"), "Chain must allow a new target.");
            Require(chain.VisitedCombatantIds.SequenceEqual(new[] { "enemy_a", "enemy_b" }), "Chain visited IDs must be deterministic.");

            DemoCard execution = new DemoCard
            {
                Id = "technique_target_transfer_test",
                Name = "定点飞剑",
                Type = DemoCardType.Attack,
                Style = DemoSwordStyle.General,
                Cost = 0,
                Damage = 6
            };
            DemoBattleState battle = new DemoBattleState();
            battle.StartBattle(new DemoBattleSetup
            {
                Deck = new[] { execution, execution, execution },
                Enemies = new[]
                {
                    new DemoBattleEnemySetup { CombatantId = "runtime_a", DefinitionId = "mine_beast", PositionId = "front", Depth = 0, MaxHealth = 5 },
                    new DemoBattleEnemySetup { CombatantId = "runtime_b", DefinitionId = "mine_wraith", PositionId = "middle", Depth = 1, MaxHealth = 5 },
                    new DemoBattleEnemySetup { CombatantId = "runtime_c", DefinitionId = "mine_wraith", PositionId = "rear", Depth = 2, MaxHealth = 5 }
                },
                InitialHandSize = 3,
                HandLimit = 3,
                InitialEnergy = 3,
                IntroSeconds = 0f,
                RandomSeed = 19
            });
            Require(battle.Enemies.Count == 3 && battle.ActiveEnemyCount == 3, "Battle runtime must initialize 1-3 stable enemy targets.");
            Require(battle.LockTarget("runtime_b"), "Battle runtime must accept an immediate manual lock.");
            Require(battle.TryPlayCard(0), "The locked target test card must play.");
            Require(battle.ActiveEnemyCount == 2 && battle.LockedTargetId != "runtime_b", "A killed locked target must transfer without ending a multi-target battle.");
            Require(battle.LockTarget("runtime_a") && battle.TryPlayCard(0), "The second required target must remain playable.");
            Require(battle.Phase == DemoBattlePhase.Running, "Battle must continue while one required target remains.");
            Require(battle.LockTarget("runtime_c") && battle.TryPlayCard(0), "The final required target must remain playable.");
            Require(battle.Phase == DemoBattlePhase.Won, "Battle must end only after every required target is defeated.");

            DemoBattleState independentIntents = new DemoBattleState();
            independentIntents.StartBattle(new DemoBattleSetup
            {
                Enemies = new[]
                {
                    new DemoBattleEnemySetup { CombatantId = "intent_a", DefinitionId = "a", PositionId = "front", Depth = 0, MaxHealth = 999 },
                    new DemoBattleEnemySetup { CombatantId = "intent_b", DefinitionId = "b", PositionId = "middle", Depth = 1, MaxHealth = 999 },
                    new DemoBattleEnemySetup { CombatantId = "intent_c", DefinitionId = "c", PositionId = "rear", Depth = 2, MaxHealth = 999 }
                },
                Deck = Array.Empty<DemoCard>(),
                PlayerMaxHealth = 100,
                PlayerHealth = 100,
                IntroSeconds = 0f,
                DrawIntervalSeconds = 99f,
                FlyingSwordIntervalSeconds = 99f,
                EnemyIntentMinSeconds = 10f,
                EnemyIntentMaxSeconds = 10f,
                RandomSeed = 23
            });
            independentIntents.ClearPresentationSteps();
            foreach (DemoCombatTarget target in independentIntents.Enemies)
            {
                target.Intent.RemainingSeconds = 0.1f;
                target.Intent.DurationSeconds = 0.1f;
            }
            independentIntents.Tick(0.2f);
            Require(independentIntents.EnemyActionCount == 3,
                "Every ready enemy must resolve its own intent in the same realtime tick.");
            HashSet<string> intentSources = new HashSet<string>(
                independentIntents.ConsumePresentationSteps()
                    .Where(step => step.Type == DemoBattlePresentationStepType.EnemyAttack)
                    .Select(step => step.SourceCombatantId),
                StringComparer.Ordinal);
            Require(intentSources.SetEquals(new[] { "intent_a", "intent_b", "intent_c" }),
                "Enemy presentations must preserve the independent source combatant ID.");

            DemoCard breakPart = new DemoCard
            {
                Id = "break_boss_part",
                Name = "断契试剑",
                Type = DemoCardType.Attack,
                Style = DemoSwordStyle.General,
                Cost = 0,
                Damage = 10
            };
            DemoBattleState swordPuppet = new DemoBattleState();
            swordPuppet.StartBattle(new DemoBattleSetup
            {
                EnemyId = "enemy_xuantie_mine_sword_puppet",
                IsBoss = true,
                Enemies = new[]
                {
                    new DemoBattleEnemySetup { CombatantId = "puppet_armor", DefinitionId = "target_xuantie_armor", PositionId = "boss_upper_armor", Depth = 0, MaxHealth = 5 },
                    new DemoBattleEnemySetup { CombatantId = "puppet_spike", DefinitionId = "target_contract_spike", PositionId = "boss_contract_spike", Depth = 1, MaxHealth = 5 },
                    new DemoBattleEnemySetup { CombatantId = "puppet_core", DefinitionId = "target_sword_furnace_core", PositionId = "boss_furnace_core", Depth = 2, MaxHealth = 5 }
                },
                Deck = new[] { breakPart, breakPart, breakPart },
                InitialHandSize = 3,
                HandLimit = 3,
                InitialEnergy = 3,
                IntroSeconds = 0f,
                RandomSeed = 29
            });
            Require(swordPuppet.ActiveEnemyCount == 1
                && swordPuppet.BossPhaseId == DemoBattleState.BossPhaseXuantieArmor,
                "The sword puppet must open with only its armor target exposed.");
            Require(swordPuppet.TryPlayCard(0)
                && swordPuppet.BossPhaseId == DemoBattleState.BossPhaseXuantieContractSpike
                && swordPuppet.LockedTargetId == "puppet_spike",
                "Breaking armor must expose and retarget the cinnabar contract spike.");
            Require(swordPuppet.TryPlayCard(0)
                && swordPuppet.BossPhaseId == DemoBattleState.BossPhaseXuantieCore
                && swordPuppet.LockedTargetId == "puppet_core",
                "Breaking the contract spike must expose and retarget the sword furnace core.");
            Require(swordPuppet.TryPlayCard(0) && swordPuppet.Phase == DemoBattlePhase.Won,
                "Breaking the exposed sword furnace core must complete the final Boss battle.");
        }

        private static void VerifyDeterministicJourneyGraph()
        {
            for (int seed = 0; seed < 100; seed++)
            {
                DemoJourneyGraph graph = DemoJourneyGraphGenerator.Generate(seed);
                DemoJourneyGraph repeated = DemoJourneyGraphGenerator.Generate(seed);
                Require(graph.Validate(out IReadOnlyList<string> errors),
                    $"Journey seed {seed} is invalid: {string.Join(" | ", errors)}");
                Require(JourneyFingerprint(graph) == JourneyFingerprint(repeated),
                    $"Journey seed {seed} is not deterministic.");
                Require(graph.Nodes.Count == graph.ReachableNodeIds.Count,
                    $"Journey seed {seed} contains unreachable nodes.");
                Require(graph.Nodes.Count(node => node.Type == DemoJourneyNodeType.Start) == 1,
                    $"Journey seed {seed} must contain one Start node.");
                Require(graph.Nodes.Count(node => node.Type == DemoJourneyNodeType.MiniBoss) == 2,
                    $"Journey seed {seed} must contain two MiniBoss nodes.");
                Require(graph.Nodes.Count(node => node.Type == DemoJourneyNodeType.Boss) == 1,
                    $"Journey seed {seed} must contain one final Boss node.");
                Require(graph.Nodes.Single(node => node.ActIndex == 1 && node.DepthIndex == 2).ContentId
                        == "event_miner_spirit_first",
                    $"Journey seed {seed} must place the first miner spirit event in act one.");
                Require(graph.Nodes.Single(node => node.ActIndex == 2 && node.DepthIndex == 2).ContentId
                        == "event_miner_spirit_return",
                    $"Journey seed {seed} must revisit the miner spirit in act two.");
                Require(graph.Nodes.Single(node => node.ActIndex == 2 && node.DepthIndex == 6).ContentId
                        == "event_old_contract_truth",
                    $"Journey seed {seed} must reveal the old contract before the second gate.");
                Require(graph.Nodes.Single(node => node.ActIndex == 3 && node.DepthIndex == 0).Type
                        == DemoJourneyNodeType.Breakthrough,
                    $"Journey seed {seed} must enter act three through the story breakthrough.");
                Require(graph.Nodes.Single(node => node.ActIndex == 3 && node.DepthIndex == 6).ContentId
                        == "event_sword_furnace_contract",
                    $"Journey seed {seed} must settle the sword furnace contract before the Boss.");
                DemoJourneyNode hiddenContract = graph.Nodes.Single(node => node.IsHidden);
                DemoJourneyNode beforeHidden = graph.Nodes.First(node => node.ActIndex == 3 && node.DepthIndex == 4);
                Require(!graph.GetReachableNodeIds(new[] { beforeHidden.NodeId }).Contains(hiddenContract.NodeId),
                    $"Journey seed {seed} must keep the contract cache hidden without its concrete cause.");
                Require(graph.GetReachableNodeIds(
                        new[] { beforeHidden.NodeId },
                        new[] { "experience_miner_spirit_bound" })
                    .Contains(hiddenContract.NodeId),
                    $"Journey seed {seed} must reveal the contract cache after binding the miner spirit.");

                for (int act = 1; act <= 3; act++)
                {
                    Require(graph.GetActNodes(act)
                        .GroupBy(node => node.DepthIndex)
                        .Any(layer => layer.Count() > 1
                            && layer.Select(node => node.Type).Distinct().Count() > 1),
                        $"Journey seed {seed} act {act} must offer at least one structurally different branch.");
                    int combatDepths = graph.GetActNodes(act)
                        .GroupBy(node => node.DepthIndex)
                        .Count(layer => layer.Any(node => node.IsCombat));
                    Require(combatDepths >= 3 && combatDepths <= 5,
                        $"Journey seed {seed} act {act} must stay near the intended combat/event split.");
                }

                for (int act = 1; act <= 3; act++)
                {
                    DemoJourneyPathLengthRange range = graph.GetActPathLengthRange(act);
                    Require(range.Minimum >= 7 && range.Maximum <= 8,
                        $"Journey seed {seed} act {act} standard path must contain 7-8 nodes.");
                    Require(range.Minimum >= 6 && range.Maximum <= 10,
                        $"Journey seed {seed} act {act} legal path range is 6-10 nodes.");
                }

                DemoJourneyNode boss = graph.Nodes.Single(node => node.Type == DemoJourneyNodeType.Boss);
                Require(graph.GetIncomingNodeIds(boss.NodeId).All(nodeId =>
                    graph.TryGetNode(nodeId, out DemoJourneyNode node) && node.IsPreparation),
                    $"Journey seed {seed} must show preparation immediately before the Boss.");
                Require(graph.GetReachableNodeIds(Array.Empty<string>()).SequenceEqual(new[] { graph.StartNodeId }),
                    $"Journey seed {seed} initial frontier must contain only Start.");

                IReadOnlyList<string> firstFrontier = graph.GetReachableNodeIds(new[] { graph.StartNodeId });
                string selectedBranch = firstFrontier[0];
                IReadOnlyList<string> nextFrontier = graph.GetReachableNodeIds(new[] { graph.StartNodeId, selectedBranch });
                Require(firstFrontier.Skip(1).All(sibling => !nextFrontier.Contains(sibling)),
                    $"Journey seed {seed} must close unchosen sibling nodes after committing a branch.");

                DemoRunSaveV2 graphSnapshot = new DemoRunSaveV2
                {
                    ResolvedGraphNodes = DemoJourneyGraphSnapshotCodec.CaptureNodes(graph),
                    ResolvedGraphEdges = DemoJourneyGraphSnapshotCodec.CaptureEdges(graph)
                };
                Require(
                    DemoJourneyGraphSnapshotCodec.TryRestore(
                        seed,
                        graphSnapshot.ResolvedGraphNodes,
                        graphSnapshot.ResolvedGraphEdges,
                        out DemoJourneyGraph restored,
                        out string restoreError),
                    restoreError);
                Require(JourneyFingerprint(restored) == JourneyFingerprint(graph),
                    $"Journey seed {seed} full graph snapshot must round-trip exactly.");
            }
        }

        private static string JourneyFingerprint(DemoJourneyGraph graph)
        {
            string nodes = string.Join("|", graph.Nodes.Select(node =>
                $"{node.NodeId}:{node.ActIndex}:{node.DepthIndex}:{node.LaneIndex}:{node.Type}:{node.ContentId}:{node.RequiredStoryFlagId}:{node.IsHidden}"));
            string edges = string.Join("|", graph.Edges.Select(edge => $"{edge.FromNodeId}>{edge.ToNodeId}"));
            return nodes + "||" + edges;
        }

        private static void VerifyRunSaveContract()
        {
            DemoRunSaveV2 stable = CreateRunSaveFixture();
            Require(stable.Validate().Count == 0, "The V2 run checkpoint fixture must validate.");
            Require(stable.MetaCommitIdempotencyKey == stable.RunId, "Meta settlement must use run_id as its idempotency key.");

            DemoMemoryRunSaveStore nodeStore = new DemoMemoryRunSaveStore();
            Require(nodeStore.TryWriteCheckpoint(stable, out string initialWriteError), initialWriteError);

            DemoNodeOutcomeTransaction transaction = new DemoNodeOutcomeTransaction(stable);
            Require(
                transaction.TryStageCompletion(
                    "act1_start",
                    "act1_battle",
                    DemoRunFlowPhaseId.EncounterIntro,
                    new[] { "act1_battle", "act1_event" },
                    out string stageError),
                stageError);
            transaction.AddExperienceFlag("experience_miner_spirit_helped");
            transaction.AddPendingMetaDiscovery("discovery_miner_spirit");
            Require(transaction.TryCommit(nodeStore, out DemoRunSaveV2 committed, out string commitError), commitError);
            Require(stable.CompletedNodeIds.Count == 0, "A node transaction must not mutate its stable input checkpoint.");
            Require(committed.CheckpointSequence == 1 && committed.LastCommittedNodeId == "act1_start", "Node outcome commit identity is inconsistent.");
            Require(committed.ExperienceFlagIds.Contains("experience_miner_spirit_helped"), "Node outcome experience was not committed.");
            Require(nodeStore.TryLoadPrevious(out DemoRunSaveV2 previous, out string previousError), previousError);
            Require(previous.CheckpointSequence == 0, "The previous slot must retain the prior stable checkpoint.");

            nodeStore.ReplaceLatestPayloadForTesting("{broken checkpoint");
            Require(
                nodeStore.TryLoadLatestOrPrevious(out DemoRunSaveV2 recovered, out bool recoveredPrevious, out string recoveryError),
                recoveryError);
            Require(recoveredPrevious && recovered.CheckpointSequence == 0, "A damaged latest slot must recover the readable previous checkpoint.");
            Require(nodeStore.ArchiveIncompatible("content_version retired", out string archiveError), archiveError);
            Require(nodeStore.Archived.Count == 2, "Archiving an incompatible run must preserve both occupied slots.");
            Require(!nodeStore.TryLoadLatest(out _, out _), "Archived incompatible saves must not remain resumable.");

            DemoRunSaveV2 preBattle = CreateRunSaveFixture();
            preBattle.FlowPhaseId = DemoRunFlowPhaseId.EncounterIntro;
            preBattle.CurrentNodeId = "act1_battle";
            preBattle.PendingEncounterId = "encounter_old_mine_entry";
            preBattle.PendingEncounterSeed = 73129;
            preBattle.MinerSpiritLife.Granted = true;
            DemoMemoryRunSaveStore lifeStore = new DemoMemoryRunSaveStore();
            Require(lifeStore.TryWriteCheckpoint(preBattle, out string lifeWriteError), lifeWriteError);
            Require(
                DemoMinerSpiritLifeRetry.TryPersistConsumptionBeforeRetry(
                    preBattle,
                    lifeStore,
                    out DemoRunSaveV2 retry,
                    out string retryError),
                retryError);
            Require(retry.MinerSpiritLife.Consumed, "Miner spirit life must be durably consumed before retry is allowed.");
            Require(retry.PendingEncounterSeed == preBattle.PendingEncounterSeed, "Miner spirit retry must preserve the encounter seed.");
            Require(retry.CheckpointSequence == preBattle.CheckpointSequence + 1, "Miner spirit consumption must create a new stable checkpoint.");
            Require(
                !DemoMinerSpiritLifeRetry.TryPersistConsumptionBeforeRetry(retry, lifeStore, out _, out _),
                "A consumed miner spirit life must never allow a second retry.");

            DemoRunSaveV2 incompatible = preBattle.DeepClone();
            incompatible.SaveVersion = 1;
            Require(!lifeStore.TryWriteCheckpoint(incompatible, out _), "An incompatible save version must fail before rotating stable slots.");
            Require(lifeStore.TryLoadLatest(out DemoRunSaveV2 afterRejectedWrite, out _)
                && afterRejectedWrite.MinerSpiritLife.Consumed, "A rejected write must leave the latest stable checkpoint intact.");

            string diskDirectory = Path.Combine(Path.GetTempPath(), "ptw_run_save_" + Guid.NewGuid().ToString("N"));
            try
            {
                DemoFileRunSaveStore diskStore = new DemoFileRunSaveStore(diskDirectory, "compile_check");
                DemoRunSaveV2 diskInitial = CreateRunSaveFixture();
                Require(diskStore.TryWriteCheckpoint(diskInitial, out string diskInitialError), diskInitialError);
                DemoRunSaveV2 diskNext = diskInitial.DeepClone();
                diskNext.CheckpointSequence++;
                Require(diskStore.TryWriteCheckpoint(diskNext, out string diskNextError), diskNextError);
                Require(diskStore.TryLoadPrevious(out DemoRunSaveV2 diskPrevious, out string diskPreviousError), diskPreviousError);
                Require(diskPrevious.CheckpointSequence == 0, "Atomic disk replacement must preserve the previous stable checkpoint.");
                File.WriteAllText(diskStore.LatestPath, "{broken latest");
                Require(
                    diskStore.TryLoadLatestOrPrevious(out DemoRunSaveV2 diskRecovered, out bool diskRecoveredPrevious, out string diskRecoveryError),
                    diskRecoveryError);
                Require(diskRecoveredPrevious && diskRecovered.CheckpointSequence == 0, "Disk restore must fall back from a damaged latest checkpoint.");
                Require(diskStore.ArchiveIncompatible("compile check archive", out string diskArchiveError), diskArchiveError);
                Require(!File.Exists(diskStore.LatestPath) && !File.Exists(diskStore.PreviousPath), "Archived disk slots must no longer be resumable.");
            }
            finally
            {
                if (Directory.Exists(diskDirectory))
                {
                    Directory.Delete(diskDirectory, true);
                }
            }
        }

        private static void VerifyJourneyRunSession()
        {
            const int seed = 43127;
            DemoJourneyGraph graph = DemoJourneyGraphGenerator.Generate(seed);
            DemoMemoryRunSaveStore store = new DemoMemoryRunSaveStore();
            DemoJourneyRunSessionOptions options = new DemoJourneyRunSessionOptions
            {
                RunId = "run_journey_session_compile_check",
                ConfigSchemaVersion = "2",
                ContentVersion = "old_mine_vertical_slice_1",
                MapAlgorithmVersion = "journey_graph_v1",
                RegionId = "region_old_mine",
                Build = new DemoRunBuildSnapshot
                {
                    StartingPracticePackageId = "package_branch_sword_embryo",
                    MindMethodId = "practice_branch_breathing",
                    MindMethodLevel = 1,
                    InnateArtifactId = "artifact_broken_sword_embryo",
                    TechniqueIds = new List<string>
                    {
                        "technique_breathing_recovery",
                        "technique_incomplete_sword_scroll"
                    }
                },
                Realm = new DemoRunRealmSnapshot { RealmId = "realm_qi_refining", Stage = 1 }
            };

            Require(
                DemoJourneyRunSession.TryCreateNew(graph, store, options, seed, out DemoJourneyRunSession session, out string createError),
                createError);
            Require(session.FlowPhaseId == DemoRunFlowPhaseId.JourneyMap, "A new journey run must start at the map frontier.");
            Require(session.Snapshot.ReachableNodeIds.SequenceEqual(new[] { graph.StartNodeId }), "A new journey run must expose only its start node.");
            Require(session.TrySelectReachableNode(graph.StartNodeId, out DemoRunSaveV2 selectedStart, out string selectStartError), selectStartError);
            Require(selectedStart.FlowPhaseId == DemoRunFlowPhaseId.NodeScene, "Selecting the opening journey node must persist its node scene.");
            DemoJourneyNodeOutcome openingOutcome = new DemoJourneyNodeOutcome();
            openingOutcome.AddExperienceFlag("experience_entered_old_mine");
            Require(session.TryCompleteCurrentNode(openingOutcome, out DemoRunSaveV2 completedStart, out string completeStartError), completeStartError);
            Require(completedStart.CompletedNodeIds.Contains(graph.StartNodeId), "Node completion must be committed exactly once.");
            Require(!session.TryCompleteCurrentNode(openingOutcome, out _, out _), "A settled node must reject duplicate outcome application.");

            DemoJourneyNode combat = null;
            int traversalGuard = 0;
            while (combat == null && traversalGuard++ < graph.Nodes.Count)
            {
                DemoRunSaveV2 mapSnapshot = session.Snapshot;
                combat = mapSnapshot.ReachableNodeIds
                    .Select(id => graph.TryGetNode(id, out DemoJourneyNode node) ? node : null)
                    .FirstOrDefault(node => node != null && node.IsCombat);
                if (combat != null)
                {
                    break;
                }

                DemoJourneyNode next = mapSnapshot.ReachableNodeIds
                    .Select(id => graph.TryGetNode(id, out DemoJourneyNode node) ? node : null)
                    .First(node => node != null);
                Require(session.TrySelectReachableNode(next.NodeId, out _, out string selectNodeError), selectNodeError);
                Require(session.TryCompleteCurrentNode(new DemoJourneyNodeOutcome(), out _, out string completeNodeError), completeNodeError);
            }

            Require(combat != null, "The deterministic journey must reach a combat node.");
            Require(
                session.TrySelectEncounter(combat.NodeId, "encounter_compile_check", out DemoRunSaveV2 selectedEncounter, out string selectEncounterError),
                selectEncounterError);
            int expectedEncounterSeed = DemoJourneyRunSession.ComputeEncounterSeed(seed, combat.NodeId, "encounter_compile_check");
            Require(selectedEncounter.PendingEncounterSeed == expectedEncounterSeed, "Encounter selection must persist a deterministic pre-battle seed.");
            int encounterSequence = selectedEncounter.CheckpointSequence;
            Require(
                session.TrySelectEncounter(combat.NodeId, "encounter_compile_check", out DemoRunSaveV2 repeatedEncounter, out string repeatedEncounterError),
                repeatedEncounterError);
            Require(repeatedEncounter.CheckpointSequence == encounterSequence, "Repeating the same encounter selection must be idempotent.");
            Require(
                DemoJourneyRunSession.TryRestore(
                    graph,
                    store,
                    options.ConfigSchemaVersion,
                    options.ContentVersion,
                    options.MapAlgorithmVersion,
                    options.RegionId,
                    out DemoJourneyRunSession restored,
                    out bool recoveredPrevious,
                    out string restoreError),
                restoreError);
            Require(!recoveredPrevious && restored.CurrentNodeId == combat.NodeId, "Latest journey checkpoint must restore the same selected encounter.");

            DemoJourneyNodeOutcome battleOutcome = new DemoJourneyNodeOutcome { BattlesWonDelta = 1 };
            Require(restored.TryCompleteCurrentNode(battleOutcome, out DemoRunSaveV2 completedBattle, out string completeBattleError), completeBattleError);
            Require(completedBattle.Statistics.BattlesWon == 1, "Battle outcome statistics must commit with the node transaction.");
            store.ReplaceLatestPayloadForTesting("{broken journey checkpoint");
            Require(
                DemoJourneyRunSession.TryRestore(
                    graph,
                    store,
                    options.ConfigSchemaVersion,
                    options.ContentVersion,
                    options.MapAlgorithmVersion,
                    options.RegionId,
                    out DemoJourneyRunSession recovered,
                    out bool usedPrevious,
                    out string recoveryError),
                recoveryError);
            Require(usedPrevious && recovered.CurrentNodeId == combat.NodeId, "A damaged journey head must recover its previous pre-battle checkpoint.");
            Require(
                !DemoJourneyRunSession.TryRestore(
                    graph,
                    store,
                    options.ConfigSchemaVersion,
                    "retired_content_version",
                    options.MapAlgorithmVersion,
                    options.RegionId,
                    out _,
                    out _,
                    out _),
                "Journey restore must reject a mismatched content version.");
        }

        private static DemoRunSaveV2 CreateRunSaveFixture()
        {
            return new DemoRunSaveV2
            {
                ConfigSchemaVersion = "2",
                ContentVersion = "old_mine_vertical_slice_1",
                MapAlgorithmVersion = "journey_graph_v1",
                RunId = "run_compile_check_001",
                RootSeed = 19840721,
                FlowPhaseId = DemoRunFlowPhaseId.JourneyMap,
                RegionId = "region_old_mine",
                ActIndex = 1,
                CurrentNodeId = "act1_start",
                ResolvedGraphNodeSnapshotIds = new List<string>
                {
                    "act1_start",
                    "act1_battle",
                    "act1_event",
                    "act1_mini_boss"
                },
                ReachableNodeIds = new List<string> { "act1_start" },
                Build = new DemoRunBuildSnapshot
                {
                    StartingPracticePackageId = "package_branch_sword_embryo",
                    MindMethodId = "practice_branch_breathing",
                    MindMethodLevel = 1,
                    InnateArtifactId = "vessel_branch_sword_embryo",
                    TechniqueIds = new List<string>
                    {
                        "technique_breathing_recovery",
                        "technique_incomplete_sword_scroll"
                    }
                },
                Realm = new DemoRunRealmSnapshot
                {
                    RealmId = "realm_qi_refining",
                    Stage = 1
                }
            };
        }

        private static void VerifyOpeningVesselFlow()
        {
            Require(DemoConfigRepository.HasLoadedConfig, "game_config.json was not loaded.");

            List<DemoRootDefinition> playableRoots = DemoConfigRepository.GetDefaultRoots(4);
            List<DemoRootDefinition> displayedRoots = DemoConfigRepository.GetRootsForOpening(4);
            Require(playableRoots.Count == 1 && playableRoots[0].Id == "root_branch", "Only 世家旁支 may be entered in the P0 slice.");
            Require(displayedRoots.Count > playableRoots.Count, "Locked roots must remain visible in opening selection.");
            Require(displayedRoots.Count(root => root.IsAvailable) == 1, "Exactly one root must be available.");

            List<DemoJourneyVesselDefinition> vessels = DemoConfigRepository.GetJourneyVesselsForRoot("root_branch", 4, true);
            Require(vessels.Count >= 3, "Carried item selection must display locked alternatives.");
            Require(vessels.Count(vessel => vessel.IsAvailable) == 1, "Exactly one carried item must be available.");
            DemoJourneyVesselDefinition swordEmbryo = vessels.Single(vessel => vessel.IsAvailable);
            Require(swordEmbryo.Id == "vessel_branch_sword_embryo", "The playable carried item must be 残剑胚.");
            Require(swordEmbryo.StarterPoolId == "starter_story_sword_package", "残剑胚 must reference the story starting package pool.");
            Require(swordEmbryo.StartingPracticePackageId == "package_branch_sword_embryo", "残剑胚 must reference its story starting practice package.");

            foreach (string regionId in swordEmbryo.RegionCandidateIds)
            {
                Require(DemoConfigRepository.TryGetRegion(regionId, out DemoRegionDefinition region), $"Missing region reference: {regionId}");
                if (regionId == "region_old_mine")
                {
                    Require(region.IsAvailable, "旧矿地窟 must be available.");
                }
                else
                {
                    Require(!region.IsAvailable, $"{region.Name} must remain visible but locked.");
                }
            }

            DemoRunState run = CreateSwordEmbryoRun();
            Require(run.OpeningSelection.StartingPracticePackage?.Id == "package_branch_sword_embryo", "The selected vessel must resolve its story starting practice package.");
            Require(DemoConfigRepository.TryGetRegion("region_ancestral_vault", out DemoRegionDefinition lockedRegion), "Locked region config is missing.");
            Require(!run.TrySetFirstRegion(lockedRegion), "An unavailable map must not be enterable.");
            Require(DemoConfigRepository.TryGetRegion("region_old_mine", out DemoRegionDefinition oldMine) && run.TrySetFirstRegion(oldMine), "旧矿地窟 must be enterable.");
            Require(run.Deck.Count == 2, "The opening story must grant exactly two active techniques.");
            Require(run.Deck.Count(card => card.Id == "technique_breathing_recovery") == 1, "旁支吐纳法 must grant 吐纳归息.");
            Require(run.Deck.Count(card => card.Id == "technique_incomplete_sword_scroll") == 1, "The story must grant 残卷御剑式.");
            Require(run.Deck.All(card => card.Id != "sword_slash" && card.Id != "guard_step" && card.Id != "cloud_step"), "Legacy basic cards must not enter the new opening deck.");
            Require(run.MainGongfa == DemoGongfaType.None, "The run must start without a main gongfa.");
            Require(run.Artifacts.Count == 0 && run.Relics.Count == 0, "The run must start without formal artifacts or relics.");

            DemoBattleState openingBattle = new DemoBattleState();
            openingBattle.StartBattle(new DemoBattleSetup
            {
                Deck = run.Deck,
                PlayerHealth = run.CurrentHealth,
                EnemyId = "enemy_opening_contract",
                EnemyHealth = 9999,
                IsOpeningBattle = true,
                MaxEnergy = 5,
                InitialEnergy = 3,
                HandLimit = 6,
                InitialHandSize = 2,
                IntroSeconds = 0f,
                RandomSeed = 29
            });
            Require(openingBattle.MaxEnergy == 5 && openingBattle.Energy == 3, "炼气 opening battle must load 5 max energy and 3 initial energy.");
            Require(openingBattle.HandLimit == 6 && openingBattle.Hand.Count == 2, "Opening battle must show the two story techniques under the six-card hand limit.");
            Require(openingBattle.Hand.Select(card => card.Id).OrderBy(id => id).SequenceEqual(run.Deck.Select(card => card.Id).OrderBy(id => id)), "Both story techniques must be visible in the opening hand.");
        }

        private static void VerifyRealtimeBattleLoop()
        {
            DemoCard strike = TestCard("test_strike", "试锋", damage: 3);
            DemoBattleState flow = new DemoBattleState();
            flow.StartBattle(new DemoBattleSetup
            {
                Deck = Enumerable.Range(0, 8).Select(_ => strike).ToList(),
                EnemyId = "enemy_opening_contract",
                EnemyHealth = 9999,
                PlayerHealth = 99,
                InitialEnergy = 0,
                InitialHandSize = 7,
                HandLimit = 7,
                DrawIntervalSeconds = 1f,
                FlyingSwordIntervalSeconds = 100f,
                EnemyIntentMinSeconds = 100f,
                EnemyIntentMaxSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 7
            });

            Require(flow.EnemyId == "enemy_opening_contract", "Battle state must expose the stable enemy id.");
            Require(
                flow.TryConsumePresentationStep(out DemoBattlePresentationStep openingStep)
                && openingStep.Type == DemoBattlePresentationStepType.BattleStart
                && openingStep.SourceId == "enemy_opening_contract",
                "Battle-start presentation must retain the enemy source id.");

            float fullHandTimer = flow.DrawTimer;
            flow.Tick(1.1f);
            Require(flow.Energy >= 1, "Energy must regenerate continuously at one per second.");
            Require(flow.Hand.Count == 7, "A full hand must not draw or burn the next card.");
            Require(Math.Abs(flow.DrawTimer - fullHandTimer) < 0.01f, "Draw timer must pause while the hand is full.");

            int enemyBefore = flow.Enemy.Health;
            Require(flow.TryPlayCard(0), "A playable card must resolve immediately.");
            Require(flow.Enemy.Health == enemyBefore - 3, "Immediate card damage did not resolve.");
            Require(flow.DiscardPile.Count == 1 && flow.Hand.Count == 6, "Played card must enter discard immediately.");
            Require(
                flow.TryConsumePresentationStep(out DemoBattlePresentationStep cardStep)
                && cardStep.Type == DemoBattlePresentationStepType.CardCast
                && cardStep.SourceId == strike.Id,
                "Card presentation must retain the stable card source id.");
            flow.Tick(1.1f);
            Require(flow.Hand.Count == 7, "Drawing must resume after hand space opens.");

            DemoCard costlyCard = Cards("sword_slash")[0];
            DemoBattleSetup insufficientSetup = BasicSetup(new[] { costlyCard }, enemyHealth: 9999, initialHand: 1);
            insufficientSetup.InitialEnergy = 0;
            DemoBattleState insufficient = new DemoBattleState();
            insufficient.StartBattle(insufficientSetup);
            string[] handBeforeFailedPlay = insufficient.Hand.Select(card => card.Id).ToArray();
            int discardBeforeFailedPlay = insufficient.DiscardPile.Count;
            int healthBeforeFailedPlay = insufficient.Enemy.Health;
            int presentationBeforeFailedPlay = insufficient.PendingPresentationStepCount;
            Require(!insufficient.TryPlayCard(0), "A card must not resolve without enough energy.");
            Require(insufficient.Hand.Select(card => card.Id).SequenceEqual(handBeforeFailedPlay), "A failed card play must not change the hand.");
            Require(insufficient.DiscardPile.Count == discardBeforeFailedPlay, "A failed card play must not change the discard pile.");
            Require(insufficient.Enemy.Health == healthBeforeFailedPlay, "A failed card play must not damage the enemy.");
            Require(insufficient.PendingPresentationStepCount == presentationBeforeFailedPlay, "A failed card play must not enqueue presentation feedback.");

            foreach (string sourceId in new[] { "sword_slash", "guard_step", "cloud_step" })
            {
                DemoCard sourceCard = Cards(sourceId)[0];
                DemoBattleState cardSource = new DemoBattleState();
                cardSource.StartBattle(BasicSetup(new[] { sourceCard }, enemyHealth: 9999, initialHand: 1));
                cardSource.ClearPresentationSteps();
                Require(cardSource.TryPlayCard(0), $"{sourceId} must be playable in the presentation source contract.");
                DemoBattlePresentationStep sourceStep = ConsumePresentationStep(cardSource, DemoBattlePresentationStepType.CardCast);
                Require(sourceStep.SourceId == sourceId, $"{sourceId} card presentation must retain its stable source id.");
            }

            DemoCard drawSourceCard = Cards("cloud_step")[0];
            DemoBattleState drawSource = new DemoBattleState();
            drawSource.StartBattle(BasicSetup(new[] { drawSourceCard }, enemyHealth: 9999, initialHand: 0, drawInterval: 0.2f));
            drawSource.ClearPresentationSteps();
            drawSource.Tick(0.21f);
            DemoBattlePresentationStep drawStep = ConsumePresentationStep(drawSource, DemoBattlePresentationStepType.CardDraw);
            Require(drawStep.SourceId == drawSourceCard.Id, "Card-draw presentation must retain the drawn card source id.");

            DemoBattleState shuffle = new DemoBattleState();
            shuffle.StartBattle(BasicSetup(new[] { strike }, enemyHealth: 9999, initialHand: 1, drawInterval: 0.2f));
            Require(shuffle.TryPlayCard(0), "Single-card shuffle setup failed to play.");
            shuffle.Tick(0.25f);
            Require(shuffle.ShuffleCount == 1 && shuffle.Hand.Count == 1, "Empty draw pile must shuffle discard back before drawing.");

            DemoCard summon = TestCard("test_summon", "三巡剑影", temporarySwords: 2);
            DemoBattleState swords = new DemoBattleState();
            swords.StartBattle(BasicSetup(new[] { summon }, enemyHealth: 9999, initialHand: 1, volleyInterval: 0.5f));
            Require(swords.TryPlayCard(0) && swords.TemporarySwords == 2, "Temporary swords were not created.");
            swords.Tick(0.51f);
            swords.Tick(0.51f);
            Require(swords.TemporarySwords == 2 && swords.VolleysFired == 2, "Temporary swords must participate in the first two volleys.");
            swords.Tick(0.51f);
            Require(swords.TemporarySwords == 0 && swords.VolleysFired == 3, "Temporary swords must expire after exactly three volleys.");

            DemoBattleState intent = new DemoBattleState();
            DemoBattleSetup intentSetup = BasicSetup(new[] { strike }, enemyHealth: 9999, initialHand: 0, volleyInterval: 100f);
            intentSetup.EnemyIntentMinSeconds = 1f;
            intentSetup.EnemyIntentMaxSeconds = 1f;
            intentSetup.EnemyId = "enemy_intent_contract";
            intent.StartBattle(intentSetup);
            intent.ClearPresentationSteps();
            intent.Tick(1.01f);
            Require(intent.EnemyActionCount == 1, "Enemy intent must resolve on its realtime countdown.");
            DemoBattlePresentationStep enemyStep = ConsumePresentationStep(intent, DemoBattlePresentationStepType.EnemyAttack);
            Require(enemyStep.SourceId == intent.EnemyId, "Enemy attack presentation must retain the stable enemy id.");
            intent.ClearBattle();
            Require(string.IsNullOrEmpty(intent.EnemyId), "Clearing a battle must clear the exposed enemy id.");

            DemoBattleState boss = new DemoBattleState();
            DemoCard phaseBreaker = TestCard("phase_breaker", "破劫试锋", damage: 66);
            DemoBattleSetup bossSetup = BasicSetup(new[] { phaseBreaker }, enemyHealth: 100, initialHand: 1, volleyInterval: 100f);
            bossSetup.IsBoss = true;
            bossSetup.PlayerHealth = 99;
            boss.StartBattle(bossSetup);
            Require(boss.BossPhase == DemoBossPhase.ThunderCloud, "Boss must start in ThunderCloud phase.");
            Require(boss.TryPlayCard(0), "Boss phase test card was not playable.");
            Require(boss.BossPhase == DemoBossPhase.CalamityDescends, "Boss health thresholds must move into the final phase.");
            boss.Tick(5.01f);
            Require(boss.EnemyActionCount == 1, "Boss charge intent did not resolve.");
            Require(boss.EnemyIntentDuration <= 3.51f && boss.BossIntentText.Contains("短读条"), "Boss charge must open a 3.5 second burst window.");

            long lastSequence = -1;
            int consumed = 0;
            while (boss.TryConsumePresentationStep(out DemoBattlePresentationStep step))
            {
                Require(step.Sequence > lastSequence, "Presentation events must be consumed in strict sequence.");
                lastSequence = step.Sequence;
                consumed++;
            }
            Require(consumed >= 3, "Boss phase test must emit battle, card, phase and charge feedback.");
        }

        private static void VerifyRewardProgression()
        {
            DemoRunState run = CreateSwordEmbryoRun();
            DemoRewardService service = new DemoRewardService(17);

            DemoRewardContext opening = Context(1, DemoRewardSource.OpeningBattle, DemoRewardTier.Opening, DemoRouteRisk.Stable, 101);
            List<DemoReward> openingChoices = service.CreateChoices(opening, run);
            string[] expectedOpening = { "凝剑诀", "踏云遁", "聚灵换息" };
            Require(openingChoices.Select(reward => reward.Name).SequenceEqual(expectedOpening), "First battle reward must be the fixed designed trio.");
            Require(openingChoices.All(IsP0Reward), "Relics must not enter the P0 reward pool.");
            ApplyReward(run, openingChoices[0]);

            for (int seed = 0; seed < 20; seed++)
            {
                List<DemoReward> early = service.CreateChoices(Context(1, DemoRewardSource.EliteBattle, DemoRewardTier.Elite, DemoRouteRisk.Risky, seed), run);
                Require(early.All(reward => reward.Card == null || reward.Card.Type != DemoCardType.Finisher), "Layer one must never offer a finisher.");
                Require(early.All(reward => reward.Type != DemoRewardType.Relic), "Layer one must not expose relic rewards.");
            }

            ApplyReward(run, Focus(service.CreateChoices(Context(1, DemoRewardSource.NormalBattle, DemoRewardTier.Standard, DemoRouteRisk.Stable, 201), run)));
            ApplyReward(run, Focus(service.CreateChoices(Context(1, DemoRewardSource.Training, DemoRewardTier.Build, DemoRouteRisk.Build, 202), run)));
            int earlyComponents = new[] { "sword_focus", "summon_sword", "returning_array", "sword_rain", "sheathe_edge", "sword_tide", "heaven_opening" }
                .Count(run.HasBuildComponent);
            Require(earlyComponents >= 3, "By the end of layer one the build must contain a starter and 2-3 Wanjian components.");

            ApplyReward(run, Focus(service.CreateChoices(Context(2, DemoRewardSource.Training, DemoRewardTier.Core, DemoRouteRisk.Stable, 301), run)));
            Require(run.HasBuildComponent("sword_array"), "Layer two must guarantee 小诛仙剑阵.");
            ApplyReward(run, Focus(service.CreateChoices(Context(2, DemoRewardSource.NormalBattle, DemoRewardTier.Core, DemoRouteRisk.Stable, 302), run)));
            Require(run.HasGongfa(DemoGongfaType.SwordControlArt) || run.HasArtifact(DemoArtifactType.SwordBox), "Layer two must guarantee 御剑诀 or 剑匣.");

            List<DemoReward> finalStandard = service.CreateChoices(Context(3, DemoRewardSource.Preparation, DemoRewardTier.Finisher, DemoRouteRisk.Stable, 401, allowsFinisher: true), run);
            Require(Focus(finalStandard).Card?.Id == "wanjian_burst", "Layer three must guarantee 万剑诀 in the focus slot.");
            ApplyReward(run, Focus(finalStandard));

            List<DemoReward> riskyFinal = service.CreateChoices(Context(3, DemoRewardSource.EliteBattle, DemoRewardTier.High, DemoRouteRisk.Risky, 402, allowsFinisher: true, allowsDivine: true), run);
            Require(riskyFinal.Any(reward => reward.Type == DemoRewardType.Gongfa && reward.GongfaType == DemoGongfaType.WanjianReturn), "High-risk layer three must be able to add 万剑归宗.");
            Require(riskyFinal.Select(reward => reward.Slot).SequenceEqual(new[] { DemoRewardSlot.Focus, DemoRewardSlot.Utility, DemoRewardSlot.Wildcard }), "Rewards must use focus, survival and wildcard slots.");

            DemoReward bossSurvival = service.CreateGuaranteedReward("survival_boss", run);
            Require(bossSurvival.Slot == DemoRewardSlot.Utility, "Boss survival guarantee must replace the utility slot, never the finisher focus slot.");
        }

        private static void VerifyThreeLayerRoutesAndIds()
        {
            string[] routeIds =
            {
                "route_branch_stable", "route_branch_risky", "route_branch_build",
                "route_middle_stable", "route_middle_aggressive", "route_middle_artifact",
                "route_final_stable", "route_final_seclusion", "route_final_desperate"
            };

            foreach (string routeId in routeIds)
            {
                Require(DemoConfigRepository.TryGetRoutePlan(routeId, out DemoRoutePlanDefinition definition), $"Missing route plan: {routeId}");
                Require(!string.IsNullOrEmpty(definition.Plan.Id) && definition.Plan.Nodes.Count > 0, $"Route {routeId} has no stable ID or nodes.");
                foreach (DemoMapNode node in definition.Plan.Nodes)
                {
                    Require(!string.IsNullOrEmpty(node.NodeId), $"{routeId}/{node.Name} is missing node_id.");
                    if (node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss)
                    {
                        Require(DemoConfigRepository.TryGetEnemyById(node.EncounterId, out DemoEnemyDefinition enemy), $"{node.NodeId} has an invalid encounter_id.");
                        Require(enemy.MaxHealth > 0, $"{node.NodeId} enemy health must be positive.");
                        Require(DemoConfigRepository.TryGetRewardProfile(node.RewardProfileId, out _), $"{node.NodeId} has an invalid reward_profile_id.");
                    }

                    if (!string.IsNullOrEmpty(node.ActionProfileId))
                    {
                        Require(DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out _), $"{node.NodeId} has an invalid action_profile_id.");
                    }
                }
            }

            DemoRoutePlanDefinition stableOne = Route("route_branch_stable");
            DemoRoutePlanDefinition riskyOne = Route("route_branch_risky");
            DemoRoutePlanDefinition buildOne = Route("route_branch_build");
            Require(NodeTypes(stableOne).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Training, DemoNodeType.RouteChoice }), "Layer one stable route order changed.");
            Require(NodeTypes(riskyOne).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Battle, DemoNodeType.RouteChoice }), "Layer one risky route order changed.");
            Require(NodeTypes(buildOne).SequenceEqual(new[] { DemoNodeType.Training, DemoNodeType.Battle, DemoNodeType.RouteChoice }), "Layer one build route order changed.");
            Require(riskyOne.Plan.Nodes.Count(node => node.Type == DemoNodeType.Battle) > stableOne.Plan.Nodes.Count(node => node.Type == DemoNodeType.Battle), "Risky route must front-load more combat rewards.");
            Require(buildOne.Plan.Nodes.Any(node => node.RewardProfileId == "reward_layer1_build" || node.ActionProfileId == "action_training_focus_l1"), "Build route must contain deterministic focus filling.");

            Require(NodeTypes(Route("route_middle_stable")).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Training, DemoNodeType.RouteChoice }), "Layer two stable route order changed.");
            Require(NodeTypes(Route("route_middle_aggressive")).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Battle, DemoNodeType.RouteChoice }), "Layer two aggressive route order changed.");
            Require(NodeTypes(Route("route_middle_artifact")).SequenceEqual(new[] { DemoNodeType.Shop, DemoNodeType.Battle, DemoNodeType.RouteChoice }), "Layer two artifact route order changed.");
            Require(NodeTypes(Route("route_final_stable")).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Shop, DemoNodeType.Boss }), "Layer three stable route order changed.");
            Require(NodeTypes(Route("route_final_seclusion")).SequenceEqual(new[] { DemoNodeType.Training, DemoNodeType.Battle, DemoNodeType.Boss }), "Layer three seclusion route order changed.");
            Require(NodeTypes(Route("route_final_desperate")).SequenceEqual(new[] { DemoNodeType.Battle, DemoNodeType.Boss }), "Layer three desperate route order changed.");
            Require(new[] { "route_final_stable", "route_final_seclusion", "route_final_desperate" }
                .All(routeId => Route(routeId).Plan.Nodes.All(node => node.Type != DemoNodeType.Result)), "Final routes must use the dynamic run result rather than dead static Result nodes.");

            DemoMapRun map = new DemoMapRun();
            map.CompleteCurrentNode();
            Require(map.CurrentNode.Type == DemoNodeType.Battle, "Opening selection must enter the opening battle.");
            Require(!string.IsNullOrEmpty(map.CurrentNode.NodeId) && !string.IsNullOrEmpty(map.CurrentNode.EncounterId), "Opening battle must use stable IDs.");
            map.CompleteCurrentNode();
            Require(map.CurrentNode.Type == DemoNodeType.RouteChoice, "Each battle now opens its reward before the map advances directly to route choice.");

            SelectAndCompleteToChoice(map, "route_branch_stable");
            SelectAndCompleteToChoice(map, "route_middle_stable");
            Require(map.CurrentNode.Type == DemoNodeType.RouteChoice && map.CurrentNode.Layer == 3, "Full flow did not reach the third-layer route choice.");
            map.SelectRoute(Route("route_final_stable").Plan);
            while (map.CurrentNode.Type != DemoNodeType.Boss)
            {
                map.CompleteCurrentNode();
            }
            Require(map.CurrentNode.EncounterId == "enemy_tianjie_avatar", "Final route must reach 天劫化身 by encounter_id.");
            map.CompleteWithResult(true);
            Require(map.CurrentNode.Type == DemoNodeType.Result && map.ResultVictory == true, "Boss victory must enter the one-life result node.");
            Require(map.SelectedRoutes.Count == 3 && map.CompletedNodes.Count > 0, "The run must retain route and node history for settlement.");
            Require(map.SelectedRoutes.All(route => route.Nodes.Any(node => node.IsCompleted)), "Completed route nodes must be reflected in route history.");

            DemoRunState summarizedRun = CreateSwordEmbryoRun();
            summarizedRun.AdvanceElapsedTime(901f);
            summarizedRun.Map.CompleteCurrentNode();
            summarizedRun.Map.CompleteWithResult(false);
            DemoRunSummary summary = summarizedRun.CreateSummary(false, false, 1);
            Require(summary.DurationSeconds == 901f && !string.IsNullOrEmpty(summary.FailureNodeId), "Settlement summary must retain duration and failure location.");
        }

        private static void VerifyConfiguredCombatPacing()
        {
            Require(DemoConfigRepository.TryGetEnemyById("enemy_old_mine_entry", out DemoEnemyDefinition openingEnemy), "Opening enemy config is missing.");
            Require(DemoConfigRepository.TryGetEnemyById("enemy_tianjie_avatar", out DemoEnemyDefinition bossEnemy), "Boss enemy config is missing.");
            Require(openingEnemy.MaxHealth == 105, "Opening battle health must remain tuned for the 30-45 second target.");
            Require(bossEnemy.MaxHealth == 3800 && bossEnemy.IsBoss, "Boss must retain the 150-210 second pacing health budget.");

            List<DemoCard> bossDeck = Cards(
                "sword_focus", "summon_sword", "returning_array", "sword_rain", "sword_array",
                "wanjian_burst", "spirit_draw", "jade_barrier", "sword_tide", "heaven_opening");
            DemoBattleSetup setup = new DemoBattleSetup
            {
                Deck = bossDeck,
                Artifacts = new[] { DemoArtifactType.SwordBox, DemoArtifactType.PurpleGourd },
                Gongfas = new[] { DemoGongfaType.SwordControlArt },
                EnemyName = bossEnemy.Name,
                EnemyHealth = bossEnemy.MaxHealth,
                IsBoss = true,
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = 2,
                InitialHandSize = 4,
                IntroSeconds = 0f,
                RandomSeed = 29
            };

            DemoBattleState battle = SimulateBattle(setup, 230f);
            Require(battle.Phase == DemoBattlePhase.Won, "Representative Wanjian build must defeat the Boss.");
            Require(battle.ElapsedSeconds >= 130f && battle.ElapsedSeconds <= 220f, $"Boss pacing drifted outside the target window: {battle.ElapsedSeconds:0.0}s.");
            Require(battle.MaxSwordsReached >= 5 && battle.HighestBurstDamage >= 20, "Boss fight must demonstrate sword proliferation and a visible burst.");
        }

        private static void VerifyMetaProgression()
        {
            DemoMemoryMetaProgressStore store = new DemoMemoryMetaProgressStore();
            DemoMetaProgress progress = store.Load();
            DemoRunSummary defeat = new DemoRunSummary { Victory = false, DefeatedBoss = false, ReachedLayer = 2 };
            Require(!progress.RecordRun(defeat), "A failed run must not unlock 残剑道痕.");
            Require(progress.CompletedRuns == 1 && progress.BossVictories == 0, "A failed run must still count as a completed attempt.");

            DemoRunSummary victory = new DemoRunSummary { Victory = true, DefeatedBoss = true, ReachedLayer = 3 };
            Require(progress.RecordRun(victory), "The first Boss victory must unlock 残剑道痕.");
            Require(progress.HasUnlock(DemoMetaProgress.BrokenSwordTraceId), "残剑道痕 unlock was not persisted.");
            Require(victory.NewUnlocks.Count == 1, "The first Boss victory must report exactly one new unlock.");

            DemoRunSummary repeatVictory = new DemoRunSummary { Victory = true, DefeatedBoss = true, ReachedLayer = 3 };
            Require(!progress.RecordRun(repeatVictory), "A repeated Boss victory must not duplicate the unlock.");
            Require(progress.CompletedRuns == 3 && progress.BossVictories == 2, "Meta run counters are inconsistent.");
            store.Save(progress);
            Require(store.Load().HasUnlock(DemoMetaProgress.BrokenSwordTraceId), "Meta progress store did not retain unlocks.");

            DemoRunState traceRun = CreateSwordEmbryoRun();
            traceRun.EquippedTraceId = DemoMetaProgress.BrokenSwordTraceId;
            traceRun.OpeningRewardRerolls = 1;
            Require(traceRun.ConsumeOpeningRewardReroll(), "残剑道痕 must grant one opening reward reroll.");
            Require(!traceRun.ConsumeOpeningRewardReroll(), "Opening reward reroll must be consumed exactly once.");
            List<DemoReward> rerolled = new DemoRewardService(903).CreateChoices(
                Context(1, DemoRewardSource.NormalBattle, DemoRewardTier.Standard, DemoRouteRisk.Stable, 903),
                traceRun);
            Require(!rerolled.Select(reward => reward.Name).SequenceEqual(new[] { "凝剑诀", "踏云遁", "聚灵换息" }), "Trace reroll must replace the fixed opening trio with a newly generated P0 choice set.");
        }

        private static DemoBattleState SimulateBattle(DemoBattleSetup setup, float maximumSeconds)
        {
            DemoBattleState battle = new DemoBattleState();
            battle.StartBattle(setup);
            while ((battle.Phase == DemoBattlePhase.Intro || battle.Phase == DemoBattlePhase.Running)
                && battle.ElapsedSeconds < maximumSeconds)
            {
                if (battle.Phase == DemoBattlePhase.Running)
                {
                    int index = ChooseCard(battle);
                    if (index >= 0)
                    {
                        battle.TryPlayCard(index);
                    }
                }

                battle.Tick(0.1f);
            }

            return battle;
        }

        private static int ChooseCard(DemoBattleState battle)
        {
            for (int i = 0; i < battle.Hand.Count; i++)
            {
                DemoCard card = battle.Hand[i];
                if (card.Cost > battle.Energy)
                {
                    continue;
                }

                if (card.ConsumeAllSwordIntent && battle.Player.SwordIntent < 8)
                {
                    continue;
                }

                return i;
            }

            return -1;
        }

        private static DemoBattleSetup BasicSetup(
            IReadOnlyList<DemoCard> deck,
            int enemyHealth,
            int initialHand,
            float drawInterval = 100f,
            float volleyInterval = 100f)
        {
            return new DemoBattleSetup
            {
                Deck = deck,
                EnemyHealth = enemyHealth,
                PlayerHealth = 99,
                PlayerMaxHealth = 99,
                InitialEnergy = 10,
                InitialHandSize = initialHand,
                DrawIntervalSeconds = drawInterval,
                FlyingSwordIntervalSeconds = volleyInterval,
                EnemyIntentMinSeconds = 100f,
                EnemyIntentMaxSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 13
            };
        }

        private static DemoCard TestCard(string id, string name, int damage = 0, int temporarySwords = 0)
        {
            return new DemoCard
            {
                Id = id,
                Name = name,
                Cost = 0,
                Damage = damage,
                TemporarySwords = temporarySwords,
                Type = temporarySwords > 0 ? DemoCardType.FlyingSword : DemoCardType.Attack,
                Style = DemoSwordStyle.Wanjian,
                Quality = DemoQuality.Mortal
            };
        }

        private static DemoBattlePresentationStep ConsumePresentationStep(
            DemoBattleState battle,
            DemoBattlePresentationStepType type)
        {
            while (battle.TryConsumePresentationStep(out DemoBattlePresentationStep step))
            {
                if (step.Type == type)
                {
                    return step;
                }
            }

            throw new InvalidOperationException($"Missing presentation step: {type}.");
        }

        private static DemoRewardContext Context(
            int layer,
            DemoRewardSource source,
            DemoRewardTier tier,
            DemoRouteRisk risk,
            int seed,
            bool allowsFinisher = false,
            bool allowsDivine = false)
        {
            return new DemoRewardContext
            {
                Layer = layer,
                Source = source,
                Tier = tier,
                RouteRisk = risk,
                CurrentHealth = 72,
                MaxHealth = 72,
                AllowsFinisher = allowsFinisher,
                AllowsDivine = allowsDivine,
                HasSeed = true,
                Seed = seed
            };
        }

        private static DemoRunState CreateSwordEmbryoRun()
        {
            DemoRootDefinition root = DemoConfigRepository.GetDefaultRoots(4).Single();
            DemoJourneyVesselDefinition vessel = DemoConfigRepository.GetJourneyVesselsForRoot(root.Id, 4, true).Single(candidate => candidate.IsAvailable);
            DemoRunState run = new DemoRunState();
            run.SetRoot(root);
            run.SetVessel(vessel);
            return run;
        }

        private static DemoReward Focus(IReadOnlyList<DemoReward> rewards)
        {
            return rewards.Single(reward => reward.Slot == DemoRewardSlot.Focus);
        }

        private static bool IsP0Reward(DemoReward reward)
        {
            return reward.Type == DemoRewardType.Card
                || reward.Type == DemoRewardType.Gongfa
                || reward.Type == DemoRewardType.Artifact
                || reward.Type == DemoRewardType.Upgrade
                || reward.Type == DemoRewardType.Heal;
        }

        private static void ApplyReward(DemoRunState run, DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Card:
                    run.AddCard(reward.Card);
                    break;
                case DemoRewardType.Gongfa:
                    run.LearnGongfa(reward.GongfaType);
                    break;
                case DemoRewardType.Artifact:
                    run.AddArtifact(reward.ArtifactType);
                    break;
                case DemoRewardType.Upgrade:
                    run.UpgradeEnergy();
                    break;
                case DemoRewardType.Heal:
                    run.Heal(18);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported P0 reward: {reward.Type}");
            }

            run.RecordRewardSelection(DemoRewardService.IsFocusComponent(reward));
        }

        private static DemoRoutePlanDefinition Route(string routeId)
        {
            Require(DemoConfigRepository.TryGetRoutePlan(routeId, out DemoRoutePlanDefinition route), $"Missing route: {routeId}");
            return route;
        }

        private static IEnumerable<DemoNodeType> NodeTypes(DemoRoutePlanDefinition route)
        {
            return route.Plan.Nodes.Select(node => node.Type);
        }

        private static void SelectAndCompleteToChoice(DemoMapRun map, string routeId)
        {
            map.SelectRoute(Route(routeId).Plan);
            int guard = 12;
            while (map.CurrentNode.Type != DemoNodeType.RouteChoice && guard-- > 0)
            {
                map.CompleteCurrentNode();
            }

            Require(guard > 0, $"{routeId} did not reach the next route choice.");
        }

        private static List<DemoCard> Cards(params string[] ids)
        {
            List<DemoCard> cards = new List<DemoCard>();
            foreach (string id in ids)
            {
                Require(DemoConfigRepository.TryCreateCard(id, out DemoCard card), $"Missing card config: {id}");
                cards.Add(card);
            }
            return cards;
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
