using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class StatAbility : IAbility
    {
        private StatAbilityData _data;
        private AbilityHost _host;

        private IPlayerStatModifier _modifier;
        private INextAttackEnhancer _nextAttackEnhancer;

        private int _currentStack = 0;
        private int _currentResetTriggerThreshold = 0;

        private PlayerApplyStatContext _applyStatContext;

        public EGameTriggerType ProgTriggers => _data.TriggerType | _data.ResetOnTrigger;

        public StatAbility(StatAbilityData data, AbilityHost host)
        {
            _data = data;
            _host = host;

            if (_host.TryGet<IPlayerStatModifier>(out var modifier))
                _modifier = modifier;

            if (_host.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))
                _nextAttackEnhancer = nextAttackEnhancer;

            if (_data.TriggerType == EGameTriggerType.Instant)
                ExcuteAbility();

            _applyStatContext = new PlayerApplyStatContext(_data.PlayerStatType, _data.ApplyType, _data.RatioType, 0);
        }

        public void ExcuteAbility()
        {
            switch (_data.StatType)
            {
                case EAbilityApplyStatType.PlayerStat:

                    PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
               _data.PlayerStatType, _data.ApplyType, _data.RatioType, _data.StatValue);

                    _modifier.ModifyStat(applyStatContext);
                    
                    _applyStatContext.AddValue(-_data.StatValue);

                    break;
                case EAbilityApplyStatType.NextAttack:

                    _nextAttackEnhancer.AddEnhance(_data.NextAttackType, _data.NextAttackValue);

                    break;
            }
        }

        public void ResetStat()
        {
            _modifier.ModifyStat(_applyStatContext);
            _applyStatContext.Reset();
        }

        public void ProcTrigger(EGameTriggerType triggerType)
        {
            if(_data.TriggerType == triggerType)
            {
                _currentStack++;
                _currentResetTriggerThreshold = 0;

                if (_currentStack >= _data.NeedStackCount)
                {
                    _currentStack = 0;

                    ExcuteAbility();
                }

            }
            else if (_data.ResetOnTrigger == triggerType && _data.IsResetEnabled)
            {
                _currentResetTriggerThreshold++;

                if (_currentResetTriggerThreshold >= _data.ResetThreshold)
                {
                    ResetStat();

                    _currentStack = 0;
                    _currentResetTriggerThreshold = 0;
                }
            }
        }
    }
}
