using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public static class AbilityBusyCounter
    {
        private static int _workingAbility;
        public static bool IsBusy => _workingAbility > 0;
        
        public static event Action OnWorkingAbility;
        public static event Action OnEndAllAbility;

        public static void RegisterWorkAbility()
        {
            _workingAbility++;
            OnWorkingAbility?.Invoke();
        }
        public static void UnRegisterWorkAbility()
        {
            _workingAbility--;

            if(_workingAbility == 0)
            {
                OnEndAllAbility?.Invoke();
            }
        }
        public static void Clear()
        {
            _workingAbility = 0;
            OnWorkingAbility = null;
            OnEndAllAbility = null;
        }
    }
}
