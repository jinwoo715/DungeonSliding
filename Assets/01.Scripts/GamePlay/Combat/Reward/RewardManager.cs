using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding 
{
    public class RewardManager
    {
        public Action<RewardData> GetRewardEvent;

        public void AddReward(RewardData rewardData)
        {
            GetRewardEvent?.Invoke(rewardData);
        }
    }
}
