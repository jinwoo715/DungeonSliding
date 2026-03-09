using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "StackableStatAbiltyData", menuName = "Create Ability/StackableStatAbiltyData", order = 0)]
    public class StackableStatAbilityData : StatAbiltySOData
    {
        [Header("Stackable Data")]

        public int ExcuteTriggerCount;

        public bool IsResetEnabled;
 
        [ShowIf("IsResetEnabled")]
        public List<EGameEventTrigger> ResetOnTriggerTypes;

        [ShowIf("IsResetEnabled")]
        public int ResetOnOtherTriggerCount;

        public override List<EGameEventTrigger> GetEnrollTriggers
        {
            get 
            {
                List<EGameEventTrigger> enrollTriggers = new List<EGameEventTrigger>();

                for (int i = 0; i < ResetOnTriggerTypes.Count; i++)
                {
                    enrollTriggers.Add(ResetOnTriggerTypes[i]);
                }
                for (int i = 0; i < AbilityTriggerTypes.Count; i++)
                {
                    enrollTriggers.Add(AbilityTriggerTypes[i]);
                }

                return enrollTriggers;
            }
        }
    }
}