using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class StatAbilityData : AbilityDataBase
    {
        public EGameTriggerType TriggerType = EGameTriggerType.None;
        public EAbilityApplyStatType StatType = EAbilityApplyStatType.None;
        public EPlayerStatType PlayerStatType = EPlayerStatType.None;
        public EApplyStatType ApplyType = EApplyStatType.None;
        public EPlayerStatType RatioType = EPlayerStatType.None;
        public float StatValue = 0;
        public ENextAttackType NextAttackType = ENextAttackType.None;
        public float NextAttackValue = 0;
        public int NeedStackCount = 0;
        public bool IsResetEnabled = false;
        public EGameTriggerType ResetOnTrigger = EGameTriggerType.None;
        public int ResetThreshold = 0;
    }
}
