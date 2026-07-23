using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Battle;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Systems;
using UnityEngine;
using UnityEngine.UI;

namespace PathOfTenThousandWays.Demo.UI
{
    public sealed class DemoBattleHudView : MonoBehaviour
    {
        private const string PlayerStatusResourcePath = "Art/UI/ui_battle_player_status_003";
        private const string EnemyStatusResourcePath = "Art/UI/ui_battle_enemy_status_003";
        private const string PhaseSealResourcePath = "Art/UI/ui_battle_phase_seal_003";
        private const string PhaseSealFallbackResourcePath = "Art/UI/ui_battle_phase_seal_002";
        private const string IntentStripResourcePath = "Art/UI/ui_battle_intent_strip_002";
        private const string CommandSurfaceResourcePath = "Art/UI/ui_battle_command_surface_001";
        private const string CardFrameResourcePath = "Art/UI/ui_battle_card_frame_003";
        private static readonly Color Paper = new Color(0.93f, 0.95f, 0.94f, 1f);
        private static readonly Color Mist = new Color(0.68f, 0.71f, 0.69f, 1f);
        private static readonly Color Gold = new Color(0.80f, 0.67f, 0.40f, 1f);
        private static readonly Color Jade = new Color(0.31f, 0.62f, 0.60f, 1f);
        private static readonly Color Crimson = new Color(0.72f, 0.24f, 0.20f, 1f);

        private readonly List<GameObject> cardObjects = new List<GameObject>();
        private readonly Dictionary<string, Sprite> loadedSprites = new Dictionary<string, Sprite>();
        private DemoGameController controller;
        private Font titleFont;
        private Font bodyFont;
        private Sprite whiteSprite;
        private RectTransform commandSurface;
        private RectTransform energyCluster;
        private RectTransform handRoot;
        private RectTransform resultBanner;
        private RectTransform cardDetailPanel;
        private Text resultTitleText;
        private Text resultSubtitleText;
        private Text cardDetailTitle;
        private Text cardDetailRules;
        private Text playerNameText;
        private Text playerHealthText;
        private Text playerStateText;
        private Text enemyNameText;
        private Text enemyHealthText;
        private Text enemyStateText;
        private Text phaseText;
        private Text encounterText;
        private Text intentText;
        private Text energyValueText;
        private Text energyRateText;
        private Text deckText;
        private Text discardText;
        private Text drawText;
        private Text pauseText;
        private Text speedText;
        private Image playerHealthFill;
        private Image enemyHealthFill;
        private Image intentPanelSurface;
        private Image intentFill;
        private Image energyFill;
        private Button pauseButton;
        private Button speedButton;
        private string handSignature = string.Empty;
        private float resumeSpeed = 1f;

