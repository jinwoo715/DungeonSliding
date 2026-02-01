using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class TriggerStatAbilty : IAbility
    {
        public readonly TriggerStatAbiltyData _data;

        private IPlayerStatModifier _modifier;
        public TriggerStatAbilty(IAbilityHost host, TriggerStatAbiltyData data)
        {
            _data = data;
            
            if(host.TryGet<IPlayerStatModifier>(out var service))
            {
                _modifier = service;
            }
        }

        public void ExcuteAbility()
        {
            PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
                _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);

            _modifier.ModifyStat(applyStatContext);
        }

        public void ProcTrigger(EGameTriggerType triggerType)
        {
            ExcuteAbility();
        }
    }
}
