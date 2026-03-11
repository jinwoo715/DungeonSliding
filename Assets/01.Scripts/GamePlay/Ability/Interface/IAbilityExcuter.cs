using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbilityExcuter
    {
        public void ExecuteGameEventAbility<T>(EGameEventTrigger trigger, T data);
        public void ExecuteGameEventAbility(EGameEventTrigger trigger);
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger);
        public void ExecuteCreatureTrigger<T>(ECreatureTrigger trigger, T data);
    }
}
