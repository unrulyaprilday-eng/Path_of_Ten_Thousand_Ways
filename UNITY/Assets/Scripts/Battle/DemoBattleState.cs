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
        public IReadOnlyList<DemoCard> Deck;
        public IReadOnlyList<DemoArtifactType> Artifacts;
        public IReadOnlyList<DemoGongfaType> Gongfas;
        public IReadOnlyCollection<string> Relics;
        public string PlayerName = "剑修";
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

    public sealed class DemoBattleState
    {
        private const float TimerEpsilon = 0.0001f;

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

        private DemoBattleSetup setup;
        private bool isBossBattle;
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
        public DemoCombatant Enemy { get; private set; }
        public DemoBattlePhase Phase { get; private set; } = DemoBattlePhase.Intro;
        public DemoBossPhase BossPhase { get; private set; }
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
        public float EnemyIntentRemaining => enemyIntentRemaining;
        public float EnemyIntentDuration => enemyIntentDuration;
        public float EnemyIntentProgress => enemyIntentDuration <= TimerEpsilon
            ? 0f
            : Math.Max(0f, Math.Min(1f, 1f - enemyIntentRemaining / enemyIntentDuration));
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
            Enemy = null;
            setup = null;
            Phase = DemoBattlePhase.Intro;
            BossPhase = DemoBossPhase.None;
            PhaseTimer = 0f;
            EnemyIntentText = string.Empty;
            BossIntentText = string.Empty;
            isBossBattle = false;
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
            random = battleSetup.RandomSeed >= 0 ? new Random(battleSetup.RandomSeed) : new Random();

            int playerMaxHealth = battleSetup.PlayerMaxHealth > 0 ? battleSetup.PlayerMaxHealth : BasePlayerMaxHealth;
            Player = new DemoCombatant(
                string.IsNullOrWhiteSpace(battleSetup.PlayerName) ? "剑修" : battleSetup.PlayerName,
                playerMaxHealth);
            Player.Health = Math.Min(Player.MaxHealth, Math.Max(1, battleSetup.PlayerHealth));

            string enemyName = string.IsNullOrWhiteSpace(battleSetup.EnemyName) ? "拦路妖物" : battleSetup.EnemyName;
            Enemy = new DemoCombatant(enemyName, Math.Max(1, battleSetup.EnemyHealth));

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
            BossPhase = isBossBattle ? DemoBossPhase.ThunderCloud : DemoBossPhase.None;
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
            if (openingBattlePacing)
            {
                EnsureOpeningPathCardInHand();
            }

            drawRemaining = Math.Max(TimerEpsilon, battleSetup.DrawIntervalSeconds);
            volleyRemaining = Math.Max(TimerEpsilon, battleSetup.FlyingSwordIntervalSeconds);
            ConfigureNextEnemyIntent();

            introRemaining = Math.Max(0f, battleSetup.IntroSeconds);
            Phase = introRemaining > TimerEpsilon ? DemoBattlePhase.Intro : DemoBattlePhase.Running;
            PhaseTimer = Phase == DemoBattlePhase.Intro ? introRemaining : enemyIntentRemaining;
            Log.Add(isBossBattle ? "天劫化身显现，劫云开始聚拢。" : $"{Enemy.Name} 拦路，斗法持续演算。");
            Emit(DemoBattlePresentationStep.BattleStart(Enemy.Name, isBossBattle));
            PublishPresentationOperation();
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
                    PhaseTimer = enemyIntentRemaining;
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
            Emit(DemoBattlePresentationStep.Card(card, damage));

            if (!Enemy.IsDead && !Player.IsDead && mirrorAvailable && IsMirrorTarget(card))
            {
                mirrorAvailable = false;
                Log.Add($"昊天镜映照 {card.Name}，再次结算。");
                int mirroredDamage = ResolveCard(card, true);
                TrackDamage(mirroredDamage);
                DemoBattlePresentationStep mirrorStep = DemoBattlePresentationStep.Card(card, mirroredDamage);
                mirrorStep.Label = $"昊天镜·{card.Name}";
                mirrorStep.HeavyImpact = true;
                Emit(mirrorStep);
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
                step = Math.Min(step, Math.Max(0f, enemyIntentRemaining));
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

                if (Phase == DemoBattlePhase.Running && enemyIntentRemaining <= TimerEpsilon)
                {
                    ResolveEnemyIntent();
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
            enemyIntentRemaining = Math.Max(0f, enemyIntentRemaining - deltaTime);
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

            PhaseTimer = enemyIntentRemaining;
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
                Emit(DemoBattlePresentationStep.SwordVolley(GetVolleyStyle(), swordCount, swordDamage, shockTriggered));
            }

            thunderSealActive = false;
            cardsPlayedSinceVolley = 0;
            AgeTemporarySwords();
            TryAddBossPhaseShift();
            CheckForBattleResult();
        }

        private void ResolveEnemyIntent()
        {
            if (Player.Shock > 0)
            {
                Player.Shock = Math.Max(0, Player.Shock - 1);
            }

            EnemyActionCount++;
            Round = EnemyActionCount;
            DemoBattlePresentationStep step = isBossBattle ? ResolveBossIntent() : ResolveNormalEnemyIntent();
            Emit(step);
            CheckForBattleResult();

            if (Phase == DemoBattlePhase.Running)
            {
                ConfigureNextEnemyIntent();
            }
        }

        private int ResolveCard(DemoCard card, bool mirrored = false)
        {
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

            if (card.Style == DemoSwordStyle.Blood && Enemy.Bleed > 0)
            {
                damage += Enemy.Bleed / 2;
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
                damage = Enemy.TakeDamage(damage);
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
                Enemy.Shock += shock + extraShock;
                Log.Add($"{logPrefix}敌人感电 +{shock + extraShock}。");
            }

            if (bleed > 0)
            {
                Enemy.Bleed += bleed;
                Log.Add($"{logPrefix}敌人流血 +{bleed}。");
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
            int swordCount = TotalSwords;
            int swordDamage = swordCount * 3;
            shockTriggered = false;
            int shockBeforeTrigger = Enemy.Shock;

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
                Enemy.Shock += 2;
                Log.Add("引雷入窍先行为敌身覆雷，感电 +2。");
            }

            if (Enemy.Shock > 0)
            {
                if (thunderSealActive)
                {
                    int storedThunder = Math.Max(4, Enemy.Shock / 2 + 1);
                    deferredThunderBonusDamage += storedThunder;
                    Enemy.Shock += 2;
                    Log.Add($"封雷匣将雷意继续压入敌躯，本次齐射不引爆，并为下次雷击蓄下 {storedThunder} 点天罚。");
                }
                else
                {
                    int shockDamage = Math.Max(1, Enemy.Shock / 2);
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
                    Enemy.Shock = Math.Max(0, Enemy.Shock - 2);
                    shockTriggered = true;
                    Log.Add($"感电被飞剑引爆，追加 {shockDamage} 伤害。");
                }
            }

            if (Enemy.Bleed > 0)
            {
                if (HasRelic("血剑胚"))
                {
                    Enemy.Bleed += 2;
                    Log.Add("血剑胚浸染剑锋，额外施加 2 层流血。");
                }

                if (HasGongfa(DemoGongfaType.BloodFiendCanon))
                {
                    int bloodBonus = Math.Max(2, Enemy.Bleed / 2);
                    swordDamage += bloodBonus;
                    Player.Heal(2);
                    Log.Add($"血煞经借血催锋，齐射追加 {bloodBonus} 伤害并回复 2 点生命。");
                }
            }

            swordDamage = ApplyBloodOrbBonus(swordDamage, "飞剑齐发");
            int dealt = Enemy.TakeDamage(swordDamage);
            int totalDamage = dealt;
            Log.Add($"{swordCount} 把飞剑自动攻击，造成 {dealt} 点伤害。");

            if (!Enemy.IsDead && HasRelic("万剑剑匣") && random.NextDouble() < 0.35d)
            {
                int echoDamage = ApplyBloodOrbBonus(Math.Max(6, swordCount * 2), "万剑剑匣");
                int echoed = Enemy.TakeDamage(echoDamage);
                totalDamage += echoed;
                Log.Add($"万剑剑匣牵动归锋，再追斩 {echoed} 点伤害。");
            }

            int bleedDamage = Enemy.TickBleed();
            if (bleedDamage > 0)
            {
                totalDamage += bleedDamage;
                Log.Add($"流血造成 {bleedDamage} 伤害。");

                if (HasGongfa(DemoGongfaType.BloodRefiningBody))
                {
                    Player.Heal(2);
                    Player.SwordIntent += 1;
                    Log.Add("血炼归元炼化煞气，回复 2 点生命并获得 1 点剑意。");
                }
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.WanjianReturn) && (Player.SwordIntent >= 4 || swordCount >= 5))
            {
                int returnDamage = ApplyBloodOrbBonus(Math.Max(8, swordCount * 2), "万剑归宗");
                int returned = Enemy.TakeDamage(returnDamage);
                totalDamage += returned;
                Log.Add($"万剑归宗回潮，再次斩出 {returned} 点伤害。");
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.HeavenlyThunderEdict) && shockTriggered)
            {
                int thunderDamage = ApplyBloodOrbBonus(10, "九天引雷");
                int dealtThunder = Enemy.TakeDamage(thunderDamage);
                totalDamage += dealtThunder;
                Log.Add($"九天引雷落下天罚，追加 {dealtThunder} 点伤害。");
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.BloodPrisonExecution) && Enemy.Bleed >= 8)
            {
                int bloodPrisonDamage = ApplyBloodOrbBonus(12 + Enemy.Bleed / 2, "血狱断生");
                int dealtBloodPrison = Enemy.TakeDamage(bloodPrisonDamage);
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

        private DemoBattlePresentationStep ResolveNormalEnemyIntent()
        {
            int damage = 7 + Math.Min(3, Math.Max(0, EnemyActionCount - 1) / 3);
            int dealt = ApplyIncomingDamage(damage, Enemy.Name, false, out bool gourdTriggered);
            Log.Add($"{Enemy.Name} 读条完成，造成 {dealt} 点伤害。");
            DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy(Enemy.Name, dealt, false);
            if (gourdTriggered)
            {
                step.Label = $"{Enemy.Name} / 葫芦收煞";
            }
            return step;
        }

        private DemoBattlePresentationStep ResolveBossIntent()
        {
            switch (BossPhase)
            {
                case DemoBossPhase.ThunderCloud:
                {
                    int damage = 6 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 5);
                    int dealt = ApplyIncomingDamage(damage, "雷云压境", true, out bool gourdTriggered);
                    Player.Shock += 1;
                    Log.Add($"雷云压境造成 {dealt} 伤害并施加 1 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("雷云压境", dealt, true);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 1;
                    if (gourdTriggered)
                    {
                        step.Label = "雷云压境 / 葫芦收煞";
                    }
                    return step;
                }
                case DemoBossPhase.SoulLock:
                {
                    int damage = 8 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 6) + (Player.Shock >= 3 ? 1 : 0);
                    int dealt = ApplyIncomingDamage(damage, "天雷锁魂", true, out bool gourdTriggered);
                    Player.Shock += 2;
                    energyRegenerationSuppressedRemaining = Math.Max(energyRegenerationSuppressedRemaining, 2f);
                    Log.Add($"天雷锁魂造成 {dealt} 伤害、施加 2 感电，并压制灵气恢复 2 秒。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天雷锁魂", dealt, true);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 2;
                    if (gourdTriggered)
                    {
                        step.Label = "天雷锁魂 / 葫芦收煞";
                    }
                    return step;
                }
                case DemoBossPhase.CalamityDescends:
                {
                    if (!calamityCharged)
                    {
                        calamityCharged = true;
                        Player.Shock += 1;
                        Log.Add("天劫化身开始蓄雷，短读条结束后将降下重击。");
                        RefreshIntentText();
                        return DemoBattlePresentationStep.Charge("天劫蓄雷", 1);
                    }

                    int damage = 12 + Math.Min(2, Math.Max(0, EnemyActionCount - 1) / 8) + Math.Min(2, Player.Shock);
                    int dealt = ApplyIncomingDamage(damage, "天劫降临", true, out bool gourdTriggered);
                    Player.Shock += 1;
                    calamityCharged = false;
                    Log.Add($"天劫降临造成 {dealt} 伤害并施加 1 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天劫降临", dealt, true);
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
                    return DemoBattlePresentationStep.Enemy(Enemy.Name, 0, true);
            }
        }

        private void ConfigureNextEnemyIntent()
        {
            if (isBossBattle)
            {
                switch (BossPhase)
                {
                    case DemoBossPhase.ThunderCloud:
                        enemyIntentDuration = 6.5f;
                        break;
                    case DemoBossPhase.SoulLock:
                        enemyIntentDuration = 5.5f;
                        break;
                    case DemoBossPhase.CalamityDescends:
                        enemyIntentDuration = calamityCharged ? 3.5f : 5f;
                        break;
                    default:
                        enemyIntentDuration = 6f;
                        break;
                }
            }
            else
            {
                float min = Math.Max(TimerEpsilon, setup.EnemyIntentMinSeconds);
                float max = Math.Max(min, setup.EnemyIntentMaxSeconds);
                enemyIntentDuration = min + (float)random.NextDouble() * (max - min);
            }

            enemyIntentRemaining = enemyIntentDuration;
            PhaseTimer = enemyIntentRemaining;
            RefreshIntentText();
        }

        private void RefreshIntentText()
        {
            if (!isBossBattle)
            {
                EnemyIntentText = Enemy == null ? string.Empty : $"{Enemy.Name} 正在酝酿下一击";
                BossIntentText = string.Empty;
                return;
            }

            switch (BossPhase)
            {
                case DemoBossPhase.ThunderCloud:
                    EnemyIntentText = "雷云压境：稳定压血，施加轻度感电。";
                    break;
                case DemoBossPhase.SoulLock:
                    EnemyIntentText = "天雷锁魂：高压伤害，并短暂压制灵气恢复。";
                    break;
                case DemoBossPhase.CalamityDescends:
                    EnemyIntentText = calamityCharged
                        ? "天劫降临：短读条重击，此刻是爆发斩杀窗口。"
                        : "天劫蓄雷：完成蓄势后进入短读条重击。";
                    break;
                default:
                    EnemyIntentText = string.Empty;
                    break;
            }

            BossIntentText = EnemyIntentText;
        }

        private void TryAddBossPhaseShift()
        {
            if (!isBossBattle || Enemy == null || Enemy.IsDead)
            {
                return;
            }

            DemoBossPhase nextPhase = GetBossPhaseForHealth(Enemy.Health, Enemy.MaxHealth);
            if (nextPhase == BossPhase)
            {
                return;
            }

            BossPhase = nextPhase;
            calamityCharged = false;
            string label = BossPhase == DemoBossPhase.SoulLock
                ? "雷云翻涌，天雷锁魂"
                : "劫云尽开，天劫降临";
            Log.Add(label);
            Emit(DemoBattlePresentationStep.PhaseShift(label));
            ConfigureNextEnemyIntent();
        }

        private static DemoBossPhase GetBossPhaseForHealth(int health, int maxHealth)
        {
            float ratio = maxHealth <= 0 ? 0f : (float)health / maxHealth;
            if (ratio <= 0.35f)
            {
                return DemoBossPhase.CalamityDescends;
            }

            return ratio <= 0.7f ? DemoBossPhase.SoulLock : DemoBossPhase.ThunderCloud;
        }

        private void CheckForBattleResult()
        {
            if (Enemy != null && Enemy.IsDead)
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
                Emit(DemoBattlePresentationStep.Victory());
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
