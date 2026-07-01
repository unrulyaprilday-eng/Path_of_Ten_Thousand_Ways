param(
    [string]$Path = 'D:\MyGame\Path_of_Ten_Thousand_Ways\UNITY\Assets\Scripts\UI\DemoBattleSceneView.cs'
)

$text = Get-Content -LiteralPath $Path -Raw -Encoding UTF8

function Replace-Once([string]$Source, [string]$Pattern, [string]$Replacement) {
    $count = 0
    $result = [regex]::Replace($Source, $Pattern, { param($m) $script:count++; $Replacement }, 1)
    if ($count -eq 0) {
        throw "Pattern not found: $Pattern"
    }
    return $result
}

$text = [regex]::Replace(
    $text,
    'private sealed class FloatingPopup\s*\{[\s\S]*?\}\s*',
    {
        param($m)
        return $m.Value + @'

        private enum EncounterVisualTier
        {
            Minor,
            Elite,
            MiniBoss,
            FinalBoss
        }
'@
    },
    1
)

$text = [regex]::Replace(
    $text,
    'private RectTransform enemyBody;\s*',
    {
        param($m)
        return $m.Value + @'
        private Image enemyAuraImage;
        private Image enemySwordImage;
        private Image enemySwordTrailImage;
        private Image enemyBodyImage;
        private Text enemyLabelText;
'@
    },
    1
)

$text = $text.Replace(
@'
            UpdateBossAtmosphere();
            UpdateHudPanels();
'@,
@'
            UpdateBossAtmosphere();
            UpdateEncounterPresentation();
            UpdateHudPanels();
'@
)

$text = [regex]::Replace(
    $text,
    'playerRoot = CreateEntity\("PlayerRoot", playerAnchor, new Color\(0\.20f, 0\.22f, 0\.24f, 1f\), new Color\(0\.62f, 0\.69f, 0\.74f, 1f\), "剑修", true, true\);\s*playerSword = playerRoot\.Find\("Sword"\) as RectTransform;\s*playerBody = playerRoot\.Find\("Body"\) as RectTransform;\s*enemyRoot = CreateEntity\("EnemyRoot", enemyAnchor, new Color\(0\.90f, 0\.93f, 0\.94f, 1f\), new Color\(0\.50f, 0\.74f, 0\.92f, 1f\), "天劫", false, false\);\s*enemySword = enemyRoot\.Find\("Sword"\) as RectTransform;\s*enemyBody = enemyRoot\.Find\("Body"\) as RectTransform;\s*',
    {
        param($m)
        return @'
            playerRoot = CreateEntity("PlayerRoot", playerAnchor, new Color(0.20f, 0.22f, 0.24f, 1f), new Color(0.62f, 0.69f, 0.74f, 1f), "剑修", true, true);
            playerSword = playerRoot.Find("Sword") as RectTransform;
            playerBody = playerRoot.Find("Body") as RectTransform;

            enemyRoot = CreateEntity("EnemyRoot", enemyAnchor, new Color(0.90f, 0.93f, 0.94f, 1f), new Color(0.50f, 0.74f, 0.92f, 1f), "敌手", false, false);
            enemySword = enemyRoot.Find("Sword") as RectTransform;
            enemySwordImage = enemySword != null ? enemySword.GetComponent<Image>() : null;
            enemyBody = enemyRoot.Find("Body") as RectTransform;
            enemyBodyImage = enemyBody != null ? enemyBody.GetComponent<Image>() : null;
            Transform enemyAura = enemyRoot.Find("Aura");
            enemyAuraImage = enemyAura != null ? enemyAura.GetComponent<Image>() : null;
            Transform enemyTrail = enemyRoot.Find("SwordTrail");
            enemySwordTrailImage = enemyTrail != null ? enemyTrail.GetComponent<Image>() : null;
            Transform enemyLabel = enemyRoot.Find("Label");
            enemyLabelText = enemyLabel != null ? enemyLabel.GetComponent<Text>() : null;
'@
    },
    1
)

