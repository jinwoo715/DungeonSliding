using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    [CreateAssetMenu(fileName = "StatAbility", menuName = "Ability/Stat Ability", order = 0)]
    public class StatAbilityDataSO : AbilityDataSOBase
    {
        public ECreatureStatType PlayerStatType = ECreatureStatType.None;
        public EApplyStatType ApplyType = EApplyStatType.None;
        public ECreatureStatType RatioType = ECreatureStatType.None;
        public float StatValue = 0;

        public override AbilityDataBase ToRuntimeData()
        {
            var data = new StatAbilityData
            {
                PlayerStatType = PlayerStatType,
                ApplyType = ApplyType,
                RatioType = RatioType,
                StatValue = StatValue
            };

            CopyBaseDataTo(data);
            return data;
        }
    }
}
