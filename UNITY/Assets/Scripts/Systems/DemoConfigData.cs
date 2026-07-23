using System.Collections.Generic;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Map;

namespace PathOfTenThousandWays.Demo.Systems
{
    public sealed class DemoGameConfig
    {
        public List<DemoSystemConstantEntry> SystemConstants { get; set; } = new List<DemoSystemConstantEntry>();
        public List<DemoStyleConfig> Styles { get; set; } = new List<DemoStyleConfig>();
        public DemoOpeningConfig Opening { get; set; } = new DemoOpeningConfig();
        public DemoRuntimeConfig Demo { get; set; } = new DemoRuntimeConfig();
    }

    public sealed class DemoOpeningConfig
    {
        public Dictionary<string, DemoRootConfig> Roots { get; set; } = new Dictionary<string, DemoRootConfig>();
        public Dictionary<string, DemoTraceConfig> Traces { get; set; } = new Dictionary<string, DemoTraceConfig>();
        public Dictionary<string, DemoRegionConfig> Regions { get; set; } = new Dictionary<string, DemoRegionConfig>();
        public Dictionary<string, DemoJourneyVesselConfig> JourneyVessels { get; set; } = new Dictionary<string, DemoJourneyVesselConfig>();
        public Dictionary<string, DemoJourneyLineConfig> JourneyLines { get; set; } = new Dictionary<string, DemoJourneyLineConfig>();
    }

    public sealed class DemoSystemConstantEntry
    {
        public string Group { get; set; }
        public string Key { get; set; }
        public object Value { get; set; }
        public string ValueType { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DemoStyleConfig
    {
        public string StyleId { get; set; }
        public string Name { get; set; }
        public string FocusText { get; set; }
        public string BasicPathCardId { get; set; }
        public string ThemeTags { get; set; }
    }

    public sealed class DemoRuntimeConfig
    {
        public Dictionary<string, DemoCardConfig> Cards { get; set; } = new Dictionary<string, DemoCardConfig>();
        public Dictionary<string, List<DemoCardPoolEntryConfig>> CardPools { get; set; } = new Dictionary<string, List<DemoCardPoolEntryConfig>>();
        public Dictionary<string, DemoCorePracticeConfig> CorePractices { get; set; } = new Dictionary<string, DemoCorePracticeConfig>();
        public Dictionary<string, DemoTechniqueConfig> Techniques { get; set; } = new Dictionary<string, DemoTechniqueConfig>();
        public Dictionary<string, DemoBearerConfig> Bearers { get; set; } = new Dictionary<string, DemoBearerConfig>();
        public Dictionary<string, DemoStartingPracticePackageConfig> StartingPracticePackages { get; set; } = new Dictionary<string, DemoStartingPracticePackageConfig>();
        public Dictionary<string, DemoInnateArtifactConfig> InnateArtifacts { get; set; } = new Dictionary<string, DemoInnateArtifactConfig>();
        public Dictionary<string, DemoMindMethodConfig> MindMethods { get; set; } = new Dictionary<string, DemoMindMethodConfig>();
        public Dictionary<string, DemoRealmBreakthroughConfig> RealmBreakthroughs { get; set; } = new Dictionary<string, DemoRealmBreakthroughConfig>();
        public Dictionary<string, DemoFoundationRuleConfig> FoundationRules { get; set; } = new Dictionary<string, DemoFoundationRuleConfig>();
        public Dictionary<string, DemoEncounterGroupConfig> EncounterGroups { get; set; } = new Dictionary<string, DemoEncounterGroupConfig>();
        public Dictionary<string, DemoMapTemplateConfig> MapTemplates { get; set; } = new Dictionary<string, DemoMapTemplateConfig>();
        public Dictionary<string, DemoEventConfig> Events { get; set; } = new Dictionary<string, DemoEventConfig>();
        public Dictionary<string, DemoStoryFlagConfig> StoryFlags { get; set; } = new Dictionary<string, DemoStoryFlagConfig>();
        public Dictionary<string, DemoGongfaConfig> Gongfas { get; set; } = new Dictionary<string, DemoGongfaConfig>();
        public Dictionary<string, DemoArtifactConfig> Artifacts { get; set; } = new Dictionary<string, DemoArtifactConfig>();
        public Dictionary<string, DemoRelicConfig> Relics { get; set; } = new Dictionary<string, DemoRelicConfig>();
        public Dictionary<string, DemoRoutePlanConfig> RoutePlans { get; set; } = new Dictionary<string, DemoRoutePlanConfig>();
        public Dictionary<string, DemoRewardProfileConfig> RewardProfiles { get; set; } = new Dictionary<string, DemoRewardProfileConfig>();
        public Dictionary<string, DemoNodeActionProfileConfig> NodeActionProfiles { get; set; } = new Dictionary<string, DemoNodeActionProfileConfig>();
        public Dictionary<string, DemoEnemyConfig> Enemies { get; set; } = new Dictionary<string, DemoEnemyConfig>();
        public Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>> RewardPriorities { get; set; }
            = new Dictionary<string, Dictionary<string, List<DemoRewardPriorityEntryConfig>>>();
    }

    public sealed class DemoRootConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Rarity { get; set; }
        public string UnlockCondition { get; set; }
        public bool IsDefaultPool { get; set; }
        public bool IsAvailable { get; set; }
        public string Summary { get; set; }
    }

