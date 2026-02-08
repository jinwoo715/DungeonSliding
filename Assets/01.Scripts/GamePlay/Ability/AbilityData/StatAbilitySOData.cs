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
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.PlayerStat)]
        public EPlayerStatType PlayerStat;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.PlayerStat)]
        public EApplyStatType ApplyType;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.PlayerStat)]
        public EPlayerStatType RatioType;

        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.PlayerStat)]
        public float Value;

        [Header("NextAttack")]
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextAttack)]

        public ENextAttackType nextAttackEnhanceType; 
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextAttack)]

        public int AddNextAttackDamage;
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextAttack)]

        public float MultiNextAttackDamage;
        [ShowIf(nameof(ApplyStatType), EAbilityApplyStatType.NextAttack)]

        public int ExtraAttackCount;
    }
}
