using System.Collections.Generic;
using UnityEngine;
namespace JW.DungeonSliding.GamePlay.Ability
{

    [CreateAssetMenu(fileName = "StackableStatAbiltyData", menuName = "Create Ability/StackableStatAbiltyData", order = 0)]
    public class StackableStatAbilityData : StatAbiltyData
    {
        public int ExcuteTriggerCount;

        public bool IsResetEnabled;
        public List<EGameTriggerType> ResetOnTriggerTypes;
        public int ResetOnOtherTriggerCount;

        public override List<EGameTriggerType> GetEnrollTriggers
        {
            get 
            {
                List<EGameTriggerType> enrollTriggers = AbilityTriggerTypes;
                enrollTriggers.AddRange(ResetOnTriggerTypes);
                return enrollTriggers;
            }
        }
    }
}