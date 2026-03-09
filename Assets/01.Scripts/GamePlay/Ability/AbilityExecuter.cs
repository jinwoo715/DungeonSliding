using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public struct AbilityArgs
    {
        public readonly EGameEventTrigger GameTrigger;
        public readonly ECreatureTrigger CreatureTrigger;

        public AbilityArgs(EGameEventTrigger gameTrigger, ECreatureTrigger creatureTrigger)
        {
            GameTrigger = gameTrigger;
            CreatureTrigger = creatureTrigger;
        }
    }

    public class AbilityExecuter : MonoBehaviour, IAbilityExcuter, IAbilityRegister
    {
        private Dictionary<EGameEventTrigger, List<IAbility>> _gameTriggerAbilities = new();
        private Dictionary<ECreatureTrigger, List<IAbility>> _creatureTriggerAbilities = new();

        public event Action OnEndCreatureAbility;
        public event Action OnEndGameEventAbility;

        #region Excute
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger)
        {
            
            if (_creatureTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                AbilityArgs args = new AbilityArgs(EGameEventTrigger.None, trigger);
                foreach (var ability in abilities)
                {
                    StartCoroutine(ability.Execute(args));
                }
            }

            OnEndCreatureAbility?.Invoke();
        }
        public void ExecuteCreatureTrigger<T>(ECreatureTrigger trigger, T data)
        {
            if (_creatureTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                AbilityArgs args = new AbilityArgs(EGameEventTrigger.None, trigger);

                foreach (var ability in abilities)
                {
                    if(ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(data);
                    }

                    StartCoroutine(ability.Execute(args));
                }
            }

            OnEndCreatureAbility?.Invoke();
        }
        public void ExecuteGameEventAbility(EGameEventTrigger trigger)
        {
            if (_gameTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                AbilityArgs args = new AbilityArgs(trigger, ECreatureTrigger.None);

                foreach (var ability in abilities)
                {
                    StartCoroutine(ability.Execute(args));
                }
            }
        }
        public void ExecuteGameEventAbility<T>(EGameEventTrigger trigger, T data)
        {
            if (_gameTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                AbilityArgs args = new AbilityArgs(trigger, ECreatureTrigger.None);
                foreach (var ability in abilities)
                {
                    if(ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(data);
                    }

                    StartCoroutine(ability.Execute(args));
                }
            }
        }
        #endregion


        #region Registration
        public void RegisterAbility(IAbility ability)
        {
            if (ability.CreatureTrigger != ECreatureTrigger.None)
            {
                RegisterCreatureEventAbility(ability.CreatureTrigger, ability);
            }

            if (ability.GameTrigger != EGameEventTrigger.None)
                RegisterGameEventAbility(ability.GameTrigger, ability);
        }
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability)
        {
            if (trigger == ECreatureTrigger.OnAdded)
            {
                AbilityArgs args = new AbilityArgs(EGameEventTrigger.None, trigger);

                ability.Execute(args);
                return;
            }

            if (!_creatureTriggerAbilities.ContainsKey(trigger))
            {
                _creatureTriggerAbilities.Add(trigger, new List<IAbility>());
            }

            _creatureTriggerAbilities[trigger].Add(ability);
        }
        public void RegisterGameEventAbility(EGameEventTrigger trigger, IAbility ability)
        {
            if (!_gameTriggerAbilities.ContainsKey(trigger))
            {
                _gameTriggerAbilities.Add(trigger, new List<IAbility>());

                GameTriggerEventBus.Instance.SubscribeTriggerEvent(trigger, () => ExecuteGameEventAbility(trigger));
            }

            _gameTriggerAbilities[trigger].Add(ability);
        }
        public void RegisterAutoAllAbility(List<IAbility> abilities)
        {
            foreach (var ability in abilities)
            {
                if(ability.CreatureTrigger != ECreatureTrigger.None)
                    RegisterCreatureEventAbility(ability.CreatureTrigger, ability);

                if (ability.GameTrigger != EGameEventTrigger.None)
                    RegisterGameEventAbility(ability.GameTrigger, ability);
            }
        }
        #endregion

        public void Clear()
        {
            _creatureTriggerAbilities.Clear();

            var keys = _gameTriggerAbilities.Keys;

            foreach (var key in keys)
            {
                foreach (var ability in _gameTriggerAbilities[key])
                {
                    ability.ReleaseAbility();
                }

                GameTriggerEventBus.Instance.SubscribeTriggerEvent(key, () => ExecuteGameEventAbility(key));
            }

            _gameTriggerAbilities.Clear();

            foreach (var abilities in _creatureTriggerAbilities)
            {
                foreach (var ability in abilities.Value)
                {
                    ability.ReleaseAbility();
                }
            }

            _creatureTriggerAbilities.Clear();
        }

        
    }
}
