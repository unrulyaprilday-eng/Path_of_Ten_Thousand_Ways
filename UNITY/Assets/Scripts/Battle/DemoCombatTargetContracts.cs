using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Combatants;

namespace PathOfTenThousandWays.Demo.Battle
{
    /// <summary>
    /// The small, stable identity and targeting surface shared by automatic attacks,
    /// techniques, statuses and presentation code.
    /// </summary>
    public interface IDemoCombatTarget
    {
        string CombatantId { get; }
        string DefinitionId { get; }
        string PositionId { get; }
        int Depth { get; }
        bool CanLock { get; }
        bool RequiredForVictory { get; }
        int ThreatPriority { get; }
        bool IsDead { get; }
        bool IsActive { get; }
        DemoIntentState Intent { get; }
    }

    [Serializable]
    public sealed class DemoCombatTarget : IDemoCombatTarget
    {
        private bool forcedDead;
        private int health;
        private readonly DemoCombatant combatant;

        public string CombatantId { get; private set; }
        public string DefinitionId { get; private set; }
        public string PositionId { get; private set; }
        public int Depth { get; private set; }
        public bool CanLock { get; set; }
        public bool RequiredForVictory { get; set; }
        public int ThreatPriority { get; set; }
        public bool IsActive { get; set; }
        public DemoIntentState Intent { get; set; }
        public DemoCombatant Combatant { get { return combatant; } }
        public int Health { get { return combatant == null ? health : combatant.Health; } }
        public bool IsDead { get { return forcedDead || (combatant != null && combatant.IsDead) || (combatant == null && health <= 0); } }

        public DemoCombatTarget(
            string combatantId,
            string definitionId,
            string positionId,
            int depth,
            bool canLock,
            bool requiredForVictory,
            int threatPriority,
            int maxHealth = 1)
            : this(
                combatantId,
                definitionId,
                positionId,
                depth,
                canLock,
                requiredForVictory,
                threatPriority,
                new DemoCombatant(definitionId, Math.Max(1, maxHealth)))
        {
        }

        public DemoCombatTarget(
            string combatantId,
            string definitionId,
            string positionId,
            int depth,
            bool canLock,
            bool requiredForVictory,
            int threatPriority,
            DemoCombatant combatant)
        {
            if (string.IsNullOrEmpty(combatantId))
            {
                throw new ArgumentException("CombatantId is required.", "combatantId");
            }

            if (string.IsNullOrEmpty(definitionId))
            {
                throw new ArgumentException("DefinitionId is required.", "definitionId");
            }

            if (string.IsNullOrEmpty(positionId))
            {
                throw new ArgumentException("PositionId is required.", "positionId");
            }

            CombatantId = combatantId;
            DefinitionId = definitionId;
            PositionId = positionId;
            Depth = depth;
            CanLock = canLock;
            RequiredForVictory = requiredForVictory;
            ThreatPriority = threatPriority;
            IsActive = true;
            Intent = DemoIntentState.None;
            this.combatant = combatant;
            health = combatant == null ? 1 : Math.Max(1, combatant.MaxHealth);
        }

        public void MarkDead()
        {
            forcedDead = true;
            IsActive = false;
            if (combatant != null)
            {
                combatant.Health = 0;
            }
            else
            {
                health = 0;
            }
        }

        public int ApplyDamage(int amount)
        {
            if (amount <= 0 || IsDead)
            {
                return 0;
            }

            int healthDamage = combatant == null
                ? Math.Min(health, amount)
                : combatant.TakeDamage(amount);
            if (combatant == null)
            {
                health = Math.Max(0, health - healthDamage);
            }
            if (IsDead)
            {
                IsActive = false;
            }
            return healthDamage;
        }

        public override string ToString()
        {
            return CombatantId + "@" + PositionId;
        }
    }

