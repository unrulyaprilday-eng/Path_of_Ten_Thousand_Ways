using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Cards;

namespace PathOfTenThousandWays.Demo.Systems
{
    public enum DemoGongfaSlot
    {
        Main,
        Support,
        Divine
    }

    public enum DemoGongfaType
    {
        None,
        SwordControlArt,
        ThunderScripture,
        BloodFiendCanon,
        SwordHeartResonance,
        LightningMeridians,
        BloodRefiningBody,
        WanjianReturn,
        HeavenlyThunderEdict,
        BloodPrisonExecution
    }

    public sealed class DemoGongfaDefinition
    {
        public DemoGongfaType Type;
        public DemoGongfaSlot Slot;
        public DemoSwordStyle Style;
        public string Name;
        public string IconGlyph;
        public string Title;
        public string Description;
        public DemoQuality Quality;
    }

    public static class DemoGongfaLibrary
    {
        public static DemoGongfaDefinition Get(DemoGongfaType type)
        {
            if (DemoConfigRepository.TryGetGongfa(type, out DemoGongfaDefinition configured))
            {
                return configured;
            }

            switch (type)
            {
                case DemoGongfaType.SwordControlArt:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Main,
                        Style = DemoSwordStyle.Wanjian,
                        Name = "御剑诀",
                        IconGlyph = "剑",
                        Title = "主修剑诀",
                        Quality = DemoQuality.Earth,
                        Description = "每回合第一张飞剑牌额外生成 1 把临时飞剑，让演武从起手就进入剑势。"
                    };
                case DemoGongfaType.ThunderScripture:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Main,
                        Style = DemoSwordStyle.Thunder,
                        Name = "九霄神雷诀",
                        IconGlyph = "雷",
                        Title = "主修雷法",
                        Quality = DemoQuality.Heaven,
                        Description = "感电被飞剑引爆时追加雷击，让伤害高点留在演武结算而不是多出牌。"
                    };
                case DemoGongfaType.BloodFiendCanon:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Main,
                        Style = DemoSwordStyle.Blood,
                        Name = "血煞经",
                        IconGlyph = "煞",
                        Title = "主修血法",
                        Quality = DemoQuality.Heaven,
                        Description = "敌人带有流血时，飞剑齐发追加伤害并回补气血，越打越凶。"
                    };
                case DemoGongfaType.SwordHeartResonance:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Support,
                        Style = DemoSwordStyle.Wanjian,
                        Name = "剑心通明",
                        IconGlyph = "心",
                        Title = "辅修心法",
                        Quality = DemoQuality.Spirit,
                        Description = "每回合开始获得 1 点剑意。若本回合只规划 2 张以内牌，飞剑演武获得额外增伤。"
                    };
                case DemoGongfaType.LightningMeridians:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Support,
                        Style = DemoSwordStyle.Thunder,
                        Name = "引雷入窍",
                        IconGlyph = "窍",
                        Title = "辅修心法",
                        Quality = DemoQuality.Mysterious,
                        Description = "飞剑齐发前先灌入 2 层感电，让雷击伤害更多在自动结算里爆开。"
                    };
                case DemoGongfaType.BloodRefiningBody:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Support,
                        Style = DemoSwordStyle.Blood,
                        Name = "血炼归元",
                        IconGlyph = "血",
                        Title = "辅修心法",
                        Quality = DemoQuality.Mysterious,
                        Description = "流血结算会回复少量生命并凝聚剑意，让血剑流在演武中持续滚起。"
                    };
                case DemoGongfaType.WanjianReturn:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Divine,
                        Style = DemoSwordStyle.Wanjian,
                        Name = "万剑归宗",
                        IconGlyph = "宗",
                        Title = "神通",
                        Quality = DemoQuality.Immortal,
                        Description = "当剑意或飞剑规模成型时，演武阶段追加一轮剑阵归潮。"
                    };
                case DemoGongfaType.HeavenlyThunderEdict:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Divine,
                        Style = DemoSwordStyle.Thunder,
                        Name = "九天引雷",
                        IconGlyph = "霆",
                        Title = "神通",
                        Quality = DemoQuality.Immortal,
                        Description = "感电引爆后召来天雷追击，把 Build 的高点集中到自动收尾。"
                    };
                case DemoGongfaType.BloodPrisonExecution:
                    return new DemoGongfaDefinition
                    {
                        Type = type,
                        Slot = DemoGongfaSlot.Divine,
                        Style = DemoSwordStyle.Blood,
                        Name = "血狱断生",
                        IconGlyph = "狱",
                        Title = "神通",
                        Quality = DemoQuality.Immortal,
                        Description = "敌人流血足够深时，在演武阶段触发血狱斩杀并回复生命。"
                    };
                default:
                    return new DemoGongfaDefinition
                    {
                        Type = DemoGongfaType.None,
                        Slot = DemoGongfaSlot.Main,
                        Style = DemoSwordStyle.General,
                        Name = "未悟功法",
                        IconGlyph = "空",
                        Title = "空位",
                        Quality = DemoQuality.Mortal,
                        Description = "尚未选择。"
                    };
            }
        }

        public static List<DemoGongfaType> GetTypesForSlot(DemoGongfaSlot slot)
        {
            List<DemoGongfaType> configuredTypes = DemoConfigRepository.GetGongfaTypesForSlot(slot);
            if (configuredTypes.Count > 0)
            {
                return configuredTypes;
            }

            switch (slot)
            {
                case DemoGongfaSlot.Main:
                    return new List<DemoGongfaType>
                    {
                        DemoGongfaType.SwordControlArt,
                        DemoGongfaType.ThunderScripture,
                        DemoGongfaType.BloodFiendCanon
                    };
                case DemoGongfaSlot.Support:
                    return new List<DemoGongfaType>
                    {
                        DemoGongfaType.SwordHeartResonance,
                        DemoGongfaType.LightningMeridians,
                        DemoGongfaType.BloodRefiningBody
                    };
                case DemoGongfaSlot.Divine:
                    return new List<DemoGongfaType>
                    {
                        DemoGongfaType.WanjianReturn,
                        DemoGongfaType.HeavenlyThunderEdict,
                        DemoGongfaType.BloodPrisonExecution
                    };
                default:
                    return new List<DemoGongfaType>();
            }
        }
    }
}
