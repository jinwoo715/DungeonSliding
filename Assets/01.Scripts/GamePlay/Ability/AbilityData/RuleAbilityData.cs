using UnityEngine;
namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "StackableStatAbiltyData", menuName = "Create Ability/StackableStatAbiltyData", order = 0)]
    public class RuleAbilityData : AbilityData
    {
        public ERuleAbilityType RuleAbilityType;
        public float CostValue;
        public float GainValue;
    }
}
