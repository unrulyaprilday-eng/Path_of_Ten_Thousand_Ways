using System.Collections.Generic;
using System.Linq;
#if ENABLE_INPUT_SYSTEM
using System.Reflection;
#endif
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;
using UnityEngine;
using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif
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
        private static readonly Color HomeButtonInk = new Color(0.16f, 0.15f, 0.12f, 0.98f);
        private static readonly Color HomeButtonInkMuted = new Color(0.31f, 0.30f, 0.25f, 0.86f);
        private static readonly Color HomeButtonPrimaryBase = new Color(0.91f, 0.83f, 0.63f, 0.62f);
        private static readonly Color HomeButtonSecondaryBase = new Color(0.88f, 0.84f, 0.73f, 0.34f);
        private static readonly Color HomeButtonPrimaryEdge = new Color(0.56f, 0.39f, 0.17f, 0.50f);
        private static readonly Color HomeButtonSecondaryEdge = new Color(0.38f, 0.43f, 0.36f, 0.22f);
        private const string HeaderCloudBandResourcePath = "Art/UI/ui_header_cloudband_001";
        private const string PathChoiceWanjianResourcePath = "Art/UI/ui_path_wanjian_001";
        private const string PathChoiceWanjianFallbackResourcePath = "Art/UI/ui_path_wanjian_002";
        private const string PathChoiceThunderResourcePath = "Art/UI/ui_path_thunder_001";
        private const string PathChoiceThunderFallbackResourcePath = "Art/UI/ui_path_thunder_003";
        private const string PathChoiceBloodResourcePath = "Art/UI/ui_path_blood_001";
        private const string OpeningItemSwordcaseResourcePath = "Art/UI/ui_opening_item_swordcase_001";
        private const string OpeningItemThunderboneResourcePath = "Art/UI/ui_opening_item_thunderbone_001";
        private const string OpeningItemBloodjadeResourcePath = "Art/UI/ui_opening_item_bloodjade_001";
        private const string OpeningItemProtectiveJadeResourcePath = "Art/UI/ui_opening_item_protective_jade_001";
        private const string OpeningItemEmberTalismanCaseResourcePath = "Art/UI/ui_opening_item_ember_talisman_case_001";
        private const string OpeningItemAncestralCasketResourcePath = "Art/UI/ui_opening_item_ancestral_casket_001";
        private const string OpeningSceneTradeRoadResourcePath = "Art/UI/ui_opening_scene_trade_road_001";
        private const string OpeningSceneOldMineResourcePath = "Art/UI/ui_opening_scene_old_mine_001";
        private const string OpeningSceneThunderMarshResourcePath = "Art/UI/ui_opening_scene_thunder_marsh_001";
        private const string OpeningSceneHerbForestResourcePath = "Art/UI/ui_opening_scene_herb_forest_001";
        private const string OpeningSceneAncestralVaultResourcePath = "Art/UI/ui_opening_scene_ancestral_vault_001";
        private const string OpeningSceneDemonTowerResourcePath = "Art/UI/ui_opening_scene_demon_tower_001";
        private const string OpeningSceneBackdropResourcePath = "Art/UI/ui_opening_scene_select_backdrop_001";
        private const string HomeHeroInkResourcePath = "Art/UI/ui_home_hero_ink_002";
        private const string HomeHeroInkFallbackResourcePath = "Art/UI/ui_home_hero_ink_001";
        private const string HomePrimaryButtonResourcePath = "Art/UI/ui_btn_home_primary_001";
        private const string HomeSecondaryButtonResourcePath = "Art/UI/ui_btn_home_secondary_001";
        private const string HomeTitleCalligraphyResourcePath = "Art/UI/ui_home_title_calligraphy_001";
        private const string HomeLogoSealResourcePath = "Art/UI/ui_home_logo_seal_001";
        private const string HomeInfoTagResourcePath = "Art/UI/ui_home_info_tag_001";
        private const string HomeIconCodexResourcePath = "Art/UI/ui_icon_codex_001";
        private const string HomeIconSettingsResourcePath = "Art/UI/ui_icon_settings_001";
        private const string HomeIconExitResourcePath = "Art/UI/ui_icon_exit_001";
        private const string PanelScrollDarkResourcePath = "Art/UI/ui_panel_scroll_dark_001";
        private const string PanelScrollDarkTransparentResourcePath = "Art/UI/ui_panel_scroll_dark_001_transparent";
        private const string BattleHeaderRibbonResourcePath = "Art/UI/ui_battle_header_ribbon_001";


        private const string BattleIntentPlateResourcePath = "Art/UI/ui_battle_intent_plate_001";
        private const string BattleCardFrameResourcePath = "Art/UI/ui_battle_card_frame_001";
        private const string BattleActionButtonResourcePath = "Art/UI/ui_battle_action_button_001";
        private const string TransitionInkWipeResourcePath = "Art/UI/ui_transition_ink_wipe_001";
        private const string TransitionFocusRingResourcePath = "Art/UI/ui_transition_focus_ring_001";
        private const string RootDestinySceneTeaDeskResourcePath = "Art/UI/ui_root_destiny_scene_tea_desk_002";
        private const string RootDestinyDeskResourcePath = "Art/UI/ui_root_destiny_desk_bg_001";
        private const string RootDestinyBackdropResourcePath = "Art/UI/ui_root_destiny_backdrop_001";
        private const string RootObjectServantResourcePath = "Art/UI/ui_root_object_servant_001";
        private const string RootObjectSmithResourcePath = "Art/UI/ui_root_object_smith_001";
        private const string RootObjectCaravanResourcePath = "Art/UI/ui_root_object_caravan_001";
        private const string RootObjectBranchResourcePath = "Art/UI/ui_root_object_branch_001";
        private const string RootLotTagPaperResourcePath = "Art/UI/ui_root_lot_tag_002";
        private const string RootLotTagResourcePath = "Art/UI/ui_root_lot_tag_001";
        private const string RootConfirmSealCinnabarResourcePath = "Art/UI/ui_root_confirm_seal_002";
        private const string RootConfirmSealResourcePath = "Art/UI/ui_root_confirm_seal_001";
        private const string RootSmokeWispSoftResourcePath = "Art/UI/ui_root_smoke_wisp_002";
        private const string RootSmokeWispResourcePath = "Art/UI/ui_root_smoke_wisp_001";
        private const string SceneBattleCloudseaResourcePath = "Art/Scenes/scene_battle_cloudsea_001";
        private const string SceneCloudseaFarResourcePath = "Art/Scenes/scene_cloudsea_far_001";
        private const string SceneBattleOldMineEntryResourcePath = "Art/Scenes/scene_battle_old_mine_entry_001";
        private const string SceneBattleThunderMarshEntryResourcePath = "Art/Scenes/scene_battle_thunder_marsh_entry_001";
        private const float OpeningSceneTransitionDuration = 1.05f;
        private const float OpeningSceneTransitionCommitProgress = 0.76f;
        private static readonly string[] HomeTitleFontCandidates =
        {
            "STXingkai",
            "华文行楷",
            "STKaiti",
            "华文楷体",
            "KaiTi",
            "楷体",
            "DFKai-SB",
            "SimKai",
            "FangSong",
            "仿宋",
            "SimSun",
            "Microsoft YaHei"
        };

        private static Sprite cachedHeaderCloudBandSprite;
        private static Sprite cachedWanjianPathChoiceSprite;
        private static Sprite cachedThunderPathChoiceSprite;
        private static Sprite cachedBloodPathChoiceSprite;
        private static Sprite cachedOpeningItemSwordcaseSprite;
        private static Sprite cachedOpeningItemThunderboneSprite;
        private static Sprite cachedOpeningItemBloodjadeSprite;
        private static Sprite cachedOpeningItemProtectiveJadeSprite;
        private static Sprite cachedOpeningItemEmberTalismanCaseSprite;
        private static Sprite cachedOpeningItemAncestralCasketSprite;
        private static Sprite cachedOpeningSceneTradeRoadSprite;
        private static Sprite cachedOpeningSceneOldMineSprite;
        private static Sprite cachedOpeningSceneThunderMarshSprite;
        private static Sprite cachedOpeningSceneHerbForestSprite;
        private static Sprite cachedOpeningSceneAncestralVaultSprite;
        private static Sprite cachedOpeningSceneDemonTowerSprite;
        private static Sprite cachedOpeningSceneBackdropSprite;
        private static Sprite cachedHomeHeroInkSprite;
        private static Sprite cachedHomePrimaryButtonSprite;
        private static Sprite cachedHomeSecondaryButtonSprite;
        private static Sprite cachedHomeTitleCalligraphySprite;
        private static Sprite cachedHomeLogoSealSprite;
        private static Sprite cachedHomeInfoTagSprite;
        private static Sprite cachedHomeIconCodexSprite;
        private static Sprite cachedHomeIconSettingsSprite;
        private static Sprite cachedHomeIconExitSprite;
        private static Font cachedHomeTitleFont;
        private static Sprite cachedPanelScrollDarkSprite;
        private static Sprite cachedBattleHeaderRibbonSprite;

        private static Sprite cachedBattleIntentPlateSprite;
        private static Sprite cachedBattleCardFrameSprite;
        private static Sprite cachedBattleActionButtonSprite;
        private static Sprite cachedTransitionInkWipeSprite;
        private static Sprite cachedTransitionFocusRingSprite;
        private static Sprite cachedRootDestinyDeskSprite;
        private static Sprite cachedRootDestinyBackdropSprite;
        private static Sprite cachedRootObjectServantSprite;
        private static Sprite cachedRootObjectSmithSprite;
        private static Sprite cachedRootObjectCaravanSprite;
        private static Sprite cachedRootObjectBranchSprite;
        private static Sprite cachedRootLotTagSprite;
        private static Sprite cachedRootConfirmSealSprite;
        private static Sprite cachedRootSmokeWispSprite;
        private static Sprite cachedBattleCloudseaSprite;
        private static Sprite cachedCloudseaFarSprite;
        private static Sprite cachedRouteOldMineSprite;
        private static Sprite cachedRouteThunderMarshSprite;

        private readonly List<GameObject> handEntries = new List<GameObject>();
        private readonly List<GameObject> rewardEntries = new List<GameObject>();
        private readonly List<RectTransform> homeMistThreads = new List<RectTransform>();
        private readonly List<RectTransform> homeLightningStrokes = new List<RectTransform>();

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

        private Text handTitleText;
        private Text handInfoText;
        private Text handEmptyText;
        private Text battleFlowText;
        private Text battleDivineStateText;
        private Text battleArtifactStateText;

        private Text contextTitleText;
        private Text contextInfoText;
        private Text buildBodyText;
        private Text deckBodyText;

        private Button utilityButton;
        private Text utilityButtonText;
        private Button battleActionButton;
        private Text battleActionButtonText;

        private Button battleSpeedButton;
        private Text battleSpeedButtonText;
        private RectTransform utilityButtonRect;

        private RectTransform handContainer;
        private RectTransform rewardContainer;
        private HorizontalLayoutGroup rewardLayoutGroup;
        private CanvasGroup rewardOverlayCanvasGroup;
        private Button rewardRerollButton;
        private Text rewardRerollButtonText;
        private Button openingBackButton;
        private DemoBattleSceneView battleSceneView;
        private GameObject rewardPanelBody;
        private GameObject buildPanelBody;
        private GameObject nodeOverlayRoot;
        private GameObject rewardOverlayRoot;
        private GameObject postBattleBackdropRoot;
        private Text postBattleChapterText;
        private Text postBattleTrailText;
        private GameObject rootDestinyBackdropRoot;
        private Image rootDestinyBackdropVeilImage;
        private GameObject openingSceneBackdropRoot;
        private Image openingSceneBackdropImage;
        private GameObject openingScenePathRoot;
        private Text openingScenePathText;
        private Text openingSceneCauseText;
        private GameObject openingSceneTransitionRoot;
        private Image openingSceneTransitionVeilImage;
        private RectTransform openingSceneTransitionInkWipeRect;
        private Image openingSceneTransitionInkWipeImage;
        private RectTransform openingSceneTransitionLeftCloudRect;
        private Image openingSceneTransitionLeftCloudImage;
        private RectTransform openingSceneTransitionRightCloudRect;
        private Image openingSceneTransitionRightCloudImage;
        private RectTransform openingSceneTransitionGateGlowRect;
        private Image openingSceneTransitionGateGlowImage;
        private RectTransform openingSceneTransitionFocusRect;
        private Image openingSceneTransitionFocusImage;
        private CanvasGroup openingSceneTransitionFocusCanvasGroup;
        private Image openingSceneTransitionFocusWashImage;
        private RectTransform openingSceneTransitionFocusRingRect;
        private Image openingSceneTransitionFocusRingImage;
        private RectTransform openingSceneTransitionBlurRect;
        private Image openingSceneTransitionBlurImage;
        private Text openingSceneTransitionTitleText;
        private Text openingSceneTransitionHintText;
        private GameObject battleHudRoot;
        private GameObject legacyBattleHintPanelRoot;
        private GameObject legacyBattleHandPanelRoot;
        private DemoBattleHudView commercialBattleHudView;
        private DemoCommercialJourneyView commercialJourneyView;
        private GameObject nodeOverlayPanelRoot;
        private GameObject nodeJourneyBackdropRoot;
        private GameObject nodeMapPanelRoot;
        private GameObject nodeBuildPanelRoot;
        private GameObject rewardInfoPanelRoot;
        private GameObject rewardDeckPanelRoot;
        private RectTransform nodeChoiceStage;
        private GameObject nodeStartStageRoot;
        private GameObject nodeGuidanceStageRoot;
        private Text nodeStageTitleText;
        private Text nodeStageBodyText;
        private Text nodeStageChecklistText;
        private Text nodeStartBaseText;
        private Text nodeStartInheritanceText;
        private Button homeStartButton;
        private Text homeStartButtonText;
        private Button homeContinueButton;
        private Button homeCodexButton;
        private Button homeSettingsButton;
        private Text homeContinueButtonText;
        private Text homeCodexButtonText;
        private Text homeSettingsButtonText;
        private GameObject homeTopActionRoot;
        private GameObject homeModalRoot;
        private Text homeModalTitleText;
        private Text homeModalBodyText;
        private RectTransform homeTitleRoot;
        private RectTransform homeTitleCalligraphyRoot;
        private RectTransform homeTitleRule;
        private RectTransform homeStartButtonShimmer;
        private RectTransform homeGoldCrack;

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
        private float resumeBattleSpeed = 1f;
        private string rewardSignature = string.Empty;
        private Vector2 openingSceneTransitionStartPosition;
        private Vector2 openingSceneTransitionStartSize;
        private Color openingSceneTransitionAccent = Color.white;
        private int openingSceneTransitionRewardIndex = -1;
        private float openingSceneTransitionTimer;
        private bool openingSceneTransitionActive;
        private bool openingSceneTransitionCommitted;

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
            UpdateHomePresentation(Time.time);
            UpdateOpeningSceneTransition(Time.unscaledDeltaTime);
            if (commercialJourneyView != null)
            {
                commercialJourneyView.RefreshNow();
                ApplyCommercialJourneyVisibility();
            }
        }

        private void EnsureEventSystem()
        {
            EventSystem eventSystem = Object.FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new GameObject("DEMO_EventSystem");
                eventSystemObject.transform.SetParent(transform, false);
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

#if ENABLE_INPUT_SYSTEM
            StandaloneInputModule legacyInputModule = eventSystem.GetComponent<StandaloneInputModule>();
            if (legacyInputModule != null)
            {
                Destroy(legacyInputModule);
            }

            InputSystemUIInputModule inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
            }

            TryAssignDefaultInputActions(inputModule);
