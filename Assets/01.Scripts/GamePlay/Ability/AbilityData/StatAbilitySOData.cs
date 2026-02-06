using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class StatAbiltySOData : AbilityData
    {
        [Header("Default Stat Ability Data")]

        [Header("Apply Stat\n")]
        public EAbilityApplyStatType ApplyStatType;

        [Header("Player Stat")]
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.EntityStat)]
        public EPlayerStatType PlayerStat;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.EntityStat)]
        public EApplyStatType ApplyType;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.EntityStat)]
        public EPlayerStatType RatioType;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.EntityStat)]
        public float Value;

        [Header("NextAttack")]
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextActStat)]

        public ENextAttackType nextAttackEnhanceType; 
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextActStat)]

        public int AddNextAttackDamage;
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextActStat)]

        public float MultiNextAttackDamage;
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextActStat)]

        public int ExtraAttackCount;
    }
}
