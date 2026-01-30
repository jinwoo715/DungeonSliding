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
    public interface IAbilityEntity
    {
        public void ModifyStat(ApplyStatContext applyStatContext);
        public void GainBarrier();
    }

    //Query

    public struct ApplyStatContext
    {
        public readonly EPlayerStat PlayerStat;
        public readonly EApplyStatType ApplyType;
        public readonly EPlayerStat RatioType;
        public readonly float Value;

        public ApplyStatContext(EPlayerStat playerStat, EApplyStatType applyType, float value, EPlayerStat ratio)
        {
            PlayerStat = playerStat;
            ApplyType = applyType;
            Value = value;
            RatioType = ratio;
        }
    }
}