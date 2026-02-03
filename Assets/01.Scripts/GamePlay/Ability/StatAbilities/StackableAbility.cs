using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class StackableAbility : IAbility
    {
        public readonly StackableStatAbilityData _data;
        
        public IAbilityHost Host { get; private set; }
        private IPlayerStatModifier _modifier;

        private int _currentTriggerCount = 0;
        private int _currentResetTriggerThreshold = 0;

        private float _value;
        PlayerApplyStatContext _applyStatContext = new PlayerApplyStatContext();

        public StackableAbility(IAbilityHost host, StackableStatAbilityData data)
        {
            Host = host;
            _data = data;

            _applyStatContext = new PlayerApplyStatContext(
                        _data.PlayerStat, _data.ApplyType, 0, _data.RatioType);

            if (Host.TryGet<IPlayerStatModifier>(out var modifier))
                _modifier = modifier;
        }

        public void ExcuteAbility()
        {
            switch (_data.ApplyStatType)
            {
                case EAbilityApplyStatType.EntityStat:

                    if(Host.TryGet<IPlayerStatModifier>(out var modifier))
                    {
                        PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
                        _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);

                        _applyStatContext.AddValue(-_data.Value);

                        modifier.ModifyStat(applyStatContext);
                    }

                    break;
                case EAbilityApplyStatType.NextActStat:

                    if (Host.TryGet<INextAttackEnhancer>(out var service))
                    {
                        switch (_data.ApplyType)
                        {
                            case EApplyStatType.Add:
                                service.AddEnhance(EnextAttackEnhanceType.Add, _data.Value);
                                break;
                            case EApplyStatType.Multiple:
                                service.AddEnhance(EnextAttackEnhanceType.Multi, _data.Value);
                                break;
                        }

                        switch (_data.nextAttackEnhanceType)
                        {
                            case EnextAttackEnhanceType.Add:
                                service.AddEnhance(EnextAttackEnhanceType.Add, _data.AddNextAttackDamage);

                                break;
                            case EnextAttackEnhanceType.Multi:
                                service.AddEnhance(EnextAttackEnhanceType.Add, _data.MultiNextAttackDamage);

                                break;
                            case EnextAttackEnhanceType.ExtraAttack:
                                service.AddEnhance(EnextAttackEnhanceType.Add, _data.ExtraAttackCount);

                                break;
                        }
                    }

                    break;
            }
        }

        //TODO Ability 버그 수정 및 정리 작업
        public void ProcTrigger(EGameTriggerType triggerType)
        {
            if(_data.AbilityTriggerTypes.Contains(triggerType))
            {
                Debug.Log("Stack");
                _currentTriggerCount++;

                if (_currentTriggerCount >= _data.ExcuteTriggerCount)
                {
                    ExcuteAbility();
                    _currentTriggerCount = 0;
                }
            }
            else if(_data.IsResetEnabled || _data.ResetOnTriggerTypes.Contains(triggerType))
            {
                Debug.Log("Release");
                if (_currentResetTriggerThreshold >= _data.ResetOnOtherTriggerCount)
                {
                    _currentTriggerCount = 0;
                    _currentResetTriggerThreshold = 0;

                    _modifier.ModifyStat(_applyStatContext);
                }
                else
                {
                    _currentResetTriggerThreshold++;
                }
            }
        }
    }
}
