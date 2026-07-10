using System;
using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Map;
using PathOfTenThousandWays.Demo.Rewards;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.CompileCheck
{
    internal static class Program
    {
        private static int Main()
        {
            try
            {
                VerifyPostOpeningBattleFlow();
                Console.WriteLine("Post-opening battle flow smoke check passed.");
                return 0;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
        }

        private static void VerifyPostOpeningBattleFlow()
        {
            Require(DemoConfigRepository.HasLoadedConfig, "game_config.json was not loaded.");

            DemoRunState run = new DemoRunState();
            Require(run.Map.CurrentNode.Type == DemoNodeType.Start, "Run must begin at the opening node.");

            run.Map.CompleteCurrentNode();
            Require(run.Map.CurrentNode.Type == DemoNodeType.Battle, "Opening selection must lead into the entry battle.");

            run.Map.CompleteCurrentNode();
            Require(run.Map.CurrentNode.Type == DemoNodeType.Reward, "Entry battle victory must lead into the first reward.");

            DemoRewardService rewardService = new DemoRewardService();
            List<DemoReward> rewardChoices = rewardService.CreateChoices(run.Map.CurrentNode.Layer, run);
            Require(rewardChoices.Count == 3, "The first battle reward must offer exactly three choices.");
            Require(rewardChoices.Select(reward => reward.Name).Distinct().Count() == 3, "The first battle reward choices must be distinct.");
            Require(rewardChoices.All(IsBuildReward), "The first battle reward must only contain persistent build rewards.");

            ApplyReward(run, rewardChoices[0]);
            run.Map.CompleteCurrentNode();
            Require(run.Map.CurrentNode.Type == DemoNodeType.RouteChoice, "Claiming the first reward must open the first route choice.");

            DemoRouteRewardService routeService = new DemoRouteRewardService();
            List<DemoReward> routeChoices = routeService.CreateChoices(run.Map.CurrentNode.Layer, run);
            Require(routeChoices.Count == 3, "The first route choice must offer stable, risky, and build routes.");
            Require(routeChoices.All(reward => reward.Type == DemoRewardType.Route && reward.RoutePlan != null), "Every route choice must contain a route plan.");

            string[] expectedRoutes = { "矿口余烬", "塌井深处", "旧账暗室" };
            Require(expectedRoutes.All(name => routeChoices.Any(reward => reward.Name == name)), "The first route choice is missing a designed old-mine route.");

            foreach (DemoReward routeReward in routeChoices)
            {
                VerifyRouteReachesConfiguredBattle(routeReward);
            }
        }

        private static void VerifyRouteReachesConfiguredBattle(DemoReward routeReward)
        {
            DemoMapRun map = new DemoMapRun();
            map.CompleteCurrentNode();
            map.CompleteCurrentNode();
            map.CompleteCurrentNode();
            Require(map.CurrentNode.Type == DemoNodeType.RouteChoice, $"{routeReward.Name} setup did not reach route choice.");

            map.SelectRoute(routeReward.RoutePlan);

            int stepLimit = routeReward.RoutePlan.Nodes.Count;
            while (map.CurrentNode.Type != DemoNodeType.Battle && stepLimit-- > 0)
            {
                Require(map.CurrentNode.Type != DemoNodeType.RouteChoice, $"{routeReward.Name} reached another route choice before a battle.");
                map.CompleteCurrentNode();
            }

            Require(map.CurrentNode.Type == DemoNodeType.Battle, $"{routeReward.Name} does not reach a battle.");
            Require(
                DemoConfigRepository.TryGetEnemyByName(map.CurrentNode.Name, out DemoEnemyDefinition enemy),
                $"{routeReward.Name} battle '{map.CurrentNode.Name}' has no enemy configuration.");
            Require(enemy.MaxHealth > 0, $"{map.CurrentNode.Name} must have positive health.");
        }

        private static bool IsBuildReward(DemoReward reward)
        {
            return reward.Type == DemoRewardType.Card
                || reward.Type == DemoRewardType.Gongfa
                || reward.Type == DemoRewardType.Artifact
                || reward.Type == DemoRewardType.Relic
                || reward.Type == DemoRewardType.Upgrade
                || reward.Type == DemoRewardType.Heal;
        }

        private static void ApplyReward(DemoRunState run, DemoReward reward)
        {
            switch (reward.Type)
            {
                case DemoRewardType.Card:
                    run.AddCard(reward.Card);
                    break;
                case DemoRewardType.Gongfa:
                    run.LearnGongfa(reward.GongfaType);
                    break;
                case DemoRewardType.Artifact:
                    run.AddArtifact(reward.ArtifactType);
                    break;
                case DemoRewardType.Relic:
                    run.AddRelic(reward.Name);
                    break;
                case DemoRewardType.Upgrade:
                    run.UpgradeEnergy();
                    break;
                case DemoRewardType.Heal:
                    run.Heal(DemoConfigRepository.GetIntConstant("battle", "heal_reward_amount", 18));
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported first battle reward: {reward.Type}");
            }
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}