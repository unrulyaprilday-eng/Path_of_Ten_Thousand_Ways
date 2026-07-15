using System;
using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Map
{
    public enum DemoNodeType
    {
        Start,
        RouteChoice,
        Battle,
        Reward,
        Shop,
        Training,
        Boss,
        Victory,
        Result
    }

    public sealed class DemoMapNode
    {
        public int Layer;
        public DemoNodeType Type;
        public string Name;
        public string NodeId;
        public string EncounterId;
        public string RewardProfileId;
        public string ActionProfileId;
        public bool Completed;

        public DemoMapNode(int layer, DemoNodeType type, string name)
            : this(layer, type, name, null, null, null, null)
        {
        }

        public DemoMapNode(
            int layer,
            DemoNodeType type,
            string name,
            string nodeId,
            string encounterId,
            string rewardProfileId,
            string actionProfileId)
        {
            Layer = layer;
            Type = type;
            Name = name;
            NodeId = nodeId;
            EncounterId = encounterId;
            RewardProfileId = rewardProfileId;
            ActionProfileId = actionProfileId;
        }

        public DemoMapNode Clone()
        {
            return new DemoMapNode(
                Layer,
                Type,
                Name,
                NodeId,
                EncounterId,
                RewardProfileId,
                ActionProfileId);
        }
    }

    public sealed class DemoMapRoutePlan
    {
        public string Id { get; }
        public string Name { get; }
        public string Description { get; }
        public List<DemoMapNode> Nodes { get; } = new List<DemoMapNode>();

        public DemoMapRoutePlan(string name, string description, params DemoMapNode[] nodes)
            : this(null, name, description, nodes)
        {
        }

        public DemoMapRoutePlan(string id, string name, string description, params DemoMapNode[] nodes)
        {
            Id = id;
            Name = name;
            Description = description;

            if (nodes == null)
            {
                return;
            }

            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != null)
                {
                    Nodes.Add(nodes[i]);
                }
            }
        }
    }

    public sealed class DemoMapRun
    {
        public List<DemoMapNode> Nodes { get; } = new List<DemoMapNode>();
        public int CurrentIndex { get; private set; }
        public bool? ResultVictory { get; private set; }

        public DemoMapNode CurrentNode => Nodes[CurrentIndex];
        public bool IsComplete => CurrentNode.Type == DemoNodeType.Victory || CurrentNode.Type == DemoNodeType.Result;
        public bool HasResult => ResultVictory.HasValue || IsComplete;
        public bool WasVictory => ResultVictory ?? CurrentNode.Type == DemoNodeType.Victory;

        public DemoMapRun()
        {
            Reset();
        }

        public void Reset()
        {
            Nodes.Clear();
            CurrentIndex = 0;
            ResultVictory = null;
            Nodes.Add(new DemoMapNode(
                0,
                DemoNodeType.Start,
                "定根脚",
                "node_opening_selection",
                null,
                null,
                "opening_selection"));
            Nodes.Add(new DemoMapNode(
                1,
                DemoNodeType.Battle,
                "旧矿入口遭遇",
                "node_opening_battle",
                "enemy_old_mine_entry",
                "reward_opening_battle",
                null));

            Nodes.Add(new DemoMapNode(
                1,
                DemoNodeType.RouteChoice,
                "旧矿岔路",
                "node_route_choice_layer_1",
                null,
                null,
                "choose_route_layer_1"));
        }

        public void CompleteCurrentNode()
        {
            if (CurrentNode.Type == DemoNodeType.RouteChoice || IsComplete)
            {
                return;
            }

            CurrentNode.Completed = true;

            if (CurrentIndex < Nodes.Count - 1)
            {
                CurrentIndex++;
                if (CurrentNode.Type == DemoNodeType.Victory || CurrentNode.Type == DemoNodeType.Result)
                {
                    ResultVictory = true;
                }
            }
        }

        public void CompleteWithResult(bool victory)
        {
            if (Nodes.Count == 0)
            {
                return;
            }

            CurrentNode.Completed = true;

            int resultIndex = CurrentIndex + 1;
            if (resultIndex < Nodes.Count)
            {
                Nodes.RemoveRange(resultIndex, Nodes.Count - resultIndex);
            }

            Nodes.Add(new DemoMapNode(
                Math.Max(4, CurrentNode.Layer + 1),
                DemoNodeType.Result,
                victory ? "一世修行完成" : "此世道途已断",
                victory ? "node_run_result_victory" : "node_run_result_defeat",
                null,
                null,
                victory ? "show_victory_result" : "show_defeat_result"));

            CurrentIndex = resultIndex;
            ResultVictory = victory;
        }

        public void SelectRoute(DemoMapRoutePlan routePlan)
        {
            if (routePlan == null || routePlan.Nodes.Count == 0 || CurrentNode.Type != DemoNodeType.RouteChoice)
            {
                return;
            }

            CurrentNode.Completed = true;

            int insertIndex = CurrentIndex + 1;
            for (int i = 0; i < routePlan.Nodes.Count; i++)
            {
                Nodes.Insert(insertIndex + i, routePlan.Nodes[i].Clone());
            }

            CurrentIndex = insertIndex;
        }

        public void SetOpeningBattle(
            string encounterId,
            string nodeName,
            string rewardProfileId = "reward_opening_battle")
        {
            for (int i = 0; i < Nodes.Count; i++)
            {
                if (Nodes[i].Type != DemoNodeType.Battle)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(nodeName))
                {
                    Nodes[i].Name = nodeName;
                }

                if (!string.IsNullOrWhiteSpace(encounterId))
                {
                    Nodes[i].EncounterId = encounterId;
                }

                if (!string.IsNullOrWhiteSpace(rewardProfileId))
                {
                    Nodes[i].RewardProfileId = rewardProfileId;
                }

                return;
            }
        }

        public void SetOpeningBattleName(string nodeName)
        {
            SetOpeningBattle(null, nodeName);
        }
    }
}
