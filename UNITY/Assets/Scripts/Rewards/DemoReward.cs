using System;
using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public enum DemoRewardType
    {
        Root,
        Journey,
        OpeningScene,
        Route,
        Card,
        Gongfa,
        Artifact,
        Relic,
        Upgrade,
        Heal,
        Vessel,
        Trace
    }

    public enum DemoRewardSource
    {
        Generic,
        OpeningBattle,
        NormalBattle,
        EliteBattle,
        Training,
        Preparation,
        Boss
    }

    public enum DemoRewardTier
    {
        Opening,
        Standard,
        Elite,
        Build,
        Core,
        Finisher,
        High,
        Boss
    }

    public enum DemoRouteRisk
    {
        Stable,
        Risky,
        Build
    }

    public enum DemoRewardSlot
    {
        None,
        Focus,
        Utility,
        Wildcard
    }

    public sealed class DemoRewardContext
    {
        public int Layer;
        public DemoRewardSource Source = DemoRewardSource.Generic;
        public DemoRewardTier Tier = DemoRewardTier.Standard;
        public DemoRouteRisk RouteRisk = DemoRouteRisk.Stable;
        public string RewardProfileId;
        public HashSet<string> ExistingComponentIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        public int CurrentHealth;
        public int MaxHealth;
        public int ConsecutiveRewardsWithoutFocus;
        public bool AllowsFinisher;
        public bool AllowsDivine;
        public bool HasSeed;
        public int Seed;

        public bool HasComponent(string componentId)
        {
            return !string.IsNullOrEmpty(componentId) && ExistingComponentIds.Contains(componentId);
        }

        public static DemoRewardContext FromNode(DemoMapNode node, DemoRunState run, int? seed = null)
        {
            DemoRewardContext context = new DemoRewardContext
            {
                Layer = node?.Layer ?? 1,
                RewardProfileId = node?.RewardProfileId,
                CurrentHealth = run?.CurrentHealth ?? 0,
                MaxHealth = run?.MaxHealth ?? 0,
                ConsecutiveRewardsWithoutFocus = run?.ConsecutiveRewardsWithoutFocus ?? 0,
                ExistingComponentIds = run?.GetBuildComponentIds()
                    ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase),
                HasSeed = seed.HasValue,
                Seed = seed ?? 0
            };

            if (string.IsNullOrEmpty(context.RewardProfileId)
                && !string.IsNullOrEmpty(node?.ActionProfileId)
                && DemoConfigRepository.TryGetNodeActionProfile(node.ActionProfileId, out DemoNodeActionProfileDefinition actionProfile))
            {
                context.RewardProfileId = actionProfile.RewardProfileId;
            }

            if (!string.IsNullOrEmpty(context.RewardProfileId)
                && DemoConfigRepository.TryGetRewardProfile(context.RewardProfileId, out DemoRewardProfileDefinition profile))
            {
                context.Source = ParseSource(profile.Source);
                context.Tier = ParseTier(profile.Tier);
                context.RouteRisk = ParseRisk(profile.RouteRisk);
                context.AllowsFinisher = profile.AllowsFinisher;
                context.AllowsDivine = profile.AllowsDivine;
            }

            return context;
        }

        private static DemoRewardSource ParseSource(string value)
        {
            switch ((value ?? string.Empty).Trim().ToLowerInvariant())
            {
                case "opening_battle":
                    return DemoRewardSource.OpeningBattle;
                case "normal_battle":
                    return DemoRewardSource.NormalBattle;
                case "elite_battle":
                    return DemoRewardSource.EliteBattle;
                case "training":
                    return DemoRewardSource.Training;
                case "preparation":
                    return DemoRewardSource.Preparation;
                case "boss":
                    return DemoRewardSource.Boss;
                default:
                    return DemoRewardSource.Generic;
            }
        }

        private static DemoRewardTier ParseTier(string value)
        {
            return Enum.TryParse(value, true, out DemoRewardTier parsed)
                ? parsed
                : DemoRewardTier.Standard;
        }

        private static DemoRouteRisk ParseRisk(string value)
        {
            return Enum.TryParse(value, true, out DemoRouteRisk parsed)
                ? parsed
                : DemoRouteRisk.Stable;
        }
    }

    [Serializable]
    public sealed class DemoReward
    {
        public DemoRewardType Type;
        public DemoRewardSlot Slot;
        public string Name;
        public string Description;
        public string Summary;
        public string BuildDelta;
        public DemoCard Card;
        public DemoGongfaType GongfaType;
        public DemoArtifactType ArtifactType;
        public DemoMapRoutePlan RoutePlan;
        public DemoSwordStyle RouteStyle;
        public DemoQuality RouteQuality;
        public string RouteGlyph;
        public string RouteTag;
        public DemoRootDefinition Root;
        public DemoJourneyVesselDefinition Vessel;
        public DemoJourneyLineDefinition JourneyLine;
        public DemoRegionDefinition Region;
        public string TraceId;

        public DemoReward WithSlot(DemoRewardSlot slot, string buildDelta = null)
        {
            Slot = slot;
            BuildDelta = buildDelta;
            return this;
        }

        public static DemoReward Route(DemoMapRoutePlan routePlan, DemoSwordStyle style, DemoQuality quality, string glyph, string tag)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Route,
                Name = routePlan.Name,
                Description = routePlan.Description,
                RoutePlan = routePlan,
                RouteStyle = style,
                RouteQuality = quality,
                RouteGlyph = glyph,
                RouteTag = tag
            };
        }

        public static DemoReward FromRoot(DemoRootDefinition root)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Root,
                Name = root.Name,
                Description = root.Summary,
                Summary = root.IsAvailable ? root.UnlockCondition : "命数未显",
                Root = root
            };
        }

        public static DemoReward FromVessel(DemoJourneyVesselDefinition vessel, DemoRootDefinition root)
        {
            string summary = string.IsNullOrEmpty(vessel.StartingEffectText)
                ? $"所携：{vessel.Name}"
                : $"所携：{vessel.Name} | {vessel.StartingEffectText}";

            return new DemoReward
            {
                Type = DemoRewardType.Vessel,
                Name = vessel.Name,
                Description = vessel.OriginText,
                Summary = vessel.IsAvailable
                    ? $"{(root == null ? "未知根脚" : root.Name)} | {summary}"
                    : "此器物尚未显化",
                Root = root,
                Vessel = vessel,
                JourneyLine = ToLegacyJourneyLine(vessel)
            };
        }

        public static DemoReward Journey(DemoJourneyLineDefinition line, DemoRootDefinition root)
        {
            string summary = string.IsNullOrEmpty(line.CarryItemEffect)
                ? $"所携：{line.CarryItemName}"
                : $"所携：{line.CarryItemName} | {line.CarryItemEffect}";

            return new DemoReward
            {
                Type = DemoRewardType.Journey,
                Name = line.Title,
                Description = line.OriginText,
                Summary = $"{(root == null ? "未知根脚" : root.Name)} | {summary}",
                Root = root,
                Vessel = ToJourneyVessel(line),
                JourneyLine = line
            };
        }

        public static DemoReward OpeningScene(DemoRegionDefinition region, DemoJourneyVesselDefinition vessel)
        {
            string carryText = vessel == null || string.IsNullOrEmpty(vessel.Name)
                ? "所携待定"
                : $"所携：{vessel.Name}";

            return new DemoReward
            {
                Type = DemoRewardType.OpeningScene,
                Name = region.Name,
                Description = region.Description,
                Summary = region.IsAvailable ? $"{carryText} | 所往候选" : $"{carryText} | 尚未开放",
                Vessel = vessel,
                JourneyLine = vessel == null ? null : ToLegacyJourneyLine(vessel),
                Region = region
            };
        }

        public static DemoReward OpeningScene(DemoRegionDefinition region, DemoJourneyLineDefinition line)
        {
            DemoJourneyVesselDefinition vessel = line == null ? null : ToJourneyVessel(line);
            DemoReward reward = OpeningScene(region, vessel);
            reward.JourneyLine = line;
            return reward;
        }

        public static DemoReward FromCard(DemoCard card)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Card,
                Name = card.Name,
                Description = card.GetRulesText(),
                Card = card
            };
        }

        public static DemoReward Heal()
        {
            int healAmount = DemoConfigRepository.GetIntConstant("battle", "heal_reward_amount", 18);
            return new DemoReward
            {
                Type = DemoRewardType.Heal,
                Name = "调息",
                Description = $"恢复 {healAmount} 点生命。"
            };
        }

        public static DemoReward Upgrade()
        {
            int energyGain = DemoConfigRepository.GetIntConstant("battle", "upgrade_reward_energy_bonus", 1);
            return new DemoReward
            {
                Type = DemoRewardType.Upgrade,
                Name = "剑诀精修",
                Description = $"最大灵气 +{energyGain}。"
            };
        }

        public static DemoReward Trace(string traceId, string name, string description)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Trace,
                TraceId = traceId,
                Name = name,
                Description = description
            };
        }

        public static DemoReward Relic(string name, string description)
        {
            return new DemoReward
            {
                Type = DemoRewardType.Relic,
                Name = name,
                Description = description
            };
        }

        public static DemoReward Relic(string relicName)
        {
            DemoRelicDefinition definition = DemoRelicLibrary.Get(relicName);
            return new DemoReward
            {
                Type = DemoRewardType.Relic,
                Name = definition.Name,
                Description = $"{definition.Style} | {definition.Description}"
            };
        }

        public static DemoReward Artifact(DemoArtifactType artifactType)
        {
            DemoArtifactDefinition definition = DemoArtifactLibrary.Get(artifactType);
            return new DemoReward
            {
                Type = DemoRewardType.Artifact,
                Name = definition.Name,
                Description = $"{definition.Style} | {definition.Description}",
                ArtifactType = artifactType
            };
        }

        public static DemoReward Gongfa(DemoGongfaType gongfaType)
        {
            DemoGongfaDefinition definition = DemoGongfaLibrary.Get(gongfaType);
            return new DemoReward
            {
                Type = DemoRewardType.Gongfa,
                Name = definition.Name,
                Description = $"{definition.Title} | {definition.Description}",
                GongfaType = gongfaType
            };
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
