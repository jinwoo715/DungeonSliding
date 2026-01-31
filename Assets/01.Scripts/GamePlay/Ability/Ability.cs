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

    public struct ApplyStatContext
    {
        public readonly EPlayerStat PlayerStat;
        public readonly EApplyStatType ApplyType;
        public readonly EPlayerStat RatioStatType;
        public readonly float Value;

        public ApplyStatContext(EPlayerStat playerStat, EApplyStatType applyType, float value, EPlayerStat ratio)
        {
            PlayerStat = playerStat;
            ApplyType = applyType;
            Value = value;
            RatioStatType = ratio;
        }
    }
}