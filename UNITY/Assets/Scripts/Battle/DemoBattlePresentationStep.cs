using System;
using PathOfTenThousandWays.Demo.Cards;

namespace PathOfTenThousandWays.Demo.Battle
{
    public enum DemoBattlePresentationStepType
    {
        BattleStart,
        PhaseShift,
        CardCast,
        CardDraw,
        SwordVolley,
        SwordStored,
        BossCharge,
        EnemyAttack,
        Victory,
        TargetDefeated,
        Defeat
    }

    public sealed class DemoBattlePresentationStep
    {
        public long Sequence;
        public float BattleTime;
        public DemoBattlePresentationStepType Type;
        public DemoSwordStyle Style;
        public string SourceId;
        public string SourceCombatantId;
        public string TargetCombatantId;
        public string[] AffectedTargetIds;
        public string Label;
        public int Damage;
        public int HitCount;
        public bool HeavyImpact;
        public bool TriggerShock;
        public bool TriggerBleed;
        public bool IsBossAction;
        public int PlayerShockDelta;

        public static DemoBattlePresentationStep BattleStart(string enemyName, bool isBoss, string sourceId = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.BattleStart,
                SourceId = sourceId,
                SourceCombatantId = sourceId,
                TargetCombatantId = "player",
                Style = DemoSwordStyle.General,
                Label = enemyName,
                IsBossAction = isBoss
            };
        }

        public static DemoBattlePresentationStep PhaseShift(string label, string targetCombatantId = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.PhaseShift,
                Label = label,
                TargetCombatantId = targetCombatantId,
                AffectedTargetIds = string.IsNullOrEmpty(targetCombatantId) ? null : new[] { targetCombatantId },
                IsBossAction = true
            };
        }

        public static DemoBattlePresentationStep Card(
            DemoCard card,
            int damage,
            string sourceCombatantId = "player",
            string targetCombatantId = null,
            string[] affectedTargetIds = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.CardCast,
                SourceId = card.Id,
                SourceCombatantId = sourceCombatantId,
                TargetCombatantId = targetCombatantId,
                AffectedTargetIds = affectedTargetIds,
                Style = card.Style,
                Label = card.Name,
                Damage = damage,
                HitCount = card.TemporarySwords > 0 ? 2 : 1,
                HeavyImpact = card.Type == DemoCardType.Finisher || damage >= 12,
                TriggerShock = card.Shock > 0,
                TriggerBleed = card.Bleed > 0
            };
        }

        public static DemoBattlePresentationStep Draw(DemoCard card)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.CardDraw,
                SourceId = card == null ? null : card.Id,
                Style = card == null ? DemoSwordStyle.General : card.Style,
                Label = card == null ? "抽牌" : card.Name,
                HitCount = 1
            };
        }

        public static DemoBattlePresentationStep SwordVolley(
            DemoSwordStyle style,
            int swordCount,
            int damage,
            bool triggerShock,
            string targetCombatantId = null,
            string[] affectedTargetIds = null)
        {
            string labelPrefix = style == DemoSwordStyle.Thunder
                ? "雷剑"
                : style == DemoSwordStyle.Blood
                    ? "血剑"
                    : "飞剑";

            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.SwordVolley,
                SourceId = "auto_sword_volley",
                SourceCombatantId = "player",
                TargetCombatantId = targetCombatantId,
                AffectedTargetIds = affectedTargetIds,
                Style = style,
                Label = swordCount <= 1 ? $"1 把{labelPrefix}试锋" : $"{swordCount} 把{labelPrefix}齐发",
                Damage = damage,
                HitCount = swordCount,
                HeavyImpact = swordCount >= 4 || damage >= 16,
                TriggerShock = triggerShock
            };
        }

        public static DemoBattlePresentationStep SwordStored(
            DemoSwordStyle style,
            int swordCount,
            int gainedIntent)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.SwordStored,
                SourceId = "auto_sword_store",
                Style = style,
                Label = $"{swordCount} 把飞剑收锋入鞘",
                HitCount = swordCount,
                Damage = gainedIntent,
                HeavyImpact = swordCount >= 5
            };
        }

        public static DemoBattlePresentationStep Enemy(string enemyName, int damage, bool isBoss, string sourceId = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.EnemyAttack,
                SourceId = sourceId,
                SourceCombatantId = sourceId,
                TargetCombatantId = "player",
                Style = DemoSwordStyle.General,
                Label = enemyName,
                Damage = damage,
                HitCount = isBoss ? 2 : 1,
                HeavyImpact = isBoss || damage >= 12,
                IsBossAction = isBoss
            };
        }

        public static DemoBattlePresentationStep Charge(string label, int playerShockDelta, string sourceCombatantId = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.BossCharge,
                SourceId = "boss_charge",
                SourceCombatantId = sourceCombatantId,
                TargetCombatantId = "player",
                Label = label,
                IsBossAction = true,
                TriggerShock = playerShockDelta > 0,
                PlayerShockDelta = playerShockDelta
            };
        }

        public static DemoBattlePresentationStep Victory(string defeatedTargetId = null)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.Victory,
                SourceId = "battle_victory",
                SourceCombatantId = "player",
                TargetCombatantId = defeatedTargetId,
                AffectedTargetIds = string.IsNullOrEmpty(defeatedTargetId) ? null : new[] { defeatedTargetId },
                Label = "剑光破局"
            };
        }

        public static DemoBattlePresentationStep Defeat()
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.Defeat,
                SourceId = "battle_defeat",
                Label = "道心受创"
            };
        }

        public static DemoBattlePresentationStep TargetDefeated(string targetCombatantId, string label)
        {
            return new DemoBattlePresentationStep
            {
                Type = DemoBattlePresentationStepType.TargetDefeated,
                SourceId = "target_defeated",
                SourceCombatantId = "player",
                TargetCombatantId = targetCombatantId,
                AffectedTargetIds = string.IsNullOrEmpty(targetCombatantId) ? null : new[] { targetCombatantId },
                Label = label,
                HeavyImpact = true
            };
        }
    }
}
