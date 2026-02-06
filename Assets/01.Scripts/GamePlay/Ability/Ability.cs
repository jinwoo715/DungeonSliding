using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IAbilityHost
    {
        bool TryGet<T>(out T service) where T : class;
    }

    //Query

    public struct PlayerApplyStatContext
    {
        public readonly EPlayerStatType PlayerStat;
        public readonly EApplyStatType ApplyType;
        public readonly EPlayerStatType RatioStatType;
        public float Value;

        public PlayerApplyStatContext(EPlayerStatType playerStat, EApplyStatType applyType, float value, EPlayerStatType ratio)
        {
            PlayerStat = playerStat;
            ApplyType = applyType;
            Value = value;
            RatioStatType = ratio;
        }

        public void AddValue(float value)
        {
            Value += value;
        }
    }
    public struct EnemyApplyStatContext
    {
        public readonly EEnemyStatType EnemyStat;
        public readonly EApplyStatType ApplyType;
        public readonly EEnemyStatType RatioStatType;
        public readonly float Value;

        public EnemyApplyStatContext(EEnemyStatType playerStat, EApplyStatType applyType, float value, EEnemyStatType ratio)
        {
            EnemyStat = playerStat;
            ApplyType = applyType;
            Value = value;
            RatioStatType = ratio;
        }
    }
}