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
                return RulesOverride;
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
                text += $"本回合生成 {TemporarySwords} 临时飞剑。";
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
    }
}
