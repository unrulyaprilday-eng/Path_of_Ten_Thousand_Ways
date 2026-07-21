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
        public string Risk { get; }
        public List<DemoMapNode> Nodes { get; } = new List<DemoMapNode>();

        public DemoMapRoutePlan(string name, string description, params DemoMapNode[] nodes)
            : this(null, name, description, null, nodes)
        {
        }

        public DemoMapRoutePlan(string id, string name, string description, params DemoMapNode[] nodes)
            : this(id, name, description, null, nodes)
        {
        }

        public DemoMapRoutePlan(string id, string name, string description, string risk, params DemoMapNode[] nodes)
        {
            Id = id;
            Name = name;
            Description = description;
            Risk = risk;

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

    public sealed class DemoMapNodeRecord
    {
        public string NodeId { get; }
        public string Name { get; }
        public DemoNodeType Type { get; }
        public int Layer { get; }
        public string EncounterId { get; }
        public string RewardProfileId { get; }
        public string ActionProfileId { get; }
        public bool IsCompleted { get; private set; }
        public bool Succeeded { get; private set; }

        internal DemoMapNodeRecord(DemoMapNode node, bool? succeeded)
        {
            NodeId = node?.NodeId ?? string.Empty;
            Name = node?.Name ?? string.Empty;
            Type = node?.Type ?? DemoNodeType.Start;
            Layer = node?.Layer ?? 0;
            EncounterId = node?.EncounterId ?? string.Empty;
            RewardProfileId = node?.RewardProfileId ?? string.Empty;
            ActionProfileId = node?.ActionProfileId ?? string.Empty;
            IsCompleted = succeeded.HasValue;
            Succeeded = succeeded ?? false;
        }

        internal void Complete(bool succeeded)
        {
            IsCompleted = true;
            Succeeded = succeeded;
        }
    }

    public sealed class DemoMapRouteRecord
    {
        private readonly List<DemoMapNodeRecord> nodes = new List<DemoMapNodeRecord>();

        public string RouteId { get; }
        public string Name { get; }
        public int Layer { get; }
        public string Risk { get; }
        public IReadOnlyList<DemoMapNodeRecord> Nodes => nodes;

        internal DemoMapRouteRecord(DemoMapRoutePlan routePlan, int layer, string risk)
        {
            RouteId = routePlan?.Id ?? string.Empty;
            Name = routePlan?.Name ?? string.Empty;
            Layer = Math.Max(0, layer);
            Risk = risk ?? string.Empty;

            if (routePlan == null)
            {
                return;
            }

            for (int i = 0; i < routePlan.Nodes.Count; i++)
            {
                DemoMapNode node = routePlan.Nodes[i];
                if (node != null)
                {
                    nodes.Add(new DemoMapNodeRecord(node, null));
                }
            }
        }

        internal bool CompleteNode(DemoMapNode node, bool succeeded)
        {
            if (node == null)
            {
                return false;
            }

            for (int i = 0; i < nodes.Count; i++)
            {
                DemoMapNodeRecord record = nodes[i];
                if (!string.IsNullOrEmpty(node.NodeId)
                    && string.Equals(record.NodeId, node.NodeId, StringComparison.OrdinalIgnoreCase))
                {
                    record.Complete(succeeded);
                    return true;
                }
            }

            return false;
        }
    }

    public sealed class DemoMapRun
    {
        private readonly List<DemoMapRouteRecord> selectedRoutes = new List<DemoMapRouteRecord>();
        private readonly List<DemoMapNodeRecord> completedNodes = new List<DemoMapNodeRecord>();

        public List<DemoMapNode> Nodes { get; } = new List<DemoMapNode>();
        public int CurrentIndex { get; private set; }
        public bool? ResultVictory { get; private set; }
        public IReadOnlyList<DemoMapRouteRecord> SelectedRoutes => selectedRoutes;
        public IReadOnlyList<DemoMapNodeRecord> CompletedNodes => completedNodes;
        public DemoMapRouteRecord CurrentRoute => selectedRoutes.Count == 0
            ? null
            : selectedRoutes[selectedRoutes.Count - 1];
        public DemoMapNodeRecord FailedNode { get; private set; }

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
            selectedRoutes.Clear();
            completedNodes.Clear();
            CurrentIndex = 0;
            ResultVictory = null;
            FailedNode = null;
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
            if (CurrentNode.Type == DemoNodeType.RouteChoice || IsComplete || CurrentNode.Completed)
            {
                return;
            }

            CurrentNode.Completed = true;
            completedNodes.Add(new DemoMapNodeRecord(CurrentNode, true));
            CompleteSelectedRouteNode(CurrentNode, true);

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

            if (!CurrentNode.Completed)
            {
                CurrentNode.Completed = true;
                DemoMapNodeRecord completed = new DemoMapNodeRecord(CurrentNode, victory);
                completedNodes.Add(completed);
                CompleteSelectedRouteNode(CurrentNode, victory);
                if (!victory)
                {
                    FailedNode = completed;
                }
            }

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

        private void CompleteSelectedRouteNode(DemoMapNode node, bool succeeded)
        {
            for (int i = selectedRoutes.Count - 1; i >= 0; i--)
            {
                if (selectedRoutes[i].CompleteNode(node, succeeded))
                {
                    return;
                }
            }
        }

        public void SelectRoute(DemoMapRoutePlan routePlan)
        {
            SelectRoute(routePlan, routePlan?.Risk);
        }

        public void SelectRoute(DemoMapRoutePlan routePlan, string risk)
        {
            if (routePlan == null || routePlan.Nodes.Count == 0 || CurrentNode.Type != DemoNodeType.RouteChoice)
            {
                return;
            }

            CurrentNode.Completed = true;
            completedNodes.Add(new DemoMapNodeRecord(CurrentNode, true));
            selectedRoutes.Add(new DemoMapRouteRecord(
                routePlan,
                Math.Max(CurrentNode.Layer, routePlan.Nodes[0].Layer),
                string.IsNullOrWhiteSpace(risk) ? InferRouteRisk(routePlan.Id) : risk));

            int insertIndex = CurrentIndex + 1;
            for (int i = 0; i < routePlan.Nodes.Count; i++)
            {
                Nodes.Insert(insertIndex + i, routePlan.Nodes[i].Clone());
            }

            CurrentIndex = insertIndex;
        }

        private static string InferRouteRisk(string routeId)
        {
            if (string.IsNullOrWhiteSpace(routeId))
            {
                return string.Empty;
            }

            if (routeId.IndexOf("risky", StringComparison.OrdinalIgnoreCase) >= 0
                || routeId.IndexOf("aggressive", StringComparison.OrdinalIgnoreCase) >= 0
                || routeId.IndexOf("desperate", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "risky";
            }

            if (routeId.IndexOf("build", StringComparison.OrdinalIgnoreCase) >= 0
                || routeId.IndexOf("artifact", StringComparison.OrdinalIgnoreCase) >= 0
                || routeId.IndexOf("seclusion", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return "build";
            }

            return "stable";
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
