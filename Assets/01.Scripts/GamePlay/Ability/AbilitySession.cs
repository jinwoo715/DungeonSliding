using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilitySession
    {
        public Func<AbilityDataBase[]> GetRerollAbilityEvent;
        public Action<string> SelectAbiltyUIDEvent;
        public AbilityDataBase[] Abilities;
        public int RerollCount;

        public AbilitySession(AbilityDataBase[] abilities, Action<string> selectEvent, Func<AbilityDataBase[]> rerollEvent, int rerollCount)
        {
            Abilities = abilities;
            SelectAbiltyUIDEvent = selectEvent;
            GetRerollAbilityEvent = rerollEvent;
            RerollCount = rerollCount;
        }

        public bool TryRerollAbilities()
        {
            if (RerollCount > 0)
            {
                Abilities = GetRerollAbilityEvent?.Invoke();

                RerollCount--;

                return true;
            }
            else return false;
        }


    }
}
