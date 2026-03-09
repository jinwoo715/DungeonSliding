using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IAbilityEffect
    {
        void Apply();
        void Reset();
    }
    public class PlayerStatEffect : IAbilityEffect
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
            _statContext.AddValue(_data.StatValue);
            _stackedValue += _data.StatValue;

            _modifier.ModifyStat(_statContext);
        }

        public void Reset() 
        {
            _statContext.AddValue(-_stackedValue * 2);
            _modifier.ModifyStat(_statContext);

            _statContext.Reset();
        }
    }

    public class NextAttackEffect : IAbilityEffect
    {
        private INextAttackEnhancer _nextAttackEnhancer;
        private StatAbilityData _data;
        
        public NextAttackEffect(INextAttackEnhancer nextAttackEnhancer, StatAbilityData data)
        {
            _nextAttackEnhancer = nextAttackEnhancer;
            _data = data;
        }

        public void Apply()
        {
            switch (_data.NextAttackType)
            {
                case ENextAttackType.Add:
                    _nextAttackEnhancer.AddNextAttackDamage((Mathf.RoundToInt(_data.NextAttackValue)));
                    break;
                case ENextAttackType.Multiple:
                    _nextAttackEnhancer.AddNextAttackDamageMulti(_data.NextAttackValue);
                    break;
                case ENextAttackType.ExtraAttack:
                    _nextAttackEnhancer.AddNextAttackCount(Mathf.RoundToInt(_data.NextAttackValue));
                    break;
            }
        }

        public void Reset() {}
    }


    public class StatAbility : IAbility
    {
        private StatAbilityData _data;
        private IAbilityContextService _context;

        private int _currentStack = 0;
        private int _currentResetTriggerThreshold = 0;

        private IAbilityEffect _effect;

        public EGameEventTrigger GameTrigger => _data.GameTriggerType | _data.ResetGameTrigger;
        public ECreatureTrigger CreatureTrigger => _data.CreatureTrigger | _data.ResetCreatureTrigger;

        //TODO 수정 필요
        public StatAbility(StatAbilityData data, IAbilityContextService context)
        {
            _data = data;
            _context = context;

            if (_context.TryGet<IStatModifier>(out var modifier))

            if (_context.TryGet<INextAttackEnhancer>(out var nextAttackEnhancer))

            if (_data.StatType == EAbilityApplyStatType.PlayerStat)
                _effect = new PlayerStatEffect(modifier, data);
            else if (_data.StatType == EAbilityApplyStatType.NextAttack)
                _effect = new NextAttackEffect(nextAttackEnhancer, data);
        }

        public IEnumerator Execute(AbilityArgs args)
        {
            if(args.GameTrigger == _data.GameTriggerType || args.CreatureTrigger == _data.CreatureTrigger)
            {
                _currentStack++;

                if(_currentStack >= _data.NeedStackCount)
                {
                    _effect.Apply();
                    _currentStack = 0;
                }
            }

            if (_data.IsResetEnabled == true)
            {
                if (args.GameTrigger == _data.GameTriggerType || args.CreatureTrigger == _data.CreatureTrigger)
                {
                    _currentResetTriggerThreshold++;

                    if(_currentResetTriggerThreshold >= _data.ResetThreshold)
                    {
                        ReleaseAbility();
                    }

                }
            }

            yield return null;
        }

        public void ReleaseAbility()
        {
            _effect.Reset();
        }
    }
}
