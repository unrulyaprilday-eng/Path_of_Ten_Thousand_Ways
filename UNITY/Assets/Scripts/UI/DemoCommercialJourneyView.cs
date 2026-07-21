using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public sealed class DemoCommercialJourneyView : MonoBehaviour
    {
        private const string SceneResourcePath = "Art/Scenes/scene_battle_old_mine_entry_001";
        private const string BossResourcePath = "Art/Boss/boss_tianjie_halfbody_001";
        private const string NormalEnemyResourcePath = "Art/Characters/char_enemy_old_mine_wraith_battle_003";

        private static readonly Color Ink = new Color(0.15f, 0.13f, 0.10f, 1f);
        private static readonly Color SoftInk = new Color(0.31f, 0.29f, 0.24f, 1f);
        private static readonly Color FaintInk = new Color(0.48f, 0.45f, 0.37f, 1f);
        private static readonly Color Paper = new Color(0.965f, 0.945f, 0.875f, 0.97f);
        private static readonly Color PaperWash = new Color(0.94f, 0.92f, 0.85f, 0.89f);
        private static readonly Color Gold = new Color(0.64f, 0.48f, 0.22f, 1f);
        private static readonly Color Cinnabar = new Color(0.65f, 0.23f, 0.18f, 1f);
        private static readonly Color Jade = new Color(0.24f, 0.49f, 0.43f, 1f);
        private static readonly Color Azure = new Color(0.30f, 0.46f, 0.57f, 1f);

        private readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        private DemoGameController controller;
        private Font bodyFont;
        private Font titleFont;
        private Sprite whiteSprite;
        private RectTransform contentRoot;
        private string renderedSignature = string.Empty;
        private long selectionSequence = -1;
        private int selectedIndex = -1;

        public void Initialize(DemoGameController demoController, Font font)
        {
            controller = demoController;
            bodyFont = ResolveFont(new[] { "Source Han Sans SC", "Microsoft YaHei", "SimSun" }, font, 24);
            titleFont = ResolveFont(new[] { "Source Han Serif SC", "SimSun", "Microsoft YaHei" }, bodyFont, 32);

            RectTransform root = GetComponent<RectTransform>();
            Stretch(root, Vector2.zero, Vector2.zero);
            ConfigureCanvasScaler();
            ClearChildren(root);

            whiteSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            contentRoot = CreatePanel(root, "CommercialJourneySurface", Vector2.zero, Vector2.one, Color.clear);
            renderedSignature = string.Empty;
            selectionSequence = -1;
            selectedIndex = -1;
            RefreshNow();
        }

        public void RefreshNow()
        {
            if (controller == null || contentRoot == null)
            {
                return;
            }

            DemoFlowSnapshot snapshot = controller.FlowSnapshot;
            if (snapshot.Sequence != selectionSequence)
            {
                selectionSequence = snapshot.Sequence;
                selectedIndex = -1;
            }

            string signature = BuildSignature(snapshot);
            if (string.Equals(renderedSignature, signature, StringComparison.Ordinal))
            {
                return;
            }

            renderedSignature = signature;
            bool visible = IsJourneyPhase(snapshot.Phase);
            contentRoot.gameObject.SetActive(visible);
            if (!visible)
            {
                return;
            }

            ClearChildren(contentRoot);
            BuildPageChrome(snapshot.Phase);
            switch (snapshot.Phase)
            {
                case DemoFlowPhase.JourneyMap:
                    BuildJourneyMap();
                    break;
                case DemoFlowPhase.NodeScene:
                case DemoFlowPhase.Breakthrough:
                    BuildJourneyNodeScene(snapshot.Phase == DemoFlowPhase.Breakthrough);
                    break;
                case DemoFlowPhase.RewardChoice:
                case DemoFlowPhase.Training:
                case DemoFlowPhase.Preparation:
                    BuildRewardChoice(snapshot.Phase);
                    break;
                case DemoFlowPhase.RouteChoice:
                    BuildRouteChoice();
                    break;
                case DemoFlowPhase.EncounterIntro:
                case DemoFlowPhase.BossGate:
                    BuildEncounter(snapshot.Phase == DemoFlowPhase.BossGate);
                    break;
                case DemoFlowPhase.RunResult:
                    BuildRunResult();
                    break;
            }
        }

        private void Update()
        {
            RefreshNow();
        }

        private void OnDestroy()
        {
            if (whiteSprite != null)
            {
                DestroyCreatedObject(whiteSprite);
            }
        }

        private void BuildPageChrome(DemoFlowPhase phase)
        {
            RectTransform background = CreatePanel(contentRoot, "Scene", Vector2.zero, Vector2.one, Color.white);
            ApplySprite(background.GetComponent<Image>(), LoadSprite(SceneResourcePath), Color.white, false);
            CreatePanel(contentRoot, "PaperWash", Vector2.zero, Vector2.one, PaperWash);
            CreatePanel(contentRoot, "TopWash", new Vector2(0f, 0.79f), Vector2.one, new Color(0.98f, 0.96f, 0.90f, 0.94f));
            RectTransform topRule = CreatePanel(contentRoot, "TopRule", new Vector2(0f, 0.79f), new Vector2(1f, 0.79f), new Color(Gold.r, Gold.g, Gold.b, 0.42f));
            topRule.offsetMin = new Vector2(72f, -1f);
            topRule.offsetMax = new Vector2(-72f, 1f);

            string title = GetPageTitle(phase);
            string subtitle = GetPageSubtitle(phase);
            Color accent = GetPageAccent(phase);

            Text eyebrow = CreateText(contentRoot, "Eyebrow", bodyFont, 14, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            eyebrow.text = BuildChapterLabel(phase);
            SetFixed(eyebrow.rectTransform, Vector2.up, Vector2.up, new Vector2(78f, -34f), new Vector2(640f, 24f));

            Text titleText = CreateText(contentRoot, "PageTitle", titleFont, 38, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            titleText.text = title;
            titleText.resizeTextForBestFit = true;
            titleText.resizeTextMinSize = 25;
            titleText.resizeTextMaxSize = 38;
            SetFixed(titleText.rectTransform, Vector2.up, Vector2.up, new Vector2(76f, -61f), new Vector2(650f, 58f));

            Text subtitleText = CreateText(contentRoot, "PageSubtitle", bodyFont, 16, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            subtitleText.text = subtitle;
            SetFixed(subtitleText.rectTransform, Vector2.up, Vector2.up, new Vector2(80f, -119f), new Vector2(760f, 38f));

            BuildRunSummaryStrip();
            BuildProgressRail(phase, accent);
        }

        private void BuildRunSummaryStrip()
        {
            DemoRunState run = controller.Run;
            string gongfa = run.MainGongfa == DemoGongfaType.None
                ? "未定"
                : DemoGongfaLibrary.Get(run.MainGongfa).Name;
            string artifact = run.Artifacts.Count == 0
                ? "未得"
                : DemoArtifactLibrary.Get(run.Artifacts[0]).Name;
            string summary = $"气血  {run.CurrentHealth}/{run.MaxHealth}     牌组  {run.Deck.Count}     主修  {gongfa}     法器  {artifact}";

            Text text = CreateText(contentRoot, "RunSummary", bodyFont, 16, FontStyle.Bold, TextAnchor.UpperRight, Ink);
            text.text = summary;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 13;
            text.resizeTextMaxSize = 16;
            SetFixed(text.rectTransform, Vector2.one, Vector2.one, new Vector2(-76f, -52f), new Vector2(930f, 34f));

            DemoMapNode node = run.Map.CurrentNode;
            string place = node == null ? "道途未定" : $"第 {Mathf.Max(1, node.Layer)} 层 · {node.Name}";
            Text placeText = CreateText(contentRoot, "CurrentPlace", bodyFont, 13, FontStyle.Normal, TextAnchor.UpperRight, FaintInk);
            placeText.text = place;
            SetFixed(placeText.rectTransform, Vector2.one, Vector2.one, new Vector2(-76f, -91f), new Vector2(700f, 28f));
        }

        private void BuildProgressRail(DemoFlowPhase phase, Color accent)
        {
            bool configuredJourney = controller.HasJourneySession;
            string[] labels = configuredJourney
                ? new[] { "矿门", "浅层", "铁脊", "塌井", "筑基", "剑炉", "剑傀" }
                : new[] { "首战", "战后", "一层", "二层", "三层", "天劫", "结算" };
            int active = GetProgressIndex(phase);
            float left = 96f;
            float width = 1728f;
            float step = width / (labels.Length - 1);
            RectTransform line = CreateFixedPanel(
                contentRoot, "ProgressLine", Vector2.up, Vector2.up,
                new Vector2(left, -176f), new Vector2(width, 2f), new Color(Gold.r, Gold.g, Gold.b, 0.30f));
            line.pivot = Vector2.up;

            for (int i = 0; i < labels.Length; i++)
            {
                bool reached = i <= active;
                float x = left + step * i;
                RectTransform mark = CreateFixedPanel(
                    contentRoot, "ProgressMark_" + i, Vector2.up, new Vector2(0.5f, 0.5f),
                    new Vector2(x, -175f), new Vector2(i == active ? 20f : 14f, i == active ? 20f : 14f),
                    reached ? accent : new Color(0.56f, 0.53f, 0.46f, 0.62f));
                Text label = CreateText(contentRoot, "ProgressLabel_" + i, bodyFont, 12, reached ? FontStyle.Bold : FontStyle.Normal, TextAnchor.UpperCenter, reached ? Ink : FaintInk);
                label.text = labels[i];
                SetFixed(label.rectTransform, Vector2.up, Vector2.up, new Vector2(x - 48f, -188f), new Vector2(96f, 24f));
            }
        }

        private void BuildJourneyMap()
        {
            DemoJourneyGraph graph = controller.JourneyGraph;
            DemoRunSaveV2 snapshot = controller.JourneySnapshot;
            if (graph == null || snapshot == null)
            {
                Text unavailable = CreateText(contentRoot, "JourneyUnavailable", bodyFont, 20, FontStyle.Normal, TextAnchor.MiddleCenter, SoftInk);
                unavailable.text = "旧矿图卷尚未展开。";
                Stretch(unavailable.rectTransform, new Vector2(80f, 230f), new Vector2(-80f, -120f));
                return;
            }

            int act = Mathf.Clamp(snapshot.ActIndex, 1, 3);
            HashSet<string> completed = new HashSet<string>(snapshot.CompletedNodeIds ?? new List<string>(), StringComparer.Ordinal);
            HashSet<string> frontier = new HashSet<string>(snapshot.ReachableNodeIds ?? new List<string>(), StringComparer.Ordinal);
            List<DemoJourneyNode> actNodes = graph.GetActNodes(act).ToList();
            Dictionary<string, Vector2> positions = new Dictionary<string, Vector2>(StringComparer.Ordinal);

            Text guidance = CreateText(contentRoot, "JourneyGuidance", bodyFont, 15, FontStyle.Bold, TextAnchor.MiddleCenter, Jade);
            guidance.text = "墨迹自下向上延伸。亮起之处可进入，未显之路只保留矿脉轮廓。";
            SetFixed(guidance.rectTransform, Vector2.up, Vector2.up, new Vector2(0f, -215f), new Vector2(1280f, 34f));

            RectTransform mapWash = CreatePanel(
                contentRoot,
                "JourneyMapWash",
                new Vector2(0.06f, 0.075f),
                new Vector2(0.94f, 0.765f),
                new Color(0.94f, 0.93f, 0.88f, 0.34f));
            CreateBorder(mapWash, new Color(Ink.r, Ink.g, Ink.b, 0.14f), 1f);

            for (int depth = 0; depth < 8; depth++)
            {
                List<DemoJourneyNode> layer = actNodes
                    .Where(node => node.DepthIndex == depth)
                    .OrderBy(node => node.LaneIndex)
                    .ToList();
                for (int index = 0; index < layer.Count; index++)
                {
                    float spacing = 270f;
                    float x = (index - (layer.Count - 1) * 0.5f) * spacing;
                    float y = -742f + depth * 72f;
                    positions[layer[index].NodeId] = new Vector2(x, y);
                }
            }

            foreach (DemoJourneyEdge edge in graph.Edges)
            {
                if (!positions.TryGetValue(edge.FromNodeId, out Vector2 from)
                    || !positions.TryGetValue(edge.ToNodeId, out Vector2 to))
                {
                    continue;
                }

                bool travelled = completed.Contains(edge.FromNodeId)
                    && (completed.Contains(edge.ToNodeId) || frontier.Contains(edge.ToNodeId));
                CreateJourneyMapEdge(mapWash, from, to, travelled ? Jade : new Color(Ink.r, Ink.g, Ink.b, 0.16f), travelled ? 4f : 2f);
            }

            foreach (DemoJourneyNode node in actNodes.OrderBy(item => item.DepthIndex).ThenBy(item => item.LaneIndex))
            {
                bool isCompleted = completed.Contains(node.NodeId);
                bool isReachable = frontier.Contains(node.NodeId) && !isCompleted;
                BuildJourneyMapNode(mapWash, node, positions[node.NodeId], isCompleted, isReachable);
            }

            Text actMark = CreateText(mapWash, "ActMark", titleFont, 56, FontStyle.Bold, TextAnchor.UpperLeft, new Color(Ink.r, Ink.g, Ink.b, 0.075f));
            actMark.text = GetJourneyActTitle(act);
            SetFixed(actMark.rectTransform, Vector2.up, Vector2.up, new Vector2(34f, -28f), new Vector2(520f, 72f));

            Text progress = CreateText(mapWash, "ActProgress", bodyFont, 14, FontStyle.Bold, TextAnchor.UpperRight, SoftInk);
            int completedInAct = actNodes.Count(node => completed.Contains(node.NodeId));
            progress.text = $"本幕已行 {completedInAct}/8    全程 {snapshot.CompletedNodeIds.Count}/24";
            SetFixed(progress.rectTransform, Vector2.one, Vector2.one, new Vector2(-34f, -28f), new Vector2(520f, 32f));

            if (!string.IsNullOrWhiteSpace(controller.JourneyError))
            {
                Text error = CreateText(contentRoot, "JourneyError", bodyFont, 13, FontStyle.Bold, TextAnchor.LowerCenter, Cinnabar);
                error.text = controller.JourneyError;
                SetFixed(error.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 30f), new Vector2(1200f, 26f));
            }
        }

        private void CreateJourneyMapEdge(RectTransform parent, Vector2 from, Vector2 to, Color color, float thickness)
        {
            Vector2 delta = to - from;
            RectTransform edge = CreateFixedPanel(
                parent,
                "PathInk",
                new Vector2(0.5f, 1f),
                new Vector2(0f, 0.5f),
                from,
                new Vector2(delta.magnitude, thickness),
                color);
            edge.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
        }

        private void BuildJourneyMapNode(
            RectTransform parent,
            DemoJourneyNode node,
            Vector2 position,
            bool completed,
            bool reachable)
        {
            Color accent = GetJourneyNodeAccent(node.Type);
            float size = node.Type == DemoJourneyNodeType.Boss ? 76f : node.Type == DemoJourneyNodeType.MiniBoss ? 66f : 54f;
            Color surface = completed
                ? new Color(Ink.r, Ink.g, Ink.b, 0.84f)
                : reachable
                    ? Color.Lerp(Paper, accent, 0.20f)
                    : new Color(0.76f, 0.75f, 0.70f, 0.48f);
            RectTransform mark = CreateFixedPanel(
                parent,
                "JourneyNode_" + node.NodeId,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 0.5f),
                position,
                new Vector2(size, size),
                surface);
            CreateBorder(mark, new Color(accent.r, accent.g, accent.b, reachable ? 0.95f : 0.42f), reachable ? 3f : 1f);

            if (reachable)
            {
                Image image = mark.GetComponent<Image>();
                image.raycastTarget = true;
                Button button = mark.gameObject.AddComponent<Button>();
                button.targetGraphic = image;
                ConfigureChoiceButton(button);
                string capturedNodeId = node.NodeId;
                button.onClick.AddListener(() => SelectJourneyMapNode(capturedNodeId));
            }

            Text glyph = CreateText(mark, "Glyph", titleFont, node.Type == DemoJourneyNodeType.Boss ? 28 : 22, FontStyle.Bold, TextAnchor.MiddleCenter, completed ? Paper : Ink);
            glyph.text = GetJourneyNodeGlyph(node.Type);
            Stretch(glyph.rectTransform, new Vector2(3f, 3f), new Vector2(-3f, -3f));

            Text label = CreateText(parent, "JourneyLabel_" + node.NodeId, bodyFont, 12, reachable ? FontStyle.Bold : FontStyle.Normal, TextAnchor.UpperCenter, reachable ? Ink : FaintInk);
            label.text = GetJourneyNodeDisplayName(node);
            SetFixed(label.rectTransform, new Vector2(0.5f, 1f), Vector2.up, position + new Vector2(0f, -size * 0.55f - 7f), new Vector2(210f, 36f));
        }

        private void BuildJourneyNodeScene(bool breakthrough)
        {
            DemoJourneyNode node = controller.PendingJourneyNode;
            if (node == null)
            {
                Text unavailable = CreateText(contentRoot, "NodeUnavailable", bodyFont, 20, FontStyle.Normal, TextAnchor.MiddleCenter, SoftInk);
                unavailable.text = "此处场景已散入矿雾。";
                Stretch(unavailable.rectTransform, new Vector2(80f, 230f), new Vector2(-80f, -120f));
                return;
            }

            Color accent = breakthrough ? Gold : GetJourneyNodeAccent(node.Type);
            Text glyph = CreateText(contentRoot, "NodeGlyph", titleFont, 128, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.16f));
            glyph.text = GetJourneyNodeGlyph(node.Type);
            SetFixed(glyph.rectTransform, new Vector2(0.24f, 0.52f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(310f, 310f));

            Text title = CreateText(contentRoot, "NodeTitle", titleFont, breakthrough ? 42 : 34, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            title.text = breakthrough ? "旧矿灵眼 · 以此筑基" : GetJourneyNodeDisplayName(node);
            title.resizeTextForBestFit = true;
            title.resizeTextMinSize = 25;
            title.resizeTextMaxSize = breakthrough ? 42 : 34;
            SetFixed(title.rectTransform, new Vector2(0.44f, 0.68f), Vector2.up, Vector2.zero, new Vector2(820f, 70f));

            Text location = CreateText(contentRoot, "NodeLocation", bodyFont, 14, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            location.text = $"第 {node.ActIndex} 幕 · 纵深 {node.DepthIndex + 1}/8 · {GetJourneyNodeTypeLabel(node.Type)}";
            SetFixed(location.rectTransform, new Vector2(0.44f, 0.68f), Vector2.up, new Vector2(0f, -76f), new Vector2(780f, 30f));

            Text narrative = CreateText(contentRoot, "NodeNarrative", bodyFont, 19, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            narrative.text = BuildJourneyNodeNarrative(node, breakthrough);
            SetFixed(narrative.rectTransform, new Vector2(0.44f, 0.68f), Vector2.up, new Vector2(0f, -122f), new Vector2(800f, 190f));

            RectTransform rule = CreateFixedPanel(
                contentRoot,
                "NodeRule",
                new Vector2(0.44f, 0.68f),
                Vector2.up,
                new Vector2(0f, -330f),
                new Vector2(720f, 2f),
                new Color(accent.r, accent.g, accent.b, 0.42f));
            rule.pivot = Vector2.up;

            Text consequence = CreateText(contentRoot, "NodeConsequence", bodyFont, 16, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            consequence.text = BuildJourneyNodeConsequence(node, breakthrough);
            SetFixed(consequence.rectTransform, new Vector2(0.44f, 0.68f), Vector2.up, new Vector2(0f, -356f), new Vector2(780f, 92f));

            CreateActionButton(
                contentRoot,
                "CompleteJourneyNode",
                new Vector2(0.66f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 58f),
                new Vector2(390f, 66f),
                breakthrough ? "引气贯脉 · 筑成道基" : GetJourneyNodeActionLabel(node.Type),
                accent,
                true,
                CompleteJourneyNode);
        }

        private void SelectJourneyMapNode(string nodeId)
        {
            renderedSignature = string.Empty;
            controller.SelectJourneyNode(nodeId);
            RefreshNow();
        }

        private void CompleteJourneyNode()
        {
            renderedSignature = string.Empty;
            controller.CompleteJourneyNode();
            RefreshNow();
        }

        private void BuildRewardChoice(DemoFlowPhase phase)
        {
            Color pageAccent = GetPageAccent(phase);
            string duty = BuildUtilityDutyText(phase);
            Text dutyText = CreateText(contentRoot, "Duty", bodyFont, 16, FontStyle.Bold, TextAnchor.MiddleCenter, pageAccent);
            dutyText.text = duty;
            SetFixed(dutyText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -221f), new Vector2(1320f, 38f));

            const float cardWidth = 470f;
            const float cardHeight = 470f;
            for (int i = 0; i < 3; i++)
            {
                float x = (i - 1) * 510f;
                DemoReward reward = i < controller.CurrentRewards.Count ? controller.CurrentRewards[i] : null;
                BuildRewardCard(i, reward, new Vector2(x, -4f), new Vector2(cardWidth, cardHeight), phase, pageAccent);
            }

            string buttonLabel = selectedIndex >= 0 && selectedIndex < controller.CurrentRewards.Count
                ? "确认收下 · " + Trim(controller.CurrentRewards[selectedIndex].Name, 12)
                : "先选定一项所得";
            CreateActionButton(
                contentRoot,
                "ConfirmReward",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 48f),
                new Vector2(390f, 66f),
                buttonLabel,
                pageAccent,
                selectedIndex >= 0 && selectedIndex < controller.CurrentRewards.Count,
                ConfirmSelectedReward);
        }

        private void BuildRewardCard(
            int index,
            DemoReward reward,
            Vector2 position,
            Vector2 size,
            DemoFlowPhase phase,
            Color pageAccent)
        {
            DemoRewardSlot slot = reward?.Slot ?? (DemoRewardSlot)(Mathf.Clamp(index + 1, 1, 3));
            Color slotAccent = Color.Lerp(GetSlotAccent(slot), pageAccent, phase == DemoFlowPhase.RewardChoice ? 0.12f : 0.42f);
            bool selected = index == selectedIndex && reward != null;
            Color surface = selected
                ? Color.Lerp(Paper, new Color(slotAccent.r, slotAccent.g, slotAccent.b, 1f), 0.13f)
                : Paper;
            RectTransform card = CreateFixedPanel(
                contentRoot, "RewardCard_" + index, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                position, size, surface);
            Image cardImage = card.GetComponent<Image>();
            cardImage.raycastTarget = reward != null;
            CreateBorder(card, selected ? slotAccent : new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.55f), selected ? 3f : 1.5f);
            CreatePanel(card, "SlotWash", new Vector2(0f, 0.90f), Vector2.one, new Color(slotAccent.r, slotAccent.g, slotAccent.b, selected ? 0.18f : 0.10f));

            if (reward == null)
            {
                Text empty = CreateText(card, "Empty", bodyFont, 18, FontStyle.Normal, TextAnchor.MiddleCenter, FaintInk);
                empty.text = "候选待显";
                Stretch(empty.rectTransform, new Vector2(24f, 24f), new Vector2(-24f, -24f));
                return;
            }

            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = cardImage;
            ConfigureChoiceButton(button);
            int captured = index;
            button.onClick.AddListener(() => SelectChoice(captured));

            Text slotText = CreateText(card, "Slot", bodyFont, 14, FontStyle.Bold, TextAnchor.MiddleLeft, slotAccent);
            slotText.text = GetSlotLabel(slot);
            SetAnchors(slotText.rectTransform, new Vector2(0.065f, 0.915f), new Vector2(0.72f, 0.98f));
            Text order = CreateText(card, "Order", bodyFont, 12, FontStyle.Bold, TextAnchor.MiddleRight, new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.82f));
            order.text = "0" + (index + 1);
            SetAnchors(order.rectTransform, new Vector2(0.75f, 0.915f), new Vector2(0.935f, 0.98f));

            Text name = CreateText(card, "Name", titleFont, 25, FontStyle.Bold, TextAnchor.MiddleLeft, Ink);
            name.text = reward.Name;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 18;
            name.resizeTextMaxSize = 25;
            SetAnchors(name.rectTransform, new Vector2(0.065f, 0.765f), new Vector2(0.935f, 0.895f));

            Text type = CreateText(card, "Type", bodyFont, 13, FontStyle.Bold, TextAnchor.UpperLeft, FaintInk);
            type.text = GetRewardTypeLabel(reward);
            SetAnchors(type.rectTransform, new Vector2(0.065f, 0.695f), new Vector2(0.935f, 0.755f));

            RectTransform typeSeal = CreateFixedPanel(
                card, "TypeSeal", new Vector2(1f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(-72f, 18f), new Vector2(82f, 82f), new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.065f));
            typeSeal.GetComponent<Image>().raycastTarget = false;
            CreateBorder(typeSeal, new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.20f), 1f);
            Text typeGlyph = CreateText(typeSeal, "TypeGlyph", titleFont, 40, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.24f));
            typeGlyph.text = GetRewardTypeGlyph(reward);
            Stretch(typeGlyph.rectTransform, new Vector2(4f, 4f), new Vector2(-4f, -4f));

            RectTransform divider = CreatePanel(card, "Divider", new Vector2(0.065f, 0.675f), new Vector2(0.935f, 0.675f), new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.34f));
            divider.offsetMin = new Vector2(0f, -1f);
            divider.offsetMax = new Vector2(0f, 1f);

            Text rules = CreateText(card, "Rules", bodyFont, 17, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            rules.text = string.IsNullOrWhiteSpace(reward.Description) ? "此项将补强当前道途。" : reward.Description;
            SetAnchors(rules.rectTransform, new Vector2(0.065f, 0.405f), new Vector2(0.735f, 0.65f));

            RectTransform deltaWash = CreatePanel(card, "BuildDeltaWash", new Vector2(0.04f, 0.14f), new Vector2(0.96f, 0.36f), new Color(slotAccent.r, slotAccent.g, slotAccent.b, 0.075f));
            Text deltaTitle = CreateText(deltaWash, "DeltaTitle", bodyFont, 12, FontStyle.Bold, TextAnchor.UpperLeft, slotAccent);
            deltaTitle.text = "构筑变化";
            SetAnchors(deltaTitle.rectTransform, new Vector2(0.035f, 0.64f), new Vector2(0.965f, 0.94f));
            Text delta = CreateText(deltaWash, "Delta", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            delta.text = string.IsNullOrWhiteSpace(reward.BuildDelta)
                ? BuildRewardDeltaFallback(reward)
                : reward.BuildDelta;
            SetAnchors(delta.rectTransform, new Vector2(0.035f, 0.08f), new Vector2(0.965f, 0.62f));

            Text selection = CreateText(card, "Selection", bodyFont, 13, FontStyle.Bold, TextAnchor.LowerCenter, selected ? slotAccent : FaintInk);
            selection.text = selected ? "已选定 · 在下方确认" : "点击选定";
            SetAnchors(selection.rectTransform, new Vector2(0.05f, 0.025f), new Vector2(0.95f, 0.105f));
        }

        private void BuildRouteChoice()
        {
            Text duty = CreateText(contentRoot, "Duty", bodyFont, 16, FontStyle.Bold, TextAnchor.MiddleCenter, Azure);
            duty.text = "先看节点顺序，再比较风险、恢复与保证收益。选择的是接下来怎样历练。";
            SetFixed(duty.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -221f), new Vector2(1320f, 38f));

            const float cardWidth = 490f;
            const float cardHeight = 480f;
            for (int i = 0; i < 3; i++)
            {
                float x = (i - 1) * 520f;
                DemoReward reward = i < controller.CurrentRewards.Count ? controller.CurrentRewards[i] : null;
                BuildRouteCard(i, reward, new Vector2(x, 0f), new Vector2(cardWidth, cardHeight));
            }

            string buttonLabel = selectedIndex >= 0 && selectedIndex < controller.CurrentRewards.Count
                ? "踏上 · " + Trim(controller.CurrentRewards[selectedIndex].Name, 12)
                : "先选定一条前路";
            CreateActionButton(
                contentRoot, "ConfirmRoute", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 48f), new Vector2(390f, 66f), buttonLabel, Azure,
                selectedIndex >= 0 && selectedIndex < controller.CurrentRewards.Count,
                ConfirmSelectedReward);
        }

        private void BuildRouteCard(int index, DemoReward reward, Vector2 position, Vector2 size)
        {
            Color accent = GetRouteAccent(reward);
            bool selected = reward != null && selectedIndex == index;
            RectTransform card = CreateFixedPanel(
                contentRoot, "RouteCard_" + index, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                position, size, selected ? Color.Lerp(Paper, accent, 0.12f) : Paper);
            Image image = card.GetComponent<Image>();
            image.raycastTarget = reward != null;
            CreateBorder(card, new Color(accent.r, accent.g, accent.b, selected ? 0.95f : 0.55f), selected ? 3f : 1.5f);

            if (reward?.RoutePlan == null)
            {
                Text empty = CreateText(card, "Empty", bodyFont, 18, FontStyle.Normal, TextAnchor.MiddleCenter, FaintInk);
                empty.text = "前路尚在雾中";
                Stretch(empty.rectTransform, new Vector2(24f, 24f), new Vector2(-24f, -24f));
                return;
            }

            Button button = card.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ConfigureChoiceButton(button);
            int captured = index;
            button.onClick.AddListener(() => SelectChoice(captured));

            DemoMapRoutePlan route = reward.RoutePlan;
            Text risk = CreateText(card, "Risk", bodyFont, 13, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            risk.text = GetRouteRiskLabel(route.Risk, reward.RouteTag);
            SetAnchors(risk.rectTransform, new Vector2(0.06f, 0.90f), new Vector2(0.94f, 0.97f));
            Text name = CreateText(card, "Name", titleFont, 26, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            name.text = route.Name;
            name.resizeTextForBestFit = true;
            name.resizeTextMinSize = 19;
            name.resizeTextMaxSize = 26;
            SetAnchors(name.rectTransform, new Vector2(0.06f, 0.79f), new Vector2(0.94f, 0.90f));
            Text description = CreateText(card, "Description", bodyFont, 16, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            description.text = route.Description;
            SetAnchors(description.rectTransform, new Vector2(0.06f, 0.67f), new Vector2(0.94f, 0.78f));

            BuildRouteNodeSequence(card, route, accent);

            int battleCount = route.Nodes.Count(node => node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss);
            int healing = GetRouteGuaranteedHealing(route);
            Text facts = CreateText(card, "Facts", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            facts.text = $"战斗  {battleCount} 场     恢复  {(healing > 0 ? healing + " 气血" : "无保证")}";
            SetAnchors(facts.rectTransform, new Vector2(0.06f, 0.25f), new Vector2(0.94f, 0.33f));

            Text guaranteeTitle = CreateText(card, "GuaranteeTitle", bodyFont, 12, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            guaranteeTitle.text = "保证收益";
            SetAnchors(guaranteeTitle.rectTransform, new Vector2(0.06f, 0.18f), new Vector2(0.94f, 0.24f));
            Text guarantee = CreateText(card, "Guarantee", bodyFont, 15, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            guarantee.text = BuildRouteGuarantee(route);
            SetAnchors(guarantee.rectTransform, new Vector2(0.06f, 0.07f), new Vector2(0.94f, 0.18f));

            Text selection = CreateText(card, "Selection", bodyFont, 12, FontStyle.Bold, TextAnchor.LowerRight, selected ? accent : FaintInk);
            selection.text = selected ? "已选定" : "点击选定";
            SetAnchors(selection.rectTransform, new Vector2(0.55f, 0.015f), new Vector2(0.94f, 0.065f));
        }

        private void BuildRouteNodeSequence(RectTransform card, DemoMapRoutePlan route, Color accent)
        {
            int count = Mathf.Clamp(route.Nodes.Count, 0, 4);
            if (count == 0)
            {
                return;
            }

            RectTransform line = CreatePanel(card, "NodeLine", new Vector2(0.12f, 0.49f), new Vector2(0.88f, 0.49f), new Color(accent.r, accent.g, accent.b, 0.34f));
            line.offsetMin = new Vector2(0f, -1f);
            line.offsetMax = new Vector2(0f, 1f);
            for (int i = 0; i < count; i++)
            {
                float normalized = count == 1 ? 0.5f : Mathf.Lerp(0.12f, 0.88f, i / (float)(count - 1));
                DemoMapNode node = route.Nodes[i];
                Color nodeColor = GetNodeAccent(node, accent);
                RectTransform dot = CreateFixedPanel(
                    card, "NodeDot_" + i, new Vector2(normalized, 0.49f), new Vector2(0.5f, 0.5f),
                    Vector2.zero, new Vector2(34f, 34f), new Color(nodeColor.r, nodeColor.g, nodeColor.b, 0.96f));
                Text order = CreateText(dot, "Order", bodyFont, 11, FontStyle.Bold, TextAnchor.MiddleCenter, Color.white);
                order.text = (i + 1).ToString();
                Stretch(order.rectTransform, Vector2.zero, Vector2.zero);
                Text type = CreateText(card, "NodeType_" + i, bodyFont, 12, FontStyle.Bold, TextAnchor.UpperCenter, Ink);
                type.text = GetNodeTypeLabel(node);
                SetFixed(type.rectTransform, new Vector2(normalized, 0.49f), new Vector2(0.5f, 1f), new Vector2(0f, -25f), new Vector2(102f, 22f));
                Text nodeName = CreateText(card, "NodeName_" + i, bodyFont, 11, FontStyle.Normal, TextAnchor.UpperCenter, FaintInk);
                nodeName.text = Trim(node.Name, 7);
                SetFixed(nodeName.rectTransform, new Vector2(normalized, 0.49f), new Vector2(0.5f, 1f), new Vector2(0f, -48f), new Vector2(104f, 34f));
            }
        }

        private void BuildEncounter(bool bossGate)
        {
            DemoMapNode node = controller.PendingEncounter ?? controller.Run.Map.CurrentNode;
            DemoEnemyDefinition enemy = null;
            if (node != null && !string.IsNullOrWhiteSpace(node.EncounterId))
            {
                DemoConfigRepository.TryGetEnemyById(node.EncounterId, out enemy);
            }

            Color accent = bossGate ? Cinnabar : Azure;
            RectTransform accentLine = CreateFixedPanel(
                contentRoot, "EncounterAccent", new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(116f, -282f), new Vector2(4f, 514f), new Color(accent.r, accent.g, accent.b, 0.84f));
            Text preface = CreateText(contentRoot, "EncounterPreface", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            preface.text = bossGate ? "渡劫门前 · 构筑终检" : "前情已明 · 即将交锋";
            SetFixed(preface.rectTransform, Vector2.up, Vector2.up, new Vector2(146f, -278f), new Vector2(730f, 28f));

            Text enemyName = CreateText(contentRoot, "EnemyName", titleFont, bossGate ? 44 : 38, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            enemyName.text = enemy?.Name ?? node?.Name ?? (bossGate ? "天劫化身" : "旧矿来敌");
            enemyName.resizeTextForBestFit = true;
            enemyName.resizeTextMinSize = 28;
            enemyName.resizeTextMaxSize = bossGate ? 44 : 38;
            SetFixed(enemyName.rectTransform, Vector2.up, Vector2.up, new Vector2(144f, -316f), new Vector2(760f, 70f));

            string risk = GetEncounterRisk(enemy, bossGate);
            DemoRewardProfileDefinition rewardProfile = null;
            if (node != null && !string.IsNullOrWhiteSpace(node.RewardProfileId))
            {
                DemoConfigRepository.TryGetRewardProfile(node.RewardProfileId, out rewardProfile);
            }

            Text facts = CreateText(contentRoot, "EncounterFacts", bodyFont, 17, FontStyle.Bold, TextAnchor.UpperLeft, SoftInk);
            facts.text = $"风险  {risk}     敌方气血  {Mathf.Max(0, enemy?.MaxHealth ?? 0)}     奖励档  {GetRewardTierLabel(rewardProfile?.Tier)}";
            SetFixed(facts.rectTransform, Vector2.up, Vector2.up, new Vector2(146f, -392f), new Vector2(800f, 34f));

            Text notes = CreateText(contentRoot, "EncounterNotes", bodyFont, 17, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            notes.text = BuildEncounterDescription(enemy, rewardProfile, bossGate);
            SetFixed(notes.rectTransform, Vector2.up, Vector2.up, new Vector2(146f, -446f), new Vector2(bossGate ? 710f : 800f, 112f));

            if (bossGate)
            {
                BuildBossGateReadiness(new Vector2(146f, -580f));
                RectTransform bossImage = CreateFixedPanel(
                    contentRoot, "BossPortrait", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                    new Vector2(-80f, -15f), new Vector2(780f, 760f), Color.clear);
                ApplySprite(bossImage.GetComponent<Image>(), LoadSprite(BossResourcePath), new Color(1f, 1f, 1f, 0.88f), true);
            }
            else
            {
                BuildEncounterPromise(new Vector2(146f, -590f), accent, rewardProfile);
                RectTransform enemyImage = CreateFixedPanel(
                    contentRoot, "EncounterPortrait", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                    new Vector2(-78f, -10f), new Vector2(690f, 690f), Color.clear);
                ApplySprite(enemyImage.GetComponent<Image>(), LoadSprite(NormalEnemyResourcePath), new Color(1f, 1f, 1f, 0.82f), true);
            }

            CreateActionButton(
                contentRoot,
                bossGate ? "BeginBoss" : "BeginEncounter",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 52f),
                new Vector2(410f, 68f),
                bossGate ? "引劫 · 直面天劫化身" : "入阵 · 开始此战",
                accent,
                controller.HasPendingEncounter,
                BeginEncounter);
        }

        private void BuildEncounterPromise(Vector2 position, Color accent, DemoRewardProfileDefinition profile)
        {
            Text title = CreateText(contentRoot, "EncounterPromiseTitle", titleFont, 26, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            title.text = "此战检验";
            SetFixed(title.rectTransform, Vector2.up, Vector2.up, position, new Vector2(650f, 46f));
            string profileText = string.IsNullOrWhiteSpace(profile?.Description)
                ? "飞剑循环能否稳定运转，并在敌方意图落下前建立优势。"
                : profile.Description;
            Text body = CreateText(contentRoot, "EncounterPromiseBody", bodyFont, 18, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            body.text = profileText + "\n\n胜后立即进入战后所得，再由新组件决定下一段前路。";
            SetFixed(body.rectTransform, Vector2.up, Vector2.up, position + new Vector2(0f, -58f), new Vector2(650f, 190f));
            RectTransform rule = CreateFixedPanel(
                contentRoot, "EncounterPromiseRule", Vector2.up, Vector2.up,
                position + new Vector2(0f, -270f), new Vector2(540f, 2f), new Color(accent.r, accent.g, accent.b, 0.42f));
            rule.pivot = Vector2.up;
        }

        private void BuildBossGateReadiness(Vector2 position)
        {
            DemoRunState run = controller.Run;
            Text heading = CreateText(contentRoot, "ReadinessTitle", titleFont, 22, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            heading.text = "渡劫构筑门槛";
            SetFixed(heading.rectTransform, Vector2.up, Vector2.up, position, new Vector2(680f, 38f));

            bool growth = run.BonusPermanentSwords > 0 || run.Deck.Any(card => card != null && (card.PermanentSword || card.TemporarySwords > 0));
            bool array = run.HasBuildComponent("sword_array");
            bool engine = run.HasBuildComponent("gongfa_sword_control_art") || run.HasBuildComponent("artifact_sword_box");
            bool finisher = run.HasBuildComponent("wanjian_burst");
            bool health = run.CurrentHealth >= Mathf.CeilToInt(run.MaxHealth * 0.45f);

            BuildReadinessRow(position + new Vector2(0f, -48f), "飞剑增殖", "至少一项飞剑生成能力", growth);
            BuildReadinessRow(position + new Vector2(0f, -91f), "剑阵运转", "小诛仙剑阵", array);
            BuildReadinessRow(position + new Vector2(0f, -134f), "引擎核心", "御剑诀或剑匣", engine);
            BuildReadinessRow(position + new Vector2(0f, -177f), "万剑收束", "万剑诀", finisher);
            BuildReadinessRow(position + new Vector2(0f, -220f), "生存底线", "气血不低于四成半", health);
        }

        private void BuildReadinessRow(Vector2 position, string label, string detail, bool ready)
        {
            Color color = ready ? Jade : Cinnabar;
            RectTransform dot = CreateFixedPanel(
                contentRoot, "ReadinessDot_" + label, Vector2.up, new Vector2(0.5f, 0.5f),
                position + new Vector2(8f, -10f), new Vector2(15f, 15f), color);
            Text text = CreateText(contentRoot, "Readiness_" + label, bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            text.text = $"{label}     {detail}     {(ready ? "已成" : "尚缺")}";
            SetFixed(text.rectTransform, Vector2.up, Vector2.up, position + new Vector2(30f, 0f), new Vector2(620f, 30f));
        }

        private void BuildRunResult()
        {
            DemoRunSummary summary = controller.RunSummary;
            if (summary == null)
            {
                Text unavailable = CreateText(contentRoot, "NoResult", bodyFont, 20, FontStyle.Normal, TextAnchor.MiddleCenter, SoftInk);
                unavailable.text = "此世尚未结卷。";
                Stretch(unavailable.rectTransform, new Vector2(80f, 220f), new Vector2(-80f, -140f));
                return;
            }

            Color accent = summary.Victory ? Gold : Cinnabar;
            RectTransform portrait = CreateFixedPanel(
                contentRoot, "ResultBossEcho", new Vector2(1f, 0.5f), new Vector2(1f, 0.5f),
                new Vector2(-48f, -25f), new Vector2(690f, 690f), Color.clear);
            ApplySprite(portrait.GetComponent<Image>(), LoadSprite(BossResourcePath), new Color(1f, 1f, 1f, summary.Victory ? 0.22f : 0.12f), true);

            Text outcome = CreateText(contentRoot, "Outcome", titleFont, 42, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            outcome.text = summary.Victory ? "天劫已渡，道途留痕" : "此世止步，道途归卷";
            outcome.resizeTextForBestFit = true;
            outcome.resizeTextMinSize = 28;
            outcome.resizeTextMaxSize = 42;
            SetFixed(outcome.rectTransform, Vector2.up, Vector2.up, new Vector2(116f, -275f), new Vector2(900f, 66f));
            Text resultDetail = CreateText(contentRoot, "OutcomeDetail", bodyFont, 16, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            resultDetail.text = summary.Victory
                ? $"抵达第 {summary.ReachedLayer} 层，击破天劫化身。本世构筑已完整走完一次验证。"
                : BuildFailureText(summary);
            SetFixed(resultDetail.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -344f), new Vector2(910f, 58f));

            BuildMetric(new Vector2(120f, -425f), "胜战", summary.BattlesWon.ToString(), accent);
            BuildMetric(new Vector2(350f, -425f), "最大飞剑", summary.MaxSwordCount.ToString(), accent);
            BuildMetric(new Vector2(580f, -425f), "最高爆发", summary.HighestBurstDamage.ToString(), accent);
            BuildMetric(new Vector2(810f, -425f), "用时", FormatDuration(summary.DurationSeconds), accent);

            Text routeTitle = CreateText(contentRoot, "RouteHistoryTitle", bodyFont, 13, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            routeTitle.text = "所行前路";
            SetFixed(routeTitle.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -565f), new Vector2(840f, 26f));
            Text routes = CreateText(contentRoot, "RouteHistory", titleFont, 21, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            routes.text = BuildRouteHistory(summary);
            SetFixed(routes.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -596f), new Vector2(900f, 58f));

            Text buildTitle = CreateText(contentRoot, "BuildTitle", bodyFont, 13, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            buildTitle.text = "核心构筑";
            SetFixed(buildTitle.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -676f), new Vector2(840f, 26f));
            Text build = CreateText(contentRoot, "Build", bodyFont, 16, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            build.text = BuildCoreComponentText(summary);
            SetFixed(build.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -708f), new Vector2(910f, 70f));

            Text schools = CreateText(contentRoot, "Schools", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            schools.text = $"主修  {Fallback(summary.MainGongfaName, "未定主修")}     法器  {Fallback(summary.CoreArtifactName, "未获核心法器")}";
            SetFixed(schools.rectTransform, Vector2.up, Vector2.up, new Vector2(120f, -792f), new Vector2(930f, 34f));

            RectTransform unlockWash = CreatePanel(contentRoot, "UnlockWash", new Vector2(0.58f, 0.24f), new Vector2(0.94f, 0.48f), new Color(accent.r, accent.g, accent.b, 0.075f));
            Text unlockTitle = CreateText(unlockWash, "UnlockTitle", titleFont, 22, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            unlockTitle.text = summary.NewUnlocks != null && summary.NewUnlocks.Count > 0 ? "新解锁" : "卷后所得";
            SetAnchors(unlockTitle.rectTransform, new Vector2(0.07f, 0.70f), new Vector2(0.93f, 0.92f));
            Text unlock = CreateText(unlockWash, "Unlock", bodyFont, 17, FontStyle.Normal, TextAnchor.UpperLeft, SoftInk);
            unlock.text = BuildUnlockText(summary);
            SetAnchors(unlock.rectTransform, new Vector2(0.07f, 0.14f), new Vector2(0.93f, 0.66f));

            CreateActionButton(
                contentRoot, "NextRun", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(-210f, 50f), new Vector2(360f, 66f), "再启一世", accent, true, StartNextRun);
            CreateSecondaryButton(
                contentRoot, "ReturnHome", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(210f, 50f), new Vector2(300f, 66f), "归于开卷", ReturnHome);
        }

        private void BuildMetric(Vector2 position, string label, string value, Color accent)
        {
            Text labelText = CreateText(contentRoot, "MetricLabel_" + label, bodyFont, 12, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            labelText.text = label;
            SetFixed(labelText.rectTransform, Vector2.up, Vector2.up, position, new Vector2(190f, 24f));
            Text valueText = CreateText(contentRoot, "MetricValue_" + label, titleFont, 29, FontStyle.Bold, TextAnchor.UpperLeft, Ink);
            valueText.text = value;
            valueText.resizeTextForBestFit = true;
            valueText.resizeTextMinSize = 21;
            valueText.resizeTextMaxSize = 29;
            SetFixed(valueText.rectTransform, Vector2.up, Vector2.up, position + new Vector2(0f, -27f), new Vector2(190f, 46f));
            RectTransform separator = CreateFixedPanel(
                contentRoot, "MetricRule_" + label, Vector2.up, Vector2.up,
                position + new Vector2(196f, -4f), new Vector2(1f, 65f), new Color(accent.r, accent.g, accent.b, 0.28f));
            separator.pivot = Vector2.up;
        }

        private void SelectChoice(int index)
        {
            if (controller == null || index < 0 || index >= controller.CurrentRewards.Count)
            {
                return;
            }

            selectedIndex = index;
            renderedSignature = string.Empty;
            RefreshNow();
        }

        private void ConfirmSelectedReward()
        {
            if (controller == null || selectedIndex < 0 || selectedIndex >= controller.CurrentRewards.Count)
            {
                return;
            }

            int claimedIndex = selectedIndex;
            selectedIndex = -1;
            renderedSignature = string.Empty;
            controller.ClaimRewardAt(claimedIndex);
            RefreshNow();
        }

        private void BeginEncounter()
        {
            if (controller == null || !controller.HasPendingEncounter)
            {
                return;
            }

            renderedSignature = string.Empty;
            controller.BeginCurrentEncounter();
            RefreshNow();
        }

        private void StartNextRun()
        {
            if (controller == null)
            {
                return;
            }

            renderedSignature = string.Empty;
            controller.StartNextRun();
            RefreshNow();
        }

        private void ReturnHome()
        {
            if (controller == null)
            {
                return;
            }

            renderedSignature = string.Empty;
            controller.ReturnHome();
            RefreshNow();
        }

        private string BuildSignature(DemoFlowSnapshot snapshot)
        {
            StringBuilder builder = new StringBuilder(512);
            builder.Append(snapshot.Sequence).Append('|').Append(snapshot.Phase).Append('|').Append(selectedIndex);
            if (!IsJourneyPhase(snapshot.Phase))
            {
                return builder.ToString();
            }

            DemoRunState run = controller.Run;
            builder.Append('|').Append(run.CurrentHealth).Append('/').Append(run.MaxHealth)
                .Append('|').Append(run.Deck.Count)
                .Append('|').Append(run.MainGongfa)
                .Append('|').Append(run.Artifacts.Count)
                .Append('|').Append(run.Map.CurrentNode?.NodeId)
                .Append('|').Append(controller.PendingEncounter?.NodeId);

            DemoRunSaveV2 journey = controller.JourneySnapshot;
            if (journey != null)
            {
                builder.Append("|journey:").Append(journey.CheckpointSequence)
                    .Append(':').Append(journey.ActIndex)
                    .Append(':').Append(journey.CurrentNodeId)
                    .Append(':').Append(journey.FlowPhaseId)
                    .Append(':').Append(journey.Realm?.RealmId)
                    .Append(':').Append(journey.Realm?.FoundationRuleId);
                AppendStrings(builder, journey.CompletedNodeIds);
                AppendStrings(builder, journey.ReachableNodeIds);
                AppendStrings(builder, journey.ExperienceFlagIds);
            }

            for (int i = 0; i < controller.CurrentRewards.Count; i++)
            {
                DemoReward reward = controller.CurrentRewards[i];
                builder.Append("|r:").Append(i).Append(':').Append(reward?.Type).Append(':')
                    .Append(reward?.Slot).Append(':').Append(reward?.Name).Append(':')
                    .Append(reward?.BuildDelta).Append(':').Append(reward?.RoutePlan?.Id);
            }

            DemoRunSummary summary = controller.RunSummary;
            if (summary != null)
            {
                builder.Append("|s:").Append(summary.Victory).Append(':').Append(summary.ReachedLayer)
                    .Append(':').Append(summary.BattlesWon).Append(':').Append(summary.MaxSwordCount)
                    .Append(':').Append(summary.HighestBurstDamage).Append(':').Append(summary.DurationSeconds)
                    .Append(':').Append(summary.MainGongfaName).Append(':').Append(summary.CoreArtifactName);
                AppendStrings(builder, summary.CoreComponents);
                AppendStrings(builder, summary.NewUnlocks);
                if (summary.RouteHistory != null)
                {
                    for (int i = 0; i < summary.RouteHistory.Count; i++)
                    {
                        builder.Append("|route:").Append(summary.RouteHistory[i]?.RouteId).Append(':').Append(summary.RouteHistory[i]?.RouteName);
                    }
                }
            }

            return builder.ToString();
        }

        private static void AppendStrings(StringBuilder builder, IEnumerable<string> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (string value in values)
            {
                builder.Append('|').Append(value);
            }
        }

        private static bool IsJourneyPhase(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.JourneyMap:
                case DemoFlowPhase.NodeScene:
                case DemoFlowPhase.Breakthrough:
                case DemoFlowPhase.EncounterIntro:
                case DemoFlowPhase.RewardChoice:
                case DemoFlowPhase.RouteChoice:
                case DemoFlowPhase.Training:
                case DemoFlowPhase.Preparation:
                case DemoFlowPhase.BossGate:
                case DemoFlowPhase.RunResult:
                    return true;
                default:
                    return false;
            }
        }

        private static string GetPageTitle(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.JourneyMap:
                    return "旧矿长卷";
                case DemoFlowPhase.NodeScene:
                    return "矿中一遇";
                case DemoFlowPhase.Breakthrough:
                    return "炼气筑基";
                case DemoFlowPhase.RewardChoice:
                    return "战后所得";
                case DemoFlowPhase.RouteChoice:
                    return "择前路";
                case DemoFlowPhase.Training:
                    return "静修悟法";
                case DemoFlowPhase.Preparation:
                    return "整备道基";
                case DemoFlowPhase.BossGate:
                    return "剑傀镇炉";
                case DemoFlowPhase.RunResult:
                    return "一世结算";
                default:
                    return "战前情势";
            }
        }

        private static string GetPageSubtitle(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.JourneyMap:
                    return "循着已走墨迹选择下一处可达节点，三幕始终属于同一座旧矿。";
                case DemoFlowPhase.NodeScene:
                    return "能力、线索与代价都从眼前场景而来，不在战后凭空出现。";
                case DemoFlowPhase.Breakthrough:
                    return "前两幕所得在此汇成道基，基础方案永远可用，经历决定额外规则。";
                case DemoFlowPhase.RewardChoice:
                    return "所得只取其一。先看规则如何改变，再看它把构筑推向何处。";
                case DemoFlowPhase.RouteChoice:
                    return "前路不是流派重选；节点先后、风险和恢复才是这一页的决定。";
                case DemoFlowPhase.Training:
                    return "此处修炼用于定向补缺，在下一场斗法前把关键环节接上。";
                case DemoFlowPhase.Preparation:
                    return "整备重在器物、循环与生存，为之后的连续斗法留出余地。";
                case DemoFlowPhase.BossGate:
                    return "玄铁甲片、朱砂契钉与剑炉核心俱在上方，锁定并逐一击破。";
                case DemoFlowPhase.RunResult:
                    return "把走过的路、形成的构筑和真正打出的高点留在此卷。";
                default:
                    return "敌情、风险与奖励已列明。确认当前构筑能够承担此战。";
            }
        }

        private static string BuildChapterLabel(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.JourneyMap:
                    return "旧矿三幕";
                case DemoFlowPhase.NodeScene:
                    return "此处所求";
                case DemoFlowPhase.Breakthrough:
                    return "第二幕终章";
                case DemoFlowPhase.RewardChoice:
                    return "破敌之后";
                case DemoFlowPhase.RouteChoice:
                    return "三路分岔";
                case DemoFlowPhase.Training:
                    return "修炼节点";
                case DemoFlowPhase.Preparation:
                    return "整备节点";
                case DemoFlowPhase.BossGate:
                    return "剑炉之前";
                case DemoFlowPhase.RunResult:
                    return "此世归卷";
                default:
                    return "遭遇之前";
            }
        }

        private static Color GetPageAccent(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.JourneyMap:
                case DemoFlowPhase.NodeScene:
                    return Jade;
                case DemoFlowPhase.Breakthrough:
                    return Gold;
                case DemoFlowPhase.RewardChoice:
                    return Cinnabar;
                case DemoFlowPhase.Training:
                    return Jade;
                case DemoFlowPhase.RouteChoice:
                case DemoFlowPhase.EncounterIntro:
                    return Azure;
                case DemoFlowPhase.Preparation:
                    return Gold;
                case DemoFlowPhase.BossGate:
                    return Cinnabar;
                default:
                    return Gold;
            }
        }

        private int GetProgressIndex(DemoFlowPhase phase)
        {
            if (controller.HasJourneySession)
            {
                DemoRunSaveV2 snapshot = controller.JourneySnapshot;
                int act = snapshot == null ? 1 : Mathf.Clamp(snapshot.ActIndex, 1, 3);
                if (phase == DemoFlowPhase.RunResult)
                {
                    return 6;
                }
                if (phase == DemoFlowPhase.BossGate)
                {
                    return 6;
                }
                if (phase == DemoFlowPhase.Breakthrough)
                {
                    return 4;
                }
                if (act == 1)
                {
                    return snapshot != null && snapshot.CompletedNodeIds.Count >= 7 ? 2 : 1;
                }
                return act == 2 ? 3 : 5;
            }

            if (phase == DemoFlowPhase.RunResult)
            {
                return 6;
            }

            if (phase == DemoFlowPhase.BossGate)
            {
                return 5;
            }

            if (phase == DemoFlowPhase.RewardChoice && controller.Run.Map.CurrentNode.Layer <= 1)
            {
                return 1;
            }

            int layer = controller.Run.Map.CurrentNode?.Layer ?? 1;
            return Mathf.Clamp(layer + 1, 2, 4);
        }

        private static string GetJourneyActTitle(int act)
        {
            switch (act)
            {
                case 1:
                    return "第一幕 · 浅层矿道";
                case 2:
                    return "第二幕 · 塌井与矿契";
                default:
                    return "第三幕 · 玄铁剑炉";
            }
        }

        private static Color GetJourneyNodeAccent(DemoJourneyNodeType type)
        {
            switch (type)
            {
                case DemoJourneyNodeType.Battle:
                case DemoJourneyNodeType.Elite:
                    return Cinnabar;
                case DemoJourneyNodeType.Cultivation:
                case DemoJourneyNodeType.Breakthrough:
                    return Jade;
                case DemoJourneyNodeType.Secret:
                case DemoJourneyNodeType.Refinement:
                    return Gold;
                case DemoJourneyNodeType.MiniBoss:
                case DemoJourneyNodeType.Boss:
                    return new Color(0.52f, 0.18f, 0.15f, 1f);
                default:
                    return Azure;
            }
        }

        private static string GetJourneyNodeGlyph(DemoJourneyNodeType type)
        {
            switch (type)
            {
                case DemoJourneyNodeType.Start:
                    return "入";
                case DemoJourneyNodeType.Battle:
                    return "斗";
                case DemoJourneyNodeType.Elite:
                    return "凶";
                case DemoJourneyNodeType.Event:
                    return "遇";
                case DemoJourneyNodeType.Cultivation:
                    return "修";
                case DemoJourneyNodeType.Secret:
                    return "藏";
                case DemoJourneyNodeType.Refinement:
                    return "炼";
                case DemoJourneyNodeType.Story:
                    return "契";
                case DemoJourneyNodeType.MiniBoss:
                    return "兽";
                case DemoJourneyNodeType.Breakthrough:
                    return "基";
                case DemoJourneyNodeType.Boss:
                    return "傀";
                default:
                    return "·";
            }
        }

        private static string GetJourneyNodeTypeLabel(DemoJourneyNodeType type)
        {
            switch (type)
            {
                case DemoJourneyNodeType.Start:
                    return "幕首故事";
                case DemoJourneyNodeType.Battle:
                    return "普通遭遇";
                case DemoJourneyNodeType.Elite:
                    return "高压遭遇";
                case DemoJourneyNodeType.Event:
                    return "人物奇遇";
                case DemoJourneyNodeType.Cultivation:
                    return "修炼";
                case DemoJourneyNodeType.Secret:
                    return "秘藏";
                case DemoJourneyNodeType.Refinement:
                    return "炼形";
                case DemoJourneyNodeType.Story:
                    return "矿契旧事";
                case DemoJourneyNodeType.MiniBoss:
                    return "幕末矿兽";
                case DemoJourneyNodeType.Breakthrough:
                    return "突破";
                case DemoJourneyNodeType.Boss:
                    return "区域守关";
                default:
                    return type.ToString();
            }
        }

        private static string GetJourneyNodeDisplayName(DemoJourneyNode node)
        {
            if (node == null)
            {
                return "未知矿痕";
            }

            if (node.Type == DemoJourneyNodeType.Boss)
            {
                return "玄铁镇矿剑傀";
            }
            if (node.Type == DemoJourneyNodeType.MiniBoss)
            {
                return node.ActIndex == 1 ? "铁脊矿兽" : "吞剑矿魈";
            }
            if (node.Type == DemoJourneyNodeType.Breakthrough)
            {
                return "旧矿灵眼";
            }

            string[] actOne = { "残照矿门", "碎轨伏影", "旁支矿印", "断壁剑痕", "契屑巢穴", "矿灵低语", "废炉炼形", "铁脊巢" };
            string[] actTwo = { "塌井余声", "悬桥残魂", "青绿矿脉", "吞剑伏巢", "旧账石室", "矿灵暗径", "剑碑静台", "矿魈壁窟" };
            string[] actThree = { "筑基灵眼", "炉前试锋", "朱契裂隙", "封剑廊", "旁支见证", "玄铁炼台", "剑炉门扉", "镇矿剑傀" };
            string[] names = node.ActIndex == 1 ? actOne : node.ActIndex == 2 ? actTwo : actThree;
            return names[Mathf.Clamp(node.DepthIndex, 0, names.Length - 1)];
        }

        private static string BuildJourneyNodeNarrative(DemoJourneyNode node, bool breakthrough)
        {
            if (breakthrough)
            {
                return "青绿矿脉在脚下缓慢呼吸，前两幕所得的剑痕、矿契与矿灵回应一同浮现。你不再从陌生选项里抽取境界，而是让这一世真正走过的路决定筑基时多出的那条规则。";
            }

            switch (node.Type)
            {
                case DemoJourneyNodeType.Start:
                    return "残剑胚牵动腕上旧痕，矿门深处回应祖龛里那张残契。入口天光仍在身后，但第一行旁支刻痕已经指向更深处。";
                case DemoJourneyNodeType.Cultivation:
                    return "断壁上的剑痕尚有余意。循着旁支吐纳法调息，可以稳住灵气循环，并从残痕中补全一式法诀。";
                case DemoJourneyNodeType.Refinement:
                    return "废炉仍存余温。将残剑胚放入炉心温养，可让本命法器更快归位，也让脚下承载与剑势一同变得清晰。";
                case DemoJourneyNodeType.Secret:
                    return "矿图背后藏着一处被封死的窄室。里面没有通用宝箱，只有与这座旧矿、这门剑法和旁支旧契直接相关的遗物。";
                case DemoJourneyNodeType.Event:
                case DemoJourneyNodeType.Story:
                    return "矿灵与残魂留下的并非善恶刻度，而是一件可以被记住的事实。你如何回应，将在筑基、剑傀阶段与最终因果中再次出现。";
                default:
                    return "旧矿深处传来器物摩擦与低沉回声。此处的选择会立刻改变这一世的构筑或状态，并在返回长卷后显出下一段墨迹。";
            }
        }

        private static string BuildJourneyNodeConsequence(DemoJourneyNode node, bool breakthrough)
        {
            if (breakthrough)
            {
                return "境界变化：炼气 -> 筑基。灵气上限与恢复提升，全部已学法诀获得统一成长；本局经历再解锁一条道基规则。";
            }

            switch (node.Type)
            {
                case DemoJourneyNodeType.Cultivation:
                    return "所得：心法升阶、恢复气血，并在牌库未满时悟得一门场景法诀。";
                case DemoJourneyNodeType.Refinement:
                    return "所得：残剑胚炼形升阶，自动攻击冷却缩短，恢复少量气血。";
                case DemoJourneyNodeType.Secret:
                    return "所得：记录秘藏经历，并在牌库未满时取得一门对应法诀。";
                case DemoJourneyNodeType.Event:
                case DemoJourneyNodeType.Story:
                    return "所得：记录具体经历标记；第二幕矿灵经历将影响道基与剑傀阶段。";
                default:
                    return "推进：完成此处交互并保存节点结算，新的可达路径随即显现。";
            }
        }

        private static string GetJourneyNodeActionLabel(DemoJourneyNodeType type)
        {
            switch (type)
            {
                case DemoJourneyNodeType.Cultivation:
                    return "循剑痕静修";
                case DemoJourneyNodeType.Refinement:
                    return "温养残剑胚";
                case DemoJourneyNodeType.Secret:
                    return "启封秘藏";
                case DemoJourneyNodeType.Event:
                case DemoJourneyNodeType.Story:
                    return "记下这一段因果";
                default:
                    return "循墨迹继续深入";
            }
        }

        private string BuildUtilityDutyText(DemoFlowPhase phase)
        {
            if (phase == DemoFlowPhase.RewardChoice)
            {
                return "三槽各司其职：补主轴、补生存循环，或承担一次高波动选择。";
            }

            DemoMapNode node = controller.Run.Map.CurrentNode;
            DemoNodeActionProfileDefinition profile = null;
            if (node != null && !string.IsNullOrWhiteSpace(node.ActionProfileId))
            {
                DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out profile);
            }

            string fixedBenefit = string.IsNullOrWhiteSpace(profile?.Description)
                ? phase == DemoFlowPhase.Training
                    ? "定向补齐当前层缺少的启动、剑阵或收束组件。"
                    : "围绕核心法器、回灵循环和生存缺口完成一次整备。"
                : profile.Description;
            string prefix = phase == DemoFlowPhase.Training ? "本次修炼" : "本次整备";
            return $"{prefix}固定方向：{fixedBenefit}";
        }

        private static Color GetSlotAccent(DemoRewardSlot slot)
        {
            switch (slot)
            {
                case DemoRewardSlot.Focus:
                    return Cinnabar;
                case DemoRewardSlot.Utility:
                    return Jade;
                case DemoRewardSlot.Wildcard:
                    return Gold;
                default:
                    return Azure;
            }
        }

        private static string GetSlotLabel(DemoRewardSlot slot)
        {
            switch (slot)
            {
                case DemoRewardSlot.Focus:
                    return "当前主轴 · FOCUS";
                case DemoRewardSlot.Utility:
                    return "生存资源 · UTILITY";
                case DemoRewardSlot.Wildcard:
                    return "高波动 · WILDCARD";
                default:
                    return "构筑候选";
            }
        }

        private static string GetRewardTypeLabel(DemoReward reward)
        {
            if (reward == null)
            {
                return string.Empty;
            }

            switch (reward.Type)
            {
                case DemoRewardType.Card:
                    return reward.Card == null ? "卡牌" : $"卡牌 · {reward.Card.Cost} 灵气";
                case DemoRewardType.Gongfa:
                    return "功法 · 长效规则";
                case DemoRewardType.Artifact:
                    return "法器 · 规则改变";
                case DemoRewardType.Heal:
                    return "调息 · 即时恢复";
                case DemoRewardType.Upgrade:
                    return "精修 · 灵气上限";
                default:
                    return "道途补强";
            }
        }

        private static string GetRewardTypeGlyph(DemoReward reward)
        {
            if (reward == null)
            {
                return "得";
            }

            switch (reward.Type)
            {
                case DemoRewardType.Card:
                    return "牌";
                case DemoRewardType.Gongfa:
                    return "诀";
                case DemoRewardType.Artifact:
                    return "器";
                case DemoRewardType.Heal:
                    return "息";
                case DemoRewardType.Upgrade:
                    return "炼";
                default:
                    return "得";
            }
        }

        private static string BuildRewardDeltaFallback(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Heal:
                    return "抬高当前气血，为下一次连续战斗留出容错。";
                case DemoRewardType.Upgrade:
                    return "扩大灵气容量，让高费爆发更容易接入循环。";
                case DemoRewardType.Artifact:
                    return "加入一项持续生效的法器规则。";
                case DemoRewardType.Gongfa:
                    return "确立一项长期运转的功法规则。";
                default:
                    return "将此组件加入牌组，改变后续抽取与联动。";
            }
        }

        private static Color GetRouteAccent(DemoReward reward)
        {
            DemoMapRoutePlan route = reward?.RoutePlan;
            if (route == null)
            {
                return Azure;
            }

            int battles = route.Nodes.Count(node => node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss);
            bool support = route.Nodes.Any(node => node.Type == DemoNodeType.Training || node.Type == DemoNodeType.Shop);
            if (battles >= 2)
            {
                return Cinnabar;
            }

            return support ? Jade : Azure;
        }

        private static string GetRouteRiskLabel(string risk, string fallbackTag)
        {
            string value = (risk ?? fallbackTag ?? string.Empty).Trim().ToLowerInvariant();
            if (value.Contains("risky") || value.Contains("aggressive") || value.Contains("desperate") || value.Contains("high"))
            {
                return "高险 · 奖励上探";
            }

            if (value.Contains("build") || value.Contains("artifact") || value.Contains("seclusion"))
            {
                return "构筑 · 定向补缺";
            }

            return "稳修 · 容错优先";
        }

        private static int GetRouteGuaranteedHealing(DemoMapRoutePlan route)
        {
            int total = 0;
            for (int i = 0; i < route.Nodes.Count; i++)
            {
                DemoMapNode node = route.Nodes[i];
                if (node != null
                    && !string.IsNullOrWhiteSpace(node.ActionProfileId)
                    && DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out DemoNodeActionProfileDefinition profile))
                {
                    total += Mathf.Max(0, profile.HealAmount);
                }
            }

            return total;
        }

        private static string BuildRouteGuarantee(DemoMapRoutePlan route)
        {
            List<string> guarantees = new List<string>();
            for (int i = 0; i < route.Nodes.Count; i++)
            {
                DemoMapNode node = route.Nodes[i];
                if (node == null || string.IsNullOrWhiteSpace(node.ActionProfileId))
                {
                    continue;
                }

                if (DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out DemoNodeActionProfileDefinition profile)
                    && !string.IsNullOrWhiteSpace(profile.Description))
                {
                    guarantees.Add(profile.Description);
                }
            }

            if (guarantees.Count > 0)
            {
                return Trim(string.Join("；", guarantees), 54);
            }

            return string.IsNullOrWhiteSpace(route.Description)
                ? "按节点顺序完成对应战斗与补强。"
                : Trim(route.Description, 54);
        }

        private static Color GetNodeAccent(DemoMapNode node, Color fallback)
        {
            if (node == null)
            {
                return fallback;
            }

            switch (node.Type)
            {
                case DemoNodeType.Battle:
                    return Azure;
                case DemoNodeType.Training:
                    return Jade;
                case DemoNodeType.Shop:
                    return Gold;
                case DemoNodeType.Boss:
                    return Cinnabar;
                default:
                    return fallback;
            }
        }

        private static string GetNodeTypeLabel(DemoMapNode node)
        {
            if (node == null)
            {
                return "节点";
            }

            switch (node.Type)
            {
                case DemoNodeType.Battle:
                    return "战斗";
                case DemoNodeType.Training:
                    return "修炼";
                case DemoNodeType.Shop:
                    return "整备";
                case DemoNodeType.Reward:
                    return "所得";
                case DemoNodeType.Boss:
                    return "天劫";
                case DemoNodeType.RouteChoice:
                    return "路口";
                default:
                    return "节点";
            }
        }

        private static string GetEncounterRisk(DemoEnemyDefinition enemy, bool bossGate)
        {
            if (bossGate || enemy?.IsBoss == true)
            {
                return "天劫 · 极高";
            }

            string role = (enemy?.BattleRole ?? string.Empty).ToLowerInvariant();
            return role.Contains("elite") ? "精英 · 高" : "寻常 · 中";
        }

        private static string GetRewardTierLabel(string tier)
        {
            switch ((tier ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "opening":
                    return "起势";
                case "elite":
                case "high":
                    return "高阶";
                case "build":
                    return "构筑";
                case "core":
                    return "核心";
                case "finisher":
                    return "收束";
                case "boss":
                    return "渡劫";
                default:
                    return "标准";
            }
        }

        private static string BuildEncounterDescription(
            DemoEnemyDefinition enemy,
            DemoRewardProfileDefinition profile,
            bool bossGate)
        {
            if (bossGate)
            {
                return "天劫化身以三段雷意持续压迫气血，最终蓄雷窗口是万剑爆发的主要时机。门槛不是强制锁门，而是提醒这一局还缺哪一环。";
            }

            string enemyText = string.IsNullOrWhiteSpace(enemy?.Notes)
                ? "敌方会按意图持续行动，需要在飞剑自动齐射之间用手牌稳定循环。"
                : enemy.Notes;
            string rewardText = string.IsNullOrWhiteSpace(profile?.Description)
                ? "胜后从三槽奖励中选择一项长期补强。"
                : profile.Description;
            return enemyText + "\n胜后所得：" + rewardText;
        }

        private static string BuildFailureText(DemoRunSummary summary)
        {
            if (string.IsNullOrWhiteSpace(summary.FailureNodeName))
            {
                return $"抵达第 {summary.ReachedLayer} 层。本世构筑已记录，可从路线和缺失组件中判断下一次调整。";
            }

            return $"止步于第 {summary.ReachedLayer} 层 · {summary.FailureNodeName}。本世构筑已记录，可从路线和缺失组件中判断下一次调整。";
        }

        private static string BuildRouteHistory(DemoRunSummary summary)
        {
            if (summary.RouteHistory == null || summary.RouteHistory.Count == 0)
            {
                return "未形成完整路线序列";
            }

            return string.Join("  →  ", summary.RouteHistory.Where(route => route != null).Select(route => Fallback(route.RouteName, "无名路")));
        }

        private static string BuildCoreComponentText(DemoRunSummary summary)
        {
            if (summary.CoreComponentDetails != null && summary.CoreComponentDetails.Count > 0)
            {
                return string.Join(" · ", summary.CoreComponentDetails
                    .Where(component => component != null)
                    .Select(component => Fallback(component.DisplayName, component.Id))
                    .Take(8));
            }

            if (summary.CoreComponents != null && summary.CoreComponents.Count > 0)
            {
                return string.Join(" · ", summary.CoreComponents.Take(8));
            }

            return "尚未形成可记录的核心组件";
        }

        private static string BuildUnlockText(DemoRunSummary summary)
        {
            if (summary.NewUnlocks == null || summary.NewUnlocks.Count == 0)
            {
                return "本世没有新增局外战力。下一世仍从一把飞剑与基础牌开始。";
            }

            return string.Join(" · ", summary.NewUnlocks.Select(unlock =>
                string.Equals(unlock, DemoMetaProgress.BrokenSwordTraceId, StringComparison.OrdinalIgnoreCase)
                    ? "残剑道痕：下一世首战所得可重铸一次"
                    : unlock));
        }

        private static string FormatDuration(float seconds)
        {
            int total = Mathf.Max(0, Mathf.RoundToInt(seconds));
            return $"{total / 60:00}:{total % 60:00}";
        }

        private void ConfigureCanvasScaler()
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                return;
            }

            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
            scaler.referencePixelsPerUnit = 100f;
        }

        private RectTransform CreatePanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Image image = gameObject.GetComponent<Image>();
            image.sprite = whiteSprite;
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private RectTransform CreateFixedPanel(
            Transform parent,
            string name,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size,
            Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            RectTransform rect = gameObject.GetComponent<RectTransform>();
            SetFixed(rect, anchor, pivot, position, size);
            Image image = gameObject.GetComponent<Image>();
            image.sprite = whiteSprite;
            image.color = color;
            image.raycastTarget = false;
            return rect;
        }

        private Text CreateText(
            Transform parent,
            string name,
            Font font,
            int size,
            FontStyle style,
            TextAnchor alignment,
            Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            gameObject.transform.SetParent(parent, false);
            Text text = gameObject.GetComponent<Text>();
            text.font = font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            text.raycastTarget = false;
            text.supportRichText = false;
            return text;
        }

        private Button CreateActionButton(
            Transform parent,
            string name,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size,
            string label,
            Color accent,
            bool interactable,
            UnityAction onClick)
        {
            RectTransform rect = CreateFixedPanel(
                parent, name, anchor, pivot, position, size,
                interactable ? Color.Lerp(Paper, accent, 0.16f) : new Color(0.85f, 0.83f, 0.76f, 0.82f));
            Image image = rect.GetComponent<Image>();
            image.raycastTarget = true;
            CreateBorder(rect, new Color(accent.r, accent.g, accent.b, interactable ? 0.90f : 0.30f), 2f);
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.interactable = interactable;
            ConfigureChoiceButton(button);
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            Text text = CreateText(rect, "Label", bodyFont, 19, FontStyle.Bold, TextAnchor.MiddleCenter, interactable ? Ink : FaintInk);
            text.text = label;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 14;
            text.resizeTextMaxSize = 19;
            Stretch(text.rectTransform, new Vector2(16f, 8f), new Vector2(-16f, -8f));
            return button;
        }

        private Button CreateSecondaryButton(
            Transform parent,
            string name,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size,
            string label,
            UnityAction onClick)
        {
            RectTransform rect = CreateFixedPanel(parent, name, anchor, pivot, position, size, new Color(Paper.r, Paper.g, Paper.b, 0.72f));
            Image image = rect.GetComponent<Image>();
            image.raycastTarget = true;
            CreateBorder(rect, new Color(Ink.r, Ink.g, Ink.b, 0.42f), 1.5f);
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            ConfigureChoiceButton(button);
            if (onClick != null)
            {
                button.onClick.AddListener(onClick);
            }

            Text text = CreateText(rect, "Label", bodyFont, 18, FontStyle.Bold, TextAnchor.MiddleCenter, Ink);
            text.text = label;
            Stretch(text.rectTransform, new Vector2(12f, 8f), new Vector2(-12f, -8f));
            return button;
        }

        private static void ConfigureChoiceButton(Button button)
        {
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.98f, 0.91f, 1f);
            colors.pressedColor = new Color(0.86f, 0.83f, 0.74f, 1f);
            colors.selectedColor = colors.highlightedColor;
            colors.disabledColor = new Color(0.78f, 0.77f, 0.72f, 0.66f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
        }

        private void CreateBorder(RectTransform parent, Color color, float thickness)
        {
            RectTransform top = CreatePanel(parent, "BorderTop", new Vector2(0f, 1f), Vector2.one, color);
            top.offsetMin = new Vector2(0f, -thickness);
            RectTransform bottom = CreatePanel(parent, "BorderBottom", Vector2.zero, new Vector2(1f, 0f), color);
            bottom.offsetMax = new Vector2(0f, thickness);
            RectTransform left = CreatePanel(parent, "BorderLeft", Vector2.zero, new Vector2(0f, 1f), color);
            left.offsetMax = new Vector2(thickness, 0f);
            RectTransform right = CreatePanel(parent, "BorderRight", new Vector2(1f, 0f), Vector2.one, color);
            right.offsetMin = new Vector2(-thickness, 0f);
        }

        private Sprite LoadSprite(string resourcePath)
        {
            if (loadedSprites.TryGetValue(resourcePath, out Sprite cached))
            {
                return cached;
            }

            Sprite imported = Resources.Load<Sprite>(resourcePath);
            if (imported != null)
            {
                loadedSprites[resourcePath] = imported;
                return imported;
            }

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                return null;
            }

            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            loadedSprites[resourcePath] = sprite;
            return sprite;
        }

        private static void ApplySprite(Image image, Sprite sprite, Color color, bool preserveAspect)
        {
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.color = sprite == null ? Color.clear : color;
            image.type = Image.Type.Simple;
            image.preserveAspect = preserveAspect;
            image.raycastTarget = false;
        }

        private static void SetFixed(
            RectTransform rect,
            Vector2 anchor,
            Vector2 pivot,
            Vector2 position,
            Vector2 size)
        {
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void SetAnchors(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                DestroyCreatedObject(parent.GetChild(i).gameObject);
            }
        }

        private static void DestroyCreatedObject(UnityEngine.Object value)
        {
            if (value == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(value);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(value);
            }
        }

        private static string Trim(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value ?? string.Empty;
            }

            return value.Substring(0, Mathf.Max(0, maxLength - 1)) + "…";
        }

        private static string Fallback(string value, string fallback)
        {
            return string.IsNullOrWhiteSpace(value) ? fallback : value;
        }

        private static Font ResolveFont(string[] candidates, Font fallback, int size)
        {
            try
            {
                Font font = Font.CreateDynamicFontFromOSFont(candidates, size);
                return font != null ? font : fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}
