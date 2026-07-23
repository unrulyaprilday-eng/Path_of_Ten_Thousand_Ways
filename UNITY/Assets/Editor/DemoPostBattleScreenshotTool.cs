using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;
using PathOfTenThousandWays.Demo.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.EditorTools
{
    [InitializeOnLoad]
    public static class DemoPostBattleScreenshotTool
    {
        private const string ActiveKey = "PathOfTenThousandWays.PostBattleCapture.Active";
        private const string StageKey = "PathOfTenThousandWays.PostBattleCapture.Stage";
        private const string OpeningBattleOnlyKey = "PathOfTenThousandWays.PostBattleCapture.OpeningBattleOnly";
        private const string JourneyFlowKey = "PathOfTenThousandWays.PostBattleCapture.JourneyFlow";
        private const int SettleFrames = 18;
        private const int CaptureWidth = 1920;
        private const int CaptureHeight = 1080;

        private static int waitFrames;
        private static int victoryReadyPolls;
        private static CaptureSession captureSession;

        static DemoPostBattleScreenshotTool()
        {
            if (EditorPrefs.GetBool(ActiveKey, false))
            {
                EditorApplication.update -= UpdateCapture;
                EditorApplication.update += UpdateCapture;
            }
        }

        public static void Capture()
        {
            EditorPrefs.SetBool(ActiveKey, true);
            EditorPrefs.SetBool(OpeningBattleOnlyKey, false);
            EditorPrefs.SetBool(JourneyFlowKey, false);
            EditorPrefs.SetInt(StageKey, 0);
            waitFrames = 0;
            victoryReadyPolls = 0;
            EditorApplication.update -= UpdateCapture;
            EditorApplication.update += UpdateCapture;
            EditorApplication.isPlaying = true;
        }

        public static void CaptureOpeningBattle()
        {
            EditorPrefs.SetBool(ActiveKey, true);
            EditorPrefs.SetBool(OpeningBattleOnlyKey, true);
            EditorPrefs.SetBool(JourneyFlowKey, false);
            EditorPrefs.SetInt(StageKey, 0);
            waitFrames = 0;
            victoryReadyPolls = 0;
            EditorApplication.update -= UpdateCapture;
            EditorApplication.update += UpdateCapture;
            EditorApplication.isPlaying = true;
        }

        public static void CaptureJourney()
        {
            EditorPrefs.SetBool(ActiveKey, true);
            EditorPrefs.SetBool(OpeningBattleOnlyKey, false);
            EditorPrefs.SetBool(JourneyFlowKey, true);
            EditorPrefs.SetInt(StageKey, 0);
            waitFrames = 0;
            victoryReadyPolls = 0;
            EditorApplication.update -= UpdateCapture;
            EditorApplication.update += UpdateCapture;
            EditorApplication.isPlaying = true;
        }

        private static void UpdateCapture()
        {
            if (!EditorPrefs.GetBool(ActiveKey, false))
            {
                EditorApplication.update -= UpdateCapture;
                return;
            }

            if (!EditorApplication.isPlaying)
            {
                CancelCaptureSession();
                if (EditorPrefs.GetInt(StageKey, 0) >= 99)
                {
                    Finish(0);
                }

                return;
            }

            try
            {
                if (captureSession != null)
                {
                    if (captureSession.Tick())
                    {
                        captureSession.Dispose();
                        captureSession = null;
                        AdvanceStage();
                        waitFrames = 0;
                    }

                    return;
                }

                DemoGameController controller = UnityEngine.Object.FindAnyObjectByType<DemoGameController>();
                if (controller == null)
                {
                    return;
                }

                Screen.SetResolution(CaptureWidth, CaptureHeight, false);
                if (++waitFrames < SettleFrames)
                {
                    return;
                }

                waitFrames = 0;
                if (EditorPrefs.GetBool(OpeningBattleOnlyKey, false))
                {
                    UpdateOpeningBattleCapture(controller);
                    return;
                }
                if (EditorPrefs.GetBool(JourneyFlowKey, false))
                {
                    UpdateJourneyCapture(controller);
                    return;
                }

                int stage = EditorPrefs.GetInt(StageKey, 0);
                switch (stage)
                {
                    case 0:
                        BeginCapture("flow_home_commercial.png");
                        break;
                    case 1:
                        AdvanceToOpeningRoot(controller);
                        BeginCapture("flow_opening_root_commercial.png");
                        break;
                    case 2:
                        ClaimOpeningAvailable(controller, DemoRewardType.Root);
                        BeginCapture("flow_opening_vessel_commercial.png");
                        break;
                    case 3:
                        ClaimOpeningAvailable(controller, DemoRewardType.Vessel);
                        BeginCapture("flow_opening_region_commercial.png");
                        break;
                    case 4:
                        ClaimOpeningAvailable(controller, DemoRewardType.OpeningScene);
                        BeginCapture("battle_opening_realtime.png");
                        break;
                    case 5:
                        BeginCapture("battle_opening_realtime_1280x720.png", 1280, 720);
                        break;
                    case 6:
                        CompleteCurrentBattle(controller, true);
                        SelectFirstCommercialChoice();
                        BeginCapture("post_battle_reward_commercial.png");
                        break;
                    case 7:
                        BeginCapture("post_battle_reward_commercial_1280x720.png", 1280, 720);
                        break;
                    case 8:
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("post_battle_route_commercial.png");
                        break;
                    case 9:
                        BeginCapture("post_battle_route_commercial_1280x720.png", 1280, 720);
                        break;
                    case 10:
                        ClaimRoute(controller, "route_branch_stable");
                        BeginCapture("flow_encounter_intro_commercial.png");
                        break;
                    case 11:
                        controller.BeginCurrentEncounter();
                        CompleteCurrentBattle(controller, true);
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_training_commercial.png");
                        break;
                    case 12:
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_route_layer2_commercial.png");
                        break;
                    case 13:
                        ClaimRoute(controller, "route_middle_stable");
                        controller.BeginCurrentEncounter();
                        CompleteCurrentBattle(controller, true);
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_training_layer2_commercial.png");
                        break;
                    case 14:
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_route_layer3_commercial.png");
                        break;
                    case 15:
                        ClaimRoute(controller, "route_final_stable");
                        controller.BeginCurrentEncounter();
                        CompleteCurrentBattle(controller, true);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_final_reward_commercial.png");
                        break;
                    case 16:
                        ClaimPreferredReward(controller);
                        SelectFirstCommercialChoice();
                        BeginCapture("flow_preparation_commercial.png");
                        break;
                    case 17:
                        ClaimPreferredReward(controller);
                        BeginCapture("flow_boss_gate_commercial.png");
                        break;
                    case 18:
                        PrepareResultMetrics(controller, true);
                        controller.BeginCurrentEncounter();
                        CompleteCurrentBattle(controller, true);
                        BeginCapture("run_result_victory.png");
                        break;
                    case 19:
                        controller.StartNextRun();
                        AdvanceOpeningToBattle(controller);
                        PrepareResultMetrics(controller, false);
                        CompleteCurrentBattle(controller, false);
                        BeginCapture("run_result_defeat.png");
                        break;
                    case 20:
                        EditorPrefs.SetInt(StageKey, 99);
                        EditorApplication.isPlaying = false;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown full-flow capture stage: " + stage);
                }
            }
            catch (Exception exception)
            {
                CancelCaptureSession();
                Debug.LogException(exception);
                Finish(1);
            }
        }

        private static void UpdateOpeningBattleCapture(DemoGameController controller)
        {
            int stage = EditorPrefs.GetInt(StageKey, 0);
            switch (stage)
            {
                case 0:
                    AdvanceOpeningToBattle(controller);
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.Intro);
                    AdvanceStage();
                    break;
                case 1:
                    BeginCapture("battle_opening_intro_commercial.png");
                    break;
                case 2:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.Playable);
                    AdvanceStage();
                    break;
                case 3:
                    BeginCapture("battle_opening_running_commercial.png");
                    break;
                case 4:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.Playable);
                    BeginCapture("battle_opening_running_commercial_1280x720.png", 1280, 720);
                    break;
                case 5:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.Playable);
                    HoverOpeningBattleCard();
                    AdvanceStage();
                    break;
                case 6:
                    BeginCapture("battle_opening_playable_hover_commercial.png");
                    break;
                case 7:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.EnergyBlocked);
                    AdvanceStage();
                    break;
                case 8:
                    BeginCapture("battle_opening_energy_blocked_commercial.png");
                    break;
                case 9:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.IntentWarning);
                    AdvanceStage();
                    break;
                case 10:
                    BeginCapture("battle_opening_enemy_intent_commercial.png");
                    break;
                case 11:
                    StartOpeningBattlePreview(controller, OpeningBattleCaptureState.Victory);
                    victoryReadyPolls = 0;
                    AdvanceStage();
                    break;
                case 12:
                    if (!IsOpeningVictoryPresentationReady())
                    {
                        if (++victoryReadyPolls > 40)
                        {
                            throw new InvalidOperationException("Opening victory dissolve did not become visible before capture.");
                        }
                        break;
                    }
                    BeginCapture("battle_opening_victory_commercial.png");
                    break;
                case 13:
                    SetPrivateField(controller, "battleResultHandled", false);
                    SetPrivateField(controller, "battleOutcomeDelay", 1.19f);
                    AdvanceStage();
                    break;
                case 14:
                    if (controller.HasBattle || controller.CurrentRewards.Count != 3)
                    {
                        throw new InvalidOperationException("Opening battle victory did not enter the three-choice reward flow after its presentation hold.");
                    }

                    Debug.Log("Opening battle victory transition check passed.");
                    EditorPrefs.SetInt(StageKey, 99);
                    EditorApplication.isPlaying = false;
                    break;
                default:
                    throw new InvalidOperationException("Unknown opening battle capture stage: " + stage);
            }
        }

        private static void UpdateJourneyCapture(DemoGameController controller)
        {
            int stage = EditorPrefs.GetInt(StageKey, 0);
            switch (stage)
            {
                case 0:
                    BeginCapture("journey_home.png");
                    break;
                case 1:
                    controller.BeginOpeningStory();
                    RefreshJourneyView();
                    BeginCapture("journey_opening_story.png");
                    break;
                case 2:
                    if (!controller.CompleteOpeningStory())
                    {
                        throw new InvalidOperationException("Opening story did not reach the region choice.");
                    }
                    RefreshJourneyView();
                    BeginCapture("journey_region_choice.png");
                    break;
                case 3:
                    if (!DemoConfigRepository.TryGetRegion("region_old_mine", out DemoRegionDefinition oldMine)
                        || !controller.BeginConfiguredJourney(oldMine, 42017))
                    {
                        throw new InvalidOperationException("The deterministic old mine journey could not begin: " + controller.JourneyError);
                    }
                    RefreshJourneyView();
                    BeginCapture("journey_act1_map.png");
                    break;
                case 4:
                    SelectCurrentJourneyFrontier(controller);
                    RefreshJourneyView();
                    BeginCapture("journey_entry_scene.png");
                    break;
                case 5:
                    AdvanceJourneyUntil(controller, node => node.ContentId == "event_miner_spirit_first");
                    RefreshJourneyView();
                    BeginCapture("journey_miner_spirit_event.png");
                    break;
                case 6:
                    CompleteJourneyScene(controller, "choice_miner_spirit_help");
                    AdvanceJourneyUntil(controller, node => node.ActIndex == 1 && node.IsCombat
                        && node.Type != DemoJourneyNodeType.MiniBoss);
                    controller.BeginCurrentEncounter();
                    RefreshBattleViews();
                    BeginCapture("journey_battle_multi_1920x1080.png");
                    break;
                case 7:
                    BeginCapture("journey_battle_multi_1280x720.png", 1280, 720);
                    break;
                case 8:
                    CompleteCurrentBattle(controller, true);
                    AdvanceJourneyUntil(controller, node => node.Type == DemoJourneyNodeType.MiniBoss
                        && node.ActIndex == 1);
                    controller.BeginCurrentEncounter();
                    RefreshBattleViews();
                    BeginCapture("journey_miniboss_ironback.png");
                    break;
                case 9:
                    CompleteCurrentBattle(controller, true);
                    AdvanceJourneyUntil(controller, node => node.Type == DemoJourneyNodeType.MiniBoss
                        && node.ActIndex == 2);
                    controller.BeginCurrentEncounter();
                    RefreshBattleViews();
                    BeginCapture("journey_miniboss_sword_eating_xiao.png");
                    break;
                case 10:
                    CompleteCurrentBattle(controller, true);
                    AdvanceJourneyUntil(controller, node => node.Type == DemoJourneyNodeType.Breakthrough);
                    RefreshJourneyView();
                    BeginCapture("journey_breakthrough.png");
                    break;
                case 11:
                    CompleteJourneyScene(controller, "foundation_stable");
                    RefreshJourneyView();
                    BeginCapture("journey_act3_map.png");
                    break;
                case 12:
                    AdvanceJourneyUntil(controller, node => node.ActIndex == 3
                        && node.Type == DemoJourneyNodeType.Battle);
                    controller.BeginCurrentEncounter();
                    RefreshBattleViews();
                    BeginCapture("journey_battle_foundation_multi.png");
                    break;
                case 13:
                    CompleteCurrentBattle(controller, true);
                    AdvanceJourneyUntil(controller, node => node.Type == DemoJourneyNodeType.Boss);
                    RefreshJourneyView();
                    BeginCapture("journey_boss_gate.png");
                    break;
                case 14:
                    controller.BeginCurrentEncounter();
                    StartJourneySwordPuppetPreview(controller);
                    BeginCapture("journey_boss_armor.png");
                    break;
                case 15:
                    PlayJourneyBossPartBreak(controller, DemoBattleState.BossPhaseXuantieContractSpike);
                    BeginCapture("journey_boss_contract_spike.png");
                    break;
                case 16:
                    PlayJourneyBossPartBreak(controller, DemoBattleState.BossPhaseXuantieCore);
                    BeginCapture("journey_boss_core.png");
                    break;
                case 17:
                    SetPrivateField(controller, "battleResultHandled", false);
                    if (!controller.Battle.TryPlayCard(0)
                        || controller.Battle.Phase != DemoBattlePhase.Won)
                    {
                        throw new InvalidOperationException("The deterministic sword puppet core did not resolve.");
                    }
                    InvokePrivate(controller, "HandleBattleWon", null);
                    RefreshJourneyView();
                    BeginCapture("journey_run_result.png");
                    break;
                case 18:
                    Debug.Log("Journey commercial flow screenshot check passed.");
                    EditorPrefs.SetInt(StageKey, 99);
                    EditorPrefs.SetBool(JourneyFlowKey, false);
                    EditorApplication.isPlaying = false;
                    break;
                default:
                    throw new InvalidOperationException("Unknown journey capture stage: " + stage);
            }
        }

        private static void AdvanceJourneyUntil(
            DemoGameController controller,
            Func<DemoJourneyNode, bool> predicate)
        {
            int guard = 96;
            while (guard-- > 0)
            {
                if (controller.HasBattle)
                {
                    CompleteCurrentBattle(controller, true);
                    continue;
                }

                if (controller.FlowPhase == DemoFlowPhase.NodeScene
                    || controller.FlowPhase == DemoFlowPhase.Breakthrough)
                {
                    CompleteJourneyScene(controller);
                    continue;
                }

                if (controller.FlowPhase == DemoFlowPhase.EncounterIntro
                    || controller.FlowPhase == DemoFlowPhase.BossGate)
                {
                    DemoJourneyNode pending = controller.PendingJourneyNode;
                    if (pending != null && predicate(pending)) return;
                    controller.BeginCurrentEncounter();
                    CompleteCurrentBattle(controller, true);
                    continue;
                }

                if (controller.FlowPhase != DemoFlowPhase.JourneyMap
                    || controller.JourneySnapshot == null
                    || controller.JourneyGraph == null)
                {
                    throw new InvalidOperationException("Journey traversal stopped in " + controller.FlowPhase + ".");
                }

                List<DemoJourneyNode> frontier = controller.JourneySnapshot.ReachableNodeIds
                    .Select(id => controller.JourneyGraph.TryGetNode(id, out DemoJourneyNode node) ? node : null)
                    .Where(node => node != null)
                    .ToList();
                DemoJourneyNode next = frontier.FirstOrDefault(predicate) ?? frontier.FirstOrDefault();
                if (next == null || !controller.SelectJourneyNode(next.NodeId))
                {
                    throw new InvalidOperationException("Journey traversal could not select its next frontier node: " + controller.JourneyError);
                }
                if (predicate(next)) return;
            }

            throw new InvalidOperationException("Journey traversal exceeded its deterministic guard.");
        }

        private static void SelectCurrentJourneyFrontier(DemoGameController controller)
        {
            string nodeId = controller.JourneySnapshot?.ReachableNodeIds?.FirstOrDefault();
            if (string.IsNullOrEmpty(nodeId) || !controller.SelectJourneyNode(nodeId))
            {
                throw new InvalidOperationException("The journey map had no selectable frontier.");
            }
        }

        private static void CompleteJourneyScene(DemoGameController controller, string preferredChoiceId = null)
        {
            IReadOnlyList<DemoJourneyChoice> choices = controller.CurrentJourneyChoices;
            DemoJourneyChoice choice = choices.FirstOrDefault(item => string.Equals(
                    item.ChoiceId,
                    preferredChoiceId,
                    StringComparison.Ordinal))
                ?? choices.FirstOrDefault(item => item.IsRecommended)
                ?? choices.FirstOrDefault();
            if (choice == null || !controller.ChooseJourneyOption(choice.ChoiceId))
            {
                throw new InvalidOperationException("Journey scene had no committable choice: " + controller.JourneyError);
            }
        }

        private static void StartJourneySwordPuppetPreview(DemoGameController controller)
        {
            DemoCard breakPart = new DemoCard
            {
                Id = "capture_break_sword_puppet_part",
                Name = "断契试剑",
                Type = DemoCardType.Attack,
                Style = DemoSwordStyle.Wanjian,
                Cost = 0,
                Damage = 160
            };
            controller.Battle.StartBattle(new DemoBattleSetup
            {
                EnemyId = "enemy_xuantie_mine_sword_puppet",
                EnemyName = "玄铁镇矿剑傀",
                IsBoss = true,
                Enemies = new[]
                {
                    new DemoBattleEnemySetup { CombatantId = "capture_puppet_armor", DefinitionId = "target_xuantie_armor", Name = "玄铁甲片", PositionId = "boss_upper_armor", Depth = 0, MaxHealth = 120, ThreatPriority = 5 },
                    new DemoBattleEnemySetup { CombatantId = "capture_puppet_spike", DefinitionId = "target_contract_spike", Name = "朱砂契钉", PositionId = "boss_contract_spike", Depth = 1, MaxHealth = 120, ThreatPriority = 4 },
                    new DemoBattleEnemySetup { CombatantId = "capture_puppet_core", DefinitionId = "target_sword_furnace_core", Name = "剑炉核心", PositionId = "boss_furnace_core", Depth = 2, MaxHealth = 120, ThreatPriority = 3 }
                },
                Deck = new[] { breakPart, breakPart, breakPart },
                PlayerMaxHealth = 84,
                PlayerHealth = 84,
                InitialEnergy = 3,
                MaxEnergy = 7,
                InitialHandSize = 3,
                HandLimit = 6,
                DrawIntervalSeconds = 100f,
                FlyingSwordIntervalSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 42017
            });
            SetPrivateField(controller, "battleResultHandled", false);
            SetPrivateField(controller, "battleOutcomeDelay", 0f);
            RefreshBattleViews();
        }

        private static void PlayJourneyBossPartBreak(
            DemoGameController controller,
            string expectedPhaseId)
        {
            if (!controller.Battle.TryPlayCard(0)
                || !string.Equals(controller.Battle.BossPhaseId, expectedPhaseId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException("Sword puppet preview did not reach phase " + expectedPhaseId + ".");
            }
            RefreshBattleViews();
        }

        private static void RefreshJourneyView()
        {
            DemoCommercialJourneyView view = UnityEngine.Object.FindAnyObjectByType<DemoCommercialJourneyView>(FindObjectsInactive.Include);
            view?.RefreshNow();
        }

        private static void RefreshBattleViews()
        {
            DemoBattleSceneView battleScene = UnityEngine.Object.FindAnyObjectByType<DemoBattleSceneView>(FindObjectsInactive.Include);
            DemoBattleHudView battleHud = UnityEngine.Object.FindAnyObjectByType<DemoBattleHudView>(FindObjectsInactive.Include);
            battleScene?.RefreshForCurrentBattle(false);
            battleHud?.RefreshNow();
        }

        private static void HoverOpeningBattleCard()
        {
            DemoBattleCardView[] cards = UnityEngine.Object.FindObjectsByType<DemoBattleCardView>(FindObjectsSortMode.None);
            if (cards.Length != 2)
            {
                throw new InvalidOperationException("Expected exactly two visible story techniques before hover capture.");
            }

            DemoBattleCardView card = cards
                .OrderBy(view => view.transform.GetSiblingIndex())
                .ElementAt(cards.Length / 2);
            card.OnPointerEnter(null);
        }

        private static bool IsOpeningVictoryPresentationReady()
        {
            RectTransform enemyRoot = UnityEngine.Object.FindObjectsByType<RectTransform>(
                    FindObjectsInactive.Exclude,
                    FindObjectsSortMode.None)
                .FirstOrDefault(rect => rect.name == "EnemyRoot");
            CanvasGroup enemyGroup = enemyRoot != null ? enemyRoot.GetComponent<CanvasGroup>() : null;
            return enemyGroup != null && enemyGroup.alpha <= 0.20f;
        }

        private static void ClaimFirstAvailable(DemoGameController controller)
        {
            if (controller.CurrentRewards.Count == 0)
            {
                throw new InvalidOperationException("Expected at least one reward choice.");
            }

            controller.ClaimRewardAt(0);
        }

        private static void AdvanceToOpeningRoot(DemoGameController controller)
        {
            controller.StartNewRun();
            if (controller.FlowPhase == DemoFlowPhase.OpeningTrace)
            {
                int noTrace = Enumerable.Range(0, controller.CurrentRewards.Count)
                    .FirstOrDefault(index => string.IsNullOrEmpty(controller.CurrentRewards[index].TraceId));
                controller.ClaimRewardAt(noTrace);
            }

            if (controller.FlowPhase != DemoFlowPhase.OpeningRoot)
            {
                throw new InvalidOperationException("New run did not reach the root ceremony.");
            }
        }

        private static void ClaimOpeningAvailable(DemoGameController controller, DemoRewardType expectedType)
        {
            int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                .FirstOrDefault(i => IsOpeningRewardAvailable(controller.CurrentRewards[i])
                    && (controller.CurrentRewards[i].Type == expectedType
                        || expectedType == DemoRewardType.Vessel && controller.CurrentRewards[i].Type == DemoRewardType.Journey));
            DemoReward reward = controller.CurrentRewards.Count > index ? controller.CurrentRewards[index] : null;
            if (reward == null || !IsOpeningRewardAvailable(reward))
            {
                throw new InvalidOperationException("Opening ceremony had no available " + expectedType + " choice.");
            }

            controller.ClaimRewardAt(index);
        }

        private static void SelectFirstCommercialChoice()
        {
            DemoCommercialJourneyView view = UnityEngine.Object.FindAnyObjectByType<DemoCommercialJourneyView>(FindObjectsInactive.Include);
            if (view == null)
            {
                throw new InvalidOperationException("Commercial journey view was unavailable before selection preview.");
            }

            view.RefreshNow();
            Button choice = UnityEngine.Object.FindObjectsByType<Button>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .FirstOrDefault(button => button.name.StartsWith("RewardCard_", StringComparison.Ordinal)
                    || button.name.StartsWith("RouteCard_", StringComparison.Ordinal));
            if (choice == null)
            {
                throw new InvalidOperationException("Commercial journey page had no selectable choice card.");
            }

            choice.onClick.Invoke();
        }

        private static void ClaimPreferredReward(DemoGameController controller)
        {
            if (controller.CurrentRewards.Count == 0)
            {
                throw new InvalidOperationException("Expected a reward before continuing the authentic flow.");
            }

            int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                .FirstOrDefault(i => controller.CurrentRewards[i].Slot == DemoRewardSlot.Focus);
            controller.ClaimRewardAt(index);
        }

        private static void ClaimRoute(DemoGameController controller, string routeId)
        {
            if (controller.FlowPhase != DemoFlowPhase.RouteChoice)
            {
                throw new InvalidOperationException("Expected route choice before selecting " + routeId + ".");
            }

            int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                .FirstOrDefault(i => string.Equals(controller.CurrentRewards[i].RoutePlan?.Id, routeId, StringComparison.OrdinalIgnoreCase));
            DemoReward reward = controller.CurrentRewards.Count > index ? controller.CurrentRewards[index] : null;
            if (reward?.RoutePlan == null || !string.Equals(reward.RoutePlan.Id, routeId, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Route choice did not contain " + routeId + ".");
            }

            controller.ClaimRewardAt(index);
        }

        private static void CompleteCurrentBattle(DemoGameController controller, bool victory)
        {
            if (!controller.HasBattle)
            {
                throw new InvalidOperationException("Authentic flow skip expected an active battle.");
            }

            InvokePrivate(controller, victory ? "HandleBattleWon" : "HandleBattleLost", null);
            if (victory && controller.HasBattle)
            {
                throw new InvalidOperationException("Battle victory skip did not leave the battle state.");
            }

            if (!victory && (!controller.HasRunResult || controller.FlowPhase != DemoFlowPhase.RunResult))
            {
                throw new InvalidOperationException("Battle defeat skip did not reach the authentic run result.");
            }
        }

        private static void PrepareResultMetrics(DemoGameController controller, bool victory)
        {
            float targetDuration = victory ? 14f * 60f + 20f : 2f * 60f + 35f;
            controller.Run.AdvanceElapsedTime(Mathf.Max(0f, targetDuration - controller.Run.ElapsedSeconds));
            controller.Run.RecordSwordCount(victory ? 9 : 3);
            controller.Run.RecordBurstDamage(victory ? 148 : 18);
        }


        private static void AdvanceOpeningToBattle(DemoGameController controller)
        {
            if (controller.Run.Map.CurrentNode.Type == DemoNodeType.Start && controller.CurrentRewards.Count == 0)
            {
                controller.AdvanceUtilityNode();
            }

            int guard = 8;
            while (!controller.HasBattle && guard-- > 0)
            {
                if (controller.CurrentRewards.Count == 0)
                {
                    throw new InvalidOperationException("Opening flow stopped before a selectable choice.");
                }

                int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                    .FirstOrDefault(i => IsOpeningRewardAvailable(controller.CurrentRewards[i]));
                controller.ClaimRewardAt(index);
            }

            if (!controller.HasBattle)
            {
                throw new InvalidOperationException("Opening selections did not reach the realtime entry battle.");
            }
        }

        private static bool IsOpeningRewardAvailable(DemoReward reward)
        {
            if (reward == null)
            {
                return false;
            }

            if (reward.Type == DemoRewardType.Root)
            {
                return reward.Root != null && reward.Root.IsAvailable;
            }

            if (reward.Type == DemoRewardType.Vessel || reward.Type == DemoRewardType.Journey)
            {
                return reward.Vessel != null
                    ? reward.Vessel.IsAvailable
                    : reward.JourneyLine != null && reward.JourneyLine.IsAvailable;
            }

            if (reward.Type == DemoRewardType.OpeningScene)
            {
                return reward.Region != null && reward.Region.IsAvailable;
            }

            return true;
        }

        private static void SkipOpeningBattle(DemoGameController controller)
        {
            if (!controller.HasBattle)
            {
                throw new InvalidOperationException("Opening battle was not active before capture skip.");
            }

            InvokePrivate(controller, "HandleBattleWon", null);
            if (controller.CurrentRewards.Count != 3)
            {
                throw new InvalidOperationException("Opening battle did not enter the three-choice reward state.");
            }
        }

        private static void StartOpeningBattlePreview(
            DemoGameController controller,
            OpeningBattleCaptureState captureState)
        {
            if (!DemoConfigRepository.TryCreateDeckFromPool("starter_story_sword_package", out List<DemoCard> storyTechniques)
                || storyTechniques.Count != 2)
            {
                throw new InvalidOperationException("Story starting package did not provide exactly two active techniques.");
            }

            DemoCard swordArt = storyTechniques.Single(card => card.Id == "technique_incomplete_sword_scroll");
            int initialEnergy = captureState == OpeningBattleCaptureState.EnergyBlocked
                || captureState == OpeningBattleCaptureState.IntentWarning ? 0 : 3;
            float introSeconds = captureState == OpeningBattleCaptureState.Intro ? 30f : 0f;
            float intentSeconds = captureState == OpeningBattleCaptureState.IntentWarning ? 30f : 6.4f;
            bool frozenResourceState = captureState == OpeningBattleCaptureState.EnergyBlocked
                || captureState == OpeningBattleCaptureState.IntentWarning;

            if (captureState == OpeningBattleCaptureState.Victory)
            {
                swordArt.Cost = 0;
                swordArt.Damage = 999;
            }

            controller.Battle.StartBattle(new DemoBattleSetup
            {
                Deck = storyTechniques,
                EnemyId = "enemy_old_mine_entry",
                EnemyName = "旧矿入口遭遇",
                EnemyHealth = captureState == OpeningBattleCaptureState.Victory ? 72 : 105,
                PlayerName = "凌云剑修",
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = initialEnergy,
                MaxEnergy = 5,
                EnergyRegenerationPerSecond = frozenResourceState ? 0f : 1f,
                InitialHandSize = 2,
                HandLimit = 6,
                DrawIntervalSeconds = captureState == OpeningBattleCaptureState.IntentWarning ? 100f : 4f,
                FlyingSwordIntervalSeconds = captureState == OpeningBattleCaptureState.IntentWarning ? 100f : 2.2f,
                EnemyIntentMinSeconds = intentSeconds,
                EnemyIntentMaxSeconds = intentSeconds,
                IntroSeconds = introSeconds,
                IsOpeningBattle = true,
                RandomSeed = 41
            });
            controller.BattleSpeed = 1f;
            SetPrivateField(controller, "battleResultHandled", false);
            SetPrivateField(controller, "battleOutcomeDelay", 0f);

            if (captureState == OpeningBattleCaptureState.IntentWarning)
            {
                controller.Battle.Tick(28.8f);
            }
            else if (captureState == OpeningBattleCaptureState.Victory)
            {
                int swordArtIndex = Enumerable.Range(0, controller.Battle.Hand.Count)
                    .FirstOrDefault(index => controller.Battle.Hand[index].Id == "technique_incomplete_sword_scroll");
                if (!controller.Battle.TryPlayCard(swordArtIndex))
                {
                    throw new InvalidOperationException("Opening battle victory preview card could not be played.");
                }

                SetPrivateField(controller, "battleResultHandled", true);
            }

            DemoBattleSceneView battleScene = UnityEngine.Object.FindAnyObjectByType<DemoBattleSceneView>();
            if (battleScene == null)
            {
                throw new InvalidOperationException("Opening battle scene view was unavailable for deterministic refresh.");
            }

            battleScene.RefreshForCurrentBattle(captureState == OpeningBattleCaptureState.Intro);

            DemoBattleHudView battleHud = UnityEngine.Object.FindAnyObjectByType<DemoBattleHudView>(FindObjectsInactive.Include);
            if (battleHud == null)
            {
                throw new InvalidOperationException("Opening battle HUD was unavailable for deterministic refresh.");
            }

            battleHud.RefreshNow();
        }

        private static void StartMultiSwordPreview(DemoGameController controller)
        {
            DemoCard summon = new DemoCard
            {
                Id = "capture_multi_sword",
                Name = "剑影分光",
                Cost = 0,
                Type = DemoCardType.FlyingSword,
                Style = DemoSwordStyle.Wanjian,
                Quality = DemoQuality.Earth,
                TemporarySwords = 6
            };
            controller.Battle.StartBattle(new DemoBattleSetup
            {
                Deck = new[] { summon },
                EnemyName = "劫云守卫",
                EnemyHealth = 9999,
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = 2,
                InitialHandSize = 1,
                DrawIntervalSeconds = 100f,
                FlyingSwordIntervalSeconds = 100f,
                EnemyIntentMinSeconds = 100f,
                EnemyIntentMaxSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 23
            });
            controller.Battle.TryPlayCard(0);
        }

        private static void StartBossIntentPreview(DemoGameController controller)
        {
            controller.Battle.ClearBattle();
            DemoCard phaseBreaker = new DemoCard
            {
                Id = "capture_phase_breaker",
                Name = "万剑破劫",
                Cost = 0,
                Damage = 2500,
                Type = DemoCardType.Finisher,
                Style = DemoSwordStyle.Wanjian,
                Quality = DemoQuality.Immortal
            };
            controller.Battle.StartBattle(new DemoBattleSetup
            {
                Deck = new[] { phaseBreaker },
                EnemyName = "天劫化身",
                EnemyHealth = 3800,
                IsBoss = true,
                PlayerHealth = 72,
                PlayerMaxHealth = 72,
                InitialEnergy = 2,
                InitialHandSize = 1,
                DrawIntervalSeconds = 100f,
                FlyingSwordIntervalSeconds = 100f,
                IntroSeconds = 0f,
                RandomSeed = 29
            });
            controller.Battle.TryPlayCard(0);
        }

        private static void ForceRunResult(DemoGameController controller, bool victory)
        {
            controller.Battle.ClearBattle();
            controller.Run.Map.CompleteWithResult(victory);
            SetPrivateField(controller, "lastReachedLayer", victory ? 3 : 2);
            InvokePrivate(controller, "FinishRun", new object[] { victory, victory });
        }

        private static void SetPrivateField(DemoGameController controller, string fieldName, object value)
        {
            FieldInfo field = typeof(DemoGameController).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new MissingFieldException(typeof(DemoGameController).FullName, fieldName);
            }

            field.SetValue(controller, value);
        }

        private static void InvokePrivate(DemoGameController controller, string methodName, object[] arguments)
        {
            MethodInfo method = typeof(DemoGameController).GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                throw new MissingMethodException(typeof(DemoGameController).FullName, methodName);
            }

            method.Invoke(controller, arguments);
        }

        private static void ClaimUtilityFirstRoute(DemoGameController controller)
        {
            int index = Enumerable.Range(0, controller.CurrentRewards.Count)
                .FirstOrDefault(i =>
                {
                    DemoReward reward = controller.CurrentRewards[i];
                    return reward.RoutePlan != null
                        && reward.RoutePlan.Nodes.Count > 0
                        && reward.RoutePlan.Nodes[0].Type != DemoNodeType.Battle
                        && reward.RoutePlan.Nodes[0].Type != DemoNodeType.Boss;
                });
            controller.ClaimRewardAt(index);

            if (controller.HasBattle)
            {
                throw new InvalidOperationException("No utility-first route was available for the node screenshot.");
            }
        }


        private static void BeginCapture(string fileName)
        {
            BeginCapture(fileName, CaptureWidth, CaptureHeight);
        }

        private static void BeginCapture(string fileName, int width, int height)
        {
            if (captureSession != null)
            {
                throw new InvalidOperationException("A commercial UI capture is already in progress.");
            }

            if (fileName.StartsWith("battle_opening_", StringComparison.OrdinalIgnoreCase)
                && fileName.Contains("commercial"))
            {
                ValidateOpeningBattleCaptureState(fileName);
            }

            if (fileName.StartsWith("flow_", StringComparison.OrdinalIgnoreCase)
                || fileName.StartsWith("post_battle_", StringComparison.OrdinalIgnoreCase)
                || fileName.StartsWith("run_result_", StringComparison.OrdinalIgnoreCase))
            {
                ValidateFullFlowCaptureState(fileName);
            }

            Canvas canvas = UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None)
                .FirstOrDefault(candidate => candidate.name == "DEMO_RuntimeCanvas");
            if (canvas == null)
            {
                throw new InvalidOperationException("Runtime canvas was not available for capture.");
            }

            string outputDirectory = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "..", "tmp"));
            Directory.CreateDirectory(outputDirectory);
            string outputPath = Path.Combine(outputDirectory, fileName);
            captureSession = new CaptureSession(canvas, outputPath, width, height);
        }

        private static void ValidateFullFlowCaptureState(string fileName)
        {
            DemoGameController controller = UnityEngine.Object.FindAnyObjectByType<DemoGameController>();
            if (controller == null)
            {
                throw new InvalidOperationException("Full-flow capture had no controller.");
            }

            DemoFlowPhase expected;
            if (fileName.Contains("home"))
            {
                expected = DemoFlowPhase.Home;
            }
            else if (fileName.Contains("opening_root"))
            {
                expected = DemoFlowPhase.OpeningRoot;
            }
            else if (fileName.Contains("opening_vessel"))
            {
                expected = DemoFlowPhase.OpeningVessel;
            }
            else if (fileName.Contains("opening_region"))
            {
                expected = DemoFlowPhase.OpeningRegion;
            }
            else if (fileName.Contains("encounter_intro"))
            {
                expected = DemoFlowPhase.EncounterIntro;
            }
            else if (fileName.Contains("training"))
            {
                expected = DemoFlowPhase.Training;
            }
            else if (fileName.Contains("preparation"))
            {
                expected = DemoFlowPhase.Preparation;
            }
            else if (fileName.Contains("boss_gate"))
            {
                expected = DemoFlowPhase.BossGate;
            }
            else if (fileName.Contains("route"))
            {
                expected = DemoFlowPhase.RouteChoice;
            }
            else if (fileName.Contains("reward"))
            {
                expected = DemoFlowPhase.RewardChoice;
            }
            else if (fileName.StartsWith("run_result_", StringComparison.OrdinalIgnoreCase))
            {
                expected = DemoFlowPhase.RunResult;
            }
            else
            {
                return;
            }

            if (controller.FlowPhase != expected)
            {
                throw new InvalidOperationException($"Capture {fileName} expected {expected} but found {controller.FlowPhase}.");
            }

            if (expected == DemoFlowPhase.RunResult)
            {
                bool expectedVictory = fileName.Contains("victory");
                if (controller.RunSummary == null || controller.RunSummary.Victory != expectedVictory)
                {
                    throw new InvalidOperationException("Run result capture outcome did not match its filename.");
                }
            }

            if (expected == DemoFlowPhase.Home
                || expected == DemoFlowPhase.OpeningRoot
                || expected == DemoFlowPhase.OpeningVessel
                || expected == DemoFlowPhase.OpeningRegion)
            {
                return;
            }

            DemoCommercialJourneyView view = UnityEngine.Object.FindAnyObjectByType<DemoCommercialJourneyView>(FindObjectsInactive.Include);
            view?.RefreshNow();
            RectTransform surface = UnityEngine.Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .FirstOrDefault(rect => rect.name == "CommercialJourneySurface");
            if (view == null || surface == null || !surface.gameObject.activeInHierarchy)
            {
                throw new InvalidOperationException("Commercial journey surface was not active for " + fileName + ".");
            }
        }

        private static void ValidateOpeningBattleCaptureState(string fileName)
        {
            DemoGameController controller = UnityEngine.Object.FindAnyObjectByType<DemoGameController>();
            DemoBattleHudView hud = UnityEngine.Object.FindAnyObjectByType<DemoBattleHudView>(FindObjectsInactive.Include);
            if (controller == null || hud == null || !hud.gameObject.activeInHierarchy)
            {
                throw new InvalidOperationException("Opening battle capture did not have an active commercial HUD.");
            }

            RectTransform[] activeRects = UnityEngine.Object.FindObjectsByType<RectTransform>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
            string[] requiredNames = { "PlayerRoot", "EnemyRoot", "PhaseSeal", "EnemyIntent", "Backdrop" };
            for (int i = 0; i < requiredNames.Length; i++)
            {
                if (!activeRects.Any(rect => rect.name == requiredNames[i]))
                {
                    throw new InvalidOperationException("Opening battle capture was missing active element: " + requiredNames[i]);
                }
            }

            Image backdrop = activeRects
                .Where(rect => rect.name == "Backdrop")
                .Select(rect => rect.GetComponent<Image>())
                .FirstOrDefault(image => image != null
                    && image.sprite != null
                    && image.sprite.texture != null
                    && (image.sprite.texture.name.Contains("scene_battle_old_mine_combat_far_001")
                        || image.sprite.texture.name.Contains("scene_battle_old_mine_opening_far_002")));
            string textureName = backdrop != null && backdrop.sprite != null && backdrop.sprite.texture != null
                ? backdrop.sprite.texture.name
                : string.Empty;
            if (!textureName.Contains("scene_battle_old_mine_combat_far_001")
                && !textureName.Contains("scene_battle_old_mine_opening_far_002"))
            {
                throw new InvalidOperationException("Opening battle backdrop did not load the old-mine opening resource: " + textureName);
            }

            DemoBattleCardView[] cards = UnityEngine.Object.FindObjectsByType<DemoBattleCardView>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);
            int expectedCards = fileName.Contains("victory") ? 0 : 2;
            if (cards.Length != expectedCards)
            {
                throw new InvalidOperationException($"Opening battle capture expected {expectedCards} cards but found {cards.Length}.");
            }

            if (!fileName.Contains("playable_hover") && cards.Length == 2)
            {
                RectTransform[] cardRects = cards
                    .Select(card => card.GetComponent<RectTransform>())
                    .OrderBy(rect => rect.position.x)
                    .ToArray();
                for (int i = 1; i < cardRects.Length; i++)
                {
                    Vector3[] previousCorners = new Vector3[4];
                    Vector3[] currentCorners = new Vector3[4];
                    cardRects[i - 1].GetWorldCorners(previousCorners);
                    cardRects[i].GetWorldCorners(currentCorners);
                    float previousRight = previousCorners.Max(corner => corner.x);
                    float currentLeft = currentCorners.Min(corner => corner.x);
                    if (currentLeft < previousRight - 1f)
                    {
                        throw new InvalidOperationException("Opening battle starter cards overlapped before hover.");
                    }
                }
            }

            DemoBattleState battle = controller.Battle;
            if (fileName.Contains("intro") && battle.Phase != DemoBattlePhase.Intro)
            {
                throw new InvalidOperationException("Opening intro capture left the Intro phase before rendering.");
            }
            if ((fileName.Contains("running") || fileName.Contains("playable_hover"))
                && (battle.Phase != DemoBattlePhase.Running
                    || !battle.Hand.Any(card => card.Id == "technique_breathing_recovery" && card.Cost <= battle.Energy)
                    || !battle.Hand.Any(card => card.Id == "technique_incomplete_sword_scroll")))
            {
                throw new InvalidOperationException("Opening playable capture did not contain a playable starter card.");
            }
            if (fileName.Contains("energy_blocked")
                && (battle.Energy != 0
                    || !battle.Hand.Any(card => card.Id == "technique_breathing_recovery" && card.Cost == 0)
                    || !battle.Hand.Any(card => card.Id == "technique_incomplete_sword_scroll" && card.Cost > battle.Energy)))
            {
                throw new InvalidOperationException("Opening zero-energy capture did not preserve the free heart formula and blocked sword art split.");
            }
            if (fileName.Contains("enemy_intent")
                && (battle.EnemyIntentProgress < 0.90f || battle.EnemyActionCount != 0))
            {
                throw new InvalidOperationException("Opening intent capture was not inside the pre-impact warning window.");
            }
            if (fileName.Contains("victory") && battle.Phase != DemoBattlePhase.Won)
            {
                throw new InvalidOperationException("Opening victory capture was not held in the Won phase.");
            }
            if (fileName.Contains("victory")
                && !activeRects.Any(rect => rect.name == "ResultBanner" && rect.gameObject.activeInHierarchy))
            {
                throw new InvalidOperationException("Opening victory capture did not switch to the compact result banner.");
            }
            if (fileName.Contains("playable_hover"))
            {
                RectTransform detail = activeRects.FirstOrDefault(rect => rect.name == "CardDetail");
                if (detail == null || !detail.gameObject.activeInHierarchy || !cards.Any(card => card.transform.localScale.x > 1.02f))
                {
                    throw new InvalidOperationException("Opening playable capture did not expand hover rules and lift a card.");
                }
            }

            string[] decorativeNames = { "OpeningBattleMid", "OpeningBattleNear" };
            foreach (Image image in UnityEngine.Object.FindObjectsByType<Image>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            {
                if (decorativeNames.Contains(image.name) && image.raycastTarget)
                {
                    throw new InvalidOperationException("Opening battle decorative layer intercepted raycasts: " + image.name);
                }
            }
        }

        private static void CancelCaptureSession()
        {
            if (captureSession == null)
            {
                return;
            }

            captureSession.Dispose();
            captureSession = null;
        }

        private sealed class CaptureSession : IDisposable
        {
            private const int RequiredStableFrames = 8;
            private const int MaximumLayoutFrames = 120;
            private const int PixelSampleStride = 4;
            private const float DimensionTolerance = 1f;
            private const double MinimumRegionVariance = 4d;
            private const double MinimumRegionCoverage = 0.02d;
            private const int ClearColorDistanceSquared = 324;

            private readonly Canvas canvas;
            private readonly RectTransform canvasRect;
            private readonly string outputPath;
            private readonly int width;
            private readonly int height;
            private readonly RenderMode originalRenderMode;
            private readonly Camera originalCamera;
            private readonly float originalPlaneDistance;
            private readonly GameObject cameraObject;
            private readonly Camera captureCamera;
            private readonly RenderTexture renderTexture;
            private readonly Color clearColor = new Color(0.96f, 0.94f, 0.88f, 1f);

            private CapturePhase phase = CapturePhase.Prepare;
            private int layoutFrames;
            private int stableFrames;
            private int lastObservedFrame = -1;
            private bool disposed;

            public CaptureSession(Canvas canvas, string outputPath, int width, int height)
            {
                this.canvas = canvas;
                canvasRect = canvas.transform as RectTransform;
                this.outputPath = outputPath;
                this.width = width;
                this.height = height;
                originalRenderMode = canvas.renderMode;
                originalCamera = canvas.worldCamera;
                originalPlaneDistance = canvas.planeDistance;

                if (canvasRect == null)
                {
                    throw new InvalidOperationException("Runtime canvas did not have a RectTransform.");
                }

                cameraObject = new GameObject("DemoCommercialCaptureCamera");
                cameraObject.hideFlags = HideFlags.HideAndDontSave;
                captureCamera = cameraObject.AddComponent<Camera>();
                captureCamera.enabled = true;
                captureCamera.clearFlags = CameraClearFlags.SolidColor;
                captureCamera.backgroundColor = clearColor;
                captureCamera.orthographic = true;
                captureCamera.orthographicSize = 5f;
                captureCamera.nearClipPlane = 0.01f;
                captureCamera.farClipPlane = 100f;
                captureCamera.transform.position = new Vector3(0f, 0f, -10f);
                captureCamera.rect = new Rect(0f, 0f, 1f, 1f);
                captureCamera.aspect = width / (float)height;

                renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32)
                {
                    name = "DemoCommercialCaptureTexture",
                    antiAliasing = 1,
                    useMipMap = false,
                    autoGenerateMips = false,
                    hideFlags = HideFlags.HideAndDontSave
                };
                renderTexture.Create();
                captureCamera.targetTexture = renderTexture;
            }

            public bool Tick()
            {
                if (disposed)
                {
                    throw new ObjectDisposedException(nameof(CaptureSession));
                }

                switch (phase)
                {
                    case CapturePhase.Prepare:
                        Prepare();
                        phase = CapturePhase.WaitForLayout;
                        return false;
                    case CapturePhase.WaitForLayout:
                        WaitForLayout();
                        return false;
                    case CapturePhase.Capture:
                        Capture();
                        phase = CapturePhase.Restore;
                        return false;
                    case CapturePhase.Restore:
                        Dispose();
                        Debug.Log("Captured commercial UI screenshot: " + outputPath);
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            public void Dispose()
            {
                if (disposed)
                {
                    return;
                }

                disposed = true;
                if (canvas != null)
                {
                    canvas.renderMode = originalRenderMode;
                    canvas.worldCamera = originalCamera;
                    canvas.planeDistance = originalPlaneDistance;
                    Canvas.ForceUpdateCanvases();
                }

                if (captureCamera != null)
                {
                    captureCamera.targetTexture = null;
                }

                if (renderTexture != null)
                {
                    renderTexture.Release();
                    UnityEngine.Object.DestroyImmediate(renderTexture);
                }

                if (cameraObject != null)
                {
                    UnityEngine.Object.DestroyImmediate(cameraObject);
                }
            }

            private void Prepare()
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = captureCamera;
                canvas.planeDistance = 1f;
                Canvas.ForceUpdateCanvases();
            }

            private void WaitForLayout()
            {
                if (Time.frameCount == lastObservedFrame)
                {
                    return;
                }

                lastObservedFrame = Time.frameCount;
                layoutFrames++;
                Canvas.ForceUpdateCanvases();

                string layoutIssue = GetLayoutIssue();
                if (layoutIssue == null)
                {
                    stableFrames++;
                    if (stableFrames >= RequiredStableFrames)
                    {
                        phase = CapturePhase.Capture;
                    }

                    return;
                }

                stableFrames = 0;
                if (layoutFrames >= MaximumLayoutFrames)
                {
                    throw new InvalidOperationException(
                        $"Capture layout did not stabilize at {width}x{height} after {layoutFrames} frames: {layoutIssue}");
                }
            }

            private void Capture()
            {
                string layoutIssue = GetLayoutIssue();
                if (layoutIssue != null)
                {
                    throw new InvalidOperationException("Capture layout changed before render: " + layoutIssue);
                }

                captureCamera.Render();
                Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
                RenderTexture previous = RenderTexture.active;
                try
                {
                    RenderTexture.active = renderTexture;
                    screenshot.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
                    screenshot.Apply(false, false);
                    ValidatePixels(screenshot);
                    File.WriteAllBytes(outputPath, screenshot.EncodeToPNG());
                }
                finally
                {
                    RenderTexture.active = previous;
                    UnityEngine.Object.DestroyImmediate(screenshot);
                }
            }

            private string GetLayoutIssue()
            {
                if (canvas == null || canvasRect == null)
                {
                    return "runtime canvas was destroyed";
                }

                if (canvas.renderMode != RenderMode.ScreenSpaceCamera || canvas.worldCamera != captureCamera)
                {
                    return "runtime canvas was not bound to the capture camera";
                }

                if (captureCamera.targetTexture != renderTexture)
                {
                    return "capture camera lost its target texture";
                }

                if (captureCamera.pixelWidth != width || captureCamera.pixelHeight != height)
                {
                    return $"camera pixel size was {captureCamera.pixelWidth}x{captureCamera.pixelHeight}";
                }

                Rect pixelRect = canvas.pixelRect;
                if (!Approximately(pixelRect.width, width) || !Approximately(pixelRect.height, height))
                {
                    return $"canvas pixel rect was {pixelRect.width:0.##}x{pixelRect.height:0.##}";
                }

                Rect rootRect = canvasRect.rect;
                float expectedAspect = width / (float)height;
                float rootAspect = rootRect.height <= 0f ? 0f : rootRect.width / rootRect.height;
                if (rootRect.width <= 0f || rootRect.height <= 0f || Mathf.Abs(rootAspect - expectedAspect) > 0.01f)
                {
                    return $"canvas root rect was {rootRect.width:0.##}x{rootRect.height:0.##} with an unexpected aspect";
                }

                return null;
            }

            private void ValidatePixels(Texture2D screenshot)
            {
                Color32[] pixels = screenshot.GetPixels32();
                Color32 clear = clearColor;
                RegionStats lower = MeasureRegion(pixels, 0, height / 3, clear);
                RegionStats middle = MeasureRegion(pixels, height / 3, height * 2 / 3, clear);
                RegionStats upper = MeasureRegion(pixels, height * 2 / 3, height, clear);

                Debug.Log(
                    $"Capture pixel check {Path.GetFileName(outputPath)}: "
                    + $"upper variance={upper.Variance:0.00}, coverage={upper.Coverage:P1}; "
                    + $"middle variance={middle.Variance:0.00}, coverage={middle.Coverage:P1}; "
                    + $"lower variance={lower.Variance:0.00}, coverage={lower.Coverage:P1}.");

                ValidateRegion("upper", upper);
                ValidateRegion("middle", middle);
                ValidateRegion("lower", lower);
            }

            private RegionStats MeasureRegion(Color32[] pixels, int startY, int endY, Color32 clear)
            {
                double red = 0d;
                double green = 0d;
                double blue = 0d;
                double redSquared = 0d;
                double greenSquared = 0d;
                double blueSquared = 0d;
                int covered = 0;
                int samples = 0;

                for (int y = startY; y < endY; y += PixelSampleStride)
                {
                    int row = y * width;
                    for (int x = 0; x < width; x += PixelSampleStride)
                    {
                        Color32 pixel = pixels[row + x];
                        red += pixel.r;
                        green += pixel.g;
                        blue += pixel.b;
                        redSquared += pixel.r * pixel.r;
                        greenSquared += pixel.g * pixel.g;
                        blueSquared += pixel.b * pixel.b;

                        int deltaRed = pixel.r - clear.r;
                        int deltaGreen = pixel.g - clear.g;
                        int deltaBlue = pixel.b - clear.b;
                        if (deltaRed * deltaRed + deltaGreen * deltaGreen + deltaBlue * deltaBlue >= ClearColorDistanceSquared)
                        {
                            covered++;
                        }

                        samples++;
                    }
                }

                double inverseSamples = 1d / samples;
                double variance = (
                    redSquared * inverseSamples - Math.Pow(red * inverseSamples, 2d)
                    + greenSquared * inverseSamples - Math.Pow(green * inverseSamples, 2d)
                    + blueSquared * inverseSamples - Math.Pow(blue * inverseSamples, 2d)) / 3d;
                return new RegionStats(Math.Max(0d, variance), covered * inverseSamples);
            }

            private static void ValidateRegion(string name, RegionStats stats)
            {
                if (stats.Coverage < MinimumRegionCoverage)
                {
                    throw new InvalidOperationException(
                        $"Capture {name} third was effectively clear ({stats.Coverage:P1} coverage); the Canvas was likely only partially rendered.");
                }

                if (stats.Variance < MinimumRegionVariance)
                {
                    throw new InvalidOperationException(
                        $"Capture {name} third was effectively flat ({stats.Variance:0.00} variance); refusing a gray or blank screenshot.");
                }
            }

            private static bool Approximately(float actual, float expected)
            {
                return Mathf.Abs(actual - expected) <= DimensionTolerance;
            }

            private enum CapturePhase
            {
                Prepare,
                WaitForLayout,
                Capture,
                Restore
            }

            private readonly struct RegionStats
            {
                public RegionStats(double variance, double coverage)
                {
                    Variance = variance;
                    Coverage = coverage;
                }

                public double Variance { get; }

                public double Coverage { get; }
            }
        }

        private static void AdvanceStage()
        {
            EditorPrefs.SetInt(StageKey, EditorPrefs.GetInt(StageKey, 0) + 1);
        }

        private static void Finish(int exitCode)
        {
            CancelCaptureSession();
            EditorPrefs.DeleteKey(ActiveKey);
            EditorPrefs.DeleteKey(StageKey);
            EditorPrefs.DeleteKey(OpeningBattleOnlyKey);
            EditorApplication.update -= UpdateCapture;
            EditorApplication.Exit(exitCode);
        }

        private enum OpeningBattleCaptureState
        {
            Intro,
            Playable,
            EnergyBlocked,
            IntentWarning,
            Victory
        }
    }
}
