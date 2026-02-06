using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class AbilityDataBase
    {
        public string UID;
        public string Name;
        public string Description;
        public string IconName;
        public EAbilityRank Rank;
    }

    [System.Serializable]
    public class RuleAbilityData : AbilityDataBase
    {
        public EGameTriggerType TriggerType;
        public ERuleAbilityType RuleType;
        public float P1;
        public float P2;
        public string Notes;
    }
}
