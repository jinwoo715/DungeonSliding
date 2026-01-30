using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilitySession
    {
        public Func<AbilityData[]> GetRerollAbilityEvent;
        public Action<int> SelectAbiltyUIDEvent;
        public AbilityData[] Abilities;
        public int RerollCount;

        public AbilitySession(AbilityData[] abilities, Action<int> selectEvent, Func<AbilityData[]> rerollEvent, int rerollCount)
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
