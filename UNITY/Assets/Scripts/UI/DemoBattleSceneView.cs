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
        private const string BossPortraitResourcePath = "Art/Boss/boss_tianjie_halfbody_001";
        private static readonly Color HudPaper = new Color(0.94f, 0.93f, 0.89f, 1f);
        private static readonly Color HudMist = new Color(0.63f, 0.67f, 0.72f, 1f);
        private static readonly Color HudGold = new Color(0.85f, 0.72f, 0.44f, 1f);
        private static readonly Color HudGoldDim = new Color(0.52f, 0.41f, 0.22f, 0.88f);
        private static readonly Color HudInk = new Color(0.12f, 0.12f, 0.12f, 0.90f);
        private static readonly Color HudJade = new Color(0.43f, 0.63f, 0.72f, 1f);
        private static readonly Color HudCrimson = new Color(0.74f, 0.34f, 0.31f, 1f);
        private static Sprite cachedBattleBackgroundSprite;
        private static Sprite cachedBossPortraitSprite;

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

        private DemoGameController controller;
        private Font uiFont;
        private Sprite whiteSprite;
        private Sprite battleBackgroundSprite;
        private Sprite bossPortraitSprite;

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
        private RectTransform enemyRoot;
        private RectTransform enemySword;
        private RectTransform enemyBody;
        private RectTransform playerStatusPanel;
        private RectTransform enemyStatusPanel;
        private RectTransform roundStatusPanel;
        private RectTransform intentPanel;
        private RectTransform idlePanel;
        private Text playerStatusText;
        private Text enemyStatusText;
        private Text roundStatusText;
        private Text phaseText;
        private Text intentText;
        private Text idleTitleText;
        private Text idleBodyText;
        private Image flashOverlay;
        private Image thunderOverlay;
        private Image brushOverlay;
        private Image bossPortrait;

        private readonly List<AmbientDrift> ambientDrifts = new List<AmbientDrift>();
        private readonly List<FloatingPopup> popups = new List<FloatingPopup>();
        private readonly List<Image> transientMarks = new List<Image>();

        private Coroutine sequenceCoroutine;
        private int lastSequenceVersion = -1;
        private float elapsed;

        public void Initialize(DemoGameController demoController, Font font)
        {
            controller = demoController;
            uiFont = font;
            whiteSprite = CreateWhiteSprite();
            battleBackgroundSprite = LoadBattleBackgroundSprite();
            bossPortraitSprite = LoadBossPortraitSprite();
            BuildScene();
        }

        private void Update()
        {
            if (controller == null || rootRect == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            UpdateAmbientMotion();
            UpdatePopups(Time.deltaTime);

            if (!controller.HasBattle)
            {
                UpdateIdleStage();
                return;
            }

            SetCombatVisualState(true);

            if (controller.Battle.ExecutionSequenceVersion != lastSequenceVersion &&
                controller.Battle.CurrentPresentationSteps.Count > 0)
            {
                lastSequenceVersion = controller.Battle.ExecutionSequenceVersion;

                if (sequenceCoroutine != null)
                {
                    StopCoroutine(sequenceCoroutine);
                }

                sequenceCoroutine = StartCoroutine(PlaySequence(controller.Battle.CurrentPresentationSteps));
            }

            if (controller.Battle.Phase == DemoBattlePhase.Planning)
            {
                phaseText.text = "规划阶段：御剑悬空，蓄势待发";
                phaseText.color = new Color(0.80f, 0.87f, 0.92f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Executing)
            {
                phaseText.text = "演武阶段：剑势既出，诸法自行";
                phaseText.color = new Color(0.94f, 0.83f, 0.55f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Won)
            {
                phaseText.text = "敌势已破，道途更进一步";
                phaseText.color = new Color(0.96f, 0.88f, 0.54f, 1f);
            }
            else if (controller.Battle.Phase == DemoBattlePhase.Lost)
            {
                phaseText.text = "剑势尽散，道心受挫";
                phaseText.color = new Color(0.93f, 0.50f, 0.43f, 1f);
            }

            UpdateBossAtmosphere();
            UpdateHudPanels();
        }

        private void BuildScene()
        {
            rootRect = gameObject.GetComponent<RectTransform>();
            rootRect.gameObject.AddComponent<RectMask2D>();

            CreateBackground();
            CreateActors();
            CreateOverlay();
        }

        private void CreateBackground()
        {
            Image backdrop = CreateImage("Backdrop", rootRect, battleBackgroundSprite != null
                ? new Color(0.86f, 0.88f, 0.90f, 1f)
                : new Color(0.90f, 0.90f, 0.88f, 1f));
            backdrop.type = Image.Type.Simple;
            backdrop.sprite = battleBackgroundSprite != null ? battleBackgroundSprite : whiteSprite;
            backdrop.rectTransform.anchorMin = Vector2.zero;
            backdrop.rectTransform.anchorMax = Vector2.one;
            backdrop.rectTransform.offsetMin = Vector2.zero;
            backdrop.rectTransform.offsetMax = Vector2.zero;

            CreatePanel("BackdropPaper", rootRect, new Color(1f, 1f, 1f, battleBackgroundSprite != null ? 0.16f : 0.24f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            CreatePanel("BackdropShade", rootRect, new Color(0.05f, 0.06f, 0.07f, battleBackgroundSprite != null ? 0.18f : 0.06f), Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            skyGlow = CreatePanelRect("GlowTop", rootRect, new Color(0.78f, 0.82f, 0.84f, 0.32f), new Vector2(0f, 0.58f), new Vector2(1f, 1f), Vector2.zero, Vector2.zero);
            horizonGlow = CreatePanelRect("HorizonGlow", rootRect, new Color(0.22f, 0.20f, 0.18f, 0.06f), new Vector2(0f, 0.08f), new Vector2(1f, 0.34f), Vector2.zero, Vector2.zero);

            // Far scene: distant ridges and high cloud flow.
            farCloudA = CreateBand("FarCloudA", new Color(0.28f, 0.31f, 0.34f, 0.10f), 0.76f, 1.25f, 86f);
            farCloudB = CreateBand("FarCloudB", new Color(0.36f, 0.38f, 0.40f, 0.09f), 0.63f, 1.10f, 64f);
            farRidgeA = CreateBand("FarRidgeA", new Color(0.14f, 0.15f, 0.17f, 0.16f), 0.45f, 1.12f, 94f);
            farRidgeB = CreateBand("FarRidgeB", new Color(0.18f, 0.19f, 0.21f, 0.12f), 0.39f, 1.04f, 68f);

            // Mid scene: floating shelves and cloud band where projectiles travel.
            mistBand = CreateBand("MistBand", new Color(0.62f, 0.65f, 0.68f, 0.07f), 0.20f, 1.16f, 60f);
            midCloudShelf = CreateBand("MidCloudShelf", new Color(0.24f, 0.26f, 0.28f, 0.10f), 0.56f, 1.08f, 126f);
            midRuinBand = CreateBand("MidRuinBand", new Color(0.11f, 0.12f, 0.13f, 0.10f), 0.30f, 1.08f, 42f);
            midIslandLeft = CreateBand("MidIslandLeft", new Color(0.10f, 0.10f, 0.11f, 0.24f), 0.35f, 0.28f, 28f);
            midIslandLeft.anchoredPosition += new Vector2(-420f, 0f);
            midIslandLeft.localRotation = Quaternion.Euler(0f, 0f, -5f);
            midIslandRight = CreateBand("MidIslandRight", new Color(0.10f, 0.10f, 0.11f, 0.28f), 0.58f, 0.28f, 30f);
            midIslandRight.anchoredPosition += new Vector2(450f, 0f);
            midIslandRight.localRotation = Quaternion.Euler(0f, 0f, 7f);

            // Near scene: front mist and cliff lips framing the duel.
            foregroundMist = CreateBand("ForegroundMist", new Color(0.54f, 0.58f, 0.60f, 0.05f), 0.27f, 1.06f, 64f);
            frontFogA = CreateBand("FrontFogA", new Color(0.72f, 0.74f, 0.76f, 0.08f), 0.12f, 1.22f, 92f);
            frontFogB = CreateBand("FrontFogB", new Color(0.26f, 0.27f, 0.28f, 0.08f), 0.17f, 1.08f, 52f);
            inkVeil = CreateBand("InkVeil", new Color(0.05f, 0.05f, 0.06f, 0.05f), 0.58f, 1.26f, 160f);
            nearCliffLeft = CreateBand("NearCliffLeft", new Color(0.08f, 0.08f, 0.09f, 0.44f), 0.03f, 0.28f, 118f);
            nearCliffLeft.anchoredPosition += new Vector2(-560f, 0f);
            nearCliffLeft.localRotation = Quaternion.Euler(0f, 0f, 12f);
            nearCliffRight = CreateBand("NearCliffRight", new Color(0.08f, 0.08f, 0.09f, 0.46f), 0.05f, 0.30f, 126f);
            nearCliffRight.anchoredPosition += new Vector2(586f, 0f);
            nearCliffRight.localRotation = Quaternion.Euler(0f, 0f, -12f);
            nearPlatformGlowLeft = CreateBand("NearPlatformGlowLeft", new Color(0.78f, 0.60f, 0.32f, 0.05f), 0.09f, 0.18f, 18f);
            nearPlatformGlowLeft.anchoredPosition += new Vector2(-472f, 0f);
            nearPlatformGlowRight = CreateBand("NearPlatformGlowRight", new Color(0.78f, 0.60f, 0.32f, 0.05f), 0.09f, 0.18f, 18f);
            nearPlatformGlowRight.anchoredPosition += new Vector2(482f, 0f);

            if (bossPortraitSprite != null)
            {
                bossPortrait = CreateImage("BossPortrait", rootRect, new Color(0.54f, 0.56f, 0.60f, 0f));
                bossPortrait.type = Image.Type.Simple;
                bossPortrait.sprite = bossPortraitSprite;
                bossPortrait.preserveAspect = true;
                bossPortrait.rectTransform.anchorMin = new Vector2(0.57f, 0.06f);
                bossPortrait.rectTransform.anchorMax = new Vector2(0.98f, 0.96f);
                bossPortrait.rectTransform.offsetMin = Vector2.zero;
                bossPortrait.rectTransform.offsetMax = Vector2.zero;
            }

            RectTransform mountainA = CreateBand("MountainA", new Color(0.12f, 0.12f, 0.13f, 0.28f), 0.09f, 1.02f, 58f);
            mountainA.anchoredPosition += new Vector2(-42f, 0f);
            mountainA.localRotation = Quaternion.Euler(0f, 0f, 5f);

            RectTransform mountainB = CreateBand("MountainB", new Color(0.16f, 0.16f, 0.17f, 0.20f), 0.13f, 0.96f, 46f);
            mountainB.anchoredPosition += new Vector2(96f, 0f);
            mountainB.localRotation = Quaternion.Euler(0f, 0f, -4f);

            for (int i = 0; i < 4; i++)
            {
                RectTransform streak = CreateRect("Wind_" + i, rootRect, new Color(0.30f, 0.34f, 0.38f, 0.08f), new Vector2(140f + 28f * i, 2f));
                streak.anchorMin = new Vector2(0.22f + i * 0.16f, 0.72f - i * 0.09f);
                streak.anchorMax = streak.anchorMin;
                streak.anchoredPosition = Vector2.zero;
                streak.localRotation = Quaternion.Euler(0f, 0f, -11f);
            }

            CreateAmbientField();
        }

        private void CreateActors()
        {
            Vector2 playerAnchor = GetPlayerAnchor();
            Vector2 enemyAnchor = GetEnemyAnchor();

            playerRoot = CreateEntity("PlayerRoot", playerAnchor, new Color(0.20f, 0.22f, 0.24f, 1f), new Color(0.62f, 0.69f, 0.74f, 1f), "剑修", true, true);
            playerSword = playerRoot.Find("Sword") as RectTransform;
            playerBody = playerRoot.Find("Body") as RectTransform;

            enemyRoot = CreateEntity("EnemyRoot", enemyAnchor, new Color(0.90f, 0.93f, 0.94f, 1f), new Color(0.50f, 0.74f, 0.92f, 1f), "天劫", false, false);
            enemySword = enemyRoot.Find("Sword") as RectTransform;
            enemyBody = enemyRoot.Find("Body") as RectTransform;
        }

        private void CreateOverlay()
        {
            RectTransform topScroll = CreateHudPanel(
                "TopScroll",
                new Vector2(0.22f, 0.88f),
                new Vector2(0.78f, 0.97f),
                new Color(0.13f, 0.12f, 0.11f, 0.78f),
                new Color(0.40f, 0.34f, 0.22f, 0.62f));
            phaseText = CreateText("PhaseText", topScroll, 19, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.92f, 0.91f, 0.88f, 1f));
            StretchText(phaseText.rectTransform, new Vector2(18f, 10f), new Vector2(-18f, -10f));
            phaseText.text = "规划阶段：御剑悬空，蓄势待发";

            playerStatusPanel = CreateHudPanel(
                "PlayerScroll",
                new Vector2(0.03f, 0.66f),
                new Vector2(0.24f, 0.83f),
                new Color(0.10f, 0.11f, 0.12f, 0.72f),
                new Color(0.38f, 0.34f, 0.26f, 0.54f));
            playerStatusText = CreateText("PlayerStatus", playerStatusPanel, 14, FontStyle.Normal, TextAnchor.UpperLeft, HudMist);
            StretchText(playerStatusText.rectTransform, new Vector2(14f, 12f), new Vector2(-14f, -12f));

            enemyStatusPanel = CreateHudPanel(
                "EnemyScroll",
                new Vector2(0.76f, 0.66f),
                new Vector2(0.97f, 0.83f),
                new Color(0.12f, 0.10f, 0.10f, 0.72f),
                new Color(HudCrimson.r * 0.7f, HudCrimson.g * 0.7f, HudCrimson.b * 0.7f, 0.62f));
            enemyStatusText = CreateText("EnemyStatus", enemyStatusPanel, 14, FontStyle.Normal, TextAnchor.UpperLeft, HudMist);
            StretchText(enemyStatusText.rectTransform, new Vector2(14f, 12f), new Vector2(-14f, -12f));

            roundStatusPanel = CreateHudPanel(
                "RoundSeal",
                new Vector2(0.42f, 0.14f),
                new Vector2(0.58f, 0.21f),
                new Color(0.12f, 0.10f, 0.09f, 0.72f),
                new Color(0.40f, 0.34f, 0.22f, 0.58f));
            roundStatusText = CreateText("RoundStatus", roundStatusPanel, 15, FontStyle.Bold, TextAnchor.MiddleCenter, HudGold);
            StretchText(roundStatusText.rectTransform, new Vector2(12f, 8f), new Vector2(-12f, -8f));

            flashOverlay = CreateImage("FlashOverlay", rootRect, new Color(1f, 1f, 1f, 0f));
            flashOverlay.rectTransform.anchorMin = Vector2.zero;
            flashOverlay.rectTransform.anchorMax = Vector2.one;
            flashOverlay.rectTransform.offsetMin = Vector2.zero;
            flashOverlay.rectTransform.offsetMax = Vector2.zero;
            flashOverlay.raycastTarget = false;

            thunderOverlay = CreateImage("ThunderOverlay", rootRect, new Color(0.36f, 0.48f, 0.70f, 0f));
            thunderOverlay.rectTransform.anchorMin = Vector2.zero;
            thunderOverlay.rectTransform.anchorMax = Vector2.one;
            thunderOverlay.rectTransform.offsetMin = Vector2.zero;
            thunderOverlay.rectTransform.offsetMax = Vector2.zero;
            thunderOverlay.raycastTarget = false;

            brushOverlay = CreateImage("BrushOverlay", rootRect, new Color(0.07f, 0.08f, 0.10f, 0f));
            brushOverlay.rectTransform.anchorMin = Vector2.zero;
            brushOverlay.rectTransform.anchorMax = Vector2.one;
            brushOverlay.rectTransform.offsetMin = Vector2.zero;
            brushOverlay.rectTransform.offsetMax = Vector2.zero;
            brushOverlay.raycastTarget = false;

            intentPanel = CreateHudPanel(
                "IntentPanel",
                new Vector2(0.61f, 0.84f),
                new Vector2(0.95f, 0.92f),
                new Color(0.11f, 0.11f, 0.14f, 0.78f),
                new Color(HudJade.r * 0.8f, HudJade.g * 0.8f, HudJade.b * 0.8f, 0.62f));
            intentText = CreateText("IntentText", intentPanel, 14, FontStyle.Normal, TextAnchor.MiddleLeft, new Color(0.88f, 0.90f, 0.96f, 1f));
            StretchText(intentText.rectTransform, new Vector2(16f, 8f), new Vector2(-16f, -8f));

            idlePanel = CreateHudPanel(
                "IdlePanel",
                new Vector2(0.23f, 0.16f),
                new Vector2(0.77f, 0.34f),
                new Color(0.12f, 0.12f, 0.11f, 0.72f),
                new Color(0.40f, 0.34f, 0.22f, 0.56f));
            idleTitleText = CreateText("IdleTitle", idlePanel, 22, FontStyle.Bold, TextAnchor.UpperCenter, HudPaper);
            idleTitleText.rectTransform.anchorMin = new Vector2(0f, 0.52f);
            idleTitleText.rectTransform.anchorMax = new Vector2(1f, 1f);
            idleTitleText.rectTransform.offsetMin = new Vector2(18f, -8f);
            idleTitleText.rectTransform.offsetMax = new Vector2(-18f, -10f);
            idleBodyText = CreateText("IdleBody", idlePanel, 15, FontStyle.Normal, TextAnchor.UpperCenter, HudMist);
            idleBodyText.rectTransform.anchorMin = new Vector2(0f, 0f);
            idleBodyText.rectTransform.anchorMax = new Vector2(1f, 0.62f);
            idleBodyText.rectTransform.offsetMin = new Vector2(20f, 12f);
            idleBodyText.rectTransform.offsetMax = new Vector2(-20f, -6f);
        }

        private IEnumerator PlaySequence(List<DemoBattlePresentationStep> steps)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                yield return PlayStep(steps[i]);
            }
        }

        private IEnumerator PlayStep(DemoBattlePresentationStep step)
        {
            phaseText.text = step.Label;

            switch (step.Type)
            {
                case DemoBattlePresentationStepType.PhaseShift:
                    yield return PlayPhaseShift(step);
                    break;
                case DemoBattlePresentationStepType.CardCast:
                    yield return PlayCardCast(step);
                    break;
                case DemoBattlePresentationStepType.SwordVolley:
                    yield return PlaySwordVolley(step);
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
                yield return AnimateSwordFlight(start, end, styleColor, GetFlightDuration(step, false), GetFlightThickness(step, false));

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
                StartCoroutine(AnimateSwordFlight(start, end, GetVolleyFlightColor(step), GetFlightDuration(step, true), GetFlightThickness(step, true)));

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
            phaseText.text = "天劫蓄势，下一轮将有重击";
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
            phaseText.text = victory ? "万剑归鞘，劫云退散" : "剑势散乱，灵台失守";
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

        private IEnumerator AnimateSwordFlight(Vector2 start, Vector2 end, Color color, float duration, float thickness)
        {
            Color trailColor = new Color(color.r * 0.55f, color.g * 0.55f, color.b * 0.58f, 0.28f);
            StartCoroutine(SpawnTransientMark((start + end) * 0.5f, new Vector2(140f, thickness * 1.5f), trailColor, duration * 0.9f, Mathf.Atan2(end.y - start.y, end.x - start.x) * Mathf.Rad2Deg));

            Image slash = CreateImage("SwordFlight", rootRect, color);
            transientMarks.Add(slash);
            RectTransform rect = slash.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(96f, thickness);

            Vector2 delta = end - start;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);

            float timer = 0f;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                float t = Mathf.Clamp01(timer / duration);
                rect.anchoredPosition = Vector2.Lerp(start, end, t);
                slash.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0.92f, 0.20f, t));
                yield return null;
            }

            transientMarks.Remove(slash);
            Destroy(slash.gameObject);
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

        private IEnumerator SpawnInkWash(Vector2 position, float size, Color color, float duration)
        {
            Image wash = CreateImage("InkWash", rootRect, new Color(color.r, color.g, color.b, color.a * 0.75f));
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

            // Far layer: slow drift, wide amplitude.
            farCloudA.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.12f) * 42f, farCloudA.anchoredPosition.y);
            farCloudB.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.16f + 1.1f) * 36f, farCloudB.anchoredPosition.y);
            farRidgeA.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.07f + 0.6f) * 18f, farRidgeA.anchoredPosition.y);
            farRidgeB.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.09f + 1.4f) * 14f, farRidgeB.anchoredPosition.y);

            // Mid layer: clearer parallax where spells and swords travel.
            mistBand.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.22f + 0.4f) * 28f, mistBand.anchoredPosition.y);
            midCloudShelf.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.19f + 0.2f) * 24f, midCloudShelf.anchoredPosition.y);
            midRuinBand.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.24f + 1.7f) * 30f, midRuinBand.anchoredPosition.y);
            midIslandLeft.anchoredPosition = new Vector2(-286f + Mathf.Sin(elapsed * 0.21f) * 22f, midIslandLeft.anchoredPosition.y);
            midIslandRight.anchoredPosition = new Vector2(344f + Mathf.Sin(elapsed * 0.18f + 0.8f) * 18f, midIslandRight.anchoredPosition.y);

            // Near layer: stronger movement to frame the duel and sell depth.
            foregroundMist.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.25f + 0.9f) * 34f, foregroundMist.anchoredPosition.y);
            frontFogA.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.28f + 0.5f) * 46f, frontFogA.anchoredPosition.y);
            frontFogB.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.34f + 1.8f) * 52f, frontFogB.anchoredPosition.y);
            inkVeil.anchoredPosition = new Vector2(Mathf.Sin(elapsed * 0.08f + 0.3f) * 16f, inkVeil.anchoredPosition.y);
            nearCliffLeft.anchoredPosition = new Vector2(-468f + Mathf.Sin(elapsed * 0.11f + 0.6f) * 10f, nearCliffLeft.anchoredPosition.y);
            nearCliffRight.anchoredPosition = new Vector2(474f + Mathf.Sin(elapsed * 0.10f + 1.2f) * 12f, nearCliffRight.anchoredPosition.y);
            nearPlatformGlowLeft.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.82f) * 0.04f, 1f, 1f);
            nearPlatformGlowRight.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.76f + 0.5f) * 0.04f, 1f, 1f);
            skyGlow.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.22f) * 0.02f, 1f, 1f);
            horizonGlow.localScale = new Vector3(1f + Mathf.Sin(elapsed * 0.36f + 1.4f) * 0.03f, 1f, 1f);

            Vector2 playerAnchor = GetPlayerAnchor();
            playerRoot.anchoredPosition = ScenePoint(playerAnchor.x, playerAnchor.y) + new Vector2(Mathf.Sin(elapsed * 1.2f) * 9f, Mathf.Sin(elapsed * 1.9f) * 8f);
            playerRoot.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 1.4f) * 2.6f - 4f);
            playerSword.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 2.1f) * 4f - 7f);

            Vector2 enemyAnchor = GetEnemyAnchor();
            enemyRoot.anchoredPosition = ScenePoint(enemyAnchor.x, enemyAnchor.y) + new Vector2(Mathf.Sin(elapsed * 1.0f + 0.7f) * 11f, Mathf.Sin(elapsed * 1.6f + 0.2f) * 10f);
            enemyRoot.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 1.1f + 0.2f) * 2.2f + 5f);
            enemySword.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(elapsed * 2.0f + 0.8f) * 3f + 8f);

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

        private void UpdateBossAtmosphere()
        {
            if (!controller.Battle.IsBossBattle)
            {
                intentText.text = "当前斗法暂无天机预警。";
                thunderOverlay.color = new Color(0.36f, 0.48f, 0.70f, 0f);
                if (bossPortrait != null)
                {
                    bossPortrait.color = new Color(0.74f, 0.80f, 0.92f, 0f);
                }
                return;
            }

            intentText.text = controller.Battle.BossIntentText;

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

        private void UpdateHudPanels()
        {
            if (controller == null || controller.Battle.Player == null || controller.Battle.Enemy == null)
            {
                return;
            }

            playerStatusText.text =
                $"剑修\n" +
                $"HP {controller.Battle.Player.Health}/{controller.Battle.Player.MaxHealth}\n" +
                $"剑意 {controller.Battle.Player.SwordIntent}  护盾 {controller.Battle.Player.Block}\n" +
                $"飞剑 {controller.Battle.TotalSwords}  灵气 {controller.Battle.Energy}/{controller.Battle.MaxEnergy}";

            enemyStatusText.text =
                $"{controller.Battle.Enemy.Name}\n" +
                $"HP {controller.Battle.Enemy.Health}/{controller.Battle.Enemy.MaxHealth}\n" +
                $"感电 {controller.Battle.Enemy.Shock}  流血 {controller.Battle.Enemy.Bleed}\n" +
                (controller.Battle.IsBossBattle
                    ? $"阶段 {GetBossPhaseLabel(controller.Battle.BossPhase)}"
                    : "当前为常规斗法");

            roundStatusText.text = $"第 {controller.Battle.Round} 回合";
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
                    return "起手定道：先看清你要修哪一脉";
                case DemoNodeType.RouteChoice:
                    return "路线分叉：下一段历练由你自己拐出来";
                case DemoNodeType.Reward:
                    return "战后补强：把散件收束成真正的 build";
                case DemoNodeType.Training:
                    return "修炼节点：功法与法宝正在改写规则";
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

        private RectTransform CreateHudPanel(string name, Vector2 anchorMin, Vector2 anchorMax, Color backgroundColor, Color borderColor)
        {
            RectTransform panel = CreatePanelRect(name, rootRect, backgroundColor, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            DecorateHudFrame(panel, borderColor);
            return panel;
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
            if (cachedBattleBackgroundSprite != null)
            {
                return cachedBattleBackgroundSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(BattleBackgroundResourcePath);
            if (texture == null)
            {
                return null;
            }

            cachedBattleBackgroundSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            return cachedBattleBackgroundSprite;
        }

        private Sprite LoadBossPortraitSprite()
        {
            if (cachedBossPortraitSprite != null)
            {
                return cachedBossPortraitSprite;
            }

            Texture2D texture = Resources.Load<Texture2D>(BossPortraitResourcePath);
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
                && controller.Battle.Round == 1
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
            return new Vector2(0.14f, 0.18f);
        }

        private Vector2 GetEnemyAnchor()
        {
            if (controller != null && controller.HasBattle && controller.Battle.IsBossBattle)
            {
                return new Vector2(0.74f, 0.70f);
            }

            return new Vector2(0.84f, 0.54f);
        }
    }
}
