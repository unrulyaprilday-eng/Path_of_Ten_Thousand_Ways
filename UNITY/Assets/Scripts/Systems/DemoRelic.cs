using System.Collections.Generic;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoRelicDefinition
    {
        public string Name;
        public string IconGlyph;
        public string Description;
        public string Style;
        public DemoQuality Quality;
    }

    public static class DemoRelicLibrary
    {
        private static readonly Dictionary<string, DemoRelicDefinition> Definitions = new Dictionary<string, DemoRelicDefinition>
        {
            ["剑骨"] = new DemoRelicDefinition
            {
                Name = "剑骨",
                IconGlyph = "骨",
                Style = "万剑组件",
                Quality = DemoQuality.Spirit,
                Description = "永久飞剑 +1，让演武高点更早成形。"
            },
            ["剑冢残碑"] = new DemoRelicDefinition
            {
                Name = "剑冢残碑",
                IconGlyph = "碑",
                Style = "万剑引擎",
                Quality = DemoQuality.Earth,
                Description = "每次消耗至少 6 点剑意时，额外凝成临时飞剑。"
            },
            ["万剑剑匣"] = new DemoRelicDefinition
            {
                Name = "万剑剑匣",
                IconGlyph = "匣",
                Style = "万剑爆发",
                Quality = DemoQuality.Heaven,
                Description = "飞剑齐发后有概率再追斩一轮剑潮。"
            },
            ["雷心"] = new DemoRelicDefinition
            {
                Name = "雷心",
                IconGlyph = "心",
                Style = "雷剑组件",
                Quality = DemoQuality.Spirit,
                Description = "飞剑引爆感电时，额外追加 3 点雷击伤害。"
            },
            ["九霄雷印"] = new DemoRelicDefinition
            {
                Name = "九霄雷印",
                IconGlyph = "印",
                Style = "雷剑爆发",
                Quality = DemoQuality.Earth,
                Description = "引爆 6 层以上感电时，再降下 6 点雷击。"
            },
            ["血魔珠"] = new DemoRelicDefinition
            {
                Name = "血魔珠",
                IconGlyph = "珠",
                Style = "血剑爆发",
                Quality = DemoQuality.Earth,
                Description = "生命每失去 10%，造成的伤害提高 8%。"
            },
            ["血剑胚"] = new DemoRelicDefinition
            {
                Name = "血剑胚",
                IconGlyph = "胚",
                Style = "血剑组件",
                Quality = DemoQuality.Mysterious,
                Description = "飞剑演武附加 2 层流血，让斩杀越滚越深。"
            },
            ["护心镜"] = new DemoRelicDefinition
            {
                Name = "护心镜",
                IconGlyph = "护",
                Style = "生存结构",
                Quality = DemoQuality.Earth,
                Description = "Boss 战首次致死伤害会被截停在 1 点生命前。"
            },
            ["聚灵符"] = new DemoRelicDefinition
            {
                Name = "聚灵符",
                IconGlyph = "符",
                Style = "规则修改",
                Quality = DemoQuality.Spirit,
                Description = "每回合第一张 1 费牌改为免费。"
            },
            ["残破古镜"] = new DemoRelicDefinition
            {
                Name = "残破古镜",
                IconGlyph = "镜",
                Style = "复制强化",
                Quality = DemoQuality.Heaven,
                Description = "每场战斗第一次复制效果会额外强化。"
            }
        };

        public static DemoRelicDefinition Get(string relicName)
        {
            if (!string.IsNullOrEmpty(relicName) && Definitions.TryGetValue(relicName, out DemoRelicDefinition definition))
            {
                return definition;
            }

            return new DemoRelicDefinition
            {
                Name = string.IsNullOrEmpty(relicName) ? "未知遗物" : relicName,
                IconGlyph = "遗",
                Style = "未记录",
                Quality = DemoQuality.Mortal,
                Description = "尚未录入的遗物。"
            };
        }

        public static bool TryGet(string relicName, out DemoRelicDefinition definition)
        {
            return Definitions.TryGetValue(relicName, out definition);
        }
    }
}
