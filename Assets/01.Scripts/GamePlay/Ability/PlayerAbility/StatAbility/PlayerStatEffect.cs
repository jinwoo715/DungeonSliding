using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class PlayerStatEffect : IStatAbilityEffect
    {
        private StatAbilityData _data;
        private IStatModifier _modifier;
        private StatModifierContext _statContext;
        private float _stackedValue = 0;

        public PlayerStatEffect(IStatModifier modifier, StatAbilityData data)
        {
            _modifier = modifier;
            _data = data;

            if (_data.ApplyType == EApplyStatType.Ratio)
            {
                _statContext.SetRatioModify(_data.PlayerStatType, _data.RatioType, _data.StatValue);
            }
            else
            {
                _statContext.SetAddOrMultiModify(_data.PlayerStatType, _data.ApplyType, _data.StatValue);
            }
        }

        public void Apply()
        {
            _stackedValue += _data.StatValue;

            _modifier.ModifyStat(_statContext);
        }

        public void Reset()
        {
            StatModifierContext revert = _statContext;
            revert.SetValue(-_stackedValue);
            
            _modifier.ModifyStat(revert);

            _stackedValue = 0;
        }
    }

}
