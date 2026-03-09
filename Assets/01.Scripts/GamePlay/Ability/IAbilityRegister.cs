using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IAbilityRegister
    {
        public void RegisterGameEventAbility(EGameEventTrigger trigger, IAbility ability);
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability);
        public void RegisterAbility(IAbility ability);
        public void RegisterAutoAllAbility(List<IAbility> abilities);
    }
    public interface IAbilityExcuter
    {
        public void ExecuteGameEventAbility<T>(EGameEventTrigger trigger, T data);
        public void ExecuteGameEventAbility(EGameEventTrigger trigger);
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger);
        public void ExecuteCreatureTrigger<T>(ECreatureTrigger trigger, T data);
    }
}