$text = [regex]::Replace(
    $text,
    '        private void UpdateBossAtmosphere\(\)\s*\{[\s\S]*?\n        \}\r?\n\r?\n        private void UpdateHudPanels\(\)',
    {
        param($m)
        return @'
        private void UpdateBossAtmosphere()
        {
            if (!controller.Battle.IsBossBattle)
            {
                EncounterVisualTier encounterTier = GetEncounterVisualTier();
                switch (encounterTier)
                {
                    case EncounterVisualTier.Elite:
                        intentText.text = "精英斗法：敌势更稳，先拆压制，再把连锁收束出来。";
                        thunderOverlay.color = new Color(0.28f, 0.38f, 0.48f, 0.028f + Mathf.Sin(elapsed * 1.1f) * 0.008f);
                        brushOverlay.color = new Color(0.07f, 0.08f, 0.10f, 0.025f);
                        break;
                    case EncounterVisualTier.MiniBoss:
                        intentText.text = "守关小Boss：压迫感已经抬起来了，但还没到终局天劫。";
                        thunderOverlay.color = new Color(0.40f, 0.38f, 0.26f, 0.045f + Mathf.Sin(elapsed * 1.0f) * 0.012f);
                        brushOverlay.color = new Color(0.08f, 0.07f, 0.06f, 0.045f);
                        break;
                    default:
                        intentText.text = "常规斗法：先试锋，再看这一段真正缺什么。";
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
                enemyLabelText.text = GetEncounterTierLabel(encounterTier);
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
                    enemyPanelImage.color = GetEncounterPanelColor(encounterTier);
                }
            }

            enemyRoot.localScale = Vector3.one * GetEncounterScale(encounterTier);
        }

        private void UpdateHudPanels()
'@
    },
    1
)

$text = $text.Replace(
@'
            enemyStatusText.text =
                $"{controller.Battle.Enemy.Name}\n" +
                $"HP {controller.Battle.Enemy.Health}/{controller.Battle.Enemy.MaxHealth}\n" +
                $"感电 {controller.Battle.Enemy.Shock}  流血 {controller.Battle.Enemy.Bleed}\n" +
                (controller.Battle.IsBossBattle
                    ? $"阶段 {GetBossPhaseLabel(controller.Battle.BossPhase)}"
                    : "当前为常规斗法");
'@,
@'
            enemyStatusText.text =
                $"{controller.Battle.Enemy.Name}\n" +
                $"HP {controller.Battle.Enemy.Health}/{controller.Battle.Enemy.MaxHealth}\n" +
                $"感电 {controller.Battle.Enemy.Shock}  流血 {controller.Battle.Enemy.Bleed}\n" +
                (controller.Battle.IsBossBattle
                    ? $"阶段 {GetBossPhaseLabel(controller.Battle.BossPhase)} · 终局Boss"
                    : GetEncounterTierLabel(GetEncounterVisualTier()));
'@
)

$text = [regex]::Replace(
    $text,
    '            Vector2 enemyAnchor = GetEnemyAnchor\(\);\s*enemyRoot\.anchoredPosition = ScenePoint\(enemyAnchor\.x, enemyAnchor\.y\) \+ new Vector2\(Mathf\.Sin\(elapsed \* 1\.0f \+ 0\.7f\) \* 11f, Mathf\.Sin\(elapsed \* 1\.6f \+ 0\.2f\) \* 10f\);\s*enemyRoot\.localRotation = Quaternion\.Euler\(0f, 0f, Mathf\.Sin\(elapsed \* 1\.1f \+ 0\.2f\) \* 2\.2f \+ 5f\);\s*enemySword\.localRotation = Quaternion\.Euler\(0f, 0f, Mathf\.Sin\(elapsed \* 2\.0f \+ 0\.8f\) \* 3f \+ 8f\);',
    {
        param($m)
        return @'
            EncounterVisualTier encounterTier = GetEncounterVisualTier();
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
'@
    },
    1
)

$helpers = @'
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
                && (nodeName.Contains("守门") || nodeName.Contains("守卫") || nodeName.Contains("执兵") || nodeName.Contains("试炼"));
        }

        private static string GetEncounterTierLabel(EncounterVisualTier tier)
        {
            switch (tier)
            {
                case EncounterVisualTier.Elite:
                    return "当前为精英斗法";
                case EncounterVisualTier.MiniBoss:
                    return "当前为守关小Boss";
                case EncounterVisualTier.FinalBoss:
                    return "当前为终局Boss";
                default:
                    return "当前为常规斗法";
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
'@

$text = [regex]::Replace(
    $text,
    '        private static string GetBossPhaseLabel\(DemoBossPhase phase\)',
    {
        param($m)
        return $helpers + "`r`n`r`n" + $m.Value
    },
    1
)

$text = [regex]::Replace(
    $text,
    '        private Vector2 GetEnemyAnchor\(\)\s*\{[\s\S]*?\n        \}\s*\}\s*$',
    {
        param($m)
        return @'
        private Vector2 GetEnemyAnchor()
        {
            switch (GetEncounterVisualTier())
            {
                case EncounterVisualTier.FinalBoss:
                    return new Vector2(0.74f, 0.70f);
                case EncounterVisualTier.MiniBoss:
                    return new Vector2(0.77f, 0.63f);
                case EncounterVisualTier.Elite:
                    return new Vector2(0.80f, 0.57f);
                default:
                    return new Vector2(0.84f, 0.50f);
            }
        }
    }
}
'@
    },
    1
)

Set-Content -LiteralPath $Path -Encoding UTF8 -Value $text
