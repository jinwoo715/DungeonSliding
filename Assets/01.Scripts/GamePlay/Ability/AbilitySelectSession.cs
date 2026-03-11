using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilitySelectSession
    {
        public Func<AbilityDataBase[]> GetRerollAbilityEvent;
        public Action<AbilityDataBase> SelectAbiltyUIDEvent;
        public AbilityDataBase[] SelectableAbilities;
        public int RerollCount;

        public AbilitySelectSession(AbilityDataBase[] abilities, Action<AbilityDataBase> selectEvent, Func<AbilityDataBase[]> rerollEvent, int rerollCount)
        {
            SelectableAbilities = abilities;
            SelectAbiltyUIDEvent = selectEvent;
            GetRerollAbilityEvent = rerollEvent;
            RerollCount = rerollCount;
        }

        public bool TryRerollAbilities()
        {
            if (RerollCount > 0)
            {
                SelectableAbilities = GetRerollAbilityEvent?.Invoke();

                RerollCount--;

                return true;
            }
            else return false;
        }
    }
}
