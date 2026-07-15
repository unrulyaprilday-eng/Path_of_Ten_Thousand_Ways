using System;
using System.Collections.Generic;
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
            Require(swordEmbryo.StarterPoolId == "starter_sword_embryo", "残剑胚 must reference the basic starter pool.");

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
            Require(DemoConfigRepository.TryGetRegion("region_ancestral_vault", out DemoRegionDefinition lockedRegion), "Locked region config is missing.");
            Require(!run.TrySetFirstRegion(lockedRegion), "An unavailable map must not be enterable.");
            Require(DemoConfigRepository.TryGetRegion("region_old_mine", out DemoRegionDefinition oldMine) && run.TrySetFirstRegion(oldMine), "旧矿地窟 must be enterable.");
            Require(run.Deck.Count == 5, "残剑胚 must start with exactly five basic cards.");
            Require(run.Deck.Count(card => card.Id == "sword_slash") == 2, "Starter deck must contain two 剑气斩.");
            Require(run.Deck.Count(card => card.Id == "guard_step") == 2, "Starter deck must contain two 剑遁.");
            Require(run.Deck.Count(card => card.Id == "cloud_step") == 1, "Starter deck must contain one 踏云遁.");
            Require(run.Deck.All(card => card.Id != "sword_focus"), "凝剑诀 must be a first-battle reward, not a starter card.");
            Require(run.MainGongfa == DemoGongfaType.None, "The run must start without a main gongfa.");
            Require(run.Artifacts.Count == 0 && run.Relics.Count == 0, "The run must start without formal artifacts or relics.");
        }

        private static void VerifyRealtimeBattleLoop()
        {
            DemoCard strike = TestCard("test_strike", "试锋", damage: 3);
            DemoBattleState flow = new DemoBattleState();
            flow.StartBattle(new DemoBattleSetup
            {
                Deck = Enumerable.Range(0, 8).Select(_ => strike).ToList(),
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

            float fullHandTimer = flow.DrawTimer;
            flow.Tick(1.1f);
            Require(flow.Energy >= 1, "Energy must regenerate continuously at one per second.");
            Require(flow.Hand.Count == 7, "A full hand must not draw or burn the next card.");
            Require(Math.Abs(flow.DrawTimer - fullHandTimer) < 0.01f, "Draw timer must pause while the hand is full.");

            int enemyBefore = flow.Enemy.Health;
            Require(flow.TryPlayCard(0), "A playable card must resolve immediately.");
            Require(flow.Enemy.Health == enemyBefore - 3, "Immediate card damage did not resolve.");
            Require(flow.DiscardPile.Count == 1 && flow.Hand.Count == 6, "Played card must enter discard immediately.");
            flow.Tick(1.1f);
            Require(flow.Hand.Count == 7, "Drawing must resume after hand space opens.");

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
            intent.StartBattle(intentSetup);
            intent.Tick(1.01f);
            Require(intent.EnemyActionCount == 1, "Enemy intent must resolve on its realtime countdown.");

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
            Require(NodeTypes(buildOne).SequenceEqual(new[] { DemoNodeType.Training, DemoNodeType.Shop, DemoNodeType.Battle, DemoNodeType.RouteChoice }), "Layer one build route order changed.");
            Require(riskyOne.Plan.Nodes.Count(node => node.Type == DemoNodeType.Battle) > stableOne.Plan.Nodes.Count(node => node.Type == DemoNodeType.Battle), "Risky route must front-load more combat rewards.");
            Require(buildOne.Plan.Nodes.Any(node => node.RewardProfileId == "reward_layer1_build" || node.ActionProfileId == "action_training_focus_l1"), "Build route must contain deterministic focus filling.");

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
