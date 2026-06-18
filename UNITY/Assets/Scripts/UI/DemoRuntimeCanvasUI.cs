using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.UI
{
    public sealed class DemoRuntimeCanvasUI : MonoBehaviour
    {
        private static readonly Color ColorInkBlack = new Color(0.05f, 0.06f, 0.07f, 1f);
        private static readonly Color ColorInkPanel = new Color(0.09f, 0.10f, 0.12f, 0.96f);
        private static readonly Color ColorInkRaised = new Color(0.13f, 0.12f, 0.11f, 0.97f);
        private static readonly Color ColorGold = new Color(0.84f, 0.70f, 0.42f, 1f);
        private static readonly Color ColorGoldDim = new Color(0.54f, 0.43f, 0.24f, 0.82f);
        private static readonly Color ColorJade = new Color(0.37f, 0.66f, 0.74f, 1f);
        private static readonly Color ColorPaper = new Color(0.93f, 0.90f, 0.82f, 1f);
        private static readonly Color ColorMist = new Color(0.72f, 0.77f, 0.82f, 1f);
        private const string HeaderCloudBandResourcePath = "Art/UI/ui_header_cloudband_001";
        private const string PathChoiceWanjianResourcePath = "Art/UI/ui_path_wanjian_001";
        private const string PathChoiceWanjianFallbackResourcePath = "Art/UI/ui_path_wanjian_002";
        private const string PathChoiceThunderResourcePath = "Art/UI/ui_path_thunder_001";
        private const string PathChoiceThunderFallbackResourcePath = "Art/UI/ui_path_thunder_003";
        private const string PathChoiceBloodResourcePath = "Art/UI/ui_path_blood_001";

        private static Sprite cachedHeaderCloudBandSprite;
        private static Sprite cachedWanjianPathChoiceSprite;
        private static Sprite cachedThunderPathChoiceSprite;
        private static Sprite cachedBloodPathChoiceSprite;

        private readonly List<GameObject> handEntries = new List<GameObject>();
        private readonly List<GameObject> rewardEntries = new List<GameObject>();

        private DemoGameController controller;
        private Font uiFont;

        private Text headerTitleText;
        private Text headerSummaryText;
        private Text headerChipText;
        private Text headerContextText;
        private GameObject topHudRoot;

        private Text mapBodyText;
        private Text runBodyText;
        private Text logBodyText;

        private Text battleBodyText;
        private Text battleStateText;

        private Text handInfoText;
        private Text handEmptyText;

        private Text contextTitleText;
        private Text contextInfoText;
        private Text buildBodyText;
        private Text deckBodyText;

        private Button utilityButton;
        private Text utilityButtonText;
        private Button battleActionButton;
        private Text battleActionButtonText;
        private RectTransform utilityButtonRect;

        private RectTransform handContainer;
        private RectTransform rewardContainer;
        private HorizontalLayoutGroup rewardLayoutGroup;
        private DemoBattleSceneView battleSceneView;
        private GameObject rewardPanelBody;
        private GameObject buildPanelBody;
        private GameObject nodeOverlayRoot;
        private GameObject rewardOverlayRoot;
        private GameObject battleHudRoot;
        private GameObject nodeMapPanelRoot;
        private GameObject nodeBuildPanelRoot;
        private RectTransform nodeChoiceStage;
        private GameObject nodeStartStageRoot;
        private GameObject nodeGuidanceStageRoot;
        private Text nodeStageTitleText;
        private Text nodeStageBodyText;
        private Text nodeStageChecklistText;
        private Text nodeStartBaseText;
        private Text nodeStartInheritanceText;

        private Text nodeTitleText;
        private Text nodeBodyText;
        private Text nodeMapText;
        private Text nodeBuildText;

        private Text rewardTitleText;
        private Text rewardBodyText;
        private Text rewardBuildText;
        private Text rewardDeckText;
        private GameObject rewardDetailPanel;
        private Text rewardDetailTitleText;
        private Text rewardDetailBodyText;
        private Text rewardDetailHintText;
        private Text rewardSectionTitleText;
        private Text rewardSectionHintText;

        private Text battleHintText;

        private string handSignature = string.Empty;
        private string rewardSignature = string.Empty;

        private void Awake()
        {
            controller = GetComponent<DemoGameController>();
            uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            EnsureEventSystem();
            BuildCanvas();
        }

        private void Update()
        {
            if (controller == null)
            {
                return;
            }

            RefreshTextPanels();
            RefreshButtons();
            RefreshContextPanel();
            RefreshHand();
            RefreshRewards();
        }

        private void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            GameObject eventSystem = new GameObject("DEMO_EventSystem");
            eventSystem.transform.SetParent(transform, false);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private void BuildCanvas()
        {
            GameObject canvasObject = new GameObject("DEMO_RuntimeCanvas");
            canvasObject.transform.SetParent(transform, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<GraphicRaycaster>();

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
            canvasRect.anchorMin = Vector2.zero;
            canvasRect.anchorMax = Vector2.one;
            canvasRect.offsetMin = Vector2.zero;
            canvasRect.offsetMax = Vector2.zero;

            CreateBackdrop(canvasRect);
            BuildFullScreenBattleStage(canvasRect);
            BuildTopHud(canvasRect);
            BuildNodeOverlay(canvasRect);
            BuildRewardOverlay(canvasRect);
            BuildBattleHud(canvasRect);
        }

        private void CreateBackdrop(RectTransform parent)
        {
            CreateStretchPanel(parent, "Backdrop", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, ColorInkBlack);
            CreateStretchPanel(parent, "BackdropGlow", new Vector2(0f, 0.58f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero, new Color(0.07f, 0.12f, 0.16f, 0.90f));
            CreateStretchPanel(parent, "BackdropWarmMist", new Vector2(0f, 0f), new Vector2(1f, 0.32f), Vector2.zero, Vector2.zero, new Color(0.27f, 0.19f, 0.12f, 0.10f));
            CreateStretchPanel(parent, "BackdropInkVeil", new Vector2(0f, 0.18f), new Vector2(1f, 0.82f), Vector2.zero, Vector2.zero, new Color(0.03f, 0.03f, 0.04f, 0.28f));
        }

        private void BuildFullScreenBattleStage(RectTransform parent)
        {
            GameObject stageObject = new GameObject("BattleStageFullScreen", typeof(RectTransform), typeof(Image), typeof(DemoBattleSceneView));
            stageObject.transform.SetParent(parent, false);

            RectTransform stageRect = stageObject.GetComponent<RectTransform>();
            stageRect.anchorMin = Vector2.zero;
            stageRect.anchorMax = Vector2.one;
            stageRect.offsetMin = Vector2.zero;
            stageRect.offsetMax = Vector2.zero;

            Image stageFrame = stageObject.GetComponent<Image>();
            stageFrame.color = new Color(0.03f, 0.04f, 0.05f, 1f);
            stageFrame.raycastTarget = false;

            battleSceneView = stageObject.GetComponent<DemoBattleSceneView>();
            battleSceneView.Initialize(controller, uiFont);
        }

        private void BuildTopHud(RectTransform parent)
        {
            topHudRoot = new GameObject("TopHudRoot", typeof(RectTransform));
            topHudRoot.transform.SetParent(parent, false);
            RectTransform topHudRect = topHudRoot.GetComponent<RectTransform>();
            topHudRect.anchorMin = Vector2.zero;
            topHudRect.anchorMax = Vector2.one;
            topHudRect.offsetMin = Vector2.zero;
            topHudRect.offsetMax = Vector2.zero;

            Sprite headerCloudBand = LoadHeaderCloudBandSprite();

            RectTransform panel = CreateFixedPanel(
                topHudRoot.transform,
                "TopHudPanel",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -54f),
                new Vector2(1480f, 72f),
                new Color(0.08f, 0.09f, 0.11f, 0.90f));
            ApplySpriteToImage(panel, headerCloudBand, headerCloudBand != null ? Color.white : new Color(0.08f, 0.09f, 0.11f, 0.90f));
            DecorateFrame(panel, ColorGold, new Color(0.18f, 0.14f, 0.08f, 0.16f), 2f, true);

            CreateStretchPanel(
                panel,
                "PanelVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(0.03f, 0.03f, 0.04f, 0.34f));
            CreateStretchPanel(
                panel,
                "PanelGlow",
                new Vector2(0f, 0.58f),
                new Vector2(1f, 1f),
                new Vector2(18f, 2f),
                new Vector2(-18f, -8f),
                new Color(0.88f, 0.76f, 0.48f, 0.08f));

            headerTitleText = CreateText(panel, "HeaderTitle", "万道归途", 28, FontStyle.Bold, TextAnchor.MiddleLeft, ColorPaper);
            headerTitleText.rectTransform.anchorMin = new Vector2(0f, 0f);
            headerTitleText.rectTransform.anchorMax = new Vector2(0.30f, 1f);
            headerTitleText.rectTransform.offsetMin = new Vector2(34f, 10f);
            headerTitleText.rectTransform.offsetMax = new Vector2(-12f, -10f);

            headerSummaryText = CreateText(panel, "HeaderSummary", string.Empty, 13, FontStyle.Normal, TextAnchor.MiddleRight, ColorMist);
            headerSummaryText.rectTransform.anchorMin = new Vector2(0.28f, 0f);
            headerSummaryText.rectTransform.anchorMax = new Vector2(1f, 1f);
            headerSummaryText.rectTransform.offsetMin = new Vector2(16f, 10f);
            headerSummaryText.rectTransform.offsetMax = new Vector2(-178f, -10f);

            RectTransform chip = CreateFixedPanel(
                panel,
                "HeaderChip",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-28f, 0f),
                new Vector2(136f, 36f),
                new Color(0.16f, 0.14f, 0.10f, 0.96f));
            DecorateFrame(chip, ColorGold, new Color(0.28f, 0.22f, 0.12f, 0.55f), 2f, true);

            headerChipText = CreateText(chip, "ChipText", string.Empty, 14, FontStyle.Bold, TextAnchor.MiddleCenter, ColorGold);
            StretchText(headerChipText.rectTransform, new Vector2(10f, 4f), new Vector2(-10f, -4f));

            RectTransform contextBand = CreateFixedPanel(
                topHudRoot.transform,
                "TopHudContextBand",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -102f),
                new Vector2(1040f, 32f),
                new Color(0.10f, 0.10f, 0.11f, 0.90f));
            ApplySpriteToImage(contextBand, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.92f) : new Color(0.10f, 0.10f, 0.11f, 0.90f));
            DecorateFrame(contextBand, new Color(0.60f, 0.49f, 0.26f, 0.86f), new Color(0.20f, 0.16f, 0.10f, 0.12f), 1.5f, false);
            CreateStretchPanel(
                contextBand,
                "ContextVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(8f, 4f),
                new Vector2(-8f, -4f),
                new Color(0.04f, 0.04f, 0.05f, 0.42f));

            headerContextText = CreateText(contextBand, "HeaderContextText", string.Empty, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.79f, 0.45f, 1f));
            StretchText(headerContextText.rectTransform, new Vector2(18f, 4f), new Vector2(-18f, -4f));
        }

        private void BuildNodeOverlay(RectTransform parent)
        {
            nodeOverlayRoot = new GameObject("NodeOverlayRoot", typeof(RectTransform), typeof(Image));
            nodeOverlayRoot.transform.SetParent(parent, false);

            RectTransform overlayRect = nodeOverlayRoot.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayImage = nodeOverlayRoot.GetComponent<Image>();
            overlayImage.color = new Color(0.04f, 0.05f, 0.06f, 0.24f);

            Sprite headerCloudBand = LoadHeaderCloudBandSprite();

            RectTransform panel = CreateFixedPanel(
                nodeOverlayRoot.transform,
                "NodeOverlayPanel",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1420f, 812f),
                new Color(0.07f, 0.08f, 0.09f, 0.52f));
            DecorateFrame(panel, new Color(0.62f, 0.50f, 0.29f, 0.86f), new Color(0.03f, 0.03f, 0.04f, 0.10f), 2f, true);
            if (headerCloudBand != null)
            {
                RectTransform cloudVeil = CreateFixedPanel(
                    panel,
                    "PanelCloudVeil",
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.zero,
                    new Vector2(1260f, 220f),
                    Color.white);
                ApplySpriteToImage(cloudVeil, headerCloudBand, new Color(1f, 1f, 1f, 0.16f));
            }

            CreateStretchPanel(
                panel,
                "PanelInkWash",
                new Vector2(0f, 0.42f),
                new Vector2(1f, 1f),
                new Vector2(20f, 20f),
                new Vector2(-20f, -20f),
                new Color(0.12f, 0.14f, 0.16f, 0.10f));

            nodeStartStageRoot = new GameObject("NodeStartStageRoot", typeof(RectTransform));
            nodeStartStageRoot.transform.SetParent(panel, false);
            StretchText(nodeStartStageRoot.GetComponent<RectTransform>(), new Vector2(28f, 28f), new Vector2(-28f, -28f));
            BuildNodeStartStage(nodeStartStageRoot.transform, headerCloudBand);

            nodeTitleText = CreateText(panel, "NodeTitle", string.Empty, 34, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            nodeTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nodeTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeTitleText.rectTransform.offsetMin = new Vector2(64f, -96f);
            nodeTitleText.rectTransform.offsetMax = new Vector2(-64f, -30f);
            nodeTitleText.color = ColorPaper;

            nodeBodyText = CreateText(panel, "NodeBody", string.Empty, 17, FontStyle.Normal, TextAnchor.UpperCenter, ColorMist);
            nodeBodyText.rectTransform.anchorMin = new Vector2(0.18f, 1f);
            nodeBodyText.rectTransform.anchorMax = new Vector2(0.82f, 1f);
            nodeBodyText.rectTransform.offsetMin = new Vector2(0f, -164f);
            nodeBodyText.rectTransform.offsetMax = new Vector2(0f, -98f);

            nodeChoiceStage = CreateInkPanel(
                panel,
                "NodeChoiceStage",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(78f, 190f),
                new Vector2(-78f, -182f),
                new Color(1f, 1f, 1f, 0.03f));
            DecorateFrame(nodeChoiceStage, new Color(0.44f, 0.36f, 0.22f, 0.52f), new Color(0f, 0f, 0f, 0f), 1.5f, false);
            BuildNodeChoiceStage(nodeChoiceStage);

            RectTransform mapPanel = CreateInkPanel(
                panel,
                "NodeMapPanel",
                new Vector2(0f, 0f),
                new Vector2(0.40f, 0f),
                new Vector2(44f, 26f),
                new Vector2(-28f, 132f),
                new Color(0.11f, 0.12f, 0.14f, 0.82f));
            ApplySpriteToImage(mapPanel, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.26f) : new Color(0.11f, 0.12f, 0.14f, 0.82f));
            CreateStretchPanel(
                mapPanel,
                "MapPanelVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.05f, 0.05f, 0.06f, 0.54f));
            CreatePinnedPanelTitle(mapPanel, "道途进程");
            nodeMapText = CreateBodyText(mapPanel, "NodeMapText", 14, ColorMist);
            StretchText(nodeMapText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));
            nodeMapPanelRoot = mapPanel.gameObject;

            RectTransform buildPanel = CreateInkPanel(
                panel,
                "NodeBuildPanel",
                new Vector2(0.60f, 0f),
                new Vector2(1f, 0f),
                new Vector2(28f, 26f),
                new Vector2(-44f, 132f),
                new Color(0.11f, 0.12f, 0.14f, 0.82f));
            ApplySpriteToImage(buildPanel, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.26f) : new Color(0.11f, 0.12f, 0.14f, 0.82f));
            CreateStretchPanel(
                buildPanel,
                "BuildPanelVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.05f, 0.05f, 0.06f, 0.54f));
            CreatePinnedPanelTitle(buildPanel, "当前道基");
            nodeBuildText = CreateBodyText(buildPanel, "NodeBuildText", 14, ColorMist);
            StretchText(nodeBuildText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));
            nodeBuildPanelRoot = buildPanel.gameObject;

            utilityButton = CreateActionButton(panel, "NodeActionButton", out utilityButtonText, new Color(0.23f, 0.18f, 0.11f, 0.98f), ColorPaper);
            utilityButtonRect = utilityButton.GetComponent<RectTransform>();
            utilityButtonRect.anchorMin = new Vector2(0.5f, 0f);
            utilityButtonRect.anchorMax = new Vector2(0.5f, 0f);
            utilityButtonRect.pivot = new Vector2(0.5f, 0f);
            utilityButtonRect.anchoredPosition = new Vector2(0f, 72f);
            utilityButtonRect.sizeDelta = new Vector2(312f, 74f);
            ApplySpriteToImage(utilityButtonRect, headerCloudBand, headerCloudBand != null ? Color.white : new Color(0.23f, 0.18f, 0.11f, 0.98f));
            RectTransform buttonCoreVeil = CreateStretchPanel(
                utilityButtonRect,
                "ButtonCoreVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(12f, 8f),
                new Vector2(-12f, -8f),
                new Color(0.06f, 0.06f, 0.07f, 0.34f));
            buttonCoreVeil.GetComponent<Image>().raycastTarget = false;
            utilityButtonText.transform.SetAsLastSibling();
            utilityButtonText.fontSize = 18;
            utilityButton.onClick.AddListener(OnUtilityButtonClicked);
        }

        private void BuildNodeStartStage(Transform parent, Sprite headerCloudBand)
        {
            CreateStretchPanel(
                parent,
                "StartTopMist",
                new Vector2(0f, 0.64f),
                new Vector2(1f, 1f),
                Vector2.zero,
                Vector2.zero,
                new Color(1f, 1f, 1f, 0.02f));
            CreateStretchPanel(
                parent,
                "StartBottomMist",
                new Vector2(0f, 0f),
                new Vector2(1f, 0.28f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.26f, 0.20f, 0.12f, 0.08f));

            RectTransform backdropVeil = CreateStretchPanel(
                parent,
                "StartBackdropVeil",
                new Vector2(0.05f, 0.10f),
                new Vector2(0.95f, 0.88f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.05f, 0.06f, 0.07f, 0.36f));
            DecorateFrame(backdropVeil, new Color(0.42f, 0.34f, 0.20f, 0.28f), new Color(0f, 0f, 0f, 0f), 1f, false);

            RectTransform heroPlate = CreateFixedPanel(
                parent,
                "StartHeroPlate",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -8f),
                new Vector2(1020f, 188f),
                new Color(0.10f, 0.10f, 0.11f, 0.96f));
            ApplySpriteToImage(heroPlate, headerCloudBand, headerCloudBand != null ? Color.white : new Color(0.10f, 0.10f, 0.11f, 0.96f));
            DecorateFrame(heroPlate, ColorGold, new Color(0.10f, 0.09f, 0.08f, 0.22f), 2f, true);
            CreateStretchPanel(
                heroPlate,
                "HeroPlateVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(12f, 12f),
                new Vector2(-12f, -12f),
                new Color(0.05f, 0.05f, 0.06f, 0.26f));

            Text heroBadgeText = CreateText(heroPlate, "HeroBadgeText", "DEMO 首轮轮回", 12, FontStyle.Bold, TextAnchor.MiddleCenter, ColorGold);
            heroBadgeText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            heroBadgeText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            heroBadgeText.rectTransform.pivot = new Vector2(0.5f, 1f);
            heroBadgeText.rectTransform.anchoredPosition = new Vector2(0f, -22f);
            heroBadgeText.rectTransform.sizeDelta = new Vector2(220f, 24f);

            Text heroTitleText = CreateText(heroPlate, "HeroTitleText", "万道归途", 62, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            heroTitleText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            heroTitleText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            heroTitleText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            heroTitleText.rectTransform.anchoredPosition = new Vector2(0f, 8f);
            heroTitleText.rectTransform.sizeDelta = new Vector2(680f, 78f);

            Text heroSubtitleText = CreateText(heroPlate, "HeroSubtitleText", "修仙不是打怪升级，而是构筑一条属于自己的道途", 20, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.85f, 0.85f, 0.81f, 0.92f));
            heroSubtitleText.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            heroSubtitleText.rectTransform.anchorMax = new Vector2(0.5f, 0f);
            heroSubtitleText.rectTransform.pivot = new Vector2(0.5f, 0f);
            heroSubtitleText.rectTransform.anchoredPosition = new Vector2(0f, 24f);
            heroSubtitleText.rectTransform.sizeDelta = new Vector2(820f, 36f);

            Text centerGlyphText = CreateText(parent, "StartCenterGlyph", "道", 110, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.86f, 0.72f, 0.44f, 0.12f));
            centerGlyphText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            centerGlyphText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            centerGlyphText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            centerGlyphText.rectTransform.anchoredPosition = new Vector2(0f, 8f);
            centerGlyphText.rectTransform.sizeDelta = new Vector2(128f, 128f);

            RectTransform centerSeal = CreateFixedPanel(
                parent,
                "StartCenterSeal",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -18f),
                new Vector2(228f, 118f),
                new Color(0.09f, 0.09f, 0.10f, 0.92f));
            ApplySpriteToImage(centerSeal, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.78f) : new Color(0.09f, 0.09f, 0.10f, 0.92f));
            DecorateFrame(centerSeal, new Color(0.62f, 0.49f, 0.27f, 0.72f), new Color(0f, 0f, 0f, 0f), 1.5f, false);
            CreateStretchPanel(
                centerSeal,
                "CenterSealVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(0.05f, 0.05f, 0.06f, 0.38f));

            Text sealTitleText = CreateText(centerSeal, "SealTitleText", "今朝起势", 24, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            sealTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            sealTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            sealTitleText.rectTransform.offsetMin = new Vector2(12f, -42f);
            sealTitleText.rectTransform.offsetMax = new Vector2(-12f, -10f);

            Text sealBodyText = CreateText(centerSeal, "SealBodyText", "云海静候\n道途未定", 15, FontStyle.Normal, TextAnchor.MiddleCenter, ColorMist);
            sealBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            sealBodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            sealBodyText.rectTransform.offsetMin = new Vector2(16f, 18f);
            sealBodyText.rectTransform.offsetMax = new Vector2(-16f, -42f);

            nodeStartBaseText = CreateStartStageScroll(
                parent,
                "StartBaseScroll",
                new Vector2(-318f, -46f),
                "基础属性",
                "道基卷",
                ColorGold,
                headerCloudBand);
            nodeStartInheritanceText = CreateStartStageScroll(
                parent,
                "StartInheritanceScroll",
                new Vector2(318f, -46f),
                "携入遗物",
                "传承卷",
                ColorJade,
                headerCloudBand);
        }

        private Text CreateStartStageScroll(Transform parent, string name, Vector2 anchoredPosition, string title, string mark, Color accent, Sprite headerCloudBand)
        {
            GameObject rootObject = new GameObject(name, typeof(RectTransform));
            rootObject.transform.SetParent(parent, false);

            RectTransform rootRect = rootObject.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = anchoredPosition;
            rootRect.sizeDelta = new Vector2(336f, 540f);

            RectTransform body = CreateFixedPanel(
                rootObject.transform,
                "ScrollBody",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(272f, 496f),
                new Color(0.10f, 0.10f, 0.11f, 0.95f));
            DecorateFrame(body, new Color(accent.r, accent.g, accent.b, 0.62f), new Color(0.16f, 0.13f, 0.09f, 0.12f), 1.5f, true);

            if (headerCloudBand != null)
            {
                RectTransform cloudBand = CreateFixedPanel(
                    body,
                    "ScrollCloudBand",
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.zero,
                    new Vector2(440f, 220f),
                    Color.white);
                ApplySpriteToImage(cloudBand, headerCloudBand, new Color(1f, 1f, 1f, 0.14f));
                cloudBand.localEulerAngles = new Vector3(0f, 0f, 90f);
            }

            CreateStretchPanel(
                body,
                "ScrollInkVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(10f, 12f),
                new Vector2(-10f, -12f),
                new Color(0.04f, 0.04f, 0.05f, 0.34f));

            RectTransform topRod = CreateFixedPanel(
                rootObject.transform,
                "TopRod",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, 2f),
                new Vector2(286f, 18f),
                new Color(0.22f, 0.17f, 0.10f, 0.98f));
            DecorateFrame(topRod, new Color(accent.r, accent.g, accent.b, 0.56f), new Color(0f, 0f, 0f, 0f), 1f, false);

            RectTransform bottomRod = CreateFixedPanel(
                rootObject.transform,
                "BottomRod",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, -2f),
                new Vector2(286f, 18f),
                new Color(0.22f, 0.17f, 0.10f, 0.98f));
            DecorateFrame(bottomRod, new Color(accent.r, accent.g, accent.b, 0.56f), new Color(0f, 0f, 0f, 0f), 1f, false);

            RectTransform titleBand = CreateFixedPanel(
                body,
                "TitleBand",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -42f),
                new Vector2(184f, 58f),
                new Color(0.10f, 0.10f, 0.11f, 0.94f));
            DecorateFrame(titleBand, new Color(accent.r, accent.g, accent.b, 0.70f), new Color(0f, 0f, 0f, 0f), 1.5f, false);

            Text titleText = CreateText(titleBand, "TitleText", title, 20, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            titleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            titleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            titleText.rectTransform.offsetMin = new Vector2(10f, -32f);
            titleText.rectTransform.offsetMax = new Vector2(-10f, -8f);

            Text markText = CreateText(titleBand, "MarkText", mark, 12, FontStyle.Bold, TextAnchor.LowerCenter, accent);
            markText.rectTransform.anchorMin = new Vector2(0f, 0f);
            markText.rectTransform.anchorMax = new Vector2(1f, 0f);
            markText.rectTransform.offsetMin = new Vector2(8f, 8f);
            markText.rectTransform.offsetMax = new Vector2(-8f, 26f);

            Text bodyText = CreateText(body, "BodyText", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.90f, 0.90f, 0.87f, 0.96f));
            bodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            bodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            bodyText.rectTransform.offsetMin = new Vector2(28f, 38f);
            bodyText.rectTransform.offsetMax = new Vector2(-28f, -92f);

            Text footerText = CreateText(body, "FooterText", "卷藏今朝起势", 12, FontStyle.Normal, TextAnchor.LowerCenter, new Color(accent.r, accent.g, accent.b, 0.82f));
            footerText.rectTransform.anchorMin = new Vector2(0f, 0f);
            footerText.rectTransform.anchorMax = new Vector2(1f, 0f);
            footerText.rectTransform.offsetMin = new Vector2(16f, 14f);
            footerText.rectTransform.offsetMax = new Vector2(-16f, 32f);

            return bodyText;
        }

        private void BuildRewardOverlay(RectTransform parent)
        {
            rewardOverlayRoot = new GameObject("RewardOverlayRoot", typeof(RectTransform), typeof(Image));
            rewardOverlayRoot.transform.SetParent(parent, false);

            RectTransform overlayRect = rewardOverlayRoot.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayImage = rewardOverlayRoot.GetComponent<Image>();
            overlayImage.color = new Color(0.04f, 0.05f, 0.06f, 0.24f);

            rewardTitleText = CreateText(rewardOverlayRoot.transform, "RewardTitle", string.Empty, 34, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            rewardTitleText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.anchoredPosition = new Vector2(0f, -118f);
            rewardTitleText.rectTransform.sizeDelta = new Vector2(920f, 44f);
            rewardTitleText.color = new Color(0.95f, 0.94f, 0.90f, 1f);

            rewardBodyText = CreateText(rewardOverlayRoot.transform, "RewardBody", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperCenter, ColorMist);
            rewardBodyText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.anchoredPosition = new Vector2(0f, -172f);
            rewardBodyText.rectTransform.sizeDelta = new Vector2(1080f, 48f);

            rewardSectionTitleText = CreateText(rewardOverlayRoot.transform, "RewardSectionTitle", string.Empty, 18, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.95f, 0.84f, 0.52f, 1f));
            rewardSectionTitleText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.anchoredPosition = new Vector2(0f, -228f);
            rewardSectionTitleText.rectTransform.sizeDelta = new Vector2(760f, 28f);

            rewardSectionHintText = CreateText(rewardOverlayRoot.transform, "RewardSectionHint", string.Empty, 12, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.74f, 0.78f, 0.82f, 0.92f));
            rewardSectionHintText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.anchoredPosition = new Vector2(0f, -256f);
            rewardSectionHintText.rectTransform.sizeDelta = new Vector2(900f, 24f);

            GameObject rewardsRowObject = new GameObject("RewardChoices", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            rewardsRowObject.transform.SetParent(rewardOverlayRoot.transform, false);
            rewardContainer = rewardsRowObject.GetComponent<RectTransform>();
            rewardContainer.anchorMin = new Vector2(0.5f, 0.5f);
            rewardContainer.anchorMax = new Vector2(0.5f, 0.5f);
            rewardContainer.pivot = new Vector2(0.5f, 0.5f);
            rewardContainer.anchoredPosition = new Vector2(0f, 24f);
            rewardContainer.sizeDelta = new Vector2(1420f, 560f);

            rewardLayoutGroup = rewardsRowObject.GetComponent<HorizontalLayoutGroup>();
            rewardLayoutGroup.spacing = 30f;
            rewardLayoutGroup.padding = new RectOffset(20, 20, 8, 8);
            rewardLayoutGroup.childAlignment = TextAnchor.MiddleCenter;
            rewardLayoutGroup.childControlHeight = true;
            rewardLayoutGroup.childControlWidth = true;
            rewardLayoutGroup.childForceExpandHeight = false;
            rewardLayoutGroup.childForceExpandWidth = false;

            rewardDetailPanel = CreateFixedPanel(
                rewardOverlayRoot.transform,
                "RewardDetailPanel",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 246f),
                new Vector2(720f, 176f),
                new Color(0.08f, 0.09f, 0.11f, 0.97f)).gameObject;
            DecorateFrame(rewardDetailPanel.GetComponent<RectTransform>(), ColorGold, new Color(0.20f, 0.15f, 0.09f, 0.14f), 2f, true);
            rewardDetailPanel.SetActive(false);

            rewardDetailTitleText = CreateText(rewardDetailPanel.transform, "RewardDetailTitle", string.Empty, 18, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            rewardDetailTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            rewardDetailTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rewardDetailTitleText.rectTransform.offsetMin = new Vector2(20f, -34f);
            rewardDetailTitleText.rectTransform.offsetMax = new Vector2(-20f, -10f);

            rewardDetailBodyText = CreateText(rewardDetailPanel.transform, "RewardDetailBody", string.Empty, 14, FontStyle.Normal, TextAnchor.UpperLeft, ColorMist);
            rewardDetailBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            rewardDetailBodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rewardDetailBodyText.rectTransform.offsetMin = new Vector2(20f, 24f);
            rewardDetailBodyText.rectTransform.offsetMax = new Vector2(-20f, -42f);

            rewardDetailHintText = CreateText(rewardDetailPanel.transform, "RewardDetailHint", "移开鼠标即可收起", 12, FontStyle.Normal, TextAnchor.LowerRight, ColorGoldDim);
            rewardDetailHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
            rewardDetailHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            rewardDetailHintText.rectTransform.offsetMin = new Vector2(20f, 8f);
            rewardDetailHintText.rectTransform.offsetMax = new Vector2(-20f, 28f);

            Sprite headerCloudBand = LoadHeaderCloudBandSprite();

            RectTransform rewardInfoPanel = CreateInkPanel(
                rewardOverlayRoot.transform,
                "RewardInfoPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-700f, 18f),
                new Vector2(-20f, 186f),
                new Color(0.08f, 0.09f, 0.11f, 0.92f));
            ApplySpriteToImage(rewardInfoPanel, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.28f) : new Color(0.08f, 0.09f, 0.11f, 0.92f));
            CreateStretchPanel(
                rewardInfoPanel,
                "RewardInfoVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.04f, 0.04f, 0.05f, 0.56f));
            CreatePinnedPanelTitle(rewardInfoPanel, "当前道基");
            rewardBuildText = CreateBodyText(rewardInfoPanel, "RewardBuildText", 14, ColorMist);
            StretchText(rewardBuildText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));

            RectTransform rewardDeckPanel = CreateInkPanel(
                rewardOverlayRoot.transform,
                "RewardDeckPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(20f, 18f),
                new Vector2(700f, 186f),
                new Color(0.08f, 0.09f, 0.11f, 0.92f));
            ApplySpriteToImage(rewardDeckPanel, headerCloudBand, headerCloudBand != null ? new Color(1f, 1f, 1f, 0.28f) : new Color(0.08f, 0.09f, 0.11f, 0.92f));
            CreateStretchPanel(
                rewardDeckPanel,
                "RewardDeckVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.04f, 0.04f, 0.05f, 0.56f));
            CreatePinnedPanelTitle(rewardDeckPanel, "当前牌组方向");
            rewardDeckText = CreateBodyText(rewardDeckPanel, "RewardDeckText", 14, ColorMist);
            StretchText(rewardDeckText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));
        }

        private void BuildBattleHud(RectTransform parent)
        {
            battleHudRoot = new GameObject("BattleHudRoot", typeof(RectTransform));
            battleHudRoot.transform.SetParent(parent, false);

            RectTransform hudRect = battleHudRoot.GetComponent<RectTransform>();
            hudRect.anchorMin = Vector2.zero;
            hudRect.anchorMax = Vector2.one;
            hudRect.offsetMin = Vector2.zero;
            hudRect.offsetMax = Vector2.zero;

            RectTransform hintPanel = CreateInkPanel(
                battleHudRoot.transform,
                "BattleHintPanel",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-430f, 248f),
                new Vector2(-20f, 348f),
                new Color(0.08f, 0.09f, 0.11f, 0.88f));
            battleStateText = CreateText(hintPanel, "BattleStateText", string.Empty, 16, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            battleStateText.rectTransform.anchorMin = new Vector2(0f, 0.5f);
            battleStateText.rectTransform.anchorMax = new Vector2(1f, 1f);
            battleStateText.rectTransform.offsetMin = new Vector2(18f, -10f);
            battleStateText.rectTransform.offsetMax = new Vector2(-18f, -10f);

            battleHintText = CreateText(hintPanel, "BattleHintText", string.Empty, 13, FontStyle.Normal, TextAnchor.UpperLeft, ColorMist);
            battleHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
            battleHintText.rectTransform.anchorMax = new Vector2(1f, 0.56f);
            battleHintText.rectTransform.offsetMin = new Vector2(18f, 14f);
            battleHintText.rectTransform.offsetMax = new Vector2(-18f, -10f);

            RectTransform handPanel = CreateFixedPanel(
                battleHudRoot.transform,
                "HandHudPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 24f),
                new Vector2(1300f, 230f),
                new Color(0.07f, 0.08f, 0.10f, 0.92f));
            DecorateFrame(handPanel, ColorGoldDim, new Color(0.18f, 0.14f, 0.09f, 0.16f), 2f, true);

            Text handTitleText = CreateText(handPanel, "HandTitle", "手牌规划", 20, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            handTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            handTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            handTitleText.rectTransform.offsetMin = new Vector2(22f, -38f);
            handTitleText.rectTransform.offsetMax = new Vector2(-22f, -8f);

            handInfoText = CreateText(handPanel, "HandInfo", string.Empty, 15, FontStyle.Normal, TextAnchor.UpperLeft, ColorMist);
            handInfoText.rectTransform.anchorMin = new Vector2(0f, 1f);
            handInfoText.rectTransform.anchorMax = new Vector2(1f, 1f);
            handInfoText.rectTransform.offsetMin = new Vector2(22f, -64f);
            handInfoText.rectTransform.offsetMax = new Vector2(-22f, -34f);

            GameObject containerObject = new GameObject("HandContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            containerObject.transform.SetParent(handPanel, false);
            handContainer = containerObject.GetComponent<RectTransform>();
            handContainer.anchorMin = new Vector2(0f, 0f);
            handContainer.anchorMax = new Vector2(1f, 1f);
            handContainer.offsetMin = new Vector2(18f, 52f);
            handContainer.offsetMax = new Vector2(-182f, -72f);

            HorizontalLayoutGroup group = containerObject.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 12f;
            group.padding = new RectOffset(0, 0, 4, 4);
            group.childAlignment = TextAnchor.UpperCenter;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            handEmptyText = CreateText(handPanel, "HandEmpty", string.Empty, 14, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.66f, 0.70f, 0.74f, 1f));
            handEmptyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            handEmptyText.rectTransform.anchorMax = new Vector2(1f, 0f);
            handEmptyText.rectTransform.offsetMin = new Vector2(22f, 18f);
            handEmptyText.rectTransform.offsetMax = new Vector2(-320f, 44f);

            battleActionButton = CreateActionButton(handPanel, "BattleActionButton", out battleActionButtonText, new Color(0.24f, 0.18f, 0.10f, 0.98f), ColorPaper);
            RectTransform actionRect = battleActionButton.GetComponent<RectTransform>();
            actionRect.anchorMin = new Vector2(1f, 0f);
            actionRect.anchorMax = new Vector2(1f, 0f);
            actionRect.pivot = new Vector2(1f, 0f);
            actionRect.anchoredPosition = new Vector2(-22f, 18f);
            actionRect.sizeDelta = new Vector2(260f, 52f);
            battleActionButton.onClick.AddListener(OnBattleActionClicked);
        }

        private Text CreatePinnedPanelTitle(Transform parent, string title)
        {
            Text text = CreateText(parent, "PinnedTitle", title, 18, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            text.rectTransform.anchorMin = new Vector2(0f, 1f);
            text.rectTransform.anchorMax = new Vector2(1f, 1f);
            text.rectTransform.offsetMin = new Vector2(18f, -30f);
            text.rectTransform.offsetMax = new Vector2(-18f, -8f);
            return text;
        }

        private void RefreshTextPanels()
        {
            headerTitleText.text = controller.HasBattle
                ? "云海斗法"
                : IsStartHomeScreen()
                    ? "起手总览"
                    : "万道归途";
            headerSummaryText.text = BuildTopBarSummary();
            headerChipText.text = GetHeaderChipText();
            headerContextText.text = BuildHeaderContextText();

            nodeTitleText.text = controller.Run.Map.CurrentNode.Name;
            nodeBodyText.text = BuildNodeOverlaySummary();
            nodeMapText.text = controller.GetMapSummary();
            nodeBuildText.text = BuildRunSummary();
            if (nodeStartBaseText != null)
            {
                nodeStartBaseText.text = BuildStartBaseScrollText();
            }

            if (nodeStartInheritanceText != null)
            {
                nodeStartInheritanceText.text = BuildStartInheritanceScrollText();
            }

            RefreshNodeChoiceStage();

            rewardTitleText.text = controller.Run.Map.CurrentNode.Name;
            rewardBodyText.text = BuildRewardLeadText();
            rewardBuildText.text = BuildRunSnapshot();
            rewardDeckText.text = BuildDeckFocusSummary();
            RefreshRewardLayout();
            rewardSectionTitleText.text = IsStartPathChoiceScreen()
                ? "立住这局的道途"
                : IsRouteChoiceScreen()
                    ? "挑出下一段路"
                    : "从三项补强里只拿最关键的一项";
            rewardSectionHintText.text = IsStartPathChoiceScreen()
                ? "三张大牌直接决定起手流向与后续掉落倾向"
                : IsRouteChoiceScreen()
                    ? "节点顺序就是这一局的第二层决策"
                    : "信息下沉到底部摘要，主区只负责做选择";

            handInfoText.text = controller.GetHandStatus();
            handEmptyText.text = BuildHandHint();

            if (controller.IsRunComplete)
            {
                battleStateText.text = "天劫已渡，道途已成。";
                battleStateText.color = new Color(0.95f, 0.84f, 0.45f, 1f);
                battleHintText.text = "这一局已经收束完成，可以直接回看构筑路径。";
            }
            else if (!controller.HasBattle)
            {
                battleStateText.text = controller.Run.Map.CurrentNode.Type == DemoNodeType.RouteChoice
                    ? "前方三路未定，先选下一段历练。"
                    : "主舞台正在切往下一个阶段。";
                battleStateText.color = new Color(0.74f, 0.80f, 0.87f, 1f);
                battleHintText.text = BuildNodeOverlaySummary();
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Lost)
            {
                battleStateText.text = "这一轮败在节奏与收束。";
                battleStateText.color = new Color(0.92f, 0.42f, 0.38f, 1f);
                battleHintText.text = "重新开局时更早确定主修和爆发窗口。";
            }
            else
            {
                battleStateText.text = controller.Battle.IsBossBattle ? "Boss 已压上中天，准备收束全局。" : "演武主场开启，手牌只负责布势。";
                battleStateText.color = new Color(0.74f, 0.80f, 0.87f, 1f);
                battleHintText.text = BuildBattleHint();
            }
        }

        private void RefreshButtons()
        {
            bool showingRewards = controller.CurrentRewards.Count > 0;
            bool showingBattle = controller.HasBattle;
            bool showStartHome = IsStartHomeScreen();
            bool hideTopHud = IsStartPresentationScreen();

            if (nodeOverlayRoot != null)
            {
                nodeOverlayRoot.SetActive(!showingBattle && !showingRewards);
            }

            if (rewardOverlayRoot != null)
            {
                rewardOverlayRoot.SetActive(showingRewards);
            }

            if (battleHudRoot != null)
            {
                battleHudRoot.SetActive(showingBattle);
            }

            if (topHudRoot != null)
            {
                topHudRoot.SetActive(!hideTopHud);
            }

            utilityButton.gameObject.SetActive(!showingBattle && !showingRewards && controller.CanAdvanceUtilityNode);
            utilityButtonText.text = showStartHome ? "踏入此局" : controller.UtilityActionLabel;
            utilityButtonText.fontSize = showStartHome ? 19 : 17;

            if (utilityButtonRect != null)
            {
                utilityButtonRect.anchoredPosition = showStartHome
                    ? new Vector2(0f, 44f)
                    : new Vector2(0f, 72f);
                utilityButtonRect.sizeDelta = showStartHome
                    ? new Vector2(336f, 82f)
                    : new Vector2(292f, 68f);
            }

            string battleLabel = controller.BattleActionLabel;
            bool showBattleAction = showingBattle && !string.IsNullOrEmpty(battleLabel);
            battleActionButton.gameObject.SetActive(showBattleAction);
            battleActionButtonText.text = battleLabel;
        }

        private void RefreshRewardLayout()
        {
            if (rewardContainer == null || rewardLayoutGroup == null)
            {
                return;
            }

            if (IsStartPathChoiceScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, 56f);
                rewardContainer.sizeDelta = new Vector2(1400f, 548f);
                rewardLayoutGroup.spacing = 24f;
                rewardLayoutGroup.padding = new RectOffset(16, 16, 4, 4);
                return;
            }

            if (IsRouteChoiceScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, 30f);
                rewardContainer.sizeDelta = new Vector2(1420f, 560f);
                rewardLayoutGroup.spacing = 28f;
                rewardLayoutGroup.padding = new RectOffset(20, 20, 8, 8);
                return;
            }

            rewardContainer.anchoredPosition = new Vector2(0f, 8f);
            rewardContainer.sizeDelta = new Vector2(1460f, 592f);
            rewardLayoutGroup.spacing = 36f;
            rewardLayoutGroup.padding = new RectOffset(24, 24, 12, 12);
        }

        private void RefreshNodeChoiceStage()
        {
            if (controller == null || nodeChoiceStage == null)
            {
                return;
            }

            bool showStartHome = IsStartHomeScreen();

            if (nodeStartStageRoot != null)
            {
                nodeStartStageRoot.SetActive(showStartHome);
            }

            if (nodeGuidanceStageRoot != null)
            {
                nodeGuidanceStageRoot.SetActive(!showStartHome);
            }

            if (nodeChoiceStage != null)
            {
                nodeChoiceStage.gameObject.SetActive(!showStartHome);
            }

            if (nodeTitleText != null)
            {
                nodeTitleText.gameObject.SetActive(!showStartHome);
            }

            if (nodeBodyText != null)
            {
                nodeBodyText.gameObject.SetActive(!showStartHome);
            }

            if (nodeMapPanelRoot != null)
            {
                nodeMapPanelRoot.SetActive(!showStartHome);
            }

            if (nodeBuildPanelRoot != null)
            {
                nodeBuildPanelRoot.SetActive(!showStartHome);
            }

            if (!showStartHome)
            {
                nodeStageTitleText.text = BuildNodeStageTitle();
                nodeStageBodyText.text = BuildNodeStageBody();
                nodeStageChecklistText.text = BuildNodeStageChecklist();
            }
            else if (nodeStageTitleText != null)
            {
                nodeStageTitleText.text = string.Empty;
                nodeStageBodyText.text = string.Empty;
                nodeStageChecklistText.text = string.Empty;
            }
        }

        private void RefreshContextPanel()
        {
            // The new fullscreen layout drives its own visibility from RefreshButtons.
        }

        private void RefreshHand()
        {
            if (!controller.HasBattle)
            {
                string idleSignature = controller.GetHandStatus() + "|idle";
                if (idleSignature == handSignature)
                {
                    return;
                }

                handSignature = idleSignature;
                ClearEntries(handEntries);
                handEmptyText.text = string.Empty;
                return;
            }

            string signature = controller.GetHandStatus() + "|" + controller.Battle.Energy + "|" + controller.Battle.Phase + "|" + controller.Battle.Hand.Count;
            for (int i = 0; i < controller.Battle.Hand.Count; i++)
            {
                DemoCard card = controller.Battle.Hand[i];
                if (card == null)
                {
                    continue;
                }

                signature += "|" + card.Id + ":" + card.Cost;
            }

            if (signature == handSignature)
            {
                return;
            }

            handSignature = signature;
            ClearEntries(handEntries);

            if (controller.Battle.Phase != DemoBattlePhase.Planning)
            {
                handEmptyText.text = "演武中，手牌区仅保留观察位。";
                return;
            }

            for (int i = 0; i < controller.Battle.Hand.Count; i++)
            {
                int capturedIndex = i;
                DemoCard card = controller.Battle.Hand[i];
                if (card == null)
                {
                    continue;
                }

                GameObject cardObject = CreateCardEntry(handContainer, card, controller.Battle.Energy >= card.Cost);
                Button button = cardObject.GetComponent<Button>();
                button.onClick.AddListener(() => controller.QueueCardAt(capturedIndex));
                handEntries.Add(cardObject);
            }

            handEmptyText.text = controller.Battle.Hand.Count == 0
                ? "暂无可用手牌，系统会在下一轮自动补牌。"
                : "拖牌进入规划队列，留住高点给后续连锁。";
        }

        private void RefreshRewards()
        {
            string signature = controller.GetRewardSummary() + "|" + controller.CurrentRewards.Count;
            for (int i = 0; i < controller.CurrentRewards.Count; i++)
            {
                DemoReward reward = controller.CurrentRewards[i];
                signature += "|" + reward.Name + ":" + reward.Description;
            }

            if (signature == rewardSignature)
            {
                return;
            }

            rewardSignature = signature;
            ClearEntries(rewardEntries);

            if (controller.CurrentRewards.Count == 0)
            {
                return;
            }

            for (int i = 0; i < controller.CurrentRewards.Count; i++)
            {
                int capturedIndex = i;
                DemoReward reward = controller.CurrentRewards[i];
                GameObject rewardObject = CreateRewardEntry(rewardContainer, reward);
                Button button = rewardObject.GetComponent<Button>();
                button.onClick.AddListener(() => controller.ClaimRewardAt(capturedIndex));
                rewardEntries.Add(rewardObject);
            }
        }

        private void OnUtilityButtonClicked()
        {
            controller.AdvanceUtilityNode();
        }

        private void OnBattleActionClicked()
        {
            controller.TriggerBattleAction();
        }

        private string GetHeaderChipText()
        {
            if (controller.IsRunComplete)
            {
                return "道途圆满";
            }

            if (!controller.HasBattle)
            {
                switch (controller.Run.Map.CurrentNode.Type)
                {
                    case DemoNodeType.Start:
                        return "起手定道";
                    case DemoNodeType.RouteChoice:
                        return "路线分叉";
                    case DemoNodeType.Reward:
                        return "战后补强";
                    case DemoNodeType.Training:
                        return "修炼节点";
                    case DemoNodeType.Shop:
                        return "整备阶段";
                    default:
                        return "修行进行中";
                }
            }

            switch (controller.Battle.Phase)
            {
                case DemoBattlePhase.Planning:
                    return "规划阶段";
                case DemoBattlePhase.Executing:
                    return "演武阶段";
                case DemoBattlePhase.Won:
                    return "战斗胜利";
                case DemoBattlePhase.Lost:
                    return "战斗失利";
                default:
                    return "修行进行中";
            }
        }

        private string BuildTopBarSummary()
        {
            if (controller.HasBattle)
            {
                return controller.Battle.IsBossBattle
                    ? $"第 {controller.Battle.Round} 回合   飞剑 {controller.Battle.TotalSwords}   灵气 {controller.Battle.Energy}/{controller.Battle.MaxEnergy}   Boss {controller.Battle.BossPhase + 1}"
                    : $"第 {controller.Battle.Round} 回合   飞剑 {controller.Battle.TotalSwords}   灵气 {controller.Battle.Energy}/{controller.Battle.MaxEnergy}";
            }

            if (IsStartHomeScreen())
            {
                return $"气血 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   灵气 {3 + controller.Run.BonusEnergy}   本命飞剑 {1 + controller.Run.BonusPermanentSwords}";
            }

            return $"生命 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组 {controller.Run.Deck.Count} 张   主修 {GetGongfaName(controller.Run.MainGongfa, "未定主修")}";
        }

        private string BuildHeaderContextText()
        {
            if (controller.IsRunComplete)
            {
                return "天劫已渡：回看本局是如何从立主修走到收束成型。";
            }

            if (controller.HasBattle)
            {
                switch (controller.Battle.Phase)
                {
                    case DemoBattlePhase.Planning:
                        return "规划阶段：把飞剑增殖、状态叠层和收束牌压进同一轮。";
                    case DemoBattlePhase.Executing:
                        return "执行阶段：让飞剑、功法与法宝自动结算，观察演武是否滚起来。";
                    case DemoBattlePhase.Won:
                        return "战斗胜利：准备拿最能改变结构的那一项补强。";
                    case DemoBattlePhase.Lost:
                        return "战斗失利：下一局更早锁定主修，再把奖励往同一路收束。";
                    default:
                        return "云海斗法：低频出牌，高反馈自动演武。";
                }
            }

            if (IsStartPathChoiceScreen())
            {
                return "起手定道：先看清你要修哪一脉";
            }

            if (IsRouteChoiceScreen())
            {
                return "路线分歧：把补强时点留给最缺的那一步";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "起手总览：先照见道基与传承，再决定这局如何起势。";
                case DemoNodeType.Reward:
                    return "战后补强：只拿能改变结构的那一项，别把路线拿散。";
                case DemoNodeType.Training:
                    return "修炼节点：补齐功法与法宝之间的关键缺口。";
                case DemoNodeType.Shop:
                    return "Boss 前整备：优先看续航、灵气和收束点。";
                default:
                    return "道途推进：让飞剑、功法与法宝往同一路靠拢。";
            }
        }

        private string BuildNodeOverlaySummary()
        {
            if (controller.IsRunComplete)
            {
                return "这一局已经从起手一脉，走到渡劫成道。回看构筑，重点看哪一步让飞剑、功法和法宝真正串了起来。";
            }

            return controller.GetBattleSummary() + "\n\n" + BuildContextSummary();
        }

        private string BuildHandHint()
        {
            if (!controller.HasBattle)
            {
                return string.Empty;
            }

            switch (controller.Battle.Phase)
            {
                case DemoBattlePhase.Planning:
                    return "把低频决策压缩到底部，主舞台只负责演武爆发。";
                case DemoBattlePhase.Executing:
                    return "当前已进入执行阶段，飞剑与法术会自动结算。";
                case DemoBattlePhase.Won:
                    return "敌势已破，准备进入下一节点。";
                case DemoBattlePhase.Lost:
                    return "本轮已败，重新开局时尽早确定核心道途。";
                default:
                    return string.Empty;
            }
        }

        private string BuildBattleHint()
        {
            if (!controller.HasBattle)
            {
                return BuildNodeOverlaySummary();
            }

            if (controller.Battle.IsBossBattle)
            {
                return $"Boss 预警：{controller.Battle.BossIntentText}";
            }

            return $"规划队列 {controller.Battle.PlayQueue.Count} 张，优先把飞剑增长和终结技留在同一轮里。";
        }

        private string BuildRunSummary()
        {
            string artifactText = BuildTruncatedList(controller.Run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name), 3, "暂无");
            string relicText = BuildTruncatedList(controller.Run.Relics, 3, "暂无");

            return
                $"主修：{FormatGongfaSummary(controller.Run.MainGongfa, "未定")}\n" +
                $"辅修：{FormatGongfaSummary(controller.Run.SupportGongfa, "未定")}\n" +
                $"神通：{FormatGongfaSummary(controller.Run.DivineGongfa, "未悟")}\n" +
                $"法宝：{artifactText}\n" +
                $"遗物：{relicText}\n" +
                $"生命：{controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组：{controller.Run.Deck.Count} 张";
        }

        private string BuildRunSnapshot()
        {
            string artifactText = BuildTruncatedList(controller.Run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name), 2, "暂无");
            string relicText = BuildTruncatedList(controller.Run.Relics, 2, "暂无");

            return
                $"主修 {GetGongfaName(controller.Run.MainGongfa, "未定")}   辅修 {GetGongfaName(controller.Run.SupportGongfa, "未定")}   神通 {GetGongfaName(controller.Run.DivineGongfa, "未悟")}\n" +
                $"生命 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组 {controller.Run.Deck.Count} 张\n" +
                $"法宝 {artifactText}   遗物 {relicText}";
        }

        private string BuildDeckFocusSummary()
        {
            int wanjian = controller.Run.Deck.Count(card => card.Style == DemoSwordStyle.Wanjian);
            int thunder = controller.Run.Deck.Count(card => card.Style == DemoSwordStyle.Thunder);
            int blood = controller.Run.Deck.Count(card => card.Style == DemoSwordStyle.Blood);
            int general = controller.Run.Deck.Count(card => card.Style == DemoSwordStyle.General);

            string focus = GetStyleLabel(controller.Run.GetBuildStyle());
            string topCards = BuildTruncatedList(controller.Run.Deck.Select(card => card.Name), 3, "暂无核心牌");

            return
                $"当前倾向：{focus}\n" +
                $"万剑 {wanjian} / 雷剑 {thunder} / 血剑 {blood} / 通用 {general}\n" +
                $"起手核心：{topCards}";
        }

        private string BuildRewardLeadText()
        {
            if (IsRouteChoiceScreen())
            {
                return "路线不只决定去哪，还决定什么时候补强、什么时候冒险、什么时候把爆发留给天劫。";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "先立道途，再让这一局的掉落、功法和演武高点朝同一脉收束。";
                case DemoNodeType.Training:
                    return "补齐功法和法宝之间的缺口，让构筑开始收束。";
                case DemoNodeType.Shop:
                    return "Boss 前做最后一次取舍，优先补爆发窗口或续航。";
                case DemoNodeType.Reward:
                    return "只拿最能改变演武结构的一项，别把路线选散。";
                default:
                    return controller.GetRewardSummary();
            }
        }

        private bool IsStartHomeScreen()
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.Start
                && controller.CurrentRewards.Count == 0;
        }

        private bool IsStartPresentationScreen()
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.Start;
        }

        private string BuildStartBaseScrollText()
        {
            return
                $"气血：{controller.Run.CurrentHealth} / {controller.Run.MaxHealth}\n" +
                $"牌组：{controller.Run.Deck.Count} 张\n" +
                $"灵气上限：{3 + controller.Run.BonusEnergy}\n" +
                $"常驻飞剑：{1 + controller.Run.BonusPermanentSwords}\n" +
                "\n" +
                $"主修位：{GetGongfaName(controller.Run.MainGongfa, "未定")}\n" +
                $"辅修位：{GetGongfaName(controller.Run.SupportGongfa, "未定")}\n" +
                $"神通位：{GetGongfaName(controller.Run.DivineGongfa, "未悟")}";
        }

        private string BuildStartInheritanceScrollText()
        {
            List<string> lines = new List<string>
            {
                $"灵气传承：+{controller.Run.BonusEnergy}",
                $"飞剑传承：+{controller.Run.BonusPermanentSwords}",
                string.Empty,
                "随行法宝："
            };

            if (controller.Run.Artifacts.Count == 0)
            {
                lines.Add("暂无");
            }
            else
            {
                for (int i = 0; i < Mathf.Min(2, controller.Run.Artifacts.Count); i++)
                {
                    lines.Add($"· {DemoArtifactLibrary.Get(controller.Run.Artifacts[i]).Name}");
                }

                if (controller.Run.Artifacts.Count > 2)
                {
                    lines.Add($"· 其余 {controller.Run.Artifacts.Count - 2} 件");
                }
            }

            lines.Add(string.Empty);
            lines.Add("可继承遗物：");

            if (controller.Run.Relics.Count == 0)
            {
                lines.Add("暂无");
            }
            else
            {
                for (int i = 0; i < Mathf.Min(3, controller.Run.Relics.Count); i++)
                {
                    lines.Add($"· {controller.Run.Relics[i]}");
                }

                if (controller.Run.Relics.Count > 3)
                {
                    lines.Add($"· 其余 {controller.Run.Relics.Count - 3} 件");
                }
            }

            return string.Join("\n", lines);
        }

        private bool IsStartPathChoiceScreen()
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.Start
                && controller.CurrentRewards.Count > 0
                && controller.CurrentRewards.All(reward => reward.Type == DemoRewardType.Gongfa);
        }

        private bool IsRouteChoiceScreen()
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.RouteChoice
                && controller.CurrentRewards.Count > 0
                && controller.CurrentRewards.All(reward => reward.Type == DemoRewardType.Route);
        }

        private string BuildNodeStageTitle()
        {
            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "起手先定主修";
                case DemoNodeType.Training:
                    return "修炼节点";
                case DemoNodeType.Shop:
                    return "Boss 前整备";
                case DemoNodeType.Victory:
                    return "本局道途已成";
                default:
                    return "本节点关注";
            }
        }

        private string BuildNodeStageBody()
        {
            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "先在三条剑道里锁定主脉，再让后续掉落、奖励和演武爆发都沿着同一路收束。";
                case DemoNodeType.Training:
                    return "修炼节点最适合补齐功法、法宝和终结技之间的缺口，让自动演武从能打变成会滚。";
                case DemoNodeType.Shop:
                    return "进入 Boss 前的最后一次取舍，优先补灵气续航、关键收束点，别把资源撒成平均数。";
                case DemoNodeType.Victory:
                    return "这一局已经从立主修走到收束成型，可以回看是哪一轮补强把飞剑、功法和法宝真正串了起来。";
                default:
                    return BuildContextSummary();
            }
        }

        private string BuildNodeStageChecklist()
        {
            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "· 先定主修\n· 锁后续掉落倾向\n· 在 Boss 前完成第一轮收束";
                case DemoNodeType.Training:
                    return "· 优先补关键功法\n· 让法宝接上主脉\n· 给终结技留出爆发窗";
                case DemoNodeType.Shop:
                    return "· 看续航是否够打满三段\n· 看灵气是否支撑连锁\n· 看爆发是否能压住 Boss";
                case DemoNodeType.Victory:
                    return "· 回看起手定向是否清晰\n· 回看哪次补强最关键\n· 回看 Build 爆发是否提前成型";
                default:
                    return "· 优先补结构，不只补数值\n· 保持奖励朝同一路倾斜\n· 把高点留给演武收尾";
            }
        }

        private string BuildContextSummary()
        {
            if (controller.HasBattle && controller.Battle.IsBossBattle)
            {
                return $"Boss 预警：{controller.Battle.BossIntentText}";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "先锁定主修，再让后续奖励沿着同一条道途掉落。";
                case DemoNodeType.RouteChoice:
                    return "先想清楚你缺的是节点数量、补强时机，还是 Boss 前的最后一次搏命机会。";
                case DemoNodeType.Reward:
                    return "优先拿能改变演武结构的补强，而不是只加一点面板数值。";
                case DemoNodeType.Training:
                    return "修炼节点最适合补齐功法、法宝和终结技之间的缺口。";
                case DemoNodeType.Shop:
                    return "Boss 前整备优先看续航、灵气和爆发窗口，不要把资源花散。";
                case DemoNodeType.Victory:
                    return "这一局的道途已经成型，可以回头看自己是如何收束 build 的。";
                default:
                    return "当前没有奖励节点时，这里显示 build 摘要与演武高点。";
            }
        }

        private static string BuildTruncatedList(IEnumerable<string> items, int limit, string emptyText)
        {
            if (items == null)
            {
                return emptyText;
            }

            List<string> values = items.Where(item => !string.IsNullOrEmpty(item)).ToList();
            if (values.Count == 0)
            {
                return emptyText;
            }

            if (values.Count <= limit)
            {
                return string.Join("、", values);
            }

            return string.Join("、", values.Take(limit)) + "等";
        }

        private string BuildArtifactPreview()
        {
            List<string> lines = new List<string>();

            lines.Add("功法");
            lines.Add($"主修：{FormatGongfaSummary(controller.Run.MainGongfa, "未定主修")}");
            lines.Add($"辅修：{FormatGongfaSummary(controller.Run.SupportGongfa, "未定辅修")}");
            lines.Add($"神通：{FormatGongfaSummary(controller.Run.DivineGongfa, "未悟神通")}");
            lines.Add(string.Empty);
            lines.Add("法宝");

            if (controller.Run.Artifacts.Count == 0)
            {
                lines.Add("尚未获得法宝。");
            }
            else
            {
                foreach (DemoArtifactType artifact in controller.Run.Artifacts)
                {
                    DemoArtifactDefinition definition = DemoArtifactLibrary.Get(artifact);
                    lines.Add($"{GetQualityLabel(definition.Quality)} {definition.IconGlyph}·{definition.Name}");
                    lines.Add(definition.Description);
                    lines.Add(string.Empty);
                }
            }

            return string.Join("\n", lines).TrimEnd();
        }

        private static string GetGongfaName(DemoGongfaType type, string fallback)
        {
            return type == DemoGongfaType.None ? fallback : DemoGongfaLibrary.Get(type).Name;
        }

        private string BuildDeckPreview()
        {
            string[] cards = controller.GetDeckSummary().Split('\n');
            int previewCount = Mathf.Min(12, cards.Length);
            List<string> lines = new List<string>();

            for (int i = 0; i < previewCount; i++)
            {
                lines.Add(cards[i]);
            }

            if (cards.Length > previewCount)
            {
                lines.Add($"... 其余 {cards.Length - previewCount} 张");
            }

            return string.Join("\n", lines);
        }

        private static string BuildCardPreviewText(DemoCard card)
        {
            List<string> parts = new List<string>();

            if (card.Damage > 0)
            {
                parts.Add($"伤害 {card.Damage}");
            }

            if (card.Block > 0)
            {
                parts.Add($"护盾 {card.Block}");
            }

            if (card.SwordIntent > 0)
            {
                parts.Add($"剑意 +{card.SwordIntent}");
            }

            if (card.Shock > 0)
            {
                parts.Add($"感电 +{card.Shock}");
            }

            if (card.Bleed > 0)
            {
                parts.Add($"流血 +{card.Bleed}");
            }

            if (card.TemporarySwords > 0)
            {
                parts.Add($"临时飞剑 +{card.TemporarySwords}");
            }

            if (card.PermanentSword)
            {
                parts.Add("永久飞剑 +1");
            }

            if (card.Draw > 0)
            {
                parts.Add($"抽牌 {card.Draw}");
            }

            if (card.EnergyGain > 0)
            {
                parts.Add($"回气 {card.EnergyGain}");
            }

            if (card.ConsumeAllSwordIntent)
            {
                parts.Add("消耗全部剑意");
            }

            if (parts.Count == 0)
            {
                return "作为本轮演武的节奏补件。";
            }

            int count = Mathf.Min(3, parts.Count);
            return string.Join("\n", parts.Take(count));
        }

        private GameObject CreateCardEntry(Transform parent, DemoCard card, bool interactable)
        {
            GameObject cardObject = new GameObject("Card_" + card.Id, typeof(RectTransform), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(176f, 162f);

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 176f;
            layout.minWidth = 176f;
            layout.preferredHeight = 162f;
            layout.minHeight = 162f;

            Image image = cardObject.GetComponent<Image>();
            image.color = new Color(0.11f, 0.11f, 0.12f, 0.98f);

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.interactable = interactable;
            button.colors = CreateButtonColors(interactable ? new Color(0.18f, 0.16f, 0.13f, 0.98f) : new Color(0.12f, 0.12f, 0.12f, 0.84f));

            VerticalLayoutGroup layoutGroup = cardObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(12, 12, 10, 10);
            layoutGroup.spacing = 5f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;
            DecorateFrame(cardObject.GetComponent<RectTransform>(), GetCardAccentColor(card.Style), new Color(0.22f, 0.17f, 0.11f, 0.14f), 2f, true);

            GameObject metaRowObject = new GameObject("MetaRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            metaRowObject.transform.SetParent(cardObject.transform, false);
            LayoutElement metaLayout = metaRowObject.GetComponent<LayoutElement>();
            metaLayout.preferredHeight = 24f;
            HorizontalLayoutGroup metaRow = metaRowObject.GetComponent<HorizontalLayoutGroup>();
            metaRow.spacing = 4f;
            metaRow.childAlignment = TextAnchor.MiddleLeft;
            metaRow.childControlHeight = true;
            metaRow.childControlWidth = false;
            metaRow.childForceExpandHeight = false;
            metaRow.childForceExpandWidth = false;

            CreateLabelChip(metaRowObject.transform, $"{card.Cost} 气", new Color(0.18f, 0.14f, 0.10f, 1f), ColorGold, 44f);
            CreateLabelChip(metaRowObject.transform, GetStyleLabel(card.Style), GetCardColor(card.Style), ColorPaper, 50f);
            CreateLabelChip(metaRowObject.transform, GetQualityLabel(card.Quality), GetQualityColor(card.Quality), ColorPaper, 34f);

            Text nameText = CreateText(cardObject.transform, "Name", card.Name, 17, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            LayoutElement nameLayout = nameText.gameObject.AddComponent<LayoutElement>();
            nameLayout.preferredHeight = 26f;

            RectTransform artPanel = CreateFixedHeightBlock(cardObject.transform, "ArtPanel", 44f, new Color(0.16f, 0.13f, 0.10f, 0.96f));
            DecorateFrame(artPanel, GetQualityColor(card.Quality), new Color(0.24f, 0.20f, 0.14f, 0.18f), 1.5f, false);
            CreateStretchPanel(
                artPanel,
                "ArtGlow",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(GetQualityColor(card.Quality).r, GetQualityColor(card.Quality).g, GetQualityColor(card.Quality).b, 0.12f));

            RectTransform iconSeal = CreateFixedPanel(
                artPanel,
                "IconSeal",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(12f, 0f),
                new Vector2(32f, 32f),
                new Color(0.12f, 0.10f, 0.08f, 0.98f));
            DecorateFrame(iconSeal, GetQualityColor(card.Quality), new Color(0.18f, 0.14f, 0.10f, 0.12f), 1f, false);
            Text iconText = CreateText(iconSeal, "IconText", GetCardIconGlyph(card), 18, FontStyle.Bold, TextAnchor.MiddleCenter, GetQualityColor(card.Quality));
            StretchText(iconText.rectTransform, new Vector2(4f, 4f), new Vector2(-4f, -4f));

            Text artLabel = CreateText(artPanel, "ArtLabel", GetCardMoodText(card), 12, FontStyle.Bold, TextAnchor.MiddleLeft, GetCardAccentColor(card.Style));
            artLabel.rectTransform.anchorMin = new Vector2(0f, 0f);
            artLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
            artLabel.rectTransform.offsetMin = new Vector2(52f, 4f);
            artLabel.rectTransform.offsetMax = new Vector2(-10f, -16f);

            Text qualityText = CreateText(artPanel, "QualityText", GetCardTypeLabel(card.Type), 11, FontStyle.Bold, TextAnchor.LowerRight, GetQualityColor(card.Quality));
            qualityText.rectTransform.anchorMin = new Vector2(0f, 0f);
            qualityText.rectTransform.anchorMax = new Vector2(1f, 1f);
            qualityText.rectTransform.offsetMin = new Vector2(56f, 14f);
            qualityText.rectTransform.offsetMax = new Vector2(-10f, -4f);

            Text rulesText = CreateText(cardObject.transform, "Rules", BuildCardPreviewText(card), 12, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.82f, 0.84f, 0.86f, 1f));
            LayoutElement rulesLayout = rulesText.gameObject.AddComponent<LayoutElement>();
            rulesLayout.flexibleHeight = 1f;

            RectTransform footer = CreateFixedHeightBlock(cardObject.transform, "Footer", 24f, new Color(0.14f, 0.12f, 0.10f, 0.98f));
            CreateStretchPanel(
                footer,
                "FooterAccent",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(10f, 0f),
                new Vector2(-10f, 2f),
                GetCardAccentColor(card.Style));
            Text actionText = CreateText(footer, "Action", interactable ? "纳入规划" : "灵气不足", 12, FontStyle.Bold, TextAnchor.MiddleCenter, interactable ? ColorPaper : new Color(0.66f, 0.66f, 0.68f, 1f));
            StretchText(actionText.rectTransform, new Vector2(6f, 2f), new Vector2(-6f, -2f));

            return cardObject;
        }

        private GameObject CreateRewardEntry(Transform parent, DemoReward reward)
        {
            if (IsStartPathChoiceScreen() && reward.Type == DemoRewardType.Gongfa)
            {
                return CreatePathChoiceEntry(parent, reward);
            }

            GameObject rewardObject = new GameObject("Reward_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            rewardObject.transform.SetParent(parent, false);
            RectTransform rewardRect = rewardObject.GetComponent<RectTransform>();
            rewardRect.sizeDelta = new Vector2(392f, 500f);

            LayoutElement layout = rewardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 392f;
            layout.minWidth = 392f;
            layout.preferredHeight = 500f;
            layout.minHeight = 500f;

            Image image = rewardObject.GetComponent<Image>();
            image.color = new Color(0.10f, 0.11f, 0.13f, 0.98f);

            Button button = rewardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(new Color(0.16f, 0.17f, 0.20f, 1f));
            DecorateFrame(rewardObject.GetComponent<RectTransform>(), GetRewardAccentColor(reward), new Color(0.15f, 0.16f, 0.18f, 0.20f), 2f, true);

            VerticalLayoutGroup layoutGroup = rewardObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(20, 20, 20, 20);
            layoutGroup.spacing = 12f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            RectTransform badge = CreateFixedHeightBlock(rewardObject.transform, "RewardBadge", 176f, GetRewardPanelColor(reward));
            DecorateFrame(badge, GetRewardAccentColor(reward), new Color(0.14f, 0.15f, 0.18f, 0.14f), 1.5f, false);
            CreateStretchPanel(
                badge,
                "RewardBadgeGlow",
                new Vector2(0f, 0.48f),
                new Vector2(1f, 1f),
                new Vector2(10f, 8f),
                new Vector2(-10f, -10f),
                new Color(GetRewardAccentColor(reward).r, GetRewardAccentColor(reward).g, GetRewardAccentColor(reward).b, 0.10f));

            RectTransform iconSeal = CreateFixedPanel(
                badge,
                "RewardIconSeal",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 4f),
                new Vector2(74f, 74f),
                new Color(0.09f, 0.10f, 0.12f, 0.94f));
            DecorateFrame(iconSeal, GetRewardAccentColor(reward), new Color(0.10f, 0.11f, 0.13f, 0.10f), 1.5f, false);

            Text badgeIconText = CreateText(iconSeal, "BadgeIconText", GetRewardGlyph(reward), 34, FontStyle.Bold, TextAnchor.MiddleCenter, GetRewardAccentColor(reward));
            StretchText(badgeIconText.rectTransform, new Vector2(8f, 8f), new Vector2(-8f, -8f));

            Text badgeText = CreateText(badge, "BadgeText", GetRewardTypeLabel(reward), 14, FontStyle.Bold, TextAnchor.LowerCenter, ColorPaper);
            badgeText.rectTransform.anchorMin = new Vector2(0f, 0f);
            badgeText.rectTransform.anchorMax = new Vector2(1f, 0f);
            badgeText.rectTransform.offsetMin = new Vector2(4f, 16f);
            badgeText.rectTransform.offsetMax = new Vector2(-4f, 40f);

            GameObject bodyObject = new GameObject("RewardBody", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            bodyObject.transform.SetParent(rewardObject.transform, false);
            LayoutElement bodyLayout = bodyObject.GetComponent<LayoutElement>();
            bodyLayout.flexibleHeight = 1f;
            VerticalLayoutGroup bodyGroup = bodyObject.GetComponent<VerticalLayoutGroup>();
            bodyGroup.spacing = 8f;
            bodyGroup.childAlignment = TextAnchor.UpperLeft;
            bodyGroup.childControlHeight = true;
            bodyGroup.childControlWidth = true;
            bodyGroup.childForceExpandHeight = false;
            bodyGroup.childForceExpandWidth = true;

            GameObject bodyMeta = new GameObject("RewardMeta", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            bodyMeta.transform.SetParent(bodyObject.transform, false);
            LayoutElement bodyMetaLayout = bodyMeta.GetComponent<LayoutElement>();
            bodyMetaLayout.preferredHeight = 24f;
            HorizontalLayoutGroup bodyMetaGroup = bodyMeta.GetComponent<HorizontalLayoutGroup>();
            bodyMetaGroup.spacing = 8f;
            bodyMetaGroup.childAlignment = TextAnchor.MiddleLeft;
            bodyMetaGroup.childControlHeight = true;
            bodyMetaGroup.childControlWidth = false;
            bodyMetaGroup.childForceExpandHeight = false;
            bodyMetaGroup.childForceExpandWidth = false;

            Text rewardNameText = CreateText(bodyObject.transform, "RewardName", reward.Name, 28, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            LayoutElement rewardNameLayout = rewardNameText.gameObject.AddComponent<LayoutElement>();
            rewardNameLayout.preferredHeight = 36f;

            CreateLabelChip(bodyMeta.transform, GetQualityLongLabel(GetRewardQuality(reward)), GetQualityColor(GetRewardQuality(reward)), ColorPaper, 62f);
            CreateLabelChip(bodyMeta.transform, GetRewardFlavorText(reward), new Color(0.13f, 0.16f, 0.19f, 1f), ColorMist, 104f);

            Text rewardDescriptionText = CreateText(bodyObject.transform, "RewardDescription", BuildRewardPreviewText(reward), 17, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.86f, 0.87f, 0.90f, 1f));
            LayoutElement rewardDescriptionLayout = rewardDescriptionText.gameObject.AddComponent<LayoutElement>();
            rewardDescriptionLayout.flexibleHeight = 1f;

            RectTransform footer = CreateFixedHeightBlock(rewardObject.transform, "RewardFooter", 62f, new Color(0.09f, 0.10f, 0.12f, 0.96f));
            CreateStretchPanel(
                footer,
                "RewardFooterAccent",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(16f, 0f),
                new Vector2(-16f, 3f),
                GetRewardAccentColor(reward));
            Text footerText = CreateText(footer, "RewardFooterText", "点击收下", 16, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            StretchText(footerText.rectTransform, new Vector2(10f, 6f), new Vector2(-10f, -18f));

            Text footerHintText = CreateText(footer, "RewardFooterHint", "悬停查看完整机制", 11, FontStyle.Normal, TextAnchor.LowerCenter, ColorGoldDim);
            footerHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
            footerHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            footerHintText.rectTransform.offsetMin = new Vector2(8f, 8f);
            footerHintText.rectTransform.offsetMax = new Vector2(-8f, 24f);

            AddRewardHoverEvents(rewardObject, reward);

            return rewardObject;
        }

        private GameObject CreatePathChoiceEntry(Transform parent, DemoReward reward)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(reward.GongfaType);
            Color accent = GetCardAccentColor(definition.Style);
            Color panelColor = GetPathChoicePanelColor(definition.Style);
            Sprite pathChoiceSprite = LoadPathChoiceSprite(definition.Style);
            bool hasPathChoiceSprite = pathChoiceSprite != null;

            GameObject cardObject = new GameObject("PathChoice_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = hasPathChoiceSprite ? 296f : 330f;
            layout.minWidth = hasPathChoiceSprite ? 296f : 330f;
            layout.preferredHeight = hasPathChoiceSprite ? 560f : 612f;
            layout.minHeight = hasPathChoiceSprite ? 560f : 612f;
            cardRect.sizeDelta = new Vector2(layout.preferredWidth, layout.preferredHeight);

            Image image = cardObject.GetComponent<Image>();
            image.color = hasPathChoiceSprite ? Color.white : new Color(0.94f, 0.93f, 0.89f, 0.98f);
            image.sprite = pathChoiceSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(hasPathChoiceSprite ? new Color(1f, 1f, 1f, 1f) : new Color(0.93f, 0.92f, 0.88f, 1f));

            DecorateFrame(cardRect, new Color(accent.r * 0.85f, accent.g * 0.85f, accent.b * 0.85f, 0.95f), new Color(0.18f, 0.16f, 0.12f, 0.05f), 2f, true);

            if (hasPathChoiceSprite)
            {
                CreateStretchPanel(
                    cardObject.transform,
                    "TopVeil",
                    new Vector2(0f, 0.74f),
                    new Vector2(1f, 1f),
                    new Vector2(10f, 10f),
                    new Vector2(-10f, -10f),
                    new Color(0f, 0f, 0f, 0.16f));
                CreateStretchPanel(
                    cardObject.transform,
                    "BottomVeil",
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0.34f),
                    new Vector2(10f, 10f),
                    new Vector2(-10f, -10f),
                    new Color(0f, 0f, 0f, 0.22f));
                CreateStretchPanel(
                    cardObject.transform,
                    "PaperTint",
                    new Vector2(0f, 0f),
                    new Vector2(1f, 1f),
                    new Vector2(10f, 10f),
                    new Vector2(-10f, -10f),
                    new Color(1f, 1f, 1f, 0.04f));
            }
            else
            {
                CreateStretchPanel(
                    cardObject.transform,
                    "PaperTint",
                    new Vector2(0f, 0f),
                    new Vector2(1f, 1f),
                    new Vector2(10f, 10f),
                    new Vector2(-10f, -10f),
                    new Color(1f, 1f, 1f, 0.05f));

                RectTransform inkStage = CreateFixedPanel(
                    cardObject.transform,
                    "InkStage",
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -152f),
                    new Vector2(256f, 368f),
                    panelColor);
                DecorateFrame(inkStage, new Color(accent.r, accent.g, accent.b, 0.72f), new Color(0f, 0f, 0f, 0.02f), 1.5f, false);

                DemoPathChoiceCardFx fx = cardObject.AddComponent<DemoPathChoiceCardFx>();
                BuildPathChoiceStage(inkStage, definition, accent, fx);
            }

            Text slotText;
            if (hasPathChoiceSprite)
            {
                RectTransform slotChip = CreateFixedPanel(
                    cardObject.transform,
                    "SlotChip",
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(24f, -20f),
                    new Vector2(84f, 24f),
                    new Color(0.10f, 0.10f, 0.11f, 0.78f));
                DecorateFrame(slotChip, new Color(accent.r, accent.g, accent.b, 0.48f), new Color(0f, 0f, 0f, 0f), 1f, false);
                slotText = CreateText(slotChip, "SlotText", "起手主修", 11, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
                StretchText(slotText.rectTransform, new Vector2(6f, 2f), new Vector2(-6f, -2f));
            }
            else
            {
                slotText = CreateText(cardObject.transform, "SlotText", "起手主修", 12, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.45f, 0.37f, 0.20f, 0.92f));
                slotText.rectTransform.anchorMin = new Vector2(0f, 1f);
                slotText.rectTransform.anchorMax = new Vector2(0f, 1f);
                slotText.rectTransform.pivot = new Vector2(0f, 1f);
                slotText.rectTransform.anchoredPosition = new Vector2(26f, -22f);
                slotText.rectTransform.sizeDelta = new Vector2(96f, 20f);
            }

            RectTransform qualityChip = CreateFixedPanel(
                cardObject.transform,
                "QualityChip",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-22f, -20f),
                new Vector2(68f, 26f),
                GetQualityColor(definition.Quality));
            DecorateFrame(qualityChip, ColorPaper * 0.8f, new Color(0f, 0f, 0f, 0f), 1f, false);
            Text qualityText = CreateText(qualityChip, "QualityText", GetQualityLongLabel(definition.Quality), 11, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            StretchText(qualityText.rectTransform, new Vector2(4f, 2f), new Vector2(-4f, -2f));

            if (hasPathChoiceSprite)
            {
                RectTransform titleBand = CreateFixedPanel(
                    cardObject.transform,
                    "TitleBand",
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -62f),
                    new Vector2(236f, 84f),
                    new Color(0.08f, 0.09f, 0.10f, 0.64f));
                DecorateFrame(titleBand, new Color(accent.r, accent.g, accent.b, 0.56f), new Color(1f, 1f, 1f, 0.02f), 1.5f, false);

                Text nameText = CreateText(titleBand, "NameText", reward.Name, 28, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
                nameText.rectTransform.anchorMin = new Vector2(0f, 1f);
                nameText.rectTransform.anchorMax = new Vector2(1f, 1f);
                nameText.rectTransform.offsetMin = new Vector2(14f, -40f);
                nameText.rectTransform.offsetMax = new Vector2(-14f, -8f);

                Text styleText = CreateText(titleBand, "StyleText", GetPathChoiceStyleTitle(definition.Style), 15, FontStyle.Bold, TextAnchor.LowerCenter, accent);
                styleText.rectTransform.anchorMin = new Vector2(0f, 0f);
                styleText.rectTransform.anchorMax = new Vector2(1f, 0f);
                styleText.rectTransform.offsetMin = new Vector2(12f, 10f);
                styleText.rectTransform.offsetMax = new Vector2(-12f, 34f);

                RectTransform infoBand = CreateFixedPanel(
                    cardObject.transform,
                    "InfoBand",
                    new Vector2(0.5f, 0f),
                    new Vector2(0.5f, 0f),
                    new Vector2(0f, 82f),
                    new Vector2(266f, 146f),
                    new Color(0.08f, 0.09f, 0.10f, 0.78f));
                DecorateFrame(infoBand, new Color(accent.r, accent.g, accent.b, 0.60f), new Color(1f, 1f, 1f, 0.02f), 1.5f, false);

                Text hookText = CreateText(infoBand, "HookText", BuildPathChoiceHook(definition.Style), 15, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
                hookText.rectTransform.anchorMin = new Vector2(0f, 1f);
                hookText.rectTransform.anchorMax = new Vector2(1f, 1f);
                hookText.rectTransform.offsetMin = new Vector2(16f, -32f);
                hookText.rectTransform.offsetMax = new Vector2(-16f, -8f);

                string[] keywords = GetPathChoiceKeywords(definition.Style);
                for (int i = 0; i < keywords.Length; i++)
                {
                    Text featureText = CreateText(infoBand, "Feature_" + i, "· " + keywords[i], 13, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.92f, 0.92f, 0.88f, 0.98f));
                    featureText.rectTransform.anchorMin = new Vector2(0f, 1f);
                    featureText.rectTransform.anchorMax = new Vector2(1f, 1f);
                    featureText.rectTransform.offsetMin = new Vector2(18f, -58f - i * 22f);
                    featureText.rectTransform.offsetMax = new Vector2(-14f, -38f - i * 22f);
                }

                Text summaryHint = CreateText(infoBand, "SummaryHint", "后续奖励会向这一脉倾斜", 11, FontStyle.Normal, TextAnchor.LowerCenter, ColorGoldDim);
                summaryHint.rectTransform.anchorMin = new Vector2(0f, 0f);
                summaryHint.rectTransform.anchorMax = new Vector2(1f, 0f);
                summaryHint.rectTransform.offsetMin = new Vector2(14f, 8f);
                summaryHint.rectTransform.offsetMax = new Vector2(-14f, 24f);
            }
            else
            {
                Text nameText = CreateText(cardObject.transform, "NameText", reward.Name, 30, FontStyle.Bold, TextAnchor.UpperCenter, new Color(0.10f, 0.10f, 0.09f, 0.98f));
                nameText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                nameText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                nameText.rectTransform.pivot = new Vector2(0.5f, 1f);
                nameText.rectTransform.anchoredPosition = new Vector2(0f, -54f);
                nameText.rectTransform.sizeDelta = new Vector2(250f, 38f);

                Text styleText = CreateText(cardObject.transform, "StyleText", GetPathChoiceStyleTitle(definition.Style), 16, FontStyle.Bold, TextAnchor.UpperCenter, accent);
                styleText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                styleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                styleText.rectTransform.pivot = new Vector2(0.5f, 1f);
                styleText.rectTransform.anchoredPosition = new Vector2(0f, -96f);
                styleText.rectTransform.sizeDelta = new Vector2(240f, 24f);

                Text hookText = CreateText(cardObject.transform, "HookText", BuildPathChoiceHook(definition.Style), 16, FontStyle.Bold, TextAnchor.UpperCenter, new Color(0.15f, 0.15f, 0.14f, 0.92f));
                hookText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
                hookText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
                hookText.rectTransform.pivot = new Vector2(0.5f, 1f);
                hookText.rectTransform.anchoredPosition = new Vector2(0f, -494f);
                hookText.rectTransform.sizeDelta = new Vector2(252f, 42f);

                RectTransform featuresPanel = CreateFixedPanel(
                    cardObject.transform,
                    "FeaturesPanel",
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(0f, -560f),
                    new Vector2(272f, 96f),
                    new Color(0.10f, 0.10f, 0.11f, 0.74f));
                DecorateFrame(featuresPanel, new Color(accent.r, accent.g, accent.b, 0.56f), new Color(1f, 1f, 1f, 0.02f), 1.5f, false);

                string[] keywords = GetPathChoiceKeywords(definition.Style);
                for (int i = 0; i < keywords.Length; i++)
                {
                    Text featureText = CreateText(featuresPanel, "Feature_" + i, "· " + keywords[i], 14, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.92f, 0.92f, 0.88f, 0.98f));
                    featureText.rectTransform.anchorMin = new Vector2(0f, 1f);
                    featureText.rectTransform.anchorMax = new Vector2(1f, 1f);
                    featureText.rectTransform.offsetMin = new Vector2(18f, -24f - i * 24f);
                    featureText.rectTransform.offsetMax = new Vector2(-16f, -8f - i * 24f);
                }

                Text summaryText = CreateText(cardObject.transform, "SummaryText", BuildPathChoiceSummary(definition.Style), 13, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.34f, 0.36f, 0.38f, 0.92f));
                summaryText.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                summaryText.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                summaryText.rectTransform.pivot = new Vector2(0.5f, 0f);
                summaryText.rectTransform.anchoredPosition = new Vector2(0f, 70f);
                summaryText.rectTransform.sizeDelta = new Vector2(264f, 40f);
            }

            RectTransform footer = CreateFixedPanel(
                cardObject.transform,
                "Footer",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 14f),
                new Vector2(220f, 46f),
                new Color(0.14f, 0.12f, 0.10f, 0.96f));
            DecorateFrame(footer, accent, new Color(0f, 0f, 0f, 0f), 1.5f, false);
            Text footerText = CreateText(footer, "FooterText", "点击立此道途", 14, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            StretchText(footerText.rectTransform, new Vector2(8f, 4f), new Vector2(-8f, -4f));
            return cardObject;
        }

        private void BuildNodeChoiceStage(RectTransform parent)
        {
            CreateStretchPanel(
                parent,
                "StageGlow",
                new Vector2(0f, 0.54f),
                new Vector2(1f, 1f),
                new Vector2(10f, 10f),
                new Vector2(-10f, -10f),
                new Color(1f, 1f, 1f, 0.03f));

            nodeGuidanceStageRoot = new GameObject("NodeGuidanceStageRoot", typeof(RectTransform));
            nodeGuidanceStageRoot.transform.SetParent(parent, false);
            StretchText(nodeGuidanceStageRoot.GetComponent<RectTransform>(), new Vector2(18f, 18f), new Vector2(-18f, -18f));

            RectTransform focusPanel = CreateInkPanel(
                nodeGuidanceStageRoot.transform,
                "GuidanceFocusPanel",
                new Vector2(0f, 0f),
                new Vector2(0.64f, 1f),
                new Vector2(0f, 0f),
                new Vector2(-14f, 0f),
                new Color(0.08f, 0.09f, 0.11f, 0.86f));

            nodeStageTitleText = CreateText(focusPanel, "GuidanceTitle", string.Empty, 24, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            nodeStageTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nodeStageTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeStageTitleText.rectTransform.offsetMin = new Vector2(22f, -42f);
            nodeStageTitleText.rectTransform.offsetMax = new Vector2(-22f, -12f);

            nodeStageBodyText = CreateText(focusPanel, "GuidanceBody", string.Empty, 15, FontStyle.Normal, TextAnchor.UpperLeft, ColorMist);
            nodeStageBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            nodeStageBodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeStageBodyText.rectTransform.offsetMin = new Vector2(22f, 24f);
            nodeStageBodyText.rectTransform.offsetMax = new Vector2(-22f, -62f);

            RectTransform checklistPanel = CreateInkPanel(
                nodeGuidanceStageRoot.transform,
                "GuidanceChecklistPanel",
                new Vector2(0.64f, 0f),
                new Vector2(1f, 1f),
                new Vector2(14f, 0f),
                new Vector2(0f, 0f),
                new Color(0.08f, 0.09f, 0.11f, 0.86f));
            CreatePinnedPanelTitle(checklistPanel, "节点摘要");
            nodeStageChecklistText = CreateBodyText(checklistPanel, "GuidanceChecklistText", 15, ColorMist);
            StretchText(nodeStageChecklistText.rectTransform, new Vector2(18f, 48f), new Vector2(-18f, -18f));
        }

        private GameObject CreateNodePathPreviewCard(Transform parent, DemoGongfaType gongfaType)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(gongfaType);
            Color accent = GetCardAccentColor(definition.Style);
            Sprite sprite = LoadPathChoiceSprite(definition.Style);
            bool hasSprite = sprite != null;

            GameObject previewObject = new GameObject("NodePreview_" + gongfaType, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            previewObject.transform.SetParent(parent, false);

            LayoutElement layout = previewObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 230f;
            layout.minWidth = 230f;
            layout.preferredHeight = 302f;
            layout.minHeight = 302f;

            RectTransform previewRect = previewObject.GetComponent<RectTransform>();
            previewRect.sizeDelta = new Vector2(230f, 302f);

            Image image = previewObject.GetComponent<Image>();
            image.color = hasSprite ? Color.white : new Color(0.93f, 0.92f, 0.88f, 0.98f);
            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;

            DecorateFrame(previewRect, new Color(accent.r * 0.84f, accent.g * 0.84f, accent.b * 0.84f, 0.92f), new Color(0.18f, 0.16f, 0.12f, 0.05f), 2f, true);

            CreateStretchPanel(
                previewObject.transform,
                "TopVeil",
                new Vector2(0f, 0.70f),
                new Vector2(1f, 1f),
                new Vector2(10f, 10f),
                new Vector2(-10f, -10f),
                new Color(0f, 0f, 0f, 0.18f));
            CreateStretchPanel(
                previewObject.transform,
                "BottomVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 0.38f),
                new Vector2(10f, 10f),
                new Vector2(-10f, -10f),
                new Color(0f, 0f, 0f, 0.26f));

            RectTransform titleBand = CreateFixedPanel(
                previewObject.transform,
                "TitleBand",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -48f),
                new Vector2(184f, 70f),
                new Color(0.08f, 0.09f, 0.10f, 0.64f));
            DecorateFrame(titleBand, new Color(accent.r, accent.g, accent.b, 0.50f), new Color(1f, 1f, 1f, 0.02f), 1.5f, false);

            Text nameText = CreateText(titleBand, "NameText", definition.Name, 22, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            nameText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nameText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nameText.rectTransform.offsetMin = new Vector2(12f, -34f);
            nameText.rectTransform.offsetMax = new Vector2(-12f, -8f);

            Text styleText = CreateText(titleBand, "StyleText", GetPathChoiceStyleTitle(definition.Style), 13, FontStyle.Bold, TextAnchor.LowerCenter, accent);
            styleText.rectTransform.anchorMin = new Vector2(0f, 0f);
            styleText.rectTransform.anchorMax = new Vector2(1f, 0f);
            styleText.rectTransform.offsetMin = new Vector2(10f, 8f);
            styleText.rectTransform.offsetMax = new Vector2(-10f, 28f);

            RectTransform infoBand = CreateFixedPanel(
                previewObject.transform,
                "InfoBand",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 16f),
                new Vector2(202f, 102f),
                new Color(0.08f, 0.09f, 0.10f, 0.78f));
            DecorateFrame(infoBand, new Color(accent.r, accent.g, accent.b, 0.54f), new Color(1f, 1f, 1f, 0.02f), 1.5f, false);

            string[] keywords = GetPathChoiceKeywords(definition.Style);
            int previewKeywordCount = Mathf.Min(2, keywords.Length);
            for (int i = 0; i < previewKeywordCount; i++)
            {
                Text featureText = CreateText(infoBand, "PreviewFeature_" + i, "· " + keywords[i], 12, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.92f, 0.92f, 0.88f, 0.98f));
                featureText.rectTransform.anchorMin = new Vector2(0f, 1f);
                featureText.rectTransform.anchorMax = new Vector2(1f, 1f);
                featureText.rectTransform.offsetMin = new Vector2(14f, -28f - i * 22f);
                featureText.rectTransform.offsetMax = new Vector2(-12f, -8f - i * 22f);
            }

            Text previewHint = CreateText(infoBand, "PreviewHint", "后续奖励向该脉倾斜", 11, FontStyle.Normal, TextAnchor.LowerCenter, ColorGoldDim);
            previewHint.rectTransform.anchorMin = new Vector2(0f, 0f);
            previewHint.rectTransform.anchorMax = new Vector2(1f, 0f);
            previewHint.rectTransform.offsetMin = new Vector2(12f, 8f);
            previewHint.rectTransform.offsetMax = new Vector2(-12f, 22f);

            return previewObject;
        }

        private static Sprite LoadPathChoiceSprite(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    if (cachedWanjianPathChoiceSprite == null)
                    {
                        cachedWanjianPathChoiceSprite = LoadSpriteResource(PathChoiceWanjianResourcePath, PathChoiceWanjianFallbackResourcePath);
                    }

                    return cachedWanjianPathChoiceSprite;
                case DemoSwordStyle.Thunder:
                    if (cachedThunderPathChoiceSprite == null)
                    {
                        cachedThunderPathChoiceSprite = LoadSpriteResource(PathChoiceThunderResourcePath, PathChoiceThunderFallbackResourcePath);
                    }

                    return cachedThunderPathChoiceSprite;
                case DemoSwordStyle.Blood:
                    if (cachedBloodPathChoiceSprite == null)
                    {
                        cachedBloodPathChoiceSprite = LoadSpriteResource(PathChoiceBloodResourcePath);
                    }

                    return cachedBloodPathChoiceSprite;
                default:
                    return null;
            }
        }

        private static Sprite LoadHeaderCloudBandSprite()
        {
            if (cachedHeaderCloudBandSprite == null)
            {
                cachedHeaderCloudBandSprite = LoadSpriteResource(HeaderCloudBandResourcePath);
            }

            return cachedHeaderCloudBandSprite;
        }

        private static Sprite LoadSpriteResource(params string[] resourcePaths)
        {
            if (resourcePaths == null)
            {
                return null;
            }

            for (int i = 0; i < resourcePaths.Length; i++)
            {
                if (string.IsNullOrEmpty(resourcePaths[i]))
                {
                    continue;
                }

                Texture2D texture = Resources.Load<Texture2D>(resourcePaths[i]);
                if (texture == null)
                {
                    continue;
                }

                return Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100f);
            }

            return null;
        }

        private void BuildPathChoiceStage(RectTransform parent, DemoGongfaDefinition definition, Color accent, DemoPathChoiceCardFx fx)
        {
            CreateStretchPanel(
                parent,
                "InkBackdrop",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(12f, 12f),
                new Vector2(-12f, -12f),
                new Color(0f, 0f, 0f, 0.12f));

            RectTransform washA = CreateFixedPanel(
                parent,
                "WashA",
                new Vector2(0.32f, 0.62f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(172f, 120f),
                new Color(accent.r, accent.g, accent.b, 0.08f));
            RectTransform washB = CreateFixedPanel(
                parent,
                "WashB",
                new Vector2(0.68f, 0.34f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(188f, 108f),
                new Color(0f, 0f, 0f, 0.10f));

            Image washAImage = washA.GetComponent<Image>();
            Image washBImage = washB.GetComponent<Image>();
            fx.Register(washA, new Vector2(6f, 5f), 0.72f, 0.3f, 3f, 0.02f, washAImage, 0.04f);
            fx.Register(washB, new Vector2(5f, 4f), 0.58f, 1.1f, -2f, 0.02f, washBImage, 0.03f);

            Text glyphText = CreateText(parent, "GlyphText", definition.IconGlyph, 84, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(accent.r, accent.g, accent.b, 0.92f));
            glyphText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            glyphText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            glyphText.rectTransform.anchoredPosition = new Vector2(0f, 6f);
            glyphText.rectTransform.sizeDelta = new Vector2(140f, 140f);
            fx.Register(glyphText.rectTransform, new Vector2(0f, 5f), 0.84f, 0.6f, 0f, 0.02f, glyphText, 0.03f);

            Text subGlyphText = CreateText(parent, "SubGlyphText", GetPathChoiceSubGlyph(definition.Style), 26, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.16f, 0.16f, 0.15f, 0.78f));
            subGlyphText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            subGlyphText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            subGlyphText.rectTransform.anchoredPosition = new Vector2(0f, -66f);
            subGlyphText.rectTransform.sizeDelta = new Vector2(180f, 32f);

            switch (definition.Style)
            {
                case DemoSwordStyle.Wanjian:
                    BuildSwordStageFx(parent, accent, fx);
                    break;
                case DemoSwordStyle.Thunder:
                    BuildThunderStageFx(parent, accent, fx);
                    break;
                case DemoSwordStyle.Blood:
                    BuildBloodStageFx(parent, accent, fx);
                    break;
            }
        }

        private void BuildSwordStageFx(RectTransform parent, Color accent, DemoPathChoiceCardFx fx)
        {
            for (int i = 0; i < 3; i++)
            {
                RectTransform sword = CreateFixedPanel(
                    parent,
                    "Sword_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-62f + i * 44f, 22f - i * 18f),
                    new Vector2(18f, 142f),
                    new Color(0.16f, 0.16f, 0.17f, 0.68f));
                sword.localRotation = Quaternion.Euler(0f, 0f, -22f + i * 11f);
                RectTransform glow = CreateStretchPanel(
                    sword,
                    "SwordGlow",
                    new Vector2(0f, 0.08f),
                    new Vector2(1f, 0.92f),
                    new Vector2(5f, 0f),
                    new Vector2(-5f, 0f),
                    new Color(accent.r, accent.g, accent.b, 0.42f));
                fx.Register(sword, new Vector2(0f, 6f + i), 0.72f + i * 0.08f, i * 0.7f, 3f - i, 0.01f, glow.GetComponent<Image>(), 0.04f);
            }

            RectTransform arc = CreateFixedPanel(
                parent,
                "SwordArc",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(42f, 12f),
                new Vector2(132f, 16f),
                new Color(accent.r, accent.g, accent.b, 0.18f));
            arc.localRotation = Quaternion.Euler(0f, 0f, 18f);
            fx.Register(arc, new Vector2(4f, 3f), 0.64f, 1.2f, 4f, 0.02f, arc.GetComponent<Image>(), 0.05f);
        }

        private void BuildThunderStageFx(RectTransform parent, Color accent, DemoPathChoiceCardFx fx)
        {
            for (int i = 0; i < 3; i++)
            {
                RectTransform bolt = CreateFixedPanel(
                    parent,
                    "Bolt_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-70f + i * 48f, 20f - i * 12f),
                    new Vector2(16f, 126f),
                    new Color(accent.r, accent.g, accent.b, 0.34f));
                bolt.localRotation = Quaternion.Euler(0f, 0f, -26f + i * 15f);
                fx.Register(bolt, new Vector2(0f, 8f), 0.94f + i * 0.1f, i * 0.6f, 7f, 0.03f, bolt.GetComponent<Image>(), 0.10f);
            }

            for (int i = 0; i < 4; i++)
            {
                RectTransform spark = CreateFixedPanel(
                    parent,
                    "Spark_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-84f + i * 58f, -30f + (i % 2) * 18f),
                    new Vector2(10f, 10f),
                    new Color(accent.r, accent.g, accent.b, 0.62f));
                fx.Register(spark, new Vector2(2f + i, 6f + i), 1.08f + i * 0.12f, 0.4f * i, 0f, 0.06f, spark.GetComponent<Image>(), 0.18f);
            }
        }

        private void BuildBloodStageFx(RectTransform parent, Color accent, DemoPathChoiceCardFx fx)
        {
            for (int i = 0; i < 2; i++)
            {
                RectTransform ribbon = CreateFixedPanel(
                    parent,
                    "Ribbon_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-48f + i * 82f, 4f - i * 24f),
                    new Vector2(24f, 152f),
                    new Color(accent.r, accent.g, accent.b, 0.26f));
                ribbon.localRotation = Quaternion.Euler(0f, 0f, -30f + i * 22f);
                fx.Register(ribbon, new Vector2(2f, 10f), 0.70f + i * 0.12f, i * 0.9f, 8f - i * 3f, 0.03f, ribbon.GetComponent<Image>(), 0.08f);
            }

            for (int i = 0; i < 4; i++)
            {
                RectTransform droplet = CreateFixedPanel(
                    parent,
                    "Droplet_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-66f + i * 42f, -42f + (i % 2) * 18f),
                    new Vector2(12f + (i % 2) * 4f, 18f + (i % 2) * 4f),
                    new Color(accent.r, accent.g, accent.b, 0.48f));
                fx.Register(droplet, new Vector2(2f, 7f + i), 0.88f + i * 0.08f, i * 0.4f, 0f, 0.04f, droplet.GetComponent<Image>(), 0.10f);
            }
        }

        private static string BuildRewardPreviewText(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Route && reward.RoutePlan != null)
            {
                return BuildRoutePreviewText(reward);
            }

            if (string.IsNullOrEmpty(reward.Description))
            {
                return "补强当前道途。";
            }

            string[] split = reward.Description.Split('|');
            if (split.Length > 1)
            {
                return $"{split[0].Trim()}\n{BuildCompactLine(split[1].Trim())}";
            }

            return BuildCompactLine(reward.Description);
        }

        private void AddRewardHoverEvents(GameObject rewardObject, DemoReward reward)
        {
            EventTrigger trigger = rewardObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = rewardObject.AddComponent<EventTrigger>();
            }

            trigger.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener(_ => ShowRewardDetail(reward));
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener(_ => HideRewardDetail());
            trigger.triggers.Add(exitEntry);
        }

        private void ShowRewardDetail(DemoReward reward)
        {
            if (rewardDetailPanel == null)
            {
                return;
            }

            rewardDetailTitleText.text = BuildRewardDetailTitle(reward);
            rewardDetailBodyText.text = BuildRewardDetailText(reward);
            rewardDetailPanel.SetActive(true);
        }

        private void HideRewardDetail()
        {
            if (rewardDetailPanel == null)
            {
                return;
            }

            rewardDetailPanel.SetActive(false);
        }

        private string BuildRewardDetailTitle(DemoReward reward)
        {
            return $"{GetRewardTypeLabel(reward)} | {reward.Name}";
        }

        private string BuildRewardDetailText(DemoReward reward)
        {
            List<string> lines = new List<string>();
            lines.Add($"定位：{GetRewardCommercialTag(reward)}");
            lines.Add($"品阶：{GetQualityLongLabel(GetRewardQuality(reward))}");
            lines.Add(string.Empty);

            if (reward.Type == DemoRewardType.Card && reward.Card != null)
            {
                lines.Add($"类型：{GetCardTypeLabel(reward.Card.Type)}");
                lines.Add($"方向：{GetStyleLabel(reward.Card.Style)}");
                lines.Add($"费用：{reward.Card.Cost} 气");
                lines.Add(string.Empty);
                lines.Add(BuildCardDetailText(reward.Card));
            }
            else if (reward.Type == DemoRewardType.Gongfa)
            {
                DemoGongfaDefinition definition = DemoGongfaLibrary.Get(reward.GongfaType);
                lines.Add($"方向：{GetStyleLabel(definition.Style)}");
                lines.Add($"槽位：{GetGongfaSlotLabel(definition.Slot)}");
                lines.Add(string.Empty);
                lines.Add(definition.Description);
            }
            else if (reward.Type == DemoRewardType.Artifact)
            {
                DemoArtifactDefinition definition = DemoArtifactLibrary.Get(reward.ArtifactType);
                lines.Add($"方向：{definition.Style}");
                lines.Add(string.Empty);
                lines.Add(definition.Description);
            }
            else if (reward.Type == DemoRewardType.Route && reward.RoutePlan != null)
            {
                lines.Add($"倾向：{GetStyleLabel(reward.RouteStyle)}");
                lines.Add($"路数：{(string.IsNullOrEmpty(reward.RouteTag) ? "路线分叉" : reward.RouteTag)}");
                lines.Add(string.Empty);
                lines.Add(reward.Description);
                lines.Add(string.Empty);
                lines.Add("下一段节点：");

                for (int i = 0; i < reward.RoutePlan.Nodes.Count; i++)
                {
                    DemoMapNode node = reward.RoutePlan.Nodes[i];
                    lines.Add($"{i + 1}. {GetRouteNodeLabel(node)}");
                }
            }
            else
            {
                lines.Add(reward.Description);
            }

            return string.Join("\n", lines);
        }

        private static string BuildCardDetailText(DemoCard card)
        {
            string rules = card.GetRulesText();
            return string.IsNullOrEmpty(rules) ? "作为当前构筑的节奏补件。" : rules;
        }

        private static string BuildRoutePreviewText(DemoReward reward)
        {
            string tag = string.IsNullOrEmpty(reward.RouteTag) ? "路线分叉" : reward.RouteTag;
            return $"{tag}\n{BuildRouteNodePreview(reward.RoutePlan, 3)}";
        }

        private static string BuildRouteNodePreview(DemoMapRoutePlan routePlan, int maxNodes)
        {
            if (routePlan == null || routePlan.Nodes.Count == 0)
            {
                return "立即进入下一段历练";
            }

            int previewCount = Mathf.Min(maxNodes, routePlan.Nodes.Count);
            string preview = string.Join(" -> ", routePlan.Nodes.Take(previewCount).Select(GetRouteNodeShortLabel));
            return routePlan.Nodes.Count > previewCount ? preview + " -> ..." : preview;
        }

        private static string GetRouteNodeLabel(DemoMapNode node)
        {
            if (node == null)
            {
                return "未知节点";
            }

            return $"{GetRouteNodeShortLabel(node)} - {node.Name}";
        }

        private static string GetRouteNodeShortLabel(DemoMapNode node)
        {
            if (node == null)
            {
                return "节点";
            }

            switch (node.Type)
            {
                case DemoNodeType.RouteChoice:
                    return "路口";
                case DemoNodeType.Battle:
                    return "战斗";
                case DemoNodeType.Reward:
                    return "奖励";
                case DemoNodeType.Training:
                    return "修炼";
                case DemoNodeType.Shop:
                    return "整备";
                case DemoNodeType.Boss:
                    return "Boss";
                case DemoNodeType.Victory:
                    return "结算";
                default:
                    return "节点";
            }
        }

        private static string BuildCompactLine(string text)
        {
            return text
                .Replace("，", "  ·  ")
                .Replace("。", string.Empty);
        }

        private static string GetGongfaSlotLabel(DemoGongfaSlot slot)
        {
            switch (slot)
            {
                case DemoGongfaSlot.Main:
                    return "主修";
                case DemoGongfaSlot.Support:
                    return "辅修";
                case DemoGongfaSlot.Divine:
                    return "神通";
                default:
                    return "功法";
            }
        }

        private static string BuildPathChoiceHook(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "飞剑铺场，剑意成潮";
                case DemoSwordStyle.Thunder:
                    return "感电连锁，一轮爆开";
                case DemoSwordStyle.Blood:
                    return "残血搏命，越战越凶";
                default:
                    return "先定方向，再围绕这一脉拿补强。";
            }
        }

        private static string[] GetPathChoiceKeywords(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new[] { "飞剑越打越多", "剑意叠起后持续压场", "适合中后期满屏剑潮" };
                case DemoSwordStyle.Thunder:
                    return new[] { "飞剑多段引爆感电", "连锁雷击追伤", "适合一轮收束爆发" };
                case DemoSwordStyle.Blood:
                    return new[] { "流血越深伤害越高", "吸血稳住续战", "危险血线换更高斩杀" };
                default:
                    return new[] { "起手方向", "构筑收束", "后续补强" };
            }
        }

        private static string BuildPathChoiceSummary(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "适合喜欢先铺资源，再靠演武阶段自然滚起整轮场面的路线。";
                case DemoSwordStyle.Thunder:
                    return "适合把节奏压到同一轮里，追求感电与雷击的瞬时连锁高点。";
                case DemoSwordStyle.Blood:
                    return "适合用风险换上限，越靠近危险线，演武斩杀就越有压迫感。";
                default:
                    return "选定后，后续奖励会优先朝这个方向倾斜。";
            }
        }

        private static string GetPathChoiceStyleTitle(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "万剑道";
                case DemoSwordStyle.Thunder:
                    return "雷剑道";
                case DemoSwordStyle.Blood:
                    return "血剑道";
                default:
                    return "通用道途";
            }
        }

        private static string GetPathChoiceSubGlyph(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "剑势先起";
                case DemoSwordStyle.Thunder:
                    return "雷机待发";
                case DemoSwordStyle.Blood:
                    return "血煞凝锋";
                default:
                    return "定下方向";
            }
        }

        private RectTransform CreateVerticalContent(RectTransform parent, int horizontalPadding, int verticalPadding, float spacing)
        {
            GameObject contentObject = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup));
            contentObject.transform.SetParent(parent, false);

            RectTransform rect = contentObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            VerticalLayoutGroup layoutGroup = contentObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(horizontalPadding, horizontalPadding, verticalPadding, verticalPadding);
            layoutGroup.spacing = spacing;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            return rect;
        }

        private Text CreateSectionTitle(Transform parent, string title, int fontSize, Color color)
        {
            Text text = CreateText(parent, "Title", title, fontSize, FontStyle.Bold, TextAnchor.UpperLeft, color);
            LayoutElement layout = text.gameObject.AddComponent<LayoutElement>();
            layout.preferredHeight = fontSize + 8f;
            return text;
        }

        private Text CreateBodyText(Transform parent, string name, int fontSize, Color color)
        {
            Text text = CreateText(parent, name, string.Empty, fontSize, FontStyle.Normal, TextAnchor.UpperLeft, color);
            LayoutElement layout = text.gameObject.AddComponent<LayoutElement>();
            layout.flexibleHeight = 1f;
            return text;
        }

        private Button CreateActionButton(Transform parent, string name, out Text labelText, Color buttonColor, Color textColor)
        {
            GameObject buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonObject.transform.SetParent(parent, false);

            Image image = buttonObject.GetComponent<Image>();
            image.color = buttonColor;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(buttonColor);
            DecorateFrame(buttonObject.GetComponent<RectTransform>(), ColorGold, new Color(0.20f, 0.15f, 0.09f, 0.18f), 2f, true);

            CreateStretchPanel(
                buttonObject.transform,
                "ButtonGlow",
                new Vector2(0f, 0.55f),
                new Vector2(1f, 1f),
                new Vector2(8f, -4f),
                new Vector2(-8f, -8f),
                new Color(0.85f, 0.74f, 0.50f, 0.08f));

            labelText = CreateText(buttonObject.transform, "Label", string.Empty, 15, FontStyle.Bold, TextAnchor.MiddleCenter, textColor);
            StretchText(labelText.rectTransform, new Vector2(10f, 4f), new Vector2(-10f, -4f));

            return button;
        }

        private DemoBattleSceneView CreateBattleScene(Transform parent)
        {
            GameObject sceneObject = new GameObject("BattleSceneView", typeof(RectTransform), typeof(Image), typeof(LayoutElement), typeof(DemoBattleSceneView));
            sceneObject.transform.SetParent(parent, false);

            Image frame = sceneObject.GetComponent<Image>();
            frame.color = new Color(0.05f, 0.06f, 0.07f, 1f);
            DecorateFrame(sceneObject.GetComponent<RectTransform>(), ColorGoldDim, new Color(0.16f, 0.12f, 0.08f, 0.10f), 2f, true);

            LayoutElement layout = sceneObject.GetComponent<LayoutElement>();
            layout.preferredHeight = 440f;
            layout.flexibleHeight = 1f;

            DemoBattleSceneView sceneView = sceneObject.GetComponent<DemoBattleSceneView>();
            sceneView.Initialize(controller, uiFont);
            return sceneView;
        }

        private Text CreateText(Transform parent, string name, string textValue, int fontSize, FontStyle fontStyle, TextAnchor alignment, Color color)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);

            Text text = textObject.GetComponent<Text>();
            text.font = uiFont;
            text.fontSize = fontSize;
            text.fontStyle = fontStyle;
            text.alignment = alignment;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.color = color;
            text.text = textValue;
            text.raycastTarget = false;
            return text;
        }

        private static void ApplySpriteToImage(RectTransform rect, Sprite sprite, Color color)
        {
            if (rect == null || sprite == null)
            {
                return;
            }

            Image image = rect.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = color;
        }

        private RectTransform CreateFixedPanel(Transform parent, string name, Vector2 anchor, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta, Color color)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;

            Image image = panelObject.GetComponent<Image>();
            image.color = color;

            return rect;
        }

        private RectTransform CreateStretchPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            GameObject panelObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            panelObject.transform.SetParent(parent, false);

            RectTransform rect = panelObject.GetComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;

            Image image = panelObject.GetComponent<Image>();
            image.color = color;

            return rect;
        }

        private RectTransform CreateInkPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax, Color color)
        {
            RectTransform rect = CreateStretchPanel(parent, name, anchorMin, anchorMax, offsetMin, offsetMax, color);
            DecorateFrame(rect, ColorGoldDim, new Color(0.18f, 0.14f, 0.09f, 0.16f), 2f, true);
            return rect;
        }

        private RectTransform CreateFixedHeightBlock(Transform parent, string name, float height, Color color)
        {
            GameObject block = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            block.transform.SetParent(parent, false);
            Image image = block.GetComponent<Image>();
            image.color = color;
            LayoutElement layout = block.GetComponent<LayoutElement>();
            layout.preferredHeight = height;
            layout.minHeight = height;
            return block.GetComponent<RectTransform>();
        }

        private RectTransform CreateFixedSizeBlock(Transform parent, string name, Vector2 size, Color color)
        {
            GameObject block = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            block.transform.SetParent(parent, false);
            Image image = block.GetComponent<Image>();
            image.color = color;
            LayoutElement layout = block.GetComponent<LayoutElement>();
            layout.preferredWidth = size.x;
            layout.minWidth = size.x;
            layout.preferredHeight = size.y;
            layout.minHeight = size.y;
            return block.GetComponent<RectTransform>();
        }

        private RectTransform CreateLabelChip(Transform parent, string label, Color backgroundColor, Color textColor, float width)
        {
            GameObject chipObject = new GameObject("Chip_" + label, typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            chipObject.transform.SetParent(parent, false);
            Image image = chipObject.GetComponent<Image>();
            image.color = backgroundColor;

            LayoutElement layout = chipObject.GetComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.minWidth = width;
            layout.preferredHeight = 22f;
            layout.minHeight = 22f;

            RectTransform rect = chipObject.GetComponent<RectTransform>();
            DecorateFrame(rect, textColor * 0.8f, new Color(0f, 0f, 0f, 0f), 1f, false);

            Text text = CreateText(chipObject.transform, "ChipLabel", label, 11, FontStyle.Bold, TextAnchor.MiddleCenter, textColor);
            StretchText(text.rectTransform, new Vector2(6f, 2f), new Vector2(-6f, -2f));
            return rect;
        }

        private void DecorateFrame(RectTransform parent, Color borderColor, Color innerWashColor, float thickness, bool addCorners)
        {
            if (innerWashColor.a > 0f)
            {
                RectTransform wash = CreateStretchPanel(parent, "InnerWash", new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(4f, 4f), new Vector2(-4f, -4f), innerWashColor);
                IgnoreLayout(wash.gameObject);
            }

            IgnoreLayout(CreateStretchPanel(parent, "BorderTop", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(thickness, -thickness), new Vector2(-thickness, 0f), borderColor).gameObject);
            IgnoreLayout(CreateStretchPanel(parent, "BorderBottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(thickness, 0f), new Vector2(-thickness, thickness), borderColor).gameObject);
            IgnoreLayout(CreateStretchPanel(parent, "BorderLeft", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, thickness), new Vector2(thickness, -thickness), borderColor).gameObject);
            IgnoreLayout(CreateStretchPanel(parent, "BorderRight", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-thickness, thickness), new Vector2(0f, -thickness), borderColor).gameObject);

            if (!addCorners)
            {
                return;
            }

            AddCornerMark(parent, "TL", new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(18f, 18f), borderColor);
            AddCornerMark(parent, "TR", new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, 0f), new Vector2(18f, 18f), borderColor);
            AddCornerMark(parent, "BL", new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, -18f), new Vector2(18f, 18f), borderColor);
            AddCornerMark(parent, "BR", new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-18f, -18f), new Vector2(18f, 18f), borderColor);
        }

        private void AddCornerMark(RectTransform parent, string suffix, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size, Color color)
        {
            RectTransform horizontal = CreateStretchPanel(parent, "CornerH_" + suffix, anchorMin, anchorMax, anchoredPosition, anchoredPosition + new Vector2(size.x, 2f), color);
            RectTransform vertical = CreateStretchPanel(parent, "CornerV_" + suffix, anchorMin, anchorMax, anchoredPosition, anchoredPosition + new Vector2(2f, size.y), color);
            IgnoreLayout(horizontal.gameObject);
            IgnoreLayout(vertical.gameObject);
        }

        private void StretchText(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void IgnoreLayout(GameObject gameObject)
        {
            LayoutElement layout = gameObject.GetComponent<LayoutElement>();
            if (layout == null)
            {
                layout = gameObject.AddComponent<LayoutElement>();
            }

            layout.ignoreLayout = true;
        }

        private void ClearEntries(List<GameObject> entries)
        {
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i] != null)
                {
                    Destroy(entries[i]);
                }
            }

            entries.Clear();
        }

        private static string GetStyleLabel(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "万剑";
                case DemoSwordStyle.Thunder:
                    return "雷剑";
                case DemoSwordStyle.Blood:
                    return "血剑";
                default:
                    return "通用";
            }
        }

        private static string GetCardTypeLabel(DemoCardType type)
        {
            switch (type)
            {
                case DemoCardType.Attack:
                    return "攻击";
                case DemoCardType.FlyingSword:
                    return "飞剑";
                case DemoCardType.Status:
                    return "状态";
                case DemoCardType.Defense:
                    return "防御";
                case DemoCardType.Resource:
                    return "资源";
                case DemoCardType.Finisher:
                    return "终结";
                default:
                    return type.ToString();
            }
        }

        private static Color GetCardColor(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new Color(0.13f, 0.20f, 0.27f, 0.98f);
                case DemoSwordStyle.Thunder:
                    return new Color(0.18f, 0.16f, 0.30f, 0.98f);
                case DemoSwordStyle.Blood:
                    return new Color(0.31f, 0.15f, 0.16f, 0.98f);
                default:
                    return new Color(0.18f, 0.19f, 0.22f, 0.98f);
            }
        }

        private static Color GetCardAccentColor(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new Color(0.78f, 0.72f, 0.60f, 0.82f);
                case DemoSwordStyle.Thunder:
                    return new Color(0.52f, 0.72f, 1f, 0.82f);
                case DemoSwordStyle.Blood:
                    return new Color(0.84f, 0.36f, 0.34f, 0.82f);
                default:
                    return new Color(0.70f, 0.67f, 0.58f, 0.72f);
            }
        }

        private static Color GetPathChoicePanelColor(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new Color(0.92f, 0.92f, 0.90f, 0.94f);
                case DemoSwordStyle.Thunder:
                    return new Color(0.90f, 0.92f, 0.95f, 0.94f);
                case DemoSwordStyle.Blood:
                    return new Color(0.95f, 0.90f, 0.90f, 0.94f);
                default:
                    return new Color(0.92f, 0.92f, 0.90f, 0.94f);
            }
        }

        private static ColorBlock CreateButtonColors(Color baseColor)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseColor * 1.08f;
            colors.pressedColor = baseColor * 0.92f;
            colors.selectedColor = baseColor * 1.04f;
            colors.disabledColor = new Color(0.17f, 0.17f, 0.18f, 0.72f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            return colors;
        }

        private static string GetCardMoodText(DemoCard card)
        {
            switch (card.Type)
            {
                case DemoCardType.FlyingSword:
                    return "飞剑起势";
                case DemoCardType.Finisher:
                    return "收束杀招";
                case DemoCardType.Status:
                    return "气机铺陈";
                case DemoCardType.Defense:
                    return "回护灵台";
                case DemoCardType.Resource:
                    return "调息转气";
                default:
                    return "剑意出手";
            }
        }

        private static string GetRewardTypeLabel(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return "路线";
                case DemoRewardType.Card:
                    return "新牌";
                case DemoRewardType.Gongfa:
                    return "功法";
                case DemoRewardType.Artifact:
                    return "法宝";
                case DemoRewardType.Relic:
                    return "遗物";
                case DemoRewardType.Upgrade:
                    return "精修";
                case DemoRewardType.Heal:
                    return "调息";
                default:
                    return "补强";
            }
        }

        private static string GetRewardFlavorText(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return string.IsNullOrEmpty(reward.RouteTag) ? "路线分叉" : reward.RouteTag;
                case DemoRewardType.Card:
                    return "战斗件";
                case DemoRewardType.Gongfa:
                    return "长期被动";
                case DemoRewardType.Artifact:
                    return "规则改写";
                case DemoRewardType.Relic:
                    return "局内成长";
                case DemoRewardType.Upgrade:
                    return "灵气上限";
                case DemoRewardType.Heal:
                    return "续航恢复";
                default:
                    return "道途补强";
            }
        }

        private static string GetRewardCommercialTag(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return "节点节奏";
                case DemoRewardType.Card:
                    return "即战力";
                case DemoRewardType.Gongfa:
                    return "长期成型";
                case DemoRewardType.Artifact:
                    return "规则拐点";
                case DemoRewardType.Relic:
                    return "稳定成长";
                case DemoRewardType.Upgrade:
                    return "资源扩容";
                case DemoRewardType.Heal:
                    return "续航修整";
                default:
                    return "可立即收束";
            }
        }

        private static DemoQuality GetRewardQuality(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return reward.RouteQuality;
                case DemoRewardType.Card:
                    return reward.Card != null ? reward.Card.Quality : DemoQuality.Mortal;
                case DemoRewardType.Gongfa:
                    return DemoGongfaLibrary.Get(reward.GongfaType).Quality;
                case DemoRewardType.Artifact:
                    return DemoArtifactLibrary.Get(reward.ArtifactType).Quality;
                case DemoRewardType.Relic:
                    return DemoQuality.Earth;
                case DemoRewardType.Upgrade:
                    return DemoQuality.Mysterious;
                case DemoRewardType.Heal:
                    return DemoQuality.Spirit;
                default:
                    return DemoQuality.Mortal;
            }
        }

        private static string GetRewardGlyph(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return string.IsNullOrEmpty(reward.RouteGlyph) ? "路" : reward.RouteGlyph;
                case DemoRewardType.Card:
                    return reward.Card != null ? GetCardIconGlyph(reward.Card) : "牌";
                case DemoRewardType.Gongfa:
                    return DemoGongfaLibrary.Get(reward.GongfaType).IconGlyph;
                case DemoRewardType.Artifact:
                    return DemoArtifactLibrary.Get(reward.ArtifactType).IconGlyph;
                case DemoRewardType.Relic:
                    return "遗";
                case DemoRewardType.Upgrade:
                    return "修";
                case DemoRewardType.Heal:
                    return "息";
                default:
                    return "道";
            }
        }

        private static Color GetRewardAccentColor(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                    return GetCardAccentColor(reward.RouteStyle);
                case DemoRewardType.Gongfa:
                    return ColorGold;
                case DemoRewardType.Artifact:
                    return ColorJade;
                case DemoRewardType.Relic:
                    return new Color(0.76f, 0.47f, 0.32f, 1f);
                case DemoRewardType.Upgrade:
                    return new Color(0.83f, 0.72f, 0.40f, 1f);
                case DemoRewardType.Heal:
                    return new Color(0.49f, 0.71f, 0.51f, 1f);
                default:
                    return ColorMist;
            }
        }

        private static Color GetRewardPanelColor(DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Route:
                {
                    Color routeColor = GetCardColor(reward.RouteStyle);
                    return new Color(routeColor.r * 0.92f, routeColor.g * 0.92f, routeColor.b * 0.92f, 0.98f);
                }
                case DemoRewardType.Gongfa:
                    return new Color(0.17f, 0.14f, 0.09f, 0.98f);
                case DemoRewardType.Artifact:
                    return new Color(0.09f, 0.16f, 0.17f, 0.98f);
                case DemoRewardType.Relic:
                    return new Color(0.18f, 0.11f, 0.09f, 0.98f);
                case DemoRewardType.Upgrade:
                    return new Color(0.17f, 0.15f, 0.09f, 0.98f);
                case DemoRewardType.Heal:
                    return new Color(0.09f, 0.15f, 0.11f, 0.98f);
                default:
                    return new Color(0.11f, 0.12f, 0.15f, 0.98f);
            }
        }

        private static string GetCardIconGlyph(DemoCard card)
        {
            return string.IsNullOrEmpty(card.IconGlyph) ? "诀" : card.IconGlyph;
        }

        private static string GetQualityLabel(DemoQuality quality)
        {
            switch (quality)
            {
                case DemoQuality.Spirit:
                    return "灵";
                case DemoQuality.Mysterious:
                    return "玄";
                case DemoQuality.Earth:
                    return "地";
                case DemoQuality.Heaven:
                    return "天";
                case DemoQuality.Immortal:
                    return "仙";
                default:
                    return "凡";
            }
        }

        private static string GetQualityLongLabel(DemoQuality quality)
        {
            switch (quality)
            {
                case DemoQuality.Spirit:
                    return "灵品";
                case DemoQuality.Mysterious:
                    return "玄品";
                case DemoQuality.Earth:
                    return "地品";
                case DemoQuality.Heaven:
                    return "天品";
                case DemoQuality.Immortal:
                    return "仙品";
                default:
                    return "凡品";
            }
        }

        private static Color GetQualityColor(DemoQuality quality)
        {
            switch (quality)
            {
                case DemoQuality.Spirit:
                    return new Color(0.42f, 0.74f, 0.46f, 1f);
                case DemoQuality.Mysterious:
                    return new Color(0.39f, 0.67f, 0.92f, 1f);
                case DemoQuality.Earth:
                    return new Color(0.64f, 0.43f, 0.90f, 1f);
                case DemoQuality.Heaven:
                    return new Color(0.85f, 0.72f, 0.44f, 1f);
                case DemoQuality.Immortal:
                    return new Color(0.86f, 0.42f, 0.32f, 1f);
                default:
                    return new Color(0.72f, 0.72f, 0.72f, 1f);
            }
        }

        private static string FormatGongfaSummary(DemoGongfaType type, string fallback)
        {
            if (type == DemoGongfaType.None)
            {
                return fallback;
            }

            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(type);
            return $"{GetQualityLabel(definition.Quality)} {definition.IconGlyph}·{definition.Name}";
        }
    }
}
