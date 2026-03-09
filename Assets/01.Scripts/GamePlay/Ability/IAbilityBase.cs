using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbilityBase
    {
        public void ExcuteAbility();
        public void ProcTrigger(EGameEventTrigger triggerType);
        public EGameEventTrigger ProgTriggers { get; }
    }
}
