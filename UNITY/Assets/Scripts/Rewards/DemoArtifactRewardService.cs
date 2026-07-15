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

            while (rewards.Count < 3)
            {
                if (rewards.All(reward => reward.Type != DemoRewardType.Upgrade))
                {
                    rewards.Add(DemoReward.Upgrade());
                }
                else if (rewards.All(reward => reward.Type != DemoRewardType.Heal))
                {
                    rewards.Add(DemoReward.Heal());
                }
                else
                {
                    rewards.Add(DemoReward.FromCard(DemoCardLibrary.Create("jade_barrier")));
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

    }
}
