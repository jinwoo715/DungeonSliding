using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public struct ApplyStatContext
    {
        public readonly ECreatureStatType PlayerStat;
        public readonly EApplyStatType ApplyType;
        public readonly ECreatureStatType RatioStatType;
        public float Value;

        public ApplyStatContext(ECreatureStatType playerStat, EApplyStatType applyType, ECreatureStatType ratio, float value)
        {
            PlayerStat = playerStat;
            ApplyType = applyType;
            RatioStatType = ratio;
            Value = value;
        }

        public void AddValue(float value)
        {
            Value += value;
        }
        public void Reset()
        {
            Value = 0;
        }
    }
}
