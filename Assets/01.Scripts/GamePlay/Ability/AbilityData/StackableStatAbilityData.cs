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
        public List<EGameTriggerType> ResetOnTriggerTypes;

        [ShowIf("IsResetEnabled")]
        public int ResetOnOtherTriggerCount;

        public override List<EGameTriggerType> GetEnrollTriggers
        {
            get 
            {
                List<EGameTriggerType> enrollTriggers = new List<EGameTriggerType>();

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