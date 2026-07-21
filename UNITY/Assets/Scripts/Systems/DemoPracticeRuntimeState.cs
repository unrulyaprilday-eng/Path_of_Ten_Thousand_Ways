using System;
using System.Collections.Generic;
using System.Linq;

namespace PathOfTenThousandWays.Demo.Systems
{
    [Serializable]
    public sealed class DemoCorePracticeState
    {
        public string DefinitionId = string.Empty;
        public int Level = 1;

        public DemoCorePracticeState Clone()
        {
            return new DemoCorePracticeState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                Level = Math.Max(1, Level)
            };
        }
    }

    [Serializable]
    public sealed class DemoTechniqueState
    {
        public string DefinitionId = string.Empty;
        public int Level = 1;
        public string SourceNodeId = string.Empty;

        public DemoTechniqueState Clone()
        {
            return new DemoTechniqueState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                Level = Math.Max(1, Level),
                SourceNodeId = SourceNodeId ?? string.Empty
            };
        }
    }

    [Serializable]
    public sealed class DemoInnateArtifactState
    {
        public string DefinitionId = string.Empty;
        public int RefinementStage = 1;
        public string BearerDefinitionId = string.Empty;

        public DemoInnateArtifactState Clone()
        {
            return new DemoInnateArtifactState
            {
                DefinitionId = DefinitionId ?? string.Empty,
                RefinementStage = Math.Max(1, RefinementStage),
                BearerDefinitionId = BearerDefinitionId ?? string.Empty
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
