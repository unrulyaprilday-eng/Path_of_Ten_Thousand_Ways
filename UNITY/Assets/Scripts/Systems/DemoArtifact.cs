namespace PathOfTenThousandWays.Demo.Systems
{
    public enum DemoArtifactType
    {
        SwordBox,
        HaotianMirror,
        PurpleGourd,
        ThunderSeal
    }

    public sealed class DemoArtifactDefinition
    {
        public DemoArtifactType Type;
        public string Name;
        public string IconGlyph;
        public string Description;
        public string Style;
        public DemoQuality Quality;
    }

    public static class DemoArtifactLibrary
    {
        public static DemoArtifactDefinition Get(DemoArtifactType type)
        {
            if (DemoConfigRepository.TryGetArtifact(type, out DemoArtifactDefinition configured))
            {
                return configured;
            }

            switch (type)
            {
                case DemoArtifactType.SwordBox:
                    return new DemoArtifactDefinition
                    {
                        Type = type,
                        Name = "剑匣",
                        IconGlyph = "匣",
                        Style = "万剑核心",
                        Quality = DemoQuality.Earth,
                        Description = "战斗开始时永久飞剑 +1，且飞剑牌额外生成 1 把临时飞剑。"
                    };
                case DemoArtifactType.HaotianMirror:
                    return new DemoArtifactDefinition
                    {
                        Type = type,
                        Name = "昊天镜",
                        IconGlyph = "镜",
                        Style = "复制核心",
                        Quality = DemoQuality.Heaven,
                        Description = "每场战斗首次飞剑牌或终结牌结算两次。"
                    };
                case DemoArtifactType.PurpleGourd:
                    return new DemoArtifactDefinition
                    {
                        Type = type,
                        Name = "紫金葫芦",
                        IconGlyph = "葫",
                        Style = "生存转化",
                        Quality = DemoQuality.Mysterious,
                        Description = "每次敌方攻击吸收 4 点伤害，并立即返还 1 点灵气。"
                    };
                case DemoArtifactType.ThunderSeal:
                    return new DemoArtifactDefinition
                    {
                        Type = type,
                        Name = "雷印",
                        IconGlyph = "印",
                        Style = "雷剑核心",
                        Quality = DemoQuality.Earth,
                        Description = "感电牌额外施加 2 层感电，飞剑引爆感电时追加 4 点雷击伤害。"
                    };
                default:
                    return Get(DemoArtifactType.SwordBox);
            }
        }
    }
}