#else
            if (eventSystem.GetComponent<StandaloneInputModule>() == null)
            {
                eventSystem.gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private static void TryAssignDefaultInputActions(InputSystemUIInputModule inputModule)
        {
            MethodInfo assignDefaultActions = typeof(InputSystemUIInputModule).GetMethod(
                "AssignDefaultActions",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (assignDefaultActions == null)
            {
                return;
            }

            try
            {
                assignDefaultActions.Invoke(inputModule, null);
            }
            catch
            {
            }
        }
#endif

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
            BuildOpeningBackButton(canvasRect);
            BuildBattleHud(canvasRect);
            BuildCommercialJourneyView(canvasRect);
            BuildOpeningSceneTransitionOverlay(canvasRect);
        }

        private void BuildCommercialJourneyView(RectTransform parent)
        {
            GameObject viewObject = new GameObject(
                "CommercialJourneyFlow",
                typeof(RectTransform),
                typeof(DemoCommercialJourneyView));
            viewObject.transform.SetParent(parent, false);
            RectTransform rect = viewObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            commercialJourneyView = viewObject.GetComponent<DemoCommercialJourneyView>();
            commercialJourneyView.Initialize(controller, uiFont);
        }

        private void ApplyCommercialJourneyVisibility()
        {
            bool visible = IsCommercialJourneyPhase(controller.FlowPhase);
            if (!visible)
            {
                return;
            }

            nodeOverlayRoot?.SetActive(false);
            rewardOverlayRoot?.SetActive(false);
            topHudRoot?.SetActive(false);
            battleHudRoot?.SetActive(false);
            openingBackButton?.gameObject.SetActive(false);
        }

        private static bool IsCommercialJourneyPhase(DemoFlowPhase phase)
        {
            switch (phase)
            {
                case DemoFlowPhase.OpeningStory:
                case DemoFlowPhase.RegionChoice:
                case DemoFlowPhase.JourneyMap:
                case DemoFlowPhase.NodeScene:
                case DemoFlowPhase.Breakthrough:
                case DemoFlowPhase.EncounterIntro:
                case DemoFlowPhase.BossGate:
                case DemoFlowPhase.BattleOutcome:
                case DemoFlowPhase.RunResult:
                    return true;
                default:
                    return false;
            }
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

            Sprite headerCloudBand = LoadBattleHeaderRibbonSprite();
            RectTransform panel = CreateFixedPanel(
                topHudRoot.transform,
                "TopHudPanel",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -52f),
                new Vector2(1420f, 68f),
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
            headerTitleText.rectTransform.anchorMax = new Vector2(0.34f, 1f);
            headerTitleText.rectTransform.offsetMin = new Vector2(34f, 10f);
            headerTitleText.rectTransform.offsetMax = new Vector2(-12f, -10f);

            headerSummaryText = CreateText(panel, "HeaderSummary", string.Empty, 13, FontStyle.Normal, TextAnchor.MiddleRight, ColorMist);
            headerSummaryText.rectTransform.anchorMin = new Vector2(0.34f, 0f);
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
            overlayImage.color = new Color(0.93f, 0.90f, 0.82f, 0.08f);

            RectTransform journeyBackdrop = CreateStretchPanel(
                nodeOverlayRoot.transform,
                "NodeJourneyBackdrop",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                Color.white);
            ApplySpriteToImage(journeyBackdrop, LoadRouteOldMineSprite(), new Color(0.92f, 0.92f, 0.86f, 0.98f));
            journeyBackdrop.GetComponent<Image>().raycastTarget = false;
            nodeJourneyBackdropRoot = journeyBackdrop.gameObject;

            RectTransform journeyPaperWash = CreateStretchPanel(
                journeyBackdrop,
                "NodeJourneyPaperWash",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.96f, 0.93f, 0.84f, 0.64f));
            journeyPaperWash.GetComponent<Image>().raycastTarget = false;
            RectTransform journeyHorizonWash = CreateStretchPanel(
                journeyBackdrop,
                "NodeJourneyHorizonWash",
                new Vector2(0f, 0.58f),
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.96f, 0.90f, 0.28f));
            journeyHorizonWash.GetComponent<Image>().raycastTarget = false;

            Sprite headerCloudBand = LoadBattleHeaderRibbonSprite();
            RectTransform panel = CreateFixedPanel(
                nodeOverlayRoot.transform,
                "NodeOverlayPanel",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1500f, 840f),
                new Color(0.96f, 0.93f, 0.85f, 0.16f));
            nodeOverlayPanelRoot = panel.gameObject;
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
                ApplySpriteToImage(cloudVeil, headerCloudBand, new Color(1f, 1f, 1f, 0.10f));
            }

            CreateStretchPanel(
                panel,
                "PanelInkWash",
                new Vector2(0f, 0.42f),
                new Vector2(1f, 1f),
                new Vector2(20f, 20f),
                new Vector2(-20f, -20f),
                new Color(0.98f, 0.96f, 0.90f, 0.18f));

            nodeStartStageRoot = new GameObject("NodeStartStageRoot", typeof(RectTransform));
            nodeStartStageRoot.transform.SetParent(nodeOverlayRoot.transform, false);
            StretchText(nodeStartStageRoot.GetComponent<RectTransform>(), Vector2.zero, Vector2.zero);
            BuildNodeStartStage(nodeStartStageRoot.transform, headerCloudBand);

            nodeTitleText = CreateText(panel, "NodeTitle", string.Empty, 40, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.16f, 0.13f, 0.09f, 0.98f));
            nodeTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nodeTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeTitleText.rectTransform.offsetMin = new Vector2(92f, -98f);
            nodeTitleText.rectTransform.offsetMax = new Vector2(-92f, -30f);
            nodeTitleText.color = new Color(0.16f, 0.13f, 0.09f, 0.98f);

            nodeBodyText = CreateText(panel, "NodeBody", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.30f, 0.28f, 0.22f, 0.86f));
            nodeBodyText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nodeBodyText.rectTransform.anchorMax = new Vector2(0.78f, 1f);
            nodeBodyText.rectTransform.offsetMin = new Vector2(94f, -162f);
            nodeBodyText.rectTransform.offsetMax = new Vector2(-18f, -104f);

            nodeChoiceStage = CreateInkPanel(
                panel,
                "NodeChoiceStage",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(82f, 184f),
                new Vector2(-82f, -174f),
                new Color(0.96f, 0.92f, 0.82f, 0.36f));
            BuildNodeChoiceStage(nodeChoiceStage);

            RectTransform mapPanel = CreateInkPanel(
                panel,
                "NodeMapPanel",
                new Vector2(0f, 0f),
                new Vector2(0.40f, 0f),
                new Vector2(44f, 26f),
                new Vector2(-28f, 132f),
                new Color(0.94f, 0.89f, 0.77f, 0.74f));
            CreateStretchPanel(
                mapPanel,
                "MapPanelVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.98f, 0.95f, 0.88f, 0.30f));
            Text nodeMapTitle = CreatePinnedPanelTitle(mapPanel, "道途进程");
            nodeMapTitle.color = new Color(0.30f, 0.22f, 0.12f, 0.92f);
            nodeMapText = CreateBodyText(mapPanel, "NodeMapText", 14, new Color(0.25f, 0.25f, 0.21f, 0.90f));
            StretchText(nodeMapText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));
            nodeMapPanelRoot = mapPanel.gameObject;

            RectTransform buildPanel = CreateInkPanel(
                panel,
                "NodeBuildPanel",
                new Vector2(0.60f, 0f),
                new Vector2(1f, 0f),
                new Vector2(28f, 26f),
                new Vector2(-44f, 132f),
                new Color(0.94f, 0.89f, 0.77f, 0.74f));
            CreateStretchPanel(
                buildPanel,
                "BuildPanelVeil",
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.98f, 0.95f, 0.88f, 0.30f));
            Text nodeBuildTitle = CreatePinnedPanelTitle(buildPanel, "当前道基");
            nodeBuildTitle.color = new Color(0.30f, 0.22f, 0.12f, 0.92f);
            nodeBuildText = CreateBodyText(buildPanel, "NodeBuildText", 14, new Color(0.25f, 0.25f, 0.21f, 0.90f));
            StretchText(nodeBuildText.rectTransform, new Vector2(18f, 44f), new Vector2(-18f, -18f));
            nodeBuildPanelRoot = buildPanel.gameObject;

            utilityButton = CreateActionButton(panel, "NodeActionButton", out utilityButtonText, new Color(0.68f, 0.19f, 0.12f, 0.94f), new Color(0.98f, 0.91f, 0.78f, 1f));
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
            Sprite homeHero = LoadHomeHeroSprite();
            Sprite cloudsea = LoadSpriteResource(SceneCloudseaFarResourcePath, SceneBattleCloudseaResourcePath);
            homeTitleRoot = null;
            homeTitleCalligraphyRoot = null;
            homeTitleRule = null;
            homeStartButtonShimmer = null;
            homeGoldCrack = null;
            homeMistThreads.Clear();
            homeLightningStrokes.Clear();

            CreateStretchPanel(
                parent,
                "HomeInkBase",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.030f, 0.034f, 0.036f, 0.97f));

            if (homeHero != null)
            {
                RectTransform heroBackdrop = CreateStretchPanel(
                    parent,
                    "HomeHeroBackdrop",
                    Vector2.zero,
                    Vector2.one,
                    Vector2.zero,
                    Vector2.zero,
                    Color.white);
                ApplySpriteToImage(heroBackdrop, homeHero, Color.white);
            }
            else if (cloudsea != null)
            {
                RectTransform cloudBackdrop = CreateStretchPanel(
                    parent,
                    "HomeCloudBackdrop",
                    Vector2.zero,
                    Vector2.one,
                    Vector2.zero,
                    Vector2.zero,
                    Color.white);
                ApplySpriteToImage(cloudBackdrop, cloudsea, new Color(0.72f, 0.80f, 0.80f, 0.30f));
            }

            CreateStretchPanel(parent, "HomeFullPaperLift", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.96f, 0.93f, 0.86f, 0.035f));
            CreateHomeAtmosphereLayers(parent, headerCloudBand);

            RectTransform titleRoot = CreateFixedPanel(
                parent,
                "HomeTitleRoot",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(106f, -100f),
                new Vector2(1120f, 298f),
                new Color(0f, 0f, 0f, 0f));
            homeTitleRoot = titleRoot;

            Text eyebrowText = CreateText(titleRoot, "HomeEyebrow", "PATH  OF  TEN  THOUSAND  WAYS", 13, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.30f, 0.40f, 0.38f, 0.76f));
            eyebrowText.rectTransform.anchorMin = new Vector2(0f, 1f);
            eyebrowText.rectTransform.anchorMax = new Vector2(1f, 1f);
            eyebrowText.rectTransform.offsetMin = new Vector2(6f, -27f);
            eyebrowText.rectTransform.offsetMax = new Vector2(-4f, -5f);

            Sprite titleCalligraphy = LoadHomeTitleCalligraphySprite();
            if (titleCalligraphy != null)
            {
                RectTransform titleMark = CreateFixedPanel(
                    titleRoot,
                    "HomeTitleCalligraphy",
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(-34f, -50f),
                    new Vector2(680f, 226f),
                    Color.white);
                homeTitleCalligraphyRoot = titleMark;
                ApplySpriteToImage(titleMark, titleCalligraphy, Color.white, false);
                titleMark.GetComponent<Image>().raycastTarget = false;

                Shadow titleShadow = titleMark.gameObject.AddComponent<Shadow>();
                titleShadow.effectColor = new Color(0.58f, 0.45f, 0.21f, 0.10f);
                titleShadow.effectDistance = new Vector2(2f, -2f);
            }
            else
            {
                Text titleText = CreateText(titleRoot, "HomeTitle", "万道归途", 118, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.045f, 0.050f, 0.043f, 0.99f));
                titleText.font = GetHomeTitleFont();
                titleText.horizontalOverflow = HorizontalWrapMode.Overflow;
                Outline titleWeight = titleText.gameObject.AddComponent<Outline>();
                titleWeight.effectColor = new Color(0.045f, 0.050f, 0.043f, 0.42f);
                titleWeight.effectDistance = new Vector2(0.75f, -0.75f);
                Shadow titleShadow = titleText.gameObject.AddComponent<Shadow>();
                titleShadow.effectColor = new Color(0.58f, 0.45f, 0.21f, 0.11f);
                titleShadow.effectDistance = new Vector2(1.6f, -1.6f);
                titleText.rectTransform.anchorMin = new Vector2(0f, 1f);
                titleText.rectTransform.anchorMax = new Vector2(1f, 1f);
                titleText.rectTransform.offsetMin = new Vector2(-4f, -156f);
                titleText.rectTransform.offsetMax = new Vector2(-4f, -24f);
            }

            RectTransform titleRule = CreateStretchPanel(
                titleRoot,
                "HomeTitleRule",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(6f, -38f),
                new Vector2(346f, -36f),
                new Color(0.58f, 0.45f, 0.21f, 0.34f));
            homeTitleRule = titleRule;
            IgnoreLayout(titleRule.gameObject);

            Text subtitleText = CreateText(titleRoot, "HomeSubtitle", "修仙不是打怪升级，而是构筑一条属于自己的道途。", 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.22f, 0.24f, 0.22f, 0.74f));
            subtitleText.rectTransform.anchorMin = new Vector2(0f, 0f);
            subtitleText.rectTransform.anchorMax = new Vector2(1f, 0f);
            subtitleText.rectTransform.offsetMin = new Vector2(4f, 18f);
            subtitleText.rectTransform.offsetMax = new Vector2(-80f, 62f);

            nodeStartInheritanceText = null;
            nodeStartBaseText = CreateHomeStatusStrip(parent);

            homeStartButton = CreateActionButton(parent, "HomeStartButton", out homeStartButtonText, HomeButtonPrimaryBase, HomeButtonInk);
            RectTransform startRect = homeStartButton.GetComponent<RectTransform>();
            startRect.anchorMin = new Vector2(0f, 0.5f);
            startRect.anchorMax = new Vector2(0f, 0.5f);
            startRect.pivot = new Vector2(0f, 0.5f);
            startRect.anchoredPosition = new Vector2(112f, -58f);
            startRect.sizeDelta = new Vector2(392f, 86f);
            HideFrameDecorations(startRect);
            ApplyHomeButtonSurface(startRect, true);
            AddHomeButtonSoftEdge(startRect, true);
            homeStartButtonShimmer = CreateFixedPanel(
                startRect,
                "HomeButtonShimmer",
                new Vector2(0f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(68f, 0f),
                new Vector2(126f, 3f),
                new Color(0.68f, 0.47f, 0.17f, 0f));
            homeStartButtonShimmer.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(homeStartButtonShimmer.gameObject);
            homeStartButton.colors = CreateHomeButtonColors(HomeButtonPrimaryBase);
            homeStartButtonText.fontSize = 22;
            homeStartButtonText.color = HomeButtonInk;
            homeStartButtonText.text = "开始新局";
            homeStartButtonText.transform.SetAsLastSibling();
            homeStartButton.onClick.AddListener(OnStartRunClicked);

            homeContinueButton = CreateHomeSecondaryMenuItem(parent, "HomeContinueEntry", "尚未立道", new Vector2(112f, -156f), out homeContinueButtonText);
            homeContinueButton.onClick.AddListener(OnContinueRunClicked);
            homeCodexButton = CreateHomeSecondaryMenuItem(parent, "HomeCodexEntry", "流派图鉴", new Vector2(112f, -224f), out homeCodexButtonText);
            homeSettingsButton = CreateHomeSecondaryMenuItem(parent, "HomeSettingsEntry", "设置", new Vector2(112f, -292f), out homeSettingsButtonText);
            homeCodexButton.onClick.AddListener(() => ShowHomeModal("流派图鉴", BuildHomeCodexText()));
            homeSettingsButton.onClick.AddListener(() => ShowHomeModal("设置", BuildHomeSettingsText()));

            CreateHomeTopActionBar(parent);
            CreateHomeActionHint(parent);
            CreateHomeModal(parent);
        }

        private void CreateHomeActionHint(Transform parent)
        {
            RectTransform hintRoot = CreateFixedPanel(
                parent,
                "HomeActionHintTag",
                Vector2.zero,
                Vector2.zero,
                new Vector2(112f, 158f),
                new Vector2(430f, 34f),
                new Color(0.88f, 0.84f, 0.74f, 0.12f));
            hintRoot.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(hintRoot, new Color(0.55f, 0.43f, 0.23f, 0.16f), new Color(0f, 0f, 0f, 0f), 1f, false);

            RectTransform hintSeal = CreateFixedPanel(
                hintRoot,
                "HintSeal",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(16f, 0f),
                new Vector2(4f, 20f),
                new Color(0.45f, 0.13f, 0.09f, 0.54f));
            hintSeal.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(hintSeal.gameObject);

            Text actionHint = CreateText(hintRoot, "HomeActionHint", "从根脚、所携与所往开始这一世", 14, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.23f, 0.25f, 0.22f, 0.86f));
            StretchText(actionHint.rectTransform, new Vector2(30f, 4f), new Vector2(-14f, -4f));
        }

        private Text CreateHomeOpeningTrail(Transform parent)
        {
            RectTransform trail = CreateFixedPanel(
                parent,
                "HomeOpeningTrail",
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(96f, 196f),
                new Vector2(560f, 126f),
                new Color(0.045f, 0.052f, 0.055f, 0.58f));
            DecorateFrame(trail, new Color(0.46f, 0.68f, 0.68f, 0.34f), new Color(0f, 0f, 0f, 0f), 1f, false);

            Text labelText = CreateText(trail, "TrailLabel", "开局脉络", 14, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.70f, 0.84f, 0.82f, 0.95f));
            labelText.rectTransform.anchorMin = new Vector2(0f, 1f);
            labelText.rectTransform.anchorMax = new Vector2(1f, 1f);
            labelText.rectTransform.offsetMin = new Vector2(22f, -30f);
            labelText.rectTransform.offsetMax = new Vector2(-22f, -8f);

            Text bodyText = CreateText(trail, "TrailBody", string.Empty, 18, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            bodyText.rectTransform.anchorMin = Vector2.zero;
            bodyText.rectTransform.anchorMax = Vector2.one;
            bodyText.rectTransform.offsetMin = new Vector2(22f, 24f);
            bodyText.rectTransform.offsetMax = new Vector2(-22f, -42f);

            Text hintText = CreateText(trail, "TrailHint", "首页只看入口，具体选择在下一步展开", 12, FontStyle.Normal, TextAnchor.LowerLeft, new Color(0.66f, 0.72f, 0.72f, 0.92f));
            hintText.rectTransform.anchorMin = Vector2.zero;
            hintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            hintText.rectTransform.offsetMin = new Vector2(22f, 10f);
            hintText.rectTransform.offsetMax = new Vector2(-22f, 28f);

            return bodyText;
        }

        private Text CreateHomeStatusStrip(Transform parent)
        {
            Color cinnabar = new Color(0.50f, 0.13f, 0.08f, 0.68f);

            RectTransform strip = CreateFixedPanel(
                parent,
                "HomeStatusStrip",
                new Vector2(0.66f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-24f, 106f),
                new Vector2(236f, 202f),
                new Color(0.88f, 0.84f, 0.73f, 0.52f));
            strip.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(strip, new Color(0.55f, 0.43f, 0.23f, 0.34f), new Color(0f, 0f, 0f, 0f), 1.0f, false);

            RectTransform statusTopLine = CreateStretchPanel(
                strip,
                "StatusTopLine",
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(22f, -10f),
                new Vector2(-22f, -8f),
                new Color(0.80f, 0.63f, 0.34f, 0.12f));
            statusTopLine.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(statusTopLine.gameObject);

            RectTransform statusSeal = CreateFixedPanel(
                strip,
                "StatusSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -18f),
                new Vector2(30f, 72f),
                cinnabar);
            IgnoreLayout(statusSeal.gameObject);
            statusSeal.GetComponent<Image>().raycastTarget = false;

            Text sealText = CreateText(statusSeal, "SealText", "命\n签", 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.96f, 0.86f, 0.70f, 0.98f));
            StretchText(sealText.rectTransform, new Vector2(2f, 3f), new Vector2(-2f, -3f));

            RectTransform statusDivider = CreateFixedPanel(
                strip,
                "StatusDivider",
                new Vector2(0f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(58f, -18f),
                new Vector2(2f, 126f),
                new Color(cinnabar.r, cinnabar.g, cinnabar.b, 0.50f));
            statusDivider.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(statusDivider.gameObject);

            RectTransform bodyPaper = CreateStretchPanel(
                strip,
                "StatusBodyPaper",
                Vector2.zero,
                Vector2.one,
                new Vector2(68f, 16f),
                new Vector2(-12f, -18f),
                new Color(0.96f, 0.91f, 0.76f, 0.12f));
            bodyPaper.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(bodyPaper.gameObject);

            Text bodyText = CreateText(strip, "StatusBody", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.08f, 0.09f, 0.08f, 0.98f));
            bodyText.lineSpacing = 1.22f;
            StretchText(bodyText.rectTransform, new Vector2(72f, 18f), new Vector2(-14f, -22f));
            return bodyText;
        }

        private void CreateHomeAtmosphereLayers(Transform parent, Sprite headerCloudBand)
        {
            for (int i = 0; i < 7; i++)
            {
                float width = 160f + (i % 4) * 54f;
                RectTransform thread = CreateFixedPanel(
                    parent,
                    "HomeMistThread_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-300f + i * 128f, -92f + (i % 3) * 34f),
                    new Vector2(width, i % 2 == 0 ? 1.6f : 1.1f),
                    new Color(0.82f, 0.84f, 0.80f, 0.040f + (i % 3) * 0.010f));
                thread.localEulerAngles = new Vector3(0f, 0f, -2.4f + i * 0.7f);
                thread.GetComponent<Image>().raycastTarget = false;
                homeMistThreads.Add(thread);
            }

            for (int i = 0; i < 4; i++)
            {
                RectTransform stroke = CreateFixedPanel(
                    parent,
                    "HomeLightningStroke_" + i,
                    new Vector2(0.72f, 0.72f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(66f + i * 34f, 68f - i * 26f),
                    new Vector2(2.0f, 86f - i * 10f),
                    new Color(0.78f, 0.86f, 0.86f, 0f));
                stroke.localEulerAngles = new Vector3(0f, 0f, i % 2 == 0 ? -26f : 18f);
                stroke.GetComponent<Image>().raycastTarget = false;
                homeLightningStrokes.Add(stroke);
            }

            homeGoldCrack = CreateFixedPanel(
                parent,
                "HomeGoldCrackAfterVictory",
                new Vector2(1f, 0f),
                new Vector2(1f, 0.5f),
                new Vector2(-78f, 138f),
                new Vector2(410f, 3f),
                new Color(0.80f, 0.60f, 0.24f, 0f));
            homeGoldCrack.localEulerAngles = new Vector3(0f, 0f, -2f);
            homeGoldCrack.GetComponent<Image>().raycastTarget = false;
        }

        private Button CreateHomeSecondaryMenuItem(Transform parent, string name, string label, Vector2 anchoredPosition, out Text itemText)
        {
            RectTransform item = CreateFixedPanel(
                parent,
                name,
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                anchoredPosition,
                new Vector2(318f, 56f),
                HomeButtonSecondaryBase);
            ApplyHomeButtonSurface(item, false);
            AddHomeButtonSoftEdge(item, false);

            Button button = item.gameObject.AddComponent<Button>();
            button.targetGraphic = item.GetComponent<Image>();
            button.colors = CreateHomeButtonColors(HomeButtonSecondaryBase);

            itemText = CreateText(item, "Label", label, 16, FontStyle.Bold, TextAnchor.MiddleCenter, HomeButtonInkMuted);
            StretchText(itemText.rectTransform, new Vector2(20f, 6f), new Vector2(-20f, -6f));
            itemText.transform.SetAsLastSibling();
            return button;
        }

        private void CreateHomeTopActionBar(Transform parent)
        {
            homeTopActionRoot = new GameObject("HomeTopActionRoot", typeof(RectTransform));
            homeTopActionRoot.transform.SetParent(parent, false);

            RectTransform root = homeTopActionRoot.GetComponent<RectTransform>();
            root.anchorMin = new Vector2(1f, 1f);
            root.anchorMax = new Vector2(1f, 1f);
            root.pivot = new Vector2(1f, 1f);
            root.anchoredPosition = new Vector2(-52f, -38f);
            root.sizeDelta = new Vector2(176f, 44f);

            CreateHomeTopActionButton(root, "HomeTopCodex", null, "鉴", new Vector2(-104f, -22f), () => ShowHomeModal("流派图鉴", BuildHomeCodexText()));
            CreateHomeTopActionButton(root, "HomeTopSettings", null, "设", new Vector2(-52f, -22f), () => ShowHomeModal("设置", BuildHomeSettingsText()));
            CreateHomeTopActionButton(root, "HomeTopExit", null, "离", new Vector2(0f, -22f), () => ShowHomeModal("退出", BuildHomeExitText()));
        }

        private Button CreateHomeTopActionButton(Transform parent, string name, Sprite sprite, string fallbackGlyph, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
        {
            RectTransform buttonRect = CreateFixedPanel(
                parent,
                name,
                new Vector2(1f, 1f),
                new Vector2(0.5f, 0.5f),
                anchoredPosition,
                new Vector2(38f, 38f),
                new Color(0.90f, 0.86f, 0.74f, 0.42f));
            ApplyHomeButtonSurface(buttonRect, false);

            Button button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = buttonRect.GetComponent<Image>();
            button.colors = CreateHomeButtonColors(HomeButtonSecondaryBase);
            button.onClick.AddListener(onClick);

            if (sprite != null)
            {
                RectTransform iconRect = CreateStretchPanel(buttonRect, "Icon", Vector2.zero, Vector2.one, new Vector2(8f, 8f), new Vector2(-8f, -8f), Color.white);
                ApplySpriteToImage(iconRect, sprite, new Color(0.20f, 0.21f, 0.18f, 0.84f), true);
                iconRect.GetComponent<Image>().raycastTarget = false;
            }
            else
            {
                Text glyphText = CreateText(buttonRect, "Glyph", fallbackGlyph, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.20f, 0.21f, 0.18f, 0.86f));
                StretchText(glyphText.rectTransform, new Vector2(4f, 3f), new Vector2(-4f, -3f));
            }

            return button;
        }

        private void CreateHomeModal(Transform parent)
        {
            RectTransform modalRoot = CreateStretchPanel(
                parent,
                "HomeModalRoot",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.20f, 0.19f, 0.16f, 0.16f));
            homeModalRoot = modalRoot.gameObject;

            RectTransform panel = CreateFixedPanel(
                modalRoot,
                "HomeModalPanel",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -18f),
                new Vector2(680f, 360f),
                new Color(0.88f, 0.84f, 0.73f, 0.88f));
            DecorateFrame(panel, new Color(0.54f, 0.40f, 0.20f, 0.50f), new Color(1f, 0.98f, 0.90f, 0.08f), 1.2f, false);

            homeModalTitleText = CreateText(panel, "HomeModalTitle", string.Empty, 24, FontStyle.Bold, TextAnchor.UpperLeft, HomeButtonInk);
            homeModalTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            homeModalTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            homeModalTitleText.rectTransform.offsetMin = new Vector2(34f, -58f);
            homeModalTitleText.rectTransform.offsetMax = new Vector2(-98f, -20f);

            RectTransform rule = CreateStretchPanel(
                panel,
                "HomeModalRule",
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(34f, -74f),
                new Vector2(-34f, -72f),
                new Color(0.54f, 0.40f, 0.20f, 0.22f));
            rule.GetComponent<Image>().raycastTarget = false;

            homeModalBodyText = CreateText(panel, "HomeModalBody", string.Empty, 16, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.20f, 0.20f, 0.17f, 0.90f));
            homeModalBodyText.rectTransform.anchorMin = Vector2.zero;
            homeModalBodyText.rectTransform.anchorMax = Vector2.one;
            homeModalBodyText.rectTransform.offsetMin = new Vector2(34f, 76f);
            homeModalBodyText.rectTransform.offsetMax = new Vector2(-34f, -92f);

            Button closeButton = CreateHomeModalCloseButton(panel);
            closeButton.onClick.AddListener(HideHomeModal);
            homeModalRoot.SetActive(false);
        }

        private Button CreateHomeModalCloseButton(Transform parent)
        {
            RectTransform closeRect = CreateFixedPanel(
                parent,
                "HomeModalClose",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-30f, -24f),
                new Vector2(76f, 34f),
                new Color(0.89f, 0.82f, 0.64f, 0.72f));
            ApplyHomeButtonSurface(closeRect, false);

            Button button = closeRect.gameObject.AddComponent<Button>();
            button.targetGraphic = closeRect.GetComponent<Image>();
            button.colors = CreateHomeButtonColors(HomeButtonSecondaryBase);

            Text text = CreateText(closeRect, "Label", "收起", 14, FontStyle.Bold, TextAnchor.MiddleCenter, HomeButtonInkMuted);
            StretchText(text.rectTransform, new Vector2(8f, 3f), new Vector2(-8f, -3f));
            text.transform.SetAsLastSibling();
            return button;
        }

        private void CreateHomeFocusRelic(Transform parent, Sprite itemSprite, Sprite headerCloudBand)
        {
            RectTransform circleBack = CreateFixedPanel(
                parent,
                "HomeFocusCircleBack",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-288f, -42f),
                new Vector2(440f, 440f),
                new Color(0.24f, 0.35f, 0.34f, 0.09f));
            circleBack.localEulerAngles = new Vector3(0f, 0f, 14f);
            DecorateFrame(circleBack, new Color(0.62f, 0.78f, 0.72f, 0.20f), new Color(0f, 0f, 0f, 0f), 1.2f, false);

            RectTransform circle = CreateFixedPanel(
                parent,
                "HomeFocusCircle",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-294f, -48f),
                new Vector2(316f, 316f),
                new Color(0.70f, 0.80f, 0.72f, 0.08f));
            circle.localEulerAngles = new Vector3(0f, 0f, -10f);
            DecorateFrame(circle, new Color(0.78f, 0.70f, 0.46f, 0.36f), new Color(0.86f, 0.80f, 0.60f, 0.02f), 1.4f, false);

            RectTransform relic = CreateFixedPanel(
                parent,
                "HomeFocusRelic",
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-316f, -44f),
                new Vector2(176f, 244f),
                new Color(0.08f, 0.078f, 0.070f, 0.96f));
            relic.localEulerAngles = new Vector3(0f, 0f, 2.0f);
            ApplySpriteToImage(relic, itemSprite, itemSprite != null ? Color.white : new Color(0.08f, 0.078f, 0.070f, 0.96f), true);
            DecorateFrame(relic, new Color(0.90f, 0.78f, 0.48f, 0.78f), new Color(0f, 0f, 0f, 0f), 1.6f, true);

            Text titleText = CreateText(parent, "HomeFocusRelicTitle", "旧剑匣", 22, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            titleText.rectTransform.anchorMin = new Vector2(1f, 0.5f);
            titleText.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            titleText.rectTransform.pivot = new Vector2(1f, 0.5f);
            titleText.rectTransform.anchoredPosition = new Vector2(-246f, -214f);
            titleText.rectTransform.sizeDelta = new Vector2(250f, 32f);

            Text subText = CreateText(parent, "HomeFocusRelicSub", "此物只是引子，道途仍在局内显化", 14, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.74f, 0.80f, 0.78f, 0.94f));
            subText.rectTransform.anchorMin = new Vector2(1f, 0.5f);
            subText.rectTransform.anchorMax = new Vector2(1f, 0.5f);
            subText.rectTransform.pivot = new Vector2(1f, 0.5f);
            subText.rectTransform.anchoredPosition = new Vector2(-246f, -246f);
            subText.rectTransform.sizeDelta = new Vector2(330f, 26f);

            if (headerCloudBand != null)
            {
                RectTransform cloudBand = CreateFixedPanel(
                    parent,
                    "HomeFocusCloudBand",
                    new Vector2(1f, 0.5f),
                    new Vector2(1f, 0.5f),
                    new Vector2(-216f, -188f),
                    new Vector2(360f, 96f),
                    Color.white);
                ApplySpriteToImage(cloudBand, headerCloudBand, new Color(1f, 1f, 1f, 0.12f));
                cloudBand.localEulerAngles = new Vector3(0f, 0f, -7f);
            }
        }

        private void CreateHomeSmallRelic(Transform parent, string name, Sprite sprite, Vector2 position, Vector2 size, Color accent)
        {
            RectTransform relic = CreateFixedPanel(
                parent,
                name,
                new Vector2(1f, 0.5f),
                new Vector2(1f, 0.5f),
                new Vector2(-position.x, position.y),
                size,
                new Color(0.055f, 0.060f, 0.060f, 0.52f));
            ApplySpriteToImage(relic, sprite, sprite != null ? new Color(1f, 1f, 1f, 0.42f) : new Color(0.055f, 0.060f, 0.060f, 0.52f), true);
            DecorateFrame(relic, accent, new Color(0f, 0f, 0f, 0f), 1.1f, false);
        }

        private void CreateHomeMistTrail(Transform parent, Vector2 position, Vector2 size, Color color, Sprite headerCloudBand)
        {
            RectTransform trail = CreateFixedPanel(parent, "StartMistTrail", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, size, color);
            ApplySpriteToImage(trail, headerCloudBand, headerCloudBand != null ? color : color);
            trail.localEulerAngles = new Vector3(0f, 0f, -8f);
        }

        private void CreateHomeParticles(Transform parent, Vector2 center, Color color)
        {
            for (int i = 0; i < 18; i++)
            {
                float angle = i * 21f;
                float radius = 100f + (i % 5) * 18f;
                Vector2 position = center + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius * 0.54f);
                float size = 5f + (i % 4) * 2f;
                RectTransform mote = CreateFixedPanel(parent, "StartRelicMote_" + i, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), position, new Vector2(size, size), new Color(color.r, color.g, color.b, 0.20f + (i % 3) * 0.08f));
                mote.localEulerAngles = new Vector3(0f, 0f, angle);
            }
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

            Text bodyText = CreateText(body, "BodyText", string.Empty, 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.95f, 0.94f, 0.90f, 1f));
            bodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            bodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            bodyText.rectTransform.offsetMin = new Vector2(28f, 38f);
            bodyText.rectTransform.offsetMax = new Vector2(-28f, -92f);

            Text footerText = CreateText(body, "FooterText", "卷藏今朝起势", 13, FontStyle.Bold, TextAnchor.LowerCenter, new Color(accent.r, accent.g, accent.b, 0.96f));
            footerText.rectTransform.anchorMin = new Vector2(0f, 0f);
            footerText.rectTransform.anchorMax = new Vector2(1f, 0f);
            footerText.rectTransform.offsetMin = new Vector2(16f, 14f);
            footerText.rectTransform.offsetMax = new Vector2(-16f, 32f);

            return bodyText;
        }

        private void BuildRewardOverlay(RectTransform parent)
        {
            rewardOverlayRoot = new GameObject("RewardOverlayRoot", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            rewardOverlayRoot.transform.SetParent(parent, false);
            rewardOverlayCanvasGroup = rewardOverlayRoot.GetComponent<CanvasGroup>();

            RectTransform overlayRect = rewardOverlayRoot.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            Image overlayImage = rewardOverlayRoot.GetComponent<Image>();
            overlayImage.color = new Color(0.93f, 0.90f, 0.82f, 0.10f);

            RectTransform postBattleBackdrop = CreateStretchPanel(
                rewardOverlayRoot.transform,
                "PostBattleJourneyBackdrop",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                Color.white);
            ApplySpriteToImage(postBattleBackdrop, LoadRouteOldMineSprite(), new Color(0.90f, 0.91f, 0.86f, 0.98f));
            postBattleBackdrop.GetComponent<Image>().raycastTarget = false;
            postBattleBackdropRoot = postBattleBackdrop.gameObject;
            postBattleBackdropRoot.SetActive(false);

            RectTransform postBattlePaperWash = CreateStretchPanel(
                postBattleBackdrop,
                "PostBattlePaperWash",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.97f, 0.94f, 0.86f, 0.68f));
            postBattlePaperWash.GetComponent<Image>().raycastTarget = false;
            RectTransform postBattleFloorWash = CreateStretchPanel(
                postBattleBackdrop,
                "PostBattleFloorWash",
                Vector2.zero,
                new Vector2(1f, 0.35f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.86f, 0.78f, 0.61f, 0.22f));
            postBattleFloorWash.GetComponent<Image>().raycastTarget = false;

            RectTransform chapterSeal = CreateFixedPanel(
                postBattleBackdrop,
                "PostBattleChapterSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(64f, -54f),
                new Vector2(46f, 144f),
                new Color(0.48f, 0.13f, 0.09f, 0.90f));
            chapterSeal.GetComponent<Image>().raycastTarget = false;
            postBattleChapterText = CreateText(chapterSeal, "PostBattleChapterText", string.Empty, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.98f, 0.89f, 0.76f, 0.98f));
            StretchText(postBattleChapterText.rectTransform, new Vector2(7f, 9f), new Vector2(-7f, -9f));

            RectTransform postBattleTrail = CreateFixedPanel(
                postBattleBackdrop,
                "PostBattleTrail",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -42f),
                new Vector2(720f, 30f),
                new Color(0.92f, 0.85f, 0.70f, 0.42f));
            postBattleTrail.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(postBattleTrail, new Color(0.50f, 0.36f, 0.18f, 0.22f), new Color(0f, 0f, 0f, 0f), 1f, false);
            postBattleTrailText = CreateText(postBattleTrail, "PostBattleTrailText", string.Empty, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.26f, 0.22f, 0.16f, 0.82f));
            StretchText(postBattleTrailText.rectTransform, new Vector2(14f, 4f), new Vector2(-14f, -4f));

            RectTransform rootBackdrop = CreateStretchPanel(
                rewardOverlayRoot.transform,
                "RootDestinyBackdrop",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                Color.white);
            ApplySpriteToImage(rootBackdrop, LoadRootDestinyBackdropSprite(), Color.white);
            Image rootBackdropImage = rootBackdrop.GetComponent<Image>();
            rootBackdropImage.raycastTarget = false;
            rootDestinyBackdropRoot = rootBackdrop.gameObject;
            rootDestinyBackdropRoot.SetActive(false);

            RectTransform rootBackdropVeil = CreateStretchPanel(
                rootBackdrop,
                "RootDestinyBackdropVeil",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.015f, 0.018f, 0.018f, 0.06f));
            rootDestinyBackdropVeilImage = rootBackdropVeil.GetComponent<Image>();
            rootDestinyBackdropVeilImage.raycastTarget = false;

            RectTransform openingSceneBackdrop = CreateStretchPanel(
                rewardOverlayRoot.transform,
                "OpeningSceneWeakBackdrop",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                Color.white);
            ApplySpriteToImage(openingSceneBackdrop, LoadOpeningSceneBackdropSprite(), Color.white);
            openingSceneBackdropImage = openingSceneBackdrop.GetComponent<Image>();
            openingSceneBackdropImage.raycastTarget = false;
            openingSceneBackdropRoot = openingSceneBackdrop.gameObject;
            openingSceneBackdropRoot.SetActive(false);

            RectTransform openingSceneBackdropVeil = CreateStretchPanel(
                openingSceneBackdrop,
                "OpeningSceneWeakBackdropVeil",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.95f, 0.88f, 0.035f));
            openingSceneBackdropVeil.GetComponent<Image>().raycastTarget = false;

            RectTransform openingSceneDeskWash = CreateStretchPanel(
                openingSceneBackdrop,
                "OpeningSceneDeskWash",
                new Vector2(0f, 0f),
                new Vector2(1f, 0.50f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.46f, 0.31f, 0.15f, 0f));
            openingSceneDeskWash.GetComponent<Image>().raycastTarget = false;

            RectTransform openingSceneScrollGround = CreateFixedPanel(
                openingSceneBackdrop,
                "OpeningSceneScrollGround",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -58f),
                new Vector2(1648f, 728f),
                new Color(0.92f, 0.82f, 0.60f, 0f));
            openingSceneScrollGround.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(openingSceneScrollGround, new Color(0.48f, 0.34f, 0.17f, 0f), new Color(1f, 0.90f, 0.66f, 0f), 1f, false);

            RectTransform openingSceneTopThread = CreateFixedPanel(
                openingSceneBackdrop,
                "OpeningSceneTopThread",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 314f),
                new Vector2(1120f, 2f),
                new Color(0.64f, 0.42f, 0.18f, 0.08f));
            openingSceneTopThread.GetComponent<Image>().raycastTarget = false;

            RectTransform openingScenePathStrip = CreateFixedPanel(
                openingSceneBackdrop,
                "OpeningScenePathStrip",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 264f),
                new Vector2(820f, 40f),
                new Color(0.93f, 0.83f, 0.62f, 0.28f));
            openingScenePathRoot = openingScenePathStrip.gameObject;
            openingScenePathRoot.SetActive(false);
            openingScenePathStrip.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(openingScenePathStrip, new Color(0.64f, 0.42f, 0.18f, 0.22f), new Color(1f, 0.91f, 0.68f, 0.045f), 1f, false);
            openingScenePathText = CreateText(openingScenePathStrip, "OpeningScenePathText", string.Empty, 15, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.30f, 0.20f, 0.10f, 0.86f));
            StretchText(openingScenePathText.rectTransform, new Vector2(18f, 5f), new Vector2(-18f, -5f));

            RectTransform openingSceneCauseStrip = CreateFixedPanel(
                openingSceneBackdrop,
                "OpeningSceneCauseStrip",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, -446f),
                new Vector2(940f, 38f),
                new Color(0.92f, 0.82f, 0.60f, 0.30f));
            openingSceneCauseStrip.gameObject.SetActive(false);
            openingSceneCauseStrip.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(openingSceneCauseStrip, new Color(0.64f, 0.42f, 0.18f, 0f), new Color(1f, 0.91f, 0.68f, 0f), 1f, false);
            openingSceneCauseText = CreateText(openingSceneCauseStrip, "OpeningSceneCauseText", string.Empty, 14, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.34f, 0.25f, 0.16f, 0.78f));
            StretchText(openingSceneCauseText.rectTransform, new Vector2(18f, 5f), new Vector2(-18f, -5f));

            rewardTitleText = CreateText(rewardOverlayRoot.transform, "RewardTitle", string.Empty, 40, FontStyle.Bold, TextAnchor.UpperCenter, ColorPaper);
            rewardTitleText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardTitleText.rectTransform.anchoredPosition = new Vector2(0f, -118f);
            rewardTitleText.rectTransform.sizeDelta = new Vector2(980f, 50f);
            rewardTitleText.color = new Color(0.95f, 0.94f, 0.90f, 1f);

            rewardBodyText = CreateText(rewardOverlayRoot.transform, "RewardBody", string.Empty, 18, FontStyle.Normal, TextAnchor.UpperCenter, new Color(0.90f, 0.92f, 0.95f, 0.98f));
            rewardBodyText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardBodyText.rectTransform.anchoredPosition = new Vector2(0f, -172f);
            rewardBodyText.rectTransform.sizeDelta = new Vector2(1160f, 56f);

            rewardSectionTitleText = CreateText(rewardOverlayRoot.transform, "RewardSectionTitle", string.Empty, 22, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.97f, 0.87f, 0.56f, 1f));
            rewardSectionTitleText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardSectionTitleText.rectTransform.anchoredPosition = new Vector2(0f, -228f);
            rewardSectionTitleText.rectTransform.sizeDelta = new Vector2(840f, 34f);

            rewardSectionHintText = CreateText(rewardOverlayRoot.transform, "RewardSectionHint", string.Empty, 14, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.86f, 0.89f, 0.93f, 0.98f));
            rewardSectionHintText.rectTransform.anchorMin = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.anchorMax = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.pivot = new Vector2(0.5f, 1f);
            rewardSectionHintText.rectTransform.anchoredPosition = new Vector2(0f, -256f);
            rewardSectionHintText.rectTransform.sizeDelta = new Vector2(980f, 28f);
            rewardRerollButton = CreateActionButton(
                rewardOverlayRoot.transform,
                "RewardRerollButton",
                out rewardRerollButtonText,
                new Color(0.55f, 0.16f, 0.10f, 0.94f),
                new Color(0.98f, 0.90f, 0.76f, 1f));
            RectTransform rerollRect = rewardRerollButton.GetComponent<RectTransform>();
            rerollRect.anchorMin = new Vector2(1f, 1f);
            rerollRect.anchorMax = new Vector2(1f, 1f);
            rerollRect.pivot = new Vector2(1f, 1f);
            rerollRect.anchoredPosition = new Vector2(-72f, -190f);
            rerollRect.sizeDelta = new Vector2(184f, 48f);
            rewardRerollButton.onClick.AddListener(OnRewardRerollClicked);
            rewardRerollButton.gameObject.SetActive(false);

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
                new Color(0.95f, 0.91f, 0.81f, 0.98f)).gameObject;
            DecorateFrame(rewardDetailPanel.GetComponent<RectTransform>(), new Color(0.50f, 0.34f, 0.16f, 0.72f), new Color(0.98f, 0.94f, 0.84f, 0.10f), 1.5f, false);
            rewardDetailPanel.SetActive(false);

            rewardDetailTitleText = CreateText(rewardDetailPanel.transform, "RewardDetailTitle", string.Empty, 18, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.19f, 0.15f, 0.10f, 0.98f));
            rewardDetailTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            rewardDetailTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rewardDetailTitleText.rectTransform.offsetMin = new Vector2(20f, -34f);
            rewardDetailTitleText.rectTransform.offsetMax = new Vector2(-20f, -10f);

            rewardDetailBodyText = CreateText(rewardDetailPanel.transform, "RewardDetailBody", string.Empty, 14, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.29f, 0.27f, 0.22f, 0.92f));
            rewardDetailBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            rewardDetailBodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rewardDetailBodyText.rectTransform.offsetMin = new Vector2(20f, 24f);
            rewardDetailBodyText.rectTransform.offsetMax = new Vector2(-20f, -42f);

            rewardDetailHintText = CreateText(rewardDetailPanel.transform, "RewardDetailHint", "移开鼠标即可收起", 12, FontStyle.Normal, TextAnchor.LowerRight, new Color(0.52f, 0.36f, 0.17f, 0.80f));
            rewardDetailHintText.rectTransform.anchorMin = new Vector2(0f, 0f);
            rewardDetailHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            rewardDetailHintText.rectTransform.offsetMin = new Vector2(20f, 8f);
            rewardDetailHintText.rectTransform.offsetMax = new Vector2(-20f, 28f);

            RectTransform rewardInfoPanel = CreateInkPanel(
                rewardOverlayRoot.transform,
                "RewardInfoPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-650f, 18f),
                new Vector2(-14f, 142f),
                new Color(0.94f, 0.89f, 0.77f, 0.80f));
            DecorateFrame(rewardInfoPanel, new Color(0.46f, 0.32f, 0.16f, 0.26f), new Color(1f, 1f, 1f, 0.05f), 1f, false);
            RectTransform rewardInfoVeil = CreateStretchPanel(
                rewardInfoPanel,
                "RewardInfoVeil",
                Vector2.zero,
                Vector2.one,
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.99f, 0.96f, 0.88f, 0.30f));
            rewardInfoVeil.GetComponent<Image>().raycastTarget = false;
            Text rewardInfoTitle = CreatePinnedPanelTitle(rewardInfoPanel, "当前道基");
            rewardInfoTitle.fontSize = 16;
            rewardInfoTitle.color = new Color(0.31f, 0.22f, 0.12f, 0.94f);
            rewardBuildText = CreateBodyText(rewardInfoPanel, "RewardBuildText", 13, new Color(0.24f, 0.24f, 0.20f, 0.90f));
            rewardBuildText.lineSpacing = 1.08f;
            StretchText(rewardBuildText.rectTransform, new Vector2(18f, 38f), new Vector2(-18f, -12f));
            rewardInfoPanelRoot = rewardInfoPanel.gameObject;

            RectTransform rewardDeckPanel = CreateInkPanel(
                rewardOverlayRoot.transform,
                "RewardDeckPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(14f, 18f),
                new Vector2(650f, 142f),
                new Color(0.94f, 0.89f, 0.77f, 0.80f));
            DecorateFrame(rewardDeckPanel, new Color(0.46f, 0.32f, 0.16f, 0.26f), new Color(1f, 1f, 1f, 0.05f), 1f, false);
            RectTransform rewardDeckVeil = CreateStretchPanel(
                rewardDeckPanel,
                "RewardDeckVeil",
                Vector2.zero,
                Vector2.one,
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.99f, 0.96f, 0.88f, 0.30f));
            rewardDeckVeil.GetComponent<Image>().raycastTarget = false;
            Text rewardDeckTitle = CreatePinnedPanelTitle(rewardDeckPanel, "当前牌组方向");
            rewardDeckTitle.fontSize = 16;
            rewardDeckTitle.color = new Color(0.31f, 0.22f, 0.12f, 0.94f);
            rewardDeckText = CreateBodyText(rewardDeckPanel, "RewardDeckText", 13, new Color(0.24f, 0.24f, 0.20f, 0.90f));
            rewardDeckText.lineSpacing = 1.08f;
            StretchText(rewardDeckText.rectTransform, new Vector2(18f, 38f), new Vector2(-18f, -12f));
            rewardDeckPanelRoot = rewardDeckPanel.gameObject;
        }

        private void BuildOpeningSceneTransitionOverlay(RectTransform parent)
        {
            openingSceneTransitionRoot = new GameObject("OpeningSceneTransitionRoot", typeof(RectTransform), typeof(Image), typeof(CanvasGroup));
            openingSceneTransitionRoot.transform.SetParent(parent, false);

            RectTransform rootRect = openingSceneTransitionRoot.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            openingSceneTransitionVeilImage = openingSceneTransitionRoot.GetComponent<Image>();
            openingSceneTransitionVeilImage.color = new Color(0.02f, 0.025f, 0.03f, 0f);
            openingSceneTransitionVeilImage.raycastTarget = true;

            Sprite transitionInkWipe = LoadTransitionInkWipeSprite();
            Sprite transitionFocusRing = LoadTransitionFocusRingSprite();
            Sprite transitionCloud = LoadRootSmokeWispSprite();

            RectTransform blurRect = CreateFixedPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionBlur",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(1480f, 880f),
                new Color(1f, 1f, 1f, 0f));
            openingSceneTransitionBlurRect = blurRect;
            openingSceneTransitionBlurImage = blurRect.GetComponent<Image>();
            openingSceneTransitionBlurImage.raycastTarget = false;

            openingSceneTransitionFocusRect = CreateFixedPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionFocus",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(680f, 520f),
                new Color(1f, 1f, 1f, 0f));
            openingSceneTransitionFocusImage = openingSceneTransitionFocusRect.GetComponent<Image>();
            openingSceneTransitionFocusImage.raycastTarget = false;
            openingSceneTransitionFocusCanvasGroup = openingSceneTransitionFocusRect.gameObject.AddComponent<CanvasGroup>();
            DecorateFrame(openingSceneTransitionFocusRect, new Color(0.98f, 0.86f, 0.58f, 0.64f), new Color(0.14f, 0.09f, 0.04f, 0.18f), 2f, true);

            RectTransform focusWash = CreateStretchPanel(
                openingSceneTransitionFocusRect,
                "OpeningSceneTransitionFocusWash",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.98f, 0.88f, 0.60f, 0f));
            openingSceneTransitionFocusWashImage = focusWash.GetComponent<Image>();
            openingSceneTransitionFocusWashImage.raycastTarget = false;

            openingSceneTransitionFocusRingRect = CreateFixedPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionFocusRing",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(760f, 590f),
                new Color(1f, 1f, 1f, 0f));
            openingSceneTransitionFocusRingImage = openingSceneTransitionFocusRingRect.GetComponent<Image>();
            ApplySpriteToImage(openingSceneTransitionFocusRingRect, transitionFocusRing, new Color(1f, 1f, 1f, 0f));
            openingSceneTransitionFocusRingImage.raycastTarget = false;

            openingSceneTransitionInkWipeRect = CreateStretchPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionInkWipe",
                Vector2.zero,
                Vector2.one,
                new Vector2(-180f, -120f),
                new Vector2(180f, 120f),
                new Color(0.04f, 0.045f, 0.05f, 0f));
            openingSceneTransitionInkWipeImage = openingSceneTransitionInkWipeRect.GetComponent<Image>();
            ApplySpriteToImage(openingSceneTransitionInkWipeRect, transitionInkWipe, transitionInkWipe != null ? new Color(1f, 1f, 1f, 0f) : new Color(0.03f, 0.035f, 0.04f, 0f));
            openingSceneTransitionInkWipeImage.raycastTarget = false;

            openingSceneTransitionLeftCloudRect = CreateStretchPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionLeftCloud",
                new Vector2(0f, 0f),
                new Vector2(0.58f, 1f),
                new Vector2(-260f, -160f),
                new Vector2(180f, 160f),
                new Color(0.82f, 0.85f, 0.82f, 0f));
            openingSceneTransitionLeftCloudImage = openingSceneTransitionLeftCloudRect.GetComponent<Image>();
            ApplySpriteToImage(openingSceneTransitionLeftCloudRect, transitionCloud != null ? transitionCloud : transitionInkWipe, new Color(1f, 1f, 1f, 0f), false);
            openingSceneTransitionLeftCloudImage.raycastTarget = false;

            openingSceneTransitionRightCloudRect = CreateStretchPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionRightCloud",
                new Vector2(0.42f, 0f),
                new Vector2(1f, 1f),
                new Vector2(-180f, -160f),
                new Vector2(260f, 160f),
                new Color(0.82f, 0.85f, 0.82f, 0f));
            openingSceneTransitionRightCloudImage = openingSceneTransitionRightCloudRect.GetComponent<Image>();
            ApplySpriteToImage(openingSceneTransitionRightCloudRect, transitionCloud != null ? transitionCloud : transitionInkWipe, new Color(1f, 1f, 1f, 0f), false);
            openingSceneTransitionRightCloudImage.raycastTarget = false;
            openingSceneTransitionRightCloudRect.localScale = new Vector3(-1f, 1f, 1f);

            openingSceneTransitionGateGlowRect = CreateFixedPanel(
                openingSceneTransitionRoot.transform,
                "OpeningSceneTransitionGateGlow",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(500f, 920f),
                new Color(0.90f, 0.94f, 0.92f, 0f));
            openingSceneTransitionGateGlowImage = openingSceneTransitionGateGlowRect.GetComponent<Image>();
            ApplySpriteToImage(openingSceneTransitionGateGlowRect, transitionFocusRing, new Color(1f, 1f, 1f, 0f), false);
            openingSceneTransitionGateGlowImage.raycastTarget = false;

            openingSceneTransitionTitleText = CreateText(openingSceneTransitionRoot.transform, "OpeningSceneTransitionTitle", string.Empty, 42, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.96f, 0.91f, 0.78f, 0f));
            openingSceneTransitionTitleText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            openingSceneTransitionTitleText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            openingSceneTransitionTitleText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            openingSceneTransitionTitleText.rectTransform.anchoredPosition = new Vector2(0f, -404f);
            openingSceneTransitionTitleText.rectTransform.sizeDelta = new Vector2(980f, 54f);

            openingSceneTransitionHintText = CreateText(openingSceneTransitionRoot.transform, "OpeningSceneTransitionHint", string.Empty, 17, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.72f, 0.78f, 0.78f, 0f));
            openingSceneTransitionHintText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            openingSceneTransitionHintText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            openingSceneTransitionHintText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
            openingSceneTransitionHintText.rectTransform.anchoredPosition = new Vector2(0f, -448f);
            openingSceneTransitionHintText.rectTransform.sizeDelta = new Vector2(1180f, 34f);

            openingSceneTransitionRoot.SetActive(false);
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


            Sprite battleActionButtonSurface = LoadBattleActionButtonSprite();

            RectTransform hintPanel = CreateStretchPanel(
                battleHudRoot.transform,
                "BattleHintPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-340f, 182f),
                new Vector2(340f, 236f),
                new Color(0.94f, 0.92f, 0.86f, 0.72f));
            legacyBattleHintPanelRoot = hintPanel.gameObject;
            battleStateText = CreateText(hintPanel, "BattleStateText", string.Empty, 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.22f, 0.22f, 0.20f, 0.94f));
            StretchText(battleStateText.rectTransform, new Vector2(16f, 22f), new Vector2(-16f, -4f));
            battleHintText = CreateText(hintPanel, "BattleHintText", string.Empty, 11, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.38f, 0.40f, 0.38f, 0.88f));
            StretchText(battleHintText.rectTransform, new Vector2(16f, 4f), new Vector2(-16f, -22f));
            hintPanel.gameObject.SetActive(false);

            RectTransform handPanel = CreateFixedPanel(
                battleHudRoot.transform,
                "HandHudPanel",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(-42f, 16f),
                new Vector2(1320f, 224f),
                new Color(0.10f, 0.10f, 0.095f, 0.78f));
            legacyBattleHandPanelRoot = handPanel.gameObject;


            BuildBattleDeckSurface(handPanel);
            handTitleText = CreateText(handPanel, "HandTitle", "手牌", 15, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            handTitleText.rectTransform.anchorMin = new Vector2(0.26f, 1f);
            handTitleText.rectTransform.anchorMax = new Vector2(0.46f, 1f);
            handTitleText.rectTransform.offsetMin = new Vector2(4f, -34f);
            handTitleText.rectTransform.offsetMax = new Vector2(-6f, -5f);

            handInfoText = CreateText(handPanel, "HandInfo", string.Empty, 12, FontStyle.Normal, TextAnchor.UpperLeft, ColorMist);
            handInfoText.rectTransform.anchorMin = new Vector2(0.46f, 1f);
            handInfoText.rectTransform.anchorMax = new Vector2(0.78f, 1f);
            handInfoText.rectTransform.offsetMin = new Vector2(8f, -31f);
            handInfoText.rectTransform.offsetMax = new Vector2(-6f, -5f);

            battleFlowText = CreateText(handPanel, "BattleFlow", string.Empty, 11, FontStyle.Bold, TextAnchor.MiddleRight, ColorGoldDim);
            battleFlowText.rectTransform.anchorMin = new Vector2(0.68f, 1f);
            battleFlowText.rectTransform.anchorMax = new Vector2(1f, 1f);
            battleFlowText.rectTransform.offsetMin = new Vector2(6f, -27f);
            battleFlowText.rectTransform.offsetMax = new Vector2(-216f, -5f);

            GameObject containerObject = new GameObject("HandContainer", typeof(RectTransform), typeof(HorizontalLayoutGroup));
            containerObject.transform.SetParent(handPanel, false);
            handContainer = containerObject.GetComponent<RectTransform>();
            handContainer.anchorMin = new Vector2(0f, 0f);
            handContainer.anchorMax = new Vector2(1f, 0f);
            handContainer.pivot = new Vector2(0.5f, 0f);
            handContainer.anchoredPosition = new Vector2(-115f, 14f);
            handContainer.sizeDelta = new Vector2(-270f, 178f);

            HorizontalLayoutGroup group = containerObject.GetComponent<HorizontalLayoutGroup>();
            group.spacing = 12f;
            group.padding = new RectOffset(0, 0, 0, 0);
            group.childAlignment = TextAnchor.LowerCenter;
            group.childControlHeight = true;
            group.childControlWidth = true;
            group.childForceExpandHeight = false;
            group.childForceExpandWidth = false;

            handEmptyText = CreateText(handPanel, "HandEmpty", string.Empty, 11, FontStyle.Normal, TextAnchor.MiddleCenter, new Color(0.66f, 0.70f, 0.72f, 1f));
            handEmptyText.rectTransform.anchorMin = new Vector2(0.68f, 0f);
            handEmptyText.rectTransform.anchorMax = new Vector2(0.84f, 0f);
            handEmptyText.rectTransform.offsetMin = new Vector2(0f, 14f);
            handEmptyText.rectTransform.offsetMax = new Vector2(0f, 38f);

            battleActionButton = CreateActionButton(
                handPanel,
                "BattleActionButton",
                out battleActionButtonText,
                new Color(0.22f, 0.17f, 0.10f, 0.96f),
                ColorPaper);
            RectTransform actionRect = battleActionButton.GetComponent<RectTransform>();
            actionRect.anchorMin = new Vector2(1f, 0f);
            actionRect.anchorMax = new Vector2(1f, 0f);
            actionRect.pivot = new Vector2(1f, 0f);
            actionRect.anchoredPosition = new Vector2(-142f, 16f);
            actionRect.sizeDelta = new Vector2(108f, 38f);
            if (battleActionButtonSurface != null)
            {
                ApplySpriteToImage(actionRect, battleActionButtonSurface, Color.white);
                battleActionButton.colors = CreateSpriteButtonColors(new Color(0.32f, 0.28f, 0.22f, 0.62f));
            }

            battleActionButton.onClick.AddListener(OnBattleActionClicked);
            BuildBattleDaoDock(handPanel);

            hintPanel.gameObject.SetActive(false);
            handPanel.gameObject.SetActive(false);

            GameObject commercialHudObject = new GameObject(
                "OpeningBattleCommercialHud",
                typeof(RectTransform),
                typeof(DemoBattleHudView));
            commercialHudObject.transform.SetParent(battleHudRoot.transform, false);
            RectTransform commercialHudRect = commercialHudObject.GetComponent<RectTransform>();
            commercialHudRect.anchorMin = Vector2.zero;
            commercialHudRect.anchorMax = Vector2.one;
            commercialHudRect.offsetMin = Vector2.zero;
            commercialHudRect.offsetMax = Vector2.zero;
            commercialBattleHudView = commercialHudObject.GetComponent<DemoBattleHudView>();
            commercialBattleHudView.Initialize(controller, uiFont);
            RefreshBattleHudMode();
        }

        private void BuildOpeningBackButton(RectTransform parent)
        {
            openingBackButton = CreateActionButton(
                parent,
                "OpeningBackButton",
                out Text backText,
                new Color(0.92f, 0.88f, 0.77f, 0.94f),
                new Color(0.22f, 0.20f, 0.16f, 0.96f));
            RectTransform rect = openingBackButton.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(44f, -38f);
            rect.sizeDelta = new Vector2(120f, 44f);
            backText.text = "返回上一步";
            backText.fontSize = 14;
            openingBackButton.onClick.AddListener(() =>
            {
                controller.BackOpeningStep();
                rewardSignature = string.Empty;
            });
            openingBackButton.gameObject.SetActive(false);
        }
        private void BuildBattleDeckSurface(RectTransform handPanel)
        {
            Image baseImage = handPanel.GetComponent<Image>();
            if (baseImage != null)
            {
                baseImage.color = Color.clear;
                baseImage.raycastTarget = false;
            }

            RectTransform handRail = CreateFixedPanel(
                handPanel,
                "HandRail",
                new Vector2(0.43f, 0f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 22f),
                new Vector2(760f, 2f),
                new Color(0.78f, 0.66f, 0.40f, 0.36f));
            Image railImage = handRail.GetComponent<Image>();
            if (railImage != null)
            {
                railImage.raycastTarget = false;
            }
        }
        private void BuildBattleDaoDock(Transform parent)
        {
            RectTransform dock = CreateFixedPanel(
                parent,
                "BattleDaoDock",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-14f, 14f),
                new Vector2(248f, 190f),
                new Color(0f, 0f, 0f, 0f));
            Image dockImage = dock.GetComponent<Image>();
            if (dockImage != null)
            {
                dockImage.raycastTarget = false;
            }

            Text dockTitle = CreateText(dock, "DockTitle", "道途联动", 13, FontStyle.Bold, TextAnchor.MiddleCenter, ColorGold);
            dockTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            dockTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            dockTitle.rectTransform.offsetMin = new Vector2(12f, -32f);
            dockTitle.rectTransform.offsetMax = new Vector2(-12f, -6f);

            RectTransform divineSlot = CreateFixedPanel(
                dock,
                "DivineState",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(12f, -42f),
                new Vector2(108f, 72f),
                new Color(0.12f, 0.10f, 0.075f, 0.58f));
            DecorateFrame(divineSlot, ColorGold, new Color(0.24f, 0.20f, 0.14f, 0.06f), 1.2f, true);
            battleDivineStateText = CreateText(divineSlot, "DivineText", "神通\n未悟", 12, FontStyle.Bold, TextAnchor.MiddleCenter, ColorGold);
            StretchText(battleDivineStateText.rectTransform, new Vector2(6f, 5f), new Vector2(-6f, -5f));

            RectTransform artifactSlot = CreateFixedPanel(
                dock,
                "ArtifactState",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-12f, -42f),
                new Vector2(108f, 72f),
                new Color(0.08f, 0.12f, 0.12f, 0.58f));
            DecorateFrame(artifactSlot, ColorJade, new Color(0.24f, 0.40f, 0.42f, 0.06f), 1.2f, true);
            battleArtifactStateText = CreateText(artifactSlot, "ArtifactText", "法器\n未携", 12, FontStyle.Bold, TextAnchor.MiddleCenter, ColorJade);
            StretchText(battleArtifactStateText.rectTransform, new Vector2(6f, 5f), new Vector2(-6f, -5f));


            battleSpeedButton = CreateActionButton(
                dock,
                "BattleSpeedButton",
                out battleSpeedButtonText,
                new Color(0.12f, 0.13f, 0.12f, 0.90f),
                ColorPaper);
            RectTransform speedRect = battleSpeedButton.GetComponent<RectTransform>();
            speedRect.anchorMin = new Vector2(1f, 0f);
            speedRect.anchorMax = new Vector2(1f, 0f);
            speedRect.pivot = new Vector2(1f, 0f);
            speedRect.anchoredPosition = new Vector2(-12f, 12f);
            speedRect.sizeDelta = new Vector2(108f, 38f);
            battleSpeedButton.onClick.AddListener(OnBattleSpeedClicked);
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
            headerTitleText.text = BuildHeaderTitleText();
            headerSummaryText.text = BuildTopBarSummary();
            headerChipText.text = GetHeaderChipText();
            headerContextText.text = BuildHeaderContextText();

            if (controller.HasRunResult)
            {
                nodeTitleText.text = controller.RunSummary.Victory ? "一世已成" : "此世止步";
                nodeBodyText.text = controller.GetRunResultSummary();
                nodeMapText.text = BuildResultNodeMapSummary();
                nodeBuildText.text = BuildResultNodeBuildSummary();
            }
            else
            {
                nodeTitleText.text = controller.Run.Map.CurrentNode.Name;
                nodeBodyText.text = controller.GetBattleSummary();
                nodeMapText.text = BuildCompactNodeMapSummary();
                nodeBuildText.text = BuildCompactNodeBuildSummary();
            }
            if (nodeStartBaseText != null)
            {
                nodeStartBaseText.text = BuildStartBaseScrollText();
            }

            if (nodeStartInheritanceText != null)
            {
                nodeStartInheritanceText.text = BuildStartInheritanceScrollText();
            }

            RefreshNodeChoiceStage();

            rewardTitleText.text = IsStartOpeningScreen()
                ? BuildStartStageTitle()
                : controller.Run.Map.CurrentNode.Name;
            rewardBodyText.text = BuildRewardLeadText();
            rewardBuildText.text = BuildRunSnapshot();
            rewardDeckText.text = BuildDeckFocusSummary();
            RefreshRewardLayout();
            rewardSectionTitleText.text = BuildRewardSectionTitle();
            rewardSectionHintText.text = BuildRewardSectionHint();
            if (postBattleChapterText != null)
            {
                postBattleChapterText.text = IsRouteChoiceScreen() ? "择\n前\n路" : "战\n后\n所\n得";
            }

            if (postBattleTrailText != null)
            {
                postBattleTrailText.text = IsRouteChoiceScreen()
                    ? "所往  ·  入矿  ·  因果抉择  ·  深入旧矿"
                    : "所往  ·  入矿  ·  战痕归卷";
            }

            RefreshRewardHeaderStyle();
            RefreshOpeningSceneGuidance();

            if (handTitleText != null)
            {
                handTitleText.text = BuildHandTitleText();
            }

            handInfoText.text = BuildHandInfoText();
            if (battleFlowText != null)
            {
                battleFlowText.text = BuildBattleFlowText();
                battleFlowText.gameObject.SetActive(false);
            }
            if (battleDivineStateText != null)
            {
                battleDivineStateText.text = "神通\n" + GetGongfaName(controller.Run.DivineGongfa, "未悟");
            }

            if (battleArtifactStateText != null)
            {
                string artifactName = controller.Run.Artifacts.Count > 0
                    ? DemoArtifactLibrary.Get(controller.Run.Artifacts[0]).Name
                    : "未携";
                battleArtifactStateText.text = "法器\n" + artifactName;
            }

            handEmptyText.text = BuildHandHint();

            if (controller.HasRunResult)
            {
                battleStateText.text = controller.RunSummary.Victory ? "剑傀已止，旧契归卷。" : "此世止步，道基归卷。";
                battleStateText.color = controller.RunSummary.Victory
                    ? new Color(0.95f, 0.84f, 0.45f, 1f)
                    : new Color(0.82f, 0.38f, 0.34f, 1f);
                battleHintText.text = controller.GetRunResultSummary();
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
                battleStateText.text = "这一场败在节奏与收束。";
                battleStateText.color = new Color(0.92f, 0.42f, 0.38f, 1f);
                battleHintText.text = "重新开局时更早定根脚、所携、所往，把灵气留给关键意图窗口。";
            }
            else
            {
                battleStateText.text = BuildBattleStateTitle();
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
            bool showRootOpeningRewards = AreStartRewardsOfType(DemoRewardType.Root);
            bool showOpeningItemRewards = IsStartOpeningItemScreen();
            bool showOpeningSceneRewards = IsStartOpeningSceneScreen();
            bool showStartOpeningRewards = IsStartOpeningScreen();
            bool showRouteChoiceRewards = IsRouteChoiceScreen();
            bool showMinimalRewardPanels = showStartOpeningRewards || showRouteChoiceRewards;
            bool showPostBattleJourney = showingRewards && !showStartOpeningRewards;

            if (openingBackButton != null)
            {
                openingBackButton.gameObject.SetActive(showStartOpeningRewards && controller.CanBackOpening);
            }

            if (nodeOverlayRoot != null)
            {
                nodeOverlayRoot.SetActive(!showingBattle && !showingRewards);

                Image nodeOverlayImage = nodeOverlayRoot.GetComponent<Image>();
                if (nodeOverlayImage != null)
                {
                    nodeOverlayImage.color = showStartHome
                        ? new Color(0f, 0f, 0f, 0f)
                        : new Color(0.93f, 0.90f, 0.82f, 0.08f);
                }
            }

            if (nodeOverlayPanelRoot != null)
            {
                nodeOverlayPanelRoot.SetActive(!showStartHome);
            }

            if (nodeJourneyBackdropRoot != null)
            {
                nodeJourneyBackdropRoot.SetActive(!showStartHome);
            }

            if (rewardOverlayRoot != null)
            {
                rewardOverlayRoot.SetActive(showingRewards);

                Image rewardOverlayImage = rewardOverlayRoot.GetComponent<Image>();
                if (rewardOverlayImage != null)
                {
                    rewardOverlayImage.color = showRootOpeningRewards || showOpeningItemRewards
                        ? new Color(0.018f, 0.020f, 0.020f, 0.03f)
                        : showOpeningSceneRewards
                            ? new Color(0.96f, 0.93f, 0.86f, 0.03f)
                        : new Color(0.93f, 0.90f, 0.82f, 0.08f);
                }
            }

            if (postBattleBackdropRoot != null)
            {
                postBattleBackdropRoot.SetActive(showPostBattleJourney);
            }

            if (rootDestinyBackdropRoot != null)
            {
                rootDestinyBackdropRoot.SetActive(showingRewards && (showRootOpeningRewards || showOpeningItemRewards));
            }

            if (openingSceneBackdropRoot != null)
            {
                openingSceneBackdropRoot.SetActive(showingRewards && showOpeningSceneRewards);
            }

            if (openingSceneBackdropImage != null)
            {
                openingSceneBackdropImage.color = new Color(1f, 1f, 1f, showOpeningSceneRewards ? 0.98f : 0f);
            }

            if (rootDestinyBackdropVeilImage != null)
            {
                rootDestinyBackdropVeilImage.color = new Color(0.015f, 0.018f, 0.018f, 0.06f);
            }

            if (battleHudRoot != null)
            {
                battleHudRoot.SetActive(showingBattle);
            }

            if (topHudRoot != null)
            {
                topHudRoot.SetActive(!hideTopHud && !showingBattle && !showingRewards);
            }

            if (rewardInfoPanelRoot != null)
            {
                rewardInfoPanelRoot.SetActive(showingRewards && !showMinimalRewardPanels);
            }

            if (rewardDeckPanelRoot != null)
            {
                rewardDeckPanelRoot.SetActive(showingRewards && !showMinimalRewardPanels);
            }

            if (rewardRerollButton != null)
            {
                rewardRerollButton.gameObject.SetActive(controller.CanRerollCurrentRewards);
                rewardRerollButton.interactable = controller.CanRerollCurrentRewards;
            }

            if (rewardRerollButtonText != null)
            {
                rewardRerollButtonText.text = $"重铸所得 · {controller.Run.OpeningRewardRerolls}";
            }
            utilityButton.gameObject.SetActive(!showStartHome && !showingBattle && !showingRewards && controller.CanAdvanceUtilityNode);
            utilityButtonText.text = showStartHome ? "踏入此局" : controller.UtilityActionLabel;
            utilityButtonText.fontSize = showStartHome ? 19 : 17;

            if (homeStartButton != null)
            {
                homeStartButton.gameObject.SetActive(showStartHome && !showingBattle && !showingRewards);
                homeStartButton.interactable = true;
            }

            if (homeStartButtonText != null)
            {
                homeStartButtonText.text = "开始新局";
            }

            if (homeContinueButton != null)
            {
                homeContinueButton.gameObject.SetActive(showStartHome && !showingBattle && !showingRewards);
                homeContinueButton.interactable = true;
            }

            if (homeContinueButtonText != null)
            {
                homeContinueButtonText.text = "继续悟道";
                homeContinueButtonText.color = new Color(0.24f, 0.23f, 0.19f, 0.88f);
            }

            if (homeCodexButton != null)
            {
                homeCodexButton.gameObject.SetActive(false);
                homeCodexButton.interactable = true;
            }

            if (homeSettingsButton != null)
            {
                homeSettingsButton.gameObject.SetActive(false);
                homeSettingsButton.interactable = true;
            }

            if (homeCodexButtonText != null)
            {
                homeCodexButtonText.color = new Color(0.24f, 0.23f, 0.19f, 0.88f);
            }

            if (homeSettingsButtonText != null)
            {
                homeSettingsButtonText.color = new Color(0.24f, 0.23f, 0.19f, 0.88f);
            }

            if (homeTopActionRoot != null)
            {
                homeTopActionRoot.SetActive(showStartHome);
            }

            if (homeModalRoot != null && !showStartHome)
            {
                homeModalRoot.SetActive(false);
            }

            if (utilityButtonRect != null)
            {
                utilityButtonRect.anchoredPosition = showStartHome
                    ? new Vector2(-432f, 48f)
                    : new Vector2(0f, 72f);
                utilityButtonRect.sizeDelta = showStartHome
                    ? new Vector2(360f, 76f)
                    : new Vector2(292f, 68f);

                Sprite utilitySprite = showStartHome ? LoadHomePrimaryButtonSprite() : LoadHeaderCloudBandSprite();
                ApplySpriteToImage(
                    utilityButtonRect,
                    utilitySprite,
                    utilitySprite != null ? Color.white : new Color(0.23f, 0.18f, 0.11f, 0.98f));
            }

            bool battleTimeActive = showingBattle &&
                (controller.Battle.Phase == DemoBattlePhase.Intro || controller.Battle.Phase == DemoBattlePhase.Running);
            if (!showingBattle && controller.BattleSpeed <= 0.01f)
            {
                controller.BattleSpeed = resumeBattleSpeed;
            }
            else if (showingBattle && controller.BattleSpeed > 0.01f)
            {
                resumeBattleSpeed = controller.BattleSpeed >= 1.9f ? 2f : 1f;
            }

            bool battlePaused = battleTimeActive && controller.BattleSpeed <= 0.01f;
            battleActionButton.gameObject.SetActive(battleTimeActive);
            battleActionButtonText.text = battlePaused ? "继续" : "暂停";
            battleActionButton.image.color = battlePaused
                ? new Color(0.19f, 0.30f, 0.29f, 0.96f)
                : new Color(0.12f, 0.13f, 0.12f, 0.90f);
            battleActionButtonText.color = battlePaused ? ColorJade : ColorMist;

            float displayedBattleSpeed = controller.BattleSpeed > 0.01f ? controller.BattleSpeed : resumeBattleSpeed;
            if (battleSpeedButton != null)
            {
                battleSpeedButton.gameObject.SetActive(battleTimeActive);
                battleSpeedButton.image.color = displayedBattleSpeed >= 1.9f
                    ? new Color(0.30f, 0.22f, 0.12f, 0.96f)
                    : new Color(0.12f, 0.13f, 0.12f, 0.90f);
            }

            if (battleSpeedButtonText != null)
            {
                battleSpeedButtonText.text = displayedBattleSpeed >= 1.9f ? "倍速 · ×2" : "倍速 · ×1";
                battleSpeedButtonText.color = displayedBattleSpeed >= 1.9f ? ColorGold : ColorMist;
            }
        }

        private void RefreshRewardLayout()
        {
            if (rewardContainer == null || rewardLayoutGroup == null)
            {
                return;
            }

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                rewardContainer.anchoredPosition = Vector2.zero;
                rewardContainer.sizeDelta = new Vector2(1920f, 1080f);
                rewardLayoutGroup.spacing = 0f;
                rewardLayoutGroup.padding = new RectOffset(0, 0, 0, 0);
                return;
            }

            if (IsStartOpeningItemScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, 18f);
                rewardContainer.sizeDelta = new Vector2(1360f, 600f);
                rewardLayoutGroup.spacing = 30f;
                rewardLayoutGroup.padding = new RectOffset(24, 24, 0, 0);
                return;
            }

            if (IsStartOpeningSceneScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, -64f);
                rewardContainer.sizeDelta = new Vector2(1660f, 704f);
                rewardLayoutGroup.spacing = 24f;
                rewardLayoutGroup.padding = new RectOffset(20, 20, 8, 8);
                return;
            }

            if (IsStartOpeningScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, 24f);
                rewardContainer.sizeDelta = new Vector2(1450f, 620f);
                rewardLayoutGroup.spacing = 24f;
                rewardLayoutGroup.padding = new RectOffset(20, 20, 12, 12);
                return;
            }

            if (IsRouteChoiceScreen())
            {
                rewardContainer.anchoredPosition = new Vector2(0f, -22f);
                rewardContainer.sizeDelta = new Vector2(1420f, 610f);
                rewardLayoutGroup.spacing = 24f;
                rewardLayoutGroup.padding = new RectOffset(20, 20, 8, 8);
                return;
            }

            rewardContainer.anchoredPosition = new Vector2(0f, -16f);
            rewardContainer.sizeDelta = new Vector2(1460f, 548f);
            rewardLayoutGroup.spacing = 32f;
            rewardLayoutGroup.padding = new RectOffset(24, 24, 12, 12);
        }

        private void RefreshRewardHeaderStyle()
        {
            bool showingRootDestiny = AreStartRewardsOfType(DemoRewardType.Root);
            bool showingOpeningItem = IsStartOpeningItemScreen();
            bool showingOpeningScene = IsStartOpeningSceneScreen();

            if (rewardTitleText != null)
            {
                rewardTitleText.fontSize = showingRootDestiny ? 30 : showingOpeningItem ? 38 : showingOpeningScene ? 42 : 40;
                rewardTitleText.alignment = TextAnchor.UpperCenter;
                rewardTitleText.rectTransform.anchoredPosition = showingRootDestiny
                    ? new Vector2(0f, -86f)
                    : showingOpeningItem
                        ? new Vector2(0f, -72f)
                        : showingOpeningScene
                            ? new Vector2(0f, -68f)
                    : new Vector2(0f, -84f);
                rewardTitleText.rectTransform.sizeDelta = showingRootDestiny
                    ? new Vector2(720f, 40f)
                    : showingOpeningItem
                        ? new Vector2(760f, 48f)
                        : showingOpeningScene
                            ? new Vector2(860f, 56f)
                    : new Vector2(980f, 50f);
                rewardTitleText.color = showingRootDestiny
                    ? new Color(0.17f, 0.12f, 0.08f, 0.86f)
                    : showingOpeningItem
                        ? new Color(0.17f, 0.12f, 0.08f, 0.94f)
                        : showingOpeningScene
                            ? new Color(0.18f, 0.12f, 0.07f, 0.94f)
                    : new Color(0.17f, 0.14f, 0.10f, 0.98f);
            }

            if (rewardBodyText != null)
            {
                rewardBodyText.fontSize = showingRootDestiny ? 15 : showingOpeningItem ? 16 : showingOpeningScene ? 17 : 18;
                rewardBodyText.rectTransform.anchoredPosition = showingRootDestiny
                    ? new Vector2(0f, -124f)
                    : showingOpeningItem
                        ? new Vector2(0f, -118f)
                        : showingOpeningScene
                            ? new Vector2(0f, -124f)
                    : new Vector2(0f, -132f);
                rewardBodyText.rectTransform.sizeDelta = showingRootDestiny
                    ? new Vector2(880f, 34f)
                    : showingOpeningItem
                        ? new Vector2(920f, 34f)
                        : showingOpeningScene
                            ? new Vector2(1180f, 42f)
                    : new Vector2(1160f, 42f);
                rewardBodyText.color = showingRootDestiny
                    ? new Color(0.25f, 0.18f, 0.12f, 0.68f)
                    : showingOpeningItem
                        ? new Color(0.27f, 0.20f, 0.13f, 0.72f)
                        : showingOpeningScene
                            ? new Color(0.31f, 0.24f, 0.16f, 0.76f)
                    : new Color(0.32f, 0.30f, 0.25f, 0.88f);
            }

            if (rewardSectionTitleText != null)
            {
                rewardSectionTitleText.fontSize = showingRootDestiny ? 15 : showingOpeningItem ? 19 : showingOpeningScene ? 18 : 22;
                rewardSectionTitleText.rectTransform.anchoredPosition = showingRootDestiny
                    ? new Vector2(0f, -158f)
                    : showingOpeningItem
                        ? new Vector2(0f, -154f)
                        : showingOpeningScene
                            ? new Vector2(0f, -170f)
                    : new Vector2(0f, -178f);
                rewardSectionTitleText.rectTransform.sizeDelta = showingRootDestiny
                    ? new Vector2(560f, 24f)
                    : showingOpeningItem
                        ? new Vector2(620f, 28f)
                        : showingOpeningScene
                            ? new Vector2(940f, 28f)
                    : new Vector2(840f, 34f);
                rewardSectionTitleText.color = showingRootDestiny
                    ? new Color(0.50f, 0.31f, 0.14f, 0.72f)
                    : showingOpeningItem
                        ? new Color(0.53f, 0.35f, 0.15f, 0.82f)
                        : showingOpeningScene
                            ? new Color(0.56f, 0.34f, 0.14f, 0.86f)
                    : new Color(0.52f, 0.16f, 0.10f, 0.92f);
            }

            if (rewardSectionHintText != null)
            {
                rewardSectionHintText.fontSize = showingRootDestiny ? 12 : showingOpeningItem ? 14 : showingOpeningScene ? 13 : 14;
                rewardSectionHintText.rectTransform.anchoredPosition = showingRootDestiny
                    ? new Vector2(0f, -181f)
                    : showingOpeningItem
                        ? new Vector2(0f, -181f)
                        : showingOpeningScene
                            ? new Vector2(0f, -196f)
                    : new Vector2(0f, -208f);
                rewardSectionHintText.rectTransform.sizeDelta = showingRootDestiny
                    ? new Vector2(660f, 22f)
                    : showingOpeningItem
                        ? new Vector2(840f, 24f)
                        : showingOpeningScene
                            ? new Vector2(1040f, 24f)
                    : new Vector2(980f, 28f);
                rewardSectionHintText.color = showingRootDestiny
                    ? new Color(0.34f, 0.27f, 0.18f, 0.52f)
                    : showingOpeningItem
                        ? new Color(0.34f, 0.27f, 0.18f, 0.60f)
                        : showingOpeningScene
                            ? new Color(0.34f, 0.28f, 0.20f, 0.66f)
                    : new Color(0.39f, 0.35f, 0.27f, 0.82f);
            }
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
            if (RefreshBattleHudMode())
            {
                commercialBattleHudView.RefreshNow();
                handSignature = string.Empty;
                return;
            }

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

            string signature = controller.GetHandStatus() + "|" + controller.Battle.Energy + "|" + controller.Battle.Phase + "|" +
                controller.Battle.Hand.Count + "|" + controller.BattleSpeed;
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

            bool canPlayCards = controller.Battle.Phase == DemoBattlePhase.Running && controller.BattleSpeed > 0.01f;
            int visibleCardCount = Mathf.Max(1, controller.Battle.Hand.Count);
            float cardWidth = Mathf.Clamp((1050f - (visibleCardCount - 1) * 12f) / visibleCardCount, 136f, 168f);
            for (int i = 0; i < controller.Battle.Hand.Count; i++)
            {
                int capturedIndex = i;
                DemoCard card = controller.Battle.Hand[i];
                if (card == null)
                {
                    continue;
                }

                GameObject cardObject = CreateCardEntry(
                    handContainer,
                    card,
                    canPlayCards && controller.Battle.Energy >= card.Cost,
                    cardWidth);
                Button button = cardObject.GetComponent<Button>();
                button.onClick.AddListener(() => controller.TryPlayCardAt(capturedIndex));
                handEntries.Add(cardObject);
            }

            handEmptyText.text = BuildHandHint();
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

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                GameObject rootDesk = CreateRootDestinyDeskEntry(rewardContainer, controller.CurrentRewards);
                rewardEntries.Add(rootDesk);
                return;
            }

            for (int i = 0; i < controller.CurrentRewards.Count; i++)
            {
                int capturedIndex = i;
                DemoReward reward = controller.CurrentRewards[i];
                DemoReward capturedReward = reward;
                GameObject rewardObject = CreateRewardEntry(rewardContainer, reward);
                RectTransform rewardRect = rewardObject.GetComponent<RectTransform>();
                Button button = rewardObject.GetComponent<Button>();
                bool claimable = IsRewardClaimable(reward);
                if (button != null)
                {
                    button.interactable = claimable;
                    if (claimable && reward.Type == DemoRewardType.OpeningScene)
                    {
                        button.onClick.AddListener(() => BeginOpeningSceneTransition(capturedIndex, rewardRect, capturedReward));
                    }
                    else if (claimable)
                    {
                        button.onClick.AddListener(() => controller.ClaimRewardAt(capturedIndex));
                    }
                }

                bool openingCardHasIntegratedLockState = reward.Type == DemoRewardType.Journey
                    || reward.Type == DemoRewardType.Vessel
                    || reward.Type == DemoRewardType.OpeningScene;
                if (!claimable && !openingCardHasIntegratedLockState)
                {
                    Text lockedText = CreateText(
                        rewardRect,
                        "LockedChoice",
                        "尚未开放",
                        14,
                        FontStyle.Bold,
                        TextAnchor.MiddleCenter,
                        new Color(0.48f, 0.17f, 0.12f, 0.94f));
                    lockedText.rectTransform.anchorMin = new Vector2(0.5f, 0f);
                    lockedText.rectTransform.anchorMax = new Vector2(0.5f, 0f);
                    lockedText.rectTransform.anchoredPosition = new Vector2(0f, 28f);
                    lockedText.rectTransform.sizeDelta = new Vector2(180f, 28f);
                    lockedText.raycastTarget = false;
                }

                rewardEntries.Add(rewardObject);
            }
        }

        private bool RefreshBattleHudMode()
        {
            bool useCommercialHud = controller != null
                && controller.HasBattle;

            if (commercialBattleHudView != null)
            {
                commercialBattleHudView.gameObject.SetActive(useCommercialHud);
            }

            // The legacy hint panel was intentionally hidden before the opening-battle HUD existed.
            if (legacyBattleHintPanelRoot != null)
            {
                legacyBattleHintPanelRoot.SetActive(false);
            }

            if (legacyBattleHandPanelRoot != null)
            {
                legacyBattleHandPanelRoot.SetActive(controller != null && controller.HasBattle && !useCommercialHud);
            }

            return useCommercialHud;
        }

        private void BeginOpeningSceneTransition(int rewardIndex, RectTransform sourceRect, DemoReward reward)
        {
            if (openingSceneTransitionActive || reward == null || reward.Type != DemoRewardType.OpeningScene)
            {
                return;
            }

            if (openingSceneTransitionRoot == null || controller == null)
            {
                controller?.ClaimRewardAt(rewardIndex);
                return;
            }

            HideRewardDetail();
            SetRewardButtonsInteractable(false);

            Sprite sceneSprite = LoadOpeningSprite(reward);
            openingSceneTransitionRewardIndex = rewardIndex;
            openingSceneTransitionTimer = 0f;
            openingSceneTransitionCommitted = false;
            openingSceneTransitionActive = true;
            openingSceneTransitionAccent = GetRewardAccentColor(reward);

            if (!TryGetRectInTransitionSpace(sourceRect, out openingSceneTransitionStartPosition, out openingSceneTransitionStartSize))
            {
                openingSceneTransitionStartPosition = new Vector2(0f, -42f);
                openingSceneTransitionStartSize = new Vector2(520f, 640f);
            }

            if (openingSceneTransitionFocusImage != null)
            {
                openingSceneTransitionFocusImage.sprite = sceneSprite;
                openingSceneTransitionFocusImage.type = Image.Type.Simple;
                openingSceneTransitionFocusImage.preserveAspect = true;
            }

            if (openingSceneTransitionBlurImage != null)
            {
                openingSceneTransitionBlurImage.sprite = sceneSprite;
                openingSceneTransitionBlurImage.type = Image.Type.Simple;
                openingSceneTransitionBlurImage.preserveAspect = true;
            }

            if (openingSceneTransitionTitleText != null)
            {
                openingSceneTransitionTitleText.text = reward.Name;
            }

            if (openingSceneTransitionHintText != null)
            {
                string tagline = reward.Region != null ? BuildOpeningSceneTagline(reward.Region) : "所往已定，气机入境。";
                openingSceneTransitionHintText.text = tagline;
            }

            if (rewardOverlayCanvasGroup != null)
            {
                rewardOverlayCanvasGroup.alpha = 1f;
                rewardOverlayCanvasGroup.interactable = false;
                rewardOverlayCanvasGroup.blocksRaycasts = false;
            }

            openingSceneTransitionRoot.SetActive(true);
            openingSceneTransitionRoot.transform.SetAsLastSibling();
            ApplyOpeningSceneTransitionProgress(0f);
        }

        private void UpdateOpeningSceneTransition(float deltaTime)
        {
            if (!openingSceneTransitionActive)
            {
                if (rewardOverlayCanvasGroup != null)
                {
                    rewardOverlayCanvasGroup.alpha = 1f;
                    rewardOverlayCanvasGroup.interactable = true;
                    rewardOverlayCanvasGroup.blocksRaycasts = true;
                }

                return;
            }

            openingSceneTransitionTimer += Mathf.Max(0f, deltaTime);
            float progress = Mathf.Clamp01(openingSceneTransitionTimer / OpeningSceneTransitionDuration);

            if (!openingSceneTransitionCommitted && progress >= OpeningSceneTransitionCommitProgress)
            {
                openingSceneTransitionCommitted = true;
                int rewardIndex = openingSceneTransitionRewardIndex;
                openingSceneTransitionRewardIndex = -1;
                if (rewardIndex >= 0)
                {
                    controller.ClaimRewardAt(rewardIndex);
                    battleSceneView?.RefreshForCurrentBattle();
                }
            }

            ApplyOpeningSceneTransitionProgress(progress);

            if (progress >= 1f)
            {
                FinishOpeningSceneTransition();
            }
        }

        private void ApplyOpeningSceneTransitionProgress(float progress)
        {
            float centerProgress = SmoothRange(progress, 0.02f, 0.30f);
            float blurProgress = SmoothRange(progress, 0.42f, 0.64f);
            float holdProgress = SmoothRange(progress, 0.18f, OpeningSceneTransitionCommitProgress);

            if (rewardOverlayCanvasGroup != null)
            {
                rewardOverlayCanvasGroup.alpha = Mathf.Lerp(1f, 0.12f, SmoothRange(progress, 0.05f, 0.48f));
            }

            if (openingSceneTransitionVeilImage != null)
            {
                float veilIn = Mathf.Lerp(0.05f, 0.82f, holdProgress);
                float veilOut = Mathf.Lerp(0.88f, 0f, SmoothRange(progress, 0.66f, 1f));
                float alpha = progress < OpeningSceneTransitionCommitProgress ? veilIn : veilOut;
                openingSceneTransitionVeilImage.color = new Color(0.02f, 0.024f, 0.03f, alpha);
            }

            float pushProgress = SmoothRange(progress, 0.30f, 0.84f);
            Vector2 centeredSize = openingSceneTransitionStartSize * Mathf.Lerp(1f, 1.52f, centerProgress);
            Vector2 focusSize = centeredSize * Mathf.Lerp(1f, 2.04f, pushProgress);

            if (openingSceneTransitionFocusRect != null)
            {
                openingSceneTransitionFocusRect.anchoredPosition = Vector2.Lerp(openingSceneTransitionStartPosition, Vector2.zero, centerProgress);
                openingSceneTransitionFocusRect.sizeDelta = focusSize;
                openingSceneTransitionFocusRect.localScale = Vector3.one;
            }

            if (openingSceneTransitionFocusRingRect != null)
            {
                openingSceneTransitionFocusRingRect.anchoredPosition = Vector2.Lerp(openingSceneTransitionStartPosition, Vector2.zero, centerProgress);
                openingSceneTransitionFocusRingRect.sizeDelta = focusSize + new Vector2(96f, 76f);
                openingSceneTransitionFocusRingRect.localScale = Vector3.one;
            }

            if (openingSceneTransitionFocusRingImage != null)
            {
                float ringAlpha = SmoothRange(progress, 0.03f, 0.20f) * (1f - SmoothRange(progress, 0.62f, 0.94f)) * 0.80f;
                openingSceneTransitionFocusRingImage.color = new Color(openingSceneTransitionAccent.r, openingSceneTransitionAccent.g, openingSceneTransitionAccent.b, ringAlpha);
            }

            if (openingSceneTransitionFocusCanvasGroup != null)
            {
                openingSceneTransitionFocusCanvasGroup.alpha = Mathf.Lerp(1f, 0f, SmoothRange(progress, 0.72f, 1f));
            }

            if (openingSceneTransitionFocusImage != null)
            {
                float imageAlpha = Mathf.Lerp(0.94f, 0.42f, blurProgress * 0.55f);
                imageAlpha = Mathf.Lerp(imageAlpha, 0f, SmoothRange(progress, 0.82f, 1f));
                openingSceneTransitionFocusImage.color = new Color(1f, 0.97f, 0.90f, imageAlpha);
            }

            if (openingSceneTransitionFocusWashImage != null)
            {
                float washAlpha = Mathf.Lerp(0.03f, 0.30f, blurProgress) * (1f - SmoothRange(progress, 0.82f, 1f));
                openingSceneTransitionFocusWashImage.color = new Color(openingSceneTransitionAccent.r, openingSceneTransitionAccent.g, openingSceneTransitionAccent.b, washAlpha);
            }

            if (openingSceneTransitionBlurRect != null)
            {
                openingSceneTransitionBlurRect.anchoredPosition = Vector2.Lerp(openingSceneTransitionStartPosition, Vector2.zero, centerProgress) + new Vector2(Mathf.Sin(progress * 28f) * 18f, Mathf.Cos(progress * 21f) * 10f) * blurProgress;
                openingSceneTransitionBlurRect.sizeDelta = focusSize * (1.04f + blurProgress * 0.16f);
                openingSceneTransitionBlurRect.localScale = Vector3.one;
            }

            if (openingSceneTransitionBlurImage != null)
            {
                float blurAlpha = Mathf.Sin(Mathf.Clamp01((progress - 0.34f) / 0.52f) * Mathf.PI) * 0.36f;
                blurAlpha = Mathf.Max(blurAlpha, blurProgress * 0.18f);
                blurAlpha *= 1f - SmoothRange(progress, 0.76f, 1f);
                openingSceneTransitionBlurImage.color = new Color(1f, 0.95f, 0.82f, blurAlpha);
            }

            if (openingSceneTransitionInkWipeRect != null)
            {
                float inkSpread = SmoothRange(progress, 0.34f, 0.82f);
                openingSceneTransitionInkWipeRect.anchoredPosition = new Vector2(Mathf.Lerp(48f, -28f, inkSpread), Mathf.Lerp(-20f, 18f, inkSpread));
                openingSceneTransitionInkWipeRect.localScale = Vector3.one * Mathf.Lerp(0.98f, 1.08f, inkSpread);
            }

            if (openingSceneTransitionInkWipeImage != null)
            {
                float inkAlpha = SmoothRange(progress, 0.34f, 0.62f) * (1f - SmoothRange(progress, 0.82f, 1f)) * 0.88f;
                Color fallbackInk = new Color(0.03f, 0.035f, 0.04f, inkAlpha * 0.92f);
                openingSceneTransitionInkWipeImage.color = openingSceneTransitionInkWipeImage.sprite != null ? new Color(1f, 1f, 1f, inkAlpha) : fallbackInk;
            }

            float gateInProgress = SmoothRange(progress, 0.52f, 0.62f);
            float gateOpenProgress = SmoothRange(progress, 0.64f, 0.96f);
            float gateFadeProgress = SmoothRange(progress, 0.90f, 1f);
            float gateAlpha = gateInProgress * (1f - gateFadeProgress);
            if (openingSceneTransitionLeftCloudRect != null)
            {
                float drift = Mathf.Sin(progress * 31f) * 10f;
                openingSceneTransitionLeftCloudRect.anchoredPosition = new Vector2(Mathf.Lerp(0f, -700f, gateOpenProgress), drift);
                openingSceneTransitionLeftCloudRect.localScale = new Vector3(1f + gateOpenProgress * 0.10f, 1f + gateInProgress * 0.06f, 1f);
            }

            if (openingSceneTransitionRightCloudRect != null)
            {
                float drift = Mathf.Cos(progress * 27f) * 10f;
                openingSceneTransitionRightCloudRect.anchoredPosition = new Vector2(Mathf.Lerp(0f, 700f, gateOpenProgress), drift);
                openingSceneTransitionRightCloudRect.localScale = new Vector3(-1f - gateOpenProgress * 0.10f, 1f + gateInProgress * 0.06f, 1f);
            }

            if (openingSceneTransitionLeftCloudImage != null)
            {
                openingSceneTransitionLeftCloudImage.color = new Color(0.82f, 0.86f, 0.84f, gateAlpha * 0.88f);
            }

            if (openingSceneTransitionRightCloudImage != null)
            {
                openingSceneTransitionRightCloudImage.color = new Color(0.82f, 0.86f, 0.84f, gateAlpha * 0.88f);
            }

            if (openingSceneTransitionGateGlowRect != null)
            {
                openingSceneTransitionGateGlowRect.sizeDelta = new Vector2(Mathf.Lerp(220f, 1220f, gateOpenProgress), Mathf.Lerp(720f, 1080f, gateOpenProgress));
                openingSceneTransitionGateGlowRect.localScale = Vector3.one * (1f + gateOpenProgress * 0.12f);
            }

            if (openingSceneTransitionGateGlowImage != null)
            {
                float glowAlpha = gateInProgress * Mathf.Sin(gateOpenProgress * Mathf.PI) * (1f - gateFadeProgress) * 0.42f;
                openingSceneTransitionGateGlowImage.color = new Color(openingSceneTransitionAccent.r, openingSceneTransitionAccent.g, openingSceneTransitionAccent.b, glowAlpha);
            }

            float textAlpha = SmoothRange(progress, 0.08f, 0.28f) * (1f - SmoothRange(progress, 0.70f, 0.96f));
            SetTextAlpha(openingSceneTransitionTitleText, textAlpha);
            SetTextAlpha(openingSceneTransitionHintText, textAlpha * 0.82f);

        }

        private void FinishOpeningSceneTransition()
        {
            openingSceneTransitionActive = false;
            openingSceneTransitionCommitted = false;
            openingSceneTransitionRewardIndex = -1;

            if (openingSceneTransitionRoot != null)
            {
                openingSceneTransitionRoot.SetActive(false);
            }

            if (rewardOverlayCanvasGroup != null)
            {
                rewardOverlayCanvasGroup.alpha = 1f;
                rewardOverlayCanvasGroup.interactable = true;
                rewardOverlayCanvasGroup.blocksRaycasts = true;
            }

            SetRewardButtonsInteractable(true);
        }

        private bool TryGetRectInTransitionSpace(RectTransform sourceRect, out Vector2 center, out Vector2 size)
        {
            center = Vector2.zero;
            size = Vector2.zero;
            if (sourceRect == null || openingSceneTransitionRoot == null)
            {
                return false;
            }

            RectTransform canvasRect = openingSceneTransitionRoot.transform.parent as RectTransform;
            if (canvasRect == null)
            {
                return false;
            }

            Vector3[] corners = new Vector3[4];
            sourceRect.GetWorldCorners(corners);
            Vector2 min;
            Vector2 max;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, corners[0]), null, out min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(null, corners[2]), null, out max);
            center = (min + max) * 0.5f;
            size = new Vector2(Mathf.Abs(max.x - min.x), Mathf.Abs(max.y - min.y));
            return size.x > 1f && size.y > 1f;
        }

        private void SetRewardButtonsInteractable(bool interactable)
        {
            for (int i = 0; i < rewardEntries.Count; i++)
            {
                if (rewardEntries[i] == null)
                {
                    continue;
                }

                Button button = rewardEntries[i].GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = interactable;
                }
            }
        }

        private static float SmoothRange(float value, float start, float end)
        {
            if (Mathf.Approximately(start, end))
            {
                return value >= end ? 1f : 0f;
            }

            return Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((value - start) / (end - start)));
        }

        private static void SetTextAlpha(Text text, float alpha)
        {
            if (text == null)
            {
                return;
            }

            Color color = text.color;
            color.a = Mathf.Clamp01(alpha);
            text.color = color;
        }

        private void OnUtilityButtonClicked()
        {
            controller.AdvanceUtilityNode();
        }

        private void OnStartRunClicked()
        {
            HideHomeModal();
            controller.BeginOpeningStory();
        }

        private void OnContinueRunClicked()
        {
            HideHomeModal();
            if (!controller.TryResumeConfiguredJourney())
            {
                ShowHomeModal("暂无可续道途", "此地没有可读取的旅程记录。可从一段新的旧矿因果开始。 ");
            }
        }

        private void OnRewardRerollClicked()
        {
            controller?.RerollCurrentRewards();
            rewardSignature = string.Empty;
        }
        private void OnBattleActionClicked()
        {
            if (!controller.HasBattle)
            {
                return;
            }

            if (controller.BattleSpeed <= 0.01f)
            {
                controller.BattleSpeed = resumeBattleSpeed;
                return;
            }

            resumeBattleSpeed = controller.BattleSpeed >= 1.9f ? 2f : 1f;
            controller.BattleSpeed = 0f;
        }

        private void OnBattleSpeedClicked()
        {
            float currentSpeed = controller.BattleSpeed > 0.01f
                ? controller.BattleSpeed
                : resumeBattleSpeed;
            resumeBattleSpeed = currentSpeed >= 1.9f ? 1f : 2f;
            if (controller.BattleSpeed > 0.01f)
            {
                controller.BattleSpeed = resumeBattleSpeed;
            }
        }
        private bool HasContinuableRun()
        {
            return controller != null
                && controller.Run != null
                && controller.Run.Map != null
                && controller.Run.Map.CurrentNode.Type != DemoNodeType.Start
                && !controller.IsRunComplete;
        }

        private void ShowHomeModal(string title, string body)
        {
            if (homeModalRoot == null)
            {
                return;
            }

            if (homeModalTitleText != null)
            {
                homeModalTitleText.text = title;
            }

            if (homeModalBodyText != null)
            {
                homeModalBodyText.text = body;
            }

            homeModalRoot.SetActive(true);
        }

        private void HideHomeModal()
        {
            if (homeModalRoot != null)
            {
                homeModalRoot.SetActive(false);
            }
        }

        private string BuildHomeCodexText()
        {
            return
                "当前 DEMO 先打磨世家旁支所往后的首战与前路循环。\n\n" +
                "图鉴页先作为轻量入口保留，后续会记录已见功法、法器、Boss 与通关流派。";
        }

        private string BuildHomeSettingsText()
        {
            return
                "设置页先保留为轻量入口。\n\n" +
                "后续优先接入音量、窗口模式、画面震动与战斗演出强度，不在首页扩成复杂大厅。";
        }

        private string BuildHomeExitText()
        {
            return
                "当前在编辑器或 DEMO 环境中不会强制关闭进程。\n\n" +
                "正式包中，这个入口会退出游戏或返回平台桌面。";
        }

        private void UpdateHomePresentation(float time)
        {
            bool showStartHome = IsStartHomeScreen();

            if (homeTitleRoot != null)
            {
                float titleBreath = showStartHome ? 1f + Mathf.Sin(time * 0.72f) * 0.009f : 1f;
                homeTitleRoot.localScale = new Vector3(titleBreath, titleBreath, 1f);
            }

            for (int i = 0; i < homeMistThreads.Count; i++)
            {
                RectTransform thread = homeMistThreads[i];
                if (thread == null)
                {
                    continue;
                }

                float drift = Mathf.Sin(time * (0.11f + i * 0.012f) + i * 1.7f);
                float rise = Mathf.Sin(time * (0.17f + i * 0.01f) + i * 0.8f);
                thread.anchoredPosition = new Vector2(-300f + i * 128f + drift * 32f, -92f + (i % 3) * 34f + rise * 4f);
            }

            float flashPhase = Mathf.Repeat(time + 1.7f, 8.6f);
            float flashAlpha = 0f;
            if (flashPhase < 0.14f)
            {
                flashAlpha = Mathf.Lerp(0.34f, 0f, flashPhase / 0.14f);
            }
            else if (flashPhase > 0.24f && flashPhase < 0.34f)
            {
                flashAlpha = Mathf.Lerp(0.22f, 0f, (flashPhase - 0.24f) / 0.10f);
            }

            for (int i = 0; i < homeLightningStrokes.Count; i++)
            {
                RectTransform stroke = homeLightningStrokes[i];
                if (stroke == null)
                {
                    continue;
                }

                stroke.GetComponent<Image>().color = new Color(0.78f, 0.86f, 0.86f, showStartHome ? flashAlpha * (0.55f + i * 0.12f) : 0f);
            }

            if (homeStartButtonShimmer != null)
            {
                float sweep = Mathf.Repeat(time * 0.42f, 1f);
                float shimmerAlpha = sweep < 0.42f
                    ? Mathf.Sin((sweep / 0.42f) * Mathf.PI) * 0.36f
                    : 0f;
                homeStartButtonShimmer.anchoredPosition = new Vector2(54f + Mathf.Min(sweep, 0.42f) / 0.42f * 276f, 0f);
                homeStartButtonShimmer.GetComponent<Image>().color = new Color(0.62f, 0.42f, 0.16f, showStartHome ? shimmerAlpha : 0f);
            }

            if (homeTitleRule != null)
            {
                float ruleAlpha = showStartHome ? 0.30f + Mathf.Sin(time * 0.58f + 0.6f) * 0.030f : 0f;
                homeTitleRule.GetComponent<Image>().color = new Color(0.58f, 0.45f, 0.21f, ruleAlpha);
            }

            if (homeGoldCrack != null)
            {
                float crackAlpha = controller != null && controller.IsRunComplete ? 0.56f : 0f;
                homeGoldCrack.GetComponent<Image>().color = new Color(0.80f, 0.60f, 0.24f, showStartHome ? crackAlpha : 0f);
            }
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
                        if (AreStartRewardsOfType(DemoRewardType.Root))
                        {
                            return "定根脚";
                        }

                        if (IsStartOpeningItemScreen())
                        {
                            return "所携";
                        }

                        if (IsStartOpeningSceneScreen())
                        {
                            return "所往";
                        }

                        return "起手总览";
                    case DemoNodeType.RouteChoice:
                        return "路口";
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
                case DemoBattlePhase.Intro:
                    return "入阵";
                case DemoBattlePhase.Running:
                    return "斗法进行";
                case DemoBattlePhase.Won:
                    return "战斗胜利";
                case DemoBattlePhase.Lost:
                    return "战斗失利";
                default:
                    return "修行进行中";
            }
        }

        private string BuildRewardSectionTitle()
        {
            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return string.Empty;
            }

            if (IsStartOpeningItemScreen())
            {
                return string.Empty;
            }

            if (IsStartOpeningSceneScreen())
            {
                return string.Empty;
            }

            if (IsRouteChoiceScreen())
            {
                return "先定下一段历练";
            }

            return "从三项补强里只拿最关键的一项";
        }

        private string BuildRewardSectionHint()
        {
            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return string.Empty;
            }

            if (IsStartOpeningItemScreen())
            {
                return string.Empty;
            }

            if (IsStartOpeningSceneScreen())
            {
                return string.Empty;
            }

            if (IsRouteChoiceScreen())
            {
                return "流程先行，先看节点顺序、风险节奏和这段路会补什么";
            }

            return "信息下沉到底部摘要，主区只负责做选择";
        }

        private void RefreshOpeningSceneGuidance()
        {
            bool showingOpeningScene = IsStartOpeningSceneScreen();
            if (openingScenePathRoot != null)
            {
                openingScenePathRoot.SetActive(false);
            }

            if (!showingOpeningScene)
            {
                return;
            }

            if (openingScenePathText != null)
            {
                openingScenePathText.text = BuildOpeningScenePathText();
            }

            if (openingSceneCauseText != null)
            {
                openingSceneCauseText.text = BuildOpeningSceneCauseText();
            }
        }

        private string BuildOpeningScenePathText()
        {
            string rootName = controller?.Run?.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "未定根脚";
            string itemName = GetCurrentOpeningItemName();
            if (string.IsNullOrEmpty(itemName))
            {
                itemName = "所携之物";
            }

            return $"{rootName}  →  {itemName}  →  所往";
        }

        private string BuildOpeningSceneCauseText()
        {
            string itemName = GetCurrentOpeningItemName();
            return string.IsNullOrEmpty(itemName)
                ? "所携之物只把去处收窄，第一脚仍由你落下。"
                : $"{itemName}只把去处收窄，第一脚仍由你落下。";
        }

        private string GetCurrentOpeningItemName()
        {
            if (controller?.Run?.OpeningSelection.JourneyLine != null)
            {
                return controller.Run.OpeningSelection.JourneyLine.CarryItemName ?? string.Empty;
            }

            DemoReward reward = controller?.CurrentRewards.FirstOrDefault(current => current.JourneyLine != null);
            return reward?.JourneyLine?.CarryItemName ?? string.Empty;
        }

        private string BuildHeaderTitleText()
        {
            if (controller.HasBattle)
            {
                return BuildBattleHeaderTitle();
            }

            return IsStartHomeScreen()
                ? "起手总览"
                : "万道归途";
        }

        private string BuildBattleHeaderTitle()
        {
            string nodeName = controller.Run.Map.CurrentNode.Name;
            if (IsOpeningBattlePage())
            {
                return $"{GetSelectedRegionName()} · 入场首战";
            }

            return controller.Battle.IsBossBattle
                ? $"{nodeName} · 终局斗法"
                : $"{nodeName} · 斗法";
        }

        private string GetSelectedRegionName()
        {
            return controller.Run.OpeningSelection.FirstRegion != null
                ? controller.Run.OpeningSelection.FirstRegion.Name
                : "所往";
        }

        private bool IsOpeningBattlePage()
        {
            if (controller == null || !controller.HasBattle)
            {
                return false;
            }

            DemoMapNode currentNode = controller.Run.Map.CurrentNode;
            return currentNode.Type == DemoNodeType.Battle
                && string.Equals(currentNode.NodeId, "node_opening_battle", System.StringComparison.OrdinalIgnoreCase);
        }

        private string GetBattlePhaseShortLabel()
        {
            switch (controller.Battle.Phase)
            {
                case DemoBattlePhase.Intro:
                    return "入阵";
                case DemoBattlePhase.Running:
                    return "斗法";
                case DemoBattlePhase.Won:
                    return "胜利";
                case DemoBattlePhase.Lost:
                    return "失利";
                default:
                    return "斗法";
            }
        }
        private string BuildBattleStateTitle()
        {
            if (IsOpeningBattlePage())
            {
                switch (controller.Battle.Phase)
                {
                    case DemoBattlePhase.Intro:
                        return "入场首战将启，先看清灵气与敌方意图。";
                    case DemoBattlePhase.Running:
                        return "飞剑持续追击，手牌随时可以介入斗法。";
                    case DemoBattlePhase.Won:
                        return "首战已破，下一步进入战后奖励。";
                    default:
                        return "入场首战正在推进。";
                }
            }

            if (controller.Battle.Phase == DemoBattlePhase.Intro)
            {
                return controller.Battle.IsBossBattle ? "镇矿剑傀正在苏醒，留意它对飞剑循环的封锁。" : "双方入阵，斗法即将开始。";
            }

            return controller.Battle.IsBossBattle
                ? "剑炉杀势压上中天，读条结束前完成应对。"
                : "灵气、飞剑与敌方意图正在持续演算。";
        }        private string BuildTopBarSummary()
        {
            if (controller.HasBattle)
            {
                string battleLine = $"{GetBattlePhaseShortLabel()}   {FormatBattleTime(controller.Battle.ElapsedSeconds)}   灵气 {controller.Battle.Energy}/{controller.Battle.MaxEnergy}   飞剑 {controller.Battle.TotalSwords}";
                if (IsOpeningBattlePage())
                {
                    return battleLine + "   首战后：奖励 -> 前路";
                }

                return controller.Battle.IsBossBattle
                    ? battleLine + $"   Boss {controller.Battle.BossPhase + 1}"
                    : battleLine;
            }

            if (IsStartHomeScreen())
            {
                return $"气血 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   灵气 {3 + controller.Run.BonusEnergy}   本命飞剑 {1 + controller.Run.BonusPermanentSwords}";
            }

            return $"生命 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组 {controller.Run.Deck.Count} 张   主修 {GetGongfaName(controller.Run.MainGongfa, "未定主修")}";
        }

        private static string FormatBattleTime(float seconds)
        {
            int totalSeconds = Mathf.Max(0, Mathf.FloorToInt(seconds));
            return $"{totalSeconds / 60:00}:{totalSeconds % 60:00}";
        }

        private string BuildHeaderContextText()
        {
            if (controller.IsRunComplete)
            {
                return controller.RunSummary != null && controller.RunSummary.Victory
                    ? "剑傀已止：回看本局如何从定根脚、承本命、入旧矿，一路走到道途成型。"
                    : "此世止步：回看路线、构筑里程碑与最后一场失利，明确下一世先补什么。";
            }

            if (controller.HasBattle)
            {
                if (IsOpeningBattlePage())
                {
                    switch (controller.Battle.Phase)
                    {
                        case DemoBattlePhase.Intro:
                            return "所往已定：短斗法即将开始，胜后先拿首战所得，再进入前路。";
                        case DemoBattlePhase.Running:
                            return "首战进行中：灵气持续增长，飞剑自动追击，手牌负责即时改变局势。";
                        case DemoBattlePhase.Won:
                            return "首战胜利：奖励先补一口结构，随后才让后续节点真正展开。";
                        case DemoBattlePhase.Lost:
                            return "首战失利：开局选择还不多，问题集中在手牌节奏与灵气使用。";
                    }
                }

                switch (controller.Battle.Phase)
                {
                    case DemoBattlePhase.Intro:
                        return "入阵：确认当前手牌、灵气增速与敌方第一次行动。";
                    case DemoBattlePhase.Running:
                        return "斗法进行：飞剑、功法与法器持续演算，在读条窗口用手牌介入。";
                    case DemoBattlePhase.Won:
                        return "战斗胜利：准备拿最能改变结构的那一项补强。";
                    case DemoBattlePhase.Lost:
                        return "战斗失利：下一局更早收束奖励，并为关键意图保留灵气。";
                    default:
                        return "云海斗法：低频出牌，持续演算。";
                }
            }

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return "定根脚：先看清这一世从哪里来。";
            }

            if (IsStartOpeningItemScreen())
            {
                return "所携：先听为何出山，再看临行一物落到谁手里。";
            }

            if (IsStartOpeningSceneScreen())
            {
                return "所往：所携已定，卷中显境，第一脚该落到一个真正的地方。";
            }

            if (IsRouteChoiceScreen())
            {
                return "路口已开：先挑下一段历练，把补强时点和风险轻重握在手里。";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "起手总览：先照见道基与传承，再决定这一世如何起势。";
                case DemoNodeType.Reward:
                    return "战后补强：只拿能改变结构的那一项，别把路线拿散。";
                case DemoNodeType.Training:
                    return "修炼节点：补齐功法与法器之间的关键缺口。";
                case DemoNodeType.Shop:
                    return "Boss 前整备：优先看续航、灵气和收束点。";
                default:
                    return "道途推进：让飞剑、功法与法器往同一路靠拢。";
            }
        }

        private string BuildNodeOverlaySummary()
        {
            if (controller.HasRunResult)
            {
                return controller.GetRunResultSummary();
            }

            return controller.GetBattleSummary() + "\n\n" + BuildContextSummary();
        }

        private string BuildHandTitleText()
        {
            if (!controller.HasBattle)
            {
                return "手牌";
            }

            switch (controller.Battle.Phase)
            {
                case DemoBattlePhase.Intro:
                    return "手牌 · 待势";
                case DemoBattlePhase.Running:
                    return "手牌 · 即时介入";
                case DemoBattlePhase.Won:
                    return "战斗已胜";
                case DemoBattlePhase.Lost:
                    return "战斗失利";
                default:
                    return "手牌";
            }
        }
        private string BuildHandInfoText()
        {
            if (!controller.HasBattle)
            {
                return string.Empty;
            }

            if (controller.Battle.Phase == DemoBattlePhase.Won)
            {
                return "敌势已破 · 准备领取补强";
            }

            if (controller.Battle.Phase == DemoBattlePhase.Lost)
            {
                return "灵台失守 · 本场结束";
            }

            return $"灵 {controller.Battle.Energy}/{controller.Battle.MaxEnergy} +{controller.Battle.EnergyRegenerationPerSecond:0.#}/秒 · " +
                $"手 {controller.Battle.Hand.Count} 牌 {controller.Battle.DrawPile.Count} 弃 {controller.Battle.DiscardPile.Count} · " +
                $"抽 {Mathf.Max(0f, controller.Battle.DrawTimer):0.0}s";
        }
        private string BuildBattleFlowText()
        {
            if (!controller.HasBattle)
            {
                return string.Empty;
            }

            if (IsOpeningBattlePage())
            {
                return "旧矿地窟 · 入场试锋";
            }

            return "前路  >  节点历练  >  战后补强  >  继续前行";
        }
        private string BuildHandHint()
        {
            if (!controller.HasBattle)
            {
                return string.Empty;
            }

            if (controller.BattleSpeed <= 0.01f &&
                (controller.Battle.Phase == DemoBattlePhase.Intro || controller.Battle.Phase == DemoBattlePhase.Running))
            {
                return "斗法已暂停";
            }

            switch (controller.Battle.Phase)
            {
                case DemoBattlePhase.Intro:
                    return "斗法将启";
                case DemoBattlePhase.Running:
                    return controller.Battle.Hand.Count == 0
                        ? $"等待抽牌 · {Mathf.Max(0f, controller.Battle.DrawTimer):0.0}s"
                        : "点牌即出";
                case DemoBattlePhase.Won:
                    return "领取补强";
                case DemoBattlePhase.Lost:
                    return "本场结束";
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

            if (IsOpeningBattlePage())
            {
                switch (controller.Battle.Phase)
                {
                    case DemoBattlePhase.Intro:
                        return "灵气会持续增长；飞剑与敌人都会自动行动，手牌可以随时改变局势。";
                    case DemoBattlePhase.Running:
                        return $"敌方意图：{controller.Battle.EnemyIntentText} · {Mathf.Max(0f, controller.Battle.EnemyIntentRemaining):0.0}s";
                    case DemoBattlePhase.Won:
                        return "下一步拿一项首战补强，再进入前路，后续节点才开始分叉。";
                    default:
                        return "所往已经选定，先打这一场，再把肉鸽路线打开。";
                }
            }

            string intent = controller.Battle.IsBossBattle
                ? controller.Battle.BossIntentText
                : controller.Battle.EnemyIntentText;
            return $"{(controller.Battle.IsBossBattle ? "剑炉预警" : "敌方意图")}：{intent} · {Mathf.Max(0f, controller.Battle.EnemyIntentRemaining):0.0}s";
        }
        private string BuildCompactNodeMapSummary()
        {
            DemoMapRun map = controller.Run.Map;
            DemoMapNode current = map.CurrentNode;
            DemoMapNode next = map.Nodes.Skip(map.CurrentIndex + 1).FirstOrDefault();
            string nextText = next != null ? $"下一处  {GetRouteNodeShortLabel(next)} · {next.Name}" : "下一处  此卷将尽";
            return
                $"第 {Mathf.Max(1, current.Layer)} 层  ·  第 {map.CurrentIndex + 1}/{map.Nodes.Count} 步\n" +
                $"当前  {GetRouteNodeShortLabel(current)} · {current.Name}\n" +
                nextText;
        }

        private string BuildCompactNodeBuildSummary()
        {
            string focus = GetBuildApproachLabel(controller.Run.GetBuildStyle());
            string mainGongfa = GetGongfaName(controller.Run.MainGongfa, "未定");
            string artifact = controller.Run.Artifacts.Count > 0
                ? DemoArtifactLibrary.Get(controller.Run.Artifacts[0]).Name
                : "尚未得器";
            return
                $"气血  {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}  ·  牌组  {controller.Run.Deck.Count} 张\n" +
                $"起势  {focus}  ·  主修  {mainGongfa}\n" +
                $"法器  {artifact}";
        }

        private string BuildResultNodeMapSummary()
        {
            DemoRunSummary summary = controller.RunSummary;
            return
                $"抵达  第 {summary.ReachedLayer} 层  ·  胜战  {summary.BattlesWon} 场\n" +
                $"最大飞剑  {summary.MaxSwordCount}  ·  最高爆发  {summary.HighestBurstDamage}\n" +
                $"Boss  {(summary.DefeatedBoss ? "镇矿剑傀已破" : "未能击破")}";
        }

        private string BuildResultNodeBuildSummary()
        {
            DemoRunSummary summary = controller.RunSummary;
            IEnumerable<string> componentNames = summary.CoreComponents
                .Where(id => id != "sword_slash" && id != "guard_step" && id != "cloud_step")
                .Select(GetBuildComponentDisplayName);
            string components = BuildTruncatedList(componentNames, 4, "基础剑势");
            return
                $"主修  {summary.MainGongfaName}\n" +
                $"核心法器  {summary.CoreArtifactName}\n" +
                $"核心组件  {components}";
        }

        private static string GetBuildComponentDisplayName(string componentId)
        {
            if (DemoConfigRepository.TryCreateCard(componentId, out DemoCard card))
            {
                return card.Name;
            }

            switch (componentId)
            {
                case "gongfa_sword_control_art":
                    return "御剑诀";
                case "gongfa_wanjian_return":
                    return "万剑归宗";
                case "artifact_sword_box":
                    return "剑匣";
                default:
                    return componentId;
            }
        }

        private string BuildRunSummary()
        {
            string artifactText = BuildTruncatedList(controller.Run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name), 3, "暂无");
            string rootName = controller.Run.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "未定";
            string journeyName = controller.Run.OpeningSelection.JourneyLine != null ? controller.Run.OpeningSelection.JourneyLine.CarryItemName : "未定";
            string regionName = controller.Run.OpeningSelection.FirstRegion != null ? controller.Run.OpeningSelection.FirstRegion.Name : "未定";

            return
                $"根脚：{rootName}\n" +
                $"所携：{journeyName}\n" +
                $"所往：{regionName}\n" +
                $"主修：{FormatGongfaSummary(controller.Run.MainGongfa, "未定")}\n" +
                $"辅修：{FormatGongfaSummary(controller.Run.SupportGongfa, "未定")}\n" +
                $"神通：{FormatGongfaSummary(controller.Run.DivineGongfa, "未悟")}\n" +
                $"法器：{artifactText}\n" +
                $"生命：{controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组：{controller.Run.Deck.Count} 张";
        }

        private string BuildRunSnapshot()
        {
            string artifactText = BuildTruncatedList(controller.Run.Artifacts.Select(type => DemoArtifactLibrary.Get(type).Name), 2, "暂无");
            string rootName = controller.Run.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "未定";
            string journeyName = controller.Run.OpeningSelection.JourneyLine != null ? controller.Run.OpeningSelection.JourneyLine.CarryItemName : "未定";
            string regionName = controller.Run.OpeningSelection.FirstRegion != null ? controller.Run.OpeningSelection.FirstRegion.Name : "未定";

            return
                $"根脚 {rootName}   所携 {journeyName}   所往 {regionName}\n" +
                $"主修 {GetGongfaName(controller.Run.MainGongfa, "未定")}   辅修 {GetGongfaName(controller.Run.SupportGongfa, "未定")}   神通 {GetGongfaName(controller.Run.DivineGongfa, "未悟")}\n" +
                $"生命 {controller.Run.CurrentHealth}/{controller.Run.MaxHealth}   牌组 {controller.Run.Deck.Count} 张\n" +
                $"核心法器 {artifactText}";
        }

        private string BuildDeckFocusSummary()
        {
            string focus = GetBuildApproachLabel(controller.Run.GetBuildStyle());
            string topCards = BuildTruncatedList(controller.Run.Deck.Select(card => card.Name), 3, "暂无核心牌");
            string openingSummary = BuildOpeningSummaryLine();
            string firstRegion = controller.Run.OpeningSelection.FirstRegion != null ? controller.Run.OpeningSelection.FirstRegion.Name : "未定所往";

            return
                $"{openingSummary}\n" +
                $"当前起势：{focus}\n" +
                $"所往落点：{firstRegion}\n" +
                $"起手核心：{topCards}";
        }

        private string BuildRewardStageLabel()
        {
            DemoMapNode node = controller?.Run?.Map?.CurrentNode;
            if (node == null)
            {
                return "历练所得";
            }

            if (ContainsAny(node.Name, "首战", "入口"))
            {
                return "首战所得";
            }

            switch (node.Type)
            {
                case DemoNodeType.Training:
                    return "修炼所得";
                case DemoNodeType.Shop:
                    return "整备所得";
                case DemoNodeType.Reward:
                    return "历练所得";
                default:
                    return "道途所得";
            }
        }

        private string BuildRewardLeadText()
        {
            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return string.Empty;
            }

            if (IsStartOpeningItemScreen())
            {
                return "从案上诸件临行旧物中择一件，作为这一世的所携之物。";
            }

            if (IsStartOpeningSceneScreen())
            {
                return string.Empty;
            }

            if (IsRouteChoiceScreen())
            {
                return "先看图，再看节点顺序。这里先定下一段历练，把字压到最少。";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "起点会先给你身份，再给你所携之物与所往，最后才把主脉交给战斗与奖励去显化。";
                case DemoNodeType.Training:
                    return "补齐功法和法器之间的缺口，让构筑开始收束。";
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
            if (controller.IsRunComplete)
            {
                return
                    "道劫　旧契已断\n" +
                    "证道　可再证新道\n" +
                    "回看　道途已收束\n" +
                    "起势　根脚再定\n" +
                    "所携　重新上路";
            }

            string mainGongfa = GetGongfaName(controller.Run.MainGongfa, "未定");
            return
                "道基　本世道基\n" +
                $"气血　{controller.Run.CurrentHealth}/{controller.Run.MaxHealth}\n" +
                $"灵气　{3 + controller.Run.BonusEnergy}\n" +
                $"命剑　{1 + controller.Run.BonusPermanentSwords}\n" +
                $"起手　{controller.Run.Deck.Count} 张\n" +
                $"主修　{mainGongfa}";
        }

        private string BuildStartInheritanceScrollText()
        {
            string rootName = controller.Run.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "未定根脚";
            string journeyName = controller.Run.OpeningSelection.JourneyLine != null ? controller.Run.OpeningSelection.JourneyLine.CarryItemName : "未定所携";
            string regionName = controller.Run.OpeningSelection.FirstRegion != null ? controller.Run.OpeningSelection.FirstRegion.Name : "未定所往";
            return $"{rootName}  →  {journeyName}  →  {regionName}";
        }

        private string BuildStartStageTitle()
        {
            if (AreStartRewardsOfType(DemoRewardType.Trace))
            {
                return "整备道痕";
            }

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return "定根脚";
            }

            if (IsStartOpeningItemScreen())
            {
                return "所携";
            }

            if (IsStartOpeningSceneScreen())
            {
                return "所往";
            }

            return "开局";
        }

        private string BuildStartStageBody()
        {
            if (AreStartRewardsOfType(DemoRewardType.Trace))
            {
                return "传承只改变下一次选择，不把上一世战力带进来。";
            }

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return "命书翻开，签落案前，先认此身来处。";
            }

            if (IsStartOpeningItemScreen())
            {
                return "案上诸物，各有一段出山缘由。先定所携，再看所往。";
            }

            if (IsStartOpeningSceneScreen())
            {
                return "所携已定，先让第一段路落到一个地方。";
            }

            return "开局信息确认完毕，准备踏入这一局。";
        }

        private string BuildStartStageChecklist()
        {
            if (AreStartRewardsOfType(DemoRewardType.Trace))
            {
                return "· 看上一世所得\n· 只取一次重铸\n· 不继承战力";
            }

            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return "· 认此身来处\n· 看族谱旧痕\n· 落印定身";
            }

            if (IsStartOpeningItemScreen())
            {
                return "· 读一段来历\n· 承一件旧物\n· 定一口起势";
            }

            if (IsStartOpeningSceneScreen())
            {
                return "· 看先去哪里\n· 看险势轻重\n· 落下第一脚";
            }

            return "· 准备开局\n· 踏入地图\n· 进入第一场历练";
        }

        private string BuildStartContextSummary()
        {
            if (AreStartRewardsOfType(DemoRewardType.Root))
            {
                return "先在命案前落下此身，再等临行之物递到案上。";
            }

            if (IsStartOpeningItemScreen())
            {
                return "所携：案上诸物各牵一段旧缘，先把为何离山与带什么上路定清楚。";
            }

            if (IsStartOpeningSceneScreen())
            {
                return "所往：所携之物只把去处收窄，第一脚仍由你落下。";
            }

            return "所往已经定下，先用入场首战兑现去处，再用前路把肉鸽路线打开。";
        }

        private bool AreStartRewardsOfType(DemoRewardType type)
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.Start
                && controller.CurrentRewards.Count > 0
                && controller.CurrentRewards.All(reward => reward.Type == type);
        }

        private static bool IsRewardClaimable(DemoReward reward)
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

        private bool IsStartOpeningScreen()
        {
            return controller != null
                && !controller.HasBattle
                && controller.Run.Map.CurrentNode.Type == DemoNodeType.Start
                && controller.CurrentRewards.Count > 0
                && controller.CurrentRewards.All(
                    reward =>
                        reward.Type == DemoRewardType.Root
                        || reward.Type == DemoRewardType.Trace
                        || (reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel)
                        || reward.Type == DemoRewardType.OpeningScene);
        }

        private bool IsStartOpeningItemScreen()
        {
            return AreStartRewardsOfType(DemoRewardType.Journey) || AreStartRewardsOfType(DemoRewardType.Vessel);
        }

        private bool IsStartOpeningSceneScreen()
        {
            return AreStartRewardsOfType(DemoRewardType.OpeningScene);
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
            if (controller.HasRunResult)
            {
                return controller.RunSummary.Victory ? "一世结算" : "败局结算";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return BuildStartStageTitle();
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
            if (controller.HasRunResult)
            {
                return controller.GetRunResultSummary();
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return BuildStartStageBody();
                case DemoNodeType.Training:
                    return "修炼节点最适合补齐功法、法器和终结技之间的缺口，让自动演武从能打变成会滚。";
                case DemoNodeType.Shop:
                    return "进入 Boss 前的最后一次取舍，优先补灵气续航、关键收束点，别把资源撒成平均数。";
                case DemoNodeType.Victory:
                    return "这一局已经从定根脚、所携与所往一路走到收束成型，可以回看是哪一轮补强把飞剑、功法和法器真正串了起来。";
                default:
                    return BuildContextSummary();
            }
        }

        private string BuildNodeStageChecklist()
        {
            if (controller.HasRunResult)
            {
                return controller.RunSummary.Victory
                    ? "· 回看核心组件\n· 记录最大飞剑规模\n· 携新道痕再启一世"
                    : "· 回看止步层数\n· 识别缺失组件\n· 不继承战力再启一世";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return BuildStartStageChecklist();
                case DemoNodeType.Training:
                    return "· 优先补关键功法\n· 让法器接上主脉\n· 给终结技留出爆发窗";
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
            if (controller.HasRunResult)
            {
                return controller.GetRunResultSummary();
            }

            if (controller.HasBattle && controller.Battle.IsBossBattle)
            {
                return $"Boss 预警：{controller.Battle.BossIntentText}";
            }

            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return BuildStartContextSummary();
                case DemoNodeType.RouteChoice:
                    return "先想清楚你更缺缓冲节点、补强时点，还是更短的一段冲劫路径。";
                case DemoNodeType.Reward:
                    return "优先拿能改变演武结构的补强，而不是只加一点面板数值。";
                case DemoNodeType.Training:
                    return "修炼节点最适合补齐功法、法器和终结技之间的缺口。";
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

        private string BuildOpeningSummaryLine()
        {
            string rootName = controller.Run.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "未定根脚";
            string journeyName = controller.Run.OpeningSelection.JourneyLine != null ? controller.Run.OpeningSelection.JourneyLine.CarryItemName : "未定所携";
            string regionName = controller.Run.OpeningSelection.FirstRegion != null ? controller.Run.OpeningSelection.FirstRegion.Name : "未定所往";
            return $"开局脉络：{rootName} · {journeyName} · {regionName}";
        }

        private string BuildArtifactPreview()
        {
            List<string> lines = new List<string>();
            lines.Add(BuildOpeningSummaryLine());
            lines.Add(string.Empty);

            lines.Add("功法");
            lines.Add($"主修：{FormatGongfaSummary(controller.Run.MainGongfa, "未定主修")}");
            lines.Add($"辅修：{FormatGongfaSummary(controller.Run.SupportGongfa, "未定辅修")}");
            lines.Add($"神通：{FormatGongfaSummary(controller.Run.DivineGongfa, "未悟神通")}");
            lines.Add(string.Empty);
            lines.Add("法器");

            if (controller.Run.Artifacts.Count == 0)
            {
                lines.Add("尚未获得法器。");
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
                return "作为半实时斗法的节奏补件。";
            }

            int count = Mathf.Min(3, parts.Count);
            return string.Join("\n", parts.Take(count));
        }

        private GameObject CreateCardEntry(Transform parent, DemoCard card, bool interactable, float cardWidth)
        {
            GameObject cardObject = new GameObject(
                "Card_" + card.Id,
                typeof(RectTransform),
                typeof(Image),
                typeof(Button),
                typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);

            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(cardWidth, 166f);

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = cardWidth;
            layout.minWidth = cardWidth;
            layout.preferredHeight = 166f;
            layout.minHeight = 166f;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            Image image = cardObject.GetComponent<Image>();
            image.color = new Color(0.075f, 0.095f, 0.11f, 0.96f);
            Sprite battleCardFrame = LoadBattleCardFrameSprite();
            if (battleCardFrame != null)
            {
                ApplySpriteToImage(cardRect, battleCardFrame, interactable ? Color.white : new Color(1f, 1f, 1f, 0.52f));
            }
            else
            {
                DecorateFrame(cardRect, GetCardAccentColor(card.Style), new Color(0.22f, 0.17f, 0.11f, 0.10f), 1.5f, true);
            }

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.interactable = interactable;
            button.colors = battleCardFrame != null
                ? CreateSpriteButtonColors(new Color(0.36f, 0.32f, 0.26f, 0.52f))
                : CreateButtonColors(interactable ? new Color(0.18f, 0.16f, 0.13f, 0.98f) : new Color(0.12f, 0.12f, 0.12f, 0.78f));

            RectTransform costSeal = CreateFixedPanel(
                cardRect,
                "CostSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(21f, -20f),
                new Vector2(34f, 34f),
                new Color(0.16f, 0.13f, 0.09f, 0.94f));
            DecorateFrame(costSeal, ColorGold, new Color(0.28f, 0.22f, 0.12f, 0.08f), 1f, true);
            Text costText = CreateText(costSeal, "Cost", card.Cost.ToString(), 15, FontStyle.Bold, TextAnchor.MiddleCenter, ColorGold);
            StretchText(costText.rectTransform, new Vector2(4f, 4f), new Vector2(-4f, -4f));

            Text styleText = CreateText(cardRect, "Style", GetStyleLabel(card.Style), 10, FontStyle.Bold, TextAnchor.UpperRight, GetCardAccentColor(card.Style));
            styleText.rectTransform.anchorMin = new Vector2(0.60f, 0.78f);
            styleText.rectTransform.anchorMax = new Vector2(0.92f, 0.96f);
            styleText.rectTransform.offsetMin = Vector2.zero;
            styleText.rectTransform.offsetMax = Vector2.zero;

            Text nameText = CreateText(cardRect, "Name", card.Name, 15, FontStyle.Bold, TextAnchor.UpperLeft, ColorPaper);
            nameText.rectTransform.anchorMin = new Vector2(0.30f, 0.70f);
            nameText.rectTransform.anchorMax = new Vector2(0.92f, 0.92f);
            nameText.rectTransform.offsetMin = Vector2.zero;
            nameText.rectTransform.offsetMax = Vector2.zero;
            nameText.verticalOverflow = VerticalWrapMode.Truncate;

            RectTransform artBand = CreateStretchPanel(
                cardRect,
                "ArtBand",
                new Vector2(0.08f, 0.32f),
                new Vector2(0.92f, 0.65f),
                Vector2.zero,
                Vector2.zero,
                new Color(GetCardColor(card.Style).r, GetCardColor(card.Style).g, GetCardColor(card.Style).b, 0.38f));
            Text iconText = CreateText(artBand, "Icon", GetCardIconGlyph(card), 22, FontStyle.Bold, TextAnchor.MiddleLeft, GetQualityColor(card.Quality));
            StretchText(iconText.rectTransform, new Vector2(10f, 2f), new Vector2(-62f, -2f));
            Text moodText = CreateText(artBand, "Mood", GetCardMoodText(card), 10, FontStyle.Bold, TextAnchor.MiddleRight, GetCardAccentColor(card.Style));
            StretchText(moodText.rectTransform, new Vector2(42f, 2f), new Vector2(-8f, -2f));

            Text rulesText = CreateText(cardRect, "Rules", BuildCardPreviewText(card), 10, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.80f, 0.82f, 0.82f, 0.96f));
            rulesText.rectTransform.anchorMin = new Vector2(0.08f, 0.045f);
            rulesText.rectTransform.anchorMax = new Vector2(0.92f, 0.29f);
            rulesText.rectTransform.offsetMin = Vector2.zero;
            rulesText.rectTransform.offsetMax = Vector2.zero;
            rulesText.verticalOverflow = VerticalWrapMode.Truncate;

            if (!interactable)
            {
                Text lockedText = CreateText(cardRect, "Locked", "气不足", 9, FontStyle.Bold, TextAnchor.LowerRight, new Color(0.70f, 0.66f, 0.58f, 0.92f));
                lockedText.rectTransform.anchorMin = new Vector2(0.55f, 0f);
                lockedText.rectTransform.anchorMax = new Vector2(0.92f, 0.18f);
                lockedText.rectTransform.offsetMin = Vector2.zero;
                lockedText.rectTransform.offsetMax = Vector2.zero;
            }

            return cardObject;
        }
        private GameObject CreateRewardEntry(Transform parent, DemoReward reward)
        {
            if (IsStartOpeningScreen()
                && (reward.Type == DemoRewardType.Root
                    || (reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel)
                    || reward.Type == DemoRewardType.OpeningScene
                    || reward.Type == DemoRewardType.Trace))
            {
                return CreateOpeningChoiceEntry(parent, reward);
            }

            if (IsRouteChoiceScreen()
                && reward.Type == DemoRewardType.Route)
            {
                return CreateRouteChoiceEntry(parent, reward);
            }

            Color accent = GetRewardAccentColor(reward);
            Color paperInk = new Color(0.18f, 0.16f, 0.12f, 0.98f);
            Color mutedInk = new Color(0.34f, 0.32f, 0.26f, 0.90f);

            GameObject rewardObject = new GameObject("Reward_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            rewardObject.transform.SetParent(parent, false);
            RectTransform rewardRect = rewardObject.GetComponent<RectTransform>();
            rewardRect.sizeDelta = new Vector2(384f, 506f);

            LayoutElement layout = rewardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 384f;
            layout.minWidth = 384f;
            layout.preferredHeight = 506f;
            layout.minHeight = 506f;

            Image image = rewardObject.GetComponent<Image>();
            image.color = new Color(0.95f, 0.91f, 0.81f, 0.98f);

            Button button = rewardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(new Color(0.95f, 0.91f, 0.81f, 1f));
            DecorateFrame(rewardRect, new Color(accent.r * 0.74f, accent.g * 0.74f, accent.b * 0.74f, 0.86f), new Color(0.99f, 0.97f, 0.91f, 0.14f), 1.5f, false);

            VerticalLayoutGroup layoutGroup = rewardObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(16, 16, 16, 16);
            layoutGroup.spacing = 10f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            RectTransform trophyStage = CreateFixedHeightBlock(rewardObject.transform, "RewardTrophyStage", 188f, new Color(0.88f, 0.82f, 0.68f, 0.88f));
            DecorateFrame(trophyStage, new Color(accent.r, accent.g, accent.b, 0.52f), new Color(1f, 1f, 1f, 0.06f), 1f, false);
            RectTransform trophyLift = CreateStretchPanel(
                trophyStage,
                "RewardTrophyLift",
                Vector2.zero,
                Vector2.one,
                new Vector2(6f, 6f),
                new Vector2(-6f, -6f),
                new Color(0.99f, 0.96f, 0.87f, 0.20f));
            trophyLift.GetComponent<Image>().raycastTarget = false;

            Text typeEyebrow = CreateText(trophyStage, "RewardTypeEyebrow", GetRewardTypeLabel(reward) + " · " + GetRewardCommercialTag(reward), 12, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.40f, 0.29f, 0.15f, 0.86f));
            typeEyebrow.rectTransform.anchorMin = new Vector2(0f, 1f);
            typeEyebrow.rectTransform.anchorMax = new Vector2(1f, 1f);
            typeEyebrow.rectTransform.offsetMin = new Vector2(18f, -28f);
            typeEyebrow.rectTransform.offsetMax = new Vector2(-18f, -8f);

            RectTransform iconSeal = CreateFixedPanel(
                trophyStage,
                "RewardIconSeal",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 6f),
                new Vector2(112f, 96f),
                new Color(0f, 0f, 0f, 0f));
            iconSeal.GetComponent<Image>().raycastTarget = false;

            Text badgeIconText = CreateText(iconSeal, "BadgeIconText", GetRewardGlyph(reward), 46, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(accent.r * 0.66f, accent.g * 0.66f, accent.b * 0.66f, 0.98f));
            StretchText(badgeIconText.rectTransform, new Vector2(8f, 8f), new Vector2(-8f, -8f));
            RectTransform leftInkRule = CreateFixedPanel(
                trophyStage,
                "RewardLeftInkRule",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-96f, 6f),
                new Vector2(72f, 1f),
                new Color(accent.r, accent.g, accent.b, 0.52f));
            leftInkRule.GetComponent<Image>().raycastTarget = false;
            RectTransform rightInkRule = CreateFixedPanel(
                trophyStage,
                "RewardRightInkRule",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(96f, 6f),
                new Vector2(72f, 1f),
                new Color(accent.r, accent.g, accent.b, 0.52f));
            rightInkRule.GetComponent<Image>().raycastTarget = false;

            Text stageHint = CreateText(trophyStage, "RewardStageHint", BuildRewardStageLabel(), 13, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.53f, 0.17f, 0.11f, 0.86f));
            stageHint.rectTransform.anchorMin = Vector2.zero;
            stageHint.rectTransform.anchorMax = new Vector2(1f, 0f);
            stageHint.rectTransform.offsetMin = new Vector2(16f, 14f);
            stageHint.rectTransform.offsetMax = new Vector2(-16f, 34f);

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
            bodyMetaLayout.preferredHeight = 26f;
            HorizontalLayoutGroup bodyMetaGroup = bodyMeta.GetComponent<HorizontalLayoutGroup>();
            bodyMetaGroup.spacing = 8f;
            bodyMetaGroup.childAlignment = TextAnchor.MiddleLeft;
            bodyMetaGroup.childControlHeight = true;
            bodyMetaGroup.childControlWidth = false;
            bodyMetaGroup.childForceExpandHeight = false;
            bodyMetaGroup.childForceExpandWidth = false;

            CreateLabelChip(bodyMeta.transform, GetQualityLongLabel(GetRewardQuality(reward)), GetQualityColor(GetRewardQuality(reward)), new Color(0.98f, 0.94f, 0.84f, 1f), 64f);
            CreateLabelChip(bodyMeta.transform, GetRewardFlavorText(reward), new Color(0.86f, 0.78f, 0.62f, 0.92f), new Color(0.28f, 0.22f, 0.14f, 0.94f), 108f);

            Text rewardNameText = CreateText(bodyObject.transform, "RewardName", reward.Name, 28, FontStyle.Bold, TextAnchor.UpperLeft, paperInk);
            LayoutElement rewardNameLayout = rewardNameText.gameObject.AddComponent<LayoutElement>();
            rewardNameLayout.preferredHeight = 38f;

            RectTransform nameRule = CreateFixedHeightBlock(bodyObject.transform, "RewardNameRule", 2f, new Color(accent.r, accent.g, accent.b, 0.54f));
            nameRule.GetComponent<Image>().raycastTarget = false;

            Text rewardDescriptionText = CreateText(bodyObject.transform, "RewardDescription", BuildRewardPreviewText(reward), 16, FontStyle.Normal, TextAnchor.UpperLeft, mutedInk);
            rewardDescriptionText.lineSpacing = 1.08f;
            rewardDescriptionText.verticalOverflow = VerticalWrapMode.Truncate;
            LayoutElement rewardDescriptionLayout = rewardDescriptionText.gameObject.AddComponent<LayoutElement>();
            rewardDescriptionLayout.flexibleHeight = 1f;

            RectTransform footer = CreateFixedHeightBlock(rewardObject.transform, "RewardFooter", 58f, new Color(0.90f, 0.84f, 0.70f, 0.94f));
            DecorateFrame(footer, new Color(0.48f, 0.34f, 0.17f, 0.26f), new Color(1f, 1f, 1f, 0.04f), 1f, false);
            RectTransform footerSeal = CreateFixedPanel(
                footer,
                "RewardFooterSeal",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(18f, 0f),
                new Vector2(6f, 30f),
                new Color(0.56f, 0.16f, 0.10f, 0.88f));
            footerSeal.GetComponent<Image>().raycastTarget = false;
            Text footerText = CreateText(footer, "RewardFooterText", "收下此物", 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.24f, 0.18f, 0.11f, 0.98f));
            StretchText(footerText.rectTransform, new Vector2(20f, 5f), new Vector2(-20f, -15f));

            Text footerHintText = CreateText(footer, "RewardFooterHint", "悬停查看完整机制", 11, FontStyle.Normal, TextAnchor.LowerCenter, new Color(0.48f, 0.37f, 0.23f, 0.78f));
            footerHintText.rectTransform.anchorMin = Vector2.zero;
            footerHintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            footerHintText.rectTransform.offsetMin = new Vector2(8f, 7f);
            footerHintText.rectTransform.offsetMax = new Vector2(-8f, 23f);

            AddRewardHoverEvents(rewardObject, reward);
            return rewardObject;
        }

        private GameObject CreateRootDestinyDeskEntry(Transform parent, IReadOnlyList<DemoReward> rewards)
        {
            GameObject deskObject = new GameObject("RootDestinyDesk", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
            deskObject.transform.SetParent(parent, false);

            RectTransform deskRect = deskObject.GetComponent<RectTransform>();
            deskRect.sizeDelta = new Vector2(1920f, 1080f);

            LayoutElement layout = deskObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 1920f;
            layout.minWidth = 1920f;
            layout.preferredHeight = 1080f;
            layout.minHeight = 1080f;

            Image deskImage = deskObject.GetComponent<Image>();
            deskImage.color = new Color(1f, 1f, 1f, 0f);
            deskImage.raycastTarget = false;

            RectTransform deskVeil = CreateStretchPanel(deskObject.transform, "RootDeskSoftVeil", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero, new Color(0.98f, 0.93f, 0.82f, 0.010f));
            deskVeil.GetComponent<Image>().raycastTarget = false;

            RectTransform ledgerPanel = CreateFixedPanel(
                deskObject.transform,
                "RootLedgerPanel",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-100f, -296f),
                new Vector2(790f, 356f),
                new Color(1f, 0.95f, 0.78f, 0f));
            ledgerPanel.GetComponent<Image>().raycastTarget = false;

            Text deskLabel = CreateText(ledgerPanel, "RootDeskLabel", "签落案前，先认此身来处。", 17, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.46f, 0.34f, 0.18f, 0.88f));
            deskLabel.rectTransform.anchorMin = new Vector2(0f, 1f);
            deskLabel.rectTransform.anchorMax = new Vector2(1f, 1f);
            deskLabel.rectTransform.offsetMin = new Vector2(42f, -40f);
            deskLabel.rectTransform.offsetMax = new Vector2(-42f, -12f);

            Text rootNameText = CreateText(ledgerPanel, "RootName", string.Empty, 52, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.14f, 0.10f, 0.07f, 0.98f));
            rootNameText.rectTransform.anchorMin = new Vector2(0f, 1f);
            rootNameText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rootNameText.rectTransform.offsetMin = new Vector2(42f, -104f);
            rootNameText.rectTransform.offsetMax = new Vector2(-278f, -44f);

            Text rarityText = CreateText(ledgerPanel, "RootRarity", string.Empty, 15, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.58f, 0.42f, 0.20f, 0.90f));
            rarityText.rectTransform.anchorMin = new Vector2(0f, 1f);
            rarityText.rectTransform.anchorMax = new Vector2(1f, 1f);
            rarityText.rectTransform.offsetMin = new Vector2(46f, -136f);
            rarityText.rectTransform.offsetMax = new Vector2(-292f, -110f);

            Text identityText = CreateText(ledgerPanel, "RootIdentity", string.Empty, 19, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.21f, 0.18f, 0.13f, 0.96f));
            identityText.rectTransform.anchorMin = new Vector2(0f, 1f);
            identityText.rectTransform.anchorMax = new Vector2(1f, 1f);
            identityText.rectTransform.offsetMin = new Vector2(46f, -210f);
            identityText.rectTransform.offsetMax = new Vector2(-264f, -148f);

            RectTransform effectPanel = CreateFixedPanel(
                ledgerPanel,
                "RootEffectPanel",
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                new Vector2(46f, 88f),
                new Vector2(368f, 58f),
                new Color(0.96f, 0.84f, 0.56f, 0.075f));
            effectPanel.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(effectPanel, new Color(0.68f, 0.50f, 0.24f, 0.15f), new Color(1f, 0.84f, 0.48f, 0.010f), 1.0f, false);
            Text effectText = CreateText(effectPanel, "RootEffectText", string.Empty, 16, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.22f, 0.15f, 0.07f, 0.94f));
            StretchText(effectText.rectTransform, new Vector2(16f, 8f), new Vector2(-16f, -8f));

            RectTransform pageSmokeRect = CreateFixedPanel(
                ledgerPanel,
                "RootPageSmoke",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(54f, -8f),
                new Vector2(560f, 300f),
                new Color(1f, 1f, 1f, 0f));
            ApplySpriteToImage(pageSmokeRect, LoadRootSmokeWispSprite(), new Color(0.90f, 0.92f, 0.90f, 0f), true);
            Image pageSmokeImage = pageSmokeRect.GetComponent<Image>();
            pageSmokeImage.raycastTarget = false;

            RectTransform inkSweepRect = CreateFixedPanel(
                ledgerPanel,
                "RootInkSweep",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-310f, -4f),
                new Vector2(220f, 3f),
                new Color(0.86f, 0.62f, 0.22f, 0f));
            inkSweepRect.GetComponent<Image>().raycastTarget = false;

            RectTransform confirmRect = CreateFixedPanel(
                ledgerPanel,
                "RootConfirmSealHitArea",
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-142f, 76f),
                new Vector2(204f, 128f),
                new Color(1f, 1f, 1f, 0f));
            confirmRect.GetComponent<Image>().raycastTarget = true;
            Button confirmButton = confirmRect.gameObject.AddComponent<Button>();
            confirmButton.targetGraphic = confirmRect.GetComponent<Image>();
            confirmButton.transition = Selectable.Transition.None;

            RectTransform confirmSealRect = CreateFixedPanel(
                confirmRect,
                "RootConfirmSealImage",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 0f),
                new Vector2(188f, 118f),
                Color.white);
            ApplySpriteToImage(confirmSealRect, LoadRootConfirmSealSprite(), Color.white, true);
            confirmSealRect.localRotation = Quaternion.Euler(0f, 0f, -4.5f);
            Image confirmSealImage = confirmSealRect.GetComponent<Image>();
            confirmSealImage.raycastTarget = false;

            Text confirmText = CreateText(confirmRect, "RootConfirmText", "落印定身", 20, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1.00f, 0.88f, 0.74f, 0.98f));
            StretchText(confirmText.rectTransform, new Vector2(14f, 29f), new Vector2(-14f, -24f));
            confirmText.raycastTarget = false;

            Text footerText = CreateText(ledgerPanel, "RootFooter", "印成之后，自有人递来临行之物。", 14, FontStyle.Normal, TextAnchor.LowerLeft, new Color(0.38f, 0.29f, 0.16f, 0.84f));
            footerText.rectTransform.anchorMin = new Vector2(0f, 0f);
            footerText.rectTransform.anchorMax = new Vector2(1f, 0f);
            footerText.rectTransform.offsetMin = new Vector2(40f, 20f);
            footerText.rectTransform.offsetMax = new Vector2(-220f, 48f);

            RectTransform lotRack = CreateFixedPanel(
                deskObject.transform,
                "RootLotRack",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(560f, -140f),
                new Vector2(390f, 430f),
                new Color(0.96f, 0.88f, 0.66f, 0.010f));
            lotRack.GetComponent<Image>().raycastTarget = false;

            RectTransform lotRackWash = CreateStretchPanel(
                lotRack,
                "RootLotRackPaperWash",
                Vector2.zero,
                Vector2.one,
                new Vector2(8f, 12f),
                new Vector2(-18f, -18f),
                new Color(0.86f, 0.72f, 0.42f, 0.095f));
            lotRackWash.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(lotRackWash, new Color(0.62f, 0.44f, 0.20f, 0.12f), new Color(1f, 0.96f, 0.82f, 0.016f), 1.2f, false);

            RectTransform rackInkVeil = CreateStretchPanel(
                lotRack,
                "RootLotRackInkVeil",
                Vector2.zero,
                Vector2.one,
                new Vector2(20f, 34f),
                new Vector2(-30f, -38f),
                new Color(0.12f, 0.08f, 0.04f, 0.035f));
            rackInkVeil.GetComponent<Image>().raycastTarget = false;

            RectTransform rackTitlePlate = CreateFixedPanel(
                lotRack,
                "RootLotRackTitlePlate",
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(-4f, -20f),
                new Vector2(216f, 46f),
                Color.white);
            ApplySpriteToImage(rackTitlePlate, LoadRootLotTagSprite(), new Color(0.96f, 0.88f, 0.66f, 0.92f), false);
            rackTitlePlate.localRotation = Quaternion.Euler(0f, 0f, -1.2f);
            rackTitlePlate.GetComponent<Image>().raycastTarget = false;

            Text rackTitle = CreateText(rackTitlePlate, "RootLotRackTitle", "命签匣", 17, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.30f, 0.20f, 0.10f, 0.98f));
            StretchText(rackTitle.rectTransform, new Vector2(26f, 6f), new Vector2(-42f, -6f));
            rackTitle.raycastTarget = false;

            int count = Mathf.Min(4, rewards.Count);
            RectTransform[] lotRects = new RectTransform[count];
            Image[] lotGlowImages = new Image[count];
            Image[] lotSelectedSealImages = new Image[count];
            Text[] lotSelectedSealTexts = new Text[count];
            Text[] lotNameTexts = new Text[count];
            Text[] lotHintTexts = new Text[count];
            Sprite[] rootSprites = new Sprite[count];

            for (int i = 0; i < count; i++)
            {
                int capturedIndex = i;
                DemoReward reward = rewards[i];
                RectTransform lotShadow = CreateFixedPanel(
                    lotRack,
                    "RootLotShadow_" + i,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(18f, -64f - i * 84f),
                    new Vector2(326f, 74f),
                    new Color(0.18f, 0.12f, 0.06f, 0.18f));
                lotShadow.localRotation = Quaternion.Euler(0f, 0f, -2.0f + i * 0.9f);
                lotShadow.GetComponent<Image>().raycastTarget = false;

                RectTransform lotRect = CreateFixedPanel(
                    lotRack,
                    "RootLot_" + i,
                    new Vector2(0.5f, 1f),
                    new Vector2(0.5f, 1f),
                    new Vector2(8f, -60f - i * 84f),
                    new Vector2(326f, 74f),
                    Color.white);
                lotRect.localRotation = Quaternion.Euler(0f, 0f, -2.0f + i * 0.9f);
                lotRects[i] = lotRect;
                ApplySpriteToImage(lotRect, LoadRootLotTagSprite(), IsRootClaimable(reward) ? new Color(1f, 1f, 1f, 0.96f) : new Color(0.90f, 0.86f, 0.76f, 0.88f), false);
                Button lotButton = lotRect.gameObject.AddComponent<Button>();
                lotButton.targetGraphic = lotRect.GetComponent<Image>();
                lotButton.colors = CreateButtonColors(new Color(1f, 0.96f, 0.86f, 0.94f));

                RectTransform lotGlow = CreateStretchPanel(lotRect, "RootLotGlow", Vector2.zero, Vector2.one, new Vector2(5f, 5f), new Vector2(-5f, -5f), new Color(0.75f, 0.56f, 0.26f, 0f));
                lotGlowImages[i] = lotGlow.GetComponent<Image>();
                lotGlowImages[i].raycastTarget = false;

                RectTransform lotSelectedSeal = CreateFixedPanel(
                    lotRect,
                    "RootLotSelectedSeal",
                    new Vector2(1f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-34f, 0f),
                    new Vector2(46f, 32f),
                    new Color(1f, 1f, 1f, 0f));
                ApplySpriteToImage(lotSelectedSeal, LoadRootConfirmSealSprite(), new Color(0.78f, 0.18f, 0.12f, 0f), true);
                lotSelectedSeal.localRotation = Quaternion.Euler(0f, 0f, -8f + i * 1.2f);
                lotSelectedSealImages[i] = lotSelectedSeal.GetComponent<Image>();
                lotSelectedSealImages[i].raycastTarget = false;

                Text lotSelectedText = CreateText(lotSelectedSeal, "RootLotSelectedText", "选", 13, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1.0f, 0.86f, 0.70f, 0f));
                StretchText(lotSelectedText.rectTransform, new Vector2(5f, 5f), new Vector2(-5f, -5f));
                lotSelectedSealTexts[i] = lotSelectedText;

                Text lotName = CreateText(lotRect, "RootLotName", reward.Name, 18, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.18f, 0.13f, 0.08f, 0.96f));
                lotName.rectTransform.anchorMin = new Vector2(0f, 0f);
                lotName.rectTransform.anchorMax = new Vector2(1f, 1f);
                lotName.rectTransform.offsetMin = new Vector2(34f, 18f);
                lotName.rectTransform.offsetMax = new Vector2(-66f, -8f);
                lotNameTexts[i] = lotName;

                Text lotHint = CreateText(lotRect, "RootLotHint", IsRootClaimable(reward) ? BuildRootLotHint(reward.Root) : "命数未显", 12, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.58f, 0.42f, 0.20f, 0.86f));
                lotHint.rectTransform.anchorMin = new Vector2(0f, 0f);
                lotHint.rectTransform.anchorMax = new Vector2(1f, 0f);
                lotHint.rectTransform.offsetMin = new Vector2(36f, 7f);
                lotHint.rectTransform.offsetMax = new Vector2(-70f, 25f);
                lotHintTexts[i] = lotHint;

                rootSprites[i] = LoadRootObjectSprite(reward.Root);
                lotButton.onClick.AddListener(() => deskObject.GetComponent<DemoRootDestinyDeskFx>().Select(capturedIndex));
            }

            List<RectTransform> idleSmokeRects = new List<RectTransform>();
            List<Image> idleSmokeImages = new List<Image>();
            for (int i = 0; i < 3; i++)
            {
                RectTransform smoke = CreateFixedPanel(
                    deskObject.transform,
                    "RootIdleSmoke_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(-656f + i * 6f, -148f + i * 2f),
                    new Vector2(82f + i * 8f, 152f + i * 16f),
                    new Color(1f, 1f, 1f, 0.04f));
                ApplySpriteToImage(smoke, LoadRootSmokeWispSprite(), new Color(0.86f, 0.90f, 0.88f, 0.10f), true);
                idleSmokeRects.Add(smoke);
                idleSmokeImages.Add(smoke.GetComponent<Image>());
                idleSmokeImages[idleSmokeImages.Count - 1].raycastTarget = false;
            }

            List<RectTransform> burstSmokeRects = new List<RectTransform>();
            List<Image> burstSmokeImages = new List<Image>();
            for (int i = 0; i < 7; i++)
            {
                RectTransform smoke = CreateFixedPanel(
                    ledgerPanel,
                    "RootBurstSmoke_" + i,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    new Vector2(80f, 10f),
                    new Vector2(220f + i * 16f, 220f + i * 18f),
                    new Color(0.88f, 0.90f, 0.88f, 0f));
                ApplySpriteToImage(smoke, LoadRootSmokeWispSprite(), new Color(0.88f, 0.90f, 0.88f, 0f), true);
                burstSmokeRects.Add(smoke);
                burstSmokeImages.Add(smoke.GetComponent<Image>());
                burstSmokeImages[burstSmokeImages.Count - 1].raycastTarget = false;
            }

            DemoRootDestinyDeskFx fx = deskObject.AddComponent<DemoRootDestinyDeskFx>();
            fx.Configure(
                rewards.ToArray(),
                rootSprites,
                null,
                null,
                rootNameText,
                rarityText,
                identityText,
                effectText,
                footerText,
                lotRects,
                lotGlowImages,
                lotSelectedSealImages,
                lotSelectedSealTexts,
                lotNameTexts,
                lotHintTexts,
                idleSmokeRects.ToArray(),
                idleSmokeImages.ToArray(),
                burstSmokeRects.ToArray(),
                burstSmokeImages.ToArray(),
                pageSmokeImage,
                confirmRect,
                confirmButton,
                confirmSealImage,
                confirmText,
                inkSweepRect);
            fx.SelectImmediate(0);
            EventTrigger confirmTrigger = confirmRect.gameObject.AddComponent<EventTrigger>();
            confirmTrigger.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry confirmEnter = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            confirmEnter.callback.AddListener(_ => fx.SetConfirmHover(true));
            confirmTrigger.triggers.Add(confirmEnter);

            EventTrigger.Entry confirmExit = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            confirmExit.callback.AddListener(_ =>
            {
                fx.SetConfirmHover(false);
                fx.SetConfirmPressed(false);
            });
            confirmTrigger.triggers.Add(confirmExit);

            EventTrigger.Entry confirmDown = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            confirmDown.callback.AddListener(_ => fx.SetConfirmPressed(true));
            confirmTrigger.triggers.Add(confirmDown);

            EventTrigger.Entry confirmUp = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            confirmUp.callback.AddListener(_ => fx.SetConfirmPressed(false));
            confirmTrigger.triggers.Add(confirmUp);

            confirmButton.onClick.AddListener(() =>
            {
                if (fx.CanClaimSelected)
                {
                    controller.ClaimRewardAt(fx.SelectedIndex);
                }
            });

            return deskObject;
        }

        private GameObject CreateOpeningItemChoiceEntry(Transform parent, DemoReward reward)
        {
            DemoJourneyLineDefinition line = reward.JourneyLine;
            Color accent = GetRewardAccentColor(reward);
            Sprite heroSprite = LoadOpeningSprite(reward);
            bool claimable = IsRewardClaimable(reward);

            GameObject cardObject = new GameObject("OpeningItemChoice_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);

            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(368f, 584f);

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 368f;
            layout.minWidth = 368f;
            layout.preferredHeight = 584f;
            layout.minHeight = 584f;

            Image cardImage = cardObject.GetComponent<Image>();
            cardImage.color = new Color(0.92f, 0.84f, 0.66f, 0.24f);
            cardImage.raycastTarget = true;

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = cardImage;
            button.colors = CreateButtonColors(new Color(0.95f, 0.90f, 0.78f, 0.98f));

            Color frameAccent = GetOpeningItemFrameAccent(accent);
            DecorateFrame(cardRect, new Color(frameAccent.r, frameAccent.g, frameAccent.b, 0.50f), new Color(1f, 0.96f, 0.84f, 0.045f), 1.2f, true);

            RectTransform innerShadow = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemInnerShadow",
                Vector2.zero,
                Vector2.one,
                new Vector2(12f, 10f),
                new Vector2(-12f, -12f),
                new Color(0.11f, 0.07f, 0.03f, 0.095f));
            innerShadow.GetComponent<Image>().raycastTarget = false;

            RectTransform artViewport = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemArtViewport",
                Vector2.zero,
                Vector2.one,
                new Vector2(12f, 12f),
                new Vector2(-12f, -12f),
                new Color(0.90f, 0.84f, 0.71f, 0.66f));
            artViewport.GetComponent<Image>().raycastTarget = false;
            artViewport.gameObject.AddComponent<RectMask2D>();

            RectTransform artRoot = CreateFixedPanel(
                artViewport,
                "OpeningItemArt",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                GetOpeningItemArtOffset(line),
                GetOpeningItemArtSize(line),
                Color.white);
            artRoot.GetComponent<Image>().raycastTarget = false;

            if (heroSprite != null)
            {
                ApplySpriteToImage(artRoot, heroSprite, Color.white, true);
            }

            RectTransform inkVeil = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemInkVeil",
                Vector2.zero,
                Vector2.one,
                new Vector2(12f, 12f),
                new Vector2(-12f, -12f),
                new Color(0.08f, 0.06f, 0.03f, 0.020f));
            inkVeil.GetComponent<Image>().raycastTarget = false;

            RectTransform artLight = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemLocalLight",
                new Vector2(0f, 0.50f),
                new Vector2(1f, 1f),
                new Vector2(18f, 14f),
                new Vector2(-18f, -18f),
                GetOpeningItemLightColor(line, accent));
            artLight.GetComponent<Image>().raycastTarget = false;

            RectTransform topWash = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemTopWash",
                new Vector2(0f, 0.78f),
                new Vector2(1f, 1f),
                new Vector2(10f, -10f),
                new Vector2(-10f, -10f),
                new Color(0.95f, 0.88f, 0.70f, 0.045f));
            topWash.GetComponent<Image>().raycastTarget = false;

            Color itemOverlayColor = new Color(0.91f, 0.82f, 0.62f, 0.93f);
            RectTransform bottomWash = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemBottomWash",
                new Vector2(0f, 0f),
                new Vector2(1f, 0.48f),
                new Vector2(10f, 10f),
                new Vector2(-10f, 0f),
                itemOverlayColor);
            bottomWash.GetComponent<Image>().raycastTarget = false;
            button.targetGraphic = cardImage;
            button.colors = CreateButtonColors(new Color(0.95f, 0.90f, 0.78f, 0.98f));
            DecorateFrame(bottomWash, new Color(accent.r, accent.g, accent.b, 0.32f), new Color(1f, 1f, 1f, 0.06f), 1f, false);

            RectTransform transitionWash = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemPaperTransition",
                new Vector2(0f, 0.455f),
                new Vector2(1f, 0.495f),
                new Vector2(10f, 0f),
                new Vector2(-10f, 0f),
                new Color(0.91f, 0.82f, 0.62f, 0.20f));
            transitionWash.GetComponent<Image>().raycastTarget = false;

            RectTransform bottomInk = CreateStretchPanel(
                bottomWash,
                "OpeningItemBottomInk",
                Vector2.zero,
                Vector2.one,
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(0.17f, 0.10f, 0.04f, 0.055f));
            bottomInk.GetComponent<Image>().raycastTarget = false;

            string journeyTitle = string.IsNullOrEmpty(line?.Title) ? reward.Name : line.Title;
            string itemName = string.IsNullOrEmpty(line?.CarryItemName) ? reward.Name : line.CarryItemName;
            string effect = string.IsNullOrEmpty(line?.CarryItemEffect) ? reward.Description ?? string.Empty : line.CarryItemEffect;
            string origin = string.IsNullOrEmpty(line?.OriginText) ? reward.Description ?? string.Empty : line.OriginText;

            Text titleText = CreateText(bottomWash, "OpeningItemJourneyTitle", TrimSentence(journeyTitle, 18), 20, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.45f, 0.27f, 0.09f, 0.98f));
            titleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            titleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            titleText.rectTransform.offsetMin = new Vector2(22f, -44f);
            titleText.rectTransform.offsetMax = new Vector2(-22f, -16f);

            Text itemText = CreateText(bottomWash, "OpeningItemName", itemName, 31, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.14f, 0.10f, 0.06f, 0.98f));
            itemText.rectTransform.anchorMin = new Vector2(0f, 1f);
            itemText.rectTransform.anchorMax = new Vector2(1f, 1f);
            itemText.rectTransform.offsetMin = new Vector2(22f, -82f);
            itemText.rectTransform.offsetMax = new Vector2(-22f, -43f);

            Text originText = CreateText(bottomWash, "OpeningItemOrigin", TrimSentence(origin, 34), 16, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.22f, 0.16f, 0.10f, 0.92f));
            originText.rectTransform.anchorMin = new Vector2(0f, 1f);
            originText.rectTransform.anchorMax = new Vector2(1f, 1f);
            originText.rectTransform.offsetMin = new Vector2(22f, -130f);
            originText.rectTransform.offsetMax = new Vector2(-22f, -86f);

            RectTransform effectPlate = CreateFixedPanel(
                bottomWash,
                "OpeningItemEffectPlate",
                new Vector2(0.5f, 0f),
                new Vector2(0.5f, 0f),
                new Vector2(0f, 92f),
                new Vector2(320f, 34f),
                new Color(0.25f, 0.16f, 0.08f, 0.78f));
            effectPlate.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(effectPlate, new Color(frameAccent.r, frameAccent.g, frameAccent.b, 0.32f), new Color(1f, 0.92f, 0.68f, 0.024f), 1f, false);

            Text effectText = CreateText(effectPlate, "OpeningItemEffect", "起势：" + BuildCompactLine(TrimSentence(effect, 22)), 15, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(1f, 0.88f, 0.62f, 0.98f));
            StretchText(effectText.rectTransform, new Vector2(13f, 3f), new Vector2(-13f, -3f));

            string[] tags = GetOpeningItemTags(line);
            for (int i = 0; i < tags.Length; i++)
            {
                CreateOpeningItemChip(bottomWash, "OpeningItemTag_" + i, tags[i], new Vector2(22f + i * 80f, 56f), 72f, frameAccent);
            }

            RectTransform actionLine = CreateStretchPanel(
                bottomWash,
                "OpeningItemActionLine",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(22f, 12f),
                new Vector2(-78f, 14f),
                new Color(frameAccent.r, frameAccent.g, frameAccent.b, 0.52f));
            actionLine.GetComponent<Image>().raycastTarget = false;

            RectTransform actionSeal = CreateFixedPanel(
                bottomWash,
                "OpeningItemActionSeal",
                new Vector2(1f, 0f),
                new Vector2(0.5f, 0.5f),
                new Vector2(-44f, 27f),
                new Vector2(52f, 38f),
                new Color(1f, 1f, 1f, 0f));
            ApplySpriteToImage(actionSeal, LoadRootConfirmSealSprite(), new Color(0.72f, 0.19f, 0.12f, 0f), true);
            actionSeal.localRotation = Quaternion.Euler(0f, 0f, -6f);
            actionSeal.GetComponent<Image>().raycastTarget = false;

            Text sealText = CreateText(actionSeal, "OpeningItemSealText", "选", 17, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(1f, 0.86f, 0.68f, 0f));
            StretchText(sealText.rectTransform, new Vector2(5f, 5f), new Vector2(-5f, -5f));

            Text actionText = CreateText(bottomWash, "OpeningItemAction", claimable ? "带上此物" : "尚未开放", 17, FontStyle.Bold, TextAnchor.LowerRight, new Color(0.36f, 0.20f, 0.08f, claimable ? 0.96f : 0.58f));
            actionText.rectTransform.anchorMin = new Vector2(0f, 0f);
            actionText.rectTransform.anchorMax = new Vector2(1f, 0f);
            actionText.rectTransform.offsetMin = new Vector2(20f, 9f);
            actionText.rectTransform.offsetMax = new Vector2(-78f, 35f);

            RectTransform hoverWash = CreateStretchPanel(
                cardObject.transform,
                "OpeningItemHoverWash",
                Vector2.zero,
                Vector2.one,
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(frameAccent.r, frameAccent.g, frameAccent.b, 0f));
            Image hoverWashImage = hoverWash.GetComponent<Image>();
            hoverWashImage.raycastTarget = false;

            RectTransform smoke = CreateFixedPanel(
                cardObject.transform,
                "OpeningItemSmoke",
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 88f),
                new Vector2(328f, 328f),
                new Color(1f, 1f, 1f, 0f));
            ApplySpriteToImage(smoke, LoadRootSmokeWispSprite(), new Color(accent.r, accent.g, accent.b, 0.10f), true);
            smoke.GetComponent<Image>().raycastTarget = false;

            DemoPathChoiceCardFx fx = cardObject.AddComponent<DemoPathChoiceCardFx>();
            fx.Register(smoke, new Vector2(7f, 10f), 0.20f, GetStableChoiceIndex(reward.Name, 8) * 0.73f, 1.8f, 0.014f, smoke.GetComponent<Image>(), 0.035f);
            fx.Register(actionSeal, new Vector2(1.2f, 0.8f), 0.34f, 1.5f + GetStableChoiceIndex(itemName, 9), 0.7f, 0.006f);

            DemoOpeningItemCardFx cardFx = cardObject.AddComponent<DemoOpeningItemCardFx>();
            cardFx.Configure(cardRect, cardImage, hoverWashImage, actionSeal.GetComponent<Image>(), sealText, actionText, frameAccent, claimable);
            AddOpeningItemCardEvents(cardObject, cardFx);
            DisableChildRaycastsExcept(cardObject, cardImage);

            AddRewardHoverEvents(cardObject, reward);
            return cardObject;
        }

        private RectTransform CreateOpeningItemChip(Transform parent, string name, string label, Vector2 anchoredPosition, float width, Color accent)
        {
            RectTransform chip = CreateFixedPanel(
                parent,
                name,
                new Vector2(0f, 0f),
                new Vector2(0f, 0f),
                anchoredPosition,
                new Vector2(width, 26f),
                new Color(0.94f, 0.86f, 0.66f, 0.42f));
            chip.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(chip, new Color(accent.r, accent.g, accent.b, 0.30f), new Color(1f, 0.96f, 0.82f, 0.012f), 1f, false);

            Text text = CreateText(chip, "OpeningItemChipText", label, 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.28f, 0.18f, 0.08f, 0.86f));
            StretchText(text.rectTransform, new Vector2(5f, 2f), new Vector2(-5f, -2f));
            return chip;
        }

        private static void AddOpeningSceneCardEvents(GameObject cardObject, DemoOpeningSceneCardFx fx)
        {
            if (cardObject == null || fx == null)
            {
                return;
            }

            EventTrigger trigger = cardObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = cardObject.AddComponent<EventTrigger>();
            }

            trigger.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener(_ => fx.SetHover(true));
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener(_ =>
            {
                fx.SetHover(false);
                fx.SetPressed(false);
            });
            trigger.triggers.Add(exitEntry);

            EventTrigger.Entry downEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            downEntry.callback.AddListener(_ => fx.SetPressed(true));
            trigger.triggers.Add(downEntry);

            EventTrigger.Entry upEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            upEntry.callback.AddListener(_ => fx.SetPressed(false));
            trigger.triggers.Add(upEntry);
        }
        private static void AddOpeningItemCardEvents(GameObject cardObject, DemoOpeningItemCardFx fx)
        {
            if (cardObject == null || fx == null)
            {
                return;
            }

            EventTrigger trigger = cardObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = cardObject.AddComponent<EventTrigger>();
            }

            trigger.triggers = new List<EventTrigger.Entry>();

            EventTrigger.Entry enterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            enterEntry.callback.AddListener(_ => fx.SetHover(true));
            trigger.triggers.Add(enterEntry);

            EventTrigger.Entry exitEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerExit
            };
            exitEntry.callback.AddListener(_ =>
            {
                fx.SetHover(false);
                fx.SetPressed(false);
            });
            trigger.triggers.Add(exitEntry);

            EventTrigger.Entry downEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown
            };
            downEntry.callback.AddListener(_ => fx.SetPressed(true));
            trigger.triggers.Add(downEntry);

            EventTrigger.Entry upEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerUp
            };
            upEntry.callback.AddListener(_ => fx.SetPressed(false));
            trigger.triggers.Add(upEntry);
        }

        private static void DisableChildRaycastsExcept(GameObject root, Graphic allowedGraphic)
        {
            if (root == null)
            {
                return;
            }

            Graphic[] graphics = root.GetComponentsInChildren<Graphic>(true);
            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] != null)
                {
                    graphics[i].raycastTarget = graphics[i] == allowedGraphic;
                }
            }
        }

        private static Color GetOpeningItemFrameAccent(Color accent)
        {
            return new Color(
                Mathf.Lerp(ColorGold.r, accent.r, 0.28f),
                Mathf.Lerp(ColorGold.g, accent.g, 0.28f),
                Mathf.Lerp(ColorGold.b, accent.b, 0.28f),
                1f);
        }

        private static Color GetOpeningItemLightColor(DemoJourneyLineDefinition line, Color accent)
        {
            string itemName = line?.CarryItemName ?? string.Empty;

            if (ContainsAny(itemName, "护身旧玉", "护身古玉", "护心古玉", "旧玉", "古玉"))
            {
                return new Color(0.50f, 0.78f, 0.62f, 0.090f);
            }

            if (ContainsAny(itemName, "余烬符匣", "余烬符函", "余烬", "符匣", "符函"))
            {
                return new Color(0.82f, 0.62f, 0.38f, 0.070f);
            }

            if (ContainsAny(itemName, "祖龛残匣", "祖纹残匣", "祖龛", "祖纹", "残匣"))
            {
                return new Color(0.20f, 0.70f, 0.72f, 0.085f);
            }

            return new Color(accent.r, accent.g, accent.b, 0.070f);
        }

        private static Vector2 GetOpeningItemArtOffset(DemoJourneyLineDefinition line)
        {
            string itemName = line?.CarryItemName ?? string.Empty;

            if (ContainsAny(itemName, "余烬符匣", "余烬符函", "余烬", "符匣", "符函"))
            {
                return new Vector2(0f, 84f);
            }

            if (ContainsAny(itemName, "祖龛残匣", "祖纹残匣", "祖龛", "祖纹", "残匣"))
            {
                return new Vector2(0f, 90f);
            }

            if (ContainsAny(itemName, "护身旧玉", "护身古玉", "护心古玉", "旧玉", "古玉"))
            {
                return new Vector2(0f, 90f);
            }

            if (ContainsAny(itemName, "雷骨", "雷钉", "雷印", "雷"))
            {
                return new Vector2(0f, 18f);
            }

            if (ContainsAny(itemName, "血玉", "血沁", "血胚", "血"))
            {
                return new Vector2(0f, 10f);
            }

            return new Vector2(0f, 22f);
        }

        private static Vector2 GetOpeningItemArtSize(DemoJourneyLineDefinition line)
        {
            string itemName = line?.CarryItemName ?? string.Empty;

            if (ContainsAny(itemName, "余烬符匣", "余烬符函", "余烬", "符匣", "符函"))
            {
                return new Vector2(350f, 525f);
            }

            if (ContainsAny(itemName, "祖龛残匣", "祖纹残匣", "祖龛", "祖纹", "残匣"))
            {
                return new Vector2(366f, 549f);
            }

            if (ContainsAny(itemName, "护身旧玉", "护身古玉", "护心古玉", "旧玉", "古玉"))
            {
                return new Vector2(351f, 527f);
            }

            if (ContainsAny(itemName, "雷骨", "雷钉", "雷印", "雷", "血玉", "血沁", "血胚", "血"))
            {
                return new Vector2(348f, 522f);
            }

            return new Vector2(366f, 550f);
        }

        private static bool IsPrimaryOpeningScene(DemoReward reward)
        {
            string regionId = reward?.Region?.Id ?? string.Empty;
            string primaryRegionId = reward?.JourneyLine?.FirstRegionId ?? string.Empty;
            return !string.IsNullOrEmpty(regionId)
                && !string.IsNullOrEmpty(primaryRegionId)
                && string.Equals(regionId, primaryRegionId, System.StringComparison.OrdinalIgnoreCase);
        }

        private GameObject CreateOpeningSceneChoiceEntry(Transform parent, DemoReward reward)
        {
            Color accent = GetRewardAccentColor(reward);
            Color panelColor = GetRewardPanelColor(reward);
            Sprite sceneSprite = LoadOpeningSprite(reward);
            string regionId = (reward.Region?.Id ?? string.Empty).ToLowerInvariant();
            bool isPrimary = IsPrimaryOpeningScene(reward);
            bool claimable = IsRewardClaimable(reward);
            float cardWidth = isPrimary ? 512f : 468f;
            float cardHeight = isPrimary ? 680f : 640f;

            GameObject cardObject = new GameObject("OpeningSceneChoice_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);

            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(cardWidth, cardHeight);

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = cardWidth;
            layout.minWidth = cardWidth;
            layout.preferredHeight = cardHeight;
            layout.minHeight = cardHeight;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;

            Image cardImage = cardObject.GetComponent<Image>();
            cardImage.color = isPrimary ? new Color(0.91f, 0.83f, 0.66f, 0.98f) : new Color(0.88f, 0.81f, 0.66f, 0.94f);
            cardImage.raycastTarget = true;

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = cardImage;
            button.colors = CreateButtonColors(new Color(0.95f, 0.90f, 0.78f, 0.98f));

            DecorateFrame(cardRect, new Color(accent.r, accent.g, accent.b, 0.48f), new Color(0.98f, 0.90f, 0.70f, 0.10f), 1.2f, true);

            RectTransform paperWash = CreateStretchPanel(
                cardRect,
                "SceneCardPaperWash",
                Vector2.zero,
                Vector2.one,
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(0.94f, 0.86f, 0.66f, 0.08f));
            paperWash.GetComponent<Image>().raycastTarget = false;

            RectTransform artShadow = CreateStretchPanel(
                cardRect,
                "SceneArtShadow",
                Vector2.zero,
                Vector2.one,
                new Vector2(18f, 172f),
                new Vector2(-18f, -80f),
                new Color(0.10f, 0.07f, 0.04f, 0.14f));
            artShadow.GetComponent<Image>().raycastTarget = false;

            RectTransform artViewport = CreateStretchPanel(
                cardRect,
                "SceneArtViewport",
                Vector2.zero,
                Vector2.one,
                new Vector2(22f, 178f),
                new Vector2(-22f, -84f),
                panelColor);
            Image artImage = artViewport.GetComponent<Image>();
            artImage.raycastTarget = false;
            if (sceneSprite != null)
            {
                ApplySpriteToImage(artViewport, sceneSprite, new Color(0.96f, 0.94f, 0.88f, 0.92f));
            }
            DecorateFrame(artViewport, new Color(0.99f, 0.90f, 0.66f, 0.22f), new Color(0.08f, 0.05f, 0.03f, 0.18f), 1f, false);

            RectTransform artPaperMist = CreateStretchPanel(
                artViewport,
                "SceneArtPaperMist",
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.95f, 0.90f, 0.76f, 0f));
            artPaperMist.GetComponent<Image>().raycastTarget = false;

            RectTransform artTopWash = CreateStretchPanel(
                artViewport,
                "SceneArtTopWash",
                new Vector2(0f, 0.66f),
                Vector2.one,
                Vector2.zero,
                Vector2.zero,
                new Color(0.95f, 0.86f, 0.63f, 0.035f));
            artTopWash.GetComponent<Image>().raycastTarget = false;

            RectTransform artBottomMist = CreateStretchPanel(
                artViewport,
                "SceneArtBottomMist",
                Vector2.zero,
                new Vector2(1f, 0.26f),
                Vector2.zero,
                Vector2.zero,
                new Color(0.08f, 0.05f, 0.03f, 0.16f));
            artBottomMist.GetComponent<Image>().raycastTarget = false;

            RectTransform topBand = CreateStretchPanel(
                cardRect,
                "SceneTopPaperBand",
                new Vector2(0f, 1f),
                Vector2.one,
                new Vector2(18f, -66f),
                new Vector2(-18f, -16f),
                new Color(0.93f, 0.84f, 0.64f, 0.86f));
            topBand.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(topBand, new Color(0.99f, 0.90f, 0.68f, 0.18f), new Color(0.20f, 0.12f, 0.05f, 0.16f), 1f, false);

            RectTransform riskChip = CreateFixedPanel(
                topBand,
                "SceneRiskChip",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(14f, 0f),
                new Vector2(74f, 30f),
                new Color(accent.r, accent.g, accent.b, 0.22f));
            riskChip.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(riskChip, new Color(accent.r, accent.g, accent.b, 0.42f), new Color(0.98f, 0.89f, 0.68f, 0.18f), 1f, false);
            Text riskText = CreateText(riskChip, "SceneRiskText", GetRegionRiskLabel(reward.Region), 18, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.22f, 0.14f, 0.08f, 0.94f));
            StretchText(riskText.rectTransform, new Vector2(8f, 3f), new Vector2(-8f, -3f));

            Text focusText = CreateText(topBand, "SceneFocusText", GetRegionFocusLabel(reward.Region), 19, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.32f, 0.20f, 0.10f, 0.92f));
            StretchText(focusText.rectTransform, new Vector2(104f, 4f), new Vector2(-18f, -4f));

            Image affinitySealImage = null;

            RectTransform infoPanel = CreateStretchPanel(
                cardRect,
                "SceneInfoPaper",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(18f, 18f),
                new Vector2(-18f, 168f),
                new Color(0.94f, 0.86f, 0.66f, 0.94f));
            infoPanel.GetComponent<Image>().raycastTarget = false;
            DecorateFrame(infoPanel, new Color(0.99f, 0.91f, 0.70f, 0.18f), new Color(0.20f, 0.12f, 0.05f, 0.16f), 1f, false);

            Text titleText = CreateText(infoPanel, "SceneTitle", reward.Name, 36, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.15f, 0.09f, 0.05f, 0.98f));
            StretchText(titleText.rectTransform, new Vector2(24f, 94f), new Vector2(-24f, -12f));

            Text hintText = CreateText(infoPanel, "SceneHint", BuildOpeningSceneTagline(reward.Region), 20, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.32f, 0.24f, 0.17f, 0.90f));
            StretchText(hintText.rectTransform, new Vector2(24f, 58f), new Vector2(-24f, -62f));

            RectTransform actionGround = CreateStretchPanel(
                infoPanel,
                "SceneActionSeal",
                new Vector2(1f, 0f),
                new Vector2(1f, 0f),
                new Vector2(-172f, 18f),
                new Vector2(-24f, 54f),
                new Color(accent.r, accent.g, accent.b, 0.20f));
            Image actionImage = actionGround.GetComponent<Image>();
            ApplySpriteToImage(actionGround, LoadRootConfirmSealSprite(), new Color(accent.r, accent.g, accent.b, 0.20f), true);
            actionImage.raycastTarget = false;
            DecorateFrame(actionGround, new Color(accent.r, accent.g, accent.b, 0.46f), new Color(0.98f, 0.90f, 0.68f, 0.16f), 1f, false);

            Text actionText = CreateText(actionGround, "SceneAction", claimable ? "踏入此境" : "尚未开放", 19, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.23f, 0.12f, 0.06f, claimable ? 0.96f : 0.58f));
            StretchText(actionText.rectTransform, new Vector2(8f, 3f), new Vector2(-8f, -3f));

            RectTransform accentLine = CreateStretchPanel(
                cardRect,
                "SceneCardAccentLine",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(28f, 8f),
                new Vector2(-28f, 13f),
                new Color(accent.r, accent.g, accent.b, 0.80f));
            Image accentLineImage = accentLine.GetComponent<Image>();
            accentLineImage.raycastTarget = false;

            RectTransform hoverWash = CreateStretchPanel(
                cardRect,
                "SceneHoverWash",
                Vector2.zero,
                Vector2.one,
                new Vector2(10f, 10f),
                new Vector2(-10f, -10f),
                new Color(accent.r, accent.g, accent.b, 0f));
            Image hoverWashImage = hoverWash.GetComponent<Image>();
            hoverWashImage.raycastTarget = false;

            DemoOpeningSceneCardFx sceneFx = cardObject.AddComponent<DemoOpeningSceneCardFx>();
            sceneFx.Configure(cardRect, cardImage, hoverWashImage, actionImage, actionText, accentLineImage, affinitySealImage, accent, isPrimary, claimable);
            AddOpeningSceneCardEvents(cardObject, sceneFx);

            DisableChildRaycastsExcept(cardObject, cardImage);
            AddRewardHoverEvents(cardObject, reward);
            return cardObject;
        }

        private GameObject CreateOpeningChoiceEntry(Transform parent, DemoReward reward)
        {
            bool isRoot = reward.Type == DemoRewardType.Root;
            bool isTrace = reward.Type == DemoRewardType.Trace;
            bool isItem = (reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel);
            bool isScene = reward.Type == DemoRewardType.OpeningScene;
            if (isItem)
            {
                return CreateOpeningItemChoiceEntry(parent, reward);
            }

            if (isScene)
            {
                return CreateOpeningSceneChoiceEntry(parent, reward);
            }

            Color accent = GetRewardAccentColor(reward);
            Color panelColor = GetRewardPanelColor(reward);
            Sprite heroSprite = LoadOpeningSprite(reward);
            bool hasHeroSprite = heroSprite != null;

            GameObject cardObject = new GameObject("OpeningChoice_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();

            float width = isRoot ? 312f : isScene ? 360f : 348f;
            float height = isRoot ? 520f : isScene ? 468f : 520f;

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = width;
            layout.minWidth = width;
            layout.preferredHeight = height;
            layout.minHeight = height;
            cardRect.sizeDelta = new Vector2(width, height);

            Image image = cardObject.GetComponent<Image>();
            image.color = new Color(0.10f, 0.11f, 0.13f, 0.98f);

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(new Color(0.16f, 0.17f, 0.20f, 1f));
            DecorateFrame(cardRect, accent, new Color(0.15f, 0.16f, 0.18f, 0.20f), 2f, true);

            VerticalLayoutGroup layoutGroup = cardObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(18, 18, 18, 18);
            layoutGroup.spacing = 12f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            RectTransform heroBand = CreateFixedHeightBlock(cardObject.transform, "OpeningHero", isRoot ? 156f : isScene ? 252f : 286f, panelColor);
            DecorateFrame(heroBand, accent, new Color(0.14f, 0.15f, 0.18f, 0.12f), 1.5f, false);
            if (hasHeroSprite)
            {
                ApplySpriteToImage(heroBand, heroSprite, Color.white);
                CreateStretchPanel(
                    heroBand,
                    "OpeningHeroVeil",
                    new Vector2(0f, 0f),
                    new Vector2(1f, 1f),
                    new Vector2(6f, 6f),
                    new Vector2(-6f, -6f),
                    isScene ? new Color(0f, 0f, 0f, 0.10f) : new Color(1f, 1f, 1f, 0.08f));
                CreateStretchPanel(
                    heroBand,
                    "OpeningHeroShadow",
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0.40f),
                    new Vector2(6f, 6f),
                    new Vector2(-6f, -6f),
                    new Color(0f, 0f, 0f, isScene ? 0.30f : 0.22f));
            }
            CreateStretchPanel(
                heroBand,
                "OpeningHeroGlow",
                new Vector2(0f, 0.42f),
                new Vector2(1f, 1f),
                new Vector2(10f, 8f),
                new Vector2(-10f, -10f),
                new Color(accent.r, accent.g, accent.b, 0.12f));

            RectTransform iconSeal = CreateFixedPanel(
                heroBand,
                "OpeningIconSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -18f),
                new Vector2(56f, 56f),
                new Color(0.08f, 0.09f, 0.11f, 0.92f));
            DecorateFrame(iconSeal, accent, new Color(0f, 0f, 0f, 0f), 1.5f, false);
            Text iconText = CreateText(iconSeal, "OpeningIconText", GetRewardGlyph(reward), 26, FontStyle.Bold, TextAnchor.MiddleCenter, accent);
            StretchText(iconText.rectTransform, new Vector2(6f, 6f), new Vector2(-6f, -6f));

            Text typeText = CreateText(heroBand, "OpeningTypeText", GetRewardTypeLabel(reward), 12, FontStyle.Bold, TextAnchor.UpperLeft, accent);
            typeText.rectTransform.anchorMin = new Vector2(0f, 1f);
            typeText.rectTransform.anchorMax = new Vector2(1f, 1f);
            typeText.rectTransform.offsetMin = new Vector2(86f, -34f);
            typeText.rectTransform.offsetMax = new Vector2(-14f, -10f);

            Text heroTitle = CreateText(heroBand, "OpeningTitle", reward.Name, isRoot ? 28 : isScene ? 26 : 29, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.97f, 0.95f, 0.90f, 1f));
            heroTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            heroTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            heroTitle.rectTransform.offsetMin = new Vector2(84f, -74f);
            heroTitle.rectTransform.offsetMax = new Vector2(-14f, -30f);

            Text heroHook = CreateText(heroBand, "OpeningHook", BuildOpeningHookText(reward), isScene ? 14 : 15, FontStyle.Normal, TextAnchor.LowerLeft, new Color(0.90f, 0.91f, 0.88f, 0.98f));
            heroHook.rectTransform.anchorMin = new Vector2(0f, 0f);
            heroHook.rectTransform.anchorMax = new Vector2(1f, 0f);
            heroHook.rectTransform.offsetMin = new Vector2(16f, 18f);
            heroHook.rectTransform.offsetMax = new Vector2(-16f, isScene ? 48f : 56f);

            GameObject bodyObject = new GameObject("OpeningBody", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            bodyObject.transform.SetParent(cardObject.transform, false);
            LayoutElement bodyLayout = bodyObject.GetComponent<LayoutElement>();
            bodyLayout.flexibleHeight = 1f;
            VerticalLayoutGroup bodyGroup = bodyObject.GetComponent<VerticalLayoutGroup>();
            bodyGroup.spacing = 8f;
            bodyGroup.childAlignment = TextAnchor.UpperLeft;
            bodyGroup.childControlHeight = true;
            bodyGroup.childControlWidth = true;
            bodyGroup.childForceExpandHeight = false;
            bodyGroup.childForceExpandWidth = true;

            GameObject metaRow = new GameObject("OpeningMeta", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            metaRow.transform.SetParent(bodyObject.transform, false);
            LayoutElement metaLayout = metaRow.GetComponent<LayoutElement>();
            metaLayout.preferredHeight = 24f;
            HorizontalLayoutGroup metaGroup = metaRow.GetComponent<HorizontalLayoutGroup>();
            metaGroup.spacing = 8f;
            metaGroup.childAlignment = TextAnchor.MiddleLeft;
            metaGroup.childControlHeight = true;
            metaGroup.childControlWidth = false;
            metaGroup.childForceExpandHeight = false;
            metaGroup.childForceExpandWidth = false;

            CreateLabelChip(metaRow.transform, GetQualityLongLabel(GetRewardQuality(reward)), GetQualityColor(GetRewardQuality(reward)), ColorPaper, 62f);
            CreateLabelChip(metaRow.transform, GetRewardFlavorText(reward), new Color(0.13f, 0.16f, 0.19f, 1f), new Color(0.90f, 0.92f, 0.95f, 1f), isRoot ? 100f : 108f);

            if (!string.IsNullOrEmpty(GetOpeningMetaChipText(reward)))
            {
                CreateLabelChip(metaRow.transform, GetOpeningMetaChipText(reward), new Color(0.17f, 0.14f, 0.11f, 1f), accent, isRoot ? 112f : 118f);
            }

            Text summaryText = CreateText(bodyObject.transform, "OpeningSummary", BuildOpeningPreviewText(reward), isScene ? 16 : 18, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.95f, 0.94f, 0.90f, 1f));
            LayoutElement summaryLayout = summaryText.gameObject.AddComponent<LayoutElement>();
            summaryLayout.flexibleHeight = 1f;

            RectTransform keywordBand = CreateFixedHeightBlock(bodyObject.transform, "OpeningKeywords", isRoot ? 108f : isScene ? 96f : 118f, new Color(0.08f, 0.10f, 0.12f, 0.94f));
            DecorateFrame(keywordBand, new Color(accent.r, accent.g, accent.b, 0.56f), new Color(0f, 0f, 0f, 0f), 1.5f, false);

            string[] keywords = GetOpeningKeywords(reward);
            int keywordCount = Mathf.Min(isRoot ? 2 : isScene ? 3 : 3, keywords.Length);
            for (int i = 0; i < keywordCount; i++)
            {
                Text keywordText = CreateText(keywordBand, "OpeningKeyword_" + i, "· " + keywords[i], 13, FontStyle.Bold, TextAnchor.MiddleLeft, new Color(0.95f, 0.94f, 0.90f, 1f));
                keywordText.rectTransform.anchorMin = new Vector2(0f, 1f);
                keywordText.rectTransform.anchorMax = new Vector2(1f, 1f);
                keywordText.rectTransform.offsetMin = new Vector2(14f, -28f - i * 22f);
                keywordText.rectTransform.offsetMax = new Vector2(-14f, -8f - i * 22f);
            }

            Text keywordHint = CreateText(keywordBand, "OpeningKeywordHint", BuildOpeningFooterHint(reward), 12, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.88f, 0.76f, 0.48f, 0.98f));
            keywordHint.rectTransform.anchorMin = new Vector2(0f, 0f);
            keywordHint.rectTransform.anchorMax = new Vector2(1f, 0f);
            keywordHint.rectTransform.offsetMin = new Vector2(12f, 8f);
            keywordHint.rectTransform.offsetMax = new Vector2(-12f, 24f);

            RectTransform footer = CreateFixedHeightBlock(cardObject.transform, "OpeningFooter", 60f, new Color(0.09f, 0.10f, 0.12f, 0.96f));
            CreateStretchPanel(
                footer,
                "OpeningFooterAccent",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(14f, 0f),
                new Vector2(-14f, 3f),
                accent);
            Text footerText = CreateText(
                footer,
                "OpeningFooterText",
                isTrace ? "点击携此道痕" : isRoot ? "点击定下根脚" : isItem ? "点击携此上路" : "点击定下所往",
                17,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                ColorPaper);
            StretchText(footerText.rectTransform, new Vector2(8f, 6f), new Vector2(-8f, -18f));

            AddRewardHoverEvents(cardObject, reward);
            return cardObject;
        }

        private GameObject CreateRouteChoiceEntry(Transform parent, DemoReward reward)
        {
            Color accent = GetRouteAccentColor(reward);
            Sprite heroSprite = LoadRouteChoiceSprite(reward);
            bool hasHeroSprite = heroSprite != null;
            Color routeInk = new Color(0.18f, 0.15f, 0.10f, 0.98f);

            GameObject cardObject = new GameObject("RouteChoice_" + reward.Name, typeof(RectTransform), typeof(Image), typeof(Button), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            cardObject.transform.SetParent(parent, false);

            LayoutElement layout = cardObject.GetComponent<LayoutElement>();
            layout.preferredWidth = 404f;
            layout.minWidth = 404f;
            layout.preferredHeight = 560f;
            layout.minHeight = 560f;

            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.sizeDelta = new Vector2(404f, 560f);

            Image image = cardObject.GetComponent<Image>();
            image.color = new Color(0.95f, 0.91f, 0.81f, 0.98f);

            Button button = cardObject.GetComponent<Button>();
            button.targetGraphic = image;
            button.colors = CreateButtonColors(new Color(0.95f, 0.91f, 0.81f, 1f));
            DecorateFrame(cardRect, new Color(accent.r * 0.76f, accent.g * 0.76f, accent.b * 0.76f, 0.82f), new Color(0.99f, 0.97f, 0.91f, 0.12f), 1.5f, false);

            VerticalLayoutGroup layoutGroup = cardObject.GetComponent<VerticalLayoutGroup>();
            layoutGroup.padding = new RectOffset(12, 12, 12, 12);
            layoutGroup.spacing = 8f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlHeight = true;
            layoutGroup.childControlWidth = true;
            layoutGroup.childForceExpandHeight = false;
            layoutGroup.childForceExpandWidth = true;

            RectTransform heroBand = CreateFixedHeightBlock(cardObject.transform, "RouteHero", 228f, new Color(0.83f, 0.80f, 0.71f, 0.96f));
            DecorateFrame(heroBand, new Color(accent.r, accent.g, accent.b, 0.46f), new Color(1f, 1f, 1f, 0.04f), 1f, false);
            if (hasHeroSprite)
            {
                ApplySpriteToImage(heroBand, heroSprite, new Color(0.96f, 0.96f, 0.91f, 0.92f));
            }

            RectTransform heroPaperWash = CreateStretchPanel(
                heroBand,
                "RouteHeroPaperWash",
                Vector2.zero,
                Vector2.one,
                new Vector2(5f, 5f),
                new Vector2(-5f, -5f),
                new Color(0.96f, 0.93f, 0.84f, hasHeroSprite ? 0.14f : 0.36f));
            heroPaperWash.GetComponent<Image>().raycastTarget = false;
            RectTransform heroTitleWash = CreateStretchPanel(
                heroBand,
                "RouteHeroTitleWash",
                Vector2.zero,
                new Vector2(1f, 0.42f),
                new Vector2(5f, 5f),
                new Vector2(-5f, -5f),
                new Color(0.96f, 0.92f, 0.81f, 0.88f));
            heroTitleWash.GetComponent<Image>().raycastTarget = false;

            RectTransform glyphSeal = CreateFixedPanel(
                heroBand,
                "RouteGlyphSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(18f, -18f),
                new Vector2(50f, 50f),
                new Color(0.96f, 0.92f, 0.83f, 0.92f));
            glyphSeal.localRotation = Quaternion.Euler(0f, 0f, -3f);
            DecorateFrame(glyphSeal, new Color(accent.r, accent.g, accent.b, 0.78f), new Color(accent.r, accent.g, accent.b, 0.08f), 1f, false);
            Text glyphText = CreateText(glyphSeal, "RouteGlyphText", GetRewardGlyph(reward), 23, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(accent.r * 0.72f, accent.g * 0.72f, accent.b * 0.72f, 0.98f));
            StretchText(glyphText.rectTransform, new Vector2(6f, 6f), new Vector2(-6f, -6f));

            RectTransform paceTag = CreateFixedPanel(
                heroBand,
                "RoutePaceTag",
                new Vector2(1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(-18f, -18f),
                new Vector2(86f, 28f),
                new Color(0.95f, 0.90f, 0.79f, 0.90f));
            DecorateFrame(paceTag, new Color(0.46f, 0.32f, 0.16f, 0.32f), new Color(1f, 1f, 1f, 0.04f), 1f, false);
            Text paceText = CreateText(paceTag, "RoutePaceText", BuildRoutePaceTag(reward), 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.35f, 0.25f, 0.14f, 0.92f));
            StretchText(paceText.rectTransform, new Vector2(6f, 3f), new Vector2(-6f, -3f));

            Text titleText = CreateText(heroBand, "RouteTitle", reward.Name, 28, FontStyle.Bold, TextAnchor.LowerLeft, routeInk);
            titleText.rectTransform.anchorMin = Vector2.zero;
            titleText.rectTransform.anchorMax = new Vector2(1f, 0f);
            titleText.rectTransform.offsetMin = new Vector2(18f, 42f);
            titleText.rectTransform.offsetMax = new Vector2(-18f, 78f);

            Text hintText = CreateText(heroBand, "RouteHint", BuildRouteChoiceHint(reward), 13, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.42f, 0.31f, 0.17f, 0.88f));
            hintText.rectTransform.anchorMin = Vector2.zero;
            hintText.rectTransform.anchorMax = new Vector2(1f, 0f);
            hintText.rectTransform.offsetMin = new Vector2(18f, 15f);
            hintText.rectTransform.offsetMax = new Vector2(-18f, 40f);

            GameObject metaRow = new GameObject("RouteMeta", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            metaRow.transform.SetParent(cardObject.transform, false);
            LayoutElement metaLayout = metaRow.GetComponent<LayoutElement>();
            metaLayout.preferredHeight = 26f;
            HorizontalLayoutGroup metaGroup = metaRow.GetComponent<HorizontalLayoutGroup>();
            metaGroup.spacing = 8f;
            metaGroup.childAlignment = TextAnchor.MiddleLeft;
            metaGroup.childControlHeight = true;
            metaGroup.childControlWidth = false;
            metaGroup.childForceExpandHeight = false;
            metaGroup.childForceExpandWidth = false;
            CreateLabelChip(metaRow.transform, GetQualityLongLabel(GetRewardQuality(reward)), GetQualityColor(GetRewardQuality(reward)), new Color(0.98f, 0.94f, 0.84f, 1f), 64f);
            CreateLabelChip(metaRow.transform, $"{reward.RoutePlan.Nodes.Count} 处历练", new Color(0.86f, 0.79f, 0.64f, 0.92f), new Color(0.29f, 0.22f, 0.14f, 0.92f), 92f);
            CreateLabelChip(metaRow.transform, GetRewardFlavorText(reward), new Color(0.90f, 0.84f, 0.71f, 0.92f), new Color(accent.r * 0.68f, accent.g * 0.68f, accent.b * 0.68f, 0.98f), 104f);

            RectTransform nodeBand = CreateFixedHeightBlock(cardObject.transform, "RouteNodes", 122f, new Color(0.90f, 0.85f, 0.72f, 0.92f));
            DecorateFrame(nodeBand, new Color(accent.r, accent.g, accent.b, 0.38f), new Color(1f, 1f, 1f, 0.04f), 1f, false);
            BuildRouteNodeSequence(nodeBand, reward);

            Text decisionText = CreateText(cardObject.transform, "RouteDecision", BuildRouteDecisionText(reward), 13, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.33f, 0.31f, 0.25f, 0.90f));
            decisionText.lineSpacing = 1.06f;
            decisionText.verticalOverflow = VerticalWrapMode.Truncate;
            LayoutElement decisionLayout = decisionText.gameObject.AddComponent<LayoutElement>();
            decisionLayout.flexibleHeight = 1f;

            RectTransform footer = CreateFixedHeightBlock(cardObject.transform, "RouteAction", 50f, new Color(0.89f, 0.82f, 0.67f, 0.94f));
            DecorateFrame(footer, new Color(0.47f, 0.33f, 0.16f, 0.26f), new Color(1f, 1f, 1f, 0.04f), 1f, false);
            RectTransform actionSeal = CreateFixedPanel(
                footer,
                "RouteActionSeal",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(18f, 0f),
                new Vector2(6f, 28f),
                new Color(0.55f, 0.16f, 0.10f, 0.88f));
            actionSeal.GetComponent<Image>().raycastTarget = false;
            Text actionText = CreateText(footer, "RouteActionText", "走这段路", 16, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.24f, 0.18f, 0.11f, 0.98f));
            StretchText(actionText.rectTransform, new Vector2(18f, 6f), new Vector2(-18f, -6f));

            AddRewardHoverEvents(cardObject, reward);
            return cardObject;
        }

        private GameObject CreatePathChoiceEntry(Transform parent, DemoReward reward)
        {
            if (reward == null || reward.Type != DemoRewardType.Gongfa)
            {
                return new GameObject("InvalidPathChoice", typeof(RectTransform));
            }

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
                slotText = CreateText(slotChip, "SlotText", "起手主位", 11, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
                StretchText(slotText.rectTransform, new Vector2(6f, 2f), new Vector2(-6f, -2f));
            }
            else
            {
                slotText = CreateText(cardObject.transform, "SlotText", "起手主位", 12, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.45f, 0.37f, 0.20f, 0.92f));
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
            Text footerText = CreateText(footer, "FooterText", "点击定下主位", 14, FontStyle.Bold, TextAnchor.MiddleCenter, ColorPaper);
            StretchText(footerText.rectTransform, new Vector2(8f, 4f), new Vector2(-8f, -4f));
            return cardObject;
        }

        private void BuildNodeChoiceStage(RectTransform parent)
        {
            RectTransform stageLift = CreateStretchPanel(
                parent,
                "StagePaperLift",
                Vector2.zero,
                Vector2.one,
                new Vector2(8f, 8f),
                new Vector2(-8f, -8f),
                new Color(0.99f, 0.97f, 0.91f, 0.28f));
            stageLift.GetComponent<Image>().raycastTarget = false;

            nodeGuidanceStageRoot = new GameObject("NodeGuidanceStageRoot", typeof(RectTransform));
            nodeGuidanceStageRoot.transform.SetParent(parent, false);
            StretchText(nodeGuidanceStageRoot.GetComponent<RectTransform>(), new Vector2(24f, 22f), new Vector2(-24f, -22f));

            RectTransform focusPanel = CreateInkPanel(
                nodeGuidanceStageRoot.transform,
                "GuidanceFocusPanel",
                new Vector2(0f, 0f),
                new Vector2(0.70f, 1f),
                Vector2.zero,
                new Vector2(-18f, 0f),
                new Color(0.98f, 0.95f, 0.87f, 0.20f));

            RectTransform focusSeal = CreateFixedPanel(
                focusPanel,
                "GuidanceFocusSeal",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(28f, -26f),
                new Vector2(34f, 74f),
                new Color(0.50f, 0.14f, 0.09f, 0.88f));
            focusSeal.GetComponent<Image>().raycastTarget = false;
            Text focusSealText = CreateText(focusSeal, "GuidanceFocusSealText", "历\n练", 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.98f, 0.89f, 0.76f, 0.98f));
            StretchText(focusSealText.rectTransform, new Vector2(4f, 6f), new Vector2(-4f, -6f));

            Text focusEyebrow = CreateText(focusPanel, "GuidanceEyebrow", "旧矿地窟 · 节点历练", 13, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.52f, 0.36f, 0.16f, 0.86f));
            focusEyebrow.rectTransform.anchorMin = new Vector2(0f, 1f);
            focusEyebrow.rectTransform.anchorMax = new Vector2(1f, 1f);
            focusEyebrow.rectTransform.offsetMin = new Vector2(80f, -32f);
            focusEyebrow.rectTransform.offsetMax = new Vector2(-26f, -10f);

            nodeStageTitleText = CreateText(focusPanel, "GuidanceTitle", string.Empty, 30, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.15f, 0.13f, 0.09f, 0.98f));
            nodeStageTitleText.rectTransform.anchorMin = new Vector2(0f, 1f);
            nodeStageTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeStageTitleText.rectTransform.offsetMin = new Vector2(80f, -78f);
            nodeStageTitleText.rectTransform.offsetMax = new Vector2(-28f, -38f);

            RectTransform titleRule = CreateStretchPanel(
                focusPanel,
                "GuidanceTitleRule",
                new Vector2(0f, 1f),
                new Vector2(0f, 1f),
                new Vector2(80f, -92f),
                new Vector2(430f, -90f),
                new Color(0.54f, 0.39f, 0.19f, 0.46f));
            titleRule.GetComponent<Image>().raycastTarget = false;

            nodeStageBodyText = CreateText(focusPanel, "GuidanceBody", string.Empty, 17, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.25f, 0.24f, 0.19f, 0.92f));
            nodeStageBodyText.lineSpacing = 1.16f;
            nodeStageBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            nodeStageBodyText.rectTransform.anchorMax = new Vector2(1f, 1f);
            nodeStageBodyText.rectTransform.offsetMin = new Vector2(80f, 46f);
            nodeStageBodyText.rectTransform.offsetMax = new Vector2(-34f, -112f);

            Text decisionHint = CreateText(focusPanel, "GuidanceDecisionHint", "完成此处，继续沿已定前路深入。", 13, FontStyle.Bold, TextAnchor.LowerLeft, new Color(0.50f, 0.35f, 0.16f, 0.82f));
            decisionHint.rectTransform.anchorMin = Vector2.zero;
            decisionHint.rectTransform.anchorMax = new Vector2(1f, 0f);
            decisionHint.rectTransform.offsetMin = new Vector2(80f, 18f);
            decisionHint.rectTransform.offsetMax = new Vector2(-28f, 40f);

            RectTransform checklistPanel = CreateInkPanel(
                nodeGuidanceStageRoot.transform,
                "GuidanceChecklistPanel",
                new Vector2(0.70f, 0f),
                new Vector2(1f, 1f),
                new Vector2(18f, 0f),
                Vector2.zero,
                new Color(0.90f, 0.84f, 0.69f, 0.42f));

            Text checklistTitle = CreateText(checklistPanel, "GuidanceChecklistTitle", "此处所求", 18, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.29f, 0.20f, 0.11f, 0.96f));
            checklistTitle.rectTransform.anchorMin = new Vector2(0f, 1f);
            checklistTitle.rectTransform.anchorMax = new Vector2(1f, 1f);
            checklistTitle.rectTransform.offsetMin = new Vector2(24f, -38f);
            checklistTitle.rectTransform.offsetMax = new Vector2(-24f, -12f);

            RectTransform checklistRule = CreateStretchPanel(
                checklistPanel,
                "GuidanceChecklistRule",
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(24f, -48f),
                new Vector2(-24f, -46f),
                new Color(0.52f, 0.37f, 0.17f, 0.36f));
            checklistRule.GetComponent<Image>().raycastTarget = false;

            nodeStageChecklistText = CreateBodyText(checklistPanel, "GuidanceChecklistText", 16, new Color(0.27f, 0.25f, 0.20f, 0.92f));
            nodeStageChecklistText.lineSpacing = 1.18f;
            StretchText(nodeStageChecklistText.rectTransform, new Vector2(24f, 44f), new Vector2(-24f, -62f));

            Text checklistFooter = CreateText(checklistPanel, "GuidanceChecklistFooter", "稳住气血 · 收束构筑", 12, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.54f, 0.16f, 0.10f, 0.74f));
            checklistFooter.rectTransform.anchorMin = Vector2.zero;
            checklistFooter.rectTransform.anchorMax = new Vector2(1f, 0f);
            checklistFooter.rectTransform.offsetMin = new Vector2(16f, 16f);
            checklistFooter.rectTransform.offsetMax = new Vector2(-16f, 36f);
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

        private static Sprite LoadOpeningSprite(DemoReward reward)
        {
            if (reward == null)
            {
                return null;
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                return LoadOpeningItemSprite(reward.JourneyLine);
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                string regionId = reward.Region.Id ?? string.Empty;
                if (regionId.Contains("thunder"))
                {
                    if (cachedOpeningSceneThunderMarshSprite == null)
                    {
                        cachedOpeningSceneThunderMarshSprite = LoadSpriteResource(OpeningSceneThunderMarshResourcePath, SceneBattleCloudseaResourcePath);
                    }

                    return cachedOpeningSceneThunderMarshSprite;
                }

                if (regionId.Contains("herb") || regionId.Contains("forest"))
                {
                    if (cachedOpeningSceneHerbForestSprite == null)
                    {
                        cachedOpeningSceneHerbForestSprite = LoadSpriteResource(OpeningSceneHerbForestResourcePath, OpeningSceneTradeRoadResourcePath, SceneCloudseaFarResourcePath);
                    }

                    return cachedOpeningSceneHerbForestSprite;
                }

                if (regionId.Contains("ancestral") || regionId.Contains("vault"))
                {
                    if (cachedOpeningSceneAncestralVaultSprite == null)
                    {
                        cachedOpeningSceneAncestralVaultSprite = LoadSpriteResource(OpeningSceneAncestralVaultResourcePath, OpeningSceneOldMineResourcePath, SceneCloudseaFarResourcePath);
                    }

                    return cachedOpeningSceneAncestralVaultSprite;
                }

                if (regionId.Contains("demon") || regionId.Contains("tower"))
                {
                    if (cachedOpeningSceneDemonTowerSprite == null)
                    {
                        cachedOpeningSceneDemonTowerSprite = LoadSpriteResource(OpeningSceneDemonTowerResourcePath, OpeningSceneThunderMarshResourcePath, SceneBattleCloudseaResourcePath);
                    }

                    return cachedOpeningSceneDemonTowerSprite;
                }

                if (regionId.Contains("old_mine"))
                {
                    if (cachedOpeningSceneOldMineSprite == null)
                    {
                        cachedOpeningSceneOldMineSprite = LoadSpriteResource(OpeningSceneOldMineResourcePath, SceneCloudseaFarResourcePath);
                    }

                    return cachedOpeningSceneOldMineSprite;
                }

                if (cachedOpeningSceneTradeRoadSprite == null)
                {
                    cachedOpeningSceneTradeRoadSprite = LoadSpriteResource(OpeningSceneTradeRoadResourcePath, SceneBattleCloudseaResourcePath, SceneCloudseaFarResourcePath);
                }

                return cachedOpeningSceneTradeRoadSprite;
            }

            return null;
        }

        private static Sprite LoadOpeningItemSprite(DemoJourneyLineDefinition line)
        {
            string itemName = line?.CarryItemName ?? string.Empty;
            string lineId = line?.Id ?? itemName;

            if (ContainsAny(itemName, "护身旧玉", "护身古玉", "护心古玉", "旧玉", "古玉"))
            {
                return LoadOpeningItemProtectiveJadeSprite();
            }

            if (ContainsAny(itemName, "余烬符匣", "余烬符函", "余烬", "符匣", "符函"))
            {
                return LoadOpeningItemSwordcaseSprite();
            }

            if (ContainsAny(itemName, "祖龛残匣", "祖纹残匣", "祖龛", "祖纹", "残匣"))
            {
                return LoadOpeningItemAncestralCasketSprite();
            }

            if (ContainsAny(itemName, "雷骨", "雷钉", "雷印", "雷"))
            {
                return LoadOpeningItemThunderboneSprite();
            }

            if (ContainsAny(itemName, "血玉", "血沁", "血胚", "血"))
            {
                return LoadOpeningItemBloodjadeSprite();
            }

            if (ContainsAny(itemName, "剑匣", "旧匣", "剑胚", "旧木剑", "木剑"))
            {
                return LoadOpeningItemSwordcaseSprite();
            }

            return LoadOpeningItemExtensionSprite(GetStableChoiceIndex(lineId, 3));
        }

        private static Sprite LoadOpeningItemExtensionSprite(int index)
        {
            switch (index)
            {
                case 1:
                    return LoadOpeningItemThunderboneSprite();
                case 2:
                    return LoadOpeningItemBloodjadeSprite();
                default:
                    return LoadOpeningItemSwordcaseSprite();
            }
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            if (string.IsNullOrEmpty(value) || needles == null)
            {
                return false;
            }

            for (int i = 0; i < needles.Length; i++)
            {
                if (!string.IsNullOrEmpty(needles[i]) && value.Contains(needles[i]))
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetStableChoiceIndex(string seed, int count)
        {
            if (count <= 0)
            {
                return 0;
            }

            unchecked
            {
                int hash = 23;
                string safeSeed = string.IsNullOrEmpty(seed) ? "opening_item_extension" : seed;
                for (int i = 0; i < safeSeed.Length; i++)
                {
                    hash = hash * 31 + safeSeed[i];
                }

                return (hash & int.MaxValue) % count;
            }
        }

        private static Sprite LoadRouteChoiceSprite(DemoReward reward)
        {
            if (reward?.RoutePlan == null)
            {
                return null;
            }

            if (reward.RoutePlan.Nodes.Any(node => ContainsAny(node.Name, "旧矿", "矿", "塌井", "井底", "旧账", "暗室", "符债", "矿灯")))
            {
                return LoadRouteOldMineSprite();
            }

            if (reward.RoutePlan.Nodes.Any(node => node.Type == DemoNodeType.Boss || ContainsAny(node.Name, "雷", "劫", "镜")))
            {
                return LoadRouteThunderMarshSprite();
            }

            if (reward.RoutePlan.Nodes.Any(node => node.Type == DemoNodeType.Shop || node.Type == DemoNodeType.Training))
            {
                if (cachedCloudseaFarSprite == null)
                {
                    cachedCloudseaFarSprite = LoadSpriteResource(SceneCloudseaFarResourcePath, SceneBattleCloudseaResourcePath);
                }

                return cachedCloudseaFarSprite;
            }

            return LoadRouteOldMineSprite() ?? LoadRouteCloudseaSprite();
        }

        private static Sprite LoadRouteOldMineSprite()
        {
            if (cachedRouteOldMineSprite == null)
            {
                cachedRouteOldMineSprite = LoadSpriteResource(SceneBattleOldMineEntryResourcePath, SceneBattleCloudseaResourcePath, SceneCloudseaFarResourcePath);
            }

            return cachedRouteOldMineSprite;
        }

        private static Sprite LoadRouteThunderMarshSprite()
        {
            if (cachedRouteThunderMarshSprite == null)
            {
                cachedRouteThunderMarshSprite = LoadSpriteResource(SceneBattleThunderMarshEntryResourcePath, SceneBattleCloudseaResourcePath, SceneCloudseaFarResourcePath);
            }

            return cachedRouteThunderMarshSprite;
        }

        private static Sprite LoadRouteCloudseaSprite()
        {
            if (cachedBattleCloudseaSprite == null)
            {
                cachedBattleCloudseaSprite = LoadSpriteResource(SceneBattleCloudseaResourcePath, SceneCloudseaFarResourcePath);
            }

            return cachedBattleCloudseaSprite;
        }

        private static Sprite LoadHeaderCloudBandSprite()
        {
            if (cachedHeaderCloudBandSprite == null)
            {
                cachedHeaderCloudBandSprite = LoadSpriteResource(HeaderCloudBandResourcePath);
            }

            return cachedHeaderCloudBandSprite;
        }

        private static Sprite LoadBattleHeaderRibbonSprite()
        {
            if (cachedBattleHeaderRibbonSprite == null)
            {
                cachedBattleHeaderRibbonSprite = LoadSpriteResource(BattleHeaderRibbonResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedBattleHeaderRibbonSprite;
        }

        private static Sprite LoadHomeHeroSprite()
        {
            if (cachedHomeHeroInkSprite == null)
            {
                cachedHomeHeroInkSprite = LoadSpriteResource(HomeHeroInkResourcePath, HomeHeroInkFallbackResourcePath, SceneCloudseaFarResourcePath, SceneBattleCloudseaResourcePath);
            }

            return cachedHomeHeroInkSprite;
        }

        private static Sprite LoadHomePrimaryButtonSprite()
        {
            if (cachedHomePrimaryButtonSprite == null)
            {
                cachedHomePrimaryButtonSprite = LoadSpriteResource(HomePrimaryButtonResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedHomePrimaryButtonSprite;
        }

        private static Sprite LoadHomeSecondaryButtonSprite()
        {
            if (cachedHomeSecondaryButtonSprite == null)
            {
                cachedHomeSecondaryButtonSprite = LoadSpriteResource(HomeSecondaryButtonResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedHomeSecondaryButtonSprite;
        }

        private static Sprite LoadHomeTitleCalligraphySprite()
        {
            if (cachedHomeTitleCalligraphySprite == null)
            {
                cachedHomeTitleCalligraphySprite = LoadSpriteResource(HomeTitleCalligraphyResourcePath);
            }

            return cachedHomeTitleCalligraphySprite;
        }

        private static Sprite LoadHomeLogoSealSprite()
        {
            if (cachedHomeLogoSealSprite == null)
            {
                cachedHomeLogoSealSprite = LoadSpriteResource(HomeLogoSealResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedHomeLogoSealSprite;
        }

        private static Sprite LoadHomeInfoTagSprite()
        {
            if (cachedHomeInfoTagSprite == null)
            {
                cachedHomeInfoTagSprite = LoadSpriteResource(HomeInfoTagResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedHomeInfoTagSprite;
        }

        private static Sprite LoadHomeIconCodexSprite()
        {
            if (cachedHomeIconCodexSprite == null)
            {
                cachedHomeIconCodexSprite = LoadSpriteResource(HomeIconCodexResourcePath);
            }

            return cachedHomeIconCodexSprite;
        }

        private static Sprite LoadHomeIconSettingsSprite()
        {
            if (cachedHomeIconSettingsSprite == null)
            {
                cachedHomeIconSettingsSprite = LoadSpriteResource(HomeIconSettingsResourcePath);
            }

            return cachedHomeIconSettingsSprite;
        }

        private static Sprite LoadHomeIconExitSprite()
        {
            if (cachedHomeIconExitSprite == null)
            {
                cachedHomeIconExitSprite = LoadSpriteResource(HomeIconExitResourcePath);
            }

            return cachedHomeIconExitSprite;
        }

        private static Sprite LoadPanelScrollDarkSprite()
        {
            if (cachedPanelScrollDarkSprite == null)
            {
                cachedPanelScrollDarkSprite = LoadSpriteResource(PanelScrollDarkTransparentResourcePath, PanelScrollDarkResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedPanelScrollDarkSprite;
        }

        private static Sprite LoadBattleIntentPlateSprite()
        {
            if (cachedBattleIntentPlateSprite == null)
            {
                cachedBattleIntentPlateSprite = LoadSpriteResource(BattleIntentPlateResourcePath, PanelScrollDarkTransparentResourcePath, PanelScrollDarkResourcePath);
            }

            return cachedBattleIntentPlateSprite;
        }

        private static Sprite LoadBattleCardFrameSprite()
        {
            if (cachedBattleCardFrameSprite == null)
            {
                cachedBattleCardFrameSprite = LoadSpriteResource(BattleCardFrameResourcePath);
            }

            return cachedBattleCardFrameSprite;
        }

        private static Sprite LoadBattleActionButtonSprite()
        {
            if (cachedBattleActionButtonSprite == null)
            {
                cachedBattleActionButtonSprite = LoadSpriteResource(BattleActionButtonResourcePath);
            }

            return cachedBattleActionButtonSprite;
        }

        private static Sprite LoadTransitionInkWipeSprite()
        {
            if (cachedTransitionInkWipeSprite == null)
            {
                cachedTransitionInkWipeSprite = LoadSpriteResource(TransitionInkWipeResourcePath);
            }

            return cachedTransitionInkWipeSprite;
        }

        private static Sprite LoadTransitionFocusRingSprite()
        {
            if (cachedTransitionFocusRingSprite == null)
            {
                cachedTransitionFocusRingSprite = LoadSpriteResource(TransitionFocusRingResourcePath);
            }

            return cachedTransitionFocusRingSprite;
        }

        private static Sprite LoadRootDestinyDeskSprite()
        {
            if (cachedRootDestinyDeskSprite == null)
            {
                cachedRootDestinyDeskSprite = LoadSpriteResource(RootDestinyDeskResourcePath, HomeHeroInkResourcePath, HomeHeroInkFallbackResourcePath);
            }

            return cachedRootDestinyDeskSprite;
        }

        private static Sprite LoadRootDestinyBackdropSprite()
        {
            if (cachedRootDestinyBackdropSprite == null)
            {
                cachedRootDestinyBackdropSprite = LoadSpriteResource(
                    RootDestinySceneTeaDeskResourcePath,
                    RootDestinyBackdropResourcePath,
                    SceneCloudseaFarResourcePath,
                    RootDestinyDeskResourcePath);
            }

            return cachedRootDestinyBackdropSprite;
        }

        private static Sprite LoadOpeningSceneBackdropSprite()
        {
            if (cachedOpeningSceneBackdropSprite == null)
            {
                cachedOpeningSceneBackdropSprite = LoadSpriteResource(
                    OpeningSceneBackdropResourcePath,
                    RootDestinySceneTeaDeskResourcePath,
                    RootDestinyBackdropResourcePath);
            }

            return cachedOpeningSceneBackdropSprite;
        }

        private static Sprite LoadRootObjectSprite(DemoRootDefinition root)
        {
            string id = (root?.Id ?? string.Empty).Trim().ToLowerInvariant();
            switch (id)
            {
                case "root_servant":
                    if (cachedRootObjectServantSprite == null)
                    {
                        cachedRootObjectServantSprite = LoadSpriteResource(RootObjectServantResourcePath);
                    }

                    return cachedRootObjectServantSprite;
                case "root_smith":
                    if (cachedRootObjectSmithSprite == null)
                    {
                        cachedRootObjectSmithSprite = LoadSpriteResource(RootObjectSmithResourcePath);
                    }

                    return cachedRootObjectSmithSprite;
                case "root_caravan":
                    if (cachedRootObjectCaravanSprite == null)
                    {
                        cachedRootObjectCaravanSprite = LoadSpriteResource(RootObjectCaravanResourcePath);
                    }

                    return cachedRootObjectCaravanSprite;
                case "root_branch":
                    if (cachedRootObjectBranchSprite == null)
                    {
                        cachedRootObjectBranchSprite = LoadSpriteResource(RootObjectBranchResourcePath);
                    }

                    return cachedRootObjectBranchSprite;
                default:
                    if (cachedRootObjectServantSprite == null)
                    {
                        cachedRootObjectServantSprite = LoadSpriteResource(RootObjectServantResourcePath);
                    }

                    return cachedRootObjectServantSprite;
            }
        }

        private static Sprite LoadRootLotTagSprite()
        {
            if (cachedRootLotTagSprite == null)
            {
                cachedRootLotTagSprite = LoadSpriteResource(RootLotTagPaperResourcePath, RootLotTagResourcePath, HomeSecondaryButtonResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedRootLotTagSprite;
        }

        private static Sprite LoadRootConfirmSealSprite()
        {
            if (cachedRootConfirmSealSprite == null)
            {
                cachedRootConfirmSealSprite = LoadSpriteResource(RootConfirmSealCinnabarResourcePath, RootConfirmSealResourcePath, HomePrimaryButtonResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedRootConfirmSealSprite;
        }

        private static Sprite LoadRootSmokeWispSprite()
        {
            if (cachedRootSmokeWispSprite == null)
            {
                cachedRootSmokeWispSprite = LoadSpriteResource(RootSmokeWispSoftResourcePath, RootSmokeWispResourcePath, HeaderCloudBandResourcePath);
            }

            return cachedRootSmokeWispSprite;
        }

        private static Sprite LoadOpeningItemSwordcaseSprite()
        {
            if (cachedOpeningItemSwordcaseSprite == null)
            {
                cachedOpeningItemSwordcaseSprite = LoadSpriteResource(OpeningItemSwordcaseResourcePath, PathChoiceWanjianResourcePath, PathChoiceWanjianFallbackResourcePath);
            }

            return cachedOpeningItemSwordcaseSprite;
        }

        private static Sprite LoadOpeningItemThunderboneSprite()
        {
            if (cachedOpeningItemThunderboneSprite == null)
            {
                cachedOpeningItemThunderboneSprite = LoadSpriteResource(OpeningItemThunderboneResourcePath, PathChoiceThunderResourcePath, PathChoiceThunderFallbackResourcePath);
            }

            return cachedOpeningItemThunderboneSprite;
        }

        private static Sprite LoadOpeningItemBloodjadeSprite()
        {
            if (cachedOpeningItemBloodjadeSprite == null)
            {
                cachedOpeningItemBloodjadeSprite = LoadSpriteResource(OpeningItemBloodjadeResourcePath, PathChoiceBloodResourcePath);
            }

            return cachedOpeningItemBloodjadeSprite;
        }

        private static Sprite LoadOpeningItemProtectiveJadeSprite()
        {
            if (cachedOpeningItemProtectiveJadeSprite == null)
            {
                cachedOpeningItemProtectiveJadeSprite = LoadSpriteResource(OpeningItemProtectiveJadeResourcePath, OpeningItemSwordcaseResourcePath, PathChoiceWanjianResourcePath);
            }

            return cachedOpeningItemProtectiveJadeSprite;
        }

        private static Sprite LoadOpeningItemEmberTalismanCaseSprite()
        {
            if (cachedOpeningItemEmberTalismanCaseSprite == null)
            {
                cachedOpeningItemEmberTalismanCaseSprite = LoadSpriteResource(OpeningItemEmberTalismanCaseResourcePath, OpeningItemBloodjadeResourcePath, PathChoiceBloodResourcePath);
            }

            return cachedOpeningItemEmberTalismanCaseSprite;
        }

        private static Sprite LoadOpeningItemAncestralCasketSprite()
        {
            if (cachedOpeningItemAncestralCasketSprite == null)
            {
                cachedOpeningItemAncestralCasketSprite = LoadSpriteResource(OpeningItemAncestralCasketResourcePath, OpeningItemSwordcaseResourcePath, PathChoiceWanjianResourcePath);
            }

            return cachedOpeningItemAncestralCasketSprite;
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
            if (reward.Type == DemoRewardType.Root
                || (reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel)
                || reward.Type == DemoRewardType.OpeningScene)
            {
                return BuildOpeningPreviewText(reward);
            }

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

        private static string BuildOpeningPreviewText(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                string unlockText = string.IsNullOrEmpty(reward.Root.UnlockCondition) ? "默认在起始根脚池中出现" : reward.Root.UnlockCondition;
                return $"{BuildCompactLine(reward.Root.Summary)}\n解锁：{unlockText}";
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                DemoJourneyLineDefinition line = reward.JourneyLine;
                return $"{line.CarryItemName}\n{BuildCompactLine(TrimSentence(line.CarryItemEffect, 18))}";
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                return $"{GetRegionRiskLabel(reward.Region)}\n{BuildOpeningSceneTagline(reward.Region)}";
            }

            return string.IsNullOrEmpty(reward.Description) ? "开局信息待补充。" : BuildCompactLine(reward.Description);
        }

        private static string BuildRootIdentityText(DemoRootDefinition root)
        {
            string id = (root?.Id ?? string.Empty).Trim().ToLowerInvariant();
            switch (id)
            {
                case "root_servant":
                    return "你在山门最不起眼的地方长大，记住的是规矩、差事和别人不愿做的活。";
                case "root_smith":
                    return "你闻着炉火和铁腥长大，知道许多兵刃的名字，也知道它们为何会断。";
                case "root_caravan":
                    return "你跟着账册和商路长大，最早懂得路上的风声比货价更贵。";
                case "root_branch":
                    return "你熟悉祠堂的规矩，也熟悉规矩背后那些不能写进族谱的旧债。";
                default:
                    return root == null || string.IsNullOrEmpty(root.Summary)
                        ? "此身来处尚未记入命簿。"
                        : BuildCompactLine(root.Summary);
            }
        }

        private static string BuildRootEffectText(DemoRootDefinition root)
        {
            string id = (root?.Id ?? string.Empty).Trim().ToLowerInvariant();
            switch (id)
            {
                case "root_servant":
                    return "此身余荫：首层修炼节点权重 +6%";
                case "root_smith":
                    return "此身余荫：首次法器候选额外 +1 件";
                case "root_caravan":
                    return "此身余荫：初始灵石 +20";
                case "root_branch":
                    return "此身余荫：首次功法候选额外 +1 项";
                default:
                    return string.IsNullOrEmpty(root?.Summary) ? "此身余荫：旧缘未显" : "此身余荫：" + root.Summary;
            }
        }

        private static string BuildRootLotHint(DemoRootDefinition root)
        {
            string id = (root?.Id ?? string.Empty).Trim().ToLowerInvariant();
            switch (id)
            {
                case "root_servant":
                    return "规矩与差事";
                case "root_smith":
                    return "炉火与断锋";
                case "root_caravan":
                    return "账册与商路";
                case "root_branch":
                    return "族谱与旧库";
                default:
                    return string.IsNullOrEmpty(root?.Summary) ? "命签未明" : TrimSentence(root.Summary, 8);
            }
        }

        private static bool IsRootClaimable(DemoReward reward)
        {
            string id = (reward?.Root?.Id ?? string.Empty).Trim().ToLowerInvariant();
            return id == "root_branch" || id == "fallback_root_branch";
        }

        private void AddRewardHoverEvents(GameObject rewardObject, DemoReward reward)
        {
            if (reward == null)
            {
                return;
            }

            if (reward.Type == DemoRewardType.Root
                || (reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel)
                || reward.Type == DemoRewardType.OpeningScene
                || reward.Type == DemoRewardType.Route)
            {
                return;
            }

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
                lines.Add($"路数：{(string.IsNullOrEmpty(reward.RouteTag) ? "前路" : reward.RouteTag)}");
                lines.Add($"节奏：{BuildRoutePaceTag(reward)}");
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
            else if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                lines.Add($"身份：{reward.Root.Name}");
                lines.Add($"解锁：{(string.IsNullOrEmpty(reward.Root.UnlockCondition) ? "默认开放" : reward.Root.UnlockCondition)}");
                lines.Add(string.Empty);
                lines.Add(reward.Root.Summary);
            }
            else if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                DemoJourneyLineDefinition line = reward.JourneyLine;
                lines.Add($"根脚：{(controller.Run.OpeningSelection.Root != null ? controller.Run.OpeningSelection.Root.Name : "当前根脚")}");
                lines.Add($"所携：{line.CarryItemName}");
                lines.Add($"候选：{BuildJourneyRegionName(line)}");
                lines.Add($"起势：{GetJourneyRiskLabel(line)}");
                if (!string.IsNullOrEmpty(line.CarryItemEffect))
                {
                    lines.Add($"效果：{line.CarryItemEffect}");
                }

                if (line.SummaryTags != null && line.SummaryTags.Count > 0)
                {
                    lines.Add($"标签：{string.Join(" / ", line.SummaryTags.Take(3))}");
                }

                lines.Add(string.Empty);
                lines.Add(line.OriginText);
            }
            else if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                lines.Add($"场景：{reward.Region.Name}");
                lines.Add($"险势：{GetRegionRiskLabel(reward.Region)}");
                lines.Add(string.Empty);
                lines.Add(reward.Region.Description);
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
            return $"{BuildRoutePaceTag(reward)}\n{BuildRouteNodePreview(reward.RoutePlan, 4)}";
        }

        private static string BuildRouteDecisionText(DemoReward reward)
        {
            if (reward?.RoutePlan == null)
            {
                return "先确认下一段历练，再让当前构筑进入下一次检验。";
            }

            string pressure = BuildRoutePressureHint(reward);
            string payoff = BuildRoutePayoffHint(reward);
            return $"{pressure}\n{payoff}";
        }

        private static string BuildRoutePressureHint(DemoReward reward)
        {
            DemoMapRoutePlan routePlan = reward.RoutePlan;
            int battleCount = CountRouteBattleNodes(routePlan);
            bool hasBoss = routePlan.Nodes.Any(node => node.Type == DemoNodeType.Boss);
            bool startsWithSupport = routePlan.Nodes.Count > 0 && IsRouteSupportNode(routePlan.Nodes[0]);

            if (hasBoss && startsWithSupport)
            {
                return "先补后劫：把续航和神通窗口留到 Boss 前。";
            }

            if (hasBoss)
            {
                return "直面剑炉：少一层缓冲，换更快抵达镇矿剑傀。";
            }

            if (battleCount >= 2)
            {
                return "连战压线：更早检验输出循环。";
            }

            if (startsWithSupport)
            {
                return "先修后战：用补强换中段稳定。";
            }

            return "稳步推进：先打一场，再根据奖励转向。";
        }

        private static string BuildRoutePayoffHint(DemoReward reward)
        {
            DemoMapRoutePlan routePlan = reward.RoutePlan;

            if (routePlan.Nodes.Any(node => node.Type == DemoNodeType.Training))
            {
                return "收益重点：功法 / 神通 / 长线规则。";
            }

            if (routePlan.Nodes.Any(node => node.Type == DemoNodeType.Shop))
            {
                return "收益重点：法器、续航和 Boss 前整备。";
            }

            if (routePlan.Nodes.Any(node => node.Type == DemoNodeType.Reward))
            {
                return "收益重点：卡牌与构筑组件更快进池。";
            }

            if (routePlan.Nodes.Any(node => node.Type == DemoNodeType.Boss))
            {
                return "收益重点：直接验证这一局是否成型。";
            }

            return "收益重点：把当前节奏推进到下一层。";
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

        private static string BuildRouteCountSummary(DemoMapRoutePlan routePlan)
        {
            if (routePlan == null || routePlan.Nodes.Count == 0)
            {
                return "下一段历练";
            }

            int battleCount = CountRouteBattleNodes(routePlan);
            int supportCount = routePlan.Nodes.Count(IsRouteSupportNode);
            bool hasBoss = routePlan.Nodes.Any(node => node.Type == DemoNodeType.Boss);

            string bossText = hasBoss ? " · 含 Boss" : string.Empty;
            return $"战斗 {battleCount} · 补强 {supportCount}{bossText}";
        }

        private static int CountRouteBattleNodes(DemoMapRoutePlan routePlan)
        {
            return routePlan == null
                ? 0
                : routePlan.Nodes.Count(node => node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss);
        }

        private static bool IsRouteSupportNode(DemoMapNode node)
        {
            return node != null
                && (node.Type == DemoNodeType.Reward
                    || node.Type == DemoNodeType.Shop
                    || node.Type == DemoNodeType.Training);
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
                case DemoNodeType.Result:
                    return "结算";
                default:
                    return "节点";
            }
        }

        private static Color GetRouteNodeColor(DemoMapNode node, Color fallback)
        {
            if (node == null)
            {
                return fallback;
            }

            switch (node.Type)
            {
                case DemoNodeType.Battle:
                    return new Color(0.58f, 0.70f, 0.82f, 0.95f);
                case DemoNodeType.Reward:
                    return new Color(0.84f, 0.70f, 0.42f, 0.95f);
                case DemoNodeType.Training:
                    return new Color(0.48f, 0.70f, 0.62f, 0.95f);
                case DemoNodeType.Shop:
                    return new Color(0.78f, 0.64f, 0.42f, 0.95f);
                case DemoNodeType.Boss:
                    return new Color(0.82f, 0.45f, 0.42f, 0.96f);
                case DemoNodeType.RouteChoice:
                    return new Color(0.62f, 0.66f, 0.72f, 0.92f);
                case DemoNodeType.Victory:
                case DemoNodeType.Result:
                    return new Color(0.90f, 0.78f, 0.48f, 0.95f);
                default:
                    return fallback;
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
                    return "先走一段更适合铺开飞剑的历练";
                case DemoSwordStyle.Thunder:
                    return "先把节奏压紧，让前两层更容易打出爆发";
                case DemoSwordStyle.Blood:
                    return "先换一段更险的路，把斩杀窗口提早养起来";
                default:
                    return "先定接下来怎么走，再决定后面怎么补。";
            }
        }

        private static string[] GetPathChoiceKeywords(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new[] { "先战斗再拿飞剑补件", "更适合把飞剑数量尽快铺起来", "后面更容易接上剑潮式滚场" };
                case DemoSwordStyle.Thunder:
                    return new[] { "先吃高压战斗换爆发组件", "更早点亮感电与雷击连锁", "适合把强节奏压在同一轮里" };
                case DemoSwordStyle.Blood:
                    return new[] { "先搏命再拿流血补件", "前两层更容易养出斩杀压力", "适合用更险的节奏换上限" };
                default:
                    return new[] { "下一段路线", "节奏差异", "后续补强" };
            }
        }

        private static string BuildPathChoiceSummary(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "这条路更像是先把飞剑底子垫起来，后面再让演武自己滚大。";
                case DemoSwordStyle.Thunder:
                    return "这条路会更早把压力推高，换来更快进入感电爆发的机会。";
                case DemoSwordStyle.Blood:
                    return "这条路更愿意先吃风险，把后面的流血与斩杀窗口提前养出来。";
                default:
                    return "选定后，后面的节点节奏和补强重点都会跟着变。";
            }
        }

        private static string GetPathChoiceStyleTitle(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "铺势";
                case DemoSwordStyle.Thunder:
                    return "压势";
                case DemoSwordStyle.Blood:
                    return "藏锋";
                default:
                    return "观势";
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

        private Font GetHomeTitleFont()
        {
            if (cachedHomeTitleFont != null)
            {
                return cachedHomeTitleFont;
            }

            try
            {
                cachedHomeTitleFont = Font.CreateDynamicFontFromOSFont(HomeTitleFontCandidates, 96);
            }
            catch
            {
                cachedHomeTitleFont = null;
            }

            return cachedHomeTitleFont != null ? cachedHomeTitleFont : uiFont;
        }

        private void HideFrameDecorations(RectTransform root)
        {
            if (root == null)
            {
                return;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                string childName = child.name;
                if (childName.StartsWith("Border")
                    || childName.StartsWith("CornerH")
                    || childName.StartsWith("CornerV")
                    || childName == "InnerWash"
                    || childName == "ButtonGlow")
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        private void ApplyHomeButtonSurface(RectTransform root, bool primary)
        {
            if (root == null)
            {
                return;
            }

            Image image = root.GetComponent<Image>();
            if (image != null)
            {
                image.sprite = null;
                image.type = Image.Type.Simple;
                image.preserveAspect = false;
                image.color = primary ? HomeButtonPrimaryBase : HomeButtonSecondaryBase;
            }

            Color borderColor = primary ? HomeButtonPrimaryEdge : HomeButtonSecondaryEdge;
            DecorateFrame(root, borderColor, new Color(0f, 0f, 0f, 0f), primary ? 1.2f : 0.8f, false);
        }

        private void AddHomeButtonSoftEdge(RectTransform root, bool primary)
        {
            if (root == null)
            {
                return;
            }

            Color lowerLineColor = primary
                ? new Color(0.45f, 0.31f, 0.12f, 0.24f)
                : new Color(0.25f, 0.28f, 0.24f, 0.11f);

            RectTransform lowerLine = CreateStretchPanel(
                root,
                "HomeButtonLowerThread",
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(primary ? 42f : 36f, primary ? 17f : 13f),
                new Vector2(primary ? -42f : -36f, primary ? 18.2f : 14f),
                lowerLineColor);
            lowerLine.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(lowerLine.gameObject);

            RectTransform sealTick = CreateFixedPanel(
                root,
                "HomeButtonSealTick",
                new Vector2(0f, 0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(primary ? 20f : 18f, 0f),
                new Vector2(3f, primary ? 28f : 20f),
                primary ? new Color(0.56f, 0.17f, 0.10f, 0.32f) : new Color(0.38f, 0.13f, 0.09f, 0.13f));
            sealTick.GetComponent<Image>().raycastTarget = false;
            IgnoreLayout(sealTick.gameObject);
        }

        private static ColorBlock CreateHomeButtonColors(Color baseColor)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1.00f, 0.97f, 0.86f, 1f);
            colors.pressedColor = new Color(0.88f, 0.82f, 0.68f, 1f);
            colors.selectedColor = new Color(0.98f, 0.94f, 0.82f, 1f);
            colors.disabledColor = new Color(baseColor.r, baseColor.g, baseColor.b, 0.34f);
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.10f;
            return colors;
        }

        private static ColorBlock CreateSpriteButtonColors(Color disabledColor)
        {
            ColorBlock colors = ColorBlock.defaultColorBlock;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.96f, 0.82f, 1f);
            colors.pressedColor = new Color(0.80f, 0.72f, 0.56f, 1f);
            colors.selectedColor = new Color(0.94f, 0.86f, 0.68f, 1f);
            colors.disabledColor = disabledColor;
            colors.colorMultiplier = 1f;
            colors.fadeDuration = 0.08f;
            return colors;
        }

        private static void ApplySpriteToImage(RectTransform rect, Sprite sprite, Color color, bool preserveAspect = false)
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
            image.preserveAspect = preserveAspect;
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

        private static string GetBuildApproachLabel(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "铺势";
                case DemoSwordStyle.Thunder:
                    return "压势";
                case DemoSwordStyle.Blood:
                    return "藏锋";
                default:
                    return "未定";
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
                case DemoRewardType.Root:
                    return "根脚";
                case DemoRewardType.Trace:
                    return "道痕";
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return "启程";
                case DemoRewardType.OpeningScene:
                    return "所往";
                case DemoRewardType.Route:
                    return "路线";
                case DemoRewardType.Card:
                    return "新牌";
                case DemoRewardType.Gongfa:
                    return "功法";
                case DemoRewardType.Artifact:
                    return "法器";
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
                case DemoRewardType.Root:
                    return reward.Root != null ? GetRootRarityLabel(reward.Root.Rarity) : "开局身份";
                case DemoRewardType.Trace:
                    return "传承选择";
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return reward.JourneyLine != null ? "所携之物" : "所携";
                case DemoRewardType.OpeningScene:
                    return reward.Region != null ? "先去此境" : "所往";
                case DemoRewardType.Route:
                    return string.IsNullOrEmpty(reward.RouteTag) ? "前路" : reward.RouteTag;
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
                case DemoRewardType.Root:
                    return "开局底色";
                case DemoRewardType.Trace:
                    return "不继承战力";
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return "所携起势";
                case DemoRewardType.OpeningScene:
                    return "场景落点";
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
                case DemoRewardType.Root:
                    return reward.Root != null ? GetRootQuality(reward.Root.Rarity) : DemoQuality.Mortal;
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return reward.JourneyLine != null ? GetJourneyQuality(reward.JourneyLine.RiskLevel) : DemoQuality.Spirit;
                case DemoRewardType.OpeningScene:
                    return reward.JourneyLine != null ? GetJourneyQuality(reward.JourneyLine.RiskLevel) : DemoQuality.Spirit;
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
                case DemoRewardType.Root:
                    return "命";
                case DemoRewardType.Trace:
                    return "痕";
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return "物";
                case DemoRewardType.OpeningScene:
                    return "境";
                case DemoRewardType.Route:
                    return "路";
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
                case DemoRewardType.Root:
                    return new Color(0.88f, 0.72f, 0.46f, 1f);
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return GetJourneyAccentColor(reward.JourneyLine);
                case DemoRewardType.OpeningScene:
                    return GetRegionAccentColor(reward.Region, reward.JourneyLine);
                case DemoRewardType.Route:
                    return GetRouteAccentColor(reward);
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
                case DemoRewardType.Root:
                    return new Color(0.17f, 0.13f, 0.09f, 0.98f);
                case DemoRewardType.Journey:
                case DemoRewardType.Vessel:
                    return GetJourneyPanelColor(reward.JourneyLine);
                case DemoRewardType.OpeningScene:
                    return GetRegionPanelColor(reward.Region, reward.JourneyLine);
                case DemoRewardType.Route:
                {
                    Color routeColor = GetRouteAccentColor(reward);
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

        private static string BuildOpeningHookText(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Trace)
            {
                return string.IsNullOrEmpty(reward.Description)
                    ? "只改变这一世的首战选择，不带回上一世战力。"
                    : reward.Description;
            }

            if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                return reward.Root.Summary;
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                return TrimSentence(reward.JourneyLine.OriginText, 34);
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                return $"{GetRegionRiskLabel(reward.Region)} · {BuildOpeningSceneTagline(reward.Region)}";
            }

            return "开局方向待定。";
        }

        private static string GetOpeningMetaChipText(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Trace)
            {
                return string.IsNullOrEmpty(reward.TraceId) ? "保持原序" : "首战重铸 1 次";
            }

            if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                return reward.Root.IsDefaultPool ? "默认根脚池" : "后续解锁";
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                return reward.JourneyLine.CarryItemName;
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                return GetRegionRiskLabel(reward.Region);
            }

            return string.Empty;
        }

        private static string[] GetOpeningKeywords(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Trace)
            {
                return string.IsNullOrEmpty(reward.TraceId)
                    ? new[] { "首战所得保持原序", "不追加局外战力", "仍可正常触发后续保底" }
                    : new[] { "首战所得可重铸一次", "重铸后仍是 P0 三槽", "不继承攻击与生命" };
            }

            if (reward.Type == DemoRewardType.Root && reward.Root != null)
            {
                return new[]
                {
                    $"身份：{reward.Root.Name}",
                    $"稀有度：{GetRootRarityLabel(reward.Root.Rarity)}",
                    reward.Root.IsDefaultPool ? "默认在起始根脚池出现" : $"需满足 {reward.Root.UnlockCondition}"
                };
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel) && reward.JourneyLine != null)
            {
                DemoJourneyLineDefinition line = reward.JourneyLine;
                List<string> keywords = new List<string>
                {
                    $"所携：{line.CarryItemName}",
                    $"缘由：{TrimSentence(line.Title, 12)}",
                    $"起势：{GetJourneyRiskLabel(line)}"
                };

                if (!string.IsNullOrEmpty(line.CarryItemEffect))
                {
                    keywords[1] = $"所携：{line.CarryItemName} · {line.CarryItemEffect}";
                }

                return keywords.ToArray();
            }

            if (reward.Type == DemoRewardType.OpeningScene && reward.Region != null)
            {
                return new[]
                {
                    reward.Region.Name,
                    GetRegionRiskLabel(reward.Region),
                    BuildOpeningSceneTagline(reward.Region)
                };
            }

            return new[] { "开局信息", "待补充" };
        }

        private static string[] GetOpeningItemTags(DemoJourneyLineDefinition line)
        {
            List<string> labels = new List<string>
            {
                GetJourneyRiskLabel(line)
            };

            if (line?.SummaryTags != null)
            {
                foreach (string tag in line.SummaryTags)
                {
                    string label = GetRewardTagLabel(tag);
                    if (!string.IsNullOrEmpty(label) && !labels.Contains(label))
                    {
                        labels.Add(label);
                    }

                    if (labels.Count >= 3)
                    {
                        break;
                    }
                }
            }

            while (labels.Count < 3)
            {
                labels.Add(labels.Count == 1 ? "起势" : "线索");
            }

            return labels.Take(3).ToArray();
        }

        private static string BuildOpeningFooterHint(DemoReward reward)
        {
            if (reward.Type == DemoRewardType.Root)
            {
                return "这一层先定身份，下一层再定为何出山";
            }

            if ((reward.Type == DemoRewardType.Journey || reward.Type == DemoRewardType.Vessel))
            {
                return "定下所携，再看所往";
            }

            if (reward.Type == DemoRewardType.OpeningScene)
            {
                return "踏入此境";
            }

            return "开局信息";
        }

        private static string BuildRouteChoiceHint(DemoReward reward)
        {
            return TrimSentence(reward.Description, 28);
        }

        private static string BuildRoutePaceTag(DemoReward reward)
        {
            if (reward?.RoutePlan == null)
            {
                return "下一段历练";
            }

            int battleCount = reward.RoutePlan.Nodes.Count(node => node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss);
            int supportCount = reward.RoutePlan.Nodes.Count(node => node.Type == DemoNodeType.Reward || node.Type == DemoNodeType.Shop || node.Type == DemoNodeType.Training);

            if (battleCount >= supportCount + 1)
            {
                return "先压战斗";
            }

            if (supportCount > battleCount)
            {
                return "先补后进";
            }

            return "稳步推进";
        }

        private void BuildRouteNodeSequence(RectTransform parent, DemoReward reward)
        {
            if (parent == null || reward?.RoutePlan == null)
            {
                return;
            }

            int count = Mathf.Min(4, reward.RoutePlan.Nodes.Count);
            float slotWidth = 72f;
            float gap = 18f;
            float totalWidth = count * slotWidth + Mathf.Max(0, count - 1) * gap;
            float left = Mathf.Max(12f, (380f - totalWidth) * 0.5f);

            for (int i = 0; i < count; i++)
            {
                DemoMapNode node = reward.RoutePlan.Nodes[i];
                Color nodeColor = GetRouteNodeColor(node, GetRouteAccentColor(reward));
                float x = left + i * (slotWidth + gap);
                RectTransform chip = CreateFixedPanel(
                    parent,
                    "RouteNodeChip_" + i,
                    new Vector2(0f, 1f),
                    new Vector2(0f, 1f),
                    new Vector2(x, -14f),
                    new Vector2(slotWidth, 76f),
                    new Color(0.96f, 0.92f, 0.82f, 0.94f));
                DecorateFrame(chip, new Color(nodeColor.r, nodeColor.g, nodeColor.b, 0.68f), new Color(nodeColor.r, nodeColor.g, nodeColor.b, 0.06f), 1f, false);

                Text indexText = CreateText(chip, "RouteNodeIndex_" + i, (i + 1).ToString("00"), 10, FontStyle.Bold, TextAnchor.UpperLeft, new Color(nodeColor.r * 0.76f, nodeColor.g * 0.76f, nodeColor.b * 0.76f, 0.92f));
                indexText.rectTransform.anchorMin = new Vector2(0f, 1f);
                indexText.rectTransform.anchorMax = new Vector2(1f, 1f);
                indexText.rectTransform.offsetMin = new Vector2(7f, -18f);
                indexText.rectTransform.offsetMax = new Vector2(-7f, -4f);

                Text typeText = CreateText(chip, "RouteNodeType_" + i, GetRouteNodeShortLabel(node), 12, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.22f, 0.18f, 0.12f, 0.96f));
                typeText.rectTransform.anchorMin = new Vector2(0f, 0.40f);
                typeText.rectTransform.anchorMax = new Vector2(1f, 0.78f);
                typeText.rectTransform.offsetMin = new Vector2(4f, 0f);
                typeText.rectTransform.offsetMax = new Vector2(-4f, 0f);

                Text nameText = CreateText(chip, "RouteNodeName_" + i, TrimSentence(node.Name, 5), 10, FontStyle.Normal, TextAnchor.LowerCenter, new Color(0.40f, 0.37f, 0.30f, 0.88f));
                nameText.rectTransform.anchorMin = Vector2.zero;
                nameText.rectTransform.anchorMax = new Vector2(1f, 0.38f);
                nameText.rectTransform.offsetMin = new Vector2(4f, 5f);
                nameText.rectTransform.offsetMax = new Vector2(-4f, 0f);

                if (i < count - 1)
                {
                    RectTransform line = CreateFixedPanel(
                        parent,
                        "RouteNodeLink_" + i,
                        new Vector2(0f, 1f),
                        new Vector2(0f, 1f),
                        new Vector2(x + slotWidth + 3f, -52f),
                        new Vector2(gap - 6f, 2f),
                        new Color(0.48f, 0.34f, 0.17f, 0.54f));
                    line.GetComponent<Image>().raycastTarget = false;
                    IgnoreLayout(line.gameObject);
                }
            }

            Text detail = CreateText(parent, "RouteNodeHint", BuildRouteCountSummary(reward.RoutePlan), 12, FontStyle.Bold, TextAnchor.LowerCenter, new Color(0.39f, 0.29f, 0.17f, 0.88f));
            detail.rectTransform.anchorMin = Vector2.zero;
            detail.rectTransform.anchorMax = new Vector2(1f, 0f);
            detail.rectTransform.offsetMin = new Vector2(12f, 7f);
            detail.rectTransform.offsetMax = new Vector2(-12f, 27f);
        }

        private static string TrimSentence(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
            {
                return string.IsNullOrEmpty(text) ? string.Empty : text;
            }

            return text.Substring(0, Mathf.Max(0, maxLength - 1)) + "…";
        }

        private static string GetRegionFocusLabel(DemoRegionDefinition region)
        {
            if (region == null || string.IsNullOrEmpty(region.RewardFocus))
            {
                return "去处气口";
            }

            string[] split = region.RewardFocus.Split('|');
            return split.Length > 0 ? split[0].Trim() : region.RewardFocus;
        }

        private static string GetRegionRiskLabel(DemoRegionDefinition region)
        {
            string regionId = (region?.Id ?? string.Empty).ToLowerInvariant();
            if (regionId.Contains("thunder") || regionId.Contains("demon") || regionId.Contains("tower"))
            {
                return "高险";
            }

            if (regionId.Contains("herb") || regionId.Contains("trade"))
            {
                return "低险";
            }

            return "中险";
        }

        private static string BuildOpeningSceneTagline(DemoRegionDefinition region)
        {
            string regionId = (region?.Id ?? string.Empty).ToLowerInvariant();
            if (regionId.Contains("old_mine"))
            {
                return "旧契与铁屑埋在地底。";
            }

            if (regionId.Contains("herb") || regionId.Contains("forest"))
            {
                return "药香藏在瘴雾深处。";
            }

            if (regionId.Contains("trade"))
            {
                return "荒路尽头仍有人烟与债账。";
            }

            if (regionId.Contains("thunder"))
            {
                return "天雷未散，焦土仍在发亮。";
            }

            if (regionId.Contains("ancestral") || regionId.Contains("vault"))
            {
                return "旧库闭门，匣中微声相应。";
            }

            if (regionId.Contains("demon") || regionId.Contains("tower"))
            {
                return "塔影压云，封禁仍未松动。";
            }

            return TrimSentence(region?.Description ?? "此境仍在雾中。", 18);
        }

        private static Color GetRegionAccentColor(DemoRegionDefinition region, DemoJourneyLineDefinition line)
        {
            if (region?.Id != null)
            {
                if (region.Id.Contains("old_mine"))
                {
                    return new Color(0.72f, 0.43f, 0.27f, 1f);
                }

                if (region.Id.Contains("thunder"))
                {
                    return new Color(0.39f, 0.67f, 0.84f, 1f);
                }

                if (region.Id.Contains("herb") || region.Id.Contains("forest"))
                {
                    return new Color(0.53f, 0.72f, 0.55f, 1f);
                }

                if (region.Id.Contains("trade"))
                {
                    return new Color(0.77f, 0.66f, 0.42f, 1f);
                }

                if (region.Id.Contains("ancestral") || region.Id.Contains("vault"))
                {
                    return new Color(0.56f, 0.70f, 0.68f, 1f);
                }

                if (region.Id.Contains("demon") || region.Id.Contains("tower"))
                {
                    return new Color(0.56f, 0.62f, 0.80f, 1f);
                }
            }

            return GetJourneyAccentColor(line);
        }

        private static Color GetRegionPanelColor(DemoRegionDefinition region, DemoJourneyLineDefinition line)
        {
            Color accent = GetRegionAccentColor(region, line);
            return new Color(
                Mathf.Lerp(0.68f, accent.r, 0.18f),
                Mathf.Lerp(0.62f, accent.g, 0.16f),
                Mathf.Lerp(0.50f, accent.b, 0.12f),
                0.94f);
        }

        private static Color GetRouteAccentColor(DemoReward reward)
        {
            if (reward?.RoutePlan == null)
            {
                return ColorMist;
            }

            bool hasTraining = reward.RoutePlan.Nodes.Any(node => node.Type == DemoNodeType.Training);
            bool hasShop = reward.RoutePlan.Nodes.Any(node => node.Type == DemoNodeType.Shop);
            bool heavyBattle = reward.RoutePlan.Nodes.Count(node => node.Type == DemoNodeType.Battle || node.Type == DemoNodeType.Boss) >= 2;

            if (heavyBattle)
            {
                return new Color(0.46f, 0.63f, 0.79f, 1f);
            }

            if (hasTraining || hasShop)
            {
                return new Color(0.72f, 0.64f, 0.43f, 1f);
            }

            return new Color(0.58f, 0.66f, 0.58f, 1f);
        }

        private static string GetNodeTypeLabel(string nodeType)
        {
            switch ((nodeType ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "normal_battle":
                    return "寻常战斗";
                case "elite_battle":
                    return "精英战斗";
                case "event":
                    return "奇遇";
                case "shop":
                    return "整备";
                case "training":
                    return "修炼";
                case "cave":
                    return "洞府";
                case "treasure":
                    return "藏宝";
                default:
                    return string.IsNullOrEmpty(nodeType) ? "节点" : nodeType;
            }
        }

        private static string FormatSignedPercent(int deltaPercent)
        {
            return deltaPercent >= 0 ? $"+{deltaPercent}%" : $"{deltaPercent}%";
        }

        private static string GetRewardTagLabel(string tagId)
        {
            switch ((tagId ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "shock":
                    return "感电";
                case "thunder_strike":
                    return "雷击";
                case "crit":
                    return "暴击";
                case "flying_sword":
                    return "飞剑";
                case "sword_intent":
                    return "剑意";
                case "recovery":
                    return "恢复";
                case "alchemy":
                    return "丹炼";
                case "refinement":
                    return "炼化";
                case "spell":
                    return "术法";
                case "seal":
                    return "符印";
                case "thunder":
                    return "雷法";
                case "defense":
                    return "防御";
                case "resource":
                    return "资源";
                case "stable":
                    return "稳进";
                case "treasure":
                    return "秘藏";
                case "event":
                    return "奇遇";
                case "upgrade":
                    return "强化";
                case "follow_up":
                    return "追击";
                case "artifact":
                    return "法器";
                case "high_variance":
                case "high_variance_reward":
                    return "高波动";
                case "trade_event":
                    return "商路奇遇";
                case "high_quality_reward":
                    return "高品质奖励";
                case "strange_encounter":
                    return "异遇";
                case "gongfa":
                    return "功法";
                case "status_transition":
                    return "状态转化";
                case "bleed":
                    return "流血";
                case "risk_reward":
                    return "以险换利";
                case "attack":
                    return "攻击";
                case "high_pressure_trial":
                    return "高压试炼";
                case "survival_to_damage":
                    return "残血爆发";
                default:
                    return string.IsNullOrEmpty(tagId) ? "奖励" : tagId;
            }
        }

        private static string BuildJourneyRegionName(DemoJourneyLineDefinition line)
        {
            if (line == null)
            {
                return "未知所往";
            }

            List<string> names = new List<string>();
            if (line.RegionCandidateIds != null)
            {
                foreach (string regionId in line.RegionCandidateIds)
                {
                    if (names.Count >= 3)
                    {
                        break;
                    }

                    if (DemoConfigRepository.TryGetRegion(regionId, out DemoRegionDefinition region) && !names.Contains(region.Name))
                    {
                        names.Add(region.Name);
                    }
                }
            }

            if (names.Count == 0
                && !string.IsNullOrEmpty(line.FirstRegionId)
                && DemoConfigRepository.TryGetRegion(line.FirstRegionId, out DemoRegionDefinition fallbackRegion))
            {
                names.Add(fallbackRegion.Name);
            }

            return names.Count > 0 ? string.Join(" / ", names) : "未知所往";
        }

        private static string GetJourneyRiskLabel(DemoJourneyLineDefinition line)
        {
            return GetJourneyRiskLabel(line?.RiskLevel);
        }

        private static string GetJourneyRiskLabel(string riskLevel)
        {
            switch ((riskLevel ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "stable":
                    return "稳健";
                case "medium":
                    return "中压";
                case "risky":
                    return "高压";
                case "extreme":
                    return "极险";
                default:
                    return "起势";
            }
        }

        private static string GetRootRarityLabel(string rarity)
        {
            switch ((rarity ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "rare":
                    return "稀有";
                case "epic":
                    return "奇珍";
                case "legendary":
                    return "天命";
                default:
                    return "常见";
            }
        }

        private static DemoQuality GetRootQuality(string rarity)
        {
            switch ((rarity ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "rare":
                    return DemoQuality.Heaven;
                case "epic":
                    return DemoQuality.Immortal;
                default:
                    return DemoQuality.Earth;
            }
        }

        private static DemoQuality GetJourneyQuality(string riskLevel)
        {
            switch ((riskLevel ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "risky":
                    return DemoQuality.Heaven;
                case "extreme":
                    return DemoQuality.Immortal;
                case "medium":
                    return DemoQuality.Mysterious;
                default:
                    return DemoQuality.Spirit;
            }
        }

        private static Color GetJourneyAccentColor(DemoJourneyLineDefinition line)
        {
            if (line?.SummaryTags != null)
            {
                if (line.SummaryTags.Any(tag => tag == "thunder" || tag == "shock"))
                {
                    return new Color(0.47f, 0.69f, 0.96f, 1f);
                }

                if (line.SummaryTags.Any(tag => tag == "bleed" || tag == "risk_reward"))
                {
                    return new Color(0.86f, 0.43f, 0.42f, 1f);
                }
            }

            return new Color(0.84f, 0.70f, 0.42f, 1f);
        }

        private static Color GetJourneyPanelColor(DemoJourneyLineDefinition line)
        {
            Color accent = GetJourneyAccentColor(line);
            return new Color(
                Mathf.Lerp(0.10f, accent.r * 0.24f, 0.65f),
                Mathf.Lerp(0.11f, accent.g * 0.22f, 0.65f),
                Mathf.Lerp(0.13f, accent.b * 0.20f, 0.65f),
                0.98f);
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

        private sealed class DemoOpeningSceneCardFx : MonoBehaviour
        {
            private RectTransform cardRect;
            private Image cardImage;
            private Image hoverWashImage;
            private Image actionImage;
            private Text actionText;
            private Image accentLineImage;
            private Image affinitySealImage;
            private Color accent = ColorGold;
            private Vector3 baseScale = Vector3.one;
            private bool hover;
            private bool pressed;
            private bool primary;
            private bool claimable;
            private float pressHoldTimer;
            private float pulsePhase;

            public void Configure(
                RectTransform cardRect,
                Image cardImage,
                Image hoverWashImage,
                Image actionImage,
                Text actionText,
                Image accentLineImage,
                Image affinitySealImage,
                Color accent,
                bool primary,
                bool claimable)
            {
                this.cardRect = cardRect;
                this.cardImage = cardImage;
                this.hoverWashImage = hoverWashImage;
                this.actionImage = actionImage;
                this.actionText = actionText;
                this.accentLineImage = accentLineImage;
                this.affinitySealImage = affinitySealImage;
                this.accent = accent;
                this.primary = primary;
                this.claimable = claimable;
                pulsePhase = Mathf.Abs(accent.r * 11.7f + accent.g * 17.3f + accent.b * 23.1f);

                if (cardRect != null)
                {
                    baseScale = cardRect.localScale;
                }
            }

            public void SetHover(bool hover)
            {
                this.hover = claimable && hover;
            }

            public void SetPressed(bool pressed)
            {
                this.pressed = claimable && pressed;
                if (this.pressed)
                {
                    pressHoldTimer = 0.28f;
                }
            }

            private void LateUpdate()
            {
                if (cardRect == null)
                {
                    return;
                }

                float follow = 1f - Mathf.Exp(-Time.unscaledDeltaTime * (pressed ? 30f : 13f));
                pressHoldTimer = Mathf.Max(0f, pressHoldTimer - Time.unscaledDeltaTime);
                bool activePress = pressed || pressHoldTimer > 0f;
                float scale = pressed ? (primary ? 1.002f : 0.992f) : hover ? (primary ? 1.020f : 1.014f) : primary ? 1.006f : 1f;

                cardRect.localScale = Vector3.Lerp(cardRect.localScale, baseScale * scale, follow);

                if (cardImage != null)
                {
                    Color target = activePress
                        ? new Color(0.92f, 0.80f, 0.58f, 0.99f)
                        : hover
                            ? new Color(0.94f, 0.84f, 0.62f, primary ? 0.99f : 0.96f)
                            : primary
                                ? new Color(0.92f, 0.81f, 0.58f, 0.98f)
                                : new Color(0.86f, 0.76f, 0.56f, 0.90f);
                    cardImage.color = Color.Lerp(cardImage.color, target, follow);
                }

                if (hoverWashImage != null)
                {
                    float alpha = activePress ? (primary ? 0.135f : 0.105f) : hover ? (primary ? 0.090f : 0.065f) : primary ? 0.030f : 0f;
                    Color target = new Color(accent.r, accent.g, accent.b, alpha);
                    hoverWashImage.color = Color.Lerp(hoverWashImage.color, target, follow);
                }

                if (actionImage != null)
                {
                    float alpha = activePress ? 0.48f : hover ? 0.34f : 0.20f;
                    Color target = new Color(accent.r, accent.g, accent.b, alpha);
                    actionImage.color = Color.Lerp(actionImage.color, target, follow);
                }

                if (actionText != null)
                {
                    actionText.text = claimable ? "踏入此境" : "尚未开放";
                    Color target = activePress || hover
                        ? new Color(0.30f, 0.13f, 0.05f, 0.99f)
                        : new Color(0.23f, 0.12f, 0.06f, claimable ? 0.96f : 0.58f);
                    actionText.color = Color.Lerp(actionText.color, target, follow);
                }

                if (accentLineImage != null)
                {
                    float alpha = activePress ? 1f : hover ? 0.96f : primary ? 0.95f : 0.72f;
                    Color target = new Color(accent.r, accent.g, accent.b, alpha);
                    accentLineImage.color = Color.Lerp(accentLineImage.color, target, follow);
                }

                if (affinitySealImage != null)
                {
                    float pulse = Mathf.Sin((Time.unscaledTime + pulsePhase) * 2.2f) * 0.035f;
                    float alpha = activePress ? 1f : hover ? 0.94f : 0.82f + pulse;
                    Color target = new Color(0.62f, 0.16f, 0.10f, alpha);
                    affinitySealImage.color = Color.Lerp(affinitySealImage.color, target, follow);
                }
            }
        }
        private sealed class DemoOpeningItemCardFx : MonoBehaviour
        {
            private RectTransform cardRect;
            private Image cardImage;
            private Image hoverWashImage;
            private Image sealImage;
            private Text sealText;
            private Text actionText;
            private Color accent = ColorGold;
            private Vector3 baseScale = Vector3.one;
            private bool hover;
            private bool pressed;
            private bool claimable;
            private float sealHoldTimer;

            public void Configure(
                RectTransform cardRect,
                Image cardImage,
                Image hoverWashImage,
                Image sealImage,
                Text sealText,
                Text actionText,
                Color accent,
                bool claimable)
            {
                this.cardRect = cardRect;
                this.cardImage = cardImage;
                this.hoverWashImage = hoverWashImage;
                this.sealImage = sealImage;
                this.sealText = sealText;
                this.actionText = actionText;
                this.accent = accent;
                this.claimable = claimable;

                if (cardRect != null)
                {
                    baseScale = cardRect.localScale;
                }
            }

            public void SetHover(bool hover)
            {
                this.hover = claimable && hover;
            }

            public void SetPressed(bool pressed)
            {
                this.pressed = claimable && pressed;
                if (this.pressed)
                {
                    sealHoldTimer = 0.48f;
                }
            }

            private void LateUpdate()
            {
                if (cardRect == null)
                {
                    return;
                }

                float follow = 1f - Mathf.Exp(-Time.unscaledDeltaTime * (pressed ? 28f : 14f));
                sealHoldTimer = Mathf.Max(0f, sealHoldTimer - Time.unscaledDeltaTime);
                bool showSeal = pressed || sealHoldTimer > 0f;
                float scale = pressed ? 0.992f : hover ? 1.018f : 1f;

                cardRect.localScale = Vector3.Lerp(cardRect.localScale, baseScale * scale, follow);

                if (cardImage != null)
                {
                    Color target = hover || pressed
                        ? new Color(0.98f, 0.90f, 0.68f, pressed ? 0.38f : 0.31f)
                        : new Color(0.92f, 0.84f, 0.66f, 0.24f);
                    cardImage.color = Color.Lerp(cardImage.color, target, follow);
                }

                if (hoverWashImage != null)
                {
                    float alpha = pressed ? 0.105f : hover ? 0.070f : 0f;
                    Color target = new Color(accent.r, accent.g, accent.b, alpha);
                    hoverWashImage.color = Color.Lerp(hoverWashImage.color, target, follow);
                }

                if (sealImage != null)
                {
                    Color target = showSeal
                        ? new Color(0.80f, 0.16f, 0.10f, 0.96f)
                        : new Color(0.72f, 0.19f, 0.12f, 0f);
                    sealImage.color = Color.Lerp(sealImage.color, target, follow);
                }

                if (sealText != null)
                {
                    sealText.text = showSeal ? "定" : string.Empty;
                    Color target = showSeal
                        ? new Color(1f, 0.86f, 0.68f, 0.98f)
                        : new Color(1f, 0.86f, 0.68f, 0f);
                    sealText.color = Color.Lerp(sealText.color, target, follow);
                }

                if (actionText != null)
                {
                    actionText.text = claimable ? "带上此物" : "尚未开放";
                    Color target = new Color(0.36f, 0.20f, 0.08f, claimable ? 0.96f : 0.58f);
                    actionText.color = Color.Lerp(actionText.color, target, follow);
                }
            }
        }

        private sealed class DemoRootDestinyDeskFx : MonoBehaviour
        {
            private DemoReward[] rewards;
            private Sprite[] rootSprites;
            private Image rootObjectImage;
            private RectTransform objectFrame;
            private Text rootNameText;
            private Text rarityText;
            private Text identityText;
            private Text effectText;
            private Text footerText;
            private RectTransform[] lotRects;
            private Image[] lotGlowImages;
            private Image[] lotSelectedSealImages;
            private Text[] lotSelectedSealTexts;
            private Text[] lotNameTexts;
            private Text[] lotHintTexts;
            private Vector2[] lotBasePositions;
            private Quaternion[] lotBaseRotations;
            private RectTransform[] idleSmokeRects;
            private Image[] idleSmokeImages;
            private RectTransform[] burstSmokeRects;
            private Image[] burstSmokeImages;
            private Image pageSmokeImage;
            private RectTransform confirmRect;
            private Button confirmButton;
            private Image confirmSealImage;
            private Text confirmText;
            private RectTransform inkSweepRect;
            private Image inkSweepImage;
            private Vector2 confirmBasePosition;
            private Vector2 confirmSealBasePosition;
            private Quaternion confirmSealBaseRotation = Quaternion.identity;
            private Vector2[] idleSmokeBasePositions;
            private Vector2[] burstSmokeBasePositions;
            private Vector2[] burstSmokeDirections;
            private float[] idleSmokePhases;
            private float[] idleSmokePeriods;
            private int selectedIndex;
            private int pendingIndex = -1;
            private float transitionTimer;
            private bool transitionActive;
            private bool pendingApplied;
            private bool confirmHover;
            private bool confirmPressed;

            public int SelectedIndex
            {
                get { return pendingIndex >= 0 ? pendingIndex : selectedIndex; }
            }

            public bool CanClaimSelected
            {
                get { return rewards != null && selectedIndex >= 0 && selectedIndex < rewards.Length && DemoRuntimeCanvasUI.IsRootClaimable(rewards[selectedIndex]); }
            }

            public void Configure(
                DemoReward[] rewards,
                Sprite[] rootSprites,
                Image rootObjectImage,
                RectTransform objectFrame,
                Text rootNameText,
                Text rarityText,
                Text identityText,
                Text effectText,
                Text footerText,
                RectTransform[] lotRects,
                Image[] lotGlowImages,
                Image[] lotSelectedSealImages,
                Text[] lotSelectedSealTexts,
                Text[] lotNameTexts,
                Text[] lotHintTexts,
                RectTransform[] idleSmokeRects,
                Image[] idleSmokeImages,
                RectTransform[] burstSmokeRects,
                Image[] burstSmokeImages,
                Image pageSmokeImage,
                RectTransform confirmRect,
                Button confirmButton,
                Image confirmSealImage,
                Text confirmText,
                RectTransform inkSweepRect)
            {
                this.rewards = rewards ?? new DemoReward[0];
                this.rootSprites = rootSprites ?? new Sprite[0];
                this.rootObjectImage = rootObjectImage;
                this.objectFrame = objectFrame;
                this.rootNameText = rootNameText;
                this.rarityText = rarityText;
                this.identityText = identityText;
                this.effectText = effectText;
                this.footerText = footerText;
                this.lotRects = lotRects ?? new RectTransform[0];
                this.lotGlowImages = lotGlowImages ?? new Image[0];
                this.lotSelectedSealImages = lotSelectedSealImages ?? new Image[0];
                this.lotSelectedSealTexts = lotSelectedSealTexts ?? new Text[0];
                this.lotNameTexts = lotNameTexts ?? new Text[0];
                this.lotHintTexts = lotHintTexts ?? new Text[0];
                this.idleSmokeRects = idleSmokeRects ?? new RectTransform[0];
                this.idleSmokeImages = idleSmokeImages ?? new Image[0];
                this.burstSmokeRects = burstSmokeRects ?? new RectTransform[0];
                this.burstSmokeImages = burstSmokeImages ?? new Image[0];
                this.pageSmokeImage = pageSmokeImage;
                this.confirmRect = confirmRect;
                this.confirmButton = confirmButton;
                this.confirmSealImage = confirmSealImage;
                this.confirmText = confirmText;
                this.inkSweepRect = inkSweepRect;
                inkSweepImage = inkSweepRect != null ? inkSweepRect.GetComponent<Image>() : null;
                confirmBasePosition = confirmRect != null ? confirmRect.anchoredPosition : Vector2.zero;
                if (confirmSealImage != null)
                {
                    RectTransform sealRect = confirmSealImage.rectTransform;
                    confirmSealBasePosition = sealRect.anchoredPosition;
                    confirmSealBaseRotation = sealRect.localRotation;
                }

                lotBasePositions = new Vector2[this.lotRects.Length];
                lotBaseRotations = new Quaternion[this.lotRects.Length];
                for (int i = 0; i < this.lotRects.Length; i++)
                {
                    if (this.lotRects[i] == null)
                    {
                        continue;
                    }

                    lotBasePositions[i] = this.lotRects[i].anchoredPosition;
                    lotBaseRotations[i] = this.lotRects[i].localRotation;
                }

                idleSmokeBasePositions = new Vector2[this.idleSmokeRects.Length];
                idleSmokePhases = new float[this.idleSmokeRects.Length];
                idleSmokePeriods = new float[this.idleSmokeRects.Length];
                for (int i = 0; i < this.idleSmokeRects.Length; i++)
                {
                    idleSmokeBasePositions[i] = this.idleSmokeRects[i].anchoredPosition;
                    idleSmokePhases[i] = i * 0.27f;
                    idleSmokePeriods[i] = 13.5f + i * 2.25f;
                }

                burstSmokeBasePositions = new Vector2[this.burstSmokeRects.Length];
                burstSmokeDirections = new Vector2[this.burstSmokeRects.Length];
                for (int i = 0; i < this.burstSmokeRects.Length; i++)
                {
                    burstSmokeBasePositions[i] = this.burstSmokeRects[i].anchoredPosition;
                    float angle = (-150f + i * 50f) * Mathf.Deg2Rad;
                    burstSmokeDirections[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle) * 0.52f + 0.45f).normalized;
                }
            }

            public void SelectImmediate(int index)
            {
                if (rewards == null || rewards.Length == 0)
                {
                    return;
                }

                ApplyIndex(Mathf.Clamp(index, 0, rewards.Length - 1));
                ResetTransitionVisuals();
            }

            public void Select(int index)
            {
                if (rewards == null || rewards.Length == 0)
                {
                    return;
                }

                index = Mathf.Clamp(index, 0, rewards.Length - 1);
                if (!transitionActive && index == selectedIndex)
                {
                    return;
                }

                pendingIndex = index;
                transitionTimer = 0f;
                transitionActive = true;
                pendingApplied = false;
                confirmHover = false;
                confirmPressed = false;
                HighlightLots(index);
                SetLedgerTextAlpha(1f);
                if (confirmButton != null)
                {
                    confirmButton.interactable = false;
                }
            }

            private void Update()
            {
                UpdateIdleSmoke(Time.unscaledTime);

                if (!transitionActive)
                {
                    UpdateConfirmButtonMotion(Time.unscaledTime);
                    return;
                }

                transitionTimer += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(transitionTimer / 0.44f);

                if (!pendingApplied && progress >= 0.36f)
                {
                    ApplyIndex(pendingIndex);
                    pendingApplied = true;
                }

                UpdateTransitionSweep(progress);
                UpdateTransitionSmoke(progress, Mathf.Sin(progress * Mathf.PI));

                if (rootObjectImage != null)
                {
                    float fadeOut = Mathf.Clamp01(progress / 0.28f);
                    float fadeIn = Mathf.Clamp01((progress - 0.36f) / 0.64f);
                    float alpha = progress < 0.42f
                        ? Mathf.Lerp(1f, 0.20f, fadeOut)
                        : Mathf.Lerp(0.20f, 1f, fadeIn);
                    rootObjectImage.color = new Color(1f, 1f, 1f, alpha);
                }

                if (objectFrame != null)
                {
                    objectFrame.localScale = Vector3.one;
                }

                if (confirmRect != null)
                {
                    bool targetClaimable = rewards != null && pendingIndex >= 0 && pendingIndex < rewards.Length && DemoRuntimeCanvasUI.IsRootClaimable(rewards[pendingIndex]);
                    confirmRect.anchoredPosition = confirmBasePosition + new Vector2(0f, targetClaimable ? Mathf.Sin(progress * Mathf.PI) * -2f : 0f);
                    confirmRect.localScale = Vector3.one;
                }

                if (progress >= 1f)
                {
                    transitionActive = false;
                    pendingIndex = -1;
                    ResetTransitionVisuals();
                }
            }

            public void SetConfirmHover(bool hover)
            {
                confirmHover = hover;
            }

            public void SetConfirmPressed(bool pressed)
            {
                confirmPressed = pressed;
            }

            private void ApplyIndex(int index)
            {
                selectedIndex = Mathf.Clamp(index, 0, Mathf.Max(0, rewards.Length - 1));
                DemoReward reward = rewards[selectedIndex];
                DemoRootDefinition root = reward.Root;
                bool claimable = DemoRuntimeCanvasUI.IsRootClaimable(reward);

                if (rootNameText != null)
                {
                    rootNameText.text = reward.Name;
                }

                if (rarityText != null)
                {
                    rarityText.text = claimable
                        ? $"{DemoRuntimeCanvasUI.GetRootRarityLabel(root?.Rarity)}根脚 · 此身来处"
                        : "命数未显 · 可观不可落";
                }

                if (identityText != null)
                {
                    identityText.text = DemoRuntimeCanvasUI.BuildRootIdentityText(root);
                }

                if (effectText != null)
                {
                    effectText.text = claimable
                        ? DemoRuntimeCanvasUI.BuildRootEffectText(root)
                        : "命数未显：此根脚尚未入命案";
                }

                if (footerText != null)
                {
                    footerText.text = claimable
                        ? "印成之后，自有人递来临行之物。"
                        : "此根脚尚未入命案，待后续开放。";
                }

                if (confirmButton != null)
                {
                    confirmButton.interactable = claimable;
                }

                if (confirmSealImage != null)
                {
                    confirmSealImage.color = claimable
                        ? Color.white
                        : new Color(0.56f, 0.48f, 0.36f, 0.24f);
                }

                if (confirmText != null)
                {
                    confirmText.text = claimable ? "落印定身" : "未入命案";
                    confirmText.color = claimable
                        ? new Color(1.00f, 0.88f, 0.74f, 0.98f)
                        : new Color(0.34f, 0.26f, 0.16f, 0.76f);
                }

                if (rootObjectImage != null)
                {
                    rootObjectImage.sprite = selectedIndex < rootSprites.Length ? rootSprites[selectedIndex] : null;
                    rootObjectImage.preserveAspect = true;
                    rootObjectImage.color = Color.white;
                }

                HighlightLots(selectedIndex);
            }

            private void HighlightLots(int activeIndex)
            {
                for (int i = 0; i < lotGlowImages.Length; i++)
                {
                    bool active = i == activeIndex;
                    bool claimable = rewards != null && i >= 0 && i < rewards.Length && DemoRuntimeCanvasUI.IsRootClaimable(rewards[i]);
                    Image lotImage = i < lotRects.Length && lotRects[i] != null ? lotRects[i].GetComponent<Image>() : null;
                    if (lotImage != null)
                    {
                        lotImage.color = claimable
                            ? (active ? new Color(1f, 0.92f, 0.72f, 0.96f) : new Color(1f, 0.94f, 0.80f, 0.88f))
                            : (active ? new Color(0.94f, 0.88f, 0.76f, 0.88f) : new Color(0.92f, 0.86f, 0.74f, 0.80f));
                    }

                    if (lotGlowImages[i] != null)
                    {
                        lotGlowImages[i].color = active
                            ? (claimable ? new Color(0.82f, 0.58f, 0.25f, 0.24f) : new Color(0.58f, 0.44f, 0.25f, 0.12f))
                            : new Color(0.82f, 0.58f, 0.25f, 0f);
                    }

                    if (i < lotSelectedSealImages.Length && lotSelectedSealImages[i] != null)
                    {
                        lotSelectedSealImages[i].color = active
                            ? (claimable ? new Color(0.78f, 0.18f, 0.12f, 0.74f) : new Color(0.46f, 0.28f, 0.18f, 0.26f))
                            : new Color(0.78f, 0.18f, 0.12f, 0f);
                    }

                    if (i < lotSelectedSealTexts.Length && lotSelectedSealTexts[i] != null)
                    {
                        lotSelectedSealTexts[i].text = claimable ? "定" : "未";
                        lotSelectedSealTexts[i].color = active
                            ? (claimable ? new Color(1.0f, 0.86f, 0.70f, 0.96f) : new Color(0.62f, 0.50f, 0.36f, 0.76f))
                            : new Color(1.0f, 0.86f, 0.70f, 0f);
                    }

                    if (i < lotRects.Length && lotRects[i] != null)
                    {
                        Vector2 basePosition = i < lotBasePositions.Length ? lotBasePositions[i] : lotRects[i].anchoredPosition;
                        Quaternion baseRotation = i < lotBaseRotations.Length ? lotBaseRotations[i] : lotRects[i].localRotation;
                        lotRects[i].anchoredPosition = active ? basePosition + new Vector2(-24f, 2f) : basePosition;
                        lotRects[i].localRotation = active
                            ? Quaternion.Euler(0f, 0f, -0.6f)
                            : baseRotation;
                        lotRects[i].localScale = active ? Vector3.one * 1.035f : Vector3.one;
                    }

                    if (i < lotNameTexts.Length && lotNameTexts[i] != null)
                    {
                        lotNameTexts[i].color = active
                            ? (claimable ? new Color(0.12f, 0.08f, 0.04f, 0.98f) : new Color(0.20f, 0.15f, 0.10f, 0.90f))
                            : (claimable ? new Color(0.22f, 0.15f, 0.08f, 0.90f) : new Color(0.30f, 0.24f, 0.16f, 0.82f));
                    }

                    if (i < lotHintTexts.Length && lotHintTexts[i] != null)
                    {
                        lotHintTexts[i].color = active
                            ? (claimable ? new Color(0.58f, 0.28f, 0.16f, 0.92f) : new Color(0.46f, 0.30f, 0.18f, 0.90f))
                            : (claimable ? new Color(0.58f, 0.42f, 0.20f, 0.80f) : new Color(0.54f, 0.42f, 0.28f, 0.76f));
                    }
                }
            }

            private void UpdateConfirmButtonMotion(float time)
            {
                if (confirmRect == null)
                {
                    return;
                }

                bool claimable = CanClaimSelected;
                float follow = 1f - Mathf.Exp(-Time.unscaledDeltaTime * (confirmPressed ? 34f : 14f));
                Vector2 targetPosition = confirmBasePosition;
                if (claimable && confirmPressed)
                {
                    targetPosition += new Vector2(0f, -8f);
                }
                else if (claimable && confirmHover)
                {
                    targetPosition += new Vector2(0f, -1.5f);
                }

                confirmRect.anchoredPosition = Vector2.Lerp(confirmRect.anchoredPosition, targetPosition, follow);
                confirmRect.localScale = Vector3.one;

                if (confirmSealImage != null)
                {
                    RectTransform sealRect = confirmSealImage.rectTransform;
                    Vector2 targetSealPosition = confirmSealBasePosition + (claimable && confirmPressed ? new Vector2(0f, -2f) : Vector2.zero);
                    sealRect.anchoredPosition = Vector2.Lerp(sealRect.anchoredPosition, targetSealPosition, follow);
                    float baseRotation = confirmSealBaseRotation.eulerAngles.z;
                    if (baseRotation > 180f)
                    {
                        baseRotation -= 360f;
                    }

                    float targetRotation = claimable && confirmPressed ? baseRotation - 0.8f : claimable && confirmHover ? baseRotation + 0.4f : baseRotation;
                    sealRect.localRotation = Quaternion.Lerp(sealRect.localRotation, Quaternion.Euler(0f, 0f, targetRotation), follow);
                    confirmSealImage.color = claimable
                        ? (confirmPressed ? new Color(0.86f, 0.74f, 0.66f, 1f) : confirmHover ? new Color(1f, 0.95f, 0.88f, 1f) : Color.white)
                        : new Color(0.56f, 0.48f, 0.36f, 0.24f);
                }

                if (confirmText != null)
                {
                    confirmText.color = claimable
                        ? (confirmPressed ? new Color(1.00f, 0.78f, 0.60f, 1f) : confirmHover ? new Color(1.00f, 0.92f, 0.80f, 0.98f) : new Color(1.00f, 0.88f, 0.74f, 0.98f))
                        : new Color(0.34f, 0.26f, 0.16f, 0.76f);
                }
            }

            private void UpdateIdleSmoke(float time)
            {
                for (int i = 0; i < idleSmokeRects.Length; i++)
                {
                    if (idleSmokeRects[i] == null)
                    {
                        continue;
                    }

                    float phase = i < idleSmokePhases.Length ? idleSmokePhases[i] : i;
                    float period = i < idleSmokePeriods.Length ? idleSmokePeriods[i] : 11f;
                    float loop = Mathf.Repeat(time / Mathf.Max(0.01f, period) + phase, 1f);
                    float arc = Mathf.Sin(loop * Mathf.PI);
                    float drift = loop * (12f + i * 3f) + Mathf.Sin(loop * Mathf.PI * 2f + phase) * (5f + i * 1.5f);
                    float rise = loop * (76f + i * 12f);
                    idleSmokeRects[i].anchoredPosition = idleSmokeBasePositions[i] + new Vector2(drift, rise);
                    idleSmokeRects[i].localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(loop * Mathf.PI * 2f + i * 0.4f) * 3.2f);
                    idleSmokeRects[i].localScale = Vector3.one * (0.50f + loop * 0.32f);

                    if (i < idleSmokeImages.Length && idleSmokeImages[i] != null)
                    {
                        float alpha = arc * (0.055f + i * 0.012f);
                        idleSmokeImages[i].color = new Color(0.86f, 0.90f, 0.88f, Mathf.Clamp(alpha, 0f, 0.095f));
                    }
                }
            }

            private void UpdateTransitionSweep(float progress)
            {
                float fadeOut = Mathf.Clamp01(progress / 0.22f);
                float fadeIn = Mathf.Clamp01((progress - 0.36f) / 0.44f);
                float textAlpha = progress < 0.36f
                    ? Mathf.Lerp(1f, 0.08f, fadeOut)
                    : Mathf.Lerp(0.08f, 1f, fadeIn);
                SetLedgerTextAlpha(textAlpha);

                if (inkSweepRect != null)
                {
                    float eased = Mathf.SmoothStep(0f, 1f, progress);
                    float sweepAlpha = Mathf.Sin(progress * Mathf.PI) * 0.42f;
                    inkSweepRect.anchoredPosition = new Vector2(Mathf.Lerp(-330f, 330f, eased), -8f);
                    inkSweepRect.sizeDelta = new Vector2(Mathf.Lerp(160f, 430f, Mathf.Sin(progress * Mathf.PI)), 3f);

                    if (inkSweepImage != null)
                    {
                        inkSweepImage.color = new Color(0.86f, 0.62f, 0.22f, sweepAlpha);
                    }
                }
            }

            private void UpdateTransitionSmoke(float progress, float smokeAmount)
            {
                if (pageSmokeImage != null)
                {
                    pageSmokeImage.color = new Color(0.88f, 0.90f, 0.88f, smokeAmount * 0.34f);
                }

                for (int i = 0; i < burstSmokeRects.Length; i++)
                {
                    if (burstSmokeRects[i] == null)
                    {
                        continue;
                    }

                    Vector2 direction = i < burstSmokeDirections.Length ? burstSmokeDirections[i] : Vector2.up;
                    Vector2 origin = i < burstSmokeBasePositions.Length ? burstSmokeBasePositions[i] : Vector2.zero;
                    float distance = Mathf.Lerp(18f, 104f + i * 6f, smokeAmount);
                    burstSmokeRects[i].anchoredPosition = origin + direction * distance;
                    burstSmokeRects[i].localRotation = Quaternion.Euler(0f, 0f, -30f + i * 11f + progress * 38f);
                    burstSmokeRects[i].localScale = Vector3.one * (0.70f + smokeAmount * 0.44f);

                    if (i < burstSmokeImages.Length && burstSmokeImages[i] != null)
                    {
                        burstSmokeImages[i].color = new Color(0.86f, 0.90f, 0.88f, smokeAmount * 0.46f);
                    }
                }
            }

            private void SetLedgerTextAlpha(float alpha)
            {
                SetTextAlpha(rootNameText, alpha);
                SetTextAlpha(rarityText, alpha);
                SetTextAlpha(identityText, alpha);
                SetTextAlpha(effectText, alpha);
                SetTextAlpha(footerText, alpha);
                SetTextAlpha(confirmText, alpha);
                SetImageAlpha(confirmSealImage, alpha);
            }

            private static void SetTextAlpha(Text text, float alpha)
            {
                if (text == null)
                {
                    return;
                }

                Color color = text.color;
                color.a = Mathf.Clamp01(alpha);
                text.color = color;
            }

            private static void SetImageAlpha(Image image, float alpha)
            {
                if (image == null)
                {
                    return;
                }

                Color color = image.color;
                color.a = Mathf.Clamp01(alpha);
                image.color = color;
            }

            private void ResetTransitionVisuals()
            {
                if (pageSmokeImage != null)
                {
                    pageSmokeImage.color = new Color(0.88f, 0.90f, 0.88f, 0f);
                }

                if (rootObjectImage != null)
                {
                    rootObjectImage.color = Color.white;
                }

                if (objectFrame != null)
                {
                    objectFrame.localScale = Vector3.one;
                }

                if (confirmRect != null)
                {
                    confirmRect.anchoredPosition = confirmBasePosition;
                    confirmRect.localScale = Vector3.one;
                }

                if (confirmSealImage != null)
                {
                    RectTransform sealRect = confirmSealImage.rectTransform;
                    sealRect.anchoredPosition = confirmSealBasePosition;
                    sealRect.localRotation = confirmSealBaseRotation;
                }

                if (inkSweepImage != null)
                {
                    inkSweepImage.color = new Color(0.86f, 0.62f, 0.22f, 0f);
                }

                SetLedgerTextAlpha(1f);
                RefreshConfirmState();

                for (int i = 0; i < burstSmokeImages.Length; i++)
                {
                    if (burstSmokeImages[i] != null)
                    {
                        burstSmokeImages[i].color = new Color(0.86f, 0.90f, 0.88f, 0f);
                    }
                }
            }

            private void RefreshConfirmState()
            {
                if (rewards == null || selectedIndex < 0 || selectedIndex >= rewards.Length)
                {
                    return;
                }

                bool claimable = DemoRuntimeCanvasUI.IsRootClaimable(rewards[selectedIndex]);
                if (!claimable)
                {
                    confirmHover = false;
                    confirmPressed = false;
                }

                if (confirmButton != null)
                {
                    confirmButton.interactable = claimable;
                }

                if (confirmSealImage != null)
                {
                    confirmSealImage.color = claimable
                        ? Color.white
                        : new Color(0.56f, 0.48f, 0.36f, 0.24f);
                }

                if (confirmText != null)
                {
                    confirmText.text = claimable ? "落印定身" : "未入命案";
                    confirmText.color = claimable
                        ? new Color(1.00f, 0.88f, 0.74f, 0.98f)
                        : new Color(0.34f, 0.26f, 0.16f, 0.76f);
                }
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
