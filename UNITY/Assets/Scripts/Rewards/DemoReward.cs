using System;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public enum DemoRewardType
    {
        Root,
        Journey,
        OpeningScene,
        Route,
        Card,
        Gongfa,
        Artifact,
        Relic,
        Upgrade,
        Heal
    }

    [Serializable]
    public sealed class DemoReward
    {
        public DemoRewardType Type;
        public string Name;
        public string Description;
        public string Summary;
        public DemoCard Card;
        public DemoGongfaType GongfaType;
        public DemoArtifactType ArtifactType;
        public DemoMapRoutePlan RoutePlan;
        public DemoSwordStyle RouteStyle;
        public DemoQuality RouteQuality;
        public string RouteGlyph;
        public string RouteTag;
        public DemoRootDefinition Root;
        public DemoJourneyLineDefinition JourneyLine;
        public DemoRegionDefinition Region;

        public static DemoReward Route(DemoMapRoutePlan routePlan, DemoSwordStyle style, DemoQuality quality, string glyph, string tag)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Route,
                Name = routePlan.Name,
                Description = routePlan.Description,
                RoutePlan = routePlan,
                RouteStyle = style,
                RouteQuality = quality,
                RouteGlyph = glyph,
                RouteTag = tag
            };
        }

        public static DemoReward FromRoot(DemoRootDefinition root)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Root,
                Name = root.Name,
                Description = root.Summary,
                Summary = root.UnlockCondition,
                Root = root
            };
        }

        public static DemoReward Journey(DemoJourneyLineDefinition line, DemoRootDefinition root)
        {
            string summary = string.IsNullOrEmpty(line.CarryItemEffect)
                ? $"启程信物：{line.CarryItemName}"
                : $"启程信物：{line.CarryItemName} | {line.CarryItemEffect}";

            return new DemoReward
            {
                Type = DemoRewardType.Journey,
                Name = line.Title,
                Description = line.OriginText,
                Summary = $"{(root == null ? "未知根脚" : root.Name)} | {summary}",
                Root = root,
                JourneyLine = line
            };
        }

        public static DemoReward OpeningScene(DemoRegionDefinition region, DemoJourneyLineDefinition line)
        {
            string carryText = line == null || string.IsNullOrEmpty(line.CarryItemName)
                ? "启程信物待定"
                : $"启程信物：{line.CarryItemName}";
            string riskText = line == null || string.IsNullOrEmpty(line.RiskLevel)
                ? "首境待定"
                : "首境候选";

            return new DemoReward
            {
                Type = DemoRewardType.OpeningScene,
                Name = region.Name,
                Description = region.Description,
                Summary = $"{carryText} | {riskText}",
                JourneyLine = line,
                Region = region
            };
        }

        public static DemoReward FromCard(DemoCard card)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Card,
                Name = card.Name,
                Description = card.GetRulesText(),
                Card = card
            };
        }

        public static DemoReward Heal()
        {
            int healAmount = DemoConfigRepository.GetIntConstant("battle", "heal_reward_amount", 18);
            return new DemoReward
            {
                Type = DemoRewardType.Heal,
                Name = "调息",
                Description = $"恢复 {healAmount} 点生命。"
            };
        }

        public static DemoReward Upgrade()
        {
            int energyGain = DemoConfigRepository.GetIntConstant("battle", "upgrade_reward_energy_bonus", 1);
            return new DemoReward
            {
                Type = DemoRewardType.Upgrade,
                Name = "剑诀精修",
                Description = $"最大灵气 +{energyGain}。"
            };
        }

        public static DemoReward Relic(string name, string description)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Relic,
                Name = name,
                Description = description
            };
        }

        public static DemoReward Relic(string relicName)
        {
            DemoRelicDefinition definition = DemoRelicLibrary.Get(relicName);
            return new DemoReward
            {
                Type = DemoRewardType.Relic,
                Name = definition.Name,
                Description = $"{definition.Style} | {definition.Description}"
            };
        }

        public static DemoReward Artifact(DemoArtifactType artifactType)
        {
            DemoArtifactDefinition definition = DemoArtifactLibrary.Get(artifactType);
            return new DemoReward
            {
                Type = DemoRewardType.Artifact,
                Name = definition.Name,
                Description = $"{definition.Style} | {definition.Description}",
                ArtifactType = artifactType
            };
        }

        public static DemoReward Gongfa(DemoGongfaType gongfaType)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(gongfaType);
            return new DemoReward
            {
                Type = DemoRewardType.Gongfa,
                Name = definition.Name,
                Description = $"{definition.Title} | {definition.Description}",
                GongfaType = gongfaType
            };
        }
    }
}