        public void Initialize(DemoGameController demoController, Font fallbackFont)
        {
            controller = demoController;
            bodyFont = ResolveFont(new[] { "Source Han Sans SC", "Microsoft YaHei", "SimSun" }, fallbackFont, 24);
            titleFont = ResolveFont(new[] { "Source Han Serif SC", "SimSun", "Microsoft YaHei" }, bodyFont, 28);
            whiteSprite = Sprite.Create(
                Texture2D.whiteTexture,
                new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            BuildHierarchy();
            RefreshNow();
        }

        public void RefreshNow()
        {
            if (controller == null || !controller.HasBattle)
            {
                return;
            }

            DemoBattleState battle = controller.Battle;
            playerNameText.text = battle.Player.Name;
            playerHealthText.text = $"{battle.Player.Health} / {battle.Player.MaxHealth}";
            playerStateText.text = $"盾 {battle.Player.Block}   剑意 {battle.Player.SwordIntent}   本命飞剑 {battle.TotalSwords}";
            SetFill(playerHealthFill, battle.Player.Health / (float)Mathf.Max(1, battle.Player.MaxHealth));

            enemyNameText.text = battle.Enemy.Name;
            enemyHealthText.text = $"{battle.Enemy.Health} / {battle.Enemy.MaxHealth}";
            enemyStateText.text = $"锁定 · 敌 {battle.ActiveEnemyCount}   感电 {battle.Enemy.Shock}   流血 {battle.Enemy.Bleed}";
            SetFill(enemyHealthFill, battle.Enemy.Health / (float)Mathf.Max(1, battle.Enemy.MaxHealth));

            phaseText.text = battle.Phase == DemoBattlePhase.Intro
                ? "入阵"
                : battle.Phase == DemoBattlePhase.Won
                    ? "已破"
                    : battle.Phase == DemoBattlePhase.Lost ? "失守" : "斗法";
            encounterText.text = battle.IsOpeningBattlePacing
                ? "旧矿地窟 · 入场试锋"
                : battle.IsBossBattle
                    ? "第三幕 · 玄铁镇矿剑傀"
                    : $"第 {controller.Run.Map.CurrentNode.Layer} 层 · {controller.Run.Map.CurrentNode.Name}";
            bool intentWarning = battle.Phase == DemoBattlePhase.Running
                && battle.EnemyIntentProgress >= 0.82f
                && battle.Enemy.Health > 0;
            float warningPulse = 0.5f + Mathf.Sin(Time.unscaledTime * 10f) * 0.5f;
            intentText.text = intentWarning
                ? $"杀意将至  {battle.EnemyIntentText}   {Mathf.Max(0f, battle.EnemyIntentRemaining):0.0}s"
                : $"敌方意图  {battle.EnemyIntentText}   {Mathf.Max(0f, battle.EnemyIntentRemaining):0.0}s";
            intentText.color = intentWarning
                ? Color.Lerp(new Color(0.96f, 0.77f, 0.45f, 1f), new Color(1f, 0.46f, 0.32f, 1f), warningPulse)
                : Paper;
            intentFill.color = intentWarning
                ? Color.Lerp(new Color(0.92f, 0.58f, 0.26f, 1f), Crimson, warningPulse)
                : Jade;
            intentPanelSurface.color = intentWarning
                ? new Color(0.13f, 0.045f, 0.035f, 0.84f)
                : new Color(0.035f, 0.055f, 0.055f, 0.72f);
            SetFill(intentFill, battle.EnemyIntentProgress);

            energyValueText.text = $"手 {battle.Hand.Count}/{battle.HandLimit}";
            energyRateText.text = "抽取循环";
            deckText.text = $"牌库 {battle.DrawPile.Count}";
            discardText.text = $"弃牌 {battle.DiscardPile.Count}";
            drawText.text = battle.Hand.Count >= battle.HandLimit
                ? "手牌已满 · 抽牌暂停"
                : $"下次抽牌 {Mathf.Max(0f, battle.DrawTimer):0.0}s";
            SetFill(energyFill, battle.Hand.Count / (float)Mathf.Max(1, battle.HandLimit));

            bool timeActive = battle.Phase == DemoBattlePhase.Intro || battle.Phase == DemoBattlePhase.Running;
            bool terminal = battle.Phase == DemoBattlePhase.Won || battle.Phase == DemoBattlePhase.Lost;
            commandSurface.sizeDelta = new Vector2(1100f, terminal ? 118f : 230f);
            energyCluster.gameObject.SetActive(!terminal);
            handRoot.gameObject.SetActive(!terminal);
            resultBanner.gameObject.SetActive(terminal);
            if (terminal)
            {
                resultTitleText.text = battle.Phase == DemoBattlePhase.Won ? "破敌" : "失守";
                resultSubtitleText.text = battle.Phase == DemoBattlePhase.Won ? "剑归匣" : "道基不支";
                cardDetailPanel.gameObject.SetActive(false);
                handSignature = string.Empty;
            }
            if (controller.BattleSpeed > 0.01f)
            {
                resumeSpeed = controller.BattleSpeed >= 1.9f ? 2f : 1f;
            }
            pauseButton.gameObject.SetActive(timeActive);
            speedButton.gameObject.SetActive(timeActive);
            pauseText.text = controller.BattleSpeed <= 0.01f ? "▶" : "Ⅱ";
            speedText.text = (controller.BattleSpeed > 0.01f ? controller.BattleSpeed : resumeSpeed) >= 1.9f ? "×2" : "×1";

            string signature = BuildHandSignature(battle);
            if (!terminal && signature != handSignature)
            {
                handSignature = signature;
                RebuildHand();
            }
        }

        private void Update()
        {
            RefreshNow();
        }

        private void BuildHierarchy()
        {
            RectTransform root = GetComponent<RectTransform>();
            root.anchorMin = Vector2.zero;
            root.anchorMax = Vector2.one;
            root.offsetMin = Vector2.zero;
            root.offsetMax = Vector2.zero;
            BuildStatusHud(root);
            BuildCommandSurface(root);
        }

        private void BuildStatusHud(RectTransform root)
        {
            RectTransform playerPanel = CreateFixedPanel(
                root, "PlayerStatus", new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(36f, -24f), new Vector2(520f, 108f), new Color(0.035f, 0.045f, 0.043f, 0.72f));
            ApplyOptionalSurface(playerPanel, PlayerStatusResourcePath);
            CreateBorder(playerPanel, new Color(Gold.r, Gold.g, Gold.b, 0.42f), 2f);
            playerNameText = CreateText(playerPanel, "Name", titleFont, 23, FontStyle.Bold, TextAnchor.UpperLeft, Paper);
            SetAnchors(playerNameText.rectTransform, new Vector2(0.04f, 0.64f), new Vector2(0.55f, 0.95f));
            playerHealthText = CreateText(playerPanel, "Health", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperRight, Paper);
            SetAnchors(playerHealthText.rectTransform, new Vector2(0.55f, 0.66f), new Vector2(0.96f, 0.94f));
            playerHealthFill = CreateBar(
                playerPanel, "HealthBar", new Vector2(0.04f, 0.40f), new Vector2(0.96f, 0.57f),
                new Color(0.17f, 0.12f, 0.10f, 0.84f), Crimson);
            playerStateText = CreateText(playerPanel, "State", bodyFont, 14, FontStyle.Bold, TextAnchor.LowerLeft, Mist);
            SetAnchors(playerStateText.rectTransform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.34f));

            RectTransform enemyPanel = CreateFixedPanel(
                root, "EnemyStatus", new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-36f, -24f), new Vector2(520f, 108f), new Color(0.045f, 0.035f, 0.033f, 0.72f));
            ApplyOptionalSurface(enemyPanel, EnemyStatusResourcePath);
            CreateBorder(enemyPanel, new Color(0.66f, 0.36f, 0.28f, 0.46f), 2f);
            enemyNameText = CreateText(enemyPanel, "Name", titleFont, 23, FontStyle.Bold, TextAnchor.UpperRight, Paper);
            SetAnchors(enemyNameText.rectTransform, new Vector2(0.36f, 0.64f), new Vector2(0.96f, 0.95f));
            enemyHealthText = CreateText(enemyPanel, "Health", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Paper);
            SetAnchors(enemyHealthText.rectTransform, new Vector2(0.04f, 0.66f), new Vector2(0.38f, 0.94f));
            enemyHealthFill = CreateBar(
                enemyPanel, "HealthBar", new Vector2(0.04f, 0.40f), new Vector2(0.96f, 0.57f),
                new Color(0.18f, 0.10f, 0.09f, 0.84f), new Color(0.70f, 0.22f, 0.18f, 1f));
            enemyStateText = CreateText(enemyPanel, "State", bodyFont, 14, FontStyle.Bold, TextAnchor.LowerRight, Mist);
            SetAnchors(enemyStateText.rectTransform, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.34f));

