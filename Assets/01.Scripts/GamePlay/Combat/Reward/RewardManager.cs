using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding 
{
    public class RewardManager
    {
        public event Action<RewardData> GetRewardEvent;

        public void GainReward(RewardData rewardData)
        {
            GetRewardEvent?.Invoke(rewardData);
        }
    }
}
