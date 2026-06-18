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
        private readonly DemoRunState run = new DemoRunState();
        private readonly DemoBattleState battle = new DemoBattleState();
        private readonly DemoRewardService rewards = new DemoRewardService();
        private readonly DemoRouteRewardService routeRewards = new DemoRouteRewardService();
        private readonly DemoGongfaRewardService gongfaRewards = new DemoGongfaRewardService();
        private readonly DemoArtifactRewardService artifactRewards = new DemoArtifactRewardService();
        private List<DemoReward> currentRewards = new List<DemoReward>();
        private bool battleResultHandled;

        public DemoRunState Run => run;
        public DemoBattleState Battle => battle;
        public IReadOnlyList<DemoReward> CurrentRewards => currentRewards;
        public IEnumerable<string> BattleLogLines => battle.Log.Lines;
        public bool HasBattle => battle.Player != null && battle.Enemy != null;
        public bool HasPendingRewards => currentRewards.Count > 0;
        public bool IsRunComplete => run.Map.IsComplete;
        public bool CanAdvanceUtilityNode => currentRewards.Count == 0 && IsAdvanceNode(run.Map.CurrentNode.Type);
        public string UtilityActionLabel => GetUtilityActionLabel(run.Map.CurrentNode.Type);
        public string BattleActionLabel => GetBattleActionLabel();

        private void Start()
        {
            EnterCurrentNode();
        }

        private void Update()
        {
            battle.Tick(Time.deltaTime);

            if (battle.Phase == DemoBattlePhase.Won && !battleResultHandled)
            {
                battleResultHandled = true;
                run.CurrentHealth = battle.Player.Health;
                run.Map.CompleteCurrentNode();
                EnterCurrentNode();
            }

            if (battle.Phase == DemoBattlePhase.Lost && !battleResultHandled)
            {
                battleResultHandled = true;
                run.CurrentHealth = 0;
            }
        }

        public bool QueueCardAt(int handIndex)
        {
            return battle.QueueCard(handIndex);
        }

        public void TriggerBattleAction()
        {
            if (!HasBattle)
            {
                return;
            }

            if (battle.Phase == DemoBattlePhase.Planning)
            {
                battle.EndPlanning();
                return;
            }

            if (battle.Phase == DemoBattlePhase.Lost)
            {
                RestartRun();
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
            if (CanAdvanceUtilityNode)
            {
                CompleteUtilityNode();
            }
        }

        public string GetHeaderSummary()
        {
            string relicText = run.Relics.Count > 0 ? string.Join("、", run.Relics) : "暂无";
            string artifactText = run.Artifacts.Count > 0 ? string.Join("、", run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name)) : "暂无";
            string gongfaText = string.Join(" / ", GetGongfaNames());
            return $"当前节点：{run.Map.CurrentNode.Name} | 生命：{run.CurrentHealth}/{run.MaxHealth} | 功法：{gongfaText} | 法宝：{artifactText} | 遗物：{relicText}";
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
            if (!HasBattle)
            {
                switch (run.Map.CurrentNode.Type)
                {
                    case DemoNodeType.Start:
                        return "当前位于起点。先择定主修方向，再踏入第一场云海斗法。";
                    case DemoNodeType.RouteChoice:
                        return "当前位于路线分叉。挑出下一段历练，把风险、补强和 Boss 节奏握在自己手里。";
                    case DemoNodeType.Reward:
                        return "当前位于奖励节点。挑一项补强，把下一场演武推向更明确的流派高点。";
                    case DemoNodeType.Training:
                        return "当前位于修炼节点。补足功法或法宝，让 build 从散件开始收束。";
                    case DemoNodeType.Shop:
                        return "当前位于 Boss 前整备。优先补续航、补灵气，为天劫窗口留出爆发余地。";
                    case DemoNodeType.Victory:
                        return "这一局已经圆满收束，道途已成。";
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
                $"阶段：{GetPhaseLabel(battle.Phase)} | 倒计时：{battle.PhaseTimer:0.0}s | 回合：{battle.Round}\n" +
                $"玩家：HP {battle.Player.Health}/{battle.Player.MaxHealth} | 护盾 {battle.Player.Block} | 灵气 {battle.Energy}/{battle.MaxEnergy} | 剑意 {battle.Player.SwordIntent} | 感电 {battle.Player.Shock}\n" +
                $"飞剑：永久 {battle.PermanentSwords} + 临时 {battle.TemporarySwords} = {battle.TotalSwords} | 功法：{gongfaText} | 法宝：{artifactText}\n" +
                $"\n敌人：{battle.Enemy.Name} | HP {battle.Enemy.Health}/{battle.Enemy.MaxHealth} | 护盾 {battle.Enemy.Block}\n" +
                $"状态：感电 {battle.Enemy.Shock} | 流血 {battle.Enemy.Bleed}" +
                (battle.IsBossBattle ? $"\nBoss 阶段：{GetBossPhaseLabel(battle.BossPhase)} | 预警：{battle.BossIntentText}" : string.Empty);
        }

        public string GetHandStatus()
        {
            if (!HasBattle)
            {
                switch (run.Map.CurrentNode.Type)
                {
                    case DemoNodeType.Start:
                        return "当前不是战斗节点，先确定主修。";
                    case DemoNodeType.RouteChoice:
                        return "当前不是战斗节点，先决定下一段路线。";
                    case DemoNodeType.Reward:
                        return "当前不是战斗节点，先完成奖励选择。";
                    case DemoNodeType.Training:
                        return "当前不是战斗节点，先完成修炼补强。";
                    case DemoNodeType.Shop:
                        return "当前不是战斗节点，先完成 Boss 前整备。";
                    case DemoNodeType.Victory:
                        return "本局已结束，手牌区暂时关闭。";
                    default:
                        return "当前不是战斗节点，手牌区暂时关闭。";
                }
            }

            if (battle.Phase != DemoBattlePhase.Planning)
            {
                return "演武阶段中，飞剑与敌人正在结算。";
            }

            if (battle.Round == 1 && battle.Hand.Count <= 3)
            {
                return "首战只留一剑试锋：先看第一张牌怎么立道。";
            }

            return $"手牌 {battle.Hand.Count} 张 | 抽牌堆 {battle.DrawPile.Count} | 弃牌堆 {battle.DiscardPile.Count} | 已规划 {battle.PlayQueue.Count}";
        }

        public string GetRewardSummary()
        {
            if (currentRewards.Count == 0)
            {
                return "完成战斗或补强节点后出现三选一。";
            }

            if (currentRewards.All(reward => reward.Type == DemoRewardType.Route))
            {
                return "下一段路会直接改变节点顺序和成型速度，先挑路，再谈补强。";
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

            if (node.Type == DemoNodeType.Battle)
            {
                int health = node.Layer == 1 ? 46 : node.Layer == 2 ? 64 : 78;
                battleResultHandled = false;
                bool openingBattle = node.Layer == 1 && run.Map.CurrentIndex <= 2;
                battle.StartBattle(run.Deck, run.Artifacts, run.GetLearnedGongfas().ToList(), node.Name, health, false, run.CurrentHealth, run.BonusEnergy, run.BonusPermanentSwords, run.Relics, openingBattle);
            }
            else if (node.Type == DemoNodeType.Boss)
            {
                battleResultHandled = false;
                battle.StartBattle(run.Deck, run.Artifacts, run.GetLearnedGongfas().ToList(), "天劫化身", 150, true, run.CurrentHealth, run.BonusEnergy, run.BonusPermanentSwords, run.Relics);
            }
            else if (node.Type == DemoNodeType.RouteChoice)
            {
                battle.ClearBattle();
                currentRewards = routeRewards.CreateChoices(node.Layer, run);
            }
            else if (node.Type == DemoNodeType.Reward)
            {
                battle.ClearBattle();
                currentRewards = rewards.CreateChoices(node.Layer, run);
            }
            else
            {
                battle.ClearBattle(node.Type == DemoNodeType.Start);
            }
        }

        private void ClaimReward(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Route && reward.RoutePlan != null)
            {
                currentRewards.Clear();
                run.Map.SelectRoute(reward.RoutePlan);
                EnterCurrentNode();
                return;
            }

            if (reward.Type == DemoRewardType.Card && reward.Card != null)
            {
                run.AddCard(reward.Card);
            }
            else if (reward.Type == DemoRewardType.Gongfa)
            {
                run.LearnGongfa(reward.GongfaType);
            }
            else if (reward.Type == DemoRewardType.Artifact)
            {
                run.AddArtifact(reward.ArtifactType);
            }
            else if (reward.Type == DemoRewardType.Relic)
            {
                run.AddRelic(reward.Name);
            }
            else if (reward.Type == DemoRewardType.Heal)
            {
                run.Heal(18);
            }
            else if (reward.Type == DemoRewardType.Upgrade)
            {
                run.UpgradeEnergy();
            }

            currentRewards.Clear();
            run.Map.CompleteCurrentNode();
            EnterCurrentNode();
        }

        private void CompleteUtilityNode()
        {
            if (run.Map.CurrentNode.Type == DemoNodeType.Start)
            {
                currentRewards = gongfaRewards.CreateMainChoices();
                return;
            }

            if (run.Map.CurrentNode.Type == DemoNodeType.Training)
            {
                if (run.SupportGongfa == DemoGongfaType.None)
                {
                    currentRewards = gongfaRewards.CreateSupportChoices(run);
                }
                else
                {
                    currentRewards = artifactRewards.CreateChoices(run);
                }
                return;
            }

            if (run.Map.CurrentNode.Type == DemoNodeType.Shop)
            {
                if (run.DivineGongfa == DemoGongfaType.None)
                {
                    currentRewards = gongfaRewards.CreateDivineChoices(run);
                    currentRewards.Add(DemoReward.Heal());
                }
                else
                {
                    currentRewards = artifactRewards.CreateChoices(run)
                        .Where(reward => reward.Type != DemoRewardType.Heal)
                        .Take(2)
                        .ToList();
                    currentRewards.Add(DemoReward.Heal());
                }

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
                    return "结算";
                default:
                    return type.ToString();
            }
        }

        private static string GetPhaseLabel(DemoBattlePhase phase)
        {
            switch (phase)
            {
                case DemoBattlePhase.Planning:
                    return "规划";
                case DemoBattlePhase.Executing:
                    return "演武";
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
            switch (type)
            {
                case DemoNodeType.Start:
                    return "踏上剑道";
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

            if (battle.Phase == DemoBattlePhase.Planning)
            {
                return "开始演武";
            }

            if (battle.Phase == DemoBattlePhase.Lost)
            {
                return "重新开始";
            }

            return string.Empty;
        }

        private void RestartRun()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerable<string> GetGongfaNames()
        {
            yield return run.MainGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.MainGongfa).Name : "未定主修";
            yield return run.SupportGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.SupportGongfa).Name : "未定辅修";
            yield return run.DivineGongfa != DemoGongfaType.None ? DemoGongfaLibrary.Get(run.DivineGongfa).Name : "未悟神通";
        }
    }
}