            RectTransform seal = CreateFixedPanel(
                root, "PhaseSeal", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -18f), new Vector2(112f, 112f), new Color(0.08f, 0.08f, 0.07f, 0.30f));
            Sprite sealSprite = LoadSpriteResource(PhaseSealResourcePath)
                ?? LoadSpriteResource(PhaseSealFallbackResourcePath);
            if (sealSprite != null)
            {
                Image sealImage = seal.GetComponent<Image>();
                sealImage.sprite = sealSprite;
                sealImage.color = Color.white;
                sealImage.preserveAspect = true;
            }
            phaseText = CreateText(seal, "Phase", titleFont, 25, FontStyle.Bold, TextAnchor.MiddleCenter, new Color(0.38f, 0.26f, 0.14f, 1f));
            Stretch(phaseText.rectTransform, new Vector2(12f, 14f), new Vector2(-12f, -8f));
            encounterText = CreateText(root, "Encounter", bodyFont, 14, FontStyle.Bold, TextAnchor.UpperCenter, new Color(0.17f, 0.15f, 0.12f, 0.82f));
            SetFixed(encounterText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -122f), new Vector2(360f, 26f));

            RectTransform intentPanel = CreateFixedPanel(
                root, "EnemyIntent", new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-36f, -144f), new Vector2(472f, 54f), new Color(0.035f, 0.055f, 0.055f, 0.72f));
            intentPanelSurface = intentPanel.GetComponent<Image>();
            ApplyOptionalSurface(intentPanel, IntentStripResourcePath);
            CreateBorder(intentPanel, new Color(Jade.r, Jade.g, Jade.b, 0.36f), 1.5f);
            intentText = CreateText(intentPanel, "Intent", bodyFont, 15, FontStyle.Bold, TextAnchor.UpperLeft, Paper);
            SetAnchors(intentText.rectTransform, new Vector2(0.045f, 0.36f), new Vector2(0.955f, 0.92f));
            intentFill = CreateBar(
                intentPanel, "IntentBar", new Vector2(0.045f, 0.12f), new Vector2(0.955f, 0.27f),
                new Color(0.10f, 0.15f, 0.15f, 0.88f), Jade);
        }

        private void BuildCommandSurface(RectTransform root)
        {
            commandSurface = CreateFixedPanel(
                root, "CommandSurface", new Vector2(0.60f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 8f), new Vector2(1100f, 230f), new Color(0.035f, 0.04f, 0.038f, 0.64f));
            ApplyOptionalSurface(commandSurface, CommandSurfaceResourcePath);
            CreatePanel(commandSurface, "TopWash", new Vector2(0f, 0.86f), Vector2.one, new Color(0.17f, 0.16f, 0.13f, 0.16f));
            CreatePanel(commandSurface, "TopRule", new Vector2(0f, 0.98f), Vector2.one, new Color(Gold.r, Gold.g, Gold.b, 0.36f));

            energyCluster = CreateFixedPanel(
                commandSurface, "EnergyCluster", new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(24f, 18f), new Vector2(170f, 176f), Color.clear);
            Text energyTitle = CreateText(energyCluster, "Title", titleFont, 18, FontStyle.Bold, TextAnchor.UpperLeft, Gold);
            SetAnchors(energyTitle.rectTransform, new Vector2(0.08f, 0.74f), new Vector2(0.50f, 0.94f));
            energyTitle.text = "牌序";
            energyValueText = CreateText(energyCluster, "Value", bodyFont, 31, FontStyle.Bold, TextAnchor.UpperRight, Paper);
            SetAnchors(energyValueText.rectTransform, new Vector2(0.45f, 0.69f), new Vector2(0.92f, 0.96f));
            energyRateText = CreateText(energyCluster, "Rate", bodyFont, 13, FontStyle.Bold, TextAnchor.UpperLeft, Jade);
            SetAnchors(energyRateText.rectTransform, new Vector2(0.08f, 0.56f), new Vector2(0.92f, 0.72f));
            energyFill = CreateBar(
                energyCluster, "EnergyBar", new Vector2(0.08f, 0.43f), new Vector2(0.92f, 0.51f),
                new Color(0.08f, 0.15f, 0.15f, 0.92f), Jade);
            deckText = CreateText(energyCluster, "Deck", bodyFont, 14, FontStyle.Bold, TextAnchor.MiddleLeft, Mist);
            SetAnchors(deckText.rectTransform, new Vector2(0.08f, 0.20f), new Vector2(0.50f, 0.39f));
            discardText = CreateText(energyCluster, "Discard", bodyFont, 14, FontStyle.Bold, TextAnchor.MiddleRight, Mist);
            SetAnchors(discardText.rectTransform, new Vector2(0.50f, 0.20f), new Vector2(0.92f, 0.39f));
            drawText = CreateText(energyCluster, "Draw", bodyFont, 13, FontStyle.Normal, TextAnchor.LowerLeft, new Color(Mist.r, Mist.g, Mist.b, 0.84f));
            SetAnchors(drawText.rectTransform, new Vector2(0.08f, 0.04f), new Vector2(0.92f, 0.20f));

            handRoot = CreateFixedPanel(
                commandSurface, "Hand", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(20f, 6f), new Vector2(740f, 216f), Color.clear);
            handRoot.GetComponent<Image>().raycastTarget = false;

            resultBanner = CreateFixedPanel(
                commandSurface, "ResultBanner", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 18f), new Vector2(440f, 88f), Color.clear);
            resultTitleText = CreateText(resultBanner, "Title", titleFont, 28, FontStyle.Bold, TextAnchor.UpperCenter, Gold);
            SetAnchors(resultTitleText.rectTransform, new Vector2(0f, 0.46f), Vector2.one);
            resultSubtitleText = CreateText(resultBanner, "Subtitle", bodyFont, 14, FontStyle.Bold, TextAnchor.LowerCenter, Mist);
            SetAnchors(resultSubtitleText.rectTransform, Vector2.zero, new Vector2(1f, 0.48f));
            resultBanner.gameObject.SetActive(false);

            cardDetailPanel = CreateFixedPanel(
                root, "CardDetail", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f),
                new Vector2(0f, 260f), new Vector2(500f, 94f), new Color(0.045f, 0.05f, 0.048f, 0.94f));
            CreateBorder(cardDetailPanel, new Color(Gold.r, Gold.g, Gold.b, 0.38f), 1.5f);
            cardDetailTitle = CreateText(cardDetailPanel, "Title", titleFont, 19, FontStyle.Bold, TextAnchor.UpperLeft, Gold);
            SetAnchors(cardDetailTitle.rectTransform, new Vector2(0.045f, 0.58f), new Vector2(0.95f, 0.92f));
            cardDetailRules = CreateText(cardDetailPanel, "Rules", bodyFont, 14, FontStyle.Normal, TextAnchor.UpperLeft, Paper);
            SetAnchors(cardDetailRules.rectTransform, new Vector2(0.045f, 0.10f), new Vector2(0.95f, 0.60f));
            cardDetailRules.horizontalOverflow = HorizontalWrapMode.Wrap;
            cardDetailRules.verticalOverflow = VerticalWrapMode.Truncate;
            cardDetailPanel.gameObject.SetActive(false);

            RectTransform controls = CreateFixedPanel(
                commandSurface, "Controls", new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-28f, 28f), new Vector2(186f, 96f), Color.clear);
            controls.GetComponent<Image>().raycastTarget = false;
            pauseButton = CreateControlButton(controls, "Pause", new Vector2(0f, 0f), out pauseText);
            speedButton = CreateControlButton(controls, "Speed", new Vector2(96f, 0f), out speedText);
            pauseButton.onClick.AddListener(TogglePause);
            speedButton.onClick.AddListener(ToggleSpeed);
        }

        private void RebuildHand()
        {
            for (int i = 0; i < cardObjects.Count; i++)
            {
                if (cardObjects[i] != null)
                {
                    cardObjects[i].SetActive(false);
                    Destroy(cardObjects[i]);
                }
            }
            cardObjects.Clear();
            cardDetailPanel.gameObject.SetActive(false);

            DemoBattleState battle = controller.Battle;
            int count = battle.Hand.Count;
            if (count == 0)
            {
                return;
            }

            const float cardWidth = 136f;
            const float cardHeight = 196f;
            float step = count <= 1 ? 0f : Mathf.Min(146f, (704f - cardWidth) / (count - 1));
            float center = (count - 1) * 0.5f;
            bool cardsCanBePlayed = battle.Phase == DemoBattlePhase.Running && controller.BattleSpeed > 0.01f;

            for (int i = 0; i < count; i++)
            {
                DemoCard card = battle.Hand[i];
                bool playable = cardsCanBePlayed && battle.Energy >= card.Cost;
                DemoBattleCardView cardView = CreateCardView(card, i, playable, cardWidth, cardHeight);
                float offset = i - center;
                cardView.SetRestPose(new Vector2(offset * step, 6f - Mathf.Abs(offset) * 4f), -offset * 1.7f);
                cardObjects.Add(cardView.gameObject);
            }
        }

        private DemoBattleCardView CreateCardView(
            DemoCard card,
            int index,
            bool playable,
            float width,
            float height)
        {
            GameObject cardObject = new GameObject(
                "Card_" + card.Id,
                typeof(RectTransform),
                typeof(Image),
                typeof(DemoBattleCardView));
            cardObject.transform.SetParent(handRoot, false);
            RectTransform cardRect = cardObject.GetComponent<RectTransform>();
            cardRect.anchorMin = new Vector2(0.5f, 0f);
            cardRect.anchorMax = new Vector2(0.5f, 0f);
            cardRect.pivot = new Vector2(0.5f, 0f);
            cardRect.sizeDelta = new Vector2(width, height);

            Image surface = cardObject.GetComponent<Image>();
            surface.sprite = whiteSprite;
            surface.color = new Color(0.055f, 0.064f, 0.062f, 0.98f);
            Sprite frameSprite = LoadSpriteResource(CardFrameResourcePath);
            if (frameSprite != null)
            {
                surface.sprite = frameSprite;
                surface.type = frameSprite.border.sqrMagnitude > 0.01f ? Image.Type.Sliced : Image.Type.Simple;
                surface.color = Color.white;
            }
            surface.raycastTarget = true;
            CanvasGroup group = cardObject.AddComponent<CanvasGroup>();
            group.alpha = playable ? 1f : 0.62f;

            CreatePanel(cardRect, "InnerPaper", new Vector2(0.035f, 0.025f), new Vector2(0.965f, 0.975f), new Color(0.82f, 0.79f, 0.70f, 0.17f));
            Color accent = GetCardAccent(card);
            CreateBorder(cardRect, new Color(accent.r, accent.g, accent.b, 0.84f), 2f);

            RectTransform artStage = CreatePanel(
                cardRect, "ArtStage", new Vector2(0.075f, 0.39f), new Vector2(0.925f, 0.78f),
                new Color(0.78f, 0.76f, 0.68f, 0.18f));
            Image artImage = CreateImage(artStage, "Art", Color.white);
            Stretch(artImage.rectTransform, new Vector2(3f, 3f), new Vector2(-3f, -3f));
            Sprite art = LoadSpriteResource("Art/Cards/card_art_" + card.Id + "_001");
            artImage.sprite = art;
            artImage.preserveAspect = false;
            artImage.color = art != null ? Color.white : new Color(accent.r, accent.g, accent.b, 0.28f);
            if (art == null)
            {
                Text glyph = CreateText(artStage, "Glyph", titleFont, 34, FontStyle.Bold, TextAnchor.MiddleCenter, accent);
                Stretch(glyph.rectTransform, Vector2.zero, Vector2.zero);
                glyph.text = string.IsNullOrEmpty(card.IconGlyph) ? "诀" : card.IconGlyph;
            }

            RectTransform costSeal = CreateFixedPanel(
                cardRect, "CostSeal", new Vector2(0f, 1f), new Vector2(0.5f, 0.5f),
                new Vector2(25f, -25f), new Vector2(42f, 42f),
                playable ? new Color(0.14f, 0.12f, 0.085f, 1f) : new Color(0.36f, 0.12f, 0.10f, 1f));
            CreateBorder(costSeal, new Color(accent.r, accent.g, accent.b, 0.86f), 1f);
            Text cost = CreateText(costSeal, "Cost", bodyFont, 20, FontStyle.Bold, TextAnchor.MiddleCenter, playable ? Gold : new Color(1f, 0.72f, 0.62f, 1f));
            Stretch(cost.rectTransform, Vector2.zero, Vector2.zero);
            cost.text = card.Cost.ToString();

            Text name = CreateText(cardRect, "Name", titleFont, 18, FontStyle.Bold, TextAnchor.UpperLeft, Paper);
            SetAnchors(name.rectTransform, new Vector2(0.29f, 0.80f), new Vector2(0.94f, 0.95f));
            name.verticalOverflow = VerticalWrapMode.Truncate;
            name.text = card.Name;

            Text type = CreateText(cardRect, "Type", bodyFont, 12, FontStyle.Bold, TextAnchor.LowerRight, accent);
            SetAnchors(type.rectTransform, new Vector2(0.48f, 0.79f), new Vector2(0.94f, 0.91f));
            type.text = GetCardTypeLabel(card.Type);

            Text rules = CreateText(cardRect, "Rules", bodyFont, 13, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.88f, 0.87f, 0.82f, 0.96f));
            SetAnchors(rules.rectTransform, new Vector2(0.09f, 0.08f), new Vector2(0.91f, 0.34f));
            rules.horizontalOverflow = HorizontalWrapMode.Wrap;
            rules.verticalOverflow = VerticalWrapMode.Truncate;
            rules.text = BuildCardPreview(card);

            DemoBattleCardView view = cardObject.GetComponent<DemoBattleCardView>();
            view.Configure(card, index, playable, PlayCard, OnCardHover);
            return view;
        }

        private void OnCardHover(DemoCard card, bool active)
        {
            if (!active || card == null)
            {
                cardDetailPanel.gameObject.SetActive(false);
                return;
            }

            cardDetailTitle.text = $"{card.Name} · {GetCardTypeLabel(card.Type)} · {card.Cost} 灵气";
            cardDetailRules.text = card.GetRulesText();
            cardDetailPanel.gameObject.SetActive(true);
        }

        private void PlayCard(int index)
        {
            if (controller.TryPlayCardAt(index))
            {
                cardDetailPanel.gameObject.SetActive(false);
                handSignature = string.Empty;
            }
        }

        private void TogglePause()
        {
            if (controller.BattleSpeed <= 0.01f)
            {
                controller.BattleSpeed = resumeSpeed;
                handSignature = string.Empty;
                return;
            }

            resumeSpeed = controller.BattleSpeed >= 1.9f ? 2f : 1f;
            controller.BattleSpeed = 0f;
            handSignature = string.Empty;
        }

        private void ToggleSpeed()
        {
            float current = controller.BattleSpeed > 0.01f ? controller.BattleSpeed : resumeSpeed;
            resumeSpeed = current >= 1.9f ? 1f : 2f;
            if (controller.BattleSpeed > 0.01f)
            {
                controller.BattleSpeed = resumeSpeed;
            }
            handSignature = string.Empty;
        }

        private static string BuildHandSignature(DemoBattleState battle)
        {
            string signature = $"{battle.Phase}|{battle.Energy}|{battle.Hand.Count}|{battle.DrawPile.Count}|{battle.DiscardPile.Count}";
            for (int i = 0; i < battle.Hand.Count; i++)
            {
                DemoCard card = battle.Hand[i];
                signature += $"|{card.Id}:{card.Cost}";
            }
            return signature;
        }

        private static string BuildCardPreview(DemoCard card)
        {
            string rules = card.GetRulesText();
            if (string.IsNullOrWhiteSpace(rules))
            {
                return "以一念介入持续斗法。";
            }
            return rules.Replace("。", "。\n").Trim();
        }

        private static Color GetCardAccent(DemoCard card)
        {
            if (card.Id == "guard_step")
            {
                return Jade;
            }
            if (card.Id == "cloud_step")
            {
                return new Color(0.55f, 0.72f, 0.71f, 1f);
            }
            return Gold;
        }

        private static string GetCardTypeLabel(DemoCardType type)
        {
            switch (type)
            {
                case DemoCardType.Attack:
                    return "剑术";
                case DemoCardType.Defense:
                    return "身法";
                case DemoCardType.Resource:
                    return "吐纳";
                case DemoCardType.FlyingSword:
                    return "御剑";
                case DemoCardType.Finisher:
                    return "收束";
                default:
                    return "法诀";
            }
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
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = gameObject.GetComponent<Image>();
            image.sprite = whiteSprite;
            image.color = color;
            image.raycastTarget = false;
            return rect;
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

        private Image CreateImage(Transform parent, string name, Color color)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(Image));
            gameObject.transform.SetParent(parent, false);
            Image image = gameObject.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
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
            Text textComponent = gameObject.GetComponent<Text>();
            textComponent.font = font;
            textComponent.fontSize = size;
            textComponent.fontStyle = style;
            textComponent.alignment = alignment;
            textComponent.color = color;
            textComponent.raycastTarget = false;
            return textComponent;
        }

        private Image CreateBar(
            Transform parent,
            string name,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Color background,
            Color fillColor)
        {
            RectTransform bar = CreatePanel(parent, name, anchorMin, anchorMax, background);
            RectTransform fill = CreatePanel(bar, "Fill", Vector2.zero, Vector2.one, fillColor);
            fill.pivot = new Vector2(0f, 0.5f);
            return fill.GetComponent<Image>();
        }

        private Button CreateControlButton(Transform parent, string name, Vector2 position, out Text label)
        {
            RectTransform rect = CreateFixedPanel(
                parent, name, Vector2.zero, Vector2.zero, position, new Vector2(82f, 58f),
                new Color(0.075f, 0.085f, 0.08f, 0.96f));
            rect.pivot = Vector2.zero;
            CreateBorder(rect, new Color(Gold.r, Gold.g, Gold.b, 0.46f), 1.5f);
            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = rect.GetComponent<Image>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 0.96f, 0.86f, 1f);
            colors.pressedColor = new Color(0.76f, 0.74f, 0.68f, 1f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;
            label = CreateText(rect, "Label", bodyFont, 20, FontStyle.Bold, TextAnchor.MiddleCenter, Paper);
            Stretch(label.rectTransform, Vector2.zero, Vector2.zero);
            return button;
        }

        private void CreateBorder(RectTransform parent, Color color, float thickness)
        {
            RectTransform top = CreatePanel(parent, "BorderTop", new Vector2(0f, 1f), new Vector2(1f, 1f), color);
            top.offsetMin = new Vector2(0f, -thickness);
            RectTransform bottom = CreatePanel(parent, "BorderBottom", new Vector2(0f, 0f), new Vector2(1f, 0f), color);
            bottom.offsetMax = new Vector2(0f, thickness);
            RectTransform left = CreatePanel(parent, "BorderLeft", new Vector2(0f, 0f), new Vector2(0f, 1f), color);
            left.offsetMax = new Vector2(thickness, 0f);
            RectTransform right = CreatePanel(parent, "BorderRight", new Vector2(1f, 0f), new Vector2(1f, 1f), color);
            right.offsetMin = new Vector2(-thickness, 0f);
        }

        private static void SetAnchors(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
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

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void SetFill(Image fill, float ratio)
        {
            RectTransform rect = fill.rectTransform;
            rect.anchorMax = new Vector2(Mathf.Clamp01(ratio), 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private Sprite LoadSpriteResource(string resourcePath)
        {
            if (loadedSprites.TryGetValue(resourcePath, out Sprite cached))
            {
                return cached;
            }

            Sprite importedSprite = Resources.Load<Sprite>(resourcePath);
            if (importedSprite != null)
            {
                loadedSprites[resourcePath] = importedSprite;
                return importedSprite;
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

        private void ApplyOptionalSurface(RectTransform panel, string resourcePath)
        {
            Sprite sprite = LoadSpriteResource(resourcePath);
            if (panel == null || sprite == null)
            {
                return;
            }

            Image image = panel.GetComponent<Image>();
            image.sprite = sprite;
            image.type = sprite.border.sqrMagnitude > 0.01f ? Image.Type.Sliced : Image.Type.Simple;
            image.color = Color.white;
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
