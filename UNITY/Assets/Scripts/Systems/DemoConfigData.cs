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
        public Dictionary<string, DemoRegionConfig> Regions { get; set; } = new Dictionary<string, DemoRegionConfig>();
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
        public Dictionary<string, DemoGongfaConfig> Gongfas { get; set; } = new Dictionary<string, DemoGongfaConfig>();
        public Dictionary<string, DemoArtifactConfig> Artifacts { get; set; } = new Dictionary<string, DemoArtifactConfig>();
        public Dictionary<string, DemoRelicConfig> Relics { get; set; } = new Dictionary<string, DemoRelicConfig>();
        public Dictionary<string, DemoRoutePlanConfig> RoutePlans { get; set; } = new Dictionary<string, DemoRoutePlanConfig>();
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
        public string Summary { get; set; }
    }

    public sealed class DemoRegionConfig
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RewardFocus { get; set; }
        public string Description { get; set; }
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
        public string FirstRegionId { get; set; }
        public string RiskLevel { get; set; }
        public List<string> SummaryTags { get; set; } = new List<string>();
        public List<DemoJourneyNodeBiasConfig> NodeBiases { get; set; } = new List<DemoJourneyNodeBiasConfig>();
        public List<DemoJourneyRewardBiasConfig> RewardBiases { get; set; } = new List<DemoJourneyRewardBiasConfig>();
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
        public string Summary;
    }

    public sealed class DemoRegionDefinition
    {
        public string Id;
        public string Name;
        public string RewardFocus;
        public string Description;
        public Dictionary<string, int> NodeWeights = new Dictionary<string, int>();
    }

    public sealed class DemoJourneyLineDefinition
    {
        public string Id;
        public string RootId;
        public string Title;
        public string OriginText;
        public string CarryItemName;
        public string CarryItemEffect;
        public string FirstRegionId;
        public string RiskLevel;
        public List<string> SummaryTags = new List<string>();
        public List<DemoJourneyNodeBiasConfig> NodeBiases = new List<DemoJourneyNodeBiasConfig>();
        public List<DemoJourneyRewardBiasConfig> RewardBiases = new List<DemoJourneyRewardBiasConfig>();
    }
}
