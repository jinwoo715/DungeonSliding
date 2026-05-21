using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.Ability.Data
{
    [CreateAssetMenu(fileName = "StatAbility", menuName = "Ability/StatAbility", order = 0)]
    public class StatAbilityDataSO : ScriptableObject
    {
        public ECreatureStatType PlayerStatType = ECreatureStatType.None;
        public EApplyStatType ApplyType = EApplyStatType.None;
        public ECreatureStatType RatioType = ECreatureStatType.None;
        public float StatValue = 0;
    }
}
