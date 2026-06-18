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
        Planning,
        Executing,
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

    public sealed class DemoBattleState
    {
        private const int PlanningSeconds = 15;
        private const float StepDurationSeconds = 0.7f;
        private const float MinimumExecutionSeconds = 2.4f;
        private readonly Random random = new Random();
        private DemoBattlePhase? pendingResult;
        private bool isBossBattle;
        private bool calamityCharged;
        private bool mirrorAvailable;
        private bool heartMirrorUsed;
        private bool spiritTalismanAvailable;
        private bool openingBattlePacing;
        private bool sheatheEdgeActive;
        private bool thunderSealActive;
        private int storedGourdEnergy;
        private int storedSwordMomentum;
        private int deferredThunderBonusDamage;
        private int lastHeavenOpeningMomentum;
        private int lastHeavenOpeningIntent;
        private readonly List<DemoArtifactType> activeArtifacts = new List<DemoArtifactType>();
        private readonly List<DemoGongfaType> activeGongfas = new List<DemoGongfaType>();
        private readonly HashSet<string> activeRelics = new HashSet<string>();
        private bool firstFlyingSwordPlayed;
        private int plannedCardsThisRound;

        public DemoCombatant Player { get; private set; }
        public DemoCombatant Enemy { get; private set; }
        public DemoBattlePhase Phase { get; private set; }
        public DemoBossPhase BossPhase { get; private set; }
        public DemoBattleLog Log { get; } = new DemoBattleLog();

        public List<DemoCard> Deck { get; } = new List<DemoCard>();
        public List<DemoCard> DrawPile { get; } = new List<DemoCard>();
        public List<DemoCard> Hand { get; } = new List<DemoCard>();
        public List<DemoCard> DiscardPile { get; } = new List<DemoCard>();
        public List<DemoCard> PlayQueue { get; } = new List<DemoCard>();
        public List<DemoBattlePresentationStep> CurrentPresentationSteps { get; } = new List<DemoBattlePresentationStep>();

        public int Round { get; private set; }
        public int Energy { get; private set; }
        public int MaxEnergy { get; private set; } = 3;
        public int PermanentSwords { get; private set; } = 1;
        public int TemporarySwords { get; private set; }
        public float PhaseTimer { get; private set; }
        public int ExecutionSequenceVersion { get; private set; }
        public string BossIntentText { get; private set; } = string.Empty;
        public bool IsBossBattle => isBossBattle;
        public bool IsOpeningBattlePacing => openingBattlePacing;
        public IReadOnlyList<DemoArtifactType> ActiveArtifacts => activeArtifacts;
        public IReadOnlyList<DemoGongfaType> ActiveGongfas => activeGongfas;

        public int TotalSwords => PermanentSwords + TemporarySwords;

        public void ClearBattle(bool clearLog = false)
        {
            Player = null;
            Enemy = null;
            Phase = DemoBattlePhase.Planning;
            BossPhase = DemoBossPhase.None;
            PhaseTimer = 0f;
            BossIntentText = string.Empty;
            pendingResult = null;
            isBossBattle = false;
            calamityCharged = false;
            mirrorAvailable = false;
            heartMirrorUsed = false;
            spiritTalismanAvailable = false;
            openingBattlePacing = false;
            sheatheEdgeActive = false;
            thunderSealActive = false;
            storedGourdEnergy = 0;
            storedSwordMomentum = 0;
            deferredThunderBonusDamage = 0;
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;
            firstFlyingSwordPlayed = false;
            plannedCardsThisRound = 0;
            Round = 0;
            Energy = 0;
            TemporarySwords = 0;
            ExecutionSequenceVersion++;

            Deck.Clear();
            DrawPile.Clear();
            Hand.Clear();
            DiscardPile.Clear();
            PlayQueue.Clear();
            CurrentPresentationSteps.Clear();

            if (clearLog)
            {
                Log.Clear();
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
            Player = new DemoCombatant("剑修", 72);
            Player.Health = Math.Min(Player.MaxHealth, Math.Max(1, playerHealth));
            Enemy = new DemoCombatant(enemyName, enemyHealth);
            Deck.Clear();
            DrawPile.Clear();
            Hand.Clear();
            DiscardPile.Clear();
            PlayQueue.Clear();
            CurrentPresentationSteps.Clear();
            Log.Clear();
            activeArtifacts.Clear();
            activeGongfas.Clear();
            activeRelics.Clear();
            if (artifacts != null)
            {
                activeArtifacts.AddRange(artifacts);
            }
            if (gongfas != null)
            {
                activeGongfas.AddRange(gongfas.Where(type => type != DemoGongfaType.None));
            }
            if (relics != null)
            {
                foreach (string relic in relics)
                {
                    activeRelics.Add(relic);
                }
            }

            foreach (DemoCard card in deck)
            {
                if (card == null)
                {
                    continue;
                }

                Deck.Add(card.Clone());
                DrawPile.Add(card.Clone());
            }

            Shuffle(DrawPile);
            Round = 0;
            MaxEnergy = 3 + bonusEnergy;
            Energy = MaxEnergy;
            PermanentSwords = (boss ? 2 : 1) + bonusSwords;
            if (HasArtifact(DemoArtifactType.SwordBox))
            {
                PermanentSwords += 1;
            }
            TemporarySwords = 0;
            isBossBattle = boss;
            BossPhase = boss ? DemoBossPhase.ThunderCloud : DemoBossPhase.None;
            calamityCharged = false;
            mirrorAvailable = HasArtifact(DemoArtifactType.HaotianMirror);
            heartMirrorUsed = false;
            spiritTalismanAvailable = false;
            openingBattlePacing = openingBattle;
            sheatheEdgeActive = false;
            thunderSealActive = false;
            storedGourdEnergy = 0;
            storedSwordMomentum = 0;
            deferredThunderBonusDamage = 0;
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;
            firstFlyingSwordPlayed = false;
            plannedCardsThisRound = 0;
            BossIntentText = boss ? GetBossIntentText() : string.Empty;
            pendingResult = null;
            Phase = DemoBattlePhase.Planning;
            Log.Add(boss ? "天劫化身显现，Boss 战开始。" : $"{enemyName} 拦路，战斗开始。");
            StartPlanningRound();
        }

        public void Tick(float deltaTime)
        {
            if (Player == null || Enemy == null)
            {
                return;
            }

            if (Phase != DemoBattlePhase.Planning && Phase != DemoBattlePhase.Executing)
            {
                return;
            }

            PhaseTimer -= deltaTime;

            if (PhaseTimer > 0f)
            {
                return;
            }

            if (Phase == DemoBattlePhase.Planning)
            {
                ExecuteQueuedCards();
            }
            else
            {
                if (pendingResult.HasValue)
                {
                    Phase = pendingResult.Value;
                    pendingResult = null;
                }
                else
                {
                    StartPlanningRound();
                }
            }
        }

        public bool QueueCard(int handIndex)
        {
            if (Phase != DemoBattlePhase.Planning || handIndex < 0 || handIndex >= Hand.Count)
            {
                return false;
            }

            DemoCard card = Hand[handIndex];
            if (card == null)
            {
                Hand.RemoveAt(handIndex);
                Log.Add("一张残缺卡牌未能成形，已从手牌中移除。");
                return false;
            }

            int actualCost = GetCardCost(card);
            if (actualCost > Energy)
            {
                Log.Add($"{card.Name} 灵气不足。");
                return false;
            }

            Energy -= actualCost;
            PlayQueue.Add(card);
            Hand.RemoveAt(handIndex);
            plannedCardsThisRound++;
            if (actualCost < card.Cost)
            {
                Log.Add($"聚灵符映亮经脉，{card.Name} 本回合免费。");
            }
            Log.Add($"规划：{card.Name}。");
            return true;
        }

        public void EndPlanning()
        {
            if (Phase == DemoBattlePhase.Planning)
            {
                ExecuteQueuedCards();
            }
        }

        private void StartPlanningRound()
        {
            if (Enemy.IsDead)
            {
                Phase = DemoBattlePhase.Won;
                Log.Add("敌人溃散，本战胜利。");
                return;
            }

            if (Player.IsDead)
            {
                Phase = DemoBattlePhase.Lost;
                Log.Add("道心崩散，本局失败。");
                return;
            }

            Round++;
            Phase = DemoBattlePhase.Planning;
            PhaseTimer = PlanningSeconds;
            int baseEnergy = openingBattlePacing && Round == 1 ? 1 : MaxEnergy;
            Energy = baseEnergy + storedGourdEnergy;
            if (storedGourdEnergy > 0)
            {
                Log.Add($"紫金葫芦返还 {storedGourdEnergy} 点额外灵气。");
                storedGourdEnergy = 0;
            }

            if (HasGongfa(DemoGongfaType.SwordHeartResonance))
            {
                Player.SwordIntent += 1;
                Log.Add("剑心通明运转，回合开始获得 1 点剑意。");
            }

            TemporarySwords = 0;
            if (HasArtifact(DemoArtifactType.SwordBox))
            {
                TemporarySwords += 1;
                Log.Add("剑匣展开，本回合额外备好 1 把临时飞剑。");
            }
            Player.Block = 0;

            if (Player.Shock > 0)
            {
                int shockTax = isBossBattle && BossPhase != DemoBossPhase.ThunderCloud
                    ? Math.Min(2, Math.Max(1, (Player.Shock + 1) / 2))
                    : 0;

                if (shockTax > 0)
                {
                    Energy = Math.Max(1, Energy - shockTax);
                    Log.Add($"天雷锁魂压制灵台，灵气 -{shockTax}。");
                }

                Player.Shock = Math.Max(0, Player.Shock - 1);
            }

            BossIntentText = isBossBattle ? GetBossIntentText() : string.Empty;
            plannedCardsThisRound = 0;
            spiritTalismanAvailable = HasRelic("聚灵符");
            sheatheEdgeActive = false;
            thunderSealActive = false;
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;
            int targetHandCount = openingBattlePacing && Round == 1 ? 3 : 5;
            DrawCards(targetHandCount - Hand.Count);
            if (openingBattlePacing && Round == 1)
            {
                EnsureOpeningPathCardInHand();
                Log.Add("首战起手收束为一剑一念：本回合仅 1 点灵气，先看清这一道如何出手。");
            }
            Log.Add($"第 {Round} 回合规划开始。");
        }

        private void ExecuteQueuedCards()
        {
            Phase = DemoBattlePhase.Executing;
            Log.Add("演武阶段：卡牌与飞剑开始结算。");
            CurrentPresentationSteps.Clear();
            lastHeavenOpeningMomentum = 0;
            lastHeavenOpeningIntent = 0;

            foreach (DemoCard card in PlayQueue)
            {
                if (card == null)
                {
                    continue;
                }

                int damage = ResolveCard(card);
                CurrentPresentationSteps.Add(DemoBattlePresentationStep.Card(card, damage));

                if (!Enemy.IsDead && !Player.IsDead && mirrorAvailable && IsMirrorTarget(card))
                {
                    mirrorAvailable = false;
                    Log.Add($"昊天镜映照 {card.Name}，再次结算。");
                    int mirroredDamage = ResolveCard(card, true);
                    DemoBattlePresentationStep mirrorStep = DemoBattlePresentationStep.Card(card, mirroredDamage);
                    mirrorStep.Label = $"昊天镜·{card.Name}";
                    mirrorStep.HeavyImpact = true;
                    CurrentPresentationSteps.Add(mirrorStep);
                }

                DiscardPile.Add(card);

                if (Enemy.IsDead || Player.IsDead)
                {
                    break;
                }
            }

            PlayQueue.Clear();

            if (!Enemy.IsDead && !Player.IsDead)
            {
                if (sheatheEdgeActive)
                {
                    ResolveSheatheEdge();
                }
                else
                {
                    int swordDamage = ResolveFlyingSwords(out bool shockTriggered);
                    CurrentPresentationSteps.Add(DemoBattlePresentationStep.SwordVolley(GetVolleyStyle(), TotalSwords, swordDamage, shockTriggered));
                }
            }

            TryAddBossPhaseShift();

            if (!Enemy.IsDead && !Player.IsDead)
            {
                DemoBattlePresentationStep enemyStep = ResolveEnemyTurn();
                CurrentPresentationSteps.Add(enemyStep);
            }

            if (Enemy.IsDead)
            {
                CurrentPresentationSteps.Add(DemoBattlePresentationStep.Victory());
                Log.Add("剑光破敌，战斗胜利。");
                pendingResult = DemoBattlePhase.Won;
            }
            else if (Player.IsDead)
            {
                CurrentPresentationSteps.Add(DemoBattlePresentationStep.Defeat());
                Log.Add("天命未至，战斗失败。");
                pendingResult = DemoBattlePhase.Lost;
            }

            PhaseTimer = Math.Max(MinimumExecutionSeconds, CurrentPresentationSteps.Count * StepDurationSeconds);

            ExecutionSequenceVersion++;
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
            int extraTemporarySwords = 0;
            int extraShock = 0;
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
                    Log.Add($"{logPrefix}藏锋诀压住剑势，本回合飞剑将改为蓄锋。");
                    break;
                case DemoCardSpecialEffect.HeavenOpening:
                    damage += ResolveHeavenOpeningBonus(mirrored, logPrefix);
                    break;
                case DemoCardSpecialEffect.ThunderSeal:
                    thunderSealActive = true;
                    Log.Add($"{logPrefix}封雷匣合拢雷势，本回合感电将继续封存。");
                    break;
            }

            if (card.Type == DemoCardType.FlyingSword && !mirrored)
            {
                if (!firstFlyingSwordPlayed && HasGongfa(DemoGongfaType.SwordControlArt))
                {
                    TemporarySwords += 1;
                    firstFlyingSwordPlayed = true;
                    Log.Add("御剑诀牵引剑势，额外备好 1 把临时飞剑。");
                }
                else if (!firstFlyingSwordPlayed)
                {
                    firstFlyingSwordPlayed = true;
                }
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
                int dealt = Enemy.TakeDamage(damage);
                damage = dealt;
                Log.Add($"{logPrefix}{card.Name} 造成 {dealt} 点伤害。");
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
                extraShock = HasArtifact(DemoArtifactType.ThunderSeal) ? 2 : 0;
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
                extraTemporarySwords = HasArtifact(DemoArtifactType.SwordBox) ? 1 : 0;
                TemporarySwords += temporarySwords + extraTemporarySwords;
                Log.Add($"{logPrefix}临时飞剑 +{temporarySwords + extraTemporarySwords}。");
            }

            if (card.PermanentSword)
            {
                PermanentSwords++;
                Log.Add($"{logPrefix}永久飞剑 +1，当前 {PermanentSwords}。");
            }

            if (energyGain > 0)
            {
                Energy = Math.Min(MaxEnergy, Energy + energyGain);
                Log.Add($"{logPrefix}灵气 +{energyGain}。");
            }

            if (draw > 0)
            {
                DrawCards(draw);
                Log.Add($"{logPrefix}抽牌 +{draw}。");
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
            int swordDamage = TotalSwords * 3;
            shockTriggered = false;
            int shockBeforeTrigger = Enemy.Shock;

            if (Player.SwordIntent >= 3)
            {
                swordDamage += TotalSwords;
            }

            if (HasGongfa(DemoGongfaType.SwordHeartResonance) && plannedCardsThisRound <= 2)
            {
                swordDamage += 4;
                Log.Add("剑心通明收束出手，本轮演武额外造成 4 点伤害。");
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
                    Log.Add($"封雷匣将雷意继续压入敌躯，本回合不引爆，并为下次雷击额外蓄下 {storedThunder} 点天罚。");
                }
                else
                {
                    int shockDamage = Math.Max(1, Enemy.Shock / 2);
                    if (HasArtifact(DemoArtifactType.ThunderSeal))
                    {
                        shockDamage += 4;
                    }
                    if (activeRelics.Contains("雷心"))
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
                if (activeRelics.Contains("血剑胚"))
                {
                    Enemy.Bleed += 2;
                    Log.Add("血剑胚浸染剑锋，额外施加 2 层流血。");
                }

                if (HasGongfa(DemoGongfaType.BloodFiendCanon))
                {
                    int bloodBonus = Math.Max(2, Enemy.Bleed / 2);
                    swordDamage += bloodBonus;
                    Player.Heal(2);
                    Log.Add($"血煞经借血催锋，演武追加 {bloodBonus} 伤害并回复 2 点生命。");
                }
            }

            swordDamage = ApplyBloodOrbBonus(swordDamage, "飞剑齐发");
            int dealt = Enemy.TakeDamage(swordDamage);
            swordDamage = dealt;
            Log.Add($"{TotalSwords} 把飞剑自动攻击，造成 {dealt} 伤害。");

            if (!Enemy.IsDead && HasRelic("万剑剑匣") && random.NextDouble() < 0.35d)
            {
                int echoDamage = ApplyBloodOrbBonus(Math.Max(6, TotalSwords * 2), "万剑剑匣");
                int echoed = Enemy.TakeDamage(echoDamage);
                swordDamage += echoed;
                Log.Add($"万剑剑匣牵动归锋，再追斩 {echoed} 点伤害。");
            }

            int bleedDamage = Enemy.TickBleed();
            if (bleedDamage > 0)
            {
                Log.Add($"流血造成 {bleedDamage} 伤害。");

                if (HasGongfa(DemoGongfaType.BloodRefiningBody))
                {
                    Player.Heal(2);
                    Player.SwordIntent += 1;
                    Log.Add("血炼归元炼化煞气，回复 2 点生命并获得 1 点剑意。");
                }
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.WanjianReturn) && (Player.SwordIntent >= 4 || TotalSwords >= 5))
            {
                int returnDamage = ApplyBloodOrbBonus(Math.Max(8, TotalSwords * 2), "万剑归宗");
                int returned = Enemy.TakeDamage(returnDamage);
                swordDamage += returned;
                Log.Add($"万剑归宗回潮，再次斩出 {returned} 点伤害。");
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.HeavenlyThunderEdict) && shockTriggered)
            {
                int thunderDamage = ApplyBloodOrbBonus(10, "九天引雷");
                int dealtThunder = Enemy.TakeDamage(thunderDamage);
                swordDamage += dealtThunder;
                Log.Add($"九天引雷落下天罚，追加 {dealtThunder} 点伤害。");
            }

            if (!Enemy.IsDead && HasGongfa(DemoGongfaType.BloodPrisonExecution) && Enemy.Bleed >= 8)
            {
                int bloodPrisonDamage = ApplyBloodOrbBonus(12 + Enemy.Bleed / 2, "血狱断生");
                int dealtBloodPrison = Enemy.TakeDamage(bloodPrisonDamage);
                swordDamage += dealtBloodPrison;
                Player.Heal(4);
                Log.Add($"血狱断生收束残血，追加 {dealtBloodPrison} 点伤害并回复 4 点生命。");
            }

            return swordDamage + bleedDamage;
        }

        private void ResolveSheatheEdge()
        {
            int swords = TotalSwords;
            if (swords <= 0)
            {
                Log.Add("藏锋诀收势未成，此刻没有可入鞘的飞剑。");
                return;
            }

            int gainedIntent = Math.Max(2, (swords + 1) / 2);
            storedSwordMomentum += swords;
            Player.SwordIntent += gainedIntent;
            Log.Add($"藏锋诀收剑入鞘，{swords} 把飞剑化为 {gainedIntent} 点剑意，并积蓄 {swords} 点锋势。");
        }

        private DemoBattlePresentationStep ResolveEnemyTurn()
        {
            if (isBossBattle)
            {
                return ResolveBossTurn();
            }

            int baseDamage = 8;
            int pressure = Math.Min(6, Round);
            int damage = baseDamage + pressure;

            int dealt = ApplyIncomingDamage(damage, Enemy.Name, false, out int gourdGain);
            Log.Add($"{Enemy.Name} 反击，造成 {dealt} 伤害。");
            DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy(Enemy.Name, dealt, false);
            if (gourdGain > 0)
            {
                step.Label = $"{Enemy.Name} / 葫芦收煞";
            }
            return step;
        }

        private DemoSwordStyle GetVolleyStyle()
        {
            if (HasGongfa(DemoGongfaType.ThunderScripture) || activeRelics.Contains("雷心"))
            {
                return DemoSwordStyle.Thunder;
            }

            if (HasGongfa(DemoGongfaType.BloodFiendCanon) || activeRelics.Contains("血剑胚"))
            {
                return DemoSwordStyle.Blood;
            }

            return DemoSwordStyle.Wanjian;
        }

        private DemoBattlePresentationStep ResolveBossTurn()
        {
            BossIntentText = GetBossIntentText();

            switch (BossPhase)
            {
                case DemoBossPhase.ThunderCloud:
                {
                    int damage = 12 + Math.Min(6, Round);
                    int dealt = ApplyIncomingDamage(damage, "雷云压境", true, out int gourdGain);
                    Player.Shock += 1;
                    Log.Add($"雷云压境，天劫化身造成 {dealt} 伤害并施加 1 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("雷云压境", dealt, true);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 1;
                    if (gourdGain > 0)
                    {
                        step.Label = "雷云压境 / 葫芦收煞";
                    }
                    return step;
                }
                case DemoBossPhase.SoulLock:
                {
                    int damage = 14 + Math.Min(7, Round) + (Player.Shock >= 2 ? 3 : 0);
                    int dealt = ApplyIncomingDamage(damage, "天雷锁魂", true, out int gourdGain);
                    Player.Shock += 2;
                    Log.Add($"天雷锁魂，造成 {dealt} 伤害并施加 2 感电。");
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天雷锁魂", dealt, true);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 2;
                    if (gourdGain > 0)
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
                        Log.Add("天劫化身开始蓄雷，下一轮将降下重击。");
                        BossIntentText = GetBossIntentText();
                        return DemoBattlePresentationStep.Charge("天劫蓄雷", 1);
                    }

                    int damage = 22 + Math.Min(8, Round) + Player.Shock;
                    int dealt = ApplyIncomingDamage(damage, "天劫降临", true, out int gourdGain);
                    Player.Shock += 1;
                    calamityCharged = false;
                    Log.Add($"天劫降临，造成 {dealt} 伤害并施加 1 感电。");
                    BossIntentText = GetBossIntentText();
                    DemoBattlePresentationStep step = DemoBattlePresentationStep.Enemy("天劫降临", dealt, true);
                    step.TriggerShock = true;
                    step.PlayerShockDelta = 1;
                    step.HeavyImpact = true;
                    if (gourdGain > 0)
                    {
                        step.Label = "天劫降临 / 葫芦收煞";
                    }
                    return step;
                }
                default:
                    return DemoBattlePresentationStep.Enemy(Enemy.Name, 0, true);
            }
        }

        private void TryAddBossPhaseShift()
        {
            if (!isBossBattle || Enemy.IsDead)
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
            BossIntentText = GetBossIntentText();
            string label = BossPhase == DemoBossPhase.SoulLock ? "雷云翻涌，天雷锁魂" : "劫云尽开，天劫降临";
            Log.Add(label);
            CurrentPresentationSteps.Add(DemoBattlePresentationStep.PhaseShift(label));
        }

        private DemoBossPhase GetBossPhaseForHealth(int health, int maxHealth)
        {
            float ratio = maxHealth <= 0 ? 0f : (float)health / maxHealth;
            if (ratio <= 0.35f)
            {
                return DemoBossPhase.CalamityDescends;
            }

            if (ratio <= 0.7f)
            {
                return DemoBossPhase.SoulLock;
            }

            return DemoBossPhase.ThunderCloud;
        }

        private string GetBossIntentText()
        {
            switch (BossPhase)
            {
                case DemoBossPhase.ThunderCloud:
                    return "雷云压境：稳定压血，轻度感电。";
                case DemoBossPhase.SoulLock:
                    return "天雷锁魂：高压伤害，并压制下回合灵气。";
                case DemoBossPhase.CalamityDescends:
                    return calamityCharged
                        ? "天劫降临：下一击极重，适合爆发斩杀。"
                        : "天劫蓄雷：本轮蓄势，下一轮重击。";
                default:
                    return string.Empty;
            }
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

        private int ApplyIncomingDamage(int amount, string source, bool bossAttack, out int gourdEnergyGain)
        {
            gourdEnergyGain = 0;

            if (HasArtifact(DemoArtifactType.PurpleGourd))
            {
                int absorbed = Math.Min(4, Math.Max(0, amount - 1));
                if (absorbed > 0)
                {
                    amount -= absorbed;
                    gourdEnergyGain = 1;
                    storedGourdEnergy = Math.Min(2, storedGourdEnergy + 1);
                    Log.Add($"紫金葫芦吸收了 {absorbed} 点{source}伤害，下回合额外灵气 +1。");
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

        private int GetCardCost(DemoCard card)
        {
            if (card == null)
            {
                return 0;
            }

            if (spiritTalismanAvailable && card.Cost == 1)
            {
                spiritTalismanAvailable = false;
                return 0;
            }

            return card.Cost;
        }

        private int ApplyBloodOrbBonus(int damage, string sourceLabel)
        {
            if (damage <= 0 || !HasRelic("血魔珠"))
            {
                return damage;
            }

            int missingPercent = Math.Max(0, 100 - (Player.Health * 100 / Math.Max(1, Player.MaxHealth)));
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

        private void DrawCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (DrawPile.Count == 0)
                {
                    if (DiscardPile.Count == 0)
                    {
                        return;
                    }

                    DrawPile.AddRange(DiscardPile);
                    DiscardPile.Clear();
                    Shuffle(DrawPile);
                    Log.Add("弃牌堆洗回抽牌堆。");
                }

                DemoCard card = DrawPile[0];
                DrawPile.RemoveAt(0);
                Hand.Add(card);
            }
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
    }
}
