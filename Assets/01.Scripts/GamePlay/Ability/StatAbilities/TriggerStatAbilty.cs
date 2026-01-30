using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class TriggerStatAbilty : IAbility
    {
        public readonly TriggerStatAbiltyData _data;

        private IStatModifier _modifier;
        public TriggerStatAbilty(IAbilityHost host, TriggerStatAbiltyData data)
        {
            _data = data;
            
            if(host.TryGet<IStatModifier>(out var service))
            {
                _modifier = service;
            }
        }

        public void ExcuteAbility()
        {
            ApplyStatContext applyStatContext = new ApplyStatContext(
                _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);

            _modifier.ModifyStat(applyStatContext);
        }

        public void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}
