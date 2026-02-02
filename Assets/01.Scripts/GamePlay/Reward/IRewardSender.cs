using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public interface IRewardSender
    {
        public RewardData CreateReward();
    }
}
