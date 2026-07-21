using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using UnityEngine;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoGameController : MonoBehaviour
    {
        private enum DemoOpeningStage
        {
            SelectTrace,
            SelectRoot,
            SelectOpeningItem,
            SelectOpeningScene,
            Complete
        }

        private DemoRunState run = new DemoRunState();
        private readonly DemoBattleState battle = new DemoBattleState();
        private readonly DemoRewardService rewards = new DemoRewardService();
        private readonly DemoRouteRewardService routeRewards = new DemoRouteRewardService();
        private readonly DemoGongfaRewardService gongfaRewards = new DemoGongfaRewardService();
        private readonly DemoArtifactRewardService artifactRewards = new DemoArtifactRewardService();
        private List<DemoReward> currentRewards = new List<DemoReward>();
        private bool battleResultHandled;
        private DemoOpeningStage openingStage = DemoOpeningStage.SelectRoot;
        private DemoRewardContext currentRewardContext;
        private bool awaitingBattleReward;
        private float battleOutcomeDelay;
        private DemoRunSummary runSummary;
        private IDemoMetaProgressStore metaStore;
        private DemoMetaProgress metaProgress;
        private int rewardSeed;
        private int lastReachedLayer;
        private DemoMapNode pendingEncounter;
        private DemoJourneyNode pendingJourneyNode;
        private DemoJourneyNode activeJourneyBattleNode;
        private DemoJourneyRunSession journeySession;
        private IDemoRunSaveStore journeySaveStore;
        private string journeyError = string.Empty;

        private const string JourneyConfigSchemaVersion = "2";
        private const string JourneyContentVersion = "old_mine_vertical_slice_1";
        private const string JourneyMapAlgorithmVersion = "journey_graph_v1";
        private const string JourneySaveSlotName = "old_mine_run_v2";

        public DemoRunState Run => run;
        public DemoBattleState Battle => battle;
        public IReadOnlyList<DemoReward> CurrentRewards => currentRewards;
        public IEnumerable<string> BattleLogLines => battle.Log.Lines;
        public bool HasBattle => battle.Player != null && battle.Enemy != null;
        public bool HasPendingRewards => currentRewards.Count > 0;
        public bool IsRunComplete => run.Map.IsComplete;
        public bool HasRunResult => runSummary != null;
        public DemoRunSummary RunSummary => runSummary;
        public DemoMetaProgress MetaProgress => metaProgress;
        public DemoFlowPhase FlowPhase => run.Flow.Phase;
        public DemoFlowSnapshot FlowSnapshot => run.Flow.Snapshot;
        public DemoMapNode PendingEncounter => pendingEncounter;
        public DemoJourneyNode PendingJourneyNode => pendingJourneyNode;
        public DemoJourneyRunSession JourneySession => journeySession;
        public DemoJourneyGraph JourneyGraph => journeySession?.Graph;
        public DemoRunSaveV2 JourneySnapshot => journeySession?.Snapshot;
        public bool HasJourneySession => journeySession != null;
        public string JourneyError => journeyError;
        public bool HasPendingEncounter => pendingEncounter != null;
        public bool CanBackOpening => run.Map.CurrentNode.Type == DemoNodeType.Start
            && FlowPhase != DemoFlowPhase.Home;
        public bool CanAdvanceUtilityNode => currentRewards.Count == 0 && (HasRunResult || IsAdvanceNode(run.Map.CurrentNode.Type));
        public string UtilityActionLabel => GetUtilityActionLabel(run.Map.CurrentNode.Type);
        public string BattleActionLabel => GetBattleActionLabel();
        public bool CanRerollCurrentRewards => currentRewardContext != null
            && currentRewardContext.Source == DemoRewardSource.OpeningBattle
            && currentRewards.Count > 0
            && run.OpeningRewardRerolls > 0;

        public float BattleSpeed { get; set; } = 1f;

        public event Action<DemoFlowPhase> FlowPhaseChanged;

        private void Awake()
        {
            metaStore = new DemoPlayerPrefsMetaProgressStore();
            metaProgress = metaStore.Load();
        }

        private void Start()
        {
            openingStage = metaProgress.HasUnlock(DemoMetaProgress.BrokenSwordTraceId)
                ? DemoOpeningStage.SelectTrace
                : DemoOpeningStage.SelectRoot;
            SetFlowPhase(DemoFlowPhase.Home);
            EnterCurrentNode();
        }

        private void Update()
        {
            float targetTimeScale = HasBattle ? Mathf.Clamp(BattleSpeed, 0f, 2f) : 1f;
            if (!Mathf.Approximately(Time.timeScale, targetTimeScale))
            {
                Time.timeScale = targetTimeScale;
            }

            battle.Tick(Time.deltaTime);

            if (FlowPhase != DemoFlowPhase.Home && FlowPhase != DemoFlowPhase.RunResult)
            {
                run.AdvanceElapsedTime(Time.unscaledDeltaTime);
            }

            if (HasBattle && (battle.Phase == DemoBattlePhase.Won || battle.Phase == DemoBattlePhase.Lost))
            {
                SetFlowPhase(DemoFlowPhase.BattleOutcome);
            }

            if (HasBattle)
            {
                run.RecordSwordCount(battle.MaxSwordsReached);
                run.RecordBurstDamage(battle.HighestBurstDamage);
            }

            if (battle.Phase == DemoBattlePhase.Won && !battleResultHandled)
            {
                battleOutcomeDelay += Time.unscaledDeltaTime;
                if (battleOutcomeDelay >= 1.2f)
                {
                    HandleBattleWon();
                }
            }

            if (battle.Phase == DemoBattlePhase.Lost && !battleResultHandled)
            {
                battleOutcomeDelay += Time.unscaledDeltaTime;
                if (battleOutcomeDelay >= 1.2f)
                {
                    HandleBattleLost();
                }
            }
        }

        private void OnDisable()
        {
            Time.timeScale = 1f;
        }
        public bool TryPlayCardAt(int handIndex)
        {
            return battle.TryPlayCard(handIndex);
        }


        public void TriggerBattleAction()
        {
            if (!HasBattle)
            {
                return;
            }

            if (battle.Phase == DemoBattlePhase.Intro || battle.Phase == DemoBattlePhase.Running)
            {
                BattleSpeed = BattleSpeed <= 0.01f ? 1f : 0f;
                return;
            }

            if (battle.Phase == DemoBattlePhase.Lost)
            {
                StartNextRun();
            }
        }

        public void ClaimRewardAt(int rewardIndex)
        {
            if (rewardIndex < 0 || rewardIndex >= currentRewards.Count)
            {
                return;
            }

            ClaimReward(currentRewards[rewardIndex]);
        }

        public void AdvanceUtilityNode()
        {
            if (HasRunResult)
            {
                StartNextRun();
                return;
            }

            if (HasPendingEncounter)
            {
                BeginCurrentEncounter();
                return;
            }

            if (CanAdvanceUtilityNode)
            {
                CompleteUtilityNode();
            }
        }

        public void StartNewRun()
        {
            if (HasRunResult || run.Map.CurrentNode.Type != DemoNodeType.Start)
            {
                ResetRun();
            }

            PrepareOpeningChoices();
        }

        public void StartNextRun()
        {
            ResetRun();
            PrepareOpeningChoices();
        }

        public void ReturnHome()
        {
            ResetRun();
        }

        public void BeginCurrentEncounter()
        {
            if (pendingEncounter == null || HasBattle)
            {
                return;
            }

            DemoMapNode encounter = pendingEncounter;
            pendingEncounter = null;
            if (pendingJourneyNode != null && pendingJourneyNode.IsCombat)
            {
                activeJourneyBattleNode = pendingJourneyNode;
            }
            StartBattle(encounter);
        }

        public bool BeginConfiguredJourney(DemoRegionDefinition region, int? rootSeed = null)
        {
            if (region == null || !region.IsAvailable)
            {
                journeyError = "可进入的境域配置缺失。";
                return false;
            }

            int seed = rootSeed ?? CreateJourneySeed();
            DemoJourneyGraph graph = DemoJourneyGraphGenerator.Generate(seed);
            journeySaveStore = new DemoFileRunSaveStore(
                Application.persistentDataPath,
                JourneySaveSlotName);
            DemoJourneyRunSessionOptions options = BuildJourneySessionOptions(region, seed);
            if (!DemoJourneyRunSession.TryCreateNew(
                    graph,
                    journeySaveStore,
                    options,
                    seed,
                    out journeySession,
                    out journeyError))
            {
                journeySession = null;
                return false;
            }

            pendingJourneyNode = null;
            activeJourneyBattleNode = null;
            pendingEncounter = null;
            battle.ClearBattle(true);
            currentRewards.Clear();
            currentRewardContext = null;
            SetFlowPhase(DemoFlowPhase.JourneyMap);
            return true;
        }

        public bool TryResumeConfiguredJourney()
        {
            DemoFileRunSaveStore store = new DemoFileRunSaveStore(
                Application.persistentDataPath,
                JourneySaveSlotName);
            if (!store.TryLoadLatestOrPrevious(
                    out DemoRunSaveV2 saved,
                    out _,
                    out journeyError))
            {
                return false;
            }

            DemoJourneyGraph graph = DemoJourneyGraphGenerator.Generate(saved.RootSeed);
            if (!DemoJourneyRunSession.TryRestore(
                    graph,
                    store,
                    JourneyConfigSchemaVersion,
                    JourneyContentVersion,
                    JourneyMapAlgorithmVersion,
                    saved.RegionId,
                    out journeySession,
                    out _,
                    out journeyError))
            {
                journeySession = null;
                return false;
            }

            journeySaveStore = store;
            RestoreRunFromJourneySnapshot(journeySession.Snapshot);
            DemoRunSaveV2 snapshot = journeySession.Snapshot;
            journeySession.Graph.TryGetNode(snapshot.CurrentNodeId, out pendingJourneyNode);
            activeJourneyBattleNode = null;
            pendingEncounter = null;
            if (snapshot.FlowPhaseId == DemoRunFlowPhaseId.EncounterIntro && pendingJourneyNode != null)
            {
                pendingEncounter = ToLegacyEncounterNode(pendingJourneyNode, snapshot.PendingEncounterId);
                SetFlowPhase(pendingJourneyNode.Type == DemoJourneyNodeType.Boss
                    ? DemoFlowPhase.BossGate
                    : DemoFlowPhase.EncounterIntro);
            }
            else if (snapshot.FlowPhaseId == DemoRunFlowPhaseId.Breakthrough && pendingJourneyNode != null)
            {
                SetFlowPhase(DemoFlowPhase.Breakthrough);
            }
            else if (snapshot.FlowPhaseId == DemoRunFlowPhaseId.NodeScene && pendingJourneyNode != null)
            {
                SetFlowPhase(DemoFlowPhase.NodeScene);
            }
            else
            {
                pendingJourneyNode = null;
                SetFlowPhase(DemoFlowPhase.JourneyMap);
            }
            return true;
        }

        public bool SelectJourneyNode(string nodeId)
        {
            if (journeySession == null
                || FlowPhase != DemoFlowPhase.JourneyMap
                || !journeySession.Graph.TryGetNode(nodeId, out DemoJourneyNode node))
            {
                return false;
            }

            pendingJourneyNode = node;
            if (node.IsCombat)
            {
                string encounterId = ResolveJourneyEncounterId(node);
                if (!journeySession.TrySelectEncounter(
                        node.NodeId,
                        encounterId,
                        out _,
                        out journeyError))
                {
                    pendingJourneyNode = null;
                    return false;
                }

                pendingEncounter = ToLegacyEncounterNode(node, encounterId);
                battle.ClearBattle();
                SetFlowPhase(node.Type == DemoJourneyNodeType.Boss
                    ? DemoFlowPhase.BossGate
                    : DemoFlowPhase.EncounterIntro);
                return true;
            }

            if (!journeySession.TrySelectReachableNode(
                    node.NodeId,
                    out _,
                    out journeyError))
            {
                pendingJourneyNode = null;
                return false;
            }

            battle.ClearBattle();
            SetFlowPhase(node.Type == DemoJourneyNodeType.Breakthrough
                ? DemoFlowPhase.Breakthrough
                : DemoFlowPhase.NodeScene);
            return true;
        }

        public bool CompleteJourneyNode()
        {
            if (journeySession == null || pendingJourneyNode == null || pendingJourneyNode.IsCombat)
            {
                return false;
            }

            ApplyJourneyNodeImmediateEffect(pendingJourneyNode);
            DemoJourneyNodeOutcome outcome = BuildJourneyNodeOutcome(pendingJourneyNode, false);
            if (!journeySession.TryCompleteCurrentNode(
                    outcome,
                    out _,
                    out journeyError))
            {
                return false;
            }

            pendingJourneyNode = null;
            pendingEncounter = null;
            SetFlowPhase(DemoFlowPhase.JourneyMap);
            return true;
        }

        public void BackOpeningStep()
        {
            if (!CanBackOpening)
            {
                return;
            }

            if (openingStage == DemoOpeningStage.SelectOpeningScene)
            {
                DemoRootDefinition root = run.OpeningSelection.Root;
                run.SetRoot(root);
                openingStage = DemoOpeningStage.SelectOpeningItem;
                PrepareOpeningChoices();
                return;
            }

            if (openingStage == DemoOpeningStage.SelectOpeningItem)
            {
                run.SetRoot(null);
                openingStage = DemoOpeningStage.SelectRoot;
                PrepareOpeningChoices();
                return;
            }

            if (openingStage == DemoOpeningStage.SelectRoot
                && metaProgress.HasUnlock(DemoMetaProgress.BrokenSwordTraceId))
            {
                run.EquippedTraceId = string.Empty;
                run.OpeningRewardRerolls = 0;
                openingStage = DemoOpeningStage.SelectTrace;
                PrepareOpeningChoices();
                return;
            }

            ReturnHome();
        }

        public void RerollCurrentRewards()
        {
            if (!CanRerollCurrentRewards || !run.ConsumeOpeningRewardReroll())
            {
                return;
            }

            rewardSeed++;
            currentRewardContext.Source = DemoRewardSource.NormalBattle;
            currentRewardContext.Tier = DemoRewardTier.Standard;
            currentRewardContext.RewardProfileId = string.Empty;
            currentRewardContext.AllowsDivine = false;
            currentRewardContext.AllowsFinisher = false;
            currentRewardContext.HasSeed = true;
            currentRewardContext.Seed = rewardSeed;
            currentRewards = rewards.CreateChoices(currentRewardContext, run);
        }

        public string GetHeaderSummary()
        {
            string artifactText = run.Artifacts.Count > 0 ? string.Join("、", run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name)) : "暂无";
            string gongfaText = string.Join(" / ", GetGongfaNames());
            return $"当前节点：{run.Map.CurrentNode.Name} | 生命：{run.CurrentHealth}/{run.MaxHealth} | 牌组：{run.Deck.Count} | 主修：{gongfaText} | 核心法器：{artifactText}";
        }

        public string GetMapSummary()
        {
            return string.Join(
                "\n",
                run.Map.Nodes.Select(
                    node =>
                    {
                        string marker = node == run.Map.CurrentNode ? ">" : node.Completed ? "✓" : "·";
                        return $"{marker} 第{node.Layer}层 {GetNodeTypeLabel(node.Type)} - {node.Name}";
                    }));
        }

        public string GetBattleSummary()
        {
            if (HasRunResult)
            {
                return GetRunResultSummary();
            }

            if (!HasBattle)
            {
                switch (run.Map.CurrentNode.Type)
                {
                    case DemoNodeType.Start:
                        return openingStage == DemoOpeningStage.SelectTrace
                            ? "先整备这一世是否携带传承道痕。道痕只增加选择，不继承战力。"
                            : openingStage == DemoOpeningStage.SelectRoot
                                ? "当前位于起点。先定根脚，确认这一世从哪里来。"
                                : openingStage == DemoOpeningStage.SelectOpeningItem
                                    ? "当前位于起点。再定所携，确认带什么上路。"
                                    : openingStage == DemoOpeningStage.SelectOpeningScene
                                        ? "所携已定，从它牵引出的去处中落下第一脚。"
                                        : "所往已定，接下来先打一场入场首战。";
                    case DemoNodeType.RouteChoice:
                        return "根据刚获得的组件选择稳修、冒险或构筑路线；节点顺序会直接改变成型速度。";
                    case DemoNodeType.Reward:
                        return "挑一项补强，把下一场斗法推向更明确的万剑高点。";
                    case DemoNodeType.Training:
                        return "修炼会定向补齐当前阶段缺少的启动、剑阵或收束组件。";
                    case DemoNodeType.Shop:
                        return "整备优先补主修、核心法器、续航或渡劫前收束。";
                    case DemoNodeType.Result:
                    case DemoNodeType.Victory:
                        return GetRunResultSummary();
                    default:
                        return "当前没有战斗。";
                }
            }

            string artifactText = battle.ActiveArtifacts.Count > 0
                ? string.Join("、", battle.ActiveArtifacts.Select(type => DemoArtifactLibrary.Get(type).Name))
                : "无";
            string gongfaText = battle.ActiveGongfas.Count > 0
                ? string.Join("、", battle.ActiveGongfas.Select(type => DemoGongfaLibrary.Get(type).Name))
                : "无";

            return
                $"状态：{GetPhaseLabel(battle.Phase)}  ·  已斗法 {battle.ElapsedSeconds:0.0}s  ·  敌方意图 {battle.EnemyIntentRemaining:0.0}s\n" +
                $"玩家：气血 {battle.Player.Health}/{battle.Player.MaxHealth}  ·  护盾 {battle.Player.Block}  ·  灵气 {battle.EnergyExact:0.0}/{battle.MaxEnergy}  ·  剑意 {battle.Player.SwordIntent}\n" +
                $"飞剑：本命 {battle.PermanentSwords} + 临时 {battle.TemporarySwords} = {battle.TotalSwords}  ·  功法 {gongfaText}  ·  法器 {artifactText}\n" +
                $"敌人：{battle.Enemy.Name}  ·  气血 {battle.Enemy.Health}/{battle.Enemy.MaxHealth}  ·  感电 {battle.Enemy.Shock}  ·  流血 {battle.Enemy.Bleed}" +
                (battle.IsBossBattle ? $"\n劫相：{GetBossPhaseLabel(battle.BossPhase)}  ·  {battle.BossIntentText}" : string.Empty);
        }
        public string GetHandStatus()
        {
            if (!HasBattle)
            {
                return HasRunResult
                    ? "本世已经结算，手牌归卷。"
                    : "当前不是战斗节点。";
            }

            if (battle.Phase == DemoBattlePhase.Intro)
            {
                return "双方气机正在相接，斗法即将持续演算。";
            }

            if (battle.Phase != DemoBattlePhase.Running)
            {
                return battle.Phase == DemoBattlePhase.Won ? "剑光破敌。" : "道基失守。";
            }

            return
                $"手牌 {battle.Hand.Count}/{battle.HandLimit}  ·  牌库 {battle.DrawPile.Count}  ·  弃牌 {battle.DiscardPile.Count}  ·  " +
                $"下次抽牌 {battle.DrawTimer:0.0}s  ·  下次齐射 {battle.FlyingSwordTimer:0.0}s";
        }
        public string GetRewardSummary()
        {
            if (currentRewards.Count == 0)
            {
                return "完成战斗或补强节点后出现三选一。";
            }

            if (currentRewards.All(reward => reward.Type == DemoRewardType.Route))
            {
                return "首战已经兑现所往选择；现在挑下一段路，让节点顺序开始真正影响成型速度。";
            }

            return "选择一项长期补强，把爽点推向后面的演武阶段。";
        }

        public string GetDeckSummary()
        {
            return string.Join("\n", run.Deck.Select(card => $"{card.Name} [{card.Cost}]"));
        }

        public string GetLogSummary()
        {
            return string.Join("\n", battle.Log.Lines);
        }

        private void EnterCurrentNode()
        {
            DemoMapNode node = run.Map.CurrentNode;
            currentRewards.Clear();
            currentRewardContext = null;
            awaitingBattleReward = false;
            battleOutcomeDelay = 0f;
            pendingEncounter = null;

            if (node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss)
            {
                bool openingBattle = string.Equals(node.NodeId, "node_opening_battle", System.StringComparison.OrdinalIgnoreCase);
                if (openingBattle)
                {
                    StartBattle(node);
                    return;
                }

                battle.ClearBattle();
                pendingEncounter = node;
                SetFlowPhase(node.Type == DemoNodeType.Boss
                    ? DemoFlowPhase.BossGate
                    : DemoFlowPhase.EncounterIntro);
                return;
            }

            if (node.Type == DemoNodeType.RouteChoice)
            {
                battle.ClearBattle();
                currentRewards = routeRewards.CreateChoices(node.Layer, run);
                SetFlowPhase(DemoFlowPhase.RouteChoice);
                return;
            }

            if (node.Type == DemoNodeType.Reward)
            {
                battle.ClearBattle();
                OpenRewardsForNode(node);
                SetFlowPhase(DemoFlowPhase.RewardChoice);
                return;
            }

            if (node.Type == DemoNodeType.Training || node.Type == DemoNodeType.Shop)
            {
                battle.ClearBattle();
                OpenUtilityNode(node);
                return;
            }

            battle.ClearBattle(node.Type == DemoNodeType.Start);
            if (node.Type == DemoNodeType.Result || node.Type == DemoNodeType.Victory)
            {
                FinishRun(node.Type == DemoNodeType.Victory || run.Map.WasVictory, run.Map.WasVictory);
                SetFlowPhase(DemoFlowPhase.RunResult);
                return;
            }


            if (node.Type == DemoNodeType.Start && currentRewards.Count == 0)
            {
                SetFlowPhase(DemoFlowPhase.Home);
            }
        }

        private void StartBattle(DemoMapNode node)
        {
            DemoEnemyDefinition enemy = ResolveEnemy(node);
            bool boss = node.Type == DemoNodeType.Boss || (enemy != null && enemy.IsBoss);
            bool openingBattle = string.Equals(node.NodeId, "node_opening_battle", StringComparison.OrdinalIgnoreCase);
            IReadOnlyList<DemoBattleEnemySetup> journeyEnemies = activeJourneyBattleNode == null
                ? null
                : BuildJourneyEnemySet(activeJourneyBattleNode, enemy);
            if (activeJourneyBattleNode != null && run.BattlesWon == 0)
            {
                openingBattle = true;
            }
            battleResultHandled = false;
            battleOutcomeDelay = 0f;
            BattleSpeed = 1f;
            battle.StartBattle(new DemoBattleSetup
            {
                Deck = run.Deck,
                Enemies = journeyEnemies,
                Artifacts = run.Artifacts,
                Gongfas = run.GetLearnedGongfas().ToList(),
                Relics = run.Relics,
                PlayerMaxHealth = run.MaxHealth,
                PlayerHealth = run.CurrentHealth,
                EnemyId = enemy?.Id ?? node.EncounterId,
                EnemyName = enemy?.Name ?? node.Name,
                EnemyHealth = enemy?.MaxHealth ?? (boss ? 900 : node.Layer == 1 ? 110 : node.Layer == 2 ? 180 : 260),
                IsBoss = boss,
                IsOpeningBattle = openingBattle,
                BonusEnergyCapacity = run.BonusEnergy,
                BonusPermanentSwords = run.BonusPermanentSwords,
                MaxEnergy = run.Realm.Stage >= 2 ? 6 : 5,
                InitialEnergy = 3,
                EnergyRegenerationPerSecond = run.Realm.Stage >= 2 ? 1.25f : 1f,
                HandLimit = 6,
                InitialHandSize = 2,
                FlyingSwordIntervalSeconds = Mathf.Max(1.55f, 2.2f - 0.12f * Mathf.Max(0, run.InnateArtifact.RefinementStage - 1)),
                EnemyIntentMinSeconds = boss ? 5f : IsElite(enemy) ? 4.8f : 5.8f,
                EnemyIntentMaxSeconds = boss ? 6f : IsElite(enemy) ? 5.6f : 7f,
                RandomSeed = journeySession?.Snapshot.PendingEncounterSeed
                    ?? (1000 + run.BattlesWon * 37 + node.Layer * 101)
            });
            SetFlowPhase(DemoFlowPhase.Battle);
        }

        private DemoEnemyDefinition ResolveEnemy(DemoMapNode node)
        {
            if (node != null
                && !string.IsNullOrEmpty(node.EncounterId)
                && DemoConfigRepository.TryGetEnemyById(node.EncounterId, out DemoEnemyDefinition byId))
            {
                return byId;
            }


            return null;
        }

        private static bool IsElite(DemoEnemyDefinition enemy)
        {
            return enemy != null
                && string.Equals(enemy.BattleRole, "elite", System.StringComparison.OrdinalIgnoreCase);
        }

        private void HandleBattleWon()
        {
            if (battleResultHandled || !HasBattle)
            {
                return;
            }

            battleResultHandled = true;
            BattleSpeed = 1f;
            if (journeySession != null && activeJourneyBattleNode != null)
            {
                DemoJourneyNode completedJourneyBattle = activeJourneyBattleNode;
                lastReachedLayer = (completedJourneyBattle.ActIndex - 1) * 8
                    + completedJourneyBattle.DepthIndex + 1;
                bool defeatedJourneyBoss = completedJourneyBattle.Type == DemoJourneyNodeType.Boss;
                run.CurrentHealth = battle.Player.Health;
                run.RecordBattleVictory(battle.MaxSwordsReached, battle.HighestBurstDamage);
                DemoJourneyNodeOutcome journeyOutcome = BuildJourneyNodeOutcome(
                    completedJourneyBattle,
                    true);
                journeyOutcome.BattlesWonDelta = 1;
                journeyOutcome.MaxSwordCount = battle.MaxSwordsReached;
                journeyOutcome.HighestBurstDamage = battle.HighestBurstDamage;
                if (completedJourneyBattle.Type == DemoJourneyNodeType.MiniBoss)
                {
                    journeyOutcome.MiniBossesDefeatedDelta = 1;
                }

                if (!journeySession.TryCompleteCurrentNode(
                        journeyOutcome,
                        out _,
                        out journeyError))
                {
                    battleResultHandled = false;
                    return;
                }

                battle.ClearBattle();
                activeJourneyBattleNode = null;
                pendingJourneyNode = null;
                pendingEncounter = null;
                if (defeatedJourneyBoss)
                {
                    run.Map.CompleteWithResult(true);
                    FinishRun(true, true);
                    SetFlowPhase(DemoFlowPhase.RunResult);
                }
                else
                {
                    SetFlowPhase(DemoFlowPhase.JourneyMap);
                }
                return;
            }

            DemoMapNode completedBattle = run.Map.CurrentNode;
            lastReachedLayer = completedBattle.Layer;
            bool defeatedBoss = completedBattle.Type == DemoNodeType.Boss || battle.IsBossBattle;
            run.CurrentHealth = battle.Player.Health;
            run.RecordBattleVictory(battle.MaxSwordsReached, battle.HighestBurstDamage);

            if (defeatedBoss)
            {
                battle.ClearBattle();
                run.Map.CompleteWithResult(true);
                FinishRun(true, true);
                SetFlowPhase(DemoFlowPhase.RunResult);
                return;
            }

            DemoRewardContext context = DemoRewardContext.FromNode(completedBattle, run, ++rewardSeed);
            currentRewardContext = context;
            currentRewards = rewards.CreateChoices(context, run);
            awaitingBattleReward = currentRewards.Count > 0;
            battle.ClearBattle();

            if (!awaitingBattleReward)
            {
                run.Map.CompleteCurrentNode();
                EnterCurrentNode();
                return;
            }

            SetFlowPhase(DemoFlowPhase.RewardChoice);
        }

        private void HandleBattleLost()
        {
            if (battleResultHandled)
            {
                return;
            }

            battleResultHandled = true;
            BattleSpeed = 1f;
            if (journeySession != null && activeJourneyBattleNode != null)
            {
                lastReachedLayer = (activeJourneyBattleNode.ActIndex - 1) * 8
                    + activeJourneyBattleNode.DepthIndex + 1;
            }
            else
            {
                lastReachedLayer = run.Map.CurrentNode.Layer;
            }
            run.CurrentHealth = 0;
            run.RecordSwordCount(battle.MaxSwordsReached);
            run.RecordBurstDamage(battle.HighestBurstDamage);
            battle.ClearBattle();
            activeJourneyBattleNode = null;
            pendingJourneyNode = null;
            pendingEncounter = null;
            run.Map.CompleteWithResult(false);
            FinishRun(false, false);
            SetFlowPhase(DemoFlowPhase.RunResult);
        }
        private void OpenRewardsForNode(DemoMapNode node)
        {
            currentRewardContext = DemoRewardContext.FromNode(node, run, ++rewardSeed);
            currentRewards = rewards.CreateChoices(currentRewardContext, run);
            awaitingBattleReward = false;
        }

        private void OpenUtilityNode(DemoMapNode node)
        {
            currentRewardContext = DemoRewardContext.FromNode(node, run, ++rewardSeed);
            currentRewards = rewards.CreateChoices(currentRewardContext, run);

            if (!string.IsNullOrEmpty(node.ActionProfileId)
                && DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out DemoNodeActionProfileDefinition action))
            {
                if (!string.IsNullOrEmpty(action.GuaranteedComponentId))
                {
                    DemoReward guaranteed = rewards.CreateGuaranteedReward(action.GuaranteedComponentId, run);
                    if (currentRewards.Count == 0)
                    {
                        currentRewards.Add(guaranteed);
                    }
                    else
                    {
                        int matchingSlot = currentRewards.FindIndex(candidate => candidate.Slot == guaranteed.Slot);
                        currentRewards[matchingSlot >= 0 ? matchingSlot : 0] = guaranteed;
                    }
                }

                if (action.HealAmount > 0)
                {
                    run.Heal(action.HealAmount);
                }
            }

            if (currentRewards.Count == 0)
            {
                run.Map.CompleteCurrentNode();
                EnterCurrentNode();
                return;
            }

            SetFlowPhase(node.Type == DemoNodeType.Training
                ? DemoFlowPhase.Training
                : DemoFlowPhase.Preparation);
        }

        private void FinishRun(bool victory, bool defeatedBoss)
        {
            if (runSummary != null)
            {
                return;
            }

            runSummary = run.CreateSummary(victory, defeatedBoss, lastReachedLayer);

            metaProgress.RecordRun(runSummary);
            foreach (DemoCard card in run.Deck)
            {
                metaProgress.RecordCard(card.Id);
            }

            foreach (DemoGongfaType gongfa in run.GetLearnedGongfas())
            {
                metaProgress.RecordGongfa(gongfa.ToString());
            }

            foreach (DemoArtifactType artifact in run.Artifacts)
            {
                metaProgress.RecordArtifact(artifact.ToString());
            }

            metaStore.Save(metaProgress);
        }

        public string GetRunResultSummary()
        {
            if (runSummary == null)
            {
                return string.Empty;
            }

            string outcome = runSummary.Victory ? "天劫已渡，道途已成" : "此世止步，道基归卷";
            string unlock = runSummary.NewUnlocks.Count > 0
                ? "\n新解锁：残剑道痕 · 下一世首战所得可重铸一次"
                : string.Empty;
            int durationSeconds = Mathf.Max(0, Mathf.RoundToInt(runSummary.DurationSeconds));
            string routeLine = runSummary.RouteHistory.Count > 0
                ? "\n前路：" + string.Join(" → ", runSummary.RouteHistory.Select(route => route.RouteName))
                : string.Empty;
            string failureLine = !runSummary.Victory && !string.IsNullOrEmpty(runSummary.FailureNodeName)
                ? $"\n止步：{runSummary.FailureNodeName}"
                : string.Empty;
            return
                $"{outcome}\n" +
                $"抵达：第 {runSummary.ReachedLayer} 层  ·  胜战 {runSummary.BattlesWon}  ·  用时 {durationSeconds / 60:00}:{durationSeconds % 60:00}\n" +
                $"最大飞剑：{runSummary.MaxSwordCount}  ·  最高爆发：{runSummary.HighestBurstDamage}\n" +
                $"主修：{runSummary.MainGongfaName}  ·  法器：{runSummary.CoreArtifactName}" +
                routeLine + failureLine +
                unlock;
        }
        private void ClaimReward(DemoReward reward)
        {
            if (reward == null)
            {
                return;
            }

            if (reward.Type == DemoRewardType.Trace)
            {
                run.EquippedTraceId = reward.TraceId ?? string.Empty;
                run.OpeningRewardRerolls = reward.TraceId == DemoMetaProgress.BrokenSwordTraceId ? 1 : 0;
                openingStage = DemoOpeningStage.SelectRoot;
                PrepareOpeningChoices();
                return;
            }

            if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                if (!reward.Root.IsAvailable)
                {
                    return;
                }

                run.SetRoot(reward.Root);
                openingStage = DemoOpeningStage.SelectOpeningItem;
                PrepareOpeningChoices();
                return;
            }

            DemoJourneyVesselDefinition vessel = reward.Vessel;
            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && vessel == null && reward.JourneyLine != null)
            {
                DemoConfigRepository.TryGetJourneyVessel(reward.JourneyLine.Id, out vessel);
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && vessel != null)
            {
                if (!vessel.IsAvailable)
                {
                    return;
                }

                run.SetVessel(vessel);
                openingStage = DemoOpeningStage.SelectOpeningScene;
                PrepareOpeningChoices();
                return;
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                if (!reward.Region.IsAvailable || !run.TrySetFirstRegion(reward.Region))
                {
                    return;
                }

                openingStage = DemoOpeningStage.Complete;
                currentRewards.Clear();
                if (!BeginConfiguredJourney(reward.Region))
                {
                    run.Map.SetOpeningBattle(
                        BuildOpeningEncounterId(reward.Region),
                        BuildOpeningBattleName(reward.Region),
                        "reward_opening_battle");
                    run.Map.CompleteCurrentNode();
                    EnterCurrentNode();
                }
                return;
            }

            if (reward.Type == DemoRewardType.Route && reward.RoutePlan != null)
            {
                currentRewards.Clear();
                currentRewardContext = null;
                run.Map.SelectRoute(reward.RoutePlan);
                EnterCurrentNode();
                return;
            }

            bool focusSelection = DemoRewardService.IsFocusComponent(reward);
            if (reward.Type == DemoRewardType.Card && reward.Card != null)
            {
                run.AddCard(reward.Card);
                metaProgress.RecordCard(reward.Card.Id);
            }
            else if (reward.Type == DemoRewardType.Gongfa)
            {
                run.LearnGongfa(reward.GongfaType);
                metaProgress.RecordGongfa(reward.GongfaType.ToString());
            }
            else if (reward.Type == DemoRewardType.Artifact)
            {
                run.AddArtifact(reward.ArtifactType);
                metaProgress.RecordArtifact(reward.ArtifactType.ToString());
            }
            else if (reward.Type == DemoRewardType.Heal)
            {
                run.Heal(DemoConfigRepository.GetIntConstant("battle", "heal_reward_amount", 18));
            }
            else if (reward.Type == DemoRewardType.Upgrade)
            {
                run.UpgradeEnergy();
            }
            else
            {
                return;
            }

            run.RecordRewardSelection(focusSelection);
            metaStore.Save(metaProgress);
            currentRewards.Clear();
            currentRewardContext = null;

            if (awaitingBattleReward)
            {
                awaitingBattleReward = false;
                run.Map.CompleteCurrentNode();
                EnterCurrentNode();
                return;
            }

            run.Map.CompleteCurrentNode();
            EnterCurrentNode();
        }
        private void CompleteUtilityNode()
        {
            if (run.Map.CurrentNode.Type == DemoNodeType.Start)
            {
                PrepareOpeningChoices();
                return;
            }

            run.Map.CompleteCurrentNode();
            EnterCurrentNode();
        }
        private static bool IsAdvanceNode(DemoNodeType type)
        {
            return type == DemoNodeType.Start || type == DemoNodeType.Training || type == DemoNodeType.Shop;
        }

        private static string GetNodeTypeLabel(DemoNodeType type)
        {
            switch (type)
            {
                case DemoNodeType.Start:
                    return "起点";
                case DemoNodeType.RouteChoice:
                    return "路线";
                case DemoNodeType.Battle:
                    return "战斗";
                case DemoNodeType.Reward:
                    return "奖励";
                case DemoNodeType.Shop:
                    return "整备";
                case DemoNodeType.Training:
                    return "修炼";
                case DemoNodeType.Boss:
                    return "Boss";
                case DemoNodeType.Victory:
                case DemoNodeType.Result:
                    return "结算";
                default:
                    return type.ToString();
            }
        }

        private static string GetPhaseLabel(DemoBattlePhase phase)
        {
            switch (phase)
            {
                case DemoBattlePhase.Intro:
                    return "入阵";
                case DemoBattlePhase.Running:
                    return "斗法";
                case DemoBattlePhase.Won:
                    return "胜利";
                case DemoBattlePhase.Lost:
                    return "失败";
                default:
                    return phase.ToString();
            }
        }
        private static string GetBossPhaseLabel(DemoBossPhase phase)
        {
            switch (phase)
            {
                case DemoBossPhase.ThunderCloud:
                    return "雷云压境";
                case DemoBossPhase.SoulLock:
                    return "天雷锁魂";
                case DemoBossPhase.CalamityDescends:
                    return "天劫降临";
                default:
                    return "无";
            }
        }

        private string GetUtilityActionLabel(DemoNodeType type)
        {
            if (HasRunResult)
            {
                return "再启一世";
            }

            switch (type)
            {
                case DemoNodeType.Start:
                    if (openingStage == DemoOpeningStage.SelectTrace)
                    {
                        return "整备道痕";
                    }

                    if (openingStage == DemoOpeningStage.SelectRoot)
                    {
                        return "定下根脚";
                    }

                    if (openingStage == DemoOpeningStage.SelectOpeningItem)
                    {
                        return "带此物上路";
                    }

                    if (openingStage == DemoOpeningStage.SelectOpeningScene)
                    {
                        return "定下所往";
                    }

                    return "踏入历练";
                case DemoNodeType.Training:
                    return "接受补强";
                case DemoNodeType.Shop:
                    return "完成整备";
                default:
                    return string.Empty;
            }
        }

        private string GetBattleActionLabel()
        {
            if (!HasBattle)
            {
                return string.Empty;
            }

            if (battle.Phase == DemoBattlePhase.Intro || battle.Phase == DemoBattlePhase.Running)
            {
                return BattleSpeed <= 0.01f ? "继续斗法" : "暂停";
            }

            return string.Empty;
        }

        private void ResetRun()
        {
            BattleSpeed = 1f;
            Time.timeScale = 1f;
            battle.ClearBattle(true);
            run = new DemoRunState();
            runSummary = null;
            currentRewards.Clear();
            currentRewardContext = null;
            awaitingBattleReward = false;
            battleResultHandled = false;
            battleOutcomeDelay = 0f;
            rewardSeed = 0;
            lastReachedLayer = 0;
            pendingEncounter = null;
            pendingJourneyNode = null;
            activeJourneyBattleNode = null;
            journeySession = null;
            journeySaveStore = null;
            journeyError = string.Empty;
            openingStage = metaProgress.HasUnlock(DemoMetaProgress.BrokenSwordTraceId)
                ? DemoOpeningStage.SelectTrace
                : DemoOpeningStage.SelectRoot;
            SetFlowPhase(DemoFlowPhase.Home);
            EnterCurrentNode();
        }
        private IEnumerable<string> GetGongfaNames()
        {
            yield return run.MainGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.MainGongfa).Name : "未定主修";
            yield return run.SupportGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.SupportGongfa).Name : "未定辅修";
            yield return run.DivineGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.DivineGongfa).Name : "未悟神通";
        }

        private void PrepareOpeningChoices()
        {
            currentRewards.Clear();

            if (run.Map.CurrentNode.Type != DemoNodeType.Start)
            {
                return;
            }

            if (openingStage == DemoOpeningStage.Complete)
            {
                run.Map.CompleteCurrentNode();
                EnterCurrentNode();
                return;
            }

            if (openingStage == DemoOpeningStage.SelectTrace)
            {
                currentRewards.Add(DemoReward.Trace(string.Empty, "不携道痕", "保持这一世原本的奖励次序，不追加局外战力。"));
                currentRewards.Add(DemoReward.Trace(
                    DemoMetaProgress.BrokenSwordTraceId,
                    "残剑道痕",
                    "首战所得可重铸一次；只改变选择，不继承上一世战力。"));
                SetFlowPhase(DemoFlowPhase.OpeningTrace);
                return;
            }

            if (openingStage == DemoOpeningStage.SelectRoot)
            {
                List<DemoRootDefinition> roots = DemoConfigRepository.GetRootsForOpening(4);
                currentRewards = roots.Count == 0
                    ? BuildFallbackRootChoices()
                    : roots.Select(DemoReward.FromRoot).ToList();
                SetFlowPhase(DemoFlowPhase.OpeningRoot);
                return;
            }

            if (openingStage == DemoOpeningStage.SelectOpeningItem && run.OpeningSelection.Root != null)
            {
                List<DemoJourneyVesselDefinition> vessels = DemoConfigRepository.GetJourneyVesselsForRoot(
                    run.OpeningSelection.Root.Id,
                    3,
                    true);
                if (vessels.Count == 0)
                {
                    currentRewards = BuildFallbackJourneyChoices(run.OpeningSelection.Root);
                    SetFlowPhase(DemoFlowPhase.OpeningVessel);
                    return;
                }

                currentRewards = vessels
                    .Select(vessel => DemoReward.FromVessel(vessel, run.OpeningSelection.Root))
                    .ToList();
                SetFlowPhase(DemoFlowPhase.OpeningVessel);
                return;
            }

            if (openingStage == DemoOpeningStage.SelectOpeningScene && run.OpeningSelection.Vessel != null)
            {
                currentRewards = BuildOpeningSceneChoices(run.OpeningSelection.Vessel);
                SetFlowPhase(DemoFlowPhase.OpeningRegion);
                return;
            }

            if (run.OpeningSelection.Root == null)
            {
                openingStage = DemoOpeningStage.SelectRoot;
                currentRewards = BuildFallbackRootChoices();
                SetFlowPhase(DemoFlowPhase.OpeningRoot);
                return;
            }

            if (run.OpeningSelection.Vessel == null)
            {
                openingStage = DemoOpeningStage.SelectOpeningItem;
                currentRewards = BuildFallbackJourneyChoices(run.OpeningSelection.Root);
                SetFlowPhase(DemoFlowPhase.OpeningVessel);
                return;
            }

            if (run.OpeningSelection.FirstRegion == null)
            {
                openingStage = DemoOpeningStage.SelectOpeningScene;
                currentRewards = BuildOpeningSceneChoices(run.OpeningSelection.Vessel);
                SetFlowPhase(DemoFlowPhase.OpeningRegion);
                return;
            }

            openingStage = DemoOpeningStage.Complete;
            run.Map.CompleteCurrentNode();
            EnterCurrentNode();
        }

        private void SetFlowPhase(DemoFlowPhase next)
        {
            if (FlowPhase == next)
            {
                return;
            }

            DemoFlowSnapshot snapshot = run.Flow.TransitionToRun(next, run);
            FlowPhaseChanged?.Invoke(snapshot.Phase);
        }

        private List<DemoReward> BuildOpeningSceneChoices(DemoJourneyVesselDefinition vessel)
        {
            List<DemoReward> choices = GetOpeningSceneCandidates(vessel)
                .Take(3)
                .Select(region => DemoReward.OpeningScene(region, vessel))
                .ToList();

            if (choices.Count == 0 && DemoConfigRepository.TryGetRegion("region_old_mine", out DemoRegionDefinition oldMine))
            {
                choices.Add(DemoReward.OpeningScene(oldMine, vessel));
            }

            return choices;
        }

        private IEnumerable<DemoRegionDefinition> GetOpeningSceneCandidates(DemoJourneyVesselDefinition vessel)
        {
            if (vessel == null)
            {
                yield break;
            }

            HashSet<string> yieldedIds = new HashSet<string>(System.StringComparer.OrdinalIgnoreCase);
            foreach (string regionId in vessel.RegionCandidateIds ?? new List<string>())
            {
                if (DemoConfigRepository.TryGetRegion(regionId, out DemoRegionDefinition region) && yieldedIds.Add(region.Id))
                {
                    yield return region;
                }
            }

            if (yieldedIds.Count == 0
                && DemoConfigRepository.TryGetRegion(vessel.FirstRegionId, out DemoRegionDefinition primary)
                && yieldedIds.Add(primary.Id))
            {
                yield return primary;
            }
        }

        private static string BuildOpeningEncounterId(DemoRegionDefinition region)
        {
            switch (region?.Id)
            {
                case "region_old_mine":
                    return "enemy_old_mine_entry";
                case "region_thunder_marsh":
                    return "enemy_thunder_marsh_entry";
                case "region_herb_forest":
                    return "enemy_herb_forest_entry";
                case "region_trade_road":
                    return "enemy_trade_road_entry";
                case "region_ancestral_vault":
                    return "enemy_ancestral_vault_entry";
                case "region_demon_tower":
                    return "enemy_demon_tower_entry";
                default:
                    return "enemy_old_mine_entry";
            }
        }

        private static string BuildOpeningBattleName(DemoRegionDefinition region)
        {
            switch (region?.Id)
            {
                case "region_old_mine":
                    return "旧矿入口遭遇";
                case "region_thunder_marsh":
                    return "雷泽浅滩遭遇";
                case "region_herb_forest":
                    return "药谷雾林遭遇";
                case "region_trade_road":
                    return "荒谷驿路遭遇";
                case "region_ancestral_vault":
                    return "旧库门前遭遇";
                case "region_demon_tower":
                    return "妖塔外层遭遇";
                default:
                    return "初行遭遇";
            }
        }
        private List<DemoReward> BuildFallbackRootChoices()
        {
            return new List<DemoReward>
            {
                DemoReward.FromRoot(new DemoRootDefinition
                {
                    Id = "fallback_root_menial",
                    Name = "山门杂役",
                    IsAvailable = false,
                    Rarity = "common",
                    UnlockCondition = "默认开放",
                    IsDefaultPool = true,
                    Summary = "在山门最底层长大，起手更稳，也更知道如何从碎活里找出自己的第一条路。"
                }),
                DemoReward.FromRoot(new DemoRootDefinition
                {
                    Id = "fallback_root_smith",
                    Name = "铁坊子",
                    IsAvailable = false,
                    Rarity = "common",
                    UnlockCondition = "默认开放",
                    IsDefaultPool = true,
                    Summary = "看着炉火与兵刃长大，对器物和飞剑更敏感，容易在前期抓到能起势的那一件。"
                }),
                DemoReward.FromRoot(new DemoRootDefinition
                {
                    Id = "fallback_root_branch",
                    Name = "世家旁支",
                    IsAvailable = true,
                    Rarity = "common",
                    UnlockCondition = "默认开放",
                    IsDefaultPool = true,
                    Summary = "出身不高不低，却见过不少规矩与门道，起手选择更容易往完整构筑收束。"
                })
            };
        }

        private List<DemoReward> BuildFallbackJourneyChoices(DemoRootDefinition root)
        {
            List<DemoJourneyVesselDefinition> vessels = new List<DemoJourneyVesselDefinition>
            {
                new DemoJourneyVesselDefinition
                {
                    Id = "vessel_branch_sword_embryo",
                    RootId = root?.Id,
                    Name = "残剑胚",
                    OriginText = "祖龛坍塌后，匣中只余一枚未成的剑胚。它与你腕上的旧伤一同发热。",
                    VesselType = "sword",
                    StarterPoolId = "starter_sword_embryo",
                    BaseStyle = "wanjian",
                    StartingEffectText = "一把飞剑与五张基础剑术从此随你上路。",
                    FirstRegionId = "region_old_mine",
                    RegionCandidateIds = new List<string> { "region_old_mine", "region_ancestral_vault", "region_herb_forest" },
                    RiskLevel = "stable",
                    SummaryTags = new List<string> { "flying_sword", "sword_intent", "old_debt" },
                    IsAvailable = true
                },
                new DemoJourneyVesselDefinition
                {
                    Id = "vessel_branch_herb_caldron",
                    RootId = root?.Id,
                    Name = "百草旧鼎",
                    OriginText = "旁支药房被查封前，婶母把一口满是药痕的旧鼎塞入你怀中。",
                    VesselType = "cauldron",
                    StarterPoolId = "starter_caldron_basic",
                    BaseStyle = "general",
                    StartingEffectText = "炼化纵切尚未开放。",
                    FirstRegionId = "region_herb_forest",
                    RegionCandidateIds = new List<string> { "region_herb_forest", "region_trade_road", "region_old_mine" },
                    RiskLevel = "stable",
                    IsAvailable = false
                },
                new DemoJourneyVesselDefinition
                {
                    Id = "vessel_branch_blank_grimoire",
                    RootId = root?.Id,
                    Name = "无字法书",
                    OriginText = "族谱夹层里藏着一本无字法书，每逢雷雨便浮出陌生字迹。",
                    VesselType = "spellbook",
                    StarterPoolId = "starter_spellbook_basic",
                    BaseStyle = "general",
                    StartingEffectText = "法书纵切尚未开放。",
                    FirstRegionId = "region_ancestral_vault",
                    RegionCandidateIds = new List<string> { "region_ancestral_vault", "region_thunder_marsh", "region_old_mine" },
                    RiskLevel = "medium",
                    IsAvailable = false
                }
            };

            return vessels.Select(vessel => DemoReward.FromVessel(vessel, root)).ToList();
        }

        private static int CreateJourneySeed()
        {
            unchecked
            {
                int seed = Environment.TickCount ^ (int)DateTime.UtcNow.Ticks;
                seed &= 0x7fffffff;
                return seed == 0 ? 1 : seed;
            }
        }

        private DemoJourneyRunSessionOptions BuildJourneySessionOptions(DemoRegionDefinition region, int seed)
        {
            DemoStartingPracticePackageDefinition package = run.OpeningSelection.StartingPracticePackage;
            return new DemoJourneyRunSessionOptions
            {
                RunId = "old_mine_" + seed + "_" + DateTime.UtcNow.Ticks,
                ConfigSchemaVersion = JourneyConfigSchemaVersion,
                ContentVersion = JourneyContentVersion,
                MapAlgorithmVersion = JourneyMapAlgorithmVersion,
                RegionId = region.Id,
                Build = new DemoRunBuildSnapshot
                {
                    StartingPracticePackageId = package?.Id ?? string.Empty,
                    MindMethodId = run.CorePractice.DefinitionId,
                    MindMethodLevel = Math.Max(1, run.CorePractice.Level),
                    InnateArtifactId = run.InnateArtifact.DefinitionId,
                    InnateArtifactRefinementStage = Math.Max(1, run.InnateArtifact.RefinementStage),
                    TechniqueIds = run.Deck
                        .Where(card => card != null && !string.IsNullOrWhiteSpace(card.Id))
                        .Select(card => card.Id)
                        .Distinct(StringComparer.Ordinal)
                        .ToList()
                },
                Realm = BuildRealmSnapshot(),
                MaxHealth = run.MaxHealth,
                CurrentHealth = run.CurrentHealth
            };
        }

        private DemoRunRealmSnapshot BuildRealmSnapshot()
        {
            return new DemoRunRealmSnapshot
            {
                RealmId = run.Realm.RealmId,
                Stage = Math.Max(1, run.Realm.Stage),
                FoundationRuleId = run.Realm.FoundationRuleId,
                BreakthroughSourceIds = run.Realm.BreakthroughSourceIds
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .Distinct(StringComparer.Ordinal)
                    .ToList()
            };
        }

        private DemoRunBuildSnapshot BuildPracticeSnapshot()
        {
            return new DemoRunBuildSnapshot
            {
                StartingPracticePackageId = run.OpeningSelection.StartingPracticePackage?.Id ?? string.Empty,
                MindMethodId = run.CorePractice.DefinitionId,
                MindMethodLevel = Math.Max(1, run.CorePractice.Level),
                InnateArtifactId = run.InnateArtifact.DefinitionId,
                InnateArtifactRefinementStage = Math.Max(1, run.InnateArtifact.RefinementStage),
                TechniqueIds = run.Deck
                    .Where(card => card != null && !string.IsNullOrWhiteSpace(card.Id))
                    .Select(card => card.Id)
                    .Distinct(StringComparer.Ordinal)
                    .Take(9)
                    .ToList()
            };
        }

        private void RestoreRunFromJourneySnapshot(DemoRunSaveV2 snapshot)
        {
            run.RestoreJourneyState(snapshot);
        }

        private string ResolveJourneyEncounterId(DemoJourneyNode node)
        {
            if (node == null)
            {
                return "enemy_old_mine_entry";
            }

            if (node.Type == DemoJourneyNodeType.Boss)
            {
                return "enemy_xuantie_mine_sword_puppet";
            }

            if (node.Type == DemoJourneyNodeType.MiniBoss)
            {
                return node.ActIndex == 1
                    ? "enemy_ironback_mine_beast"
                    : "enemy_sword_eating_mine_spirit";
            }

            if (node.ActIndex == 1)
            {
                return node.Type == DemoJourneyNodeType.Elite
                    ? "enemy_old_mine_contract_guard"
                    : "enemy_old_mine_entry";
            }

            if (node.ActIndex == 2)
            {
                return node.Type == DemoJourneyNodeType.Elite
                    ? "enemy_collapsed_well_guard"
                    : "enemy_old_mine_wraith";
            }

            return node.Type == DemoJourneyNodeType.Elite
                ? "enemy_sword_furnace_elite"
                : "enemy_sword_furnace_guard";
        }

        private static DemoMapNode ToLegacyEncounterNode(DemoJourneyNode node, string encounterId)
        {
            DemoNodeType type = node.Type == DemoJourneyNodeType.Boss
                ? DemoNodeType.Boss
                : DemoNodeType.Battle;
            return new DemoMapNode(
                node.ActIndex,
                type,
                node.Name,
                node.NodeId,
                encounterId,
                string.Empty,
                string.Empty);
        }

        private IReadOnlyList<DemoBattleEnemySetup> BuildJourneyEnemySet(
            DemoJourneyNode node,
            DemoEnemyDefinition primary)
        {
            int baseHealth = primary?.MaxHealth ?? (node.Type == DemoJourneyNodeType.Boss ? 3200 : 130 + node.ActIndex * 55);
            string baseId = primary?.Id ?? ResolveJourneyEncounterId(node);
            string baseName = primary?.Name ?? node.Name;

            if (node.Type == DemoJourneyNodeType.Boss)
            {
                return new[]
                {
                    new DemoBattleEnemySetup
                    {
                        CombatantId = baseId + "_armor",
                        DefinitionId = baseId,
                        Name = "玄铁甲片",
                        PositionId = "boss_upper_armor",
                        Depth = 0,
                        MaxHealth = Math.Max(1, baseHealth * 38 / 100),
                        ThreatPriority = 3,
                        RequiredForVictory = true
                    },
                    new DemoBattleEnemySetup
                    {
                        CombatantId = baseId + "_contract_spike",
                        DefinitionId = "target_contract_spike",
                        Name = "朱砂契钉",
                        PositionId = "boss_contract_spike",
                        Depth = 1,
                        MaxHealth = Math.Max(1, baseHealth * 24 / 100),
                        ThreatPriority = 4,
                        RequiredForVictory = true
                    },
                    new DemoBattleEnemySetup
                    {
                        CombatantId = baseId + "_core",
                        DefinitionId = "target_sword_furnace_core",
                        Name = "剑炉核心",
                        PositionId = "boss_furnace_core",
                        Depth = 2,
                        MaxHealth = Math.Max(1, baseHealth * 38 / 100),
                        ThreatPriority = 5,
                        RequiredForVictory = true
                    }
                };
            }

            int count = node.Type == DemoJourneyNodeType.Elite || (node.ActIndex >= 2 && node.Type == DemoJourneyNodeType.Battle)
                ? 3
                : node.Type == DemoJourneyNodeType.MiniBoss ? 2 : 1 + Math.Abs(node.NodeId.GetHashCode()) % 2;
            List<DemoBattleEnemySetup> enemies = new List<DemoBattleEnemySetup>();
            for (int i = 0; i < count; i++)
            {
                int health = count == 1 ? baseHealth : Math.Max(24, baseHealth / count + i * 8);
                enemies.Add(new DemoBattleEnemySetup
                {
                    CombatantId = baseId + "_" + (i + 1),
                    DefinitionId = baseId,
                    Name = i == 0 ? baseName : BuildJourneySupportEnemyName(node.ActIndex, i),
                    PositionId = i == 0 ? "enemy_front" : i == 1 ? "enemy_middle" : "enemy_rear",
                    Depth = i,
                    MaxHealth = health,
                    ThreatPriority = count - i,
                    RequiredForVictory = true
                });
            }
            return enemies;
        }

        private static string BuildJourneySupportEnemyName(int actIndex, int index)
        {
            if (actIndex == 1)
            {
                return index == 1 ? "契屑矿蜉" : "浅道矿兽";
            }

            if (actIndex == 2)
            {
                return index == 1 ? "塌井残魂" : "吞剑幼魈";
            }

            return index == 1 ? "封契剑影" : "炉心构造体";
        }

        private void ApplyJourneyNodeImmediateEffect(DemoJourneyNode node)
        {
            if (node == null)
            {
                return;
            }

            switch (node.Type)
            {
                case DemoJourneyNodeType.Start:
                    run.Story.AddExperience("experience_entered_old_mine");
                    break;
                case DemoJourneyNodeType.Event:
                case DemoJourneyNodeType.Story:
                    string flag = node.ActIndex == 1
                        ? "experience_mine_contract_clue"
                        : node.ActIndex == 2
                            ? "experience_miner_spirit_helped"
                            : "experience_old_contract_witnessed";
                    run.Story.AddExperience(flag);
                    if (node.ActIndex == 2)
                    {
                        run.Heal(8);
                    }
                    break;
                case DemoJourneyNodeType.Cultivation:
                    run.CorePractice.Level++;
                    TryGrantJourneyTechnique(node, node.ActIndex == 1 ? "sword_focus" : node.ActIndex == 2 ? "returning_array" : "wanjian_burst");
                    run.Heal(10);
                    break;
                case DemoJourneyNodeType.Secret:
                    TryGrantJourneyTechnique(node, node.ActIndex == 1 ? "summon_sword" : node.ActIndex == 2 ? "sword_rain" : "sword_tide");
                    run.Story.AddExperience("experience_secret_act_" + node.ActIndex);
                    break;
                case DemoJourneyNodeType.Refinement:
                    run.InnateArtifact.RefinementStage++;
                    run.BonusPermanentSwords = Math.Max(run.BonusPermanentSwords, run.InnateArtifact.RefinementStage - 1);
                    run.Heal(6);
                    break;
                case DemoJourneyNodeType.Breakthrough:
                    run.Realm.RealmId = "realm_foundation_establishment";
                    run.Realm.Stage = 2;
                    run.Realm.FoundationRuleId = ResolveFoundationRuleId();
                    if (!run.Realm.BreakthroughSourceIds.Contains(node.ContentId))
                    {
                        run.Realm.BreakthroughSourceIds.Add(node.ContentId);
                    }
                    run.MaxHealth += 12;
                    run.CurrentHealth = run.MaxHealth;
                    run.BonusEnergy += 1;
                    run.Story.AddExperience("experience_foundation_established");
                    break;
            }
        }

        private void TryGrantJourneyTechnique(DemoJourneyNode node, string techniqueId)
        {
            if (run.Deck.Count >= 9 || !DemoConfigRepository.TryCreateCard(techniqueId, out DemoCard card))
            {
                return;
            }

            if (run.Deck.Any(existing => string.Equals(existing.Id, techniqueId, StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            run.AddCard(card);
            run.Techniques.Add(new DemoTechniqueState
            {
                DefinitionId = techniqueId,
                Level = 1,
                SourceNodeId = node.NodeId
            });
        }

        private string ResolveFoundationRuleId()
        {
            if (run.Story.HasExperience("experience_miner_spirit_helped"))
            {
                return "foundation_clear_spirit";
            }

            if (run.Story.HasExperience("experience_secret_act_2"))
            {
                return "foundation_thunder_meridian";
            }

            return "foundation_sword_bone";
        }

        private DemoJourneyNodeOutcome BuildJourneyNodeOutcome(DemoJourneyNode node, bool battleVictory)
        {
            DemoJourneyNodeOutcome outcome = new DemoJourneyNodeOutcome
            {
                Build = BuildPracticeSnapshot(),
                Realm = BuildRealmSnapshot(),
                CurrentHealth = run.CurrentHealth,
                MaxHealth = run.MaxHealth,
                ElapsedSecondsDelta = battleVictory ? battle.ElapsedSeconds : 0f
            };
            foreach (string id in run.Story.ExperienceFlagIds)
            {
                outcome.AddExperienceFlag(id);
            }
            foreach (string id in run.Story.ConsumedUniqueContentIds)
            {
                outcome.ConsumeUniqueContent(id);
            }
            foreach (string id in run.Story.PendingMetaDiscoveryIds)
            {
                outcome.AddPendingMetaDiscovery(id);
            }

            if (node != null)
            {
                outcome.ConsumeUniqueContent(node.ContentId);
                if (node.Type == DemoJourneyNodeType.MiniBoss)
                {
                    string flag = node.ActIndex == 1
                        ? "experience_ironback_beast_defeated"
                        : "experience_sword_eater_defeated";
                    outcome.AddExperienceFlag(flag);
                    run.Story.AddExperience(flag);
                }
            }
            return outcome;
        }
        private static DemoSwordStyle ResolveJourneyStyle(DemoJourneyLineDefinition line)
        {
            if (line?.SummaryTags != null)
            {
                if (line.SummaryTags.Any(tag => tag == "thunder" || tag == "shock"))
                {
                    return DemoSwordStyle.Thunder;
                }

                if (line.SummaryTags.Any(tag => tag == "bleed" || tag == "risk_reward"))
                {
                    return DemoSwordStyle.Blood;
                }
            }

            return DemoSwordStyle.Wanjian;
        }

        private static DemoGongfaType GetMainGongfaForStyle(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    return DemoGongfaType.ThunderScripture;
                case DemoSwordStyle.Blood:
                    return DemoGongfaType.BloodFiendCanon;
                case DemoSwordStyle.Wanjian:
                default:
                    return DemoGongfaType.SwordControlArt;
            }
        }
    }
}
