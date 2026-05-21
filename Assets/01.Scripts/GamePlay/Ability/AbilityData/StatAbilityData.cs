using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class StatAbilityData : AbilityDataBase
    {
        public ECreatureStatType PlayerStatType = ECreatureStatType.None;
        public EApplyStatType ApplyType = EApplyStatType.None;
        public ECreatureStatType RatioType = ECreatureStatType.None;
        public float StatValue = 0;
    }

    [System.Serializable]
    public class RuleStatAbilityData : StatAbilityData
    {
        public EGameEventTrigger GameTriggerType = EGameEventTrigger.None;
        public ECreatureTrigger CreatureTriggerType = ECreatureTrigger.None;

        public EAbilityApplyStatType StatType = EAbilityApplyStatType.None;

        public ENextAttackType NextAttackType = ENextAttackType.None;
        public float NextAttackValue = 0;

        public int NeedStackCount = 0;

        public bool IsResetEnabled = false;
        public EGameEventTrigger ResetGameTrigger = EGameEventTrigger.None;
        public ECreatureTrigger ResetCreatureTrigger = ECreatureTrigger.None;
        public int ResetThreshold = 0;
    }
}