using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.SlidingPuzzle 
{
    public class PlayerRewardManager
    {
        public Action<RewardData> GetRewardEvent;

        public void AddReward(RewardData rewardData)
        {
            GetRewardEvent?.Invoke(rewardData);
        }
    }
}
