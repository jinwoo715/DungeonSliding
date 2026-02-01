using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class StackableAbility : IAbility
    {
        public readonly StackableStatAbilityData _data;
        
        public IAbilityHost Host { get; private set; }

        private int _currentTriggerCount = 0;
        private int _currentResetTriggerThreshold = 0;
        public StackableAbility(IAbilityHost host, StackableStatAbilityData data)
        {
            Host = host;
            _data = data;
            ExcuteAbility();
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
                        
                        modifier.ModifyStat(applyStatContext);
                    }

                    break;
                case EAbilityApplyStatType.NextActStat:

                    if (Host.TryGet<INextAttackEnhancer>(out var service))
                    {
                        switch (_data.ApplyType)
                        {
                            case EApplyStatType.Add:
                                service.AddEnhance(ENextAttackEnhanceType.Add, _data.Value);
                                break;
                            case EApplyStatType.Multiple:
                                service.AddEnhance(ENextAttackEnhanceType.Multi, _data.Value);
                                break;
                        }
                    }

                    break;
            }
        }

        public void ProcTrigger(EGameTriggerType triggerType)
        {
            if(_data.AbilityTriggerTypes.Contains(triggerType))
            {
                _currentTriggerCount++;

                if (_currentTriggerCount >= _data.ExcuteTriggerCount)
                {
                    ExcuteAbility();
                    _currentTriggerCount = 0;
                }
            }
            else if(_data.IsResetEnabled || _data.ResetOnTriggerTypes.Contains(triggerType))
            {
                if (_currentResetTriggerThreshold >= _data.ResetOnOtherTriggerCount)
                {
                    _currentTriggerCount = 0;
                    _currentResetTriggerThreshold = 0;
                }
                else
                {
                    _currentResetTriggerThreshold++;
                }
            }
        }
    }
}
