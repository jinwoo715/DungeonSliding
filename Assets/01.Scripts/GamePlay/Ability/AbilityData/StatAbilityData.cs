using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class StatAbilityData
    {
        public int UID;
        public string Name;
        public string Description;
        public string IconName;
        public EAbilityRank Rank;
        public EGameTriggerType TriggerType;
        public EAbilityApplyStatType StatType;
        public EPlayerStatType PlayerStatType;
        public EApplyStatType ApplyType;
        public EPlayerStatType RatioType;
        public float StatValue;
        public ENextAttackType NextAttackType;
        public float NextAttackValue;
        public int NeedStackCount;
        public bool IsResetEnabled;
        public EGameTriggerType ResetOnTrigger;
        public int ResetThreshold;
    }
}
