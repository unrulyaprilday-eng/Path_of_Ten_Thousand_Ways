using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoRunState
    {
        public sealed class DemoOpeningSelection
        {
            public DemoRootDefinition Root;
            public DemoJourneyVesselDefinition Vessel;

            // Legacy mirror kept while the existing controller and UI migrate to Vessel.
            public DemoJourneyLineDefinition JourneyLine;
            public DemoRegionDefinition FirstRegion;
        }

        public DemoMapRun Map { get; } = new DemoMapRun();
        public List<DemoCard> Deck { get; } = new List<DemoCard>();
        public int MaxHealth;
        public int CurrentHealth;
        public int BonusEnergy;
        public int BonusPermanentSwords;
        public readonly List<string> Relics = new List<string>();
        public readonly List<DemoArtifactType> Artifacts = new List<DemoArtifactType>();
        public DemoGongfaType MainGongfa = DemoGongfaType.None;
        public DemoGongfaType SupportGongfa = DemoGongfaType.None;
        public DemoGongfaType DivineGongfa = DemoGongfaType.None;
        public DemoOpeningSelection OpeningSelection { get; } = new DemoOpeningSelection();

        public int BattlesWon;
        public int MaxSwordCount;
        public int HighestBurstDamage;
        public int ConsecutiveRewardsWithoutFocus;
        public int OpeningRewardRerolls;
        public string EquippedTraceId;

        public DemoRunState()
        {
            ResetForNewRun();
        }

        public void ResetForNewRun(
            string equippedTraceId = null,
            int openingRewardRerolls = 0)
        {
            Map.Reset();
            Deck.Clear();
            Relics.Clear();
            Artifacts.Clear();

            MaxHealth = DemoConfigRepository.GetIntConstant("battle", "player_base_max_health", 72);
            CurrentHealth = MaxHealth;
            BonusEnergy = 0;
            BonusPermanentSwords = 0;
            MainGongfa = DemoGongfaType.None;
            SupportGongfa = DemoGongfaType.None;
            DivineGongfa = DemoGongfaType.None;

            OpeningSelection.Root = null;
            OpeningSelection.Vessel = null;
            OpeningSelection.JourneyLine = null;
            OpeningSelection.FirstRegion = null;

            BattlesWon = 0;
            MaxSwordCount = 0;
            HighestBurstDamage = 0;
            ConsecutiveRewardsWithoutFocus = 0;
            EquippedTraceId = equippedTraceId;
            OpeningRewardRerolls = Math.Max(0, openingRewardRerolls);

            Deck.AddRange(DemoCardLibrary.CreateStarterDeck());
        }

        public void AddCard(DemoCard card)
        {
            if (card == null)
            {
                return;
            }

            Deck.Add(card.Clone());
        }

        public void SetRoot(DemoRootDefinition root)
        {
            OpeningSelection.Root = root;
            OpeningSelection.Vessel = null;
            OpeningSelection.JourneyLine = null;
            OpeningSelection.FirstRegion = null;
        }

        public void SetVessel(DemoJourneyVesselDefinition vessel)
        {
            OpeningSelection.Vessel = vessel;
            OpeningSelection.JourneyLine = vessel == null ? null : ToLegacyJourneyLine(vessel);
            OpeningSelection.FirstRegion = null;
            LoadVesselStarterDeck(vessel?.StarterPoolId);
        }

        public void SetJourneyLine(DemoJourneyLineDefinition line)
        {
            OpeningSelection.JourneyLine = line;
            OpeningSelection.FirstRegion = null;

            if (line != null && DemoConfigRepository.TryGetJourneyVessel(line.Id, out DemoJourneyVesselDefinition configured))
            {
                OpeningSelection.Vessel = configured;
            }
            else
            {
                OpeningSelection.Vessel = line == null ? null : ToJourneyVessel(line);
            }

            LoadVesselStarterDeck(line?.StarterPoolId);
        }

        public bool TrySetFirstRegion(DemoRegionDefinition region)
        {
            if (region == null || !region.IsAvailable)
            {
                return false;
            }

            OpeningSelection.FirstRegion = region;
            return true;
        }

        public void SetFirstRegion(DemoRegionDefinition region)
        {
            TrySetFirstRegion(region);
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
            CurrentHealth = Math.Min(MaxHealth, CurrentHealth + Math.Max(0, amount));
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

            string baseStyle = OpeningSelection.Vessel?.BaseStyle
                ?? OpeningSelection.JourneyLine?.BaseStyle;
            if (Enum.TryParse(baseStyle, true, out DemoSwordStyle vesselStyle))
            {
                return vesselStyle;
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

        public bool HasBuildComponent(string componentId)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                return false;
            }

            if (Deck.Any(card => string.Equals(card.Id, componentId, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            switch (componentId)
            {
                case "gongfa_sword_control_art":
                    return HasGongfa(DemoGongfaType.SwordControlArt);
                case "gongfa_wanjian_return":
                    return HasGongfa(DemoGongfaType.WanjianReturn);
                case "artifact_sword_box":
                    return HasArtifact(DemoArtifactType.SwordBox);
                default:
                    return false;
            }
        }

        public HashSet<string> GetBuildComponentIds()
        {
            HashSet<string> ids = new HashSet<string>(
                Deck.Where(card => card != null).Select(card => card.Id),
                StringComparer.OrdinalIgnoreCase);

            if (HasGongfa(DemoGongfaType.SwordControlArt))
            {
                ids.Add("gongfa_sword_control_art");
            }

            if (HasGongfa(DemoGongfaType.WanjianReturn))
            {
                ids.Add("gongfa_wanjian_return");
            }

            if (HasArtifact(DemoArtifactType.SwordBox))
            {
                ids.Add("artifact_sword_box");
            }

            return ids;
        }

        public void RecordBattleVictory(int swordCount, int burstDamage)
        {
            BattlesWon++;
            RecordSwordCount(swordCount);
            RecordBurstDamage(burstDamage);
        }

        public void RecordSwordCount(int swordCount)
        {
            MaxSwordCount = Math.Max(MaxSwordCount, swordCount);
        }

        public void RecordBurstDamage(int damage)
        {
            HighestBurstDamage = Math.Max(HighestBurstDamage, damage);
        }

        public void RecordRewardSelection(bool selectedFocusComponent)
        {
            ConsecutiveRewardsWithoutFocus = selectedFocusComponent
                ? 0
                : ConsecutiveRewardsWithoutFocus + 1;
        }

        public bool ConsumeOpeningRewardReroll()
        {
            if (OpeningRewardRerolls <= 0)
            {
                return false;
            }

            OpeningRewardRerolls--;
            return true;
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

        private void LoadVesselStarterDeck(string starterPoolId)
        {
            Deck.Clear();
            if (!string.IsNullOrEmpty(starterPoolId)
                && DemoConfigRepository.TryCreateDeckFromPool(starterPoolId, out List<DemoCard> configuredDeck))
            {
                Deck.AddRange(configuredDeck);
                return;
            }

            Deck.AddRange(DemoCardLibrary.CreateStarterDeck());
        }

        private static DemoJourneyVesselDefinition ToJourneyVessel(DemoJourneyLineDefinition line)
        {
            return new DemoJourneyVesselDefinition
            {
                Id = line.Id,
                RootId = line.RootId,
                Name = string.IsNullOrEmpty(line.CarryItemName) ? line.Title : line.CarryItemName,
                OriginText = line.OriginText,
                VesselType = line.VesselType,
                StarterPoolId = line.StarterPoolId,
                BaseStyle = line.BaseStyle,
                StartingEffectText = line.CarryItemEffect,
                FirstRegionId = line.FirstRegionId,
                RegionCandidateIds = line.RegionCandidateIds != null ? new List<string>(line.RegionCandidateIds) : new List<string>(),
                RiskLevel = line.RiskLevel,
                SummaryTags = line.SummaryTags != null ? new List<string>(line.SummaryTags) : new List<string>(),
                IsAvailable = line.IsAvailable
            };
        }

        private static DemoJourneyLineDefinition ToLegacyJourneyLine(DemoJourneyVesselDefinition vessel)
        {
            return new DemoJourneyLineDefinition
            {
                Id = vessel.Id,
                RootId = vessel.RootId,
                Title = vessel.Name,
                OriginText = vessel.OriginText,
                CarryItemName = vessel.Name,
                CarryItemEffect = vessel.StartingEffectText,
                VesselType = vessel.VesselType,
                StarterPoolId = vessel.StarterPoolId,
                BaseStyle = vessel.BaseStyle,
                IsAvailable = vessel.IsAvailable,
                FirstRegionId = vessel.FirstRegionId,
                RegionCandidateIds = vessel.RegionCandidateIds != null ? new List<string>(vessel.RegionCandidateIds) : new List<string>(),
                RiskLevel = vessel.RiskLevel,
                SummaryTags = vessel.SummaryTags != null ? new List<string>(vessel.SummaryTags) : new List<string>()
            };
        }
    }
}
