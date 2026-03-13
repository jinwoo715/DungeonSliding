namespace JW.DungeonSliding
{
    public enum ERewardType
    {
        None,
        KillReward,
    }
    public readonly struct RewardData
    {
        public readonly int Xp;
        public readonly ERewardType RewardType;
        public RewardData(ERewardType rewardType, int xp)
        {
            Xp = xp;
            RewardType = rewardType;
        }
    }
}