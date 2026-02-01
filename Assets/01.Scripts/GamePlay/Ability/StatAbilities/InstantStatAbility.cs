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
            if (Host.TryGet<IPlayerStatModifier>(out var service))
            {
                PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
                _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);
                service.ModifyStat(applyStatContext);
            }
        }
        public void ProcTrigger(EGameTriggerType triggerType) {}
    }
}