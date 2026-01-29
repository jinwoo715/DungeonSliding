using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
public class AbilityFactory
{
    public IAbility GetAbility(AbilityData data)
    {
        return null;
    }

    public IAbility CreateStatAbility(AbilityData data, IAbilityEntity entity)
    {
        if(data is InstantStatAbiltyData)
        {
//            IAbility ability = new InstantStatAbility(entity, (InstantStatAbiltyData)data);
        }
        else if(data is TriggerStatAbiltyData)
        {
//            IAbility ability = new TriggerStatAbilty(entity, (TriggerStatAbiltyData)data);
        }
        else if(data is StackableStatAbilityData)
        {

        }

        return null;
    }
    public IAbility CreateRuleAbility()
    {
        return null;
    }
    public IAbility CreateRewardAbility()
    {
        return null;
    }
}
}