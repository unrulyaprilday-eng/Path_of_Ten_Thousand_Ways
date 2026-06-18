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
        Victory
    }

    public sealed class DemoMapNode
    {
        public int Layer;
        public DemoNodeType Type;
        public string Name;
        public bool Completed;

        public DemoMapNode(int layer, DemoNodeType type, string name)
        {
            Layer = layer;
            Type = type;
            Name = name;
        }

        public DemoMapNode Clone()
        {
            return new DemoMapNode(Layer, Type, Name);
        }
    }

    public sealed class DemoMapRoutePlan
    {
        public string Name { get; }
        public string Description { get; }
        public List<DemoMapNode> Nodes { get; } = new List<DemoMapNode>();

        public DemoMapRoutePlan(string name, string description, params DemoMapNode[] nodes)
        {
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

        public DemoMapNode CurrentNode => Nodes[CurrentIndex];
        public bool IsComplete => CurrentNode.Type == DemoNodeType.Victory;

        public DemoMapRun()
        {
            Nodes.Add(new DemoMapNode(0, DemoNodeType.Start, "选择剑道"));
            Nodes.Add(new DemoMapNode(1, DemoNodeType.RouteChoice, "第一层路口"));
        }

        public void CompleteCurrentNode()
        {
            if (CurrentNode.Type == DemoNodeType.RouteChoice)
            {
                return;
            }

            CurrentNode.Completed = true;

            if (CurrentIndex < Nodes.Count - 1)
            {
                CurrentIndex++;
            }
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
    }
}
