using JW.DungeonSliding;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
public class InstantStatAbility : IAbility
{
    public readonly InstantStatAbiltyData _data;
    public IAbilityEntity Entity { get; private set; }

    public InstantStatAbility(IAbilityEntity entity, InstantStatAbiltyData data)
    {
        Entity = entity;
        _data = data;
        ExcuteAbility();
    }

    public void ExcuteAbility()
    {
        ApplyStatContext applyStatContext = new ApplyStatContext(
            _data.PlayerStat, _data.ApplyType, _data.Value, _data.RatioType);
        Entity.ModifyStat(applyStatContext);
    }

    public void ProcTrigger(EGameTriggerType triggerType)
    {

    }
}
}