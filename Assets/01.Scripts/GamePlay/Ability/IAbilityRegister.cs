using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IAbilityRegister
    {
        public void RegisterGameEventAbility(EGameTriggerType trigger, IAbility ability);
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability);
    }
    public interface IAbilityExcuter
    {
        public void ExecuteGameEventAbility<T>(EGameTriggerType trigger, T data);
        public void ExecuteGameEventAbility(EGameTriggerType trigger);
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger);
        public void ExecuteCreatureTrigger<T>(ECreatureTrigger trigger, T data);
    }
}
