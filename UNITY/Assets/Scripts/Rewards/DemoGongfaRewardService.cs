using System.Collections.Generic;
using System.Linq;
using PathOfTenThousandWays.Demo.Cards;
using PathOfTenThousandWays.Demo.Systems;

namespace PathOfTenThousandWays.Demo.Rewards
{
    public sealed class DemoGongfaRewardService
    {
        public List<DemoReward> CreateMainChoices()
        {
            return CreateChoices(DemoGongfaSlot.Main, DemoSwordStyle.General);
        }

        public List<DemoReward> CreateSupportChoices(DemoRunState run)
        {
            return CreateChoices(DemoGongfaSlot.Support, run.GetBuildStyle());
        }

        public List<DemoReward> CreateDivineChoices(DemoRunState run)
        {
            return CreateChoices(DemoGongfaSlot.Divine, run.GetBuildStyle());
        }

        private static List<DemoReward> CreateChoices(DemoGongfaSlot slot, DemoSwordStyle focusStyle)
        {
            List<DemoGongfaType> types = DemoGongfaLibrary
                .GetTypesForSlot(slot)
                .OrderByDescending(type => DemoGongfaLibrary.Get(type).Style == focusStyle)
                .ToList();

            List<DemoReward> rewards = new List<DemoReward>();
            for (int i = 0; i < types.Count; i++)
            {
                rewards.Add(DemoReward.Gongfa(types[i]));
            }

            return rewards;
        }
    }
}
