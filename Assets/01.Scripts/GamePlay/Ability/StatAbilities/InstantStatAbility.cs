using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class InstantStatAbility : IAbility
    {
        public readonly InstantStatAbiltyData _data;
        public IAbilityHost Host { get; private set; }

        public InstantStatAbility(IAbilityHost host, InstantStatAbiltyData data)
        {
            Host = host;
            _data = data;
            ExcuteAbility();
        }

        public void ExcuteAbility()
        {
            if (Host.TryGet<IStatModifier>(out var service))
            {
                ApplyStatContext applyStatContext = new ApplyStatContext(
                _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);
                service.ModifyStat(applyStatContext);
            }
        }
        public void ProcTrigger(EGameTriggerType triggerType) {}
    }
}