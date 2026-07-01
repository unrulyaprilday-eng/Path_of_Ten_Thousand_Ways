using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Cards
{
    public static class DemoCardLibrary
    {
        public static List<DemoCard> CreateStarterDeck()
        {
            if (DemoConfigRepository.TryCreateDeckFromPool("starter_general", out List<DemoCard> configuredDeck))
            {
                return configuredDeck;
            }

            return new List<DemoCard>
            {
                Create("sword_slash").Clone(),
                Create("sword_slash").Clone(),
                Create("guard_step").Clone(),
                Create("guard_step").Clone(),
                Create("cloud_step").Clone(),
                Create("jade_barrier").Clone()
            };
        }

        public static List<DemoCard> CreateStarterDeck(DemoSwordStyle style)
        {
            if (DemoConfigRepository.TryCreateDeckFromPool($"starter_{style.ToString().ToLowerInvariant()}", out List<DemoCard> configuredDeck))
            {
                return configuredDeck;
            }

            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return new List<DemoCard>
                    {
                        Create("sword_slash").Clone(),
                        Create("sword_slash").Clone(),
                        Create("guard_step").Clone(),
                        Create("guard_step").Clone(),
                        Create("cloud_step").Clone(),
                        Create("sword_focus").Clone(),
                        Create("sword_focus").Clone()
                    };
                case DemoSwordStyle.Thunder:
                    return new List<DemoCard>
                    {
                        Create("sword_slash").Clone(),
                        Create("sword_slash").Clone(),
                        Create("guard_step").Clone(),
                        Create("guard_step").Clone(),
                        Create("cloud_step").Clone(),
                        Create("thunder_chain").Clone(),
                        Create("thunder_lead").Clone()
                    };
                case DemoSwordStyle.Blood:
                    return new List<DemoCard>
                    {
                        Create("sword_slash").Clone(),
                        Create("sword_slash").Clone(),
                        Create("guard_step").Clone(),
                        Create("guard_step").Clone(),
                        Create("cloud_step").Clone(),
                        Create("blood_mark").Clone(),
                        Create("blood_mark").Clone()
                    };
                default:
                    return CreateStarterDeck();
            }
        }

        public static string GetBasicPathCardId(DemoSwordStyle style)
        {
            if (DemoConfigRepository.TryGetBasicPathCardId(style, out string configuredCardId))
            {
                return configuredCardId;
            }

            switch (style)
            {
                case DemoSwordStyle.Wanjian:
                    return "sword_focus";
                case DemoSwordStyle.Thunder:
                    return "thunder_chain";
                case DemoSwordStyle.Blood:
                    return "blood_mark";
                default:
                    return "sword_slash";
            };
        }

        public static List<DemoCard> CreateRewardPool()
        {
            if (DemoConfigRepository.TryCreateDeckFromPool("reward_pool", out List<DemoCard> configuredPool))
            {
                return configuredPool;
            }

            return new List<DemoCard>
            {
                Create("sword_slash"),
                Create("sword_focus"),
                Create("summon_sword"),
                Create("sword_array"),
                Create("sword_rain"),
                Create("returning_array"),
                Create("sheathe_edge"),
                Create("sword_tide"),
                Create("heaven_opening"),
                Create("thunder_sword"),
                Create("thunder_chain"),
                Create("thunder_lead"),
                Create("thunder_casket"),
                Create("storm_sword_array"),
                Create("thunder_prison"),
                Create("blood_mark"),
                Create("blood_sword"),
                Create("blood_edge_awakening"),
                Create("scarlet_feast"),
                Create("blood_guard"),
                Create("blood_tide_array"),
                Create("guard_step"),
                Create("cloud_step"),
                Create("spirit_draw"),
                Create("meridian_breath"),
                Create("jade_barrier"),
                Create("wanjian_burst"),
                Create("heaven_thunder"),
                Create("blood_execution")
            };
        }

        public static DemoCard Create(string id)
        {
            if (DemoConfigRepository.TryCreateCard(id, out DemoCard configuredCard))
            {
                return configuredCard;
            }

            switch (id)
            {
                case "sword_slash":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "剑气斩",
                        IconGlyph = "斩",
                        Type = DemoCardType.Attack,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Mortal,
                        Cost = 1,
                        Damage = 8,
                        SwordIntent = 1
                    };
                case "guard_step":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "剑遁",
                        IconGlyph = "遁",
                        Type = DemoCardType.Defense,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Mortal,
                        Cost = 1,
                        Block = 8
                    };
                case "cloud_step":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "踏云遁",
                        IconGlyph = "踏",
                        Type = DemoCardType.Defense,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        Block = 6,
                        Draw = 1
                    };
                case "sword_focus":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "凝剑诀",
                        IconGlyph = "凝",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        SwordIntent = 3
                    };
                case "summon_sword":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "御剑诀",
                        IconGlyph = "御",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        TemporarySwords = 2,
                        SwordIntent = 1
                    };
                case "sword_rain":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "剑雨横空",
                        IconGlyph = "雨",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Earth,
                        Cost = 2,
                        Damage = 5,
                        TemporarySwords = 2,
                        SwordIntent = 1
                    };
                case "returning_array":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "回锋剑阵",
                        IconGlyph = "回",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Mysterious,
                        Cost = 1,
                        TemporarySwords = 1,
                        SwordIntent = 2,
                        Draw = 1
                    };
                case "sword_array":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "小诛仙剑阵",
                        IconGlyph = "阵",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Mysterious,
                        Cost = 2,
                        TemporarySwords = 3,
                        PermanentSword = true
                    };
                case "sheathe_edge":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "藏锋诀",
                        IconGlyph = "藏",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Earth,
                        Cost = 1,
                        SpecialEffect = DemoCardSpecialEffect.SheatheEdge,
                        RulesOverride = "本回合飞剑不自动攻击，改为把当前飞剑化为剑意并积蓄锋势，为下一次斩击收束伤害。"
                    };
                case "sword_tide":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "剑潮叠浪",
                        IconGlyph = "潮",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Heaven,
                        Cost = 2,
                        Damage = 10,
                        TemporarySwords = 2,
                        ConsumeAllSwordIntent = true
                    };
                case "heaven_opening":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "开天一剑",
                        IconGlyph = "开",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Immortal,
                        Cost = 3,
                        Damage = 14,
                        SpecialEffect = DemoCardSpecialEffect.HeavenOpening,
                        RulesOverride = "消耗全部剑意与已积蓄的锋势，打出极高单体斩击。若被镜像复制，复制体沿用本次消耗的锋势。"
                    };
                case "thunder_sword":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "雷剑",
                        IconGlyph = "雷",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        Damage = 5,
                        Shock = 3,
                        TemporarySwords = 1
                    };
                case "thunder_chain":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "引雷诀",
                        IconGlyph = "引",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Mysterious,
                        Cost = 1,
                        Damage = 4,
                        Shock = 5
                    };
                case "thunder_lead":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "引雷入刃",
                        IconGlyph = "引",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Mysterious,
                        Cost = 1,
                        Damage = 6,
                        Shock = 4,
                        Draw = 1
                    };
                case "thunder_casket":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "封雷匣",
                        IconGlyph = "匣",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Earth,
                        Cost = 1,
                        Shock = 4,
                        SpecialEffect = DemoCardSpecialEffect.ThunderSeal,
                        RulesOverride = "施加 4 感电。本回合感电不引爆，改为继续蓄存，并让下一次雷击额外增强。"
                    };
                case "storm_sword_array":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "奔雷剑阵",
                        IconGlyph = "霆",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Earth,
                        Cost = 2,
                        Damage = 8,
                        Shock = 4,
                        TemporarySwords = 2
                    };
                case "thunder_prison":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "雷狱镇压",
                        IconGlyph = "狱",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Heaven,
                        Cost = 2,
                        Damage = 12,
                        Shock = 6,
                        Block = 6
                    };
                case "blood_mark":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "血痕",
                        IconGlyph = "痕",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        Damage = 4,
                        Bleed = 4
                    };
                case "blood_sword":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "血剑胚",
                        IconGlyph = "胚",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Mysterious,
                        Cost = 2,
                        Damage = 6,
                        Bleed = 6,
                        PermanentSword = true
                    };
                case "blood_edge_awakening":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "噬命开锋",
                        IconGlyph = "噬",
                        Type = DemoCardType.Resource,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Earth,
                        Cost = 0,
                        EnergyGain = 2,
                        SwordIntent = 2,
                        TemporarySwords = 2,
                        SelfDamage = 6,
                        RulesOverride = "失去 6 点生命，获得 2 灵气、2 剑意，并备好 2 把临时飞剑。"
                    };
                case "scarlet_feast":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "血祭养锋",
                        IconGlyph = "祭",
                        Type = DemoCardType.Status,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Mysterious,
                        Cost = 1,
                        Damage = 5,
                        Bleed = 3,
                        EnergyGain = 1
                    };
                case "blood_guard":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "煞气护体",
                        IconGlyph = "煞",
                        Type = DemoCardType.Defense,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Spirit,
                        Cost = 1,
                        Block = 8,
                        Bleed = 2
                    };
                case "blood_tide_array":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "血潮剑阵",
                        IconGlyph = "潮",
                        Type = DemoCardType.FlyingSword,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Earth,
                        Cost = 2,
                        Damage = 8,
                        Bleed = 5,
                        TemporarySwords = 1
                    };
                case "spirit_draw":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "聚灵换息",
                        IconGlyph = "灵",
                        Type = DemoCardType.Resource,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Spirit,
                        Cost = 0,
                        EnergyGain = 1,
                        Draw = 1
                    };
                case "meridian_breath":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "引气归元",
                        IconGlyph = "归",
                        Type = DemoCardType.Resource,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Spirit,
                        Cost = 0,
                        EnergyGain = 1,
                        SwordIntent = 1
                    };
                case "jade_barrier":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "玄门护身",
                        IconGlyph = "护",
                        Type = DemoCardType.Defense,
                        Style = DemoSwordStyle.General,
                        Quality = DemoQuality.Mysterious,
                        Cost = 2,
                        Block = 14,
                        Draw = 1
                    };
                case "wanjian_burst":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "万剑诀",
                        IconGlyph = "万",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Wanjian,
                        Quality = DemoQuality.Earth,
                        Cost = 3,
                        Damage = 12,
                        ConsumeAllSwordIntent = true
                    };
                case "heaven_thunder":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "九霄雷剑",
                        IconGlyph = "霄",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Thunder,
                        Quality = DemoQuality.Heaven,
                        Cost = 3,
                        Damage = 16,
                        Shock = 8
                    };
                case "blood_execution":
                    return new DemoCard
                    {
                        Id = id,
                        Name = "血祭斩",
                        IconGlyph = "祭",
                        Type = DemoCardType.Finisher,
                        Style = DemoSwordStyle.Blood,
                        Quality = DemoQuality.Heaven,
                        Cost = 2,
                        Damage = 10,
                        Bleed = 8
                    };
                default:
                    return CreateFallbackBasicCard();
            }
        }

        private static DemoCard CreateFallbackBasicCard()
        {
            return new DemoCard
            {
                Id = "sword_slash",
                Name = "剑气斩",
                IconGlyph = "斩",
                Type = DemoCardType.Attack,
                Style = DemoSwordStyle.General,
                Quality = DemoQuality.Mortal,
                Cost = 1,
                Damage = 8,
                SwordIntent = 1
            };
        }
    }
}
