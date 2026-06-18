using PathOfTenThousandWays.Demo.Cards;

namespace PathOfTenThousandWays.Demo.Battle
{
    public enum DemoBattlePresentationStepType
    {
        PhaseShift,
        CardCast,
        SwordVolley,
        BossCharge,
        EnemyAttack,
        Victory,
        Defeat
    }

    public sealed class DemoBattlePresentationStep
    {
        public DemoBattlePresentationStepType Type;
        public DemoSwordStyle Style;
        public string Label;
        public int Damage;
        public int HitCount;
        public bool HeavyImpact;
        public bool TriggerShock;
        public bool TriggerBleed;
        public bool IsBossAction;
        public int PlayerShockDelta;

        public static DemoBattlePresentationStep PhaseShift(string label)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.PhaseShift,
                Label = label,
                IsBossAction = true
            };
        }

        public static DemoBattlePresentationStep Card(DemoCard card, int damage)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.CardCast,
                Style = card.Style,
                Label = card.Name,
                Damage = damage,
                HitCount = card.TemporarySwords > 0 ? 2 : 1,
                HeavyImpact = card.Type == DemoCardType.Finisher || damage >= 12,
                TriggerShock = card.Shock > 0,
                TriggerBleed = card.Bleed > 0
            };
        }

        public static DemoBattlePresentationStep SwordVolley(DemoSwordStyle style, int swordCount, int damage, bool triggerShock)
        {
            string labelPrefix = style == DemoSwordStyle.Thunder
                ? "雷剑"
                : style == DemoSwordStyle.Blood
                    ? "血剑"
                    : "飞剑";

            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.SwordVolley,
                Style = style,
                Label = swordCount <= 1 ? $"1 把{labelPrefix}试锋" : $"{swordCount} 把{labelPrefix}齐发",
                Damage = damage,
                HitCount = swordCount,
                HeavyImpact = swordCount >= 4 || damage >= 16,
                TriggerShock = triggerShock
            };
        }

        public static DemoBattlePresentationStep Enemy(string enemyName, int damage, bool isBoss)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.EnemyAttack,
                Style = DemoSwordStyle.General,
                Label = enemyName,
                Damage = damage,
                HitCount = isBoss ? 2 : 1,
                HeavyImpact = isBoss || damage >= 12,
                IsBossAction = isBoss
            };
        }

        public static DemoBattlePresentationStep Charge(string label, int playerShockDelta)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.BossCharge,
                Label = label,
                IsBossAction = true,
                TriggerShock = playerShockDelta > 0,
                PlayerShockDelta = playerShockDelta
            };
        }

        public static DemoBattlePresentationStep Victory()
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.Victory,
                Label = "剑光破局"
            };
        }

        public static DemoBattlePresentationStep Defeat()
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.Defeat,
                Label = "道心受创"
            };
        }
    }
}
