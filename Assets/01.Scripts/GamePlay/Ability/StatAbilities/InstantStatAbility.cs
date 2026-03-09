using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class InstantStatAbility : IAbilityBase
    {
        public readonly InstantStatAbiltyData _data;
        public IAbilityContextService Host { get; private set; }

        public EGameEventTrigger ProgTriggers => throw new System.NotImplementedException();

        public InstantStatAbility(IAbilityContextService host, InstantStatAbiltyData data)
        {
            Host = host;
            _data = data;
            ExcuteAbility();
            Debug.Log("Instant Ability");
        }

        public void ExcuteAbility()
        {
            //Debug.Log("Excute");
            //if (Host.TryGet<IPlayerStatModifier>(out var service))
            //{
            //    Debug.Log("Excute!");
            //    PlayerApplyStatContext applyStatContext = new PlayerApplyStatContext(
            //    _data.PlayerStat, _data.ApplyType, _data.RatioType, _data.Value);
            //    service.ModifyStat(applyStatContext);
            //}
        }
        public void ProcTrigger(EGameEventTrigger triggerType) {}
    }
}