using System.Collections;
using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.UI
{
    public sealed class DemoBattleSceneView : MonoBehaviour
    {
        private const string BattleBackgroundResourcePath = "Art/Scenes/scene_battle_cloudsea_001";
        private const string BattleFarSceneResourcePath = "Art/Scenes/scene_cloudsea_far_001";
        private const string BattleMidSceneResourcePath = "Art/Scenes/scene_battle_cloudsea_mid_001";
        private const string BattleNearSceneResourcePath = "Art/Scenes/scene_battle_cloudsea_near_001";
        private const string BattleThunderMarshEntryResourcePath = "Art/Scenes/scene_battle_thunder_marsh_entry_001";
        private const string BattleOldMineEntryResourcePath = "Art/Scenes/scene_battle_old_mine_entry_001";
        private const string PlayerCharacterResourcePath = "Art/Characters/char_player_sword_cultivator_battle_002";
        private const string EnemyWraithResourcePath = "Art/Characters/char_enemy_tribulation_wraith_battle_002";
        private const string PlayerPortraitResourcePath = "Art/Characters/char_player_sword_cultivator_portrait_002";
        private const string VfxFlyingSwordResourcePath = "Art/VFX/vfx_flying_sword_001";
        private const string VfxSwordSlashResourcePath = "Art/VFX/vfx_sword_slash_001";
        private const string VfxThunderArcResourcePath = "Art/VFX/vfx_thunder_arc_001";
        private const string VfxImpactInkBurstResourcePath = "Art/VFX/vfx_impact_ink_burst_001";
        private const string BossPortraitResourcePath = "Art/Boss/boss_tianjie_halfbody_001";
        private const string BossPortraitFallbackResourcePath = "Art/Boss/boss_tianjie_halfbody_002";
        private const string BattleHeaderRibbonResourcePath = "Art/UI/ui_battle_header_ribbon_001";
        private const string BattleStatusBrushResourcePath = "Art/UI/ui_battle_status_brush_002";
        private const string BattleStatusPlateResourcePath = "Art/UI/ui_battle_status_plate_001";
        private const string BattleEnemyPlateResourcePath = "Art/UI/ui_battle_enemy_plate_001";
        private const string BattleIntentPlateResourcePath = "Art/UI/ui_battle_intent_plate_001";
        private const string BattlePhaseSealResourcePath = "Art/UI/ui_battle_phase_seal_002";
        private const string BattleCloudWispAResourcePath = "Art/UI/ui_root_smoke_wisp_001";
        private const string BattleCloudWispBResourcePath = "Art/UI/ui_root_smoke_wisp_002";
        private const float BattleEntryIntroDuration = 1.08f;
        private static readonly Color HudPaper = new Color(0.94f, 0.93f, 0.89f, 1f);
        private static readonly Color HudMist = new Color(0.63f, 0.67f, 0.72f, 1f);
        private static readonly Color HudGold = new Color(0.85f, 0.72f, 0.44f, 1f);
        private static readonly Color HudGoldDim = new Color(0.52f, 0.41f, 0.22f, 0.88f);
        private static readonly Color HudInk = new Color(0.12f, 0.12f, 0.12f, 0.90f);
        private static readonly Color HudJade = new Color(0.43f, 0.63f, 0.72f, 1f);
        private static readonly Color HudCrimson = new Color(0.74f, 0.34f, 0.31f, 1f);
        private static Sprite cachedBattleBackgroundSprite;
        private static Sprite cachedBattleFarSceneSprite;
        private static Sprite cachedBattleMidSceneSprite;
        private static Sprite cachedBattleNearSceneSprite;
        private static Sprite cachedBattleThunderMarshEntrySprite;
        private static Sprite cachedBattleOldMineEntrySprite;
        private static Sprite cachedPlayerCharacterSprite;
        private static Sprite cachedEnemyWraithSprite;
        private static Sprite cachedPlayerPortraitSprite;
        private static Sprite cachedVfxFlyingSwordSprite;
        private static Sprite cachedVfxSwordSlashSprite;
        private static Sprite cachedVfxThunderArcSprite;
        private static Sprite cachedVfxImpactInkBurstSprite;
        private static Sprite cachedBossPortraitSprite;
        private static Sprite cachedBattleHeaderRibbonSprite;
        private static Sprite cachedBattleStatusPlateSprite;
        private static Sprite cachedBattleEnemyPlateSprite;
        private static Sprite cachedBattleIntentPlateSprite;
        private static Sprite cachedBattlePhaseSealSprite;
        private static Sprite cachedBattleCloudWispASprite;
        private static Sprite cachedBattleCloudWispBSprite;

        private sealed class AmbientDrift
        {
            public RectTransform Rect;
            public Image Image;
            public Vector2 OriginNormalized;
            public Vector2 Amplitude;
            public float Speed;
            public float Phase;
            public Color BaseColor;
            public float Pulse;
        }

        private sealed class FloatingPopup
        {
            public RectTransform Rect;
            public Text Text;
            public float Age;
            public float Duration;
            public Vector2 Velocity;
            public Color BaseColor;
        }

        private enum EncounterVisualTier
        {
            Minor,
            Elite,
            MiniBoss,
            FinalBoss
        }

        private DemoGameController controller;
        private Font uiFont;
        private Sprite whiteSprite;
        private Sprite battleBackgroundSprite;
        private Sprite battleFarSceneSprite;
        private Sprite battleMidSceneSprite;
        private Sprite battleNearSceneSprite;
        private Sprite bossPortraitSprite;
        private Sprite battleHeaderRibbonSprite;
        private Sprite battleStatusPlateSprite;
        private Sprite battleEnemyPlateSprite;
        private Sprite battleIntentPlateSprite;
        private Sprite battlePhaseSealSprite;
        private Sprite playerCharacterSprite;
        private Sprite enemyWraithSprite;
        private Sprite playerPortraitSprite;
        private Sprite vfxFlyingSwordSprite;
        private Sprite vfxSwordSlashSprite;
        private Sprite vfxThunderArcSprite;
        private Sprite vfxImpactInkBurstSprite;
        private Sprite battleCloudWispASprite;
        private Sprite battleCloudWispBSprite;
        private bool usesRegionBattleBackground;

        private RectTransform rootRect;
        private RectTransform skyGlow;
        private RectTransform horizonGlow;
        private RectTransform farCloudA;
        private RectTransform farCloudB;
        private RectTransform farRidgeA;
        private RectTransform farRidgeB;
        private RectTransform mistBand;
        private RectTransform midCloudShelf;
        private RectTransform midIslandLeft;
        private RectTransform midIslandRight;
        private RectTransform midRuinBand;
        private RectTransform foregroundMist;
        private RectTransform inkVeil;
        private RectTransform nearCliffLeft;
        private RectTransform nearCliffRight;
        private RectTransform nearPlatformGlowLeft;
        private RectTransform nearPlatformGlowRight;
        private RectTransform frontFogA;
        private RectTransform frontFogB;
        private RectTransform playerRoot;
        private RectTransform playerSword;
        private RectTransform playerBody;
        private RectTransform playerPlatformSword;
        private RectTransform playerHairTrailA;
        private RectTransform playerHairTrailB;
        private RectTransform playerRobeTrail;
        private RectTransform regionCloudA;
        private RectTransform regionCloudB;
        private RectTransform enemyRoot;
        private RectTransform enemyPlatformSword;
        private RectTransform enemySword;
        private RectTransform enemyBody;
        private Image enemyAuraImage;
        private Image enemyPortraitImage;
        private Image enemySwordImage;
        private Image enemySwordTrailImage;
        private Image enemyBodyImage;
        private Text enemyLabelText;
        private RectTransform playerStatusPanel;
        private Image playerPortraitImage;
        private RectTransform enemyStatusPanel;
        private RectTransform roundStatusPanel;
        private RectTransform intentPanel;
        private RectTransform idlePanel;
        private Text playerNameText;
        private Text playerStatusText;
        private Text playerResourceText;
        private Text enemyNameText;
        private Text enemyStatusText;
        private Text enemyResourceText;
        private Text roundStatusText;
        private Text phaseText;
        private Text intentText;
        private Image playerHealthFillImage;
        private Image playerEnergyFillImage;
        private Image playerEnergyGlowImage;
        private Image enemyHealthFillImage;
        private Image intentProgressFillImage;
        private Text idleTitleText;
        private Text idleBodyText;
        private Image flashOverlay;
        private Image thunderOverlay;
        private Image brushOverlay;
        private Image bossPortrait;
        private Image entryCameraVeil;
        private Vector2 rootBaseAnchoredPosition;
        private Vector3 rootBaseScale = Vector3.one;
        private float battleEntryIntroTimer;
        private bool battleEntryIntroActive;

        private readonly List<AmbientDrift> ambientDrifts = new List<AmbientDrift>();
        private readonly List<FloatingPopup> popups = new List<FloatingPopup>();
        private readonly List<Image> transientMarks = new List<Image>();

        private Coroutine sequenceCoroutine;

        private float elapsed;

        public void Initialize(DemoGameController demoController, Font font)
        {
            controller = demoController;
            uiFont = font;
            whiteSprite = CreateWhiteSprite();
            LoadVisualResourcesForCurrentState();
            BuildScene();
        }

        public void RefreshForCurrentBattle()
        {
            if (controller == null || !controller.HasBattle)
            {
                return;
            }

            LoadVisualResourcesForCurrentState();
            RebuildSceneGraph();
            StartBattleEntryIntro();
        }

        private void LoadVisualResourcesForCurrentState()
        {
            battleBackgroundSprite = LoadBattleBackgroundSprite();
            usesRegionBattleBackground = IsUsingRegionBattleBackground();
            battleFarSceneSprite = usesRegionBattleBackground ? null : LoadSceneLayerSprite(ref cachedBattleFarSceneSprite, BattleFarSceneResourcePath);
            battleMidSceneSprite = usesRegionBattleBackground ? null : LoadSceneLayerSprite(ref cachedBattleMidSceneSprite, BattleMidSceneResourcePath);
            battleNearSceneSprite = usesRegionBattleBackground ? null : LoadSceneLayerSprite(ref cachedBattleNearSceneSprite, BattleNearSceneResourcePath);
            bossPortraitSprite = LoadBossPortraitSprite();
            battleHeaderRibbonSprite = LoadSceneLayerSprite(ref cachedBattleHeaderRibbonSprite, BattleHeaderRibbonResourcePath);
            battleStatusPlateSprite = LoadSceneLayerSprite(ref cachedBattleStatusPlateSprite, BattleStatusBrushResourcePath);
            if (battleStatusPlateSprite == null)
            {
                battleStatusPlateSprite = LoadSceneLayerSprite(ref cachedBattleStatusPlateSprite, BattleStatusPlateResourcePath);
            }

            battleEnemyPlateSprite = battleStatusPlateSprite != null
                ? battleStatusPlateSprite
                : LoadSceneLayerSprite(ref cachedBattleEnemyPlateSprite, BattleEnemyPlateResourcePath);
            battleIntentPlateSprite = LoadSceneLayerSprite(ref cachedBattleIntentPlateSprite, BattleIntentPlateResourcePath);
            battlePhaseSealSprite = LoadSceneLayerSprite(ref cachedBattlePhaseSealSprite, BattlePhaseSealResourcePath);
            playerCharacterSprite = LoadSceneLayerSprite(ref cachedPlayerCharacterSprite, PlayerCharacterResourcePath);
            enemyWraithSprite = LoadSceneLayerSprite(ref cachedEnemyWraithSprite, EnemyWraithResourcePath);
            playerPortraitSprite = LoadSceneLayerSprite(ref cachedPlayerPortraitSprite, PlayerPortraitResourcePath);
            vfxFlyingSwordSprite = LoadSceneLayerSprite(ref cachedVfxFlyingSwordSprite, VfxFlyingSwordResourcePath);
            vfxSwordSlashSprite = LoadSceneLayerSprite(ref cachedVfxSwordSlashSprite, VfxSwordSlashResourcePath);
            vfxThunderArcSprite = LoadSceneLayerSprite(ref cachedVfxThunderArcSprite, VfxThunderArcResourcePath);
            vfxImpactInkBurstSprite = LoadSceneLayerSprite(ref cachedVfxImpactInkBurstSprite, VfxImpactInkBurstResourcePath);
            battleCloudWispASprite = LoadSceneLayerSprite(ref cachedBattleCloudWispASprite, BattleCloudWispAResourcePath);
            battleCloudWispBSprite = LoadSceneLayerSprite(ref cachedBattleCloudWispBSprite, BattleCloudWispBResourcePath);
        }

        private void RebuildSceneGraph()
        {
            if (rootRect != null)
            {
                for (int i = rootRect.childCount - 1; i >= 0; i--)
                {
                    GameObject child = rootRect.GetChild(i).gameObject;
                    child.SetActive(false);
                    Destroy(child);
                }
            }

            ambientDrifts.Clear();
            popups.Clear();
            transientMarks.Clear();
            sequenceCoroutine = null;

            entryCameraVeil = null;
            BuildScene();
        }

        private void Update()
        {
            if (controller == null || rootRect == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            UpdateBattleEntryIntro(Time.deltaTime);
            UpdateAmbientMotion();
            UpdatePopups(Time.deltaTime);

            if (!controller.HasBattle)
            {
                UpdateIdleStage();
                UpdateEncounterPresentation();
                return;
            }

            SetCombatVisualState(true);

            if (sequenceCoroutine == null &&
                controller.Battle.TryConsumePresentationStep(out DemoBattlePresentationStep presentationStep))
            {
                sequenceCoroutine = StartCoroutine(PlayQueuedPresentationStep(presentationStep));
            }

            if (controller.Battle.Phase == DemoBattlePhase.Intro)
            {
                phaseText.text = "入阵";
                phaseText.color = new Color(0.80f, 0.87f, 0.92f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Running)
            {
                phaseText.text = "斗法";
                phaseText.color = new Color(0.94f, 0.83f, 0.55f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Won)
            {
                phaseText.text = "已破";
                phaseText.color = new Color(0.96f, 0.88f, 0.54f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Lost)
            {
                phaseText.text = "失守";
                phaseText.color = new Color(0.93f, 0.50f, 0.43f, 1f);
            }

            UpdateBossAtmosphere();
            UpdateEncounterPresentation();
            UpdateHudPanels();
        }

        private void BuildScene()
        {
            rootRect = gameObject.GetComponent<RectTransform>();
            if (rootRect.GetComponent<RectMask2D>() == null)
            {
                rootRect.gameObject.AddComponent<RectMask2D>();
            }

            rootBaseAnchoredPosition = rootRect.anchoredPosition;
            rootBaseScale = Vector3.one;
            rootRect.localScale = rootBaseScale;
            CreateBackground();
            CreateActors();
            CreateOverlay();
            CreateBattleEntryIntroOverlay();
        }

        private void CreateBackground()
        {
            Image backdrop = CreateImage(
                "Backdrop",
                rootRect,
                battleBackgroundSprite != null ? new Color(0.98f, 0.98f, 0.96f, 1f) : new Color(0.88f, 0.88f, 0.84f, 1f));
            backdrop.type = Image.Type.Simple;
            backdrop.sprite = battleBackgroundSprite != null ? battleBackgroundSprite : whiteSprite;
            backdrop.preserveAspect = false;
            StretchRect(backdrop.rectTransform);

            CreatePanel(
                "BackdropPaperWash",
                rootRect,
                new Color(0.98f, 0.97f, 0.92f, battleBackgroundSprite != null ? 0.07f : 0.14f),
                Vector2.zero,
                Vector2.one,
                Vector2.zero,
                Vector2.zero);
            CreatePanel(
                "LowerBattleWash",
                rootRect,
                new Color(0.04f, 0.045f, 0.045f, battleBackgroundSprite != null ? 0.035f : 0.10f),
                new Vector2(0f, 0f),
                new Vector2(1f, 0.24f),
                Vector2.zero,
                Vector2.zero);

            skyGlow = CreatePanelRect(
                "SkyBreath",
                rootRect,
                new Color(0.92f, 0.95f, 0.94f, usesRegionBattleBackground ? 0f : 0.035f),
                new Vector2(0.22f, 0.70f),
                new Vector2(0.82f, 1f),
                Vector2.zero,
                Vector2.zero);
            horizonGlow = CreatePanelRect(
                "HorizonBreath",
                rootRect,
                new Color(0.64f, 0.70f, 0.69f, usesRegionBattleBackground ? 0f : 0.018f),
                new Vector2(0.16f, 0.28f),
                new Vector2(0.84f, 0.48f),
                Vector2.zero,
                Vector2.zero);

            farCloudA = CreateQuietAtmosphereRect("FarCloudA", new Vector2(0.06f, 0.66f), new Vector2(0.38f, 0.71f), 0.012f, -4f);
            farCloudB = CreateQuietAtmosphereRect("FarCloudB", new Vector2(0.56f, 0.60f), new Vector2(0.96f, 0.645f), 0.010f, 3f);
            farRidgeA = CreateQuietAtmosphereRect("FarRidgeA", new Vector2(0.08f, 0.41f), new Vector2(0.34f, 0.455f), 0.012f, 5f);
            farRidgeB = CreateQuietAtmosphereRect("FarRidgeB", new Vector2(0.64f, 0.39f), new Vector2(0.92f, 0.435f), 0.010f, -4f);
            mistBand = CreateQuietAtmosphereRect("MistBand", new Vector2(0.10f, 0.245f), new Vector2(0.88f, 0.285f), 0.012f, 0f);
            midCloudShelf = CreateQuietAtmosphereRect("MidCloudShelf", new Vector2(0.22f, 0.53f), new Vector2(0.77f, 0.565f), 0.010f, -2f);
            midRuinBand = CreateQuietAtmosphereRect("MidRuinBand", new Vector2(0.26f, 0.33f), new Vector2(0.72f, 0.355f), 0.010f, 1f);
            midIslandLeft = CreateQuietAtmosphereRect("MidIslandLeft", new Vector2(0.08f, 0.30f), new Vector2(0.27f, 0.325f), 0.014f, -5f);
            midIslandRight = CreateQuietAtmosphereRect("MidIslandRight", new Vector2(0.72f, 0.46f), new Vector2(0.92f, 0.49f), 0.014f, 5f);
            foregroundMist = CreateQuietAtmosphereRect("ForegroundMist", new Vector2(0.04f, 0.20f), new Vector2(0.96f, 0.235f), 0.010f, 0f);
            inkVeil = CreateQuietAtmosphereRect("InkVeil", new Vector2(0.34f, 0.60f), new Vector2(0.74f, 0.625f), 0.006f, -3f);
            nearCliffLeft = CreateQuietAtmosphereRect("NearCliffLeft", new Vector2(0f, 0.15f), new Vector2(0.17f, 0.205f), 0.012f, 7f);
            nearCliffRight = CreateQuietAtmosphereRect("NearCliffRight", new Vector2(0.82f, 0.20f), new Vector2(1f, 0.255f), 0.012f, -7f);
            nearPlatformGlowLeft = CreateQuietAtmosphereRect("NearPlatformGlowLeft", new Vector2(0.08f, 0.205f), new Vector2(0.29f, 0.218f), 0.018f, -2f);
            nearPlatformGlowRight = CreateQuietAtmosphereRect("NearPlatformGlowRight", new Vector2(0.73f, 0.43f), new Vector2(0.92f, 0.443f), 0.018f, 2f);
            frontFogA = CreateQuietAtmosphereRect("FrontFogA", new Vector2(0.02f, 0.175f), new Vector2(0.50f, 0.205f), 0.010f, 0f);
            frontFogB = CreateQuietAtmosphereRect("FrontFogB", new Vector2(0.50f, 0.16f), new Vector2(0.98f, 0.19f), 0.008f, 0f);

            if (bossPortraitSprite != null && enemyWraithSprite == null)
            {
                bossPortrait = CreateImage("BossPortrait", rootRect, new Color(0.72f, 0.74f, 0.76f, 0f));
                bossPortrait.type = Image.Type.Simple;
                bossPortrait.sprite = bossPortraitSprite;
                bossPortrait.preserveAspect = true;
                bossPortrait.rectTransform.anchorMin = new Vector2(0.58f, 0.12f);
                bossPortrait.rectTransform.anchorMax = new Vector2(0.98f, 0.94f);
                bossPortrait.rectTransform.offsetMin = Vector2.zero;
                bossPortrait.rectTransform.offsetMax = Vector2.zero;
            }

            CreateRegionCloudLayers();
            ambientDrifts.Clear();
        }

        private void CreateRegionCloudLayers()
        {
            if (!usesRegionBattleBackground)
            {
                regionCloudA = null;
                regionCloudB = null;
                return;
            }

            regionCloudA = CreateCloudWisp(
                "RegionCloudFar",
                battleCloudWispASprite,
                new Vector2(1040f, 280f),
                new Color(0.90f, 0.92f, 0.90f, 0.075f),
                false);
            regionCloudB = CreateCloudWisp(
                "RegionCloudMid",
                battleCloudWispBSprite != null ? battleCloudWispBSprite : battleCloudWispASprite,
                new Vector2(820f, 240f),
                new Color(0.82f, 0.86f, 0.84f, 0.11f),
                true);
        }

        private RectTransform CreateCloudWisp(string name, Sprite sprite, Vector2 size, Color color, bool mirror)
        {
            if (sprite == null)
            {
                return null;
            }

            Image cloud = CreateImage(name, rootRect, color);
            cloud.sprite = sprite;
            cloud.type = Image.Type.Simple;
            cloud.preserveAspect = true;
            cloud.raycastTarget = false;

            RectTransform rect = cloud.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.localScale = new Vector3(mirror ? -1f : 1f, 1f, 1f);
            return rect;
        }

        private RectTransform CreateQuietAtmosphereRect(string name, Vector2 anchorMin, Vector2 anchorMax, float alpha, float rotation)
        {
            RectTransform rect = CreatePanelRect(
                name,
                rootRect,
                new Color(0.72f, 0.75f, 0.74f, usesRegionBattleBackground ? 0f : alpha),
                anchorMin,
                anchorMax,
                Vector2.zero,
                Vector2.zero);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
            return rect;
        }

        private static void StretchRect(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
        private void CreateActors()
        {
            Vector2 playerAnchor = GetPlayerAnchor();
            Vector2 enemyAnchor = GetEnemyAnchor();
            bool useBossCombatArt = controller != null && controller.HasBattle && controller.Battle.IsBossBattle && bossPortraitSprite != null;
            Sprite enemyCombatSprite = useBossCombatArt ? bossPortraitSprite : enemyWraithSprite;

            playerRoot = CreateEntity(
                "PlayerRoot",
                playerAnchor,
                new Color(0.20f, 0.22f, 0.24f, 1f),
                new Color(0.70f, 0.86f, 0.94f, 1f),
                string.Empty,
                true,
                true);
            playerRoot.sizeDelta = new Vector2(540f, 330f);
            playerSword = playerRoot.Find("Sword") as RectTransform;
            playerBody = ApplyEntitySprite(playerRoot, playerCharacterSprite, new Vector2(520f, 262f), new Vector2(-30f, 78f));
            CreatePlayerWindPresentation();
            ApplySwordSprite(playerSword, true);

            enemyRoot = CreateEntity(
                "EnemyRoot",
                enemyAnchor,
                new Color(0.90f, 0.93f, 0.94f, 1f),
                new Color(0.50f, 0.74f, 0.92f, 1f),
                string.Empty,
                false,
                false);
            enemyRoot.sizeDelta = new Vector2(520f, 400f);
            enemySword = enemyRoot.Find("Sword") as RectTransform;
            enemyBody = ApplyEntitySprite(
                enemyRoot,
                enemyCombatSprite,
                useBossCombatArt ? new Vector2(560f, 390f) : new Vector2(600f, 372f),
                useBossCombatArt ? new Vector2(-12f, 54f) : new Vector2(-8f, 70f));
            CreateEnemyGroundPresentation();
            ApplySwordSprite(enemySword, false);

            enemyAuraImage = FindEntityImage(enemyRoot, "Aura");
            enemySwordImage = FindEntityImage(enemyRoot, "Sword");
            enemySwordTrailImage = FindEntityImage(enemyRoot, "SwordTrail");
            enemyBodyImage = enemyBody != null ? enemyBody.GetComponent<Image>() : null;
            Transform enemyLabel = enemyRoot.Find("Label");
            enemyLabelText = enemyLabel != null ? enemyLabel.GetComponent<Text>() : null;
        }

        private void CreatePlayerWindPresentation()
        {
            playerPlatformSword = CreateWindSprite(
                "PlayerPlatformSword",
                vfxFlyingSwordSprite,
                new Vector2(380f, 108f),
                new Vector2(-26f, -54f),
                new Color(0.66f, 0.84f, 0.90f, 0.92f),
                -4f,
                1);

            playerHairTrailA = CreateWindSprite(
                "PlayerHairTrailA",
                vfxSwordSlashSprite,
                new Vector2(210f, 54f),
                new Vector2(-124f, 116f),
                new Color(0.05f, 0.06f, 0.075f, 0.38f),
                174f,
                2);
            playerHairTrailB = CreateWindSprite(
                "PlayerHairTrailB",
                vfxSwordSlashSprite,
                new Vector2(174f, 42f),
                new Vector2(-108f, 96f),
                new Color(0.08f, 0.09f, 0.11f, 0.28f),
                168f,
                2);
            playerRobeTrail = CreateWindSprite(
                "PlayerRobeTrail",
                vfxSwordSlashSprite,
                new Vector2(236f, 70f),
                new Vector2(-112f, 34f),
                new Color(0.16f, 0.24f, 0.30f, 0.30f),
                184f,
                2);
        }

        private void CreateEnemyGroundPresentation()
        {
            bool hasSwordArt = vfxFlyingSwordSprite != null;
            Image image = CreateImage(
                "EnemyPlatformSword",
                enemyRoot,
                hasSwordArt
                    ? new Color(0.46f, 0.68f, 0.78f, 0.62f)
                    : new Color(0.38f, 0.62f, 0.70f, 0.24f));
            image.sprite = hasSwordArt ? vfxFlyingSwordSprite : whiteSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.raycastTarget = false;

            enemyPlatformSword = image.rectTransform;
            enemyPlatformSword.anchorMin = new Vector2(0.5f, 0.5f);
            enemyPlatformSword.anchorMax = new Vector2(0.5f, 0.5f);
            enemyPlatformSword.pivot = new Vector2(0.5f, 0.5f);
            enemyPlatformSword.sizeDelta = hasSwordArt ? new Vector2(410f, 112f) : new Vector2(300f, 10f);
            enemyPlatformSword.anchoredPosition = new Vector2(18f, -70f);
            enemyPlatformSword.localRotation = Quaternion.Euler(0f, 0f, 4f);
            enemyPlatformSword.SetSiblingIndex(Mathf.Clamp(1, 0, enemyRoot.childCount - 1));
        }
        private RectTransform CreateWindSprite(string name, Sprite sprite, Vector2 size, Vector2 position, Color color, float rotation, int siblingIndex)
        {
            Image image = CreateImage(name, playerRoot, color);
            image.sprite = sprite != null ? sprite : whiteSprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = true;
            image.raycastTarget = false;

            RectTransform rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);
            rect.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, playerRoot.childCount - 1));
            return rect;
        }

        private void ApplySwordSprite(RectTransform swordRect, bool isPlayer)
        {
            if (swordRect == null || vfxFlyingSwordSprite == null)
            {
                return;
            }

            Image swordImage = swordRect.GetComponent<Image>();
            if (swordImage == null)
            {
                return;
            }

            swordImage.sprite = vfxFlyingSwordSprite;
            swordImage.type = Image.Type.Simple;
            swordImage.preserveAspect = true;
            swordImage.color = isPlayer ? new Color(0.78f, 0.92f, 1f, 1f) : new Color(0.66f, 0.82f, 0.94f, 0.90f);
            swordRect.sizeDelta = isPlayer ? new Vector2(210f, 66f) : new Vector2(156f, 48f);
            swordRect.anchoredPosition = isPlayer ? new Vector2(92f, 20f) : new Vector2(-54f, 18f);
        }

        private static Image FindEntityImage(RectTransform root, string childName)
        {
            Transform child = root != null ? root.Find(childName) : null;
            return child != null ? child.GetComponent<Image>() : null;
        }
        private void CreateOverlay()
        {
            playerStatusPanel = CreateHudPanel(
                "PlayerScroll",
                new Vector2(0.025f, 0.835f),
                new Vector2(0.37f, 0.965f),
                new Color(0.96f, 0.95f, 0.90f, 0.12f),
                new Color(0.52f, 0.42f, 0.24f, 0.32f));
            Image playerPanelImage = playerStatusPanel.GetComponent<Image>();
            if (playerPanelImage != null)
            {
                playerPanelImage.color = new Color(0.035f, 0.045f, 0.055f, 0.36f);
                playerPanelImage.raycastTarget = false;
            }

            RectTransform portraitWell = CreatePanelRect(
                "PlayerPortraitWell",
                playerStatusPanel,
                new Color(0.08f, 0.09f, 0.09f, 0.34f),
                new Vector2(0.018f, 0.08f),
                new Vector2(0.176f, 0.91f),
                Vector2.zero,
                Vector2.zero);
            DecorateHudFrame(portraitWell, new Color(0.66f, 0.56f, 0.34f, 0.42f));
            playerPortraitImage = CreateImage("PlayerPortrait", portraitWell, Color.white);
            playerPortraitImage.sprite = playerPortraitSprite != null ? playerPortraitSprite : playerCharacterSprite;
            playerPortraitImage.type = Image.Type.Simple;
            playerPortraitImage.preserveAspect = true;
            StretchRect(playerPortraitImage.rectTransform);

            playerNameText = CreateText("PlayerName", playerStatusPanel, 20, FontStyle.Bold, TextAnchor.UpperLeft, HudPaper);
            playerNameText.rectTransform.anchorMin = new Vector2(0.195f, 0.70f);
            playerNameText.rectTransform.anchorMax = new Vector2(0.62f, 0.98f);
            playerNameText.rectTransform.offsetMin = Vector2.zero;
            playerNameText.rectTransform.offsetMax = Vector2.zero;

            playerResourceText = CreateText("PlayerResource", playerStatusPanel, 13, FontStyle.Bold, TextAnchor.UpperRight, HudGold);
            playerResourceText.rectTransform.anchorMin = new Vector2(0.56f, 0.70f);
            playerResourceText.rectTransform.anchorMax = new Vector2(0.97f, 0.98f);
            playerResourceText.rectTransform.offsetMin = Vector2.zero;
            playerResourceText.rectTransform.offsetMax = Vector2.zero;

            CreateMiniLabel(playerStatusPanel, "HpLabel", "血", new Vector2(0.195f, 0.48f), new Vector2(0.235f, 0.62f), HudCrimson);
            playerHealthFillImage = CreateInlineBar(
                playerStatusPanel,
                "PlayerHealth",
                new Vector2(0.245f, 0.495f),
                new Vector2(0.965f, 0.605f),
                new Color(0.22f, 0.17f, 0.14f, 0.42f),
                new Color(0.72f, 0.22f, 0.18f, 0.96f),
                new Color(0.46f, 0.12f, 0.10f, 0.38f));

            CreateMiniLabel(playerStatusPanel, "EnergyLabel", "灵", new Vector2(0.195f, 0.30f), new Vector2(0.235f, 0.44f), HudJade);
            playerEnergyFillImage = CreateInlineBar(
                playerStatusPanel,
                "PlayerEnergy",
                new Vector2(0.245f, 0.315f),
                new Vector2(0.965f, 0.425f),
                new Color(0.14f, 0.20f, 0.20f, 0.40f),
                new Color(0.36f, 0.68f, 0.72f, 0.96f),
                new Color(0.18f, 0.48f, 0.52f, 0.34f));
            playerEnergyGlowImage = CreatePanelRect(
                "PlayerEnergyGlow",
                playerStatusPanel,
                new Color(0.58f, 0.90f, 0.92f, 0f),
                new Vector2(0.245f, 0.285f),
                new Vector2(0.965f, 0.455f),
                Vector2.zero,
                Vector2.zero).GetComponent<Image>();

            playerStatusText = CreateText("PlayerStatus", playerStatusPanel, 12, FontStyle.Bold, TextAnchor.LowerLeft, HudMist);
            playerStatusText.rectTransform.anchorMin = new Vector2(0.195f, 0.04f);
            playerStatusText.rectTransform.anchorMax = new Vector2(0.965f, 0.27f);
            playerStatusText.rectTransform.offsetMin = Vector2.zero;
            playerStatusText.rectTransform.offsetMax = Vector2.zero;

            RectTransform topScroll = CreateHudPanel(
                "BattleSeal",
                new Vector2(0.455f, 0.835f),
                new Vector2(0.545f, 0.965f),
                new Color(0.12f, 0.11f, 0.09f, 0.20f),
                new Color(0.58f, 0.48f, 0.28f, 0.40f));
            if (battlePhaseSealSprite != null)
            {
                ApplyHudSprite(topScroll, battlePhaseSealSprite, Color.white);
            }
            else
            {
                CreatePhaseSealColorModule(topScroll);
            }

            phaseText = CreateText("PhaseText", topScroll, 21, FontStyle.Bold, TextAnchor.MiddleCenter, HudInk);
            StretchRect(phaseText.rectTransform);
            phaseText.rectTransform.offsetMin = new Vector2(10f, 14f);
            phaseText.rectTransform.offsetMax = new Vector2(-10f, -10f);
            phaseText.text = "入阵";

            roundStatusPanel = CreatePanelRect(
                "RoundLabel",
                rootRect,
                new Color(0.05f, 0.055f, 0.055f, 0f),
                new Vector2(0.38f, 0.798f),
                new Vector2(0.62f, 0.832f),
                Vector2.zero,
                Vector2.zero);
            roundStatusText = CreateText("RoundStatus", roundStatusPanel, 13, FontStyle.Bold, TextAnchor.MiddleCenter, HudPaper);
            StretchRect(roundStatusText.rectTransform);
            roundStatusText.rectTransform.offsetMin = new Vector2(6f, 2f);
            roundStatusText.rectTransform.offsetMax = new Vector2(-6f, -2f);

            enemyStatusPanel = CreateHudPanel(
                "EnemyScroll",
                new Vector2(0.63f, 0.835f),
                new Vector2(0.975f, 0.965f),
                new Color(0.96f, 0.94f, 0.90f, 0.12f),
                new Color(0.52f, 0.28f, 0.24f, 0.34f));
            Image enemyPanelImage = enemyStatusPanel.GetComponent<Image>();
            if (enemyPanelImage != null)
            {
                enemyPanelImage.color = new Color(0.045f, 0.035f, 0.040f, 0.36f);
                enemyPanelImage.raycastTarget = false;
            }

            RectTransform enemyPortraitWell = CreatePanelRect(
                "EnemyPortraitWell",
                enemyStatusPanel,
                new Color(0.08f, 0.09f, 0.09f, 0.26f),
                new Vector2(0.82f, 0.08f),
                new Vector2(0.982f, 0.91f),
                Vector2.zero,
                Vector2.zero);
            DecorateHudFrame(enemyPortraitWell, new Color(0.58f, 0.34f, 0.28f, 0.32f));
            enemyPortraitImage = CreateImage("EnemyPortrait", enemyPortraitWell, Color.white);
            enemyPortraitImage.sprite = controller != null && controller.HasBattle && controller.Battle.IsBossBattle && bossPortraitSprite != null
                ? bossPortraitSprite
                : enemyWraithSprite;
            enemyPortraitImage.type = Image.Type.Simple;
            enemyPortraitImage.preserveAspect = true;
            StretchRect(enemyPortraitImage.rectTransform);

            enemyNameText = CreateText("EnemyName", enemyStatusPanel, 20, FontStyle.Bold, TextAnchor.UpperRight, HudPaper);
            enemyNameText.rectTransform.anchorMin = new Vector2(0.38f, 0.66f);
            enemyNameText.rectTransform.anchorMax = new Vector2(0.80f, 0.98f);
            enemyNameText.rectTransform.offsetMin = Vector2.zero;
            enemyNameText.rectTransform.offsetMax = Vector2.zero;

            enemyResourceText = CreateText("EnemyResource", enemyStatusPanel, 13, FontStyle.Bold, TextAnchor.UpperLeft, HudCrimson);
            enemyResourceText.rectTransform.anchorMin = new Vector2(0.05f, 0.66f);
            enemyResourceText.rectTransform.anchorMax = new Vector2(0.36f, 0.98f);
            enemyResourceText.rectTransform.offsetMin = Vector2.zero;
            enemyResourceText.rectTransform.offsetMax = Vector2.zero;

            enemyHealthFillImage = CreateInlineBar(
                enemyStatusPanel,
                "EnemyHealth",
                new Vector2(0.06f, 0.40f),
                new Vector2(0.80f, 0.54f),
                new Color(0.24f, 0.16f, 0.14f, 0.42f),
                new Color(0.66f, 0.18f, 0.15f, 0.96f),
                new Color(0.42f, 0.10f, 0.09f, 0.38f));

            enemyStatusText = CreateText("EnemyStatus", enemyStatusPanel, 12, FontStyle.Bold, TextAnchor.LowerRight, HudMist);
            enemyStatusText.rectTransform.anchorMin = new Vector2(0.05f, 0.05f);
            enemyStatusText.rectTransform.anchorMax = new Vector2(0.80f, 0.35f);
            enemyStatusText.rectTransform.offsetMin = Vector2.zero;
            enemyStatusText.rectTransform.offsetMax = Vector2.zero;

            intentPanel = CreateHudPanel(
                "IntentPanel",
                new Vector2(0.67f, 0.775f),
                new Vector2(0.975f, 0.825f),
                new Color(0.025f, 0.075f, 0.085f, 0.28f),
                new Color(0.34f, 0.52f, 0.54f, 0.30f));

            intentProgressFillImage = CreateInlineBar(
                intentPanel,
                "IntentProgress",
                new Vector2(0.04f, 0.09f),
                new Vector2(0.96f, 0.22f),
                new Color(0.14f, 0.18f, 0.18f, 0.30f),
                new Color(0.36f, 0.62f, 0.66f, 0.68f),
                new Color(0.18f, 0.44f, 0.46f, 0.20f));
            intentText = CreateText("IntentText", intentPanel, 13, FontStyle.Bold, TextAnchor.MiddleLeft, HudPaper);
            intentText.rectTransform.anchorMin = new Vector2(0f, 0.24f);
            intentText.rectTransform.anchorMax = Vector2.one;
            intentText.rectTransform.offsetMin = new Vector2(16f, 1f);
            intentText.rectTransform.offsetMax = new Vector2(-16f, -2f);

            flashOverlay = CreateImage("FlashOverlay", rootRect, new Color(1f, 1f, 1f, 0f));
            StretchRect(flashOverlay.rectTransform);
            thunderOverlay = CreateImage("ThunderOverlay", rootRect, new Color(0.36f, 0.48f, 0.70f, 0f));
            StretchRect(thunderOverlay.rectTransform);
            brushOverlay = CreateImage("BrushOverlay", rootRect, new Color(0.07f, 0.08f, 0.10f, 0f));
            StretchRect(brushOverlay.rectTransform);

            idlePanel = CreateHudPanel(
                "IdlePanel",
                new Vector2(0.30f, 0.20f),
                new Vector2(0.70f, 0.33f),
                new Color(0.94f, 0.92f, 0.84f, 0.76f),
                new Color(0.42f, 0.34f, 0.20f, 0.42f));
            idleTitleText = CreateText("IdleTitle", idlePanel, 20, FontStyle.Bold, TextAnchor.UpperCenter, HudInk);
            idleTitleText.rectTransform.anchorMin = new Vector2(0f, 0.52f);
            idleTitleText.rectTransform.anchorMax = Vector2.one;
            idleTitleText.rectTransform.offsetMin = new Vector2(18f, -6f);
            idleTitleText.rectTransform.offsetMax = new Vector2(-18f, -8f);
            idleBodyText = CreateText("IdleBody", idlePanel, 13, FontStyle.Normal, TextAnchor.UpperCenter, new Color(0.32f, 0.33f, 0.32f, 0.86f));
            idleBodyText.rectTransform.anchorMin = Vector2.zero;
            idleBodyText.rectTransform.anchorMax = new Vector2(1f, 0.60f);
            idleBodyText.rectTransform.offsetMin = new Vector2(20f, 10f);
            idleBodyText.rectTransform.offsetMax = new Vector2(-20f, -4f);
        }
        private void CreatePhaseSealColorModule(RectTransform parent)
        {
            Image image = parent.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0.08f, 0.085f, 0.08f, 0.94f);
            }

            RectTransform core = CreatePanelRect(
                "PhaseSealCore",
                parent,
                new Color(0.12f, 0.13f, 0.13f, 0.98f),
                new Vector2(0.10f, 0.10f),
                new Vector2(0.90f, 0.90f),
                Vector2.zero,
                Vector2.zero);
            DecorateHudFrame(core, new Color(0.74f, 0.62f, 0.38f, 0.34f));
        }

        private void CreateBattleEntryIntroOverlay()
        {
            entryCameraVeil = CreateImage("BattleEntryCameraVeil", rootRect, new Color(0.78f, 0.84f, 0.82f, 0f));
            entryCameraVeil.rectTransform.anchorMin = Vector2.zero;
            entryCameraVeil.rectTransform.anchorMax = Vector2.one;
            entryCameraVeil.rectTransform.offsetMin = Vector2.zero;
            entryCameraVeil.rectTransform.offsetMax = Vector2.zero;
            entryCameraVeil.raycastTarget = false;
        }

        private void StartBattleEntryIntro()
        {
            if (rootRect == null || controller == null || !controller.HasBattle)
            {
                battleEntryIntroActive = false;
                return;
            }

            battleEntryIntroTimer = 0f;
            battleEntryIntroActive = true;
            rootRect.localScale = Vector3.one * 1.075f;
            rootRect.anchoredPosition = rootBaseAnchoredPosition + new Vector2(34f, -22f);
            if (entryCameraVeil != null)
            {
                entryCameraVeil.color = new Color(0.78f, 0.84f, 0.82f, 0.26f);
            }
        }

        private void UpdateBattleEntryIntro(float deltaTime)
        {
            if (!battleEntryIntroActive || rootRect == null)
            {
                return;
            }

            battleEntryIntroTimer += Mathf.Max(0f, deltaTime);
            float progress = Mathf.Clamp01(battleEntryIntroTimer / BattleEntryIntroDuration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            rootRect.localScale = Vector3.one * Mathf.Lerp(1.075f, 1f, eased);
            rootRect.anchoredPosition = Vector2.Lerp(rootBaseAnchoredPosition + new Vector2(34f, -22f), rootBaseAnchoredPosition, eased);

            if (entryCameraVeil != null)
            {
                float veilAlpha = Mathf.Lerp(0.26f, 0f, SmoothStep(progress));
                entryCameraVeil.color = new Color(0.78f, 0.84f, 0.82f, veilAlpha);
            }

            if (progress >= 1f)
            {
                battleEntryIntroActive = false;
                rootRect.localScale = rootBaseScale;
                rootRect.anchoredPosition = rootBaseAnchoredPosition;
                if (entryCameraVeil != null)
                {
                    entryCameraVeil.color = new Color(0.78f, 0.84f, 0.82f, 0f);
                }
            }
        }

        private static float SmoothStep(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }

        private IEnumerator PlaySequence(List<DemoBattlePresentationStep> steps)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                yield return PlayStep(steps[i]);
            }
        }

        private IEnumerator PlayQueuedPresentationStep(DemoBattlePresentationStep step)
        {
            yield return PlayStep(step);
            sequenceCoroutine = null;
        }

        private IEnumerator PlayStep(DemoBattlePresentationStep step)
        {
            phaseText.text = step.Label;

            switch (step.Type)
            {
                case DemoBattlePresentationStepType.BattleStart:
                    yield return new WaitForSeconds(0.05f);
                    break;
                case DemoBattlePresentationStepType.PhaseShift:
                    yield return PlayPhaseShift(step);
                    break;
                case DemoBattlePresentationStepType.CardCast:
                    yield return PlayCardCast(step);
                    break;
                case DemoBattlePresentationStepType.CardDraw:
                    SpawnPopup(playerRoot.anchoredPosition + new Vector2(-18f, 82f), "抽 · " + step.Label, HudJade);
                    yield return new WaitForSeconds(0.06f);
                    break;
                case DemoBattlePresentationStepType.SwordVolley:
                    yield return PlaySwordVolley(step);
                    break;
                case DemoBattlePresentationStepType.SwordStored:
                    SpawnPopup(
                        playerRoot.anchoredPosition + new Vector2(16f, 74f),
                        step.Damage > 0 ? $"剑意 +{step.Damage}" : "飞剑收锋",
                        HudGold);
                    yield return new WaitForSeconds(0.08f);
                    break;
                case DemoBattlePresentationStepType.BossCharge:
                    yield return PlayBossCharge(step);
                    break;
                case DemoBattlePresentationStepType.EnemyAttack:
                    yield return PlayEnemyAttack(step);
                    break;
                case DemoBattlePresentationStepType.Victory:
                    yield return PlayResolution(step, true);
                    break;
                case DemoBattlePresentationStepType.Defeat:
                    yield return PlayResolution(step, false);
                    break;
                default:
                    yield return null;
                    break;
            }
        }

        private IEnumerator PlayCardCast(DemoBattlePresentationStep step)
        {
            Color styleColor = GetStyleColor(step.Style);
            StartCoroutine(SpawnInkWash(
                playerRoot.anchoredPosition + GetCardCastWashOffset(step.Style),
                GetCardCastWashSize(step),
                GetCardCastWashColor(step),
                0.26f));
            StartCoroutine(PlayCardCastPrelude(step));
            yield return AnimateScalePunch(playerRoot, GetCastPunchScale(step), 0.12f);

            int visualHits = GetCardCastVisualHits(step);
            for (int i = 0; i < visualHits; i++)
            {
                Vector2 start = GetSwordTip(playerSword, true) + GetCardCastStartOffset(step, i, visualHits);
                Vector2 end = GetSwordTip(enemySword, false) + GetCardCastEndOffset(step, i, visualHits);
                yield return AnimateSwordFlight(start, end, styleColor, GetFlightDuration(step, false), GetFlightThickness(step, false), step.Style, i);

                if (step.Style == DemoSwordStyle.Thunder)
                {
                    StartCoroutine(SpawnDelayedLightningFork(end + new Vector2(-10f, 20f), step.TriggerShock ? 3 : 2, 0.04f, 0.14f));
                }
            }

            yield return PlayHitOnEnemy(step);
        }

        private IEnumerator PlaySwordVolley(DemoBattlePresentationStep step)
        {
            StartCoroutine(SpawnInkWash(
                ScenePoint(0.48f, 0.48f),
                GetVolleyWashSize(step),
                GetVolleyWashColor(step),
                0.34f));
            StartCoroutine(PlayVolleyPrelude(step));

            int visualHits = GetVolleyVisualHits(step);

            for (int i = 0; i < visualHits; i++)
            {
                Vector2 start = GetSwordTip(playerSword, true) + GetVolleyStartOffset(step, i, visualHits);
                Vector2 end = GetSwordTip(enemySword, false) + GetVolleyEndOffset(step, i, visualHits);
                StartCoroutine(AnimateSwordFlight(start, end, GetVolleyFlightColor(step), GetFlightDuration(step, true), GetFlightThickness(step, true), step.Style, i));

                if (step.Style == DemoSwordStyle.Thunder && (i % 2 == 0 || i == visualHits - 1))
                {
                    StartCoroutine(SpawnDelayedLightningFork(end + new Vector2(-14f, 18f), step.TriggerShock ? 4 : 2, 0.05f, 0.16f));
                }

                if (step.Style == DemoSwordStyle.Blood)
                {
                    StartCoroutine(SpawnDelayedTransientMark(
                        end + new Vector2(-6f, 6f),
                        new Vector2(66f, 7f),
                        new Color(0.72f, 0.18f, 0.20f, 0.34f),
                        0.05f,
                        0.18f,
                        -30f));
                }

                yield return new WaitForSeconds(GetVolleyGap(step.Style));
            }

            yield return PlayHitOnEnemy(step);
        }

        private IEnumerator PlayEnemyAttack(DemoBattlePresentationStep step)
        {
            StartCoroutine(SpawnInkWash(
                enemyRoot.anchoredPosition + new Vector2(14f, 18f),
                step.HeavyImpact ? 124f : 92f,
                new Color(0.64f, 0.28f, 0.26f, 0.24f),
                0.30f));
            yield return AnimateScalePunch(enemyRoot, new Vector3(1.08f, 1.08f, 1f), 0.10f);

            Vector2 start = GetSwordTip(enemySword, false);
            Vector2 end = GetSwordTip(playerSword, true);
            Color strikeColor = step.IsBossAction ? new Color(0.85f, 0.50f, 0.35f, 0.95f) : new Color(0.63f, 0.32f, 0.33f, 0.90f);
            yield return AnimateSwordFlight(start, end, strikeColor, step.IsBossAction ? 0.24f : 0.19f, step.IsBossAction ? 18f : 12f);

            yield return FlashTarget(playerBody, new Color(1f, 0.68f, 0.60f, 1f), step.HeavyImpact ? 1.25f : 1.12f, step.HeavyImpact ? 14f : 8f, true);
            SpawnPopup(playerRoot.anchoredPosition + new Vector2(-16f, 54f), "-" + step.Damage, new Color(1f, 0.68f, 0.60f, 1f));
        }

        private IEnumerator PlayBossCharge(DemoBattlePresentationStep step)
        {
            phaseText.text = "蓄势";
            StartCoroutine(BrushWipe(new Color(0.38f, 0.48f, 0.65f, 0.22f), 0.22f));
            yield return AnimateScalePunch(enemyRoot, new Vector3(1.16f, 1.16f, 1f), 0.18f);

            for (int i = 0; i < 3; i++)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(-10f + i * 12f, 24f + i * 8f),
                    new Vector2(88f, 4f),
                    new Color(0.66f, 0.84f, 1f, 0.45f),
                    0.22f,
                    62f - i * 18f));
            }

            for (int i = 0; i < 2; i++)
            {
                StartCoroutine(SpawnInkWash(
                    enemyRoot.anchoredPosition + new Vector2(-16f + i * 22f, 42f),
                    108f,
                    new Color(0.50f, 0.70f, 0.92f, 0.22f),
                    0.28f));
            }

            flashOverlay.color = new Color(0.65f, 0.78f, 1f, 0.12f);
            yield return new WaitForSeconds(0.14f);
            flashOverlay.color = new Color(0.65f, 0.78f, 1f, 0f);

            if (step.PlayerShockDelta > 0)
            {
                SpawnPopup(playerRoot.anchoredPosition + new Vector2(-22f, 78f), "感电 +" + step.PlayerShockDelta, new Color(0.62f, 0.82f, 1f, 1f));
            }
        }

        private IEnumerator PlayPhaseShift(DemoBattlePresentationStep step)
        {
            phaseText.text = step.Label;
            Color color = new Color(0.60f, 0.78f, 1f, 0.16f);
            StartCoroutine(BrushWipe(new Color(0.08f, 0.10f, 0.13f, 0.26f), 0.30f));
            for (int i = 0; i < 3; i++)
            {
                flashOverlay.color = color;
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(-20f + i * 20f, 44f),
                    new Vector2(118f, 6f),
                    new Color(0.72f, 0.86f, 1f, 0.40f),
                    0.18f,
                    76f - i * 12f));
                yield return new WaitForSeconds(0.08f);
                flashOverlay.color = new Color(color.r, color.g, color.b, 0f);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator PlayResolution(DemoBattlePresentationStep step, bool victory)
        {
            Color color = victory ? new Color(0.97f, 0.88f, 0.58f, 1f) : new Color(0.93f, 0.52f, 0.46f, 1f);
            phaseText.text = victory ? "已破" : "失守";
            flashOverlay.color = new Color(color.r, color.g, color.b, 0f);
            StartCoroutine(BrushWipe(
                victory ? new Color(0.62f, 0.50f, 0.24f, 0.18f) : new Color(0.28f, 0.10f, 0.10f, 0.22f),
                0.32f));

            for (int i = 0; i < 3; i++)
            {
                flashOverlay.color = new Color(color.r, color.g, color.b, 0.10f);
                yield return new WaitForSeconds(0.05f);
                flashOverlay.color = new Color(color.r, color.g, color.b, 0f);
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator PlayHitOnEnemy(DemoBattlePresentationStep step)
        {
            StartCoroutine(SpawnInkWash(
                enemyRoot.anchoredPosition + new Vector2(10f, 26f),
                GetImpactWashSize(step),
                GetImpactColor(step),
                step.HeavyImpact ? 0.34f : 0.24f));
            if (vfxSwordSlashSprite != null)
            {
                StartCoroutine(SpawnSpriteMark(
                    vfxSwordSlashSprite,
                    enemyRoot.anchoredPosition + new Vector2(10f, 24f),
                    new Vector2(step.HeavyImpact ? 176f : 132f, step.HeavyImpact ? 116f : 86f),
                    new Color(1f, 0.96f, 0.84f, step.HeavyImpact ? 0.88f : 0.70f),
                    step.HeavyImpact ? 0.28f : 0.20f,
                    -18f));
            }

            yield return FlashTarget(enemyBody, GetImpactColor(step), GetImpactScale(step), GetImpactPush(step), false);

            if (step.Style == DemoSwordStyle.Thunder)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(2f, 26f),
                    new Vector2(step.HeavyImpact ? 72f : 56f, step.HeavyImpact ? 72f : 56f),
                    new Color(0.54f, 0.78f, 1f, 0.18f),
                    0.16f,
                    18f));
            }

            if (step.TriggerShock)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(0f, 30f),
                    new Vector2(58f, 58f),
                    new Color(0.52f, 0.78f, 1f, 0.38f),
                    0.18f,
                    34f));
                StartCoroutine(SpawnLightningFork(enemyRoot.anchoredPosition + new Vector2(-16f, 44f), 3, 0.18f));
            }

            if (step.TriggerBleed)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(6f, 14f),
                    new Vector2(74f, 8f),
                    new Color(0.80f, 0.24f, 0.26f, 0.48f),
                    0.22f,
                    -22f));
                StartCoroutine(SpawnDelayedTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(-4f, 10f),
                    new Vector2(82f, 5f),
                    new Color(0.54f, 0.10f, 0.12f, 0.30f),
                    0.04f,
                    0.22f,
                    -12f));
            }

            if (step.Style == DemoSwordStyle.General || step.Style == DemoSwordStyle.Wanjian)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(14f, 22f),
                    new Vector2(step.HeavyImpact ? 126f : 92f, 5f),
                    new Color(0.92f, 0.88f, 0.76f, 0.28f),
                    0.20f,
                    -18f));
                StartCoroutine(SpawnDelayedTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(10f, 18f),
                    new Vector2(step.HeavyImpact ? 88f : 64f, 3f),
                    new Color(0.96f, 0.93f, 0.86f, 0.22f),
                    0.03f,
                    0.18f,
                    -6f));
            }

            if (step.Style == DemoSwordStyle.Blood)
            {
                StartCoroutine(SpawnTransientMark(
                    enemyRoot.anchoredPosition + new Vector2(10f, 18f),
                    new Vector2(step.HeavyImpact ? 96f : 74f, 7f),
                    new Color(0.86f, 0.24f, 0.28f, 0.30f),
                    0.24f,
                    -28f));
            }

            if (step.HeavyImpact)
            {
                StartCoroutine(SpawnEmberBurst(enemyRoot.anchoredPosition + new Vector2(6f, 12f), 5));
            }

            SpawnPopup(enemyRoot.anchoredPosition + new Vector2(18f, 54f), "-" + step.Damage, GetImpactColor(step));
        }

        private IEnumerator AnimateSwordFlight(
            Vector2 start,
            Vector2 end,
            Color color,
            float duration,
            float thickness,
            DemoSwordStyle style = DemoSwordStyle.General,
            int laneIndex = 0)
        {
            Color trailColor = new Color(color.r * 0.55f, color.g * 0.55f, color.b * 0.58f, 0.30f);
            Image slash = CreateImage("SwordFlight", rootRect, color);
            transientMarks.Add(slash);
            RectTransform rect = slash.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            if (vfxFlyingSwordSprite != null)
            {
                slash.sprite = vfxFlyingSwordSprite;
                slash.type = Image.Type.Simple;
                slash.preserveAspect = true;
                rect.sizeDelta = new Vector2(Mathf.Max(128f, thickness * 9.5f), Mathf.Max(42f, thickness * 3.0f));
            }
            else
            {
                rect.sizeDelta = new Vector2(96f, thickness);
            }

            Vector2 previousPosition = start;
            float trailTimer = 0f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                trailTimer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                Vector2 position = GetFlightPathPosition(start, end, t, style, laneIndex);
                Vector2 tangent = position - previousPosition;
                if (tangent.sqrMagnitude > 0.01f)
                {
                    rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg);
                }

                rect.anchoredPosition = position;
                slash.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0.92f, 0.20f, t));

                if (trailTimer >= 0.035f && tangent.sqrMagnitude > 1f)
                {
                    trailTimer = 0f;
                    float segmentLength = Mathf.Clamp(tangent.magnitude * 2.4f, 34f, 104f);
                    StartCoroutine(SpawnTransientMark(
                        (previousPosition + position) * 0.5f,
                        new Vector2(segmentLength, thickness * 0.72f),
                        trailColor,
                        duration * 1.35f,
                        Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg));
                }

                previousPosition = position;
                yield return null;
            }

            transientMarks.Remove(slash);
            Destroy(slash.gameObject);
        }

        private static Vector2 GetFlightPathPosition(Vector2 start, Vector2 end, float t, DemoSwordStyle style, int laneIndex)
        {
            Vector2 direct = Vector2.Lerp(start, end, t);
            Vector2 delta = end - start;
            Vector2 normal = delta.sqrMagnitude > 0.01f
                ? new Vector2(-delta.y, delta.x).normalized
                : Vector2.up;

            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    float lightningWave = Mathf.Sin(t * Mathf.PI * 5f + laneIndex * 1.7f);
                    return direct + normal * lightningWave * Mathf.Lerp(28f, 10f, t);
                case DemoSwordStyle.Blood:
                    float hookDirection = laneIndex % 2 == 0 ? -1f : 1f;
                    return direct + normal * Mathf.Sin(t * Mathf.PI) * (72f + laneIndex * 8f) * hookDirection
                        + Vector2.down * Mathf.Sin(t * Mathf.PI) * 18f;
                case DemoSwordStyle.Wanjian:
                    float fanDirection = laneIndex % 2 == 0 ? 1f : -1f;
                    float fanHeight = 24f + laneIndex * 9f;
                    return direct + normal * Mathf.Sin(t * Mathf.PI) * fanHeight * fanDirection;
                default:
                    return direct + normal * Mathf.Sin(t * Mathf.PI) * 16f;
            }
        }

        private IEnumerator FlashTarget(RectTransform targetBody, Color color, float scaleMultiplier, float pushDistance, bool pushLeft)
        {
            Image bodyImage = targetBody.GetComponent<Image>();
            Vector2 origin = targetBody.anchoredPosition;
            Vector3 startScale = targetBody.localScale;
            Vector2 push = new Vector2(pushLeft ? -pushDistance : pushDistance, 0f);
            float duration = 0.16f;
            float timer = 0f;
            Color originalColor = bodyImage.color;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = timer / duration;
                float eased = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 2f);
                targetBody.anchoredPosition = Vector2.Lerp(origin, origin + push, Mathf.Sin(eased * Mathf.PI));
                targetBody.localScale = Vector3.Lerp(startScale, startScale * scaleMultiplier, Mathf.Sin(eased * Mathf.PI));
                bodyImage.color = Color.Lerp(originalColor, color, Mathf.Sin(eased * Mathf.PI));
                yield return null;
            }

            targetBody.anchoredPosition = origin;
            targetBody.localScale = startScale;
            bodyImage.color = originalColor;
        }

        private IEnumerator AnimateScalePunch(RectTransform target, Vector3 punchScale, float duration)
        {
            Vector3 originalScale = target.localScale;
            float timer = 0f;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Sin(Mathf.Clamp01(timer / duration) * Mathf.PI);
                target.localScale = Vector3.Lerp(originalScale, punchScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private IEnumerator SpawnTransientMark(Vector2 position, Vector2 size, Color color, float duration, float rotation)
        {
            Image mark = CreateImage("TransientMark", rootRect, color);
            transientMarks.Add(mark);
            RectTransform rect = mark.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                mark.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0f, t));
                yield return null;
            }

            transientMarks.Remove(mark);
            Destroy(mark.gameObject);
        }

        private IEnumerator SpawnSpriteMark(Sprite sprite, Vector2 position, Vector2 size, Color color, float duration, float rotation)
        {
            if (sprite == null)
            {
                yield return SpawnTransientMark(position, size, color, duration, rotation);
                yield break;
            }

            Image mark = CreateImage("SpriteMark", rootRect, color);
            mark.sprite = sprite;
            mark.type = Image.Type.Simple;
            mark.preserveAspect = true;
            transientMarks.Add(mark);

            RectTransform rect = mark.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float scale = Mathf.Lerp(0.88f, 1.14f, t);
                rect.localScale = new Vector3(scale, scale, 1f);
                mark.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a, 0f, t));
                yield return null;
            }

            transientMarks.Remove(mark);
            Destroy(mark.gameObject);
        }

        private IEnumerator SpawnInkWash(Vector2 position, float size, Color color, float duration)
        {
            Image wash = CreateImage("InkWash", rootRect, new Color(color.r, color.g, color.b, color.a * 0.75f));
            if (vfxImpactInkBurstSprite != null)
            {
                wash.sprite = vfxImpactInkBurstSprite;
                wash.type = Image.Type.Simple;
                wash.preserveAspect = true;
            }

            transientMarks.Add(wash);
            RectTransform rect = wash.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(size * 0.86f, size);
            rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 3.8f + position.x * 0.02f) * 12f);

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                float scale = Mathf.Lerp(0.56f, 1.18f, t);
                rect.localScale = new Vector3(scale, scale * 0.94f, 1f);
                wash.color = new Color(color.r, color.g, color.b, Mathf.Lerp(color.a * 0.58f, 0f, t));
                yield return null;
            }

            transientMarks.Remove(wash);
            Destroy(wash.gameObject);
        }

        private IEnumerator SpawnLightningFork(Vector2 origin, int segments, float duration)
        {
            if (vfxThunderArcSprite != null)
            {
                yield return SpawnSpriteMark(
                    vfxThunderArcSprite,
                    origin + new Vector2(42f, -36f),
                    new Vector2(168f, 104f),
                    new Color(0.84f, 0.93f, 1f, 0.78f),
                    duration,
                    -34f);
                yield break;
            }

            List<Image> forks = new List<Image>();
            Vector2 cursor = origin;

            for (int i = 0; i < segments; i++)
            {
                Vector2 next = cursor + new Vector2(18f + i * 4f, -22f - i * 10f);
                Image bolt = CreateImage("LightningFork", rootRect, new Color(0.72f, 0.88f, 1f, 0.44f));
                transientMarks.Add(bolt);
                forks.Add(bolt);

                RectTransform rect = bolt.rectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(Vector2.Distance(cursor, next), 4f);
                rect.anchoredPosition = (cursor + next) * 0.5f;
                rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(next.y - cursor.y, next.x - cursor.x) * Mathf.Rad2Deg);
                cursor = next + new Vector2((i % 2 == 0 ? -1f : 1f) * 12f, 0f);
            }

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                for (int i = 0; i < forks.Count; i++)
                {
                    forks[i].color = new Color(0.72f, 0.88f, 1f, Mathf.Lerp(0.44f, 0f, t));
                }
                yield return null;
            }

            for (int i = 0; i < forks.Count; i++)
            {
                transientMarks.Remove(forks[i]);
                Destroy(forks[i].gameObject);
            }
        }

        private IEnumerator SpawnEmberBurst(Vector2 origin, int count)
        {
            List<Image> embers = new List<Image>();
            List<Vector2> velocities = new List<Vector2>();

            for (int i = 0; i < count; i++)
            {
                Image ember = CreateImage("Ember", rootRect, new Color(0.92f, 0.56f, 0.26f, 0.36f));
                transientMarks.Add(ember);
                embers.Add(ember);

                RectTransform rect = ember.rectTransform;
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = new Vector2(9f + i * 2f, 9f + i * 2f);
                rect.anchoredPosition = origin;
                velocities.Add(new Vector2(-28f + i * 14f, 18f + i * 10f));
            }

            float duration = 0.30f;
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                for (int i = 0; i < embers.Count; i++)
                {
                    RectTransform rect = embers[i].rectTransform;
                    rect.anchoredPosition += velocities[i] * Time.deltaTime;
                    embers[i].color = new Color(0.92f, 0.56f, 0.26f, Mathf.Lerp(0.36f, 0f, t));
                }
                yield return null;
            }

            for (int i = 0; i < embers.Count; i++)
            {
                transientMarks.Remove(embers[i]);
                Destroy(embers[i].gameObject);
            }
        }

        private IEnumerator BrushWipe(Color color, float duration)
        {
            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Sin(Mathf.Clamp01(timer / duration) * Mathf.PI);
                brushOverlay.color = new Color(color.r, color.g, color.b, color.a * t);
                yield return null;
            }

            brushOverlay.color = new Color(color.r, color.g, color.b, 0f);
        }

        private IEnumerator SpawnDelayedLightningFork(Vector2 origin, int segments, float delay, float duration)
        {
            yield return new WaitForSeconds(delay);
            yield return SpawnLightningFork(origin, segments, duration);
        }

        private IEnumerator SpawnDelayedTransientMark(Vector2 position, Vector2 size, Color color, float delay, float duration, float rotation)
        {
            yield return new WaitForSeconds(delay);
            yield return SpawnTransientMark(position, size, color, duration, rotation);
        }

        private void SpawnPopup(Vector2 position, string value, Color color)
        {
            Text text = CreateText("DamagePopup", rootRect, 22, FontStyle.Bold, TextAnchor.MiddleCenter, color);
            text.text = value;
            text.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            text.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            text.rectTransform.anchoredPosition = position;

            popups.Add(new FloatingPopup
            {
                Rect = text.rectTransform,
                Text = text,
                Duration = 0.62f,
                Velocity = new Vector2(0f, 46f),
                BaseColor = color
            });
        }

        private void UpdateAmbientMotion()
        {
            float width = rootRect.rect.width;
            float height = rootRect.rect.height;

            if (width <= 0f || height <= 0f)
            {
                return;
            }

            UpdateRegionCloudMotion(width);
            // Keep the generated background readable; only quiet atmosphere breathes.
            farCloudA.localRotation = Quaternion.Euler(0f, 0f, -4f + Mathf.Sin(elapsed * 0.12f) * 0.35f);
            farCloudB.localRotation = Quaternion.Euler(0f, 0f, 3f + Mathf.Sin(elapsed * 0.14f + 1.1f) * 0.35f);
            mistBand.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.18f) * 0.01f, 1f, 1f);
            midCloudShelf.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.16f + 0.7f) * 0.008f, 1f, 1f);
            foregroundMist.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.20f + 0.9f) * 0.012f, 1f, 1f);
            frontFogA.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 0.13f) * 0.25f);
            frontFogB.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 0.15f + 1.2f) * 0.25f);
            nearPlatformGlowLeft.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.82f) * 0.04f, 1f, 1f);
            nearPlatformGlowRight.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.76f + 0.5f) * 0.04f, 1f, 1f);
            skyGlow.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.22f) * 0.02f, 1f, 1f);
            horizonGlow.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.36f + 1.4f) * 0.03f, 1f, 1f);

            Vector2 playerAnchor = GetPlayerAnchor();
            playerRoot.anchoredPosition = ScenePoint(playerAnchor.x, playerAnchor.y) + new Vector2(Mathf.Sin(elapsed * 1.2f) * 9f, Mathf.Sin(elapsed * 1.9f) * 8f);
            playerRoot.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 1.4f) * 0.8f - 1.2f);
            playerSword.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 2.1f) * 4f - 7f);
            UpdatePlayerWindMotion();

            EncounterVisualTier encounterTier = GetEncounterVisualTier();
            ApplyEncounterLayerMotion(encounterTier);
            Vector2 enemyAnchor = GetEnemyAnchor();
            float enemyHorizontalDrift = encounterTier == EncounterVisualTier.FinalBoss ? 11f : encounterTier == EncounterVisualTier.MiniBoss ? 9f : encounterTier == EncounterVisualTier.Elite ? 8f : 6f;
            float enemyVerticalDrift = encounterTier == EncounterVisualTier.FinalBoss ? 10f : encounterTier == EncounterVisualTier.MiniBoss ? 8f : encounterTier == EncounterVisualTier.Elite ? 7f : 5f;
            float enemyTiltWave = encounterTier == EncounterVisualTier.FinalBoss ? 2.2f : encounterTier == EncounterVisualTier.MiniBoss ? 1.8f : encounterTier == EncounterVisualTier.Elite ? 1.4f : 1.0f;
            float enemyTiltBase = encounterTier == EncounterVisualTier.FinalBoss ? 5f : encounterTier == EncounterVisualTier.MiniBoss ? 3.2f : encounterTier == EncounterVisualTier.Elite ? 1.7f : 0.8f;
            float enemySwordWave = encounterTier == EncounterVisualTier.FinalBoss ? 3f : encounterTier == EncounterVisualTier.MiniBoss ? 2.6f : encounterTier == EncounterVisualTier.Elite ? 2.2f : 1.8f;
            float enemySwordBase = encounterTier == EncounterVisualTier.FinalBoss ? 8f : encounterTier == EncounterVisualTier.MiniBoss ? 6f : encounterTier == EncounterVisualTier.Elite ? 4.5f : 3f;
            enemyRoot.anchoredPosition = ScenePoint(enemyAnchor.x, enemyAnchor.y) + new Vector2(Mathf.Sin(elapsed * 1.0f + 0.7f) * enemyHorizontalDrift, Mathf.Sin(elapsed * 1.6f + 0.2f) * enemyVerticalDrift);
            enemyRoot.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 1.1f + 0.2f) * enemyTiltWave + enemyTiltBase);
            enemySword.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 2.0f + 0.8f) * enemySwordWave + enemySwordBase);
            if (enemyPlatformSword != null)
            {
                enemyPlatformSword.anchoredPosition = new Vector2(18f, -70f + Mathf.Sin(elapsed * 1.45f + 0.6f) * 3f);
                enemyPlatformSword.localRotation = Quaternion.Euler(0f, 0f, 4f + Mathf.Sin(elapsed * 1.1f) * 1.2f);
            }

            for (int i = 0; i < ambientDrifts.Count; i++)
            {
                AmbientDrift drift = ambientDrifts[i];
                float wave = elapsed * drift.Speed + drift.Phase;
                Vector2 origin = ScenePoint(drift.OriginNormalized.x, drift.OriginNormalized.y);
                drift.Rect.anchoredPosition = origin + new Vector2(Mathf.Sin(wave) * drift.Amplitude.x, Mathf.Cos(wave * 0.72f) * drift.Amplitude.y);
                drift.Image.color = new Color(
                    drift.BaseColor.r,
                    drift.BaseColor.g,
                    drift.BaseColor.b,
                    drift.BaseColor.a * (0.76f + Mathf.Sin(wave * drift.Pulse) * 0.18f));
            }
        }

        private void UpdateRegionCloudMotion(float width)
        {
            if (regionCloudA == null || regionCloudB == null)
            {
                return;
            }

            float farTravel = width + regionCloudA.sizeDelta.x;
            float midTravel = width + regionCloudB.sizeDelta.x;
            float farX = Mathf.Repeat(elapsed * 9f + farTravel * 0.18f, farTravel) - farTravel * 0.5f;
            float midX = Mathf.Repeat(midTravel * 0.78f - elapsed * 16f, midTravel) - midTravel * 0.5f;

            regionCloudA.anchoredPosition = new Vector2(farX, ScenePoint(0.5f, 0.62f).y + Mathf.Sin(elapsed * 0.18f) * 8f);
            regionCloudB.anchoredPosition = new Vector2(midX, ScenePoint(0.5f, 0.43f).y + Mathf.Sin(elapsed * 0.28f + 1.2f) * 12f);
        }

        private void UpdatePlayerWindMotion()
        {
            if (playerPlatformSword != null)
            {
                playerPlatformSword.anchoredPosition = new Vector2(-26f, -54f + Mathf.Sin(elapsed * 1.7f) * 3f);
                playerPlatformSword.localRotation = Quaternion.Euler(0f, 0f, -4f + Mathf.Sin(elapsed * 1.2f) * 1.4f);
            }

            if (playerHairTrailA != null)
            {
                playerHairTrailA.localRotation = Quaternion.Euler(0f, 0f, 174f + Mathf.Sin(elapsed * 2.0f) * 5f);
                playerHairTrailA.localScale = new Vector3(1f + Mathf.Sin(elapsed * 1.35f) * 0.08f, 1f, 1f);
            }

            if (playerHairTrailB != null)
            {
                playerHairTrailB.localRotation = Quaternion.Euler(0f, 0f, 168f + Mathf.Sin(elapsed * 2.4f + 0.8f) * 7f);
                playerHairTrailB.localScale = new Vector3(1f + Mathf.Sin(elapsed * 1.6f + 0.5f) * 0.10f, 1f, 1f);
            }

            if (playerRobeTrail != null)
            {
                playerRobeTrail.localRotation = Quaternion.Euler(0f, 0f, 184f + Mathf.Sin(elapsed * 1.6f + 1.1f) * 4f);
                playerRobeTrail.localScale = new Vector3(1f + Mathf.Sin(elapsed * 1.15f) * 0.07f, 1f, 1f);
            }
        }

        private void ApplyEncounterLayerMotion(EncounterVisualTier tier)
        {
            if (battleBackgroundSprite != null)
            {
                ClearQuietAtmosphereLayers();
                return;
            }

            float pulse = Mathf.Sin(elapsed * 0.9f) * 0.5f + 0.5f;

            SetLayerColor(skyGlow, GetSkyGlowColor(tier, pulse));
            SetLayerColor(horizonGlow, GetHorizonGlowColor(tier, pulse));
            SetLayerColor(mistBand, GetMistBandColor(tier));
            SetLayerColor(midCloudShelf, GetMidCloudColor(tier));
            SetLayerColor(midRuinBand, GetMidRuinColor(tier));
            SetLayerColor(foregroundMist, GetForegroundMistColor(tier));
            SetLayerColor(frontFogA, GetFrontFogAColor(tier));
            SetLayerColor(frontFogB, GetFrontFogBColor(tier));
            SetLayerColor(inkVeil, GetInkVeilColor(tier));
            SetLayerColor(nearCliffLeft, GetNearCliffColor(tier));
            SetLayerColor(nearCliffRight, GetNearCliffColor(tier));
            SetLayerColor(nearPlatformGlowLeft, GetPlatformGlowColor(tier, pulse));
            SetLayerColor(nearPlatformGlowRight, GetPlatformGlowColor(tier, pulse));

            float pressure = GetEncounterPressure(tier);
            float cliffScale = 1f + pressure * 0.08f;
            nearCliffLeft.localScale = new Vector3(cliffScale, 1f + pressure * 0.04f, 1f);
            nearCliffRight.localScale = new Vector3(cliffScale, 1f + pressure * 0.05f, 1f);
            inkVeil.localScale = new Vector3(1f + pressure * 0.05f, 1f + pressure * 0.02f, 1f);
        }

        private void ClearQuietAtmosphereLayers()
        {
            SetLayerColor(skyGlow, Color.clear);
            SetLayerColor(horizonGlow, Color.clear);
            SetLayerColor(farCloudA, Color.clear);
            SetLayerColor(farCloudB, Color.clear);
            SetLayerColor(farRidgeA, Color.clear);
            SetLayerColor(farRidgeB, Color.clear);
            SetLayerColor(mistBand, Color.clear);
            SetLayerColor(midCloudShelf, Color.clear);
            SetLayerColor(midIslandLeft, Color.clear);
            SetLayerColor(midIslandRight, Color.clear);
            SetLayerColor(midRuinBand, Color.clear);
            SetLayerColor(foregroundMist, Color.clear);
            SetLayerColor(inkVeil, Color.clear);
            SetLayerColor(nearCliffLeft, Color.clear);
            SetLayerColor(nearCliffRight, Color.clear);
            SetLayerColor(nearPlatformGlowLeft, Color.clear);
            SetLayerColor(nearPlatformGlowRight, Color.clear);
            SetLayerColor(frontFogA, Color.clear);
            SetLayerColor(frontFogB, Color.clear);
        }

        private static void SetLayerColor(RectTransform rect, Color color)
        {
            if (rect == null)
            {
                return;
            }

            Image image = rect.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }
        }

        private static float GetEncounterPressure(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return 0.35f;
                case EncounterVisualTier.MiniBoss:
                    return 0.62f;
                case EncounterVisualTier.FinalBoss:
                    return 1f;
                default:
                    return 0f;
            }
        }

        private static Color GetSkyGlowColor(EncounterVisualTier tier, float pulse)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.66f, 0.72f, 0.78f, 0.26f + pulse * 0.02f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.70f, 0.66f, 0.55f, 0.24f + pulse * 0.03f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.56f, 0.66f, 0.78f, 0.30f + pulse * 0.04f);
                default:
                    return new Color(0.78f, 0.82f, 0.84f, 0.32f);
            }
        }

        private static Color GetHorizonGlowColor(EncounterVisualTier tier, float pulse)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.18f, 0.23f, 0.28f, 0.08f + pulse * 0.02f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.28f, 0.22f, 0.14f, 0.10f + pulse * 0.02f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.20f, 0.28f, 0.38f, 0.12f + pulse * 0.03f);
                default:
                    return new Color(0.22f, 0.20f, 0.18f, 0.06f);
            }
        }

        private static Color GetMistBandColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.56f, 0.61f, 0.66f, 0.08f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.62f, 0.58f, 0.50f, 0.10f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.54f, 0.64f, 0.74f, 0.12f);
                default:
                    return new Color(0.62f, 0.65f, 0.68f, 0.07f);
            }
        }

        private static Color GetMidCloudColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.20f, 0.24f, 0.28f, 0.13f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.23f, 0.20f, 0.17f, 0.15f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.18f, 0.23f, 0.30f, 0.17f);
                default:
                    return new Color(0.24f, 0.26f, 0.28f, 0.10f);
            }
        }

        private static Color GetMidRuinColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.10f, 0.12f, 0.15f, 0.13f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.15f, 0.12f, 0.09f, 0.17f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.08f, 0.10f, 0.14f, 0.19f);
                default:
                    return new Color(0.11f, 0.12f, 0.13f, 0.10f);
            }
        }

        private static Color GetForegroundMistColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.50f, 0.56f, 0.62f, 0.07f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.55f, 0.52f, 0.46f, 0.08f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.48f, 0.58f, 0.68f, 0.10f);
                default:
                    return new Color(0.54f, 0.58f, 0.60f, 0.05f);
            }
        }

        private static Color GetFrontFogAColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.64f, 0.68f, 0.74f, 0.09f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.66f, 0.61f, 0.52f, 0.11f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.58f, 0.68f, 0.80f, 0.13f);
                default:
                    return new Color(0.72f, 0.74f, 0.76f, 0.08f);
            }
        }

        private static Color GetFrontFogBColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.22f, 0.25f, 0.29f, 0.10f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.26f, 0.22f, 0.18f, 0.12f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.18f, 0.22f, 0.28f, 0.14f);
                default:
                    return new Color(0.26f, 0.27f, 0.28f, 0.08f);
            }
        }

        private static Color GetInkVeilColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.05f, 0.06f, 0.08f, 0.07f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.07f, 0.06f, 0.05f, 0.10f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.04f, 0.05f, 0.07f, 0.13f);
                default:
                    return new Color(0.05f, 0.05f, 0.06f, 0.05f);
            }
        }

        private static Color GetNearCliffColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.07f, 0.08f, 0.10f, 0.48f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.09f, 0.08f, 0.06f, 0.54f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.06f, 0.07f, 0.09f, 0.60f);
                default:
                    return new Color(0.08f, 0.08f, 0.09f, 0.42f);
            }
        }

        private static Color GetPlatformGlowColor(EncounterVisualTier tier, float pulse)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.50f, 0.68f, 0.78f, 0.05f + pulse * 0.02f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.82f, 0.62f, 0.32f, 0.07f + pulse * 0.02f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.54f, 0.76f, 0.96f, 0.08f + pulse * 0.03f);
                default:
                    return new Color(0.78f, 0.60f, 0.32f, 0.05f);
            }
        }

        private void UpdateBossAtmosphere()
        {
            if (!controller.Battle.IsBossBattle)
            {
                EncounterVisualTier encounterTier = GetEncounterVisualTier();
                intentText.text = BuildEnemyIntentLabel();
                switch (encounterTier)
                {
                    case EncounterVisualTier.Elite:
                        thunderOverlay.color = new Color(0.28f, 0.38f, 0.48f, 0.028f + Mathf.Sin(elapsed * 1.1f) * 0.008f);
                        brushOverlay.color = new Color(0.07f, 0.08f, 0.10f, 0.025f);
                        break;
                    case EncounterVisualTier.MiniBoss:
                        thunderOverlay.color = new Color(0.40f, 0.38f, 0.26f, 0.045f + Mathf.Sin(elapsed * 1.0f) * 0.012f);
                        brushOverlay.color = new Color(0.08f, 0.07f, 0.06f, 0.045f);
                        break;
                    default:
                        thunderOverlay.color = new Color(0.36f, 0.48f, 0.70f, 0f);
                        brushOverlay.color = new Color(0.07f, 0.08f, 0.10f, 0f);
                        break;
                }

                if (bossPortrait != null)
                {
                    bossPortrait.color = new Color(0.74f, 0.80f, 0.92f, 0f);
                }
                return;
            }

            intentText.text = BuildEnemyIntentLabel();

            float alpha;
            switch (controller.Battle.BossPhase)
            {
                case DemoBossPhase.ThunderCloud:
                    alpha = 0.05f;
                    break;
                case DemoBossPhase.SoulLock:
                    alpha = 0.10f;
                    break;
                case DemoBossPhase.CalamityDescends:
                    alpha = 0.15f;
                    break;
                default:
                    alpha = 0f;
                    break;
            }

            thunderOverlay.color = new Color(0.34f, 0.46f, 0.68f, alpha + Mathf.Sin(elapsed * 1.3f) * 0.015f);
            brushOverlay.color = new Color(0.08f, 0.09f, 0.12f, alpha * 0.38f);

            if (bossPortrait != null)
            {
                float portraitAlpha = alpha switch
                {
                    0.05f => 0.16f,
                    0.10f => 0.24f,
                    0.15f => 0.34f,
                    _ => 0.12f
                };

                bossPortrait.color = new Color(0.74f, 0.80f, 0.92f, portraitAlpha + Mathf.Sin(elapsed * 0.9f) * 0.02f);
                bossPortrait.rectTransform.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.42f) * 8f, Mathf.Cos(elapsed * 0.33f) * 6f);
            }
        }

        private void UpdateEncounterPresentation()
        {
            if (controller == null || !controller.HasBattle || enemyRoot == null)
            {
                return;
            }

            EncounterVisualTier encounterTier = GetEncounterVisualTier();
            if (enemyLabelText != null)
            {
                enemyLabelText.text = IsOpeningBattlePage()
                    ? string.Empty
                    : GetEncounterTierLabel(encounterTier);
                enemyLabelText.color = GetEncounterLabelColor(encounterTier);
            }

            if (enemyAuraImage != null)
            {
                enemyAuraImage.color = GetEncounterAuraColor(encounterTier);
            }

            if (enemyBodyImage != null)
            {
                enemyBodyImage.color = GetEncounterRobeColor(encounterTier);
            }

            if (enemySwordImage != null)
            {
                enemySwordImage.color = GetEncounterSwordColor(encounterTier);
            }

            if (enemySwordTrailImage != null)
            {
                Color swordColor = GetEncounterSwordColor(encounterTier);
                enemySwordTrailImage.color = new Color(swordColor.r, swordColor.g, swordColor.b, GetEncounterTrailAlpha(encounterTier));
            }

            if (enemyStatusPanel != null)
            {
                Image enemyPanelImage = enemyStatusPanel.GetComponent<Image>();
                if (enemyPanelImage != null)
                {
                    Color enemySurface = GetEncounterPanelColor(encounterTier);
                    enemyPanelImage.color = new Color(enemySurface.r, enemySurface.g, enemySurface.b, 0.36f);
                }
            }

            if (intentPanel != null)
            {
                Image intentPanelImage = intentPanel.GetComponent<Image>();
                if (intentPanelImage != null)
                {
                    Color intentSurface = GetEncounterIntentPanelColor(encounterTier);
                    intentPanelImage.color = new Color(intentSurface.r, intentSurface.g, intentSurface.b, 0.34f);
                }
            }

            enemyRoot.localScale = Vector3.one * GetEncounterScale(encounterTier);
        }

        private void UpdateHudPanels()
        {
            if (controller == null || controller.Battle.Player == null || controller.Battle.Enemy == null)
            {
                return;
            }

            int playerHealth = controller.Battle.Player.Health;
            int playerMaxHealth = Mathf.Max(1, controller.Battle.Player.MaxHealth);
            int enemyHealth = controller.Battle.Enemy.Health;
            int enemyMaxHealth = Mathf.Max(1, controller.Battle.Enemy.MaxHealth);
            int energy = controller.Battle.Energy;
            int maxEnergy = Mathf.Max(1, controller.Battle.MaxEnergy);
            bool fullEnergy = energy >= maxEnergy;
            float energyPulse = fullEnergy ? 0.5f + Mathf.Sin(elapsed * 4.2f) * 0.5f : 0f;

            playerNameText.text = "凌云剑修";
            playerResourceText.text = $"{playerHealth}/{playerMaxHealth}   灵 {energy}/{maxEnergy} +{controller.Battle.EnergyRegenerationPerSecond:0.#}/秒";
            playerStatusText.text = $"盾 {controller.Battle.Player.Block}   剑意 {controller.Battle.Player.SwordIntent}   飞剑 {controller.Battle.TotalSwords}（临 {controller.Battle.TemporarySwords}）";
            SetHorizontalFill(playerHealthFillImage, playerHealth / (float)playerMaxHealth);
            SetHorizontalFill(playerEnergyFillImage, controller.Battle.EnergyExact / maxEnergy);
            playerEnergyFillImage.color = fullEnergy
                ? new Color(0.66f, 0.93f, 1f, 1f)
                : new Color(0.46f, 0.83f, 0.95f, 0.96f);
            if (playerEnergyGlowImage != null)
            {
                playerEnergyGlowImage.color = fullEnergy
                    ? new Color(0.70f, 0.94f, 1f, 0.18f + energyPulse * 0.18f)
                    : new Color(0.48f, 0.82f, 0.92f, 0.04f);
            }

            enemyNameText.text = controller.Battle.Enemy.Name;
            enemyResourceText.text = $"{enemyHealth}/{enemyMaxHealth}";
            enemyStatusText.text = controller.Battle.IsBossBattle
                ? $"阶段 {GetBossPhaseLabel(controller.Battle.BossPhase)}   {GetBossShortIntent()}"
                : $"感电 {controller.Battle.Enemy.Shock}   流血 {controller.Battle.Enemy.Bleed}   {GetEncounterStatusLine(GetEncounterVisualTier())}";
            SetHorizontalFill(enemyHealthFillImage, enemyHealth / (float)enemyMaxHealth);

            SetHorizontalFill(intentProgressFillImage, controller.Battle.EnemyIntentProgress);

            roundStatusText.text = $"手 {controller.Battle.Hand.Count}  牌 {controller.Battle.DrawPile.Count}  弃 {controller.Battle.DiscardPile.Count}  ·  抽 {Mathf.Max(0f, controller.Battle.DrawTimer):0.0}s";
        }
        private string BuildEnemyIntentLabel()
        {
            string intent = controller.Battle.IsBossBattle
                ? GetBossShortIntent()
                : controller.Battle.EnemyIntentText;
            if (string.IsNullOrWhiteSpace(intent))
            {
                intent = GetEncounterIntentText(GetEncounterVisualTier());
            }

            return $"{(controller.Battle.IsBossBattle ? "天劫预警" : "敌方意图")}：{intent} · {Mathf.Max(0f, controller.Battle.EnemyIntentRemaining):0.0}s";
        }

        private void UpdateIdleStage()
        {
            if (sequenceCoroutine != null)
            {
                StopCoroutine(sequenceCoroutine);
                sequenceCoroutine = null;
            }

            SetCombatVisualState(false);
            phaseText.color = new Color(0.84f, 0.72f, 0.44f, 1f);
            phaseText.text = GetIdlePhaseTitle();
            idleTitleText.text = controller.Run.Map.CurrentNode.Name;
            idleBodyText.text = GetIdleBodyText();
            flashOverlay.color = new Color(1f, 1f, 1f, 0f);
            thunderOverlay.color = new Color(0.36f, 0.48f, 0.70f, 0f);
            brushOverlay.color = new Color(0.07f, 0.08f, 0.10f, 0f);
        }

        private void SetCombatVisualState(bool activeBattle)
        {
            bool showIdleOverlay = ShouldShowIdlePanel();
            playerRoot.gameObject.SetActive(activeBattle);
            enemyRoot.gameObject.SetActive(activeBattle);
            playerStatusPanel.gameObject.SetActive(activeBattle);
            enemyStatusPanel.gameObject.SetActive(activeBattle);
            roundStatusPanel.gameObject.SetActive(activeBattle);
            intentPanel.gameObject.SetActive(activeBattle);
            idlePanel.gameObject.SetActive(!activeBattle && showIdleOverlay);

            if (phaseText != null && phaseText.transform.parent != null)
            {
                phaseText.transform.parent.gameObject.SetActive(activeBattle || showIdleOverlay);
            }

            if (!activeBattle && bossPortrait != null)
            {
                bossPortrait.color = new Color(0.74f, 0.80f, 0.92f, 0f);
            }
        }

        private bool ShouldShowIdlePanel()
        {
            return controller != null
                && (!controller.HasBattle)
                && !(controller.Run.Map.CurrentNode.Type == DemoNodeType.Start);
        }

        private string GetIdlePhaseTitle()
        {
            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "入境试锋：先让首境选择兑现成第一场斗法";
                case DemoNodeType.RouteChoice:
                    return "择前路：下一段历练由你自己拐出来";
                case DemoNodeType.Reward:
                    return "战后补强：把散件收束成真正的 build";
                case DemoNodeType.Training:
                    return "修炼节点：功法与法器正在改写规则";
                case DemoNodeType.Shop:
                    return "Boss 前整备：把最后的爆发窗口留给天劫";
                case DemoNodeType.Victory:
                    return "劫后回望：这一局的道已经立住了";
                default:
                    return "云海静候：下一场斗法尚未开始";
            }
        }

        private string GetIdleBodyText()
        {
            switch (controller.Run.Map.CurrentNode.Type)
            {
                case DemoNodeType.Start:
                    return "先定主修，战斗牌和后续掉落才会开始朝同一条道途收束。";
                case DemoNodeType.RouteChoice:
                    return "路线先决定节点顺序，再决定你是稳稳成型，还是拿着风险去换更高上限。";
                case DemoNodeType.Reward:
                    return "优先挑能改变演武手感的补强，而不只是平铺数值。";
                case DemoNodeType.Training:
                    return "补主修之外的缺口，让飞剑、状态和终结技开始联动。";
                case DemoNodeType.Shop:
                    return "天劫前先补续航或神通上限，别让最后一轮只剩挨打。";
                case DemoNodeType.Victory:
                    return "这一世修行已经完成，可以回看自己最终成型的是哪条道。";
                default:
                    return "真正的斗法开始前，主舞台先负责气氛和方向感。";
            }
        }

        private void UpdatePopups(float deltaTime)
        {
            for (int i = popups.Count - 1; i >= 0; i--)
            {
                FloatingPopup popup = popups[i];
                popup.Age += deltaTime;
                float t = popup.Age / popup.Duration;
                popup.Rect.anchoredPosition += popup.Velocity * deltaTime;
                popup.Text.color = new Color(popup.BaseColor.r, popup.BaseColor.g, popup.BaseColor.b, Mathf.Lerp(1f, 0f, t));

                if (popup.Age >= popup.Duration)
                {
                    Destroy(popup.Text.gameObject);
                    popups.RemoveAt(i);
                }
            }
        }

        private RectTransform CreateEntity(string name, Vector2 normalizedPosition, Color robeColor, Color swordColor, string label, bool faceRight, bool backFacing)
        {
            float facing = faceRight ? 1f : -1f;
            RectTransform root = CreateRect(name, rootRect, new Color(0f, 0f, 0f, 0f), new Vector2(backFacing ? 156f : 132f, backFacing ? 194f : 168f));
            root.anchorMin = new Vector2(0.5f, 0.5f);
            root.anchorMax = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = ScenePoint(normalizedPosition.x, normalizedPosition.y);

            RectTransform aura = CreateRect("Aura", root, new Color(0.45f, 0.56f, 0.64f, backFacing ? 0.05f : 0.08f), new Vector2(backFacing ? 154f : 128f, backFacing ? 108f : 86f));
            aura.anchoredPosition = new Vector2(0f, backFacing ? 28f : 18f);

            RectTransform sword = CreateRect("Sword", root, swordColor, new Vector2(backFacing ? 122f : 96f, backFacing ? 10f : 8f));
            sword.anchoredPosition = backFacing ? new Vector2(44f * facing, 8f) : new Vector2(20f * facing, -12f);
            sword.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -28f * facing : -6f * facing);

            RectTransform trail = CreateRect("SwordTrail", root, new Color(swordColor.r, swordColor.g, swordColor.b, backFacing ? 0.18f : 0.25f), new Vector2(backFacing ? 162f : 132f, backFacing ? 5f : 3f));
            trail.anchoredPosition = backFacing ? new Vector2(2f * facing, 2f) : new Vector2(-12f * facing, -16f);
            trail.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -26f * facing : -6f * facing);

            RectTransform backHair = CreateRect("BackHair", root, new Color(0.10f, 0.12f, 0.15f, 0.92f), new Vector2(backFacing ? 26f : 16f, backFacing ? 86f : 58f));
            backHair.anchoredPosition = backFacing ? new Vector2(-6f * facing, 62f) : new Vector2(-12f * facing, 48f);
            backHair.localRotation = Quaternion.Euler(0f, 0f, backFacing ? 14f * facing : 8f * facing);

            RectTransform body = CreateRect("Body", root, robeColor, new Vector2(backFacing ? 54f : 34f, backFacing ? 104f : 78f));
            body.anchoredPosition = backFacing ? new Vector2(-10f * facing, 46f) : new Vector2(-4f * facing, 30f);
            body.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -18f * facing : -4f * facing);

            RectTransform shoulder = CreateRect("Shoulder", root, new Color(robeColor.r * 0.78f, robeColor.g * 0.80f, robeColor.b * 0.86f, 0.98f), new Vector2(backFacing ? 34f : 18f, backFacing ? 54f : 42f));
            shoulder.anchoredPosition = backFacing ? new Vector2(12f * facing, 50f) : new Vector2(10f * facing, 34f);
            shoulder.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -28f * facing : -18f * facing);

            RectTransform sash = CreateRect("Sash", root, new Color(0.14f, 0.18f, 0.22f, 0.85f), new Vector2(backFacing ? 18f : 12f, backFacing ? 88f : 62f));
            sash.anchoredPosition = backFacing ? new Vector2(8f * facing, 34f) : new Vector2(6f * facing, 26f);
            sash.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -20f * facing : -14f * facing);

            RectTransform sleeve = CreateRect("Sleeve", root, new Color(robeColor.r * 0.92f, robeColor.g * 0.94f, robeColor.b, 0.98f), new Vector2(backFacing ? 26f : 18f, backFacing ? 46f : 34f));
            sleeve.anchoredPosition = backFacing ? new Vector2(28f * facing, 28f) : new Vector2(18f * facing, 20f);
            sleeve.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -36f * facing : -22f * facing);

            RectTransform head = CreateRect("Head", root, new Color(0.92f, 0.89f, 0.82f, 1f), new Vector2(backFacing ? 30f : 22f, backFacing ? 32f : 24f));
            head.anchoredPosition = backFacing ? new Vector2(-6f * facing, 102f) : new Vector2(4f * facing, 78f);

            RectTransform face = CreateRect("Face", root, new Color(0.97f, 0.93f, 0.87f, backFacing ? 0.14f : 0.92f), new Vector2(backFacing ? 14f : 10f, backFacing ? 20f : 16f));
            face.anchoredPosition = backFacing ? new Vector2(6f * facing, 100f) : new Vector2(12f * facing, 78f);

            RectTransform frontHair = CreateRect("FrontHair", root, new Color(0.08f, 0.09f, 0.12f, 0.96f), new Vector2(backFacing ? 20f : 12f, backFacing ? 28f : 20f));
            frontHair.anchoredPosition = backFacing ? new Vector2(-12f * facing, 110f) : new Vector2(0f, 84f);
            frontHair.localRotation = Quaternion.Euler(0f, 0f, backFacing ? -18f * facing : -12f * facing);

            RectTransform noseLine = CreateRect("NoseLine", root, new Color(0.34f, 0.28f, 0.22f, backFacing ? 0.08f : 0.50f), new Vector2(3f, 10f));
            noseLine.anchoredPosition = backFacing ? new Vector2(0f, 102f) : new Vector2(15f * facing, 79f);

            RectTransform gaze = CreateRect("Gaze", root, new Color(0.18f, 0.22f, 0.26f, backFacing ? 0.04f : 0.90f), new Vector2(4f, 4f));
            gaze.anchoredPosition = backFacing ? new Vector2(2f * facing, 103f) : new Vector2(12f * facing, 81f);

            Text tag = CreateText("Label", root, 14, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.90f, 0.82f, 1f));
            tag.text = label;
            tag.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            tag.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            tag.rectTransform.anchoredPosition = new Vector2(0f, backFacing ? -60f : -44f);
            tag.rectTransform.sizeDelta = new Vector2(88f, 20f);

            return root;
        }

        private RectTransform ApplyEntitySprite(RectTransform entityRoot, Sprite sprite, Vector2 size, Vector2 offset)
        {
            RectTransform fallbackBody = entityRoot != null ? entityRoot.Find("Body") as RectTransform : null;
            if (entityRoot == null || sprite == null)
            {
                return fallbackBody;
            }

            HideEntityPart(entityRoot, "BackHair");
            HideEntityPart(entityRoot, "Body");
            HideEntityPart(entityRoot, "Shoulder");
            HideEntityPart(entityRoot, "Sash");
            HideEntityPart(entityRoot, "Sleeve");
            HideEntityPart(entityRoot, "Head");
            HideEntityPart(entityRoot, "Face");
            HideEntityPart(entityRoot, "FrontHair");
            HideEntityPart(entityRoot, "NoseLine");
            HideEntityPart(entityRoot, "Gaze");

            Image art = CreateImage("CharacterArt", entityRoot, Color.white);
            art.sprite = sprite;
            art.type = Image.Type.Simple;
            art.preserveAspect = true;
            art.raycastTarget = false;

            RectTransform artRect = art.rectTransform;
            artRect.anchorMin = new Vector2(0.5f, 0.5f);
            artRect.anchorMax = new Vector2(0.5f, 0.5f);
            artRect.pivot = new Vector2(0.5f, 0.5f);
            artRect.anchoredPosition = offset;
            artRect.sizeDelta = size;
            artRect.SetSiblingIndex(Mathf.Max(0, entityRoot.childCount - 2));
            return artRect;
        }

        private void HideEntityPart(RectTransform entityRoot, string partName)
        {
            Transform child = entityRoot != null ? entityRoot.Find(partName) : null;
            Image image = child != null ? child.GetComponent<Image>() : null;
            if (image == null)
            {
                return;
            }

            Color color = image.color;
            color.a = 0f;
            image.color = color;
        }

        private void CreateAmbientField()
        {
            ambientDrifts.Clear();

            for (int i = 0; i < 6; i++)
            {
                AddAmbientDrift(
                    "CloudWisp_" + i,
                    new Color(0.66f, 0.72f, 0.78f, 0.06f),
                    new Vector2(180f + i * 18f, 22f + i * 3f),
                    new Vector2(0.14f + i * 0.13f, 0.69f - i * 0.05f),
                    new Vector2(28f + i * 4f, 10f + i * 2f),
                    0.08f + i * 0.018f,
                    i * 0.7f,
                    0.8f + i * 0.11f,
                    i % 2 == 0 ? -8f : 7f);
            }

            for (int i = 0; i < 7; i++)
            {
                AddAmbientDrift(
                    "WarmEmber_" + i,
                    new Color(0.88f, 0.46f, 0.22f, 0.12f),
                    new Vector2(6f + i, 6f + i),
                    new Vector2(0.18f + i * 0.11f, 0.14f + (i % 3) * 0.05f),
                    new Vector2(14f + i * 2f, 18f + i * 2f),
                    0.28f + i * 0.03f,
                    i * 0.9f,
                    1.2f + i * 0.06f,
                    0f);
            }

            for (int i = 0; i < 4; i++)
            {
                AddAmbientDrift(
                    "InkSpeck_" + i,
                    new Color(0.07f, 0.08f, 0.10f, 0.12f),
                    new Vector2(28f + i * 6f, 28f + i * 6f),
                    new Vector2(0.58f + i * 0.08f, 0.30f + i * 0.07f),
                    new Vector2(8f + i * 3f, 10f + i * 2f),
                    0.12f + i * 0.02f,
                    i * 1.3f,
                    0.7f + i * 0.08f,
                    i % 2 == 0 ? -14f : 11f);
            }
        }

        private void AddAmbientDrift(string name, Color color, Vector2 size, Vector2 originNormalized, Vector2 amplitude, float speed, float phase, float pulse, float rotation)
        {
            Image image = CreateImage(name, rootRect, color);
            RectTransform rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = ScenePoint(originNormalized.x, originNormalized.y);
            rect.localRotation = Quaternion.Euler(0f, 0f, rotation);

            ambientDrifts.Add(new AmbientDrift
            {
                Rect = rect,
                Image = image,
                OriginNormalized = originNormalized,
                Amplitude = amplitude,
                Speed = speed,
                Phase = phase,
                BaseColor = color,
                Pulse = pulse
            });
        }

        private RectTransform CreateBand(string name, Color color, float normalizedY, float normalizedWidth, float height)
        {
            RectTransform band = CreateRect(name, rootRect, color, new Vector2(1800f * normalizedWidth, height));
            band.anchorMin = new Vector2(0.5f, normalizedY);
            band.anchorMax = band.anchorMin;
            band.anchoredPosition = Vector2.zero;
            return band;
        }

        private Image CreateSceneLayer(string name, Sprite sprite, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            Image image = CreateImage(name, rootRect, color);
            image.type = Image.Type.Simple;
            image.sprite = sprite != null ? sprite : whiteSprite;
            image.preserveAspect = false;

            RectTransform rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return image;
        }

        private RectTransform CreateRect(string name, Transform parent, Color color, Vector2 size)
        {
            Image image = CreateImage(name, parent, color);
            RectTransform rect = image.rectTransform;
            rect.sizeDelta = size;
            return rect;
        }

        private RectTransform CreatePanelRect(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            Image image = CreateImage(name, parent, color);
            RectTransform rect = image.rectTransform;
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        private void CreatePanel(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            CreatePanelRect(name, parent, color, anchorMin, anchorMax, offsetMin, offsetMax);
        }

        private void CreateMiniLabel(RectTransform parent, string name, string label, Vector2 anchorMin, Vector2 anchorMax, Color color)
        {
            Text text = CreateText(name, parent, 13, FontStyle.Bold, TextAnchor.MiddleCenter, color);
            text.text = label;
            text.rectTransform.anchorMin = anchorMin;
            text.rectTransform.anchorMax = anchorMax;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
        }

        private Image CreateInlineBar(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Color backgroundColor, Color fillColor, Color edgeColor)
        {
            RectTransform back = CreatePanelRect(name + "Back", parent, backgroundColor, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            CreatePanelRect(name + "Top", back, edgeColor, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -1f), Vector2.zero);
            CreatePanelRect(name + "Bottom", back, edgeColor, new Vector2(0f, 0f), new Vector2(1f, 0f), Vector2.zero, new Vector2(0f, 1f));
            RectTransform fill = CreatePanelRect(name + "Fill", back, fillColor, Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
            return fill.GetComponent<Image>();
        }

        private static void SetHorizontalFill(Image image, float normalized)
        {
            if (image == null)
            {
                return;
            }

            RectTransform rect = image.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(Mathf.Clamp01(normalized), 1f);
            rect.offsetMin = new Vector2(2f, 2f);
            rect.offsetMax = new Vector2(-2f, -2f);
        }

        private string GetBossShortIntent()
        {
            string intent = controller != null && controller.Battle != null ? controller.Battle.BossIntentText : string.Empty;
            if (string.IsNullOrEmpty(intent))
            {
                return "天威将落";
            }

            int separator = intent.IndexOf('：');
            if (separator >= 0 && separator < intent.Length - 1)
            {
                return intent.Substring(0, separator);
            }

            return intent.Length > 8 ? intent.Substring(0, 8) : intent;
        }
        private RectTransform CreateHudPanel(string name, Vector2 anchorMin, Vector2 anchorMax, Color backgroundColor, Color borderColor)
        {
            RectTransform panel = CreatePanelRect(name, rootRect, backgroundColor, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            DecorateHudFrame(panel, borderColor);
            return panel;
        }

        private void ApplyHudSprite(RectTransform panel, Sprite sprite, Color color)
        {
            if (panel == null || sprite == null)
            {
                return;
            }

            Image image = panel.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            image.sprite = sprite;
            image.type = Image.Type.Simple;
            image.preserveAspect = false;
            image.color = color;
            HideFallbackHudDecoration(panel);
        }

        private static void HideFallbackHudDecoration(RectTransform panel)
        {
            string[] decorationNames =
            {
                "Wash", "Top", "Bottom", "Left", "Right",
                "CornerTL", "CornerTR", "CornerBL", "CornerBR"
            };

            for (int i = 0; i < decorationNames.Length; i++)
            {
                Transform decoration = panel.Find(decorationNames[i]);
                if (decoration != null)
                {
                    decoration.gameObject.SetActive(false);
                }
            }
        }

        private Image CreateImage(string name, Transform parent, Color color)
        {
            GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            imageObject.transform.SetParent(parent, false);
            Image image = imageObject.GetComponent<Image>();
            image.sprite = whiteSprite;
            image.type = Image.Type.Sliced;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private Text CreateText(string name, Transform parent, int size, FontStyle style, TextAnchor anchor, Color color)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            Text text = textObject.GetComponent<Text>();
            text.font = uiFont;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = anchor;
            text.color = color;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            return text;
        }

        private Sprite CreateWhiteSprite()
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            texture.SetPixels(new[]
            {
                Color.white, Color.white,
                Color.white, Color.white
            });
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100f);
        }

        private Sprite LoadBattleBackgroundSprite()
        {
            string resourcePath = ResolveBattleBackgroundResourcePath();
            if (resourcePath == BattleThunderMarshEntryResourcePath)
            {
                return LoadSceneLayerSprite(ref cachedBattleThunderMarshEntrySprite, resourcePath);
            }

            if (resourcePath == BattleOldMineEntryResourcePath)
            {
                return LoadSceneLayerSprite(ref cachedBattleOldMineEntrySprite, resourcePath);
            }

            return LoadSceneLayerSprite(ref cachedBattleBackgroundSprite, BattleBackgroundResourcePath);
        }

        private bool IsUsingRegionBattleBackground()
        {
            string resourcePath = ResolveBattleBackgroundResourcePath();
            return resourcePath == BattleThunderMarshEntryResourcePath || resourcePath == BattleOldMineEntryResourcePath;
        }

        private string ResolveBattleBackgroundResourcePath()
        {
            string regionKey = GetOpeningRegionSearchKey();
            if (ContainsAny(regionKey, "region_thunder_marsh", "thunder", "marsh"))
            {
                return BattleThunderMarshEntryResourcePath;
            }

            if (ContainsAny(regionKey, "region_old_mine", "old_mine", "mine"))
            {
                return BattleOldMineEntryResourcePath;
            }

            return BattleBackgroundResourcePath;
        }

        private string GetOpeningRegionSearchKey()
        {
            if (controller == null || controller.Run == null || controller.Run.OpeningSelection == null || controller.Run.OpeningSelection.FirstRegion == null)
            {
                return string.Empty;
            }

            DemoRegionDefinition region = controller.Run.OpeningSelection.FirstRegion;
            return ((region.Id ?? string.Empty) + "|" + (region.Name ?? string.Empty) + "|" + (region.Description ?? string.Empty)).ToLowerInvariant();
        }

        private static bool ContainsAny(string value, params string[] needles)
        {
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            for (int i = 0; i < needles.Length; i++)
            {
                string needle = needles[i];
                if (!string.IsNullOrEmpty(needle) && value.Contains(needle.ToLowerInvariant()))
                {
                    return true;
                }
            }

            return false;
        }

        private Sprite LoadSceneLayerSprite(ref Sprite cache, string resourcePath)
        {
            if (cache != null)
            {
                return cache;
            }

            Texture2D texture = Resources.Load<Texture2D>(resourcePath);
            if (texture == null)
            {
                return null;
            }

            cache = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            return cache;
        }

        private Sprite LoadBossPortraitSprite()
        {
            if (cachedBossPortraitSprite != null)
            {
                return cachedBossPortraitSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(BossPortraitFallbackResourcePath);
            if (texture == null)
            {
                texture = Resources.Load<Texture2D>(BossPortraitResourcePath);
            }

            if (texture == null)
            {
                return null;
            }

            cachedBossPortraitSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            return cachedBossPortraitSprite;
        }

        private void StretchText(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private void DecorateHudFrame(RectTransform parent, Color borderColor)
        {
            CreatePanelRect("Wash", parent, new Color(0.19f, 0.15f, 0.10f, 0.10f), new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(5f, 5f), new Vector2(-5f, -5f));
            CreatePanelRect("Top", parent, borderColor, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(8f, -2f), new Vector2(-8f, 0f));
            CreatePanelRect("Bottom", parent, borderColor, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(8f, 0f), new Vector2(-8f, 2f));
            CreatePanelRect("Left", parent, borderColor, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 8f), new Vector2(2f, -8f));
            CreatePanelRect("Right", parent, borderColor, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-2f, 8f), new Vector2(0f, -8f));
            CreatePanelRect("CornerTL", parent, borderColor, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -18f), new Vector2(18f, 0f));
            CreatePanelRect("CornerTR", parent, borderColor, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-18f, -18f), new Vector2(0f, 0f));
            CreatePanelRect("CornerBL", parent, borderColor, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(18f, 18f));
            CreatePanelRect("CornerBR", parent, borderColor, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-18f, 0f), new Vector2(0f, 18f));
        }

        private Vector2 GetSwordTip(RectTransform sword, bool rightSide)
        {
            float direction = rightSide ? 1f : -1f;
            return sword.parent.GetComponent<RectTransform>().anchoredPosition + sword.anchoredPosition + new Vector2(direction * sword.sizeDelta.x * 0.5f, 0f);
        }

        private Vector2 ScenePoint(float normalizedX, float normalizedY)
        {
            return new Vector2(
                (normalizedX - 0.5f) * rootRect.rect.width,
                (normalizedY - 0.5f) * rootRect.rect.height);
        }

        private Color GetImpactColor(DemoBattlePresentationStep step)
        {
            if (step.TriggerShock)
            {
                return new Color(0.57f, 0.82f, 1f, 1f);
            }

            if (step.TriggerBleed)
            {
                return new Color(0.90f, 0.36f, 0.35f, 1f);
            }

            return GetStyleColor(step.Style);
        }

        private Color GetStyleColor(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new Color(0.86f, 0.92f, 0.98f, 1f);
                case DemoSwordStyle.Thunder:
                    return new Color(0.56f, 0.78f, 1f, 1f);
                case DemoSwordStyle.Blood:
                    return new Color(0.89f, 0.34f, 0.35f, 1f);
                default:
                    return new Color(0.92f, 0.82f, 0.60f, 1f);
            }
        }

        private bool IsOpeningStrikePass(DemoBattlePresentationStep step)
        {
            return controller != null
                && controller.HasBattle
                && controller.Battle.IsOpeningBattlePacing
                && controller.Battle.ElapsedSeconds <= 8f
                && step != null
                && step.HitCount <= 1
                && (step.Type == DemoBattlePresentationStepType.CardCast || step.Type == DemoBattlePresentationStepType.SwordVolley);
        }

        private Color GetCardCastWashColor(DemoBattlePresentationStep step)
        {
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Color(0.54f, 0.78f, 1f, step.HeavyImpact ? 0.34f : 0.26f);
                case DemoSwordStyle.Blood:
                    return new Color(0.76f, 0.24f, 0.28f, step.HeavyImpact ? 0.30f : 0.24f);
                case DemoSwordStyle.Wanjian:
                    return new Color(0.88f, 0.92f, 0.98f, step.HeavyImpact ? 0.28f : 0.22f);
                default:
                    return GetStyleColor(step.Style);
            }
        }

        private Vector2 GetCardCastWashOffset(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector2(-2f, 28f);
                case DemoSwordStyle.Blood:
                    return new Vector2(-12f, 18f);
                default:
                    return new Vector2(-8f, 22f);
            }
        }

        private float GetCardCastWashSize(DemoBattlePresentationStep step)
        {
            float size;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    size = step.HeavyImpact ? 118f : 92f;
                    break;
                case DemoSwordStyle.Blood:
                    size = step.HeavyImpact ? 122f : 96f;
                    break;
                default:
                    size = step.HeavyImpact ? 110f : 88f;
                    break;
            }

            return IsOpeningStrikePass(step) ? size * 0.86f : size;
        }

        private Vector3 GetCastPunchScale(DemoBattlePresentationStep step)
        {
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector3(1.08f, 1.12f, 1f);
                case DemoSwordStyle.Blood:
                    return new Vector3(1.12f, 1.08f, 1f);
                default:
                    return new Vector3(1.10f, 1.10f, 1f);
            }
        }

        private int GetCardCastVisualHits(DemoBattlePresentationStep step)
        {
            if (IsOpeningStrikePass(step))
            {
                return 1;
            }

            switch (step.Style)
            {
                case DemoSwordStyle.Blood:
                    return Mathf.Clamp(step.HitCount, 1, 2);
                default:
                    return Mathf.Clamp(step.HitCount, 1, 3);
            }
        }

        private Vector2 GetCardCastStartOffset(DemoBattlePresentationStep step, int hitIndex, int visualHits)
        {
            float centeredIndex = hitIndex - (visualHits - 1) * 0.5f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector2(-3f + centeredIndex * 2f, centeredIndex * 4f);
                case DemoSwordStyle.Blood:
                    return new Vector2(-4f, centeredIndex * 3f);
                default:
                    return new Vector2(0f, centeredIndex * 5f);
            }
        }

        private Vector2 GetCardCastEndOffset(DemoBattlePresentationStep step, int hitIndex, int visualHits)
        {
            float centeredIndex = hitIndex - (visualHits - 1) * 0.5f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector2(4f, centeredIndex * 8f);
                case DemoSwordStyle.Blood:
                    return new Vector2(-2f, centeredIndex * 6f);
                default:
                    return new Vector2(0f, centeredIndex * 10f);
            }
        }

        private float GetFlightDuration(DemoBattlePresentationStep step, bool volley)
        {
            float duration;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    duration = volley ? 0.17f : 0.15f;
                    break;
                case DemoSwordStyle.Blood:
                    duration = step.HeavyImpact ? 0.22f : volley ? 0.21f : 0.19f;
                    break;
                default:
                    duration = volley ? 0.20f : 0.18f;
                    break;
            }

            return IsOpeningStrikePass(step) ? duration + 0.02f : duration;
        }

        private float GetFlightThickness(DemoBattlePresentationStep step, bool volley)
        {
            float thickness;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    thickness = volley ? 10f : 12f;
                    break;
                case DemoSwordStyle.Blood:
                    thickness = step.HeavyImpact ? 18f : volley ? 15f : 16f;
                    break;
                default:
                    thickness = volley ? 12f : 14f;
                    break;
            }

            return IsOpeningStrikePass(step) ? Mathf.Max(8f, thickness - 2f) : thickness;
        }

        private Color GetVolleyWashColor(DemoBattlePresentationStep step)
        {
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Color(0.56f, 0.80f, 1f, 0.32f);
                case DemoSwordStyle.Blood:
                    return new Color(0.56f, 0.14f, 0.18f, 0.28f);
                default:
                    return new Color(0.82f, 0.88f, 0.94f, 0.30f);
            }
        }

        private float GetVolleyWashSize(DemoBattlePresentationStep step)
        {
            float size;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    size = step.HeavyImpact ? 154f : 126f;
                    break;
                case DemoSwordStyle.Blood:
                    size = step.HeavyImpact ? 148f : 124f;
                    break;
                default:
                    size = step.HeavyImpact ? 160f : 130f;
                    break;
            }

            return IsOpeningStrikePass(step) ? size * 0.78f : size;
        }

        private int GetVolleyVisualHits(DemoBattlePresentationStep step)
        {
            if (IsOpeningStrikePass(step))
            {
                return 1;
            }

            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return Mathf.Clamp(step.HitCount, 1, 5);
                case DemoSwordStyle.Blood:
                    return Mathf.Clamp(step.HitCount, 1, 4);
                default:
                    return Mathf.Clamp(step.HitCount, 1, 6);
            }
        }

        private Vector2 GetVolleyStartOffset(DemoBattlePresentationStep step, int hitIndex, int visualHits)
        {
            float centeredIndex = hitIndex - (visualHits - 1) * 0.5f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector2(centeredIndex * 4f, centeredIndex * 10f);
                case DemoSwordStyle.Blood:
                    return new Vector2(centeredIndex * 3f, centeredIndex * 7f - 4f);
                default:
                    return new Vector2(0f, centeredIndex * 6f);
            }
        }

        private Vector2 GetVolleyEndOffset(DemoBattlePresentationStep step, int hitIndex, int visualHits)
        {
            float centeredIndex = hitIndex - (visualHits - 1) * 0.5f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Vector2(6f, centeredIndex * 13f);
                case DemoSwordStyle.Blood:
                    return new Vector2(-3f, centeredIndex * 10f);
                default:
                    return new Vector2(0f, centeredIndex * 10f);
            }
        }

        private Color GetVolleyFlightColor(DemoBattlePresentationStep step)
        {
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    return new Color(0.74f, 0.90f, 1f, 0.96f);
                case DemoSwordStyle.Blood:
                    return new Color(0.88f, 0.32f, 0.34f, 0.92f);
                default:
                    return new Color(0.83f, 0.91f, 0.98f, 0.95f);
            }
        }

        private float GetVolleyGap(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    return 0.05f;
                case DemoSwordStyle.Blood:
                    return 0.08f;
                default:
                    return 0.06f;
            }
        }

        private float GetImpactWashSize(DemoBattlePresentationStep step)
        {
            float size;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    size = step.HeavyImpact ? 118f : 80f;
                    break;
                case DemoSwordStyle.Blood:
                    size = step.HeavyImpact ? 136f : 92f;
                    break;
                default:
                    size = step.HeavyImpact ? 126f : 84f;
                    break;
            }

            return IsOpeningStrikePass(step) ? size * 0.88f : size;
        }

        private float GetImpactScale(DemoBattlePresentationStep step)
        {
            float scale;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    scale = step.HeavyImpact ? 1.22f : 1.10f;
                    break;
                case DemoSwordStyle.Blood:
                    scale = step.HeavyImpact ? 1.30f : 1.15f;
                    break;
                default:
                    scale = step.HeavyImpact ? 1.26f : 1.12f;
                    break;
            }

            return IsOpeningStrikePass(step) ? Mathf.Max(1.05f, scale - 0.04f) : scale;
        }

        private float GetImpactPush(DemoBattlePresentationStep step)
        {
            float push;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    push = step.HeavyImpact ? 14f : 7f;
                    break;
                case DemoSwordStyle.Blood:
                    push = step.HeavyImpact ? 20f : 11f;
                    break;
                default:
                    push = step.HeavyImpact ? 18f : 9f;
                    break;
            }

            return IsOpeningStrikePass(step) ? Mathf.Max(5f, push - 2f) : push;
        }

        private IEnumerator PlayCardCastPrelude(DemoBattlePresentationStep step)
        {
            float sizeScale = IsOpeningStrikePass(step) ? 0.82f : 1f;
            float durationScale = IsOpeningStrikePass(step) ? 0.86f : 1f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    yield return SpawnDelayedLightningFork(playerRoot.anchoredPosition + new Vector2(28f, 48f), 2, 0f, 0.12f * durationScale);
                    break;
                case DemoSwordStyle.Blood:
                    yield return SpawnDelayedTransientMark(
                        playerRoot.anchoredPosition + new Vector2(18f, 18f),
                        new Vector2(48f, 6f) * sizeScale,
                        new Color(0.74f, 0.16f, 0.20f, 0.24f),
                        0f,
                        0.18f * durationScale,
                        -22f);
                    break;
                case DemoSwordStyle.Wanjian:
                    yield return SpawnDelayedTransientMark(
                        playerRoot.anchoredPosition + new Vector2(16f, 26f),
                        new Vector2(60f, 3f) * sizeScale,
                        new Color(0.94f, 0.90f, 0.82f, 0.22f),
                        0f,
                        0.16f * durationScale,
                        -12f);
                    break;
            }
        }

        private IEnumerator PlayVolleyPrelude(DemoBattlePresentationStep step)
        {
            float sizeScale = IsOpeningStrikePass(step) ? 0.80f : 1f;
            float durationScale = IsOpeningStrikePass(step) ? 0.88f : 1f;
            switch (step.Style)
            {
                case DemoSwordStyle.Thunder:
                    yield return SpawnDelayedLightningFork(ScenePoint(0.40f, 0.55f), IsOpeningStrikePass(step) ? 2 : 3, 0f, 0.14f * durationScale);
                    break;
                case DemoSwordStyle.Blood:
                    yield return SpawnDelayedTransientMark(
                        ScenePoint(0.44f, 0.48f),
                        new Vector2(120f, 10f) * sizeScale,
                        new Color(0.62f, 0.12f, 0.16f, 0.18f),
                        0f,
                        0.20f * durationScale,
                        -20f);
                    break;
                case DemoSwordStyle.Wanjian:
                    yield return SpawnDelayedTransientMark(
                        ScenePoint(0.47f, 0.49f),
                        new Vector2(136f, 4f) * sizeScale,
                        new Color(0.96f, 0.94f, 0.88f, 0.18f),
                        0f,
                        0.18f * durationScale,
                        -10f);
                    break;
            }
        }

        private bool IsOpeningBattlePage()
        {
            if (controller == null || !controller.HasBattle)
            {
                return false;
            }

            DemoMapNode currentNode = controller.Run.Map.CurrentNode;
            return currentNode.Type == DemoNodeType.Battle
                && currentNode.Layer == 1
                && controller.Run.OpeningSelection.FirstRegion != null;
        }

        private string GetRoundPhaseLabel()
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

        private EncounterVisualTier GetEncounterVisualTier()
        {
            if (controller == null || !controller.HasBattle)
            {
                return EncounterVisualTier.Minor;
            }

            if (controller.Battle.IsBossBattle || controller.Run.Map.CurrentNode.Type == DemoNodeType.Boss)
            {
                return EncounterVisualTier.FinalBoss;
            }

            DemoMapNode currentNode = controller.Run.Map.CurrentNode;
            if (currentNode.Type != DemoNodeType.Battle)
            {
                return EncounterVisualTier.Minor;
            }

            if (currentNode.Layer >= 3 || IsMiniBossNodeName(currentNode.Name))
            {
                return EncounterVisualTier.MiniBoss;
            }

            if (currentNode.Layer >= 2)
            {
                return EncounterVisualTier.Elite;
            }

            return EncounterVisualTier.Minor;
        }

        private static bool IsMiniBossNodeName(string nodeName)
        {
            return !string.IsNullOrEmpty(nodeName)
                && (nodeName.Contains("守门")
                    || nodeName.Contains("守卫")
                    || nodeName.Contains("执兵")
                    || nodeName.Contains("试炼"));
        }

        private static string GetEncounterTierLabel(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return "当前为精英斗法";
                case EncounterVisualTier.MiniBoss:
                    return "当前为守关小 Boss";
                case EncounterVisualTier.FinalBoss:
                    return "当前为终局 Boss";
                default:
                    return "当前为常规斗法";
            }
        }

        private static string GetEncounterStatusLine(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return "精英压制：敌势更稳";
                case EncounterVisualTier.MiniBoss:
                    return "守关压迫：先破势再爆发";
                case EncounterVisualTier.FinalBoss:
                    return "天劫压境：检验整局构筑";
                default:
                    return "常规斗法：试锋与补节奏";
            }
        }

        private static string GetEncounterIntentText(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return "精英压制：先拆势，再收束";
                case EncounterVisualTier.MiniBoss:
                    return "守关压迫：检验构筑成型";
                case EncounterVisualTier.FinalBoss:
                    return "天劫压境：等爆发窗口";
                default:
                    return "常规斗法：试锋补节奏";
            }
        }

        private static float GetEncounterScale(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return 1.05f;
                case EncounterVisualTier.MiniBoss:
                    return 1.12f;
                case EncounterVisualTier.FinalBoss:
                    return 1.20f;
                default:
                    return 0.96f;
            }
        }

        private static Color GetEncounterAuraColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.48f, 0.56f, 0.62f, 0.11f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.56f, 0.48f, 0.32f, 0.14f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.58f, 0.66f, 0.76f, 0.16f);
                default:
                    return new Color(0.34f, 0.40f, 0.46f, 0.08f);
            }
        }

        private static Color GetEncounterRobeColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.86f, 0.90f, 0.94f, 1f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.92f, 0.88f, 0.82f, 1f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.92f, 0.93f, 0.96f, 1f);
                default:
                    return new Color(0.78f, 0.82f, 0.86f, 1f);
            }
        }

        private static Color GetEncounterSwordColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.54f, 0.72f, 0.84f, 1f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.78f, 0.66f, 0.46f, 1f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.64f, 0.82f, 0.96f, 1f);
                default:
                    return new Color(0.42f, 0.58f, 0.70f, 1f);
            }
        }

        private static float GetEncounterTrailAlpha(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return 0.28f;
                case EncounterVisualTier.MiniBoss:
                    return 0.24f;
                case EncounterVisualTier.FinalBoss:
                    return 0.30f;
                default:
                    return 0.18f;
            }
        }

        private static Color GetEncounterLabelColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.95f, 0.86f, 0.70f, 1f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.90f, 0.92f, 0.96f, 1f);
                default:
                    return new Color(0.90f, 0.89f, 0.84f, 1f);
            }
        }

        private static Color GetEncounterPanelColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.11f, 0.13f, 0.16f, 0.76f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.16f, 0.12f, 0.09f, 0.78f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.12f, 0.10f, 0.10f, 0.72f);
                default:
                    return new Color(0.10f, 0.11f, 0.13f, 0.72f);
            }
        }

        private static Color GetEncounterIntentPanelColor(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.10f, 0.13f, 0.16f, 0.80f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(0.16f, 0.12f, 0.08f, 0.82f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.10f, 0.11f, 0.16f, 0.84f);
                default:
                    return new Color(0.11f, 0.11f, 0.14f, 0.78f);
            }
        }

        private static Color GetEncounterPanelTint(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.88f, 0.95f, 1f, 1f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(1f, 0.92f, 0.78f, 1f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(1f, 0.86f, 0.82f, 1f);
                default:
                    return Color.white;
            }
        }

        private static Color GetEncounterIntentPanelTint(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return new Color(0.86f, 0.96f, 1f, 1f);
                case EncounterVisualTier.MiniBoss:
                    return new Color(1f, 0.94f, 0.80f, 1f);
                case EncounterVisualTier.FinalBoss:
                    return new Color(0.90f, 0.92f, 1f, 1f);
                default:
                    return Color.white;
            }
        }

        private static string GetBossPhaseLabel(DemoBossPhase phase)
        {
            switch (phase)
            {
                case DemoBossPhase.ThunderCloud:
                    return "雷云";
                case DemoBossPhase.SoulLock:
                    return "锁魂";
                case DemoBossPhase.CalamityDescends:
                    return "天劫";
                default:
                    return "无";
            }
        }

        private Vector2 GetPlayerAnchor()
        {
            return new Vector2(0.20f, 0.345f);
        }

        private Vector2 GetEnemyAnchor()
        {
            EncounterVisualTier tier = GetEncounterVisualTier();
            switch (tier)
            {
                case EncounterVisualTier.FinalBoss:
                    return new Vector2(0.78f, 0.50f);
                case EncounterVisualTier.MiniBoss:
                    return new Vector2(0.78f, 0.48f);
                case EncounterVisualTier.Elite:
                    return new Vector2(0.79f, 0.47f);
                default:
                    return new Vector2(0.79f, 0.46f);
            }
        }
    }
}
