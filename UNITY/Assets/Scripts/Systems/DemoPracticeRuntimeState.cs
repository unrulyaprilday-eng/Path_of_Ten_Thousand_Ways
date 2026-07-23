using System;
using System.Collections.Generic;
using System.Linq;

namespace PathOfTenThousandWays.Demo.Systems
{
    public enum DemoJourneyChoiceKind
    {
        Continue,
        Story,
        Cultivation,
        Secret,
        Refinement,
        Breakthrough
    }

    [Serializable]
    public sealed class DemoJourneyChoice
    {
        public string ChoiceId { get; }
        public DemoJourneyChoiceKind Kind { get; }
        public string Title { get; }
        public string Description { get; }
        public string Consequence { get; }
        public string FoundationRuleId { get; }
        public bool IsRecommended { get; }

        public DemoJourneyChoice(
            string choiceId,
            DemoJourneyChoiceKind kind,
            string title,
            string description,
            string consequence,
            string foundationRuleId = null,
            bool isRecommended = false)
        {
            ChoiceId = choiceId ?? string.Empty;
            Kind = kind;
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
            Consequence = consequence ?? string.Empty;
            FoundationRuleId = foundationRuleId ?? string.Empty;
            IsRecommended = isRecommended;
        }
    }

    [Serializable]
    public sealed class DemoCorePracticeState
    {
        public string DefinitionId = string.Empty;
        public int Level = 1;
        public string BranchId = string.Empty;
        public List<string> GrantedTechniqueIds = new List<string>();

        public DemoCorePracticeState Clone()
        {
            return new DemoCorePracticeState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                Level = Math.Max(1, Level),
                BranchId = BranchId ?? string.Empty,
                GrantedTechniqueIds = GrantedTechniqueIds == null
                    ? new List<string>()
                    : GrantedTechniqueIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.Ordinal).ToList()
            };
        }
    }

    [Serializable]
    public sealed class DemoTechniqueState
    {
        public string DefinitionId = string.Empty;
        public int Level = 1;
        public string SourceNodeId = string.Empty;
        public string SourceEventId = string.Empty;
        public string VariantId = string.Empty;

        public DemoTechniqueState Clone()
        {
            return new DemoTechniqueState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                Level = Math.Max(1, Level),
                SourceNodeId = SourceNodeId ?? string.Empty,
                SourceEventId = SourceEventId ?? string.Empty,
                VariantId = VariantId ?? string.Empty
            };
        }
    }

    [Serializable]
    public sealed class DemoInnateArtifactState
    {
        public string DefinitionId = string.Empty;
        public int RefinementStage = 1;
        public string BearerDefinitionId = string.Empty;
        public string BearerVisualId = string.Empty;
        public float CooldownRemaining;
        public List<string> AttackVariantIds = new List<string>();

        public DemoInnateArtifactState Clone()
        {
            return new DemoInnateArtifactState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                RefinementStage = Math.Max(1, RefinementStage),
                BearerDefinitionId = BearerDefinitionId ?? string.Empty,
                BearerVisualId = BearerVisualId ?? string.Empty,
                CooldownRemaining = Math.Max(0f, CooldownRemaining),
                AttackVariantIds = AttackVariantIds == null
                    ? new List<string>()
                    : AttackVariantIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.Ordinal).ToList()
            };
        }
    }

    [Serializable]
    public sealed class DemoRealmState
    {
        public string RealmId = "realm_qi_refining";
        public int Stage = 1;
        public string FoundationRuleId = string.Empty;
        public List<string> BreakthroughSourceIds = new List<string>();

        public DemoRealmState Clone()
        {
            return new DemoRealmState
            {
                RealmId = RealmId ?? string.Empty,
                Stage = Math.Max(1, Stage),
                FoundationRuleId = FoundationRuleId ?? string.Empty,
                BreakthroughSourceIds = BreakthroughSourceIds == null
                    ? new List<string>()
                    : BreakthroughSourceIds.Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.Ordinal).ToList()
            };
        }
    }

    [Serializable]
    public sealed class DemoStoryState
    {
        private readonly HashSet<string> experienceFlagIds = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> consumedUniqueContentIds = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> pendingMetaDiscoveryIds = new HashSet<string>(StringComparer.Ordinal);

        public IReadOnlyCollection<string> ExperienceFlagIds => experienceFlagIds;
        public IReadOnlyCollection<string> ConsumedUniqueContentIds => consumedUniqueContentIds;
        public IReadOnlyCollection<string> PendingMetaDiscoveryIds => pendingMetaDiscoveryIds;

        public bool HasExperience(string id)
        {
            return !string.IsNullOrWhiteSpace(id) && experienceFlagIds.Contains(id);
        }

        public void AddExperience(string id)
        {
            Add(experienceFlagIds, id);
        }

        public void ConsumeUniqueContent(string id)
        {
            Add(consumedUniqueContentIds, id);
        }

        public void AddPendingMetaDiscovery(string id)
        {
            Add(pendingMetaDiscoveryIds, id);
        }

        public void Restore(
            IEnumerable<string> experiences,
            IEnumerable<string> consumedContent,
            IEnumerable<string> pendingDiscoveries)
        {
            experienceFlagIds.Clear();
            consumedUniqueContentIds.Clear();
            pendingMetaDiscoveryIds.Clear();
            AddRange(experienceFlagIds, experiences);
            AddRange(consumedUniqueContentIds, consumedContent);
            AddRange(pendingMetaDiscoveryIds, pendingDiscoveries);
        }

        private static void AddRange(ISet<string> destination, IEnumerable<string> values)
        {
            if (values == null)
            {
                return;
            }

            foreach (string value in values)
            {
                Add(destination, value);
            }
        }

        private static void Add(ISet<string> destination, string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                destination.Add(id);
            }
        }
    }
}
