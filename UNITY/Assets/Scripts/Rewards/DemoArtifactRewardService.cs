using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public sealed class DemoArtifactRewardService
    {
        public List<DemoReward> CreateChoices(DemoRunState run)
        {
            List<DemoReward> rewards = new List<DemoReward>();
            IReadOnlyList<DemoArtifactType> ownedArtifacts = run?.Artifacts;

            foreach (DemoArtifactType artifact in GetArtifactPriority(run?.GetBuildStyle() ?? DemoSwordStyle.General))
            {
                if (ownedArtifacts == null || !ownedArtifacts.Contains(artifact))
                {
                    rewards.Add(DemoReward.Artifact(artifact));
                }
            }

            rewards = rewards.Take(3).ToList();

            if (rewards.Count == 0)
            {
                foreach (string relicName in GetFallbackRelicPriority(run))
                {
                    if (run == null || !run.HasRelic(relicName))
                    {
                        rewards.Add(DemoReward.Relic(relicName));
                    }

                    if (rewards.Count >= 3)
                    {
                        break;
                    }
                }
            }

            while (rewards.Count < 3)
            {
                if (rewards.All(reward => reward.Type != DemoRewardType.Upgrade))
                {
                    rewards.Add(DemoReward.Upgrade());
                }
                else
                {
                    rewards.Add(DemoReward.Heal());
                }
            }

            return rewards;
        }

        private static IEnumerable<DemoArtifactType> GetArtifactPriority(DemoSwordStyle style)
        {
            IReadOnlyList<string> configuredIds = DemoConfigRepository.GetRewardPriorityRefs("artifact_priority", style, "artifact");
            if (configuredIds.Count > 0)
            {
                List<DemoArtifactType> configured = new List<DemoArtifactType>();
                for (int i = 0; i < configuredIds.Count; i++)
                {
                    if (Enum.TryParse(configuredIds[i], true, out DemoArtifactType artifactType))
                    {
                        configured.Add(artifactType);
                    }
                }

                if (configured.Count > 0)
                {
                    return configured;
                }
            }

            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    return new[]
                    {
                        DemoArtifactType.ThunderSeal,
                        DemoArtifactType.HaotianMirror,
                        DemoArtifactType.PurpleGourd,
                        DemoArtifactType.SwordBox
                    };
                case DemoSwordStyle.Blood:
                    return new[]
                    {
                        DemoArtifactType.PurpleGourd,
                        DemoArtifactType.HaotianMirror,
                        DemoArtifactType.SwordBox,
                        DemoArtifactType.ThunderSeal
                    };
                case DemoSwordStyle.Wanjian:
                default:
                    return new[]
                    {
                        DemoArtifactType.SwordBox,
                        DemoArtifactType.HaotianMirror,
                        DemoArtifactType.PurpleGourd,
                        DemoArtifactType.ThunderSeal
                    };
            }
        }

        private static IEnumerable<string> GetFallbackRelicPriority(DemoRunState run)
        {
            DemoSwordStyle style = run?.GetBuildStyle() ?? DemoSwordStyle.General;
            IReadOnlyList<string> configuredIds = DemoConfigRepository.GetRewardPriorityRefs("artifact_fallback_relics", style, "relic");
            if (configuredIds.Count > 0)
            {
                return configuredIds;
            }

            switch (style)
            {
                case DemoSwordStyle.Thunder:
                    return new[] { "雷心", "九霄雷印", "残破古镜", "聚灵符", "护心镜" };
                case DemoSwordStyle.Blood:
                    return new[] { "血剑胚", "血魔珠", "护心镜", "聚灵符" };
                case DemoSwordStyle.Wanjian:
                default:
                    return new[] { "剑冢残碑", "万剑剑匣", "聚灵符", "残破古镜", "护心镜" };
            }
        }
    }
}
