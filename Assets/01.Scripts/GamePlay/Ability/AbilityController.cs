using JW.DungeonSliding;
using JW.DungeonSliding.GamePlay.Ability;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
public class AbilityController : MonoBehaviour
{
    public Dictionary<EPlayerStat, IAbility> StatAbilityDic = new Dictionary<EPlayerStat, IAbility>();
    public Dictionary<ERuleEffect, IAbility> RuleAbilityDic = new Dictionary<ERuleEffect, IAbility>();
    public Dictionary<ERewardEffect, IAbility> RewardAbility = new Dictionary<ERewardEffect, IAbility>();

    public void AddAbility(AbilityData data, EAbilityEffectKind abilityKind)
    {

    }
}
}