    public sealed class DemoTraceConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TraceType { get; set; }
        public string Summary { get; set; }
    }

    public sealed class DemoRegionConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RewardFocus { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }
        public List<DemoRegionNodeWeightConfig> NodeWeights { get; set; } = new List<DemoRegionNodeWeightConfig>();
    }

    public sealed class DemoRegionNodeWeightConfig
    {
        public string NodeType { get; set; }
        public int Weight { get; set; }
    }

    public sealed class DemoJourneyLineConfig
    {
        public string Id { get; set; }
        public string RootId { get; set; }
        public string Title { get; set; }
        public string OriginText { get; set; }
        public string CarryItemName { get; set; }
        public string CarryItemEffect { get; set; }
        public string VesselType { get; set; }
        public string StarterPoolId { get; set; }
        public string StartingPracticePackageId { get; set; }
        public string BaseStyle { get; set; }
        public bool IsAvailable { get; set; }
        public string FirstRegionId { get; set; }
        public List<string> RegionCandidateIds { get; set; } = new List<string>();
        public string RiskLevel { get; set; }
        public List<string> SummaryTags { get; set; } = new List<string>();
        public List<DemoJourneyNodeBiasConfig> NodeBiases { get; set; } = new List<DemoJourneyNodeBiasConfig>();
        public List<DemoJourneyRewardBiasConfig> RewardBiases { get; set; } = new List<DemoJourneyRewardBiasConfig>();
    }

    public sealed class DemoJourneyVesselConfig
    {
        public string Id { get; set; }
        public string RootId { get; set; }
        public string Name { get; set; }
        public string OriginText { get; set; }
        public string VesselType { get; set; }
        public string StarterPoolId { get; set; }
        public string StartingPracticePackageId { get; set; }
        public string BaseStyle { get; set; }
        public string StartingEffectText { get; set; }
        public string FirstRegionId { get; set; }
        public List<string> RegionCandidateIds { get; set; } = new List<string>();
        public string RiskLevel { get; set; }
        public List<string> SummaryTags { get; set; } = new List<string>();
        public bool IsAvailable { get; set; }
    }

    public sealed class DemoJourneyNodeBiasConfig
    {
        public string NodeType { get; set; }
        public int DeltaPercent { get; set; }
    }

    public sealed class DemoJourneyRewardBiasConfig
    {
        public string TagId { get; set; }
        public int DeltaPercent { get; set; }
        public string Priority { get; set; }
    }

    public sealed class DemoCardConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconGlyph { get; set; }
        public string Type { get; set; }
        public string Style { get; set; }
        public string Quality { get; set; }
        public int Cost { get; set; }
        public int Damage { get; set; }
        public int Block { get; set; }
        public int Draw { get; set; }
        public int EnergyGain { get; set; }
        public int SwordIntent { get; set; }
        public int Shock { get; set; }
        public int Bleed { get; set; }
        public int TemporarySwords { get; set; }
        public bool PermanentSword { get; set; }
        public bool ConsumeAllSwordIntent { get; set; }
        public int SelfDamage { get; set; }
        public string SpecialEffect { get; set; }
        public string RulesOverride { get; set; }
    }

    public sealed class DemoCardPoolEntryConfig
    {
        public string EntryType { get; set; }
        public string RefId { get; set; }
        public int Count { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DemoCorePracticeConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string PracticeType { get; set; }
        public string PassiveRuleText { get; set; }
        public string GrantedTechniqueId { get; set; }
        public string SourceStoryId { get; set; }
    }

    public sealed class DemoTechniqueConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Kind { get; set; }
        public string SourceStoryId { get; set; }
        public string RulesText { get; set; }
        public string VisualTag { get; set; }
    }

    public sealed class DemoBearerConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Mode { get; set; }
        public string ResourceKey { get; set; }
        public bool IsRequired { get; set; }
    }

    public sealed class DemoStartingPracticePackageConfig
    {
        public string Id { get; set; }
        public string RootId { get; set; }
        public string SourceStoryId { get; set; }
        public string InnateArtifactId { get; set; }
        public string CorePracticeId { get; set; }
        public string PrimaryTechniqueId { get; set; }
        public string BearerDefinitionId { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> ActiveTechniqueIds { get; set; } = new List<string>();
    }

    public sealed class DemoInnateArtifactConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BearerDefinitionId { get; set; }
        public string VisualResourceKey { get; set; }
        public float BaseCooldown { get; set; }
        public List<DemoInnateArtifactStageConfig> Stages { get; set; } = new List<DemoInnateArtifactStageConfig>();
    }

    public sealed class DemoInnateArtifactStageConfig
    {
        public int Stage { get; set; }
        public string Name { get; set; }
        public int BaseDamage { get; set; }
        public float Cooldown { get; set; }
        public string AttackVariantId { get; set; }
        public string RulesText { get; set; }
    }

    public sealed class DemoMindMethodConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string GrantedTechniqueId { get; set; }
        public List<DemoMindMethodLevelConfig> Levels { get; set; } = new List<DemoMindMethodLevelConfig>();
    }

    public sealed class DemoMindMethodLevelConfig
    {
        public int Level { get; set; }
        public float EnergyRegenMultiplier { get; set; }
        public int RecoveryAmount { get; set; }
        public float ArtifactCooldownMultiplier { get; set; }
        public string RulesText { get; set; }
    }

    public sealed class DemoRealmBreakthroughConfig
    {
        public string Id { get; set; }
        public string FromRealmId { get; set; }
        public string ToRealmId { get; set; }
        public int MaxEnergy { get; set; }
        public float EnergyRegenMultiplier { get; set; }
        public float TechniquePowerMultiplier { get; set; }
        public string RequiredNodeType { get; set; }
        public string SceneId { get; set; }
    }

    public sealed class DemoFoundationRuleConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RequiredStoryFlagId { get; set; }
        public string RuleKind { get; set; }
        public double RuleValue { get; set; }
        public string RulesText { get; set; }
        public string VisualTag { get; set; }
    }

    public sealed class DemoEncounterGroupConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int ActIndex { get; set; }
        public string EncounterRole { get; set; }
        public List<DemoEncounterGroupMemberConfig> Members { get; set; } = new List<DemoEncounterGroupMemberConfig>();
    }

    public sealed class DemoEncounterGroupMemberConfig
    {
        public int Slot { get; set; }
        public string EnemyId { get; set; }
        public string PositionId { get; set; }
        public int ThreatPriority { get; set; }
        public bool RequiredForVictory { get; set; }
    }

    public sealed class DemoMapTemplateConfig
    {
        public string Id { get; set; }
        public string RegionId { get; set; }
        public int ActCount { get; set; }
        public int StandardDepthCount { get; set; }
        public List<DemoMapTemplateNodeConfig> Nodes { get; set; } = new List<DemoMapTemplateNodeConfig>();
        public List<DemoMapTemplateEdgeConfig> Edges { get; set; } = new List<DemoMapTemplateEdgeConfig>();
    }

    public sealed class DemoMapTemplateNodeConfig
    {
        public string SlotId { get; set; }
        public int ActIndex { get; set; }
        public int DepthIndex { get; set; }
        public int LaneIndex { get; set; }
        public string AllowedNodeTypes { get; set; }
        public string RequiredContentId { get; set; }
        public string RequiredStoryFlagId { get; set; }
        public bool Hidden { get; set; }
    }

    public sealed class DemoMapTemplateEdgeConfig
    {
        public string FromSlotId { get; set; }
        public string ToSlotId { get; set; }
    }

    public sealed class DemoEventConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string SceneId { get; set; }
        public int ActIndex { get; set; }
        public bool Critical { get; set; }
        public string RequiredStoryFlagId { get; set; }
        public List<DemoEventChoiceConfig> Choices { get; set; } = new List<DemoEventChoiceConfig>();
    }

    public sealed class DemoEventChoiceConfig
    {
        public string ChoiceId { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string RequiredStoryFlagId { get; set; }
        public string GrantedStoryFlagId { get; set; }
        public string EffectKind { get; set; }
        public string EffectValue { get; set; }
    }

    public sealed class DemoStoryFlagConfig
    {
        public string Id { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public bool PersistsBetweenRuns { get; set; }
    }

    public sealed class DemoGongfaConfig
    {
        public string Id { get; set; }
        public string RuntimeEnum { get; set; }
        public string Slot { get; set; }
        public string Style { get; set; }
        public string Name { get; set; }
        public string IconGlyph { get; set; }
        public string Title { get; set; }
        public string Quality { get; set; }
        public string Description { get; set; }
    }

    public sealed class DemoArtifactConfig
    {
        public string Id { get; set; }
        public string RuntimeEnum { get; set; }
        public string Name { get; set; }
        public string IconGlyph { get; set; }
        public string Style { get; set; }
        public string Quality { get; set; }
        public string Description { get; set; }
    }

    public sealed class DemoRelicConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconGlyph { get; set; }
        public string Style { get; set; }
        public string Quality { get; set; }
        public string Description { get; set; }
    }

    public sealed class DemoRoutePlanConfig
    {
        public string Id { get; set; }
        public int Layer { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RouteStyle { get; set; }
        public string RouteQuality { get; set; }
        public string RouteGlyph { get; set; }
        public string RouteTag { get; set; }
        public List<DemoRoutePlanNodeConfig> Nodes { get; set; } = new List<DemoRoutePlanNodeConfig>();
    }

    public sealed class DemoRoutePlanNodeConfig
    {
        public int Seq { get; set; }
        public int Layer { get; set; }
        public string NodeType { get; set; }
        public string NodeName { get; set; }
        public string NodeId { get; set; }
        public string EncounterId { get; set; }
        public string RewardProfileId { get; set; }
        public string ActionProfileId { get; set; }
    }

    public sealed class DemoRewardProfileConfig
    {
        public string Id { get; set; }
        public string Tier { get; set; }
        public string Source { get; set; }
        public string RouteRisk { get; set; }
        public bool AllowsFinisher { get; set; }
        public bool AllowsDivine { get; set; }
        public string Description { get; set; }
    }

    public sealed class DemoNodeActionProfileConfig
    {
        public string Id { get; set; }
        public string ActionType { get; set; }
        public string RewardProfileId { get; set; }
        public string GuaranteedComponentId { get; set; }
        public int HealAmount { get; set; }
        public string Description { get; set; }
    }

    public sealed class DemoRewardPriorityEntryConfig
    {
        public string PriorityGroup { get; set; }
        public int Seq { get; set; }
        public string RefType { get; set; }
        public string RefId { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DemoEnemyConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BattleRole { get; set; }
        public bool IsBoss { get; set; }
        public int MaxHealth { get; set; }
        public string BaseDamageProfile { get; set; }
        public string Notes { get; set; }
        public List<DemoBossPhaseConfig> BossPhases { get; set; } = new List<DemoBossPhaseConfig>();
    }

    public sealed class DemoBossPhaseConfig
    {
        public string PhaseId { get; set; }
        public int PhaseOrder { get; set; }
        public string Name { get; set; }
        public double HealthRatioMax { get; set; }
        public string IntentText { get; set; }
        public bool ChargeTurn { get; set; }
        public int BaseDamage { get; set; }
        public int ShockApply { get; set; }
        public string Notes { get; set; }
    }

    public sealed class DemoRoutePlanDefinition
    {
        public string Id;
        public DemoMapRoutePlan Plan;
        public DemoSwordStyle RouteStyle;
        public DemoQuality RouteQuality;
        public string RouteGlyph;
        public string RouteTag;
    }

    public sealed class DemoEnemyDefinition
    {
        public string Id;
        public string Name;
        public string BattleRole;
        public bool IsBoss;
        public int MaxHealth;
        public string BaseDamageProfile;
        public string Notes;
        public List<DemoBossPhaseConfig> BossPhases = new List<DemoBossPhaseConfig>();
    }

    public sealed class DemoRootDefinition
    {
        public string Id;
        public string Name;
        public string Rarity;
        public string UnlockCondition;
        public bool IsDefaultPool;
        public bool IsAvailable;
        public string Summary;
    }

    public sealed class DemoTraceDefinition
    {
        public string Id;
        public string Name;
        public string TraceType;
        public string Summary;
    }

    public sealed class DemoRegionDefinition
    {
        public string Id;
        public string Name;
        public string RewardFocus;
        public string Description;
        public bool IsAvailable;
        public Dictionary<string, int> NodeWeights = new Dictionary<string, int>();
    }

    public sealed class DemoJourneyVesselDefinition
    {
        public string Id;
        public string RootId;
        public string Name;
        public string OriginText;
        public string VesselType;
        public string StarterPoolId;
        public string StartingPracticePackageId;
        public string BaseStyle;
        public string StartingEffectText;
        public string FirstRegionId;
        public List<string> RegionCandidateIds = new List<string>();
        public string RiskLevel;
        public List<string> SummaryTags = new List<string>();
        public bool IsAvailable;
    }

    public sealed class DemoJourneyLineDefinition
    {
        public string Id;
        public string RootId;
        public string Title;
        public string OriginText;
        public string CarryItemName;
        public string CarryItemEffect;
        public string VesselType;
        public string StarterPoolId;
        public string StartingPracticePackageId;
        public string BaseStyle;
        public bool IsAvailable;
        public string FirstRegionId;
        public List<string> RegionCandidateIds = new List<string>();
        public string RiskLevel;
        public List<string> SummaryTags = new List<string>();
        public List<DemoJourneyNodeBiasConfig> NodeBiases = new List<DemoJourneyNodeBiasConfig>();
        public List<DemoJourneyRewardBiasConfig> RewardBiases = new List<DemoJourneyRewardBiasConfig>();
    }

    public sealed class DemoRewardProfileDefinition
    {
        public string Id;
        public string Tier;
        public string Source;
        public string RouteRisk;
        public bool AllowsFinisher;
        public bool AllowsDivine;
        public string Description;
    }

    public sealed class DemoNodeActionProfileDefinition
    {
        public string Id;
        public string ActionType;
        public string RewardProfileId;
        public string GuaranteedComponentId;
        public int HealAmount;
        public string Description;
    }
}
