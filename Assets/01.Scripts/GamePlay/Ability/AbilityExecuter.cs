using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class AbilityExecuter : MonoBehaviour, IAbilityExcuter, IAbilityRegister
    {
        private Dictionary<EGameTriggerType, List<IAbility>> _gameTriggerAbilities = new();
        private Dictionary<ECreatureTrigger, List<IAbility>> _creatureTriggerAbilities = new();

        public event Action OnEndCreatureAbility;
        public event Action OnEndGameEventAbility;

        #region Excute
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger)
        {
            if (_creatureTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                foreach (var ability in abilities)
                {
                    StartCoroutine(ability.Excute());
                }
            }

            OnEndCreatureAbility?.Invoke();
        }

        public void ExecuteCreatureTrigger<T>(ECreatureTrigger trigger, T data)
        {
            if (_creatureTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                foreach (var ability in abilities)
                {
                    if(ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(data);
                    }

                    StartCoroutine(ability.Excute());
                }
            }

            OnEndCreatureAbility?.Invoke();
        }

        public void ExecuteGameEventAbility(EGameTriggerType trigger)
        {
            if (_gameTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                foreach (var ability in abilities)
                {
                    StartCoroutine(ability.Excute());
                }
            }
        }
        public void ExecuteGameEventAbility<T>(EGameTriggerType trigger, T data)
        {
            if (_gameTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                foreach (var ability in abilities)
                {
                    if(ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(data);
                    }

                    StartCoroutine(ability.Excute());
                }
            }
        }

        #endregion


        #region Registration
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability)
        {
            if (!_creatureTriggerAbilities.ContainsKey(trigger))
            {
                _creatureTriggerAbilities.Add(trigger, new List<IAbility>());
            }

            _creatureTriggerAbilities[trigger].Add(ability);
        }
        public void RegisterGameEventAbility(EGameTriggerType trigger, IAbility ability)
        {
            if (!_gameTriggerAbilities.ContainsKey(trigger))
            {
                _gameTriggerAbilities.Add(trigger, new List<IAbility>());

                GameTriggerEventBus.Instance.SubscribeTriggerEvent(trigger, () => ExecuteGameEventAbility(trigger));
            }

            _gameTriggerAbilities[trigger].Add(ability);
        }
        #endregion

        public void Clear()
        {
            _creatureTriggerAbilities.Clear();

            var keys = _gameTriggerAbilities.Keys;

            foreach (var key in keys)
            {
                GameTriggerEventBus.Instance.SubscribeTriggerEvent(key, () => ExecuteGameEventAbility(key));
            }

            _gameTriggerAbilities.Clear();
        }
    }
}
