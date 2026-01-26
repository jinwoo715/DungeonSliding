using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class TriggerStatAbilty : IAbility
    {
        public readonly TriggerStatAbiltyData _data;
        public IAbilityEntity Entity { get; private set; }

        public TriggerStatAbilty(IAbilityEntity entity, TriggerStatAbiltyData data)
        {
            Entity = entity;
            _data = data;
        }

        public void ExcuteAbility()
        {
            ApplyStatContext applyStatContext = new ApplyStatContext(
                _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);
            
            Entity.ModifyStat(applyStatContext);
        }

        public void ProcTrigger(EAbilityTriggerType triggerType)
        {
            if(triggerType == _data.AbilityTrigger)
            {
                ExcuteAbility();
            }
        }
    }
}
