using UnityEngine;
namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "RuleAbility", menuName = "Create Ability/RuleAbility", order = 0)]
    public class RuleAbilitySOData : AbilityData
    {
        public ERuleAbilityType RuleAbilityType;
        public float CostValue;
        public float GainValue;
    }
}
