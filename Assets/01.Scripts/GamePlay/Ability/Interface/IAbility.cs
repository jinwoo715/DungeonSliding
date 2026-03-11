using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbility
    {
        public EGameEventTrigger GameTrigger { get; }
        public ECreatureTrigger CreatureTrigger { get; }
        public IEnumerator Execute(AbilityArgs args);
        public void ReleaseAbility();
    }
}
