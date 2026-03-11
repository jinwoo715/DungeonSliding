using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbilityRegister
    {
        public void RegisterGameEventAbility(EGameEventTrigger trigger, IAbility ability);
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability);
        public void RegisterAbility(IAbility ability);
        public void RegisterAutoAllAbility(List<IAbility> abilities);
    }
}
