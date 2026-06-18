using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoRunState
    {
        public DemoMapRun Map { get; } = new DemoMapRun();
        public List<DemoCard> Deck { get; } = DemoCardLibrary.CreateStarterDeck();
        public int MaxHealth = 72;
        public int CurrentHealth = 72;
        public int BonusEnergy;
        public int BonusPermanentSwords;
        public readonly List<string> Relics = new List<string>();
        public readonly List<DemoArtifactType> Artifacts = new List<DemoArtifactType>();
        public DemoGongfaType MainGongfa = DemoGongfaType.None;
        public DemoGongfaType SupportGongfa = DemoGongfaType.None;
        public DemoGongfaType DivineGongfa = DemoGongfaType.None;

        public void AddCard(DemoCard card)
        {
            if (card == null)
            {
                return;
            }

            Deck.Add(card.Clone());
        }

        public void AddRelic(string relicName)
        {
            if (!HasRelic(relicName))
            {
                Relics.Add(relicName);
            }

            if (relicName == "剑骨")
            {
                BonusPermanentSwords += 1;
            }
        }

        public bool HasRelic(string relicName)
        {
            return Relics.Contains(relicName);
        }

        public bool HasArtifact(DemoArtifactType type)
        {
            return Artifacts.Contains(type);
        }

        public void AddArtifact(DemoArtifactType type)
        {
            if (!Artifacts.Contains(type))
            {
                Artifacts.Add(type);
            }
        }

        public void Heal(int amount)
        {
            CurrentHealth = System.Math.Min(MaxHealth, CurrentHealth + amount);
        }

        public void UpgradeEnergy()
        {
            BonusEnergy += 1;
        }

        public bool HasGongfa(DemoGongfaType type)
        {
            return MainGongfa == type || SupportGongfa == type || DivineGongfa == type;
        }

        public void LearnGongfa(DemoGongfaType type)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(type);
            switch (definition.Slot)
            {
                case DemoGongfaSlot.Main:
                    MainGongfa = type;
                    Deck.Clear();
                    Deck.AddRange(DemoCardLibrary.CreateStarterDeck(definition.Style));
                    break;
                case DemoGongfaSlot.Support:
                    SupportGongfa = type;
                    break;
                case DemoGongfaSlot.Divine:
                    DivineGongfa = type;
                    break;
            }
        }

        public DemoSwordStyle GetBuildStyle()
        {
            if (MainGongfa != DemoGongfaType.None)
            {
                return DemoGongfaLibrary.Get(MainGongfa).Style;
            }

            int wanjian = Deck.Count(card => card.Style == DemoSwordStyle.Wanjian);
            int thunder = Deck.Count(card => card.Style == DemoSwordStyle.Thunder);
            int blood = Deck.Count(card => card.Style == DemoSwordStyle.Blood);

            if (wanjian >= thunder && wanjian >= blood)
            {
                return DemoSwordStyle.Wanjian;
            }

            if (thunder >= blood)
            {
                return DemoSwordStyle.Thunder;
            }

            return DemoSwordStyle.Blood;
        }

        public IEnumerable<DemoGongfaType> GetLearnedGongfas()
        {
            if (MainGongfa != DemoGongfaType.None)
            {
                yield return MainGongfa;
            }

            if (SupportGongfa != DemoGongfaType.None)
            {
                yield return SupportGongfa;
            }

            if (DivineGongfa != DemoGongfaType.None)
            {
                yield return DivineGongfa;
            }
        }
    }
}
