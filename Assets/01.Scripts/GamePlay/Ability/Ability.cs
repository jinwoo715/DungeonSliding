using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    //Query

    public struct ApplyStatContext
    {
        public readonly ECreatureStatType PlayerStat;
        public readonly EApplyStatType ApplyType;
        public readonly ECreatureStatType RatioStatType;
        public float Value;

        public ApplyStatContext(ECreatureStatType playerStat, EApplyStatType applyType, ECreatureStatType ratio, float value)
        {
            PlayerStat = playerStat;
            ApplyType = applyType;
            RatioStatType = ratio;
            Value = value;
        }

        public void AddValue(float value)
        {
            Value += value;
        }
        public void Reset()
        {
            Value = 0;
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