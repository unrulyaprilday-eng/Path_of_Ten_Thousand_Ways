using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Combatants;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Battle
{
    public enum DemoBattlePhase
    {
        Intro,
        Running,
        Won,
        Lost
    }

    public enum DemoBossPhase
    {
        None,
        ThunderCloud,
        SoulLock,
        CalamityDescends
    }

    public sealed class DemoBattleSetup
    {
        public IReadOnlyList<DemoBattleEnemySetup> Enemies;
        public IReadOnlyList<DemoCard> Deck;
        public IReadOnlyList<DemoArtifactType> Artifacts;
        public IReadOnlyList<DemoGongfaType> Gongfas;
        public IReadOnlyCollection<string> Relics;
        public string PlayerName = "剑修";
        public string EnemyId;
        public int PlayerMaxHealth;
        public int PlayerHealth = 72;
        public string EnemyName = "拦路妖物";
        public int EnemyHealth = 46;
        public bool IsBoss;
        public bool IsOpeningBattle;
        public int BonusEnergyCapacity;
        public int BonusPermanentSwords;
        public int MaxEnergy = 10;
        public int InitialEnergy = -1;
        public float EnergyRegenerationPerSecond = 1f;
        public int InitialHandSize = -1;
        public int HandLimit = 7;
        public float DrawIntervalSeconds = 4f;
        public float FlyingSwordIntervalSeconds = 2.2f;
        public int TemporarySwordVolleyCount = 3;
        public float EnemyIntentMinSeconds = 5f;
        public float EnemyIntentMaxSeconds = 7f;
        public float IntroSeconds = 0.45f;
        public int RandomSeed = -1;
    }

    public sealed class DemoBattleEnemySetup
    {
        public string CombatantId;
        public string DefinitionId;
        public string Name = "拦路妖物";
        public string PositionId = "enemy_slot_primary";
        public int Depth;
        public int MaxHealth = 46;
        public int ThreatPriority;
        public bool CanLock = true;
        public bool RequiredForVictory = true;
    }

    public sealed class DemoBattleState
    {
        private const float TimerEpsilon = 0.0001f;
        public const string BossPhaseNone = "none";
        public const string BossPhaseXuantieArmor = "xuantie_armor";
        public const string BossPhaseXuantieContractSpike = "xuantie_contract_spike";
        public const string BossPhaseXuantieCore = "xuantie_core";
        public const string BossPhaseLegacyThunderCloud = "legacy_thunder_cloud";
        public const string BossPhaseLegacySoulLock = "legacy_soul_lock";
        public const string BossPhaseLegacyCalamity = "legacy_calamity";

        private sealed class TemporarySwordBatch
        {
            public int Count;
            public int RemainingVolleys;
        }

        private Random random = new Random();
        private readonly Queue<DemoBattlePresentationStep> presentationQueue = new Queue<DemoBattlePresentationStep>();
        private readonly List<DemoBattlePresentationStep> operationPresentationSteps = new List<DemoBattlePresentationStep>();
        private readonly List<TemporarySwordBatch> temporarySwordBatches = new List<TemporarySwordBatch>();
        private readonly List<DemoArtifactType> activeArtifacts = new List<DemoArtifactType>();
        private readonly List<DemoGongfaType> activeGongfas = new List<DemoGongfaType>();
        private readonly HashSet<string> activeRelics = new HashSet<string>();
        private readonly List<DemoCombatTarget> enemies = new List<DemoCombatTarget>();
        private readonly List<string> lastResolvedTargetIds = new List<string>();
        private readonly List<DemoBattlePresentationStep> pendingAfterAttackPresentations = new List<DemoBattlePresentationStep>();

        private DemoBattleSetup setup;
        private DemoTargetResolver targetResolver;
        private bool isBossBattle;
        private bool isSwordPuppetBoss;
        private string bossPhaseId = BossPhaseNone;
        private bool calamityCharged;
        private bool mirrorAvailable;
        private bool heartMirrorUsed;
        private bool spiritTalismanAvailable;
        private bool openingBattlePacing;
        private bool sheatheEdgeActive;
        private bool thunderSealActive;
        private bool firstFlyingSwordPlayed;
        private int cardsPlayedSinceVolley;
        private int storedSwordMomentum;
        private int deferredThunderBonusDamage;
        private int lastHeavenOpeningMomentum;
        private int lastHeavenOpeningIntent;
        private float energyValue;
        private float introRemaining;
        private float drawRemaining;
        private float volleyRemaining;
        private float enemyIntentRemaining;
        private float enemyIntentDuration;
        private float energyRegenerationSuppressedRemaining;
        private long nextPresentationSequence;

        public DemoCombatant Player { get; private set; }
        public IReadOnlyList<DemoCombatTarget> Enemies => enemies;
        public int ActiveEnemyCount => enemies.Count(target => target != null && target.IsActive && !target.IsDead);
        public DemoCombatant Enemy
        {
            get
            {
                DemoCombatant target = ResolveAutoTarget();
                if (target != null)
                {
                    return target;
                }

                // The legacy result path reads Enemy after the final target dies.
                return enemies.Count > 0 ? enemies[0].Combatant : null;
            }
        }
        public string LockedTargetId
        {
            get
            {
                DemoCombatTarget target = targetResolver == null ? null : targetResolver.LockedTarget;
                return target == null ? string.Empty : target.CombatantId;
            }
        }
        public DemoBattlePhase Phase { get; private set; } = DemoBattlePhase.Intro;
        public string BossPhaseId => bossPhaseId;
        public DemoBossPhase BossPhase => GetLegacyBossPhase(bossPhaseId);
        public DemoBattleLog Log { get; } = new DemoBattleLog();

        public List<DemoCard> Deck { get; } = new List<DemoCard>();
        public List<DemoCard> DrawPile { get; } = new List<DemoCard>();
        public List<DemoCard> Hand { get; } = new List<DemoCard>();
        public List<DemoCard> DiscardPile { get; } = new List<DemoCard>();

        public int Round { get; private set; }
        public int EnemyActionCount { get; private set; }
        public int Energy => Math.Min(MaxEnergy, Math.Max(0, (int)Math.Floor(energyValue + TimerEpsilon)));
        public float EnergyExact => Math.Min(MaxEnergy, Math.Max(0f, energyValue));
        public int MaxEnergy { get; private set; } = 10;
        public float EnergyRegenerationPerSecond { get; private set; } = 1f;
        public int HandLimit { get; private set; } = 7;
        public int PermanentSwords { get; private set; } = 1;
        public int TemporarySwords { get; private set; }
        public int TotalSwords => PermanentSwords + TemporarySwords;
        public float PhaseTimer { get; private set; }
        public float DrawTimer => drawRemaining;
        public float FlyingSwordTimer => volleyRemaining;
        public float FlyingSwordInterval => setup == null
            ? 0f
            : Math.Max(TimerEpsilon, setup.FlyingSwordIntervalSeconds);
        public float EnemyIntentRemaining => GetSelectedIntent()?.RemainingSeconds ?? 0f;
        public float EnemyIntentDuration => GetSelectedIntent()?.DurationSeconds ?? 0f;
        public float EnemyIntentProgress => EnemyIntentDuration <= TimerEpsilon
            ? 0f
            : Math.Max(0f, Math.Min(1f, 1f - EnemyIntentRemaining / EnemyIntentDuration));
        public float ElapsedSeconds { get; private set; }
        public int VolleysFired { get; private set; }
        public int CardsPlayed { get; private set; }
        public int ShuffleCount { get; private set; }
        public int MaxSwordCount { get; private set; }
        public int MaxSwordsReached => MaxSwordCount;
        public int HighestSingleDamage { get; private set; }
        public int HighestSingleHit => HighestSingleDamage;
        public int HighestBurstDamage => HighestSingleDamage;
        public int ExecutionSequenceVersion { get; private set; }
        public string EnemyIntentText { get; private set; } = string.Empty;
        public string BossIntentText { get; private set; } = string.Empty;
        public string EnemyId { get; private set; } = string.Empty;
        public bool IsBossBattle => isBossBattle;
        public bool IsOpeningBattlePacing => openingBattlePacing;
        public IReadOnlyList<DemoArtifactType> ActiveArtifacts => activeArtifacts;
        public IReadOnlyList<DemoGongfaType> ActiveGongfas => activeGongfas;
        public int PendingPresentationStepCount => presentationQueue.Count;

        private int BasePlayerMaxHealth => DemoConfigRepository.GetIntConstant("battle", "player_base_max_health", 72);
        private int BasePermanentSwords => DemoConfigRepository.GetIntConstant("battle", "player_base_permanent_swords", 1);
        private int BossExtraPermanentSwords => DemoConfigRepository.GetIntConstant("battle", "boss_extra_permanent_swords", 1);

        public void ClearBattle(bool clearLog = false)
        {
            Player = null;
            enemies.Clear();
            targetResolver = null;
            setup = null;
            Phase = DemoBattlePhase.Intro;
            bossPhaseId = BossPhaseNone;
            PhaseTimer = 0f;
            EnemyIntentText = string.Empty;
            BossIntentText = string.Empty;
            EnemyId = string.Empty;
            isBossBattle = false;
            isSwordPuppetBoss = false;
            calamityCharged = false;
            mirrorAvailable = false;
            heartMirrorUsed = false;
            spiritTalismanAvailable = false;
            openingBattlePacing = false;
            sheatheEdgeActive = false;
            thunderSealActive = false;
            firstFlyingSwordPlayed = false;
            cardsPlayedSinceVolley = 0;
            storedSwordMomentum = 0;
            deferredThunderBonusDamage = 0;
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;
            energyValue = 0f;
            introRemaining = 0f;
            drawRemaining = 0f;
            volleyRemaining = 0f;
            enemyIntentRemaining = 0f;
            enemyIntentDuration = 0f;
            energyRegenerationSuppressedRemaining = 0f;
            ElapsedSeconds = 0f;
            Round = 0;
            EnemyActionCount = 0;
            VolleysFired = 0;
            CardsPlayed = 0;
            ShuffleCount = 0;
            MaxSwordCount = 0;
            HighestSingleDamage = 0;
            MaxEnergy = 10;
            EnergyRegenerationPerSecond = 1f;
            HandLimit = 7;
            PermanentSwords = BasePermanentSwords;
            TemporarySwords = 0;
            nextPresentationSequence = 0;
            ExecutionSequenceVersion++;

            Deck.Clear();
            DrawPile.Clear();
            Hand.Clear();
            DiscardPile.Clear();
            temporarySwordBatches.Clear();
            activeArtifacts.Clear();
            activeGongfas.Clear();
            activeRelics.Clear();
            presentationQueue.Clear();
            operationPresentationSteps.Clear();
            lastResolvedTargetIds.Clear();
            pendingAfterAttackPresentations.Clear();

            if (clearLog)
            {
                Log.Clear();
            }
        }

        public void StartBattle(DemoBattleSetup battleSetup)
        {
            if (battleSetup == null)
            {
                throw new ArgumentNullException(nameof(battleSetup));
            }

            ClearBattle(true);
            BeginPresentationOperation();
            setup = battleSetup;
            EnemyId = battleSetup.EnemyId ?? string.Empty;
            random = battleSetup.RandomSeed >= 0 ? new Random(battleSetup.RandomSeed) : new Random();

            int playerMaxHealth = battleSetup.PlayerMaxHealth > 0 ? battleSetup.PlayerMaxHealth : BasePlayerMaxHealth;
            Player = new DemoCombatant(
                string.IsNullOrWhiteSpace(battleSetup.PlayerName) ? "剑修" : battleSetup.PlayerName,
                playerMaxHealth);
            Player.Health = Math.Min(Player.MaxHealth, Math.Max(1, battleSetup.PlayerHealth));

            AddConfiguredEnemies(battleSetup);
            isSwordPuppetBoss = battleSetup.IsBoss && IsSwordPuppetTargetSet(enemies);
            if (isSwordPuppetBoss)
            {
                InitializeSwordPuppetTargets();
            }
            targetResolver = new DemoTargetResolver(enemies);
            EnemyId = enemies[0].CombatantId;

            if (battleSetup.Artifacts != null)
            {
                activeArtifacts.AddRange(battleSetup.Artifacts);
            }

            if (battleSetup.Gongfas != null)
            {
                activeGongfas.AddRange(battleSetup.Gongfas.Where(type => type != DemoGongfaType.None));
            }

            if (battleSetup.Relics != null)
            {
                foreach (string relic in battleSetup.Relics)
                {
                    if (!string.IsNullOrWhiteSpace(relic))
                    {
                        activeRelics.Add(relic);
                    }
                }
            }

            if (battleSetup.Deck != null)
            {
                foreach (DemoCard card in battleSetup.Deck)
                {
                    if (card == null)
                    {
                        continue;
                    }

                    Deck.Add(card.Clone());
                    DrawPile.Add(card.Clone());
                }
            }

            Shuffle(DrawPile);
            MaxEnergy = Math.Max(1, battleSetup.MaxEnergy) + Math.Max(0, battleSetup.BonusEnergyCapacity);
            EnergyRegenerationPerSecond = Math.Max(0f, battleSetup.EnergyRegenerationPerSecond);
            HandLimit = Math.Max(1, battleSetup.HandLimit);
            openingBattlePacing = battleSetup.IsOpeningBattle;
            int initialEnergy = battleSetup.InitialEnergy >= 0
                ? battleSetup.InitialEnergy
                : openingBattlePacing ? 1 : 2;
            energyValue = Math.Min(MaxEnergy, Math.Max(0, initialEnergy));

            PermanentSwords = BasePermanentSwords
                + Math.Max(0, battleSetup.BonusPermanentSwords)
                + (battleSetup.IsBoss ? BossExtraPermanentSwords : 0);
            if (HasArtifact(DemoArtifactType.SwordBox))
            {
                PermanentSwords += 1;
            }

            isBossBattle = battleSetup.IsBoss;
            bossPhaseId = isSwordPuppetBoss
                ? BossPhaseXuantieArmor
                : isBossBattle ? BossPhaseLegacyThunderCloud : BossPhaseNone;
            mirrorAvailable = HasArtifact(DemoArtifactType.HaotianMirror);
            spiritTalismanAvailable = HasRelic("聚灵符");
            firstFlyingSwordPlayed = false;
            MaxSwordCount = TotalSwords;

            if (HasGongfa(DemoGongfaType.SwordHeartResonance))
            {
                Player.SwordIntent += 1;
                Log.Add("剑心通明随牌库展开，获得 1 点剑意。");
            }

            int initialHandSize = battleSetup.InitialHandSize >= 0
                ? battleSetup.InitialHandSize
                : openingBattlePacing ? 3 : 4;
            DrawCards(Math.Min(HandLimit, Math.Max(0, initialHandSize)));
            if (openingBattlePacing && Deck.Count > 2)
            {
                EnsureOpeningPathCardInHand();
            }

            drawRemaining = Math.Max(TimerEpsilon, battleSetup.DrawIntervalSeconds);
            volleyRemaining = Math.Max(TimerEpsilon, battleSetup.FlyingSwordIntervalSeconds);
            ConfigureAllEnemyIntents();

            introRemaining = Math.Max(0f, battleSetup.IntroSeconds);
            Phase = introRemaining > TimerEpsilon ? DemoBattlePhase.Intro : DemoBattlePhase.Running;
            PhaseTimer = Phase == DemoBattlePhase.Intro ? introRemaining : EnemyIntentRemaining;
            DemoCombatTarget openingTarget = ResolveCurrentTargetNode();
            string openingName = openingTarget?.Combatant?.Name ?? battleSetup.EnemyName;
            Log.Add(isSwordPuppetBoss
                ? "玄铁镇矿剑傀自封契中苏醒，甲片封住剑炉核心。"
                : isBossBattle ? "守关强敌现身，斗法气机骤紧。" : $"{openingName} 拦路，斗法持续演算。");
            Emit(DemoBattlePresentationStep.BattleStart(
                openingName,
                isBossBattle,
                openingTarget?.CombatantId ?? EnemyId));
            PublishPresentationOperation();
        }

        public bool LockTarget(string combatantId)
        {
            bool locked = targetResolver != null && targetResolver.LockTarget(combatantId);
            SyncLegacyIntentView();
            return locked;
        }

        public DemoCombatant ResolveAutoTarget()
        {
            DemoCombatTarget target = targetResolver == null ? null : targetResolver.ResolveAutoTarget();
            return target == null ? null : target.Combatant;
        }

        public DemoCombatTarget ResolveCurrentTargetNode()
        {
            return targetResolver == null ? null : targetResolver.ResolveAutoTarget();
        }

        public DemoCombatTarget FindTarget(string combatantId)
        {
            return enemies.FirstOrDefault(target => target != null
                && string.Equals(target.CombatantId, combatantId, StringComparison.Ordinal));
        }

        private void AddConfiguredEnemies(DemoBattleSetup battleSetup)
        {
            if (battleSetup.Enemies != null && battleSetup.Enemies.Count > 0)
            {
                int count = Math.Min(3, battleSetup.Enemies.Count);
                for (int i = 0; i < count; i++)
                {
                    DemoBattleEnemySetup enemySetup = battleSetup.Enemies[i];
                    if (enemySetup == null)
                    {
                        continue;
                    }

                    string combatantId = string.IsNullOrWhiteSpace(enemySetup.CombatantId)
                        ? "encounter_enemy_" + (i + 1)
                        : enemySetup.CombatantId;
                    string definitionId = string.IsNullOrWhiteSpace(enemySetup.DefinitionId)
                        ? combatantId
                        : enemySetup.DefinitionId;
                    string positionId = string.IsNullOrWhiteSpace(enemySetup.PositionId)
                        ? "enemy_slot_" + (i + 1)
                        : enemySetup.PositionId;
                    string name = string.IsNullOrWhiteSpace(enemySetup.Name)
                        ? "拦路妖物"
                        : enemySetup.Name;
                    enemies.Add(new DemoCombatTarget(
                        combatantId,
                        definitionId,
                        positionId,
                        Math.Max(0, enemySetup.Depth),
                        enemySetup.CanLock,
                        enemySetup.RequiredForVictory,
                        enemySetup.ThreatPriority,
                        new DemoCombatant(name, Math.Max(1, enemySetup.MaxHealth))));
                }
            }

            if (enemies.Count == 0)
            {
                string enemyName = string.IsNullOrWhiteSpace(battleSetup.EnemyName) ? "拦路妖物" : battleSetup.EnemyName;
                string stableEnemyId = string.IsNullOrWhiteSpace(battleSetup.EnemyId)
                    ? "encounter_enemy_primary"
                    : battleSetup.EnemyId;
                enemies.Add(new DemoCombatTarget(
                    stableEnemyId,
                    stableEnemyId,
                    "enemy_slot_primary",
                    0,
                    true,
                    true,
                    0,
                    new DemoCombatant(enemyName, Math.Max(1, battleSetup.EnemyHealth))));
            }

            if (!enemies.Any(target => target.RequiredForVictory))
            {
                enemies[0].RequiredForVictory = true;
            }
        }

        public void StartBattle(
            IReadOnlyList<DemoCard> deck,
            IReadOnlyList<DemoArtifactType> artifacts,
            IReadOnlyList<DemoGongfaType> gongfas,
            string enemyName,
            int enemyHealth,
            bool boss,
            int playerHealth = 72,
            int bonusEnergy = 0,
            int bonusSwords = 0,
            IReadOnlyCollection<string> relics = null,
            bool openingBattle = false)
        {
            StartBattle(new DemoBattleSetup
            {
                Deck = deck,
                Artifacts = artifacts,
                Gongfas = gongfas,
                Relics = relics,
                EnemyName = enemyName,
                EnemyHealth = enemyHealth,
                IsBoss = boss,
                PlayerHealth = playerHealth,
                BonusEnergyCapacity = bonusEnergy,
                BonusPermanentSwords = bonusSwords,
                IsOpeningBattle = openingBattle
            });
        }

        public void Tick(float deltaTime)
        {
            if (Player == null || Enemy == null || deltaTime <= 0f)
            {
                return;
            }

            if (Phase != DemoBattlePhase.Intro && Phase != DemoBattlePhase.Running)
            {
                return;
            }

            BeginPresentationOperation();
            float remaining = deltaTime;

            if (Phase == DemoBattlePhase.Intro)
            {
                float introStep = Math.Min(remaining, introRemaining);
                introRemaining = Math.Max(0f, introRemaining - introStep);
                remaining -= introStep;
                PhaseTimer = introRemaining;

                if (introRemaining <= TimerEpsilon)
                {
                    Phase = DemoBattlePhase.Running;
                    PhaseTimer = EnemyIntentRemaining;
                    Log.Add("双方气机相接，半实时斗法开始。");
                }
            }

            if (Phase == DemoBattlePhase.Running && remaining > TimerEpsilon)
            {
                AdvanceRunningBattle(remaining);
            }

            PublishPresentationOperation();
        }

        public bool TryPlayCard(int handIndex)
        {
            if (Phase != DemoBattlePhase.Running || handIndex < 0 || handIndex >= Hand.Count)
            {
                return false;
            }

            BeginPresentationOperation();
            DemoCard card = Hand[handIndex];
            if (card == null)
            {
                Hand.RemoveAt(handIndex);
                Log.Add("一张残缺卡牌未能成形，已从手牌中移除。");
                PublishPresentationOperation();
                return false;
            }

            bool useSpiritTalisman = spiritTalismanAvailable && card.Cost == 1;
            int actualCost = useSpiritTalisman ? 0 : Math.Max(0, card.Cost);
            if (actualCost > energyValue + TimerEpsilon)
            {
                Log.Add($"{card.Name} 灵气不足。");
                PublishPresentationOperation();
                return false;
            }

            if (useSpiritTalisman)
            {
                spiritTalismanAvailable = false;
                Log.Add($"聚灵符映亮经脉，{card.Name} 本次免费。");
            }

            energyValue = Math.Max(0f, energyValue - actualCost);
            Hand.RemoveAt(handIndex);
            cardsPlayedSinceVolley++;
            CardsPlayed++;
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;

            int damage = ResolveCard(card);
            TrackDamage(damage);
            string primaryTargetId = lastResolvedTargetIds.FirstOrDefault() ?? LockedTargetId;
            Emit(DemoBattlePresentationStep.Card(
                card,
                damage,
                "player",
                primaryTargetId,
                lastResolvedTargetIds.ToArray()));
            FlushAfterAttackPresentations();

            if (ResolveCurrentTargetNode() != null && !Player.IsDead && mirrorAvailable && IsMirrorTarget(card))
            {
                mirrorAvailable = false;
                Log.Add($"昊天镜映照 {card.Name}，再次结算。");
                int mirroredDamage = ResolveCard(card, true);
                TrackDamage(mirroredDamage);
                primaryTargetId = lastResolvedTargetIds.FirstOrDefault() ?? LockedTargetId;
                DemoBattlePresentationStep mirrorStep = DemoBattlePresentationStep.Card(
                    card,
                    mirroredDamage,
                    "player",
                    primaryTargetId,
                    lastResolvedTargetIds.ToArray());
                mirrorStep.Label = $"昊天镜·{card.Name}";
                mirrorStep.HeavyImpact = true;
                Emit(mirrorStep);
                FlushAfterAttackPresentations();
            }

            DiscardPile.Add(card);
            TryAddBossPhaseShift();
            CheckForBattleResult();
            PublishPresentationOperation();
            return true;
        }


        public bool TryConsumePresentationStep(out DemoBattlePresentationStep step)
        {
            if (presentationQueue.Count == 0)
            {
                step = null;
                return false;
            }

            step = presentationQueue.Dequeue();
            return true;
        }

        public List<DemoBattlePresentationStep> ConsumePresentationSteps()
        {
            List<DemoBattlePresentationStep> result = new List<DemoBattlePresentationStep>(presentationQueue.Count);
            while (presentationQueue.Count > 0)
            {
                result.Add(presentationQueue.Dequeue());
            }

            return result;
        }

        public void ClearPresentationSteps()
        {
            presentationQueue.Clear();
            operationPresentationSteps.Clear();
        }

        private void AdvanceRunningBattle(float deltaTime)
        {
            float remaining = deltaTime;
            int safety = 0;

            while (remaining > TimerEpsilon && Phase == DemoBattlePhase.Running && safety++ < 256)
            {
                float step = remaining;
                step = Math.Min(step, Math.Max(0f, volleyRemaining));
                step = Math.Min(step, Math.Max(0f, GetNextEnemyIntentRemaining()));
                if (Hand.Count < HandLimit)
                {
                    step = Math.Min(step, Math.Max(0f, drawRemaining));
                }

                if (step > TimerEpsilon)
                {
                    AdvanceContinuousClocks(step);
                    remaining -= step;
                }

                bool resolvedEvent = false;
                if (Phase == DemoBattlePhase.Running && Hand.Count < HandLimit && drawRemaining <= TimerEpsilon)
                {
                    ResolveTimedDraw();
                    resolvedEvent = true;
                }

                if (Phase == DemoBattlePhase.Running && volleyRemaining <= TimerEpsilon)
                {
                    volleyRemaining = Math.Max(TimerEpsilon, setup.FlyingSwordIntervalSeconds);
                    ResolveFlyingSwordCycle();
                    resolvedEvent = true;
                }

                if (Phase == DemoBattlePhase.Running && HasReadyEnemyIntent())
                {
                    ResolveReadyEnemyIntents();
                    resolvedEvent = true;
                }

                if (!resolvedEvent && step <= TimerEpsilon)
                {
                    float escapeStep = Math.Min(remaining, TimerEpsilon);
                    if (escapeStep <= 0f)
                    {
                        break;
                    }

                    AdvanceContinuousClocks(escapeStep);
                    remaining -= escapeStep;
                }
            }
        }

        private void AdvanceContinuousClocks(float deltaTime)
        {
            ElapsedSeconds += deltaTime;
            volleyRemaining = Math.Max(0f, volleyRemaining - deltaTime);
            for (int i = 0; i < enemies.Count; i++)
            {
                DemoCombatTarget target = enemies[i];
                if (target == null || !target.IsActive || target.IsDead || target.Intent == null || !target.Intent.IsPending)
                {
                    continue;
                }

                target.Intent.RemainingSeconds = Math.Max(0f, target.Intent.RemainingSeconds - deltaTime);
            }
            if (Hand.Count < HandLimit)
            {
                drawRemaining = Math.Max(0f, drawRemaining - deltaTime);
            }

            float regenerativeTime = deltaTime;
            if (energyRegenerationSuppressedRemaining > 0f)
            {
                float suppressed = Math.Min(energyRegenerationSuppressedRemaining, deltaTime);
                energyRegenerationSuppressedRemaining -= suppressed;
                regenerativeTime -= suppressed;
            }

            if (regenerativeTime > 0f && EnergyRegenerationPerSecond > 0f)
            {
                energyValue = Math.Min(MaxEnergy, energyValue + regenerativeTime * EnergyRegenerationPerSecond);
            }

            SyncLegacyIntentView();
            PhaseTimer = EnemyIntentRemaining;
        }

        private void ResolveTimedDraw()
        {
            drawRemaining = Math.Max(TimerEpsilon, setup.DrawIntervalSeconds);
            int drawn = DrawCards(1);
            if (drawn <= 0)
            {
                return;
            }

            DemoCard card = Hand[Hand.Count - 1];
            Log.Add($"牌库流转，抽到 {card.Name}。");
            Emit(DemoBattlePresentationStep.Draw(card));
        }

        private void ResolveFlyingSwordCycle()
        {
            int swordCount = TotalSwords;
            VolleysFired++;

            if (sheatheEdgeActive)
            {
                int gainedIntent = ResolveSheatheEdge();
                Emit(DemoBattlePresentationStep.SwordStored(GetVolleyStyle(), swordCount, gainedIntent));
                sheatheEdgeActive = false;
            }
            else
            {
                int swordDamage = ResolveFlyingSwords(out bool shockTriggered);
                TrackDamage(swordDamage);
                string targetId = lastResolvedTargetIds.FirstOrDefault() ?? LockedTargetId;
                Emit(DemoBattlePresentationStep.SwordVolley(
                    GetVolleyStyle(),
                    swordCount,
                    swordDamage,
                    shockTriggered,
                    targetId,
                    lastResolvedTargetIds.ToArray()));
                FlushAfterAttackPresentations();
            }

            thunderSealActive = false;
            cardsPlayedSinceVolley = 0;
            AgeTemporarySwords();
            TryAddBossPhaseShift();
            CheckForBattleResult();
        }

        private void ResolveReadyEnemyIntents()
        {
            if (Player.Shock > 0)
            {
                Player.Shock = Math.Max(0, Player.Shock - 1);
            }

            DemoCombatTarget[] ready = enemies
                .Where(target => target != null
                    && target.IsActive
                    && !target.IsDead
                    && target.Intent != null
                    && target.Intent.IsPending
                    && target.Intent.RemainingSeconds <= TimerEpsilon)
                .OrderBy(target => target.Depth)
                .ThenBy(target => target.PositionId, StringComparer.Ordinal)
                .ToArray();

            for (int i = 0; i < ready.Length && Phase == DemoBattlePhase.Running; i++)
            {
                DemoCombatTarget source = ready[i];
                EnemyActionCount++;
                Round = EnemyActionCount;
                DemoBattlePresentationStep step = isSwordPuppetBoss
                    ? ResolveSwordPuppetIntent(source)
                    : isBossBattle ? ResolveLegacyBossIntent(source) : ResolveNormalEnemyIntent(source);
                Emit(step);
                CheckForBattleResult();
                if (Phase == DemoBattlePhase.Running && source.IsActive && !source.IsDead)
                {
                    ConfigureEnemyIntent(source);
                }
            }

            SyncLegacyIntentView();
        }

        private int ResolveCard(DemoCard card, bool mirrored = false)
        {
            DemoCombatTarget primaryTarget = ResolveCurrentTargetNode();
            DemoCombatant enemy = primaryTarget?.Combatant;
            lastResolvedTargetIds.Clear();
            if (enemy == null)
            {
                return 0;
            }

            int damage = card.Damage;
            int block = card.Block;
            int swordIntent = card.SwordIntent;
            int shock = card.Shock;
            int bleed = card.Bleed;
            int temporarySwords = card.TemporarySwords;
            int energyGain = card.EnergyGain;
            int selfDamage = card.SelfDamage;
            int draw = card.Draw;
            string logPrefix = mirrored ? "镜像：" : string.Empty;

            if (mirrored && HasRelic("残破古镜"))
            {
                damage += damage > 0 ? 4 : 0;
                block += block > 0 ? 3 : 0;
                swordIntent += swordIntent > 0 ? 1 : 0;
                shock += shock > 0 ? 2 : 0;
                bleed += bleed > 0 ? 2 : 0;
                temporarySwords += temporarySwords > 0 ? 1 : 0;
                Log.Add("残破古镜折射出更深一层镜影，复制效果被再次强化。");
            }

            switch (card.SpecialEffect)
            {
                case DemoCardSpecialEffect.SheatheEdge:
                    sheatheEdgeActive = true;
                    Log.Add($"{logPrefix}藏锋诀压住剑势，下一次齐射将改为蓄锋。");
                    break;
                case DemoCardSpecialEffect.HeavenOpening:
                    damage += ResolveHeavenOpeningBonus(mirrored, logPrefix);
                    break;
                case DemoCardSpecialEffect.ThunderSeal:
                    thunderSealActive = true;
                    Log.Add($"{logPrefix}封雷匣合拢雷势，下一次齐射将继续封存感电。");
                    break;
            }

            if (card.Type == DemoCardType.FlyingSword && !mirrored)
            {
                if (!firstFlyingSwordPlayed && HasGongfa(DemoGongfaType.SwordControlArt))
                {
                    AddTemporarySwords(1);
                    Log.Add("御剑诀牵引本次牌库循环的第一道剑势，额外生成 1 把临时飞剑。");
                }

                firstFlyingSwordPlayed = true;
            }

            if (card.Style == DemoSwordStyle.Blood && enemy.Bleed > 0)
            {
                damage += enemy.Bleed / 2;
            }

            if (card.ConsumeAllSwordIntent)
            {
                int consumedIntent = Player.SwordIntent;
                damage += consumedIntent * 4;
                Log.Add($"{logPrefix}{card.Name} 消耗 {consumedIntent} 剑意化为万剑。");
                Player.SwordIntent = 0;

                if (HasRelic("剑冢残碑") && consumedIntent >= 6)
                {
                    int relicSwords = Math.Max(1, consumedIntent / 6);
                    temporarySwords += relicSwords;
                    Log.Add($"剑冢残碑震响，额外凝成 {relicSwords} 把临时飞剑。");
                }
            }

            if (damage > 0)
            {
                damage = ApplyBloodOrbBonus(damage, $"{logPrefix}{card.Name}");
                damage = ResolvePlayerDamageAgainstTargets(card, primaryTarget, damage);
                Log.Add($"{logPrefix}{card.Name} 造成 {damage} 点伤害。");
            }

            if (block > 0)
            {
                Player.Block += block;
                Log.Add($"{logPrefix}{card.Name} 获得 {block} 护盾。");
            }

            if (swordIntent > 0)
            {
                Player.SwordIntent += swordIntent;
                Log.Add($"{logPrefix}剑意 +{swordIntent}，当前 {Player.SwordIntent}。");
            }

            if (shock > 0)
            {
                int extraShock = HasArtifact(DemoArtifactType.ThunderSeal) ? 2 : 0;
                IReadOnlyList<DemoCombatTarget> shockTargets = ResolveEffectTargets(primaryTarget, card.Style == DemoSwordStyle.Thunder);
                for (int i = 0; i < shockTargets.Count; i++)
                {
                    shockTargets[i].Combatant.Shock += shock + extraShock;
                }
                Log.Add($"{logPrefix}{shockTargets.Count} 个目标感电 +{shock + extraShock}。");
            }

            if (bleed > 0)
            {
                if (!primaryTarget.IsDead)
                {
                    primaryTarget.Combatant.Bleed += bleed;
                    Log.Add($"{logPrefix}{primaryTarget.Combatant.Name} 流血 +{bleed}。");
                }
            }

            if (temporarySwords > 0)
            {
                int extraTemporarySwords = HasArtifact(DemoArtifactType.SwordBox) ? 1 : 0;
                int addedSwords = temporarySwords + extraTemporarySwords;
                AddTemporarySwords(addedSwords);
                Log.Add($"{logPrefix}临时飞剑 +{addedSwords}，可参与接下来 {Math.Max(1, setup.TemporarySwordVolleyCount)} 次齐射。");
            }

            if (card.PermanentSword)
            {
                PermanentSwords++;
                UpdateMaxSwordCount();
                Log.Add($"{logPrefix}永久飞剑 +1，当前 {PermanentSwords}。");
            }

            if (energyGain > 0)
            {
                energyValue = Math.Min(MaxEnergy, energyValue + energyGain);
                Log.Add($"{logPrefix}灵气 +{energyGain}。");
            }

            if (draw > 0)
            {
                int drawn = DrawCards(draw);
                Log.Add($"{logPrefix}抽牌 +{drawn}。");
            }

            if (selfDamage > 0)
            {
                int lost = LosePlayerLife(selfDamage);
                Log.Add($"{logPrefix}{card.Name} 失去 {lost} 点生命。");
            }

            return damage;
        }

        private int ResolveFlyingSwords(out bool shockTriggered)
        {
            DemoCombatTarget primaryTarget = ResolveCurrentTargetNode();
            DemoCombatant enemy = primaryTarget?.Combatant;
            lastResolvedTargetIds.Clear();
            if (enemy == null)
            {
                shockTriggered = false;
                return 0;
            }

            int swordCount = TotalSwords;
            int swordDamage = swordCount * 3;
            shockTriggered = false;
            int shockBeforeTrigger = enemy.Shock;

            if (Player.SwordIntent >= 3)
            {
                swordDamage += swordCount;
            }

            if (HasGongfa(DemoGongfaType.SwordHeartResonance) && cardsPlayedSinceVolley <= 2)
            {
                swordDamage += 4;
                Log.Add("剑心通明收束出手，本次齐射额外造成 4 点伤害。");
            }

            if (HasGongfa(DemoGongfaType.LightningMeridians))
            {
                enemy.Shock += 2;
                Log.Add("引雷入窍先行为敌身覆雷，感电 +2。");
            }

            if (enemy.Shock > 0)
            {
                if (thunderSealActive)
                {
                    int storedThunder = Math.Max(4, enemy.Shock / 2 + 1);
                    deferredThunderBonusDamage += storedThunder;
                    enemy.Shock += 2;
                    Log.Add($"封雷匣将雷意继续压入敌躯，本次齐射不引爆，并为下次雷击蓄下 {storedThunder} 点天罚。");
                }
                else
                {
                    int shockDamage = Math.Max(1, enemy.Shock / 2);
                    if (HasArtifact(DemoArtifactType.ThunderSeal))
                    {
                        shockDamage += 4;
                    }
                    if (HasRelic("雷心"))
                    {
                        shockDamage += 3;
                    }
                    if (HasGongfa(DemoGongfaType.ThunderScripture))
                    {
                        shockDamage += 4;
                        Log.Add("九霄神雷诀追引雷势，额外降下 4 点雷击。");
                    }
                    if (deferredThunderBonusDamage > 0)
                    {
                        shockDamage += deferredThunderBonusDamage;
                        Log.Add($"封雷匣放出蓄雷，额外追加 {deferredThunderBonusDamage} 点伤害。");
                        deferredThunderBonusDamage = 0;
                    }
                    if (HasRelic("九霄雷印") && shockBeforeTrigger >= 6)
                    {
                        shockDamage += 6;
                        Log.Add("九霄雷印震开覆雷，再降下 6 点雷击。");
                    }
                    swordDamage += shockDamage;
                    enemy.Shock = Math.Max(0, enemy.Shock - 2);
                    shockTriggered = true;
                    Log.Add($"感电被飞剑引爆，追加 {shockDamage} 伤害。");
                }
            }

            if (enemy.Bleed > 0)
            {
                if (HasRelic("血剑胚"))
                {
                    enemy.Bleed += 2;
                    Log.Add("血剑胚浸染剑锋，额外施加 2 层流血。");
                }

                if (HasGongfa(DemoGongfaType.BloodFiendCanon))
                {
                    int bloodBonus = Math.Max(2, enemy.Bleed / 2);
                    swordDamage += bloodBonus;
                    Player.Heal(2);
                    Log.Add($"血煞经借血催锋，齐射追加 {bloodBonus} 伤害并回复 2 点生命。");
                }
            }

            swordDamage = ApplyBloodOrbBonus(swordDamage, "飞剑齐发");
            int dealt = ResolveVolleyDamageAgainstTargets(primaryTarget, swordDamage, GetVolleyStyle());
            int totalDamage = dealt;
            Log.Add($"{swordCount} 把飞剑自动攻击，造成 {dealt} 点伤害。");

            if (!primaryTarget.IsDead && HasRelic("万剑剑匣") && random.NextDouble() < 0.35d)
            {
                int echoDamage = ApplyBloodOrbBonus(Math.Max(6, swordCount * 2), "万剑剑匣");
                int echoed = ApplyDamageToTarget(primaryTarget, echoDamage, DemoDamageType.Sword, "wanjian_sword_box");
                totalDamage += echoed;
                Log.Add($"万剑剑匣牵动归锋，再追斩 {echoed} 点伤害。");
            }

            int bleedDamage = primaryTarget.IsDead ? 0 : enemy.TickBleed();
            if (bleedDamage > 0)
            {
                totalDamage += bleedDamage;
                Log.Add($"流血造成 {bleedDamage} 伤害。");
                if (primaryTarget.IsDead)
                {
                    RegisterTargetDefeated(primaryTarget);
                }

                if (HasGongfa(DemoGongfaType.BloodRefiningBody))
                {
                    Player.Heal(2);
                    Player.SwordIntent += 1;
                    Log.Add("血炼归元炼化煞气，回复 2 点生命并获得 1 点剑意。");
                }
            }

            if (ResolveCurrentTargetNode() != null && HasGongfa(DemoGongfaType.WanjianReturn) && (Player.SwordIntent >= 4 || swordCount >= 5))
            {
                int returnDamage = ApplyBloodOrbBonus(Math.Max(8, swordCount * 2), "万剑归宗");
                int returned = ApplyAreaDamage(returnDamage, DemoDamageType.Sword, "wanjian_return", 0.65f);
                totalDamage += returned;
                Log.Add($"万剑归宗回潮，再次斩出 {returned} 点伤害。");
            }

            DemoCombatTarget followupTarget = ResolveCurrentTargetNode();
            if (followupTarget != null && HasGongfa(DemoGongfaType.HeavenlyThunderEdict) && shockTriggered)
            {
                int thunderDamage = ApplyBloodOrbBonus(10, "九天引雷");
                int dealtThunder = ApplyChainDamage(followupTarget, thunderDamage, "heavenly_thunder_edict", 3);
                totalDamage += dealtThunder;
                Log.Add($"九天引雷落下天罚，追加 {dealtThunder} 点伤害。");
            }

            followupTarget = ResolveCurrentTargetNode();
            if (followupTarget != null && HasGongfa(DemoGongfaType.BloodPrisonExecution) && followupTarget.Combatant.Bleed >= 8)
            {
                int bloodPrisonDamage = ApplyBloodOrbBonus(12 + followupTarget.Combatant.Bleed / 2, "血狱断生");
                int dealtBloodPrison = ApplyDamageToTarget(followupTarget, bloodPrisonDamage, DemoDamageType.Sword, "blood_prison_execution");
                totalDamage += dealtBloodPrison;
                Player.Heal(4);
                Log.Add($"血狱断生收束残血，追加 {dealtBloodPrison} 点伤害并回复 4 点生命。");
            }

            return totalDamage;
        }

        private int ResolveSheatheEdge()
        {
            int swords = TotalSwords;
            if (swords <= 0)
            {
                Log.Add("藏锋诀收势未成，此刻没有可入鞘的飞剑。");
                return 0;
            }

            int gainedIntent = Math.Max(2, (swords + 1) / 2);
            storedSwordMomentum += swords;
            Player.SwordIntent += gainedIntent;
            Log.Add($"藏锋诀收剑入鞘，{swords} 把飞剑化为 {gainedIntent} 点剑意，并积蓄 {swords} 点锋势。");
            return gainedIntent;
        }

        private DemoBattlePresentationStep ResolveNormalEnemyIntent(DemoCombatTarget source)
        {
            int damage = 7 + Math.Min(3, Math.Max(0, EnemyActionCount - 1) / 3);
            string sourceName = source?.Combatant?.Name ?? "矿中妖物";
            int dealt = ApplyIncomingDamage(damage, sourceName, false, out bool gourdTriggered);
            Log.Add($"{sourceName} 读条完成，造成 {dealt} 点伤害。");
            DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy(sourceName, dealt, false, source?.CombatantId);
            if (gourdTriggered)
            {
                step.Label = $"{sourceName} / 葫芦收煞";
            }
            return step;
        }

        private DemoBattlePresentationStep ResolveLegacyBossIntent(DemoCombatTarget source)
        {
            string sourceId = source?.CombatantId ?? EnemyId;
            switch (bossPhaseId)
            {
                case BossPhaseLegacyThunderCloud:
                {
                    int damage = 6 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 5);
                    int dealt = ApplyIncomingDamage(damage, "雷云压境", true, out bool gourdTriggered);
                    Player.Shock += 1;
                    Log.Add($"雷云压境造成 {dealt} 伤害并施加 1 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("雷云压境", dealt, true, sourceId);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 1;
                    if (gourdTriggered)
                    {
                        step.Label = "雷云压境 / 葫芦收煞";
                    }
                    return step;
                }
                case BossPhaseLegacySoulLock:
                {
                    int damage = 8 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 6) + (Player.Shock >= 3 ? 1 : 0);
                    int dealt = ApplyIncomingDamage(damage, "天雷锁魂", true, out bool gourdTriggered);
                    Player.Shock += 2;
                    energyRegenerationSuppressedRemaining = Math.Max(energyRegenerationSuppressedRemaining, 2f);
                    Log.Add($"天雷锁魂造成 {dealt} 伤害、施加 2 感电，并压制灵气恢复 2 秒。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天雷锁魂", dealt, true, sourceId);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 2;
                    if (gourdTriggered)
                    {
                        step.Label = "天雷锁魂 / 葫芦收煞";
                    }
                    return step;
                }
                case BossPhaseLegacyCalamity:
                {
                    if (!calamityCharged)
                    {
                        calamityCharged = true;
                        Player.Shock += 1;
                        Log.Add("天劫化身开始蓄雷，短读条结束后将降下重击。");
                        ConfigureEnemyIntent(source);
                        SyncLegacyIntentView();
                        return DemoBattlePresentationStep.Charge("天劫蓄雷", 1, sourceId);
                    }

                    int damage = 12 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 8) + Math.Min(2, Player.Shock);
                    int dealt = ApplyIncomingDamage(damage, "天劫降临", true, out bool gourdTriggered);
                    Player.Shock += 1;
                    calamityCharged = false;
                    Log.Add($"天劫降临造成 {dealt} 伤害并施加 1 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天劫降临", dealt, true, sourceId);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 1;
                    step.HeavyImpact = true;
                    if (gourdTriggered)
                    {
                        step.Label = "天劫降临 / 葫芦收煞";
                    }
                    return step;
                }
                default:
                    return DemoBattlePresentationStep.Enemy(source?.Combatant?.Name ?? "守关强敌", 0, true, sourceId);
            }
        }

        private DemoBattlePresentationStep ResolveSwordPuppetIntent(DemoCombatTarget source)
        {
            string sourceId = source?.CombatantId ?? EnemyId;
            switch (bossPhaseId)
            {
                case BossPhaseXuantieArmor:
                {
                    int dealt = ApplyIncomingDamage(7, "玄铁镇压", true, out bool gourdTriggered);
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("玄铁镇压", dealt, true, sourceId);
                    if (gourdTriggered)
                    {
                        step.Label = "玄铁镇压 / 葫芦收煞";
                    }
                    return step;
                }
                case BossPhaseXuantieContractSpike:
                {
                    int dealt = ApplyIncomingDamage(9, "残契缚剑", true, out bool gourdTriggered);
                    energyRegenerationSuppressedRemaining = Math.Max(energyRegenerationSuppressedRemaining, 1.5f);
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("残契缚剑", dealt, true, sourceId);
                    step.TriggerBleed = true;
                    if (gourdTriggered)
                    {
                        step.Label = "残契缚剑 / 葫芦收煞";
                    }
                    return step;
                }
                case BossPhaseXuantieCore:
                {
                    if (!calamityCharged)
                    {
                        calamityCharged = true;
                        return DemoBattlePresentationStep.Charge("剑炉开膛", 0, sourceId);
                    }

                    calamityCharged = false;
                    int dealt = ApplyIncomingDamage(13, "镇矿重斩", true, out bool gourdTriggered);
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("镇矿重斩", dealt, true, sourceId);
                    step.HeavyImpact = true;
                    if (gourdTriggered)
                    {
                        step.Label = "镇矿重斩 / 葫芦收煞";
                    }
                    return step;
                }
                default:
                    return DemoBattlePresentationStep.Enemy(source?.Combatant?.Name ?? "玄铁镇矿剑傀", 0, true, sourceId);
            }
        }

        private void ConfigureAllEnemyIntents()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                DemoCombatTarget target = enemies[i];
                if (target != null && target.IsActive && !target.IsDead)
                {
                    ConfigureEnemyIntent(target);
                }
            }
            SyncLegacyIntentView();
        }

        private void ConfigureEnemyIntent(DemoCombatTarget target)
        {
            if (target == null || !target.IsActive || target.IsDead)
            {
                return;
            }

            float duration;
            string behaviorId;
            string displayText;
            if (isSwordPuppetBoss)
            {
                switch (bossPhaseId)
                {
                    case BossPhaseXuantieContractSpike:
                        duration = 5.4f;
                        behaviorId = "residual_contract_bind";
                        displayText = "残契缚剑：压制回灵并扰乱飞剑";
                        break;
                    case BossPhaseXuantieCore:
                        duration = calamityCharged ? 3.6f : 5.2f;
                        behaviorId = calamityCharged ? "mine_suppression_cleave" : "sword_furnace_charge";
                        displayText = calamityCharged ? "镇矿重斩：短读条重击" : "剑炉开膛：核心正在蓄势";
                        break;
                    default:
                        duration = 6.2f;
                        behaviorId = "black_iron_suppression";
                        displayText = "玄铁镇压：甲片将迎面砸落";
                        break;
                }
            }
            else if (isBossBattle)
            {
                switch (bossPhaseId)
                {
                    case BossPhaseLegacySoulLock:
                        duration = 5.5f;
                        behaviorId = "legacy_soul_lock";
                        displayText = "锁魂重击：高压伤害并压制回灵";
                        break;
                    case BossPhaseLegacyCalamity:
                        duration = calamityCharged ? 3.5f : 5f;
                        behaviorId = "legacy_calamity";
                        displayText = calamityCharged ? "短读条重击" : "守关强敌正在蓄势";
                        break;
                    default:
                        duration = 6.5f;
                        behaviorId = "legacy_boss_pressure";
                        displayText = "守关压制：稳定压迫气血";
                        break;
                }
            }
            else
            {
                float min = Math.Max(TimerEpsilon, setup.EnemyIntentMinSeconds);
                float max = Math.Max(min, setup.EnemyIntentMaxSeconds);
                duration = min + (float)random.NextDouble() * (max - min) + target.Depth * 0.28f;
                behaviorId = "enemy_basic_attack";
                displayText = $"{target.Combatant.Name} 正在酝酿下一击";
            }

            target.Intent = new DemoIntentState
            {
                BehaviorId = behaviorId,
                TargetCombatantId = "player",
                DurationSeconds = duration,
                RemainingSeconds = duration,
                DisplayText = displayText,
                IsPending = true,
                IsKnown = true,
                ThreatPriority = target.ThreatPriority
            };
        }

        private DemoIntentState GetSelectedIntent()
        {
            return ResolveCurrentTargetNode()?.Intent;
        }

        private void SyncLegacyIntentView()
        {
            DemoIntentState intent = GetSelectedIntent();
            enemyIntentDuration = intent?.DurationSeconds ?? 0f;
            enemyIntentRemaining = intent?.RemainingSeconds ?? 0f;
            EnemyIntentText = intent?.DisplayText ?? string.Empty;
            BossIntentText = isBossBattle ? EnemyIntentText : string.Empty;
        }

        private float GetNextEnemyIntentRemaining()
        {
            float next = enemies
                .Where(target => target != null && target.IsActive && !target.IsDead
                    && target.Intent != null && target.Intent.IsPending)
                .Select(target => target.Intent.RemainingSeconds)
                .DefaultIfEmpty(float.PositiveInfinity)
                .Min();
            return float.IsPositiveInfinity(next) ? 60f : next;
        }

        private bool HasReadyEnemyIntent()
        {
            return enemies.Any(target => target != null && target.IsActive && !target.IsDead
                && target.Intent != null && target.Intent.IsPending
                && target.Intent.RemainingSeconds <= TimerEpsilon);
        }

        private void TryAddBossPhaseShift()
        {
            if (!isBossBattle || isSwordPuppetBoss)
            {
                return;
            }

            DemoCombatTarget target = ResolveCurrentTargetNode();
            if (target == null || target.IsDead)
            {
                return;
            }

            string nextPhase = GetLegacyBossPhaseIdForHealth(target.Health, target.Combatant.MaxHealth);
            if (nextPhase == bossPhaseId)
            {
                return;
            }

            bossPhaseId = nextPhase;
            calamityCharged = false;
            string label = bossPhaseId == BossPhaseLegacySoulLock
                ? "雷云翻涌，天雷锁魂"
                : "劫云尽开，天劫降临";
            Log.Add(label);
            Emit(DemoBattlePresentationStep.PhaseShift(label, target.CombatantId));
            ConfigureEnemyIntent(target);
            SyncLegacyIntentView();
        }

        private static string GetLegacyBossPhaseIdForHealth(int health, int maxHealth)
        {
            float ratio = maxHealth <= 0 ? 0f : (float)health / maxHealth;
            if (ratio <= 0.35f)
            {
                return BossPhaseLegacyCalamity;
            }

            return ratio <= 0.7f ? BossPhaseLegacySoulLock : BossPhaseLegacyThunderCloud;
        }

        private static DemoBossPhase GetLegacyBossPhase(string phaseId)
        {
            switch (phaseId)
            {
                case BossPhaseLegacySoulLock:
                case BossPhaseXuantieContractSpike:
                    return DemoBossPhase.SoulLock;
                case BossPhaseLegacyCalamity:
                case BossPhaseXuantieCore:
                    return DemoBossPhase.CalamityDescends;
                case BossPhaseLegacyThunderCloud:
                case BossPhaseXuantieArmor:
                    return DemoBossPhase.ThunderCloud;
                default:
                    return DemoBossPhase.None;
            }
        }

        private static bool IsSwordPuppetTargetSet(IEnumerable<DemoCombatTarget> targets)
        {
            return targets != null && targets.Any(target => target != null
                && (ContainsOrdinal(target.PositionId, "boss_upper_armor")
                    || ContainsOrdinal(target.PositionId, "boss_contract_spike")
                    || ContainsOrdinal(target.PositionId, "boss_furnace_core")
                    || ContainsOrdinal(target.DefinitionId, "xuantie_mine_sword_puppet")));
        }

        private static bool ContainsOrdinal(string value, string fragment)
        {
            return !string.IsNullOrEmpty(value)
                && value.IndexOf(fragment, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void InitializeSwordPuppetTargets()
        {
            DemoCombatTarget armor = enemies.FirstOrDefault(IsArmorTarget) ?? enemies.FirstOrDefault();
            for (int i = 0; i < enemies.Count; i++)
            {
                DemoCombatTarget target = enemies[i];
                bool active = ReferenceEquals(target, armor);
                target.IsActive = active;
                target.CanLock = active;
                target.Intent = DemoIntentState.None;
            }
        }

        private static bool IsArmorTarget(DemoCombatTarget target)
        {
            return target != null && (ContainsOrdinal(target.PositionId, "armor")
                || ContainsOrdinal(target.CombatantId, "armor"));
        }

        private static bool IsContractSpikeTarget(DemoCombatTarget target)
        {
            return target != null && (ContainsOrdinal(target.PositionId, "contract_spike")
                || ContainsOrdinal(target.CombatantId, "contract_spike"));
        }

        private static bool IsCoreTarget(DemoCombatTarget target)
        {
            return target != null && (ContainsOrdinal(target.PositionId, "core")
                || ContainsOrdinal(target.CombatantId, "core"));
        }

        private int ResolvePlayerDamageAgainstTargets(DemoCard card, DemoCombatTarget primaryTarget, int amount)
        {
            if (card.Style == DemoSwordStyle.Wanjian || card.ConsumeAllSwordIntent)
            {
                return ApplyAreaDamage(amount, DemoDamageType.Sword, card.Id, 0.65f, primaryTarget);
            }

            if (card.Style == DemoSwordStyle.Thunder && ActiveEnemyCount > 1)
            {
                return ApplyChainDamage(primaryTarget, amount, card.Id, 3);
            }

            return ApplyDamageToTarget(primaryTarget, amount, DemoDamageType.Sword, card.Id);
        }

        private int ResolveVolleyDamageAgainstTargets(DemoCombatTarget primaryTarget, int amount, DemoSwordStyle style)
        {
            if (style == DemoSwordStyle.Wanjian && ActiveEnemyCount > 1)
            {
                return ApplyAreaDamage(amount, DemoDamageType.Sword, "auto_sword_volley", 0.58f, primaryTarget);
            }

            if (style == DemoSwordStyle.Thunder && ActiveEnemyCount > 1)
            {
                return ApplyChainDamage(primaryTarget, amount, "auto_thunder_sword_volley", 3);
            }

            return ApplyDamageToTarget(primaryTarget, amount, DemoDamageType.Sword, "auto_sword_volley");
        }

        private int ApplyAreaDamage(
            int amount,
            DemoDamageType damageType,
            string effectId,
            float secondaryScale,
            DemoCombatTarget primaryTarget = null)
        {
            DemoCombatTarget primary = primaryTarget ?? ResolveCurrentTargetNode();
            IReadOnlyList<DemoCombatTarget> active = targetResolver?.QueryTargets(DemoTargetQuery.ActiveLockable())
                ?? Array.Empty<DemoCombatTarget>();
            int total = 0;
            for (int i = 0; i < active.Count; i++)
            {
                DemoCombatTarget target = active[i];
                int targetAmount = ReferenceEquals(target, primary)
                    ? amount
                    : Math.Max(1, (int)Math.Round(amount * secondaryScale));
                total += ApplyDamageToTarget(target, targetAmount, damageType, effectId, true, i);
            }
            return total;
        }

        private int ApplyChainDamage(DemoCombatTarget primaryTarget, int amount, string effectId, int maxTargets)
        {
            if (primaryTarget == null)
            {
                return 0;
            }

            DemoChainContext chain = new DemoChainContext();
            List<DemoCombatTarget> ordered = new List<DemoCombatTarget> { primaryTarget };
            ordered.AddRange((targetResolver?.QueryTargets(DemoTargetQuery.ActiveLockable())
                    ?? Array.Empty<DemoCombatTarget>())
                .Where(target => !ReferenceEquals(target, primaryTarget))
                .OrderBy(target => Math.Abs(target.Depth - primaryTarget.Depth))
                .ThenBy(target => target.PositionId, StringComparer.Ordinal));

            int total = 0;
            int chainIndex = 0;
            for (int i = 0; i < ordered.Count && chainIndex < Math.Max(1, maxTargets); i++)
            {
                DemoCombatTarget target = ordered[i];
                if (!chain.TryVisit(target.CombatantId))
                {
                    continue;
                }

                int targetAmount = chainIndex == 0
                    ? amount
                    : Math.Max(1, (int)Math.Round(amount * (chainIndex == 1 ? 0.60f : 0.40f)));
                total += ApplyDamageToTarget(target, targetAmount, DemoDamageType.Lightning, effectId, false, chainIndex);
                chainIndex++;
            }
            return total;
        }

        private int ApplyDamageToTarget(
            DemoCombatTarget target,
            int amount,
            DemoDamageType damageType,
            string effectId,
            bool isArea = false,
            int chainIndex = 0)
        {
            if (target == null || !target.IsActive || target.IsDead || amount <= 0)
            {
                return 0;
            }

            DemoDamageRequest request = new DemoDamageRequest(
                "player",
                target.CombatantId,
                amount,
                damageType,
                effectId,
                isArea,
                chainIndex);
            DemoDamageResult result = DemoDamageResult.Apply(request, target);
            if (result.AppliedAmount > 0 && !lastResolvedTargetIds.Contains(target.CombatantId))
            {
                lastResolvedTargetIds.Add(target.CombatantId);
            }
            if (result.WasKilled)
            {
                RegisterTargetDefeated(target);
            }
            return result.HealthDamage;
        }

        private void RegisterTargetDefeated(DemoCombatTarget target)
        {
            if (target == null)
            {
                return;
            }

            target.IsActive = false;
            target.CanLock = false;
            target.Intent = DemoIntentState.None;
            pendingAfterAttackPresentations.Add(DemoBattlePresentationStep.TargetDefeated(
                target.CombatantId,
                target.Combatant?.Name ?? "破敌"));

            if (!isSwordPuppetBoss)
            {
                targetResolver?.ResolveAutoTarget();
                SyncLegacyIntentView();
                return;
            }

            DemoCombatTarget next = null;
            string nextPhase = bossPhaseId;
            string phaseLabel = string.Empty;
            if (IsArmorTarget(target))
            {
                next = enemies.FirstOrDefault(IsContractSpikeTarget);
                nextPhase = BossPhaseXuantieContractSpike;
                phaseLabel = "甲片崩落，朱砂契钉显形";
            }
            else if (IsContractSpikeTarget(target))
            {
                next = enemies.FirstOrDefault(IsCoreTarget);
                nextPhase = BossPhaseXuantieCore;
                phaseLabel = "残契断裂，青绿剑炉核心暴露";
            }

            if (next != null && !next.IsDead)
            {
                bossPhaseId = nextPhase;
                calamityCharged = false;
                next.IsActive = true;
                next.CanLock = true;
                ConfigureEnemyIntent(next);
                targetResolver?.ClearLock();
                targetResolver?.ResolveAutoTarget();
                pendingAfterAttackPresentations.Add(DemoBattlePresentationStep.PhaseShift(
                    phaseLabel,
                    next.CombatantId));
                Log.Add(phaseLabel);
            }
            SyncLegacyIntentView();
        }

        private IReadOnlyList<DemoCombatTarget> ResolveEffectTargets(DemoCombatTarget primaryTarget, bool spread)
        {
            if (!spread || lastResolvedTargetIds.Count <= 1)
            {
                return primaryTarget != null && primaryTarget.IsActive && !primaryTarget.IsDead
                    ? new[] { primaryTarget }
                    : Array.Empty<DemoCombatTarget>();
            }

            return lastResolvedTargetIds
                .Select(FindTarget)
                .Where(target => target != null && target.IsActive && !target.IsDead)
                .ToArray();
        }

        private void FlushAfterAttackPresentations()
        {
            for (int i = 0; i < pendingAfterAttackPresentations.Count; i++)
            {
                Emit(pendingAfterAttackPresentations[i]);
            }
            pendingAfterAttackPresentations.Clear();
        }

        private void CheckForBattleResult()
        {
            bool allRequiredEnemiesDefeated = enemies.Count > 0
                && enemies
                    .Where(target => target != null && target.RequiredForVictory)
                    .All(target => target.IsDead);
            if (allRequiredEnemiesDefeated)
            {
                SetBattleResult(DemoBattlePhase.Won);
            }
            else if (Player != null && Player.IsDead)
            {
                SetBattleResult(DemoBattlePhase.Lost);
            }
        }

        private void SetBattleResult(DemoBattlePhase result)
        {
            if (Phase == DemoBattlePhase.Won || Phase == DemoBattlePhase.Lost)
            {
                return;
            }

            Phase = result;
            PhaseTimer = 0f;
            EnemyIntentText = string.Empty;
            BossIntentText = string.Empty;
            if (result == DemoBattlePhase.Won)
            {
                Log.Add("剑光破敌，战斗胜利。");
                string defeatedTargetId = enemies
                    .Where(target => target != null && target.IsDead)
                    .OrderByDescending(target => target.Depth)
                    .Select(target => target.CombatantId)
                    .FirstOrDefault();
                Emit(DemoBattlePresentationStep.Victory(defeatedTargetId));
            }
            else
            {
                Log.Add("天命未至，战斗失败。");
                Emit(DemoBattlePresentationStep.Defeat());
            }
        }

        private void AddTemporarySwords(int count)
        {
            if (count <= 0)
            {
                return;
            }

            temporarySwordBatches.Add(new TemporarySwordBatch
            {
                Count = count,
                RemainingVolleys = Math.Max(1, setup.TemporarySwordVolleyCount)
            });
            RecalculateTemporarySwords();
            UpdateMaxSwordCount();
        }

        private void AgeTemporarySwords()
        {
            int expired = 0;
            for (int i = temporarySwordBatches.Count - 1; i >= 0; i--)
            {
                TemporarySwordBatch batch = temporarySwordBatches[i];
                batch.RemainingVolleys--;
                if (batch.RemainingVolleys > 0)
                {
                    continue;
                }

                expired += batch.Count;
                temporarySwordBatches.RemoveAt(i);
            }

            RecalculateTemporarySwords();
            if (expired > 0)
            {
                Log.Add($"{expired} 把临时飞剑完成 {Math.Max(1, setup.TemporarySwordVolleyCount)} 次齐射后散去。");
            }
        }

        private void RecalculateTemporarySwords()
        {
            int total = 0;
            foreach (TemporarySwordBatch batch in temporarySwordBatches)
            {
                total += batch.Count;
            }
            TemporarySwords = total;
        }

        private void UpdateMaxSwordCount()
        {
            MaxSwordCount = Math.Max(MaxSwordCount, TotalSwords);
        }

        private void TrackDamage(int damage)
        {
            HighestSingleDamage = Math.Max(HighestSingleDamage, Math.Max(0, damage));
        }

        private bool HasArtifact(DemoArtifactType type)
        {
            return activeArtifacts.Contains(type);
        }

        private bool HasRelic(string relicName)
        {
            return activeRelics.Contains(relicName);
        }

        private bool HasGongfa(DemoGongfaType type)
        {
            return activeGongfas.Contains(type);
        }

        private static bool IsMirrorTarget(DemoCard card)
        {
            return card.Type == DemoCardType.FlyingSword || card.Type == DemoCardType.Finisher;
        }

        private int ApplyIncomingDamage(int amount, string source, bool bossAttack, out bool gourdTriggered)
        {
            gourdTriggered = false;
            if (HasArtifact(DemoArtifactType.PurpleGourd))
            {
                int absorbed = Math.Min(4, Math.Max(0, amount - 1));
                if (absorbed > 0)
                {
                    amount -= absorbed;
                    gourdTriggered = true;
                    energyValue = Math.Min(MaxEnergy, energyValue + 1f);
                    Log.Add($"紫金葫芦吸收了 {absorbed} 点{source}伤害，并立即返还 1 点灵气。");
                }
            }

            if (bossAttack && HasRelic("护心镜") && !heartMirrorUsed)
            {
                int projectedHealthDamage = Math.Max(0, amount - Player.Block);
                if (projectedHealthDamage >= Player.Health)
                {
                    amount = Math.Min(amount, Player.Block + Math.Max(0, Player.Health - 1));
                    heartMirrorUsed = true;
                    Log.Add("护心镜截住了致命一击，保住最后一线心脉。");
                }
            }

            return Player.TakeDamage(amount);
        }

        private int LosePlayerLife(int amount)
        {
            int lost = Math.Min(Player.Health, Math.Max(0, amount));
            Player.Health = Math.Max(0, Player.Health - Math.Max(0, amount));
            return lost;
        }

        private int ApplyBloodOrbBonus(int damage, string sourceLabel)
        {
            if (damage <= 0 || !HasRelic("血魔珠"))
            {
                return damage;
            }

            int missingPercent = Math.Max(0, 100 - Player.Health * 100 / Math.Max(1, Player.MaxHealth));
            int bonusSteps = missingPercent / 10;
            if (bonusSteps <= 0)
            {
                return damage;
            }

            int bonus = Math.Max(1, (int)Math.Ceiling(damage * 0.08d * bonusSteps));
            Log.Add($"血魔珠以亏空催锋，{sourceLabel} 额外提高 {bonus} 点伤害。");
            return damage + bonus;
        }

        private int ResolveHeavenOpeningBonus(bool mirrored, string logPrefix)
        {
            int spentIntent;
            int spentMomentum;
            if (mirrored && (lastHeavenOpeningIntent > 0 || lastHeavenOpeningMomentum > 0))
            {
                spentIntent = lastHeavenOpeningIntent;
                spentMomentum = lastHeavenOpeningMomentum;
                Log.Add("昊天镜借住先前开天之势，镜中再斩一剑。");
            }
            else
            {
                spentIntent = Player.SwordIntent;
                spentMomentum = storedSwordMomentum;
                lastHeavenOpeningIntent = spentIntent;
                lastHeavenOpeningMomentum = spentMomentum;
                Player.SwordIntent = 0;
                storedSwordMomentum = 0;
            }

            if (spentIntent <= 0 && spentMomentum <= 0)
            {
                return 0;
            }

            int bonus = spentIntent * 5 + spentMomentum * 6;
            if (spentMomentum >= 5)
            {
                bonus += 8;
            }
            Log.Add($"{logPrefix}开天一剑吞下 {spentIntent} 点剑意与 {spentMomentum} 点锋势。");
            return bonus;
        }

        private int DrawCards(int count)
        {
            int drawn = 0;
            for (int i = 0; i < count && Hand.Count < HandLimit; i++)
            {
                if (DrawPile.Count == 0 && !RecycleDiscardPile())
                {
                    break;
                }

                DemoCard card = DrawPile[0];
                DrawPile.RemoveAt(0);
                Hand.Add(card);
                drawn++;
            }
            return drawn;
        }

        private bool RecycleDiscardPile()
        {
            if (DiscardPile.Count == 0)
            {
                return false;
            }

            DrawPile.AddRange(DiscardPile);
            DiscardPile.Clear();
            Shuffle(DrawPile);
            ShuffleCount++;
            firstFlyingSwordPlayed = false;
            spiritTalismanAvailable = HasRelic("聚灵符");
            if (HasGongfa(DemoGongfaType.SwordHeartResonance))
            {
                Player.SwordIntent += 1;
                Log.Add("剑心通明随弃牌洗回而运转，获得 1 点剑意。");
            }
            Log.Add("弃牌堆洗回抽牌堆，新一轮牌库循环开始。");
            return true;
        }

        private void EnsureOpeningPathCardInHand()
        {
            if (Hand.Count == 0)
            {
                return;
            }

            string basicPathCardId = DemoCardLibrary.GetBasicPathCardId(GetVolleyStyle());
            if (string.IsNullOrEmpty(basicPathCardId) || Hand.Exists(card => card != null && card.Id == basicPathCardId))
            {
                return;
            }

            int drawIndex = DrawPile.FindIndex(card => card != null && card.Id == basicPathCardId);
            if (drawIndex < 0)
            {
                return;
            }

            int handIndex = Hand.FindLastIndex(card => card != null && card.Style == DemoSwordStyle.General);
            if (handIndex < 0)
            {
                handIndex = Hand.Count - 1;
            }

            DemoCard incoming = DrawPile[drawIndex];
            DemoCard outgoing = Hand[handIndex];
            Hand[handIndex] = incoming;
            DrawPile[drawIndex] = outgoing;
            Log.Add($"首战引路：{incoming.Name} 落入起手，让第一念先立道。");
        }

        private DemoSwordStyle GetVolleyStyle()
        {
            if (HasGongfa(DemoGongfaType.ThunderScripture) || HasRelic("雷心"))
            {
                return DemoSwordStyle.Thunder;
            }
            if (HasGongfa(DemoGongfaType.BloodFiendCanon) || HasRelic("血剑胚"))
            {
                return DemoSwordStyle.Blood;
            }
            return DemoSwordStyle.Wanjian;
        }

        private void Shuffle(List<DemoCard> cards)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                int swapIndex = random.Next(i, cards.Count);
                DemoCard temp = cards[i];
                cards[i] = cards[swapIndex];
                cards[swapIndex] = temp;
            }
        }

        private void BeginPresentationOperation()
        {
            operationPresentationSteps.Clear();
        }

        private void Emit(DemoBattlePresentationStep step)
        {
            if (step == null)
            {
                return;
            }

            step.Sequence = ++nextPresentationSequence;
            step.BattleTime = ElapsedSeconds;
            presentationQueue.Enqueue(step);
            operationPresentationSteps.Add(step);
        }

        private void PublishPresentationOperation()
        {
            if (operationPresentationSteps.Count == 0)
            {
                return;
            }

            ExecutionSequenceVersion++;
        }
    }
}
