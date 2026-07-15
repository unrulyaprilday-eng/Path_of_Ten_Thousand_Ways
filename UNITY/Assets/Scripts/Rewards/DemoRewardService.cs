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
        private static readonly HashSet<string> LayerTwoOrLaterCards = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "sword_array",
            "wanjian_burst"
        };

        private static readonly HashSet<string> FocusComponentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "sword_focus",
            "summon_sword",
            "returning_array",
            "sword_rain",
            "sword_array",
            "wanjian_burst",
            "gongfa_sword_control_art",
            "gongfa_wanjian_return",
            "artifact_sword_box"
        };

        private static readonly string[] WanjianFocusPriority =
        {
            "sword_focus",
            "summon_sword",
            "returning_array",
            "sword_rain",
            "sheathe_edge",
            "sword_tide",
            "heaven_opening",
            "sword_array",
            "wanjian_burst"
        };

        private static readonly Dictionary<DemoSwordStyle, string[]> FocusCardPriority = new Dictionary<DemoSwordStyle, string[]>
        {
            [DemoSwordStyle.Wanjian] = WanjianFocusPriority,
            [DemoSwordStyle.Thunder] = new[] { "thunder_sword", "thunder_chain", "thunder_lead", "thunder_casket", "storm_sword_array", "thunder_prison", "heaven_thunder" },
            [DemoSwordStyle.Blood] = new[] { "blood_mark", "blood_edge_awakening", "scarlet_feast", "blood_guard", "blood_sword", "blood_tide_array", "blood_execution" },
            [DemoSwordStyle.General] = new[] { "sword_slash", "guard_step", "cloud_step", "spirit_draw", "meridian_breath", "jade_barrier" }
        };

        private static readonly Dictionary<DemoSwordStyle, string[]> WildcardCardPriority = new Dictionary<DemoSwordStyle, string[]>
        {
            [DemoSwordStyle.Wanjian] = new[] { "sword_rain", "returning_array", "sheathe_edge", "sword_tide", "heaven_opening", "sword_array", "wanjian_burst" },
            [DemoSwordStyle.Thunder] = new[] { "thunder_chain", "thunder_lead", "thunder_casket", "storm_sword_array", "thunder_prison", "heaven_thunder" },
            [DemoSwordStyle.Blood] = new[] { "blood_sword", "blood_edge_awakening", "scarlet_feast", "blood_guard", "blood_tide_array", "blood_execution" },
            [DemoSwordStyle.General] = new[] { "jade_barrier", "sword_rain", "thunder_prison", "blood_guard" }
        };

        private static readonly string[] GeneralUtilityCards =
        {
            "guard_step",
            "cloud_step",
            "spirit_draw",
            "meridian_breath",
            "jade_barrier"
        };

        private readonly Random random;
        private readonly List<DemoCard> cardPool = DemoCardLibrary.CreateRewardPool();

        public DemoRewardService()
        {
            random = new Random();
        }

        public DemoRewardService(int seed)
        {
            random = new Random(seed);
        }

        public List<DemoReward> CreateChoices(int layer, DemoRunState run)
        {
            DemoRewardContext context = DemoRewardContext.FromNode(run?.Map?.CurrentNode, run);
            context.Layer = Math.Max(1, layer);

            if (string.IsNullOrEmpty(context.RewardProfileId) && layer == 1 && (run?.BattlesWon ?? 0) == 0)
            {
                context.Source = DemoRewardSource.OpeningBattle;
                context.Tier = DemoRewardTier.Opening;
            }

            return CreateChoices(context, run);
        }

        public List<DemoReward> CreateChoices(DemoRewardContext context, DemoRunState run)
        {
            context = context ?? new DemoRewardContext();
            Random source = context.HasSeed ? new Random(context.Seed) : random;

            if (context.Source == DemoRewardSource.OpeningBattle
                || string.Equals(context.RewardProfileId, "reward_opening_battle", StringComparison.OrdinalIgnoreCase))
            {
                return CreateOpeningBattleChoices();
            }

            DemoSwordStyle focusStyle = run?.GetBuildStyle() ?? DemoSwordStyle.Wanjian;
            HashSet<string> usedCardIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> usedRewardNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            DemoReward focusReward = CreateFocusReward(context, focusStyle, run, source, usedCardIds)
                .WithSlot(DemoRewardSlot.Focus, BuildFocusDelta(context, run));
            TrackReward(focusReward, usedCardIds, usedRewardNames);

            DemoReward utilityReward = CreateUtilityReward(context, focusStyle, run, source, usedCardIds, usedRewardNames)
                .WithSlot(DemoRewardSlot.Utility, "补足当前生存或灵气缺口");
            TrackReward(utilityReward, usedCardIds, usedRewardNames);

            DemoReward wildcardReward = CreateWildcardReward(context, focusStyle, run, source, usedCardIds, usedRewardNames)
                .WithSlot(DemoRewardSlot.Wildcard, "高波动补强，不保证当前循环");

            if (wildcardReward == null || usedRewardNames.Contains(wildcardReward.Name))
            {
                wildcardReward = (run != null && run.CurrentHealth < run.MaxHealth
                        ? DemoReward.Heal()
                        : DemoReward.Upgrade())
                    .WithSlot(DemoRewardSlot.Wildcard, "通用资源回退");
            }

            return new List<DemoReward>
            {
                focusReward,
                utilityReward,
                wildcardReward
            };
        }

        public DemoReward CreateGuaranteedReward(string componentId, DemoRunState run)
        {
            switch (componentId)
            {
                case "survival_boss":
                    if (run == null || !run.HasBuildComponent("jade_barrier"))
                    {
                        return DemoReward.FromCard(CreateCard("jade_barrier"))
                            .WithSlot(DemoRewardSlot.Utility, "Boss 前补齐强防御牌");
                    }

                    if (!run.HasArtifact(DemoArtifactType.PurpleGourd))
                    {
                        return DemoReward.Artifact(DemoArtifactType.PurpleGourd)
                            .WithSlot(DemoRewardSlot.Utility, "Boss 前补齐持续减伤法器");
                    }

                    return DemoReward.Heal()
                        .WithSlot(DemoRewardSlot.Utility, "生存组件已齐，改为恢复生命");
                case "engine_wanjian":
                    if (run == null || !run.HasGongfa(DemoGongfaType.SwordControlArt))
                    {
                        return DemoReward.Gongfa(DemoGongfaType.SwordControlArt)
                            .WithSlot(DemoRewardSlot.Focus, "补齐万剑主修引擎");
                    }

                    return DemoReward.Artifact(DemoArtifactType.SwordBox)
                        .WithSlot(DemoRewardSlot.Focus, "补齐飞剑增殖法器");
                case "gongfa_sword_control_art":
                    return DemoReward.Gongfa(DemoGongfaType.SwordControlArt)
                        .WithSlot(DemoRewardSlot.Focus, "获得万剑主修引擎");
                case "gongfa_wanjian_return":
                    return DemoReward.Gongfa(DemoGongfaType.WanjianReturn)
                        .WithSlot(DemoRewardSlot.Focus, "获得高风险神通收束");
                case "artifact_sword_box":
                    return DemoReward.Artifact(DemoArtifactType.SwordBox)
                        .WithSlot(DemoRewardSlot.Focus, "获得飞剑增殖法器");
                default:
                    DemoCard card = CreateCard(componentId);
                    return card == null
                        ? DemoReward.Upgrade().WithSlot(DemoRewardSlot.Focus, "配置缺失时的资源回退")
                        : DemoReward.FromCard(card).WithSlot(DemoRewardSlot.Focus, "获得阶段保底组件");
            }
        }

        public static bool IsFocusComponent(DemoReward reward)
        {
            if (reward == null)
            {
                return false;
            }

            if (reward.Card != null && FocusComponentIds.Contains(reward.Card.Id))
            {
                return true;
            }

            if (reward.Type == DemoRewardType.Gongfa)
            {
                return reward.GongfaType == DemoGongfaType.SwordControlArt
                    || reward.GongfaType == DemoGongfaType.WanjianReturn;
            }

            return reward.Type == DemoRewardType.Artifact
                && reward.ArtifactType == DemoArtifactType.SwordBox;
        }

        private static List<DemoReward> CreateOpeningBattleChoices()
        {
            return new List<DemoReward>
            {
                DemoReward.FromCard(CreateCard("sword_focus"))
                    .WithSlot(DemoRewardSlot.Focus, "万剑启动组件 +1"),
                DemoReward.FromCard(CreateCard("cloud_step"))
                    .WithSlot(DemoRewardSlot.Utility, "防御与过牌组件 +1"),
                DemoReward.FromCard(CreateCard("spirit_draw"))
                    .WithSlot(DemoRewardSlot.Wildcard, "零费回灵组件 +1")
            };
        }

        private DemoReward CreateFocusReward(
            DemoRewardContext context,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            Random source,
            ISet<string> usedCardIds)
        {
            if (context.Layer >= 3 && !HasComponent(context, run, "wanjian_burst"))
            {
                return DemoReward.FromCard(CreateCard("wanjian_burst"));
            }

            if (context.Layer >= 2 && !HasComponent(context, run, "sword_array"))
            {
                return DemoReward.FromCard(CreateCard("sword_array"));
            }

            if (context.Layer >= 2
                && (run == null || (!run.HasGongfa(DemoGongfaType.SwordControlArt) && !run.HasArtifact(DemoArtifactType.SwordBox))))
            {
                return DemoReward.Gongfa(DemoGongfaType.SwordControlArt);
            }

            if ((context.ConsecutiveRewardsWithoutFocus >= 2 || context.Layer == 1)
                && !HasComponent(context, run, "sword_focus"))
            {
                return DemoReward.FromCard(CreateCard("sword_focus"));
            }

            DemoCard card = PickCardFromPriority(
                GetPriorityIds("card_reward_focus", focusStyle, FocusCardPriority),
                context,
                usedCardIds,
                source);

            if (card == null)
            {
                card = PickWeightedCard(
                    context,
                    focusStyle,
                    usedCardIds,
                    source,
                    candidate => candidate.Style == focusStyle || candidate.Style == DemoSwordStyle.General);
            }

            return DemoReward.FromCard((card ?? CreateCard("sword_focus")).Clone());
        }

        private DemoReward CreateUtilityReward(
            DemoRewardContext context,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            Random source,
            ISet<string> usedCardIds,
            ISet<string> usedRewardNames)
        {
            if (run != null && run.CurrentHealth <= run.MaxHealth / 2)
            {
                return DemoReward.Heal();
            }

            int supportCardCount = run?.Deck.Count(card => card.Type == DemoCardType.Defense || card.Type == DemoCardType.Resource) ?? 0;
            if (supportCardCount < 4 || context.Layer == 1)
            {
                DemoCard utilityCard = PickCardFromPriority(GeneralUtilityCards, context, usedCardIds, source);
                if (utilityCard != null)
                {
                    return DemoReward.FromCard(utilityCard.Clone());
                }
            }

            if (context.Layer >= 2
                && (run?.BonusEnergy ?? 0) == 0
                && !usedRewardNames.Contains("剑诀精修"))
            {
                return DemoReward.Upgrade();
            }

            if (context.Layer >= 3)
            {
                return DemoReward.Heal();
            }

            DemoCard fallback = PickWeightedCard(
                context,
                focusStyle,
                usedCardIds,
                source,
                candidate => candidate.Type == DemoCardType.Defense || candidate.Type == DemoCardType.Resource);

            return fallback == null ? DemoReward.Heal() : DemoReward.FromCard(fallback.Clone());
        }

        private DemoReward CreateWildcardReward(
            DemoRewardContext context,
            DemoSwordStyle focusStyle,
            DemoRunState run,
            Random source,
            ISet<string> usedCardIds,
            ISet<string> usedRewardNames)
        {
            if (context.Layer >= 3
                && context.RouteRisk == DemoRouteRisk.Risky
                && context.AllowsDivine
                && (run == null || !run.HasGongfa(DemoGongfaType.WanjianReturn)))
            {
                return DemoReward.Gongfa(DemoGongfaType.WanjianReturn);
            }

            if (context.Layer >= 2
                && (run == null || !run.HasArtifact(DemoArtifactType.SwordBox))
                && !usedRewardNames.Contains(DemoArtifactLibrary.Get(DemoArtifactType.SwordBox).Name))
            {
                return DemoReward.Artifact(DemoArtifactType.SwordBox);
            }

            DemoCard wildcardCard = PickCardFromPriority(
                GetPriorityIds("card_reward_wildcard", focusStyle, WildcardCardPriority),
                context,
                usedCardIds,
                source);

            if (wildcardCard == null)
            {
                wildcardCard = PickWeightedCard(
                    context,
                    focusStyle,
                    usedCardIds,
                    source,
                    candidate => candidate.Quality >= DemoQuality.Mysterious
                        || candidate.Type == DemoCardType.FlyingSword
                        || (context.AllowsFinisher && candidate.Type == DemoCardType.Finisher));
            }

            return wildcardCard == null ? null : DemoReward.FromCard(wildcardCard.Clone());
        }

        private DemoCard PickCardFromPriority(
            IEnumerable<string> ids,
            DemoRewardContext context,
            ISet<string> usedCardIds,
            Random source)
        {
            if (ids == null)
            {
                return null;
            }

            List<string> candidates = ids
                .Where(id => !string.IsNullOrEmpty(id))
                .Where(id => !usedCardIds.Contains(id))
                .Where(id => IsCardAllowed(id, context))
                .ToList();

            if (candidates.Count == 0)
            {
                return null;
            }

            return CreateCard(candidates[source.Next(candidates.Count)]);
        }

        private DemoCard PickWeightedCard(
            DemoRewardContext context,
            DemoSwordStyle focusStyle,
            ISet<string> usedCardIds,
            Random source,
            Func<DemoCard, bool> predicate)
        {
            List<DemoCard> weighted = new List<DemoCard>();

            foreach (DemoCard card in cardPool)
            {
                if (usedCardIds.Contains(card.Id)
                    || !IsCardAllowed(card.Id, context)
                    || (predicate != null && !predicate(card)))
                {
                    continue;
                }

                weighted.Add(card);

                if (card.Style == focusStyle)
                {
                    weighted.Add(card);
                    weighted.Add(card);
                }

                if (context.Layer >= 2 && card.Type == DemoCardType.FlyingSword)
                {
                    weighted.Add(card);
                }

                if (context.Layer >= 3 && context.AllowsFinisher
                    && (card.Type == DemoCardType.Finisher || card.Quality >= DemoQuality.Earth))
                {
                    weighted.Add(card);
                    weighted.Add(card);
                }
            }

            return weighted.Count == 0 ? null : weighted[source.Next(weighted.Count)];
        }

        private static bool IsCardAllowed(string cardId, DemoRewardContext context)
        {
            if (string.IsNullOrEmpty(cardId))
            {
                return false;
            }

            if (context.Layer < 2 && LayerTwoOrLaterCards.Contains(cardId))
            {
                return false;
            }

            if ((context.Layer < 3 || !context.AllowsFinisher)
                && string.Equals(cardId, "wanjian_burst", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            DemoCard card = CreateCard(cardId);
            return card != null && (context.AllowsFinisher || card.Type != DemoCardType.Finisher);
        }

        private static bool HasComponent(DemoRewardContext context, DemoRunState run, string componentId)
        {
            return context.HasComponent(componentId) || (run != null && run.HasBuildComponent(componentId));
        }

        private static void TrackReward(
            DemoReward reward,
            ISet<string> usedCardIds,
            ISet<string> usedRewardNames)
        {
            if (reward == null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(reward.Name))
            {
                usedRewardNames.Add(reward.Name);
            }

            if (reward.Card != null)
            {
                usedCardIds.Add(reward.Card.Id);
            }
        }

        private static string BuildFocusDelta(DemoRewardContext context, DemoRunState run)
        {
            if (context.Layer >= 3 && (run == null || !run.HasBuildComponent("wanjian_burst")))
            {
                return "距万剑收束：补入万剑诀";
            }

            if (context.Layer >= 2 && (run == null || !run.HasBuildComponent("sword_array")))
            {
                return "距剑阵运转：补入小诛仙剑阵";
            }

            return "当前主轴：飞剑增殖与剑意积累";
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

        private static DemoCard CreateCard(string id)
        {
            return DemoCardLibrary.Create(id);
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
                    "普通战后在矿灯下调息并定向补启动缺口，以恢复换取稳定推进。",
                    new DemoMapNode(1, DemoNodeType.Battle, "矿口余烬", "node_l1_stable_battle", "enemy_mine_ember", "reward_layer1_standard", null),
                    new DemoMapNode(1, DemoNodeType.Training, "矿灯下调息", "node_l1_stable_training", null, null, "action_training_stable_l1"),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路", "node_l1_stable_choice_l2", null, null, "choose_route_layer_2")),
                CreateRouteReward(
                    "route_branch_risky",
                    focusStyle,
                    DemoQuality.Heaven,
                    "险",
                    "冒险路线",
                    "塌井深处",
                    "连续挑战两名精英，不获免费恢复，以更早获得两次高档战斗奖励。",
                    new DemoMapNode(1, DemoNodeType.Battle, "塌井守卫", "node_l1_risky_elite_1", "enemy_collapsed_well_guard", "reward_layer1_elite", null),
                    new DemoMapNode(1, DemoNodeType.Battle, "井底邪影", "node_l1_risky_elite_2", "enemy_well_bottom_shadow", "reward_layer1_elite", null),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路", "node_l1_risky_choice_l2", null, null, "choose_route_layer_2")),
                CreateRouteReward(
                    "route_branch_build",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "构",
                    "构筑路线",
                    "旧账暗室",
                    "先悟法再整备，并以普通战验证循环，用较少随机性补齐万剑启动组件。",
                    new DemoMapNode(1, DemoNodeType.Training, "暗室悟法", "node_l1_build_training", null, null, "action_training_focus_l1"),
                    new DemoMapNode(1, DemoNodeType.Shop, "旧账整备", "node_l1_build_prepare", null, null, "action_prepare_build_l1"),
                    new DemoMapNode(1, DemoNodeType.Battle, "符债残影", "node_l1_build_battle", "enemy_talisman_debt_wraith", "reward_layer1_build", null),
                    new DemoMapNode(2, DemoNodeType.RouteChoice, "第二层岔路", "node_l1_build_choice_l2", null, null, "choose_route_layer_2"))
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
                    "先定向补小诛仙剑阵，再打一场普通战，稳定获得第二层核心。",
                    new DemoMapNode(2, DemoNodeType.Training, "定向补缺", "node_l2_stable_fill", null, null, "action_directed_fill_l2"),
                    new DemoMapNode(2, DemoNodeType.Battle, "劫云守卫", "node_l2_stable_battle", "enemy_calamity_guard", "reward_layer2_standard", null),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口", "node_l2_stable_choice_l3", null, null, "choose_route_layer_3")),
                CreateRouteReward(
                    "route_middle_aggressive",
                    focusStyle,
                    DemoQuality.Heaven,
                    "锋",
                    "冒险路线",
                    "追锋破阵",
                    "连续挑战两名第二层精英，以更高压力换取更早、更高品质的引擎补强。",
                    new DemoMapNode(2, DemoNodeType.Battle, "裂空剑煞", "node_l2_aggressive_elite_1", "enemy_rift_sword_fiend", "reward_layer2_elite", null),
                    new DemoMapNode(2, DemoNodeType.Battle, "雷崖执兵", "node_l2_aggressive_elite_2", "enemy_thunder_cliff_guard", "reward_layer2_elite", null),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口", "node_l2_aggressive_choice_l3", null, null, "choose_route_layer_3")),
                CreateRouteReward(
                    "route_middle_artifact",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "器",
                    "构筑路线",
                    "秘器共振",
                    "先在核心整备中补御剑诀或剑匣缺口，再进入精英试炼。",
                    new DemoMapNode(2, DemoNodeType.Shop, "核心整备", "node_l2_artifact_prepare", null, null, "action_core_prepare_l2"),
                    new DemoMapNode(2, DemoNodeType.Battle, "镜雷试炼", "node_l2_artifact_elite", "enemy_mirror_thunder_trial", "reward_layer2_core", null),
                    new DemoMapNode(3, DemoNodeType.RouteChoice, "第三层路口", "node_l2_artifact_choice_l3", null, null, "choose_route_layer_3"))
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
                    "Boss 前恢复并完成整备，以当前成形构筑稳定渡劫。",
                    new DemoMapNode(3, DemoNodeType.Shop, "Boss 前整备", "node_l3_stable_prepare", null, null, "action_prepare_boss"),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身", "node_l3_stable_boss", "enemy_tianjie_avatar", "reward_boss_completion", null),
                    new DemoMapNode(4, DemoNodeType.Result, "一世结算", "node_l3_stable_result", null, null, "show_victory_result")),
                CreateRouteReward(
                    "route_final_seclusion",
                    focusStyle,
                    DemoQuality.Mysterious,
                    "悟",
                    "爆发路线",
                    "闭关悟道",
                    "先定向补万剑诀，再完成整备，把完整爆发窗口留给天劫。",
                    new DemoMapNode(3, DemoNodeType.Training, "终结补强", "node_l3_seclusion_finisher", null, null, "action_finisher_fill_l3"),
                    new DemoMapNode(3, DemoNodeType.Shop, "Boss 前整备", "node_l3_seclusion_prepare", null, null, "action_prepare_boss"),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身", "node_l3_seclusion_boss", "enemy_tianjie_avatar", "reward_boss_completion", null),
                    new DemoMapNode(4, DemoNodeType.Result, "一世结算", "node_l3_seclusion_result", null, null, "show_victory_result")),
                CreateRouteReward(
                    "route_final_desperate",
                    focusStyle,
                    DemoQuality.Immortal,
                    "劫",
                    "背水路线",
                    "背水破劫",
                    "渡劫前再战一名高奖精英，并允许万剑归宗出现，以风险抬高上限。",
                    new DemoMapNode(3, DemoNodeType.Battle, "劫前守门", "node_l3_desperate_elite", "enemy_calamity_gatekeeper", "reward_layer3_high", null),
                    new DemoMapNode(3, DemoNodeType.Boss, "天劫化身", "node_l3_desperate_boss", "enemy_tianjie_avatar", "reward_boss_completion", null),
                    new DemoMapNode(4, DemoNodeType.Result, "一世结算", "node_l3_desperate_result", null, null, "show_victory_result"))
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
                new DemoMapRoutePlan(routePlanId, fallbackName, fallbackDescription, fallbackNodes),
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
