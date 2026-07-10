using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public sealed class DemoRewardService
    {
        private readonly Random random = new Random();
        private readonly List<DemoCard> cardPool = DemoCardLibrary.CreateRewardPool();

        private static readonly Dictionary<DemoSwordStyle, string[]> FocusCardPriority = new Dictionary<DemoSwordStyle, string[]>
        {
            [DemoSwordStyle.Wanjian] = new[] { "sword_focus", "summon_sword", "returning_array", "sheathe_edge", "sword_rain", "sword_array", "sword_tide", "heaven_opening", "wanjian_burst" },
            [DemoSwordStyle.Thunder] = new[] { "thunder_sword", "thunder_chain", "thunder_lead", "thunder_casket", "storm_sword_array", "thunder_prison", "heaven_thunder" },
            [DemoSwordStyle.Blood] = new[] { "blood_mark", "blood_edge_awakening", "scarlet_feast", "blood_guard", "blood_sword", "blood_tide_array", "blood_execution" },
            [DemoSwordStyle.General] = new[] { "sword_slash", "guard_step", "cloud_step", "spirit_draw", "meridian_breath", "jade_barrier" }
        };

        private static readonly Dictionary<DemoSwordStyle, string[]> WildcardCardPriority = new Dictionary<DemoSwordStyle, string[]>
        {
            [DemoSwordStyle.Wanjian] = new[] { "sword_array", "sword_rain", "returning_array", "sheathe_edge", "sword_tide", "heaven_opening", "wanjian_burst" },
            [DemoSwordStyle.Thunder] = new[] { "thunder_chain", "thunder_lead", "thunder_casket", "storm_sword_array", "thunder_prison", "heaven_thunder" },
            [DemoSwordStyle.Blood] = new[] { "blood_sword", "blood_edge_awakening", "scarlet_feast", "blood_guard", "blood_tide_array", "blood_execution" },
            [DemoSwordStyle.General] = new[] { "jade_barrier", "sword_array", "thunder_prison", "blood_execution" }
        };

        private static readonly string[] GeneralUtilityCards =
        {
            "guard_step",
            "cloud_step",
            "spirit_draw",
            "meridian_breath",
            "jade_barrier"
        };

        public List<DemoReward> CreateChoices(int layer, DemoRunState run)
        {
            DemoSwordStyle focusStyle = run.GetBuildStyle();
            HashSet<string> usedCardIds = new HashSet<string>();
            HashSet<string> usedRewardNames = new HashSet<string>();

            DemoReward focusReward = CreateFocusReward(layer, focusStyle, run, usedCardIds, usedRewardNames);
            usedRewardNames.Add(focusReward.Name);
            if (focusReward.Card != null)
            {
                usedCardIds.Add(focusReward.Card.Id);
            }

            DemoReward utilityReward = CreateUtilityReward(layer, focusStyle, run, usedCardIds, usedRewardNames);
            usedRewardNames.Add(utilityReward.Name);
            if (utilityReward.Card != null)
            {
                usedCardIds.Add(utilityReward.Card.Id);
            }

            DemoReward wildcardReward = CreateWildcardReward(layer, focusStyle, run, usedCardIds, usedRewardNames);

            return new List<DemoReward>
            {
                focusReward,
                utilityReward,
                wildcardReward
            };
        }

        private DemoReward CreateFocusReward(
            int layer,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            HashSet<string> usedCardIds,
            HashSet<string> usedRewardNames)
        {
            string nextRelic = GetNextStyleRelic(run, focusStyle, layer);
            if (!string.IsNullOrEmpty(nextRelic) && !usedRewardNames.Contains(nextRelic))
            {
                return DemoReward.Relic(nextRelic);
            }

            DemoCard card = PickCardFromPriority(
                GetPriorityIds("card_reward_focus", focusStyle, FocusCardPriority),
                usedCardIds);

            if (card == null)
            {
                card = PickWeightedCard(
                    layer,
                    focusStyle,
                    usedCardIds,
                    candidate => candidate.Style == focusStyle || candidate.Style == DemoSwordStyle.General);
            }

            return DemoReward.FromCard(card.Clone());
        }

        private DemoReward CreateUtilityReward(
            int layer,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            HashSet<string> usedCardIds,
            HashSet<string> usedRewardNames)
        {
            if (run.CurrentHealth <= run.MaxHealth / 2)
            {
                return DemoReward.Heal();
            }

            int supportCardCount = run.Deck.Count(card => card.Type == DemoCardType.Defense || card.Type == DemoCardType.Resource);
            if (supportCardCount < 4 || layer == 1)
            {
                string[] utilityPriority = focusStyle == DemoSwordStyle.Blood
                    ? new[] { "blood_guard", "scarlet_feast", "cloud_step", "spirit_draw", "meridian_breath" }
                    : GeneralUtilityCards;

                DemoCard utilityCard = PickCardFromPriority(utilityPriority, usedCardIds);
                if (utilityCard != null)
                {
                    return DemoReward.FromCard(utilityCard.Clone());
                }
            }

            if (layer >= 2 && run.BonusEnergy == 0 && !usedRewardNames.Contains("剑诀精修"))
            {
                return DemoReward.Upgrade();
            }

            if (layer >= 3)
            {
                return DemoReward.Heal();
            }

            DemoCard fallback = PickCardFromPriority(GeneralUtilityCards, usedCardIds)
                ?? PickWeightedCard(layer, focusStyle, usedCardIds, candidate => candidate.Type == DemoCardType.Defense || candidate.Type == DemoCardType.Resource);
            return DemoReward.FromCard(fallback.Clone());
        }

        private DemoReward CreateWildcardReward(
            int layer,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            HashSet<string> usedCardIds,
            HashSet<string> usedRewardNames)
        {
            string generalRelic = GetNextGeneralRelic(run, layer);
            if (!string.IsNullOrEmpty(generalRelic) && !usedRewardNames.Contains(generalRelic))
            {
                return DemoReward.Relic(generalRelic);
            }

            DemoCard wildcardCard = PickCardFromPriority(
                GetPriorityIds("card_reward_wildcard", focusStyle, WildcardCardPriority),
                usedCardIds);

            if (wildcardCard == null)
            {
                wildcardCard = PickWeightedCard(
                    layer,
                    focusStyle,
                    usedCardIds,
                    candidate => candidate.Quality >= DemoQuality.Mysterious || candidate.Type == DemoCardType.FlyingSword || candidate.Type == DemoCardType.Finisher);
            }

            if (wildcardCard != null)
            {
                return DemoReward.FromCard(wildcardCard.Clone());
            }

            return run.CurrentHealth < run.MaxHealth ? DemoReward.Heal() : DemoReward.Upgrade();
        }

        private DemoCard PickCardFromPriority(IEnumerable<string> ids, ISet<string> usedCardIds)
        {
            if (ids == null)
            {
                return null;
            }

            List<string> candidates = ids
                .Where(id => !string.IsNullOrEmpty(id) && !usedCardIds.Contains(id))
                .ToList();

            if (candidates.Count == 0)
            {
                return null;
            }

            return DemoCardLibrary.Create(candidates[random.Next(candidates.Count)]);
        }

        private DemoCard PickWeightedCard(
            int layer,
            DemoSwordStyle focusStyle,
            ISet<string> usedCardIds,
            Func<DemoCard, bool> predicate)
        {
            List<DemoCard> weighted = new List<DemoCard>();

            foreach (DemoCard card in cardPool)
            {
                if (usedCardIds.Contains(card.Id))
                {
                    continue;
                }

                if (predicate != null && !predicate(card))
                {
                    continue;
                }

                weighted.Add(card);

                if (card.Style == focusStyle)
                {
                    weighted.Add(card);
                    weighted.Add(card);
                }

                if (layer >= 2 && card.Type == DemoCardType.FlyingSword)
                {
                    weighted.Add(card);
                }

                if (layer >= 3 && (card.Type == DemoCardType.Finisher || card.Quality >= DemoQuality.Earth))
                {
                    weighted.Add(card);
                    weighted.Add(card);
                }
            }

            return weighted.Count == 0 ? null : weighted[random.Next(weighted.Count)];
        }

        private static IEnumerable<string> GetPriorityIds(
            string service,
            DemoSwordStyle style,
            IReadOnlyDictionary<DemoSwordStyle, string[]> fallback)
        {
            IReadOnlyList<string> configuredIds = DemoConfigRepository.GetRewardPriorityRefs(service, style, "card");
            if (configuredIds.Count > 0)
            {
                return configuredIds;
            }

            return fallback.ContainsKey(style) ? fallback[style] : fallback[DemoSwordStyle.General];
        }

        private static string GetNextStyleRelic(DemoRunState run, DemoSwordStyle focusStyle, int layer)
        {
            if (run == null || layer < 2)
            {
                return null;
            }

            string[] relics = GetStyleRelicPriority(focusStyle);
            int ownedCount = relics.Count(run.HasRelic);
            int targetCount = Math.Min(layer - 1, relics.Length);

            if (ownedCount >= targetCount)
            {
                return null;
            }

            return relics.FirstOrDefault(relicName => !run.HasRelic(relicName));
        }

        private static string GetNextGeneralRelic(DemoRunState run, int layer)
        {
            if (run == null || layer < 2)
            {
                return null;
            }

            List<string> priority = new List<string>();
            if (run.HasArtifact(DemoArtifactType.HaotianMirror))
            {
                priority.Add("残破古镜");
            }

            if (run.CurrentHealth <= run.MaxHealth * 2 / 3)
            {
                priority.Add("护心镜");
            }

            priority.Add("聚灵符");
            priority.Add("护心镜");

            foreach (string relicName in priority)
            {
                if (!run.HasRelic(relicName))
                {
                    return relicName;
                }
            }

            return null;
        }

        private static string[] GetStyleRelicPriority(DemoSwordStyle focusStyle)
        {
            switch (focusStyle)
            {
                case DemoSwordStyle.Thunder:
                    return new[] { "雷心", "九霄雷印" };
                case DemoSwordStyle.Blood:
                    return new[] { "血剑胚", "血魔珠" };
                case DemoSwordStyle.Wanjian:
                default:
                    return new[] { "剑骨", "剑冢残碑", "万剑剑匣" };
            }
        }
    }

    public sealed class DemoRouteRewardService
    {
        public List<DemoReward> CreateChoices(int layer, DemoRunState run)
        {
            DemoSwordStyle focusStyle = run.GetBuildStyle();

            switch (layer)
            {
                case 1:
                    return CreateOpeningChoices(focusStyle);
                case 2:
                    return CreateMiddleChoices(focusStyle);
                default:
                    return CreateFinalChoices(focusStyle);
            }
        }

        private static List<DemoReward> CreateOpeningChoices(DemoSwordStyle focusStyle)
        {
            return new List<DemoReward>
            {
                CreateRouteReward(
                    "route_branch_stable",
                    focusStyle,
                    DemoQuality.Spirit,
                    "稳",
                    "稳定路线",
                    "矿口余烬",
                    "沿旧矿外沿稳步推进，先把首战所得消化成第一批飞剑组件，再用修炼节点补足生存和灵气缺口。",
                    new DemoMapNode(1, DemoNodeType.Battle, "矿口余烬"),
                    new DemoMapNode(1, DemoNodeType.Reward, "飞剑组件"),
                    new DemoMapNode(1, DemoNodeType.Training, "矿灯下调息"),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路")),
                CreateRouteReward(
                    "route_branch_risky",
                    focusStyle,
                    DemoQuality.Heaven,
                    "险",
                    "冒险路线",
                    "塌井深处",
                    "直接下探塌井深处，连续承受更高战斗压力，提前换取高价值补强和更快的成型速度。",
                    new DemoMapNode(1, DemoNodeType.Battle, "塌井守卫"),
                    new DemoMapNode(1, DemoNodeType.Reward, "塌井秘藏"),
                    new DemoMapNode(1, DemoNodeType.Battle, "井底邪影"),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路")),
                CreateRouteReward(
                    "route_branch_build",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "构",
                    "构筑路线",
                    "旧账暗室",
                    "先绕进旧账暗室补功法与器物，再打一场验证当前循环，适合把首战奖励整理成明确构筑方向。",
                    new DemoMapNode(1, DemoNodeType.Training, "暗室悟法"),
                    new DemoMapNode(1, DemoNodeType.Shop, "旧账整备"),
                    new DemoMapNode(1, DemoNodeType.Battle, "符债残影"),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路"))
            };
        }

        private static List<DemoReward> CreateMiddleChoices(DemoSwordStyle focusStyle)
        {
            return new List<DemoReward>
            {
                CreateRouteReward(
                    "route_middle_stable",
                    focusStyle,
                    DemoQuality.Spirit,
                    "稳",
                    "稳定路线",
                    "稳修收束",
                    $"先用修炼补齐{GetStyleFocusText(focusStyle)}，再打一场常规斗法，让 build 在中段更稳地收束。",
                    new DemoMapNode(2, DemoNodeType.Training, "法器与功法补强"),
                    new DemoMapNode(2, DemoNodeType.Battle, "劫云守卫"),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口")),
                CreateRouteReward(
                    "route_middle_aggressive",
                    focusStyle,
                    DemoQuality.Heaven,
                    "锋",
                    "冒险路线",
                    "追锋破阵",
                    $"连续走两场斗法换一次额外补强，适合把{GetStyleFocusText(focusStyle)}直接压到中段高点。",
                    new DemoMapNode(2, DemoNodeType.Battle, "裂空剑煞"),
                    new DemoMapNode(2, DemoNodeType.Reward, "中段补强"),
                    new DemoMapNode(2, DemoNodeType.Battle, "雷崖执兵"),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口")),
                CreateRouteReward(
                    "route_middle_artifact",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "器",
                    "构筑路线",
                    "秘器共振",
                    $"先去整备节点改写规则，再打一场试炼，让{GetStyleFocusText(focusStyle)}提前接上法器与神通。",
                    new DemoMapNode(2, DemoNodeType.Shop, "秘器整备"),
                    new DemoMapNode(2, DemoNodeType.Battle, "镜雷试炼"),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口"))
            };
        }

        private static List<DemoReward> CreateFinalChoices(DemoSwordStyle focusStyle)
        {
            return new List<DemoReward>
            {
                CreateRouteReward(
                    "route_final_stable",
                    focusStyle,
                    DemoQuality.Spirit,
                    "备",
                    "稳守路线",
                    "整备冲劫",
                    $"先整备再渡劫，让{GetStyleFocusText(focusStyle)}在 Boss 战里稳定落地。",
                    new DemoMapNode(3, DemoNodeType.Shop, "Boss 前整备"),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身"),
                    new DemoMapNode(4, DemoNodeType.Victory, "一世修行完成")),
                CreateRouteReward(
                    "route_final_seclusion",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "悟",
                    "爆发路线",
                    "闭关悟道",
                    "再闭关一轮补足神通和续航，把爆发窗口完整地留给天劫降临那一刻。",
                    new DemoMapNode(3, DemoNodeType.Training, "渡劫前闭关"),
                    new DemoMapNode(3, DemoNodeType.Shop, "Boss 前整备"),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身"),
                    new DemoMapNode(4, DemoNodeType.Victory, "一世修行完成")),
                CreateRouteReward(
                    "route_final_desperate",
                    focusStyle,
                    DemoQuality.Immortal,
                    "劫",
                    "背水路线",
                    "背水破劫",
                    "再打一场劫前守门换最后一次补强，用更高的风险把斩杀上限也一并抬上去。",
                    new DemoMapNode(3, DemoNodeType.Battle, "劫前守门"),
                    new DemoMapNode(3, DemoNodeType.Reward, "最后补强"),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身"),
                    new DemoMapNode(4, DemoNodeType.Victory, "一世修行完成"))
            };
        }

        private static DemoReward CreateRouteReward(
            string routePlanId,
            DemoSwordStyle focusStyle,
            DemoQuality fallbackQuality,
            string fallbackGlyph,
            string fallbackTag,
            string fallbackName,
            string fallbackDescription,
            params DemoMapNode[] fallbackNodes)
        {
            if (DemoConfigRepository.TryGetRoutePlan(routePlanId, out DemoRoutePlanDefinition configured))
            {
                DemoSwordStyle routeStyle = configured.RouteStyle == DemoSwordStyle.General
                    ? focusStyle
                    : configured.RouteStyle;

                return DemoReward.Route(
                    configured.Plan,
                    routeStyle,
                    configured.RouteQuality,
                    configured.RouteGlyph,
                    configured.RouteTag);
            }

            return DemoReward.Route(
                new DemoMapRoutePlan(fallbackName, fallbackDescription, fallbackNodes),
                focusStyle,
                fallbackQuality,
                fallbackGlyph,
                fallbackTag);
        }

        private static List<DemoReward> OrderByFocusStyle(List<DemoReward> rewards, DemoSwordStyle focusStyle)
        {
            return rewards
                .OrderByDescending(reward => reward.RouteStyle == focusStyle)
                .ThenBy(reward => reward.Name)
                .ToList();
        }

        private static string GetStyleFocusText(DemoSwordStyle style)
        {
            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "飞剑数量和剑潮";
                case DemoSwordStyle.Thunder:
                    return "感电连锁和雷击";
                case DemoSwordStyle.Blood:
                    return "流血深度和斩杀";
                default:
                    return "飞剑与功法联动";
            }
        }
    }
}
