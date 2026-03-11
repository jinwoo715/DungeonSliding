using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [System.Serializable]
    public class RuleAbilityData : AbilityDataBase
    {
        public EGameEventTrigger TriggerType;
        public ERuleAbilityType RuleType;
        public float P1;
        public float P2;
        public string Notes;
    }
}
