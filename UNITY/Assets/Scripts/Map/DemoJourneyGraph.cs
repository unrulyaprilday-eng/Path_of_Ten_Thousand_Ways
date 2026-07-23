using System;
using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Map
{
    // New journey node values are deliberately separate from the legacy DemoNodeType enum.
    public enum DemoJourneyNodeType
    {
        Start,
        Battle,
        Elite,
        Event,
        Cultivation,
        Secret,
        Refinement,
        Story,
        MiniBoss,
        Breakthrough,
        Boss
    }

    public sealed class DemoJourneyNode
    {
        public string NodeId { get; }
        public int ActIndex { get; }
        public int DepthIndex { get; }
        public int LaneIndex { get; }
        public DemoJourneyNodeType Type { get; }
        public string ContentId { get; }
        public string Name { get; }
        public string RequiredStoryFlagId { get; }
        public bool IsHidden { get; }

        public bool IsCombat
        {
            get
            {
                return Type == DemoJourneyNodeType.Battle
                    || Type == DemoJourneyNodeType.Elite
                    || Type == DemoJourneyNodeType.MiniBoss
                    || Type == DemoJourneyNodeType.Boss;
            }
        }

        public bool IsPreparation
        {
            get
            {
                return Type == DemoJourneyNodeType.Cultivation
                    || Type == DemoJourneyNodeType.Refinement
                    || Type == DemoJourneyNodeType.Secret
                    || Type == DemoJourneyNodeType.Story;
            }
        }

        public DemoJourneyNode(
            string nodeId,
            int actIndex,
            int depthIndex,
            int laneIndex,
            DemoJourneyNodeType type,
            string contentId,
            string name,
            string requiredStoryFlagId = null,
            bool isHidden = false)
        {
            NodeId = nodeId ?? string.Empty;
            ActIndex = actIndex;
            DepthIndex = depthIndex;
            LaneIndex = laneIndex;
            Type = type;
            ContentId = contentId ?? string.Empty;
            Name = name ?? string.Empty;
            RequiredStoryFlagId = requiredStoryFlagId ?? string.Empty;
            IsHidden = isHidden;
        }
    }

    public sealed class DemoJourneyEdge
    {
        public string FromNodeId { get; }
        public string ToNodeId { get; }

        public DemoJourneyEdge(string fromNodeId, string toNodeId)
        {
            FromNodeId = fromNodeId ?? string.Empty;
            ToNodeId = toNodeId ?? string.Empty;
        }
    }

    public sealed class DemoJourneyPathLengthRange
    {
        public int Minimum { get; }
        public int Maximum { get; }

        public DemoJourneyPathLengthRange(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }
    }

    public sealed class DemoJourneyActTemplate
    {
        private readonly List<DemoJourneyNodeType[]> nodeTypesByDepth;

        public int ActIndex { get; }
        public IReadOnlyList<DemoJourneyNodeType[]> NodeTypesByDepth => nodeTypesByDepth;

        public DemoJourneyActTemplate(int actIndex, params DemoJourneyNodeType[][] nodeTypes)
        {
            if (actIndex < 1 || actIndex > 3)
            {
                throw new ArgumentOutOfRangeException(nameof(actIndex));
            }

            ActIndex = actIndex;
            nodeTypesByDepth = new List<DemoJourneyNodeType[]>();
            if (nodeTypes == null)
            {
                return;
            }

            for (int i = 0; i < nodeTypes.Length; i++)
            {
                DemoJourneyNodeType[] choices = nodeTypes[i] ?? new DemoJourneyNodeType[0];
                nodeTypesByDepth.Add((DemoJourneyNodeType[])choices.Clone());
            }
        }
    }

    public sealed class DemoJourneyMapTemplate
    {
        private readonly List<DemoJourneyActTemplate> acts;

        public IReadOnlyList<DemoJourneyActTemplate> Acts => acts;

        public DemoJourneyMapTemplate(IEnumerable<DemoJourneyActTemplate> actTemplates)
        {
            acts = new List<DemoJourneyActTemplate>();
            if (actTemplates != null)
            {
                foreach (DemoJourneyActTemplate template in actTemplates)
                {
                    if (template != null)
                    {
                        acts.Add(template);
                    }
                }
            }

            acts.Sort((left, right) => left.ActIndex.CompareTo(right.ActIndex));
        }

        public static DemoJourneyMapTemplate Default()
        {
            // Eight nodes per standard route: entry, six variable-depth nodes, and a gate.
            return new DemoJourneyMapTemplate(new[]
            {
                new DemoJourneyActTemplate(
                    1,
                    new[] { DemoJourneyNodeType.Start },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Event },
                    new[] { DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Cultivation, DemoJourneyNodeType.Event, DemoJourneyNodeType.Secret },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Elite },
                    new[] { DemoJourneyNodeType.Event, DemoJourneyNodeType.Secret, DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.Refinement, DemoJourneyNodeType.Cultivation },
                    new[] { DemoJourneyNodeType.MiniBoss }),
                new DemoJourneyActTemplate(
                    2,
                    new[] { DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Elite },
                    new[] { DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Elite, DemoJourneyNodeType.Event, DemoJourneyNodeType.Secret },
                    new[] { DemoJourneyNodeType.Story, DemoJourneyNodeType.Cultivation },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Event },
                    new[] { DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.MiniBoss }),
                new DemoJourneyActTemplate(
                    3,
                    new[] { DemoJourneyNodeType.Breakthrough },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Elite },
                    new[] { DemoJourneyNodeType.Event, DemoJourneyNodeType.Secret },
                    new[] { DemoJourneyNodeType.Battle, DemoJourneyNodeType.Elite },
                    new[] { DemoJourneyNodeType.Secret, DemoJourneyNodeType.Cultivation, DemoJourneyNodeType.Event },
                    new[] { DemoJourneyNodeType.Refinement, DemoJourneyNodeType.Cultivation },
                    new[] { DemoJourneyNodeType.Story },
                    new[] { DemoJourneyNodeType.Boss })
            });
        }
    }

    public sealed class DemoJourneyGraph
    {
        private readonly List<DemoJourneyNode> nodes;
        private readonly List<DemoJourneyEdge> edges;
        private readonly Dictionary<string, DemoJourneyNode> nodesById;
        private readonly Dictionary<string, List<string>> outgoingById;
        private readonly HashSet<string> reachableNodeIds;
        private readonly Dictionary<int, DemoJourneyPathLengthRange> pathRanges;

        public int Seed { get; }
        public string StartNodeId { get; }
        public IReadOnlyList<DemoJourneyNode> Nodes => nodes;
        public IReadOnlyList<DemoJourneyEdge> Edges => edges;
        public IReadOnlyCollection<string> ReachableNodeIds => reachableNodeIds;

        internal DemoJourneyGraph(
            int seed,
            string startNodeId,
            List<DemoJourneyNode> graphNodes,
            List<DemoJourneyEdge> graphEdges)
        {
            Seed = seed;
            StartNodeId = startNodeId ?? string.Empty;
            nodes = graphNodes ?? new List<DemoJourneyNode>();
            edges = graphEdges ?? new List<DemoJourneyEdge>();
            nodesById = new Dictionary<string, DemoJourneyNode>(StringComparer.Ordinal);
            outgoingById = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            for (int i = 0; i < nodes.Count; i++)
            {
                DemoJourneyNode node = nodes[i];
                if (node != null && !nodesById.ContainsKey(node.NodeId))
                {
                    nodesById.Add(node.NodeId, node);
                    outgoingById[node.NodeId] = new List<string>();
                }
            }

            for (int i = 0; i < edges.Count; i++)
            {
                DemoJourneyEdge edge = edges[i];
                if (edge != null && outgoingById.ContainsKey(edge.FromNodeId))
                {
                    outgoingById[edge.FromNodeId].Add(edge.ToNodeId);
                }
            }

            reachableNodeIds = ComputeReachable(StartNodeId);
            pathRanges = ComputePathRanges();
        }

        public bool TryGetNode(string nodeId, out DemoJourneyNode node)
        {
            return nodesById.TryGetValue(nodeId ?? string.Empty, out node);
        }

        public IReadOnlyList<string> GetOutgoingNodeIds(string nodeId)
        {
            List<string> outgoing;
            if (!outgoingById.TryGetValue(nodeId ?? string.Empty, out outgoing))
            {
                return new string[0];
            }

            return outgoing.AsReadOnly();
        }

        public IReadOnlyList<string> GetReachableNodeIds()
        {
            List<string> result = new List<string>(reachableNodeIds);
            result.Sort(StringComparer.Ordinal);
            return result;
        }

        // A completed branch closes its siblings. Only the deepest completed
        // position may expose a new frontier; earlier forks never reopen.
        public IReadOnlyList<string> GetReachableNodeIds(IEnumerable<string> completedNodeIds)
        {
            return GetReachableNodeIds(completedNodeIds, Array.Empty<string>());
        }

        public IReadOnlyList<string> GetReachableNodeIds(
            IEnumerable<string> completedNodeIds,
            IEnumerable<string> experienceFlagIds)
        {
            HashSet<string> completed = new HashSet<string>(StringComparer.Ordinal);
            if (completedNodeIds != null)
            {
                foreach (string nodeId in completedNodeIds)
                {
                    if (!string.IsNullOrEmpty(nodeId))
                    {
                        completed.Add(nodeId);
                    }
                }
            }

            HashSet<string> frontier = new HashSet<string>(StringComparer.Ordinal);
            HashSet<string> experienceFlags = new HashSet<string>(
                experienceFlagIds ?? Array.Empty<string>(),
                StringComparer.Ordinal);
            if (completed.Count == 0)
            {
                if (CanReveal(StartNodeId, experienceFlags)) frontier.Add(StartNodeId);
            }
            else
            {
                int deepestProgress = int.MinValue;
                foreach (string completedId in completed)
                {
                    DemoJourneyNode completedNode;
                    if (TryGetNode(completedId, out completedNode))
                    {
                        deepestProgress = Math.Max(
                            deepestProgress,
                            completedNode.ActIndex * 100 + completedNode.DepthIndex);
                    }
                }

                foreach (string completedId in completed)
                {
                    DemoJourneyNode completedNode;
                    if (!TryGetNode(completedId, out completedNode)
                        || completedNode.ActIndex * 100 + completedNode.DepthIndex != deepestProgress)
                    {
                        continue;
                    }

                    List<string> outgoing;
                    if (!outgoingById.TryGetValue(completedId, out outgoing)) continue;
                    for (int i = 0; i < outgoing.Count; i++)
                    {
                        string candidate = outgoing[i];
                        if (!completed.Contains(candidate)
                            && reachableNodeIds.Contains(candidate)
                            && CanReveal(candidate, experienceFlags))
                        {
                            frontier.Add(candidate);
                        }
                    }
                }
            }

            List<string> result = new List<string>(frontier);
            result.Sort(StringComparer.Ordinal);
            return result;
        }

        private bool CanReveal(string nodeId, ISet<string> experienceFlags)
        {
            return TryGetNode(nodeId, out DemoJourneyNode node)
                && (!node.IsHidden
                    || (!string.IsNullOrWhiteSpace(node.RequiredStoryFlagId)
                        && experienceFlags.Contains(node.RequiredStoryFlagId)));
        }

        public IReadOnlyList<DemoJourneyNode> GetActNodes(int actIndex)
        {
            List<DemoJourneyNode> result = new List<DemoJourneyNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ActIndex == actIndex)
                {
                    result.Add(nodes[i]);
                }
            }

            return result.AsReadOnly();
        }

        public DemoJourneyPathLengthRange GetActPathLengthRange(int actIndex)
        {
            DemoJourneyPathLengthRange range;
            return pathRanges.TryGetValue(actIndex, out range)
                ? range
                : new DemoJourneyPathLengthRange(0, 0);
        }

        public bool Validate(out IReadOnlyList<string> errors)
        {
            List<string> issues = new List<string>();
            if (nodes.Count == 0 || string.IsNullOrEmpty(StartNodeId))
            {
                issues.Add("Journey graph has no start node.");
            }

            int startCount = 0;
            int bossCount = 0;
            int miniBossCount = 0;
            int breakthroughCount = 0;
            for (int i = 0; i < nodes.Count; i++)
            {
                DemoJourneyNode node = nodes[i];
                if (node.Type == DemoJourneyNodeType.Start) startCount++;
                if (node.Type == DemoJourneyNodeType.Boss) bossCount++;
                if (node.Type == DemoJourneyNodeType.MiniBoss) miniBossCount++;
                if (node.Type == DemoJourneyNodeType.Breakthrough) breakthroughCount++;
                if (!reachableNodeIds.Contains(node.NodeId)) issues.Add("Unreachable node: " + node.NodeId);
                if (node.Type != DemoJourneyNodeType.Boss && GetOutgoingNodeIds(node.NodeId).Count == 0)
                {
                    issues.Add("Dead-end node: " + node.NodeId);
                }
            }

            if (startCount != 1) issues.Add("Graph must contain exactly one Start node.");
            if (bossCount != 1) issues.Add("Graph must contain exactly one Boss node.");
            if (miniBossCount != 2) issues.Add("Graph must contain one MiniBoss in acts one and two.");
            if (breakthroughCount != 1
                || !nodes.Exists(node => node.ActIndex == 3
                    && node.DepthIndex == 0
                    && node.Type == DemoJourneyNodeType.Breakthrough))
            {
                issues.Add("Graph must contain one mandatory Breakthrough at the start of act three.");
            }

            for (int act = 1; act <= 3; act++)
            {
                DemoJourneyPathLengthRange range = GetActPathLengthRange(act);
                if (range.Minimum < 6 || range.Maximum > 10 || range.Minimum == 0)
                {
                    issues.Add("Act " + act + " path length is outside 6-10.");
                }

                List<DemoJourneyNode> actNodes = new List<DemoJourneyNode>(GetActNodes(act));
                for (int i = 0; i < actNodes.Count; i++)
                {
                    DemoJourneyNode node = actNodes[i];
                    if (node.Type == DemoJourneyNodeType.MiniBoss || node.Type == DemoJourneyNodeType.Boss)
                    {
                        IReadOnlyList<string> incoming = GetIncomingNodeIds(node.NodeId);
                        if (incoming.Count == 0)
                        {
                            issues.Add("Boss gate has no predecessor: " + node.NodeId);
                        }
                        else
                        {
                            for (int j = 0; j < incoming.Count; j++)
                            {
                                DemoJourneyNode predecessor;
                                if (!TryGetNode(incoming[j], out predecessor) || !predecessor.IsPreparation)
                                {
                                    issues.Add("Boss gate must follow preparation: " + node.NodeId);
                                    break;
                                }
                            }
                        }
                    }
                }

                ValidateCombatRuns(act, issues);
            }

            errors = issues.AsReadOnly();
            return issues.Count == 0;
        }

        public IReadOnlyList<string> GetIncomingNodeIds(string nodeId)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < edges.Count; i++)
            {
                if (string.Equals(edges[i].ToNodeId, nodeId ?? string.Empty, StringComparison.Ordinal))
                {
                    result.Add(edges[i].FromNodeId);
                }
            }

            return result.AsReadOnly();
        }

        private HashSet<string> ComputeReachable(string startNodeId)
        {
            HashSet<string> visited = new HashSet<string>(StringComparer.Ordinal);
            Queue<string> pending = new Queue<string>();
            if (!string.IsNullOrEmpty(startNodeId) && nodesById.ContainsKey(startNodeId))
            {
                pending.Enqueue(startNodeId);
            }

            while (pending.Count > 0)
            {
                string nodeId = pending.Dequeue();
                if (!visited.Add(nodeId)) continue;
                List<string> outgoing;
                if (!outgoingById.TryGetValue(nodeId, out outgoing)) continue;
                for (int i = 0; i < outgoing.Count; i++)
                {
                    if (!visited.Contains(outgoing[i])) pending.Enqueue(outgoing[i]);
                }
            }

            return visited;
        }

        private Dictionary<int, DemoJourneyPathLengthRange> ComputePathRanges()
        {
            Dictionary<int, DemoJourneyPathLengthRange> ranges = new Dictionary<int, DemoJourneyPathLengthRange>();
            for (int act = 1; act <= 3; act++)
            {
                int min = int.MaxValue;
                int max = 0;
                List<DemoJourneyNode> starts = new List<DemoJourneyNode>();
                List<DemoJourneyNode> terminals = new List<DemoJourneyNode>();
                for (int i = 0; i < nodes.Count; i++)
                {
                    DemoJourneyNode node = nodes[i];
                    if (node.ActIndex != act) continue;
                    if (node.DepthIndex == 0) starts.Add(node);
                    if (node.Type == DemoJourneyNodeType.MiniBoss || node.Type == DemoJourneyNodeType.Boss) terminals.Add(node);
                }

                for (int i = 0; i < starts.Count; i++)
                {
                    Queue<PathState> pending = new Queue<PathState>();
                    pending.Enqueue(new PathState(starts[i].NodeId, 1));
                    while (pending.Count > 0)
                    {
                        PathState state = pending.Dequeue();
                        DemoJourneyNode node;
                        if (!TryGetNode(state.NodeId, out node)) continue;
                        bool terminal = node.Type == DemoJourneyNodeType.MiniBoss || node.Type == DemoJourneyNodeType.Boss;
                        if (terminal)
                        {
                            min = Math.Min(min, state.Length);
                            max = Math.Max(max, state.Length);
                            continue;
                        }

                        List<string> outgoing;
                        if (!outgoingById.TryGetValue(state.NodeId, out outgoing)) continue;
                        for (int j = 0; j < outgoing.Count; j++)
                        {
                            DemoJourneyNode next;
                            if (TryGetNode(outgoing[j], out next) && next.ActIndex == act)
                            {
                                pending.Enqueue(new PathState(next.NodeId, state.Length + 1));
                            }
                        }
                    }
                }

                ranges[act] = new DemoJourneyPathLengthRange(min == int.MaxValue ? 0 : min, max);
            }

            return ranges;
        }

        private void ValidateCombatRuns(int act, List<string> issues)
        {
            List<DemoJourneyNode> starts = new List<DemoJourneyNode>();
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].ActIndex == act && nodes[i].DepthIndex == 0) starts.Add(nodes[i]);
            }

            for (int i = 0; i < starts.Count; i++)
            {
                Queue<CombatState> pending = new Queue<CombatState>();
                pending.Enqueue(new CombatState(starts[i].NodeId, 0));
                while (pending.Count > 0)
                {
                    CombatState state = pending.Dequeue();
                    DemoJourneyNode node;
                    if (!TryGetNode(state.NodeId, out node)) continue;
                    int run = node.IsCombat ? state.CombatRun + 1 : 0;
                    if (run >= 3)
                    {
                        issues.Add("Three consecutive combat nodes in act " + act + ".");
                        return;
                    }

                    List<string> outgoing;
                    if (!outgoingById.TryGetValue(node.NodeId, out outgoing)) continue;
                    for (int j = 0; j < outgoing.Count; j++)
                    {
                        DemoJourneyNode next;
                        if (TryGetNode(outgoing[j], out next) && next.ActIndex == act)
                        {
                            pending.Enqueue(new CombatState(next.NodeId, run));
                        }
                    }
                }
            }
        }

        private struct PathState
        {
            public string NodeId;
            public int Length;
            public PathState(string nodeId, int length) { NodeId = nodeId; Length = length; }
        }

        private struct CombatState
        {
            public string NodeId;
            public int CombatRun;
            public CombatState(string nodeId, int combatRun) { NodeId = nodeId; CombatRun = combatRun; }
        }
    }

    public sealed class DemoJourneyGraphGenerator
    {
        private readonly DemoJourneyMapTemplate template;

        public DemoJourneyGraphGenerator(DemoJourneyMapTemplate mapTemplate = null)
        {
            template = mapTemplate ?? DemoJourneyMapTemplate.Default();
        }

        public static DemoJourneyGraph Generate(int seed)
        {
            return new DemoJourneyGraphGenerator().Build(seed);
        }

        public static DemoJourneyGraph Generate(int seed, DemoJourneyMapTemplate mapTemplate)
        {
            return new DemoJourneyGraphGenerator(mapTemplate).Build(seed);
        }

        public DemoJourneyGraph Build(int seed)
        {
            if (template.Acts.Count != 3)
            {
                throw new InvalidOperationException("Journey template must contain exactly three acts.");
            }

            DeterministicRandom random = new DeterministicRandom(seed);
            List<DemoJourneyNode> nodes = new List<DemoJourneyNode>();
            List<DemoJourneyEdge> edges = new List<DemoJourneyEdge>();
            List<List<List<DemoJourneyNode>>> layers = new List<List<List<DemoJourneyNode>>>();
            for (int actIndex = 1; actIndex <= 3; actIndex++)
            {
                DemoJourneyActTemplate act = template.Acts[actIndex - 1];
                if (act.ActIndex != actIndex || act.NodeTypesByDepth.Count != 8)
                {
                    throw new InvalidOperationException("Each journey act must define eight depths in order.");
                }

                List<List<DemoJourneyNode>> actLayers = new List<List<DemoJourneyNode>>();
                for (int depth = 0; depth < act.NodeTypesByDepth.Count; depth++)
                {
                    DemoJourneyNodeType[] choices = act.NodeTypesByDepth[depth];
                    if (choices == null || choices.Length == 0) throw new InvalidOperationException("Journey depth has no node choices.");
                    int laneCount = depth == 0 || depth == 7 || IsCriticalStoryDepth(actIndex, depth)
                        ? 1
                        : 2 + random.Next(2);
                    List<DemoJourneyNode> layer = new List<DemoJourneyNode>();
                    for (int lane = 0; lane < laneCount; lane++)
                    {
                        DemoJourneyNodeType selectedType = SelectNodeTypeForLane(
                            choices,
                            random,
                            actIndex,
                            depth,
                            lane);
                        string nodeId = "journey_s" + seed + "_a" + actIndex + "_d" + depth + "_l" + lane;
                        ContentIdentity identity = ResolveContentIdentity(actIndex, depth, lane, selectedType);
                        string contentId = identity.Id;
                        string name = identity.Name;
                        DemoJourneyNode node = new DemoJourneyNode(nodeId, actIndex, depth, lane, selectedType, contentId, name);
                        nodes.Add(node);
                        layer.Add(node);
                    }
                    actLayers.Add(layer);
                }
                if (actIndex == 3)
                {
                    List<DemoJourneyNode> hiddenLayer = actLayers[5];
                    DemoJourneyNode hiddenNode = new DemoJourneyNode(
                        "journey_s" + seed + "_a3_d5_hidden_contract",
                        3,
                        5,
                        hiddenLayer.Count,
                        DemoJourneyNodeType.Secret,
                        "hidden_contract_cache",
                        "契炉暗仓",
                        "experience_miner_spirit_bound",
                        true);
                    nodes.Add(hiddenNode);
                    hiddenLayer.Add(hiddenNode);
                }
                layers.Add(actLayers);
            }

            for (int actIndex = 0; actIndex < layers.Count; actIndex++)
            {
                List<List<DemoJourneyNode>> actLayers = layers[actIndex];
                for (int depth = 0; depth < actLayers.Count - 1; depth++)
                {
                    AddCompleteLayerEdges(actLayers[depth], actLayers[depth + 1], edges);
                }

                if (actIndex < layers.Count - 1)
                {
                    AddCompleteLayerEdges(actLayers[actLayers.Count - 1], layers[actIndex + 1][0], edges);
                }
            }

            return new DemoJourneyGraph(seed, layers[0][0][0].NodeId, nodes, edges);
        }

        private static DemoJourneyNodeType SelectNodeTypeForLane(
            DemoJourneyNodeType[] choices,
            DeterministicRandom random,
            int actIndex,
            int depth,
            int lane)
        {
            if (choices.Length == 1)
            {
                return choices[0];
            }

            bool fixedStory = IsCriticalStoryDepth(actIndex, depth);
            if (fixedStory)
            {
                for (int i = 0; i < choices.Length; i++)
                {
                    if (choices[i] == DemoJourneyNodeType.Story)
                    {
                        return DemoJourneyNodeType.Story;
                    }
                }
            }

            int offset = random.Next(choices.Length);
            return choices[(offset + lane) % choices.Length];
        }

        private static ContentIdentity ResolveContentIdentity(
            int actIndex,
            int depth,
            int lane,
            DemoJourneyNodeType type)
        {
            if (type == DemoJourneyNodeType.Start)
                return new ContentIdentity("story_old_mine_entry", "残照矿门");
            if (type == DemoJourneyNodeType.Breakthrough)
                return new ContentIdentity("breakthrough_qi_to_foundation", "旧矿灵眼");
            if (type == DemoJourneyNodeType.Boss)
                return new ContentIdentity("encounter_act3_boss", "玄铁镇矿剑傀");
            if (type == DemoJourneyNodeType.MiniBoss)
                return actIndex == 1
                    ? new ContentIdentity("encounter_act1_miniboss", "铁脊矿兽巢")
                    : new ContentIdentity("encounter_act2_miniboss", "吞剑矿魈壁窟");
            if (actIndex == 1 && depth == 2 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("event_miner_spirit_first", "契索下的微光");
            if (actIndex == 1 && depth == 5 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("event_old_contract_trace", "旁支矿印");
            if (actIndex == 2 && depth == 0 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("story_collapsed_well_entry", "塌井下层");
            if (actIndex == 2 && depth == 2 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("event_miner_spirit_return", "塌井再会");
            if (actIndex == 2 && depth == 6 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("event_old_contract_truth", "祭炼真相");
            if (actIndex == 3 && depth == 6 && type == DemoJourneyNodeType.Story)
                return new ContentIdentity("event_sword_furnace_contract", "剑炉断契");

            string[] battleNames = actIndex == 1
                ? new[] { "碎轨伏影", "契屑巢穴", "浅层矿卒" }
                : actIndex == 2
                    ? new[] { "悬桥残魂", "吞剑伏巢", "塌井守卫" }
                    : new[] { "封契剑影", "炉心构造阵", "玄铁巡傀" };
            string[] eventNames = actIndex == 1
                ? new[] { "矿工遗骨", "断壁剑痕", "废炉余温" }
                : actIndex == 2
                    ? new[] { "井底雷髓", "矿灵暗径", "旧账石室" }
                    : new[] { "炉壁铭文", "残契回声", "剑胚静台" };
            bool combat = type == DemoJourneyNodeType.Battle || type == DemoJourneyNodeType.Elite;
            string name = (combat ? battleNames : eventNames)[(depth + lane) % 3];
            string id = "old_mine_a" + actIndex + "_d" + depth + "_l" + lane + "_" + type.ToString().ToLowerInvariant();
            return new ContentIdentity(id, name);
        }

        private struct ContentIdentity
        {
            public string Id;
            public string Name;
            public ContentIdentity(string id, string name) { Id = id; Name = name; }
        }

        private static bool IsCriticalStoryDepth(int actIndex, int depth)
        {
            return (actIndex == 1 && depth == 2)
                || (actIndex == 2 && (depth == 0 || depth == 2 || depth == 6))
                || (actIndex == 3 && (depth == 0 || depth == 6));
        }

        private static void AddCompleteLayerEdges(
            List<DemoJourneyNode> from,
            List<DemoJourneyNode> to,
            List<DemoJourneyEdge> edges)
        {
            for (int i = 0; i < from.Count; i++)
            {
                for (int j = 0; j < to.Count; j++)
                {
                    edges.Add(new DemoJourneyEdge(from[i].NodeId, to[j].NodeId));
                }
            }
        }

        private struct DeterministicRandom
        {
            private uint state;

            public DeterministicRandom(int seed)
            {
                state = unchecked((uint)seed);
                if (state == 0) state = 0x6D2B79F5u;
            }

            public int Next(int exclusiveMaximum)
            {
                if (exclusiveMaximum <= 1) return 0;
                state ^= state << 13;
                state ^= state >> 17;
                state ^= state << 5;
                return (int)(state % (uint)exclusiveMaximum);
            }
        }
    }
}
