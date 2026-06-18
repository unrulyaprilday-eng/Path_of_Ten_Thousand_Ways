using System;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public enum DemoRewardType
    {
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
        public DemoCard Card;
        public DemoGongfaType GongfaType;
        public DemoArtifactType ArtifactType;
        public DemoMapRoutePlan RoutePlan;
        public DemoSwordStyle RouteStyle;
        public DemoQuality RouteQuality;
        public string RouteGlyph;
        public string RouteTag;

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
            return new DemoReward
            {
                Type = DemoRewardType.Heal,
                Name = "调息",
                Description = "恢复 18 点生命。"
            };
        }

        public static DemoReward Upgrade()
        {
            return new DemoReward
            {
                Type = DemoRewardType.Upgrade,
                Name = "剑诀精修",
                Description = "最大灵气 +1。"
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
