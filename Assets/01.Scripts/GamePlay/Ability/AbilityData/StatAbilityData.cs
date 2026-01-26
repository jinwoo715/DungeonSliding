using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class StatAbiltyData : AbilityData
    {
        [Header("Default Stat Ability Data")]
        public EPlayerStat PlayerStat;
        public EApplyStatType ApplyType;
        public EPlayerStat RatioType;
        public float Value;
    }
}
