using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class StatAbilityData : AbilityDataBase
    {
        #region Trigger
        public EGameEventTrigger GameTriggerType = EGameEventTrigger.None;
        public ECreatureTrigger CreatureTriggerType = ECreatureTrigger.None;
        #endregion

        public EAbilityApplyStatType StatType = EAbilityApplyStatType.None;

        #region Stat
        public ECreatureStatType PlayerStatType = ECreatureStatType.None;
        public EApplyStatType ApplyType = EApplyStatType.None;
        public ECreatureStatType RatioType = ECreatureStatType.None;
        public float StatValue = 0;
        #endregion

        #region NextAttack
        public ENextAttackType NextAttackType = ENextAttackType.None;
        public float NextAttackValue = 0;
        #endregion

        public int NeedStackCount = 0;

        #region Reset Trigger
        public bool IsResetEnabled = false;
        public EGameEventTrigger ResetGameTrigger = EGameEventTrigger.None;
        public ECreatureTrigger ResetCreatureTrigger = ECreatureTrigger.None;
        public int ResetThreshold = 0;
        #endregion
    }
}
