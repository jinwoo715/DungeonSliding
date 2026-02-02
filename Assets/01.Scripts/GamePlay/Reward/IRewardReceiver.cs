using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public interface IRewardReceiver
    {
        public void AddReward(RewardData rewardData);
    }
}
