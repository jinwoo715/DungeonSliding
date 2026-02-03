using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class StatAbiltyData : AbilityData
    {
        [Header("Default Stat Ability Data")]

        [Header("Stat")]
        public EAbilityApplyStatType ApplyStatType;

        public EPlayerStat PlayerStat;
        public EApplyStatType ApplyType;
        public EPlayerStat RatioType;
        public float Value;

        [Header("NextAttack")]
        public EnextAttackEnhanceType nextAttackEnhanceType; 
        public int AddNextAttackDamage;
        public float MultiNextAttackDamage;
        public int ExtraAttackCount;
    }
}