    [Serializable]
    public sealed class DemoIntentState
    {
        public static DemoIntentState None
        {
            get
            {
                return new DemoIntentState
                {
                    BehaviorId = string.Empty,
                    TargetCombatantId = string.Empty,
                    RemainingSeconds = float.PositiveInfinity,
                    IsPending = false,
                    IsKnown = false,
                    ThreatPriority = 0
                };
            }
        }

        public string BehaviorId { get; set; }
        public string TargetCombatantId { get; set; }
        public float RemainingSeconds { get; set; }
        public bool IsPending { get; set; }
        public bool IsKnown { get; set; }
        public int ThreatPriority { get; set; }

        public DemoIntentState Clone()
        {
            return new DemoIntentState
            {
                BehaviorId = BehaviorId,
                TargetCombatantId = TargetCombatantId,
                RemainingSeconds = RemainingSeconds,
                IsPending = IsPending,
                IsKnown = IsKnown,
                ThreatPriority = ThreatPriority
            };
        }
    }

    [Serializable]
    public sealed class DemoStatusInstance
    {
        public string StatusId { get; set; }
        public string SourceCombatantId { get; set; }
        public string TargetCombatantId { get; set; }
        public int Stacks { get; set; }
        public float NextTriggerSeconds { get; set; }
        public float RemainingDurationSeconds { get; set; }
        public int RemainingTriggers { get; set; }
        public bool CanDispel { get; set; }
        public bool CanTransfer { get; set; }
        public bool CanConsume { get; set; }
        public bool PersistsBetweenBattles { get; set; }

        public DemoStatusInstance Clone()
        {
            return new DemoStatusInstance
            {
                StatusId = StatusId,
                SourceCombatantId = SourceCombatantId,
                TargetCombatantId = TargetCombatantId,
                Stacks = Stacks,
                NextTriggerSeconds = NextTriggerSeconds,
                RemainingDurationSeconds = RemainingDurationSeconds,
                RemainingTriggers = RemainingTriggers,
                CanDispel = CanDispel,
                CanTransfer = CanTransfer,
                CanConsume = CanConsume,
                PersistsBetweenBattles = PersistsBetweenBattles
            };
        }
    }

    public enum DemoDamageType
    {
        Physical,
        Sword,
        Spell,
        Lightning,
        True
    }

    [Serializable]
    public sealed class DemoDamageRequest
    {
        public string SourceCombatantId { get; private set; }
        public string TargetCombatantId { get; private set; }
        public string EffectId { get; private set; }
        public int Amount { get; private set; }
        public DemoDamageType DamageType { get; private set; }
        public bool IsAreaEffect { get; private set; }
        public int ChainIndex { get; private set; }

        public DemoDamageRequest(
            string sourceCombatantId,
            string targetCombatantId,
            int amount,
            DemoDamageType damageType,
            string effectId = "",
            bool isAreaEffect = false,
            int chainIndex = 0)
        {
            SourceCombatantId = sourceCombatantId ?? string.Empty;
            TargetCombatantId = targetCombatantId ?? string.Empty;
            EffectId = effectId ?? string.Empty;
            Amount = Math.Max(0, amount);
            DamageType = damageType;
            IsAreaEffect = isAreaEffect;
            ChainIndex = Math.Max(0, chainIndex);
        }
    }

    [Serializable]
    public sealed class DemoDamageResult
    {
        public DemoDamageRequest Request { get; private set; }
        public string TargetCombatantId { get; private set; }
        public int RequestedAmount { get; private set; }
        public int BlockedAmount { get; private set; }
        public int HealthDamage { get; private set; }
        public bool WasKilled { get; private set; }

        public int AppliedAmount { get { return BlockedAmount + HealthDamage; } }

        private DemoDamageResult(DemoDamageRequest request, int blockedAmount, int healthDamage, bool wasKilled)
        {
            Request = request;
            TargetCombatantId = request.TargetCombatantId;
            RequestedAmount = request.Amount;
            BlockedAmount = Math.Max(0, blockedAmount);
            HealthDamage = Math.Max(0, healthDamage);
            WasKilled = wasKilled;
        }

