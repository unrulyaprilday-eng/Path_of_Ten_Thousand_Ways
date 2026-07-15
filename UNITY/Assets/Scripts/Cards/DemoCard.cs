using System;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Cards
{
    public enum DemoCardType
    {
        Attack,
        FlyingSword,
        Status,
        Defense,
        Resource,
        Finisher
    }

    public enum DemoSwordStyle
    {
        Wanjian,
        Thunder,
        Blood,
        General
    }

    public enum DemoCardSpecialEffect
    {
        None,
        SheatheEdge,
        HeavenOpening,
        ThunderSeal
    }

    [Serializable]
    public sealed class DemoCard
    {
        public string Id;
        public string Name;
        public string IconGlyph;
        public DemoCardType Type;
        public DemoSwordStyle Style;
        public DemoQuality Quality;
        public int Cost;
        public int Damage;
        public int Block;
        public int Draw;
        public int EnergyGain;
        public int SwordIntent;
        public int Shock;
        public int Bleed;
        public int TemporarySwords;
        public bool PermanentSword;
        public bool ConsumeAllSwordIntent;
        public int SelfDamage;
        public DemoCardSpecialEffect SpecialEffect;
        public string RulesOverride;

        public DemoCard Clone()
        {
            return (DemoCard)MemberwiseClone();
        }

        public string GetRulesText()
        {
            if (!string.IsNullOrEmpty(RulesOverride))
            {
                return NormalizeRealtimeRules(RulesOverride);
            }

            string text = string.Empty;

            if (Damage > 0)
            {
                text += $"造成 {Damage} 点伤害。";
            }

            if (Block > 0)
            {
                text += $"获得 {Block} 点护盾。";
            }

            if (SwordIntent > 0)
            {
                text += $"获得 {SwordIntent} 剑意。";
            }

            if (Shock > 0)
            {
                text += $"施加 {Shock} 感电。";
            }

            if (Bleed > 0)
            {
                text += $"施加 {Bleed} 流血。";
            }

            if (TemporarySwords > 0)
            {
                text += $"生成 {TemporarySwords} 把临时飞剑，可参与接下来 3 次飞剑齐射。";
            }

            if (PermanentSword)
            {
                text += "永久飞剑 +1。";
            }

            if (Draw > 0)
            {
                text += $"抽 {Draw} 张牌。";
            }

            if (EnergyGain > 0)
            {
                text += $"回复 {EnergyGain} 灵气。";
            }

            if (SelfDamage > 0)
            {
                text += $"失去 {SelfDamage} 点生命。";
            }

            if (ConsumeAllSwordIntent)
            {
                text += "消耗全部剑意，每点剑意追加一次飞剑斩击。";
            }

            return text;
        }

        private static string NormalizeRealtimeRules(string rules)
        {
            return rules
                .Replace("本回合飞剑不自动攻击", "下一次飞剑齐射不会攻击")
                .Replace("本回合感电不引爆", "下一次飞剑齐射不引爆感电");
        }
    }
}