        public static DemoDamageResult Apply(DemoDamageRequest request, DemoCombatTarget target)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (!string.Equals(request.TargetCombatantId, target.CombatantId, StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Damage request target " + request.TargetCombatantId
                    + " does not match combatant " + target.CombatantId + ".");
            }

            int beforeBlock = target.Combatant == null ? 0 : target.Combatant.Block;
            int beforeHealth = target.Health;
            int healthDamage = target.ApplyDamage(request.Amount);
            int blocked = target.Combatant == null
                ? 0
                : Math.Min(request.Amount, Math.Max(0, beforeBlock - target.Combatant.Block));
            bool killed = beforeHealth > 0 && target.IsDead;
            return new DemoDamageResult(request, blocked, healthDamage, killed);
        }
    }

    [Serializable]
    public sealed class DemoChainContext
    {
        private readonly HashSet<string> visitedCombatantIds = new HashSet<string>(StringComparer.Ordinal);

        public IReadOnlyCollection<string> VisitedCombatantIds
        {
            get { return visitedCombatantIds.OrderBy(id => id, StringComparer.Ordinal).ToArray(); }
        }

        public bool HasVisited(string combatantId)
        {
            return !string.IsNullOrEmpty(combatantId) && visitedCombatantIds.Contains(combatantId);
        }

        public bool TryVisit(string combatantId)
        {
            if (string.IsNullOrEmpty(combatantId))
            {
                return false;
            }
            return visitedCombatantIds.Add(combatantId);
        }

        public DemoChainContext Clone()
        {
            DemoChainContext clone = new DemoChainContext();
            foreach (string id in visitedCombatantIds)
            {
                clone.visitedCombatantIds.Add(id);
            }
            return clone;
        }
    }

    [Serializable]
    public sealed class DemoTargetQuery
    {
        public bool IncludeDead { get; set; }
        public bool IncludeInactive { get; set; }
        public bool LockableOnly { get; set; }
        public bool RequiredForVictoryOnly { get; set; }
        public int? Depth { get; set; }

        public static DemoTargetQuery ActiveLockable()
        {
            return new DemoTargetQuery { LockableOnly = true };
        }
    }

    public interface IDemoTargetResolver
    {
        IReadOnlyList<DemoCombatTarget> QueryTargets();
        IReadOnlyList<DemoCombatTarget> QueryTargets(DemoTargetQuery query);
        DemoCombatTarget ResolveAutoTarget();
        bool LockTarget(string combatantId);
        void ClearLock();
        DemoCombatTarget LockedTarget { get; }
    }

    /// <summary>
    /// Deterministic target resolver. It owns targeting policy only; damage and
    /// enemy behavior remain in their respective systems.
    /// </summary>
    public sealed class DemoTargetResolver : IDemoTargetResolver
    {
        private readonly List<DemoCombatTarget> targets = new List<DemoCombatTarget>();
        private DemoCombatTarget lockedTarget;
        private bool hasManualLock;

        public DemoCombatTarget LockedTarget
        {
            get
            {
                return ResolveAutoTarget();
            }
        }

        public DemoTargetResolver(IEnumerable<DemoCombatTarget> targets)
        {
            if (targets != null)
            {
                foreach (DemoCombatTarget target in targets)
                {
                    AddTarget(target);
                }
            }
        }

        public void AddTarget(DemoCombatTarget target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (targets.Any(existing => string.Equals(existing.CombatantId, target.CombatantId, StringComparison.Ordinal)))
            {
                throw new InvalidOperationException("Duplicate CombatantId: " + target.CombatantId);
            }
            targets.Add(target);
        }

        public IReadOnlyList<DemoCombatTarget> QueryTargets()
        {
            return QueryTargets(new DemoTargetQuery());
        }

        public IReadOnlyList<DemoCombatTarget> QueryTargets(DemoTargetQuery query)
        {
            DemoTargetQuery effectiveQuery = query ?? new DemoTargetQuery();
            IEnumerable<DemoCombatTarget> result = targets;
            if (!effectiveQuery.IncludeDead)
            {
                result = result.Where(target => !target.IsDead);
            }
            if (!effectiveQuery.IncludeInactive)
            {
                result = result.Where(target => target.IsActive);
            }
            if (effectiveQuery.LockableOnly)
            {
                result = result.Where(target => target.CanLock);
            }
            if (effectiveQuery.RequiredForVictoryOnly)
            {
                result = result.Where(target => target.RequiredForVictory);
            }
            if (effectiveQuery.Depth.HasValue)
            {
                result = result.Where(target => target.Depth == effectiveQuery.Depth.Value);
            }
            return SortByPosition(result).ToArray();
        }

        public IReadOnlyList<DemoCombatTarget> QueryTargets(Func<DemoCombatTarget, bool> predicate)
        {
            IEnumerable<DemoCombatTarget> result = targets;
            if (predicate != null)
            {
                result = result.Where(predicate);
            }
            return SortByPosition(result).ToArray();
        }

        public bool LockTarget(string combatantId)
        {
            DemoCombatTarget target = targets.FirstOrDefault(candidate =>
                string.Equals(candidate.CombatantId, combatantId, StringComparison.Ordinal)
                && IsEligible(candidate, true));
            if (target == null)
            {
                ClearLock();
                ResolveAutoTarget();
                return false;
            }
            lockedTarget = target;
            hasManualLock = true;
            return true;
        }

        public DemoCombatTarget ResolveAutoTarget()
        {
            if (hasManualLock && IsEligible(lockedTarget, true))
            {
                return lockedTarget;
            }

            hasManualLock = false;
            DemoCombatTarget next = SortForAutoTarget(targets.Where(target => IsEligible(target, true))).FirstOrDefault();
            lockedTarget = next;
            return next;
        }

        public void ClearLock()
        {
            lockedTarget = null;
            hasManualLock = false;
        }

        public IReadOnlyList<DemoDamageRequest> CreateAreaDamageRequests(
            string sourceCombatantId,
            int amount,
            DemoDamageType damageType,
            string effectId = "")
        {
            return QueryTargets(DemoTargetQuery.ActiveLockable()).Select(target => new DemoDamageRequest(
                sourceCombatantId,
                target.CombatantId,
                amount,
                damageType,
                effectId,
                true)).ToArray();
        }

        private static bool IsEligible(DemoCombatTarget target, bool requireLockable)
        {
            return target != null
                && target.IsActive
                && !target.IsDead
                && (!requireLockable || target.CanLock);
        }

        private static IEnumerable<DemoCombatTarget> SortByPosition(IEnumerable<DemoCombatTarget> source)
        {
            return source
                .OrderBy(target => target.Depth)
                .ThenBy(target => target.PositionId, StringComparer.Ordinal)
                .ThenBy(target => target.CombatantId, StringComparer.Ordinal);
        }

        private static IEnumerable<DemoCombatTarget> SortForAutoTarget(IEnumerable<DemoCombatTarget> source)
        {
            return source
                .OrderByDescending(target => target.Intent != null && target.Intent.IsPending)
                .ThenBy(target => target.Intent == null ? float.PositiveInfinity : target.Intent.RemainingSeconds)
                .ThenByDescending(target => target.ThreatPriority)
                .ThenByDescending(target => target.Intent == null ? 0 : target.Intent.ThreatPriority)
                .ThenBy(target => target.Depth)
                .ThenBy(target => target.PositionId, StringComparer.Ordinal)
                .ThenBy(target => target.CombatantId, StringComparer.Ordinal);
        }
    }
}
