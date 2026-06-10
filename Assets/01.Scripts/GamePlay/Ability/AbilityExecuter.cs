using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections;
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

    public class AbilityExecuter : MonoBehaviour, IAbilityExcuter, IAbilityRegister, IAbilityPayloadSender
    {
        private Dictionary<EGameEventTrigger, List<IAbility>> _gameTriggerAbilities = new();
        private Dictionary<ECreatureTrigger, List<IAbility>> _creatureTriggerAbilities = new();
        private readonly HashSet<IAbility> _registeredAbilities = new();

        private int _workingAbilityCount = 0;

        public event Action OnEndCreatureAbility;
        public event Action OnEndGameEventAbility;

        private void OnDisable()
        {
            Clear();
        }

        #region Excute
        public void ExecuteCreatureTrigger(ECreatureTrigger trigger)
        {
            if (_creatureTriggerAbilities.TryGetValue(trigger, out var abilities))
            {
                AbilityArgs args = new AbilityArgs(EGameEventTrigger.None, trigger);
                foreach (var ability in abilities)
                {
                    StartCoroutine(RunAbilityWithCounter(ability, args));
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
                    StartCoroutine(RunAbilityWithCounter(ability, args));
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
                    StartCoroutine(RunAbilityWithCounter(ability, args));
                }
            }

            OnEndGameEventAbility?.Invoke();
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

                    StartCoroutine(RunAbilityWithCounter(ability, args));
                }
            }
            OnEndGameEventAbility?.Invoke();
        }

        private IEnumerator RunAbilityWithCounter(IAbility ability, AbilityArgs args)
        {
            Console.WriteLine(ability);

            _workingAbilityCount++;
            AbilityBusyCounter.RegisterWorkAbility(); // 1. 카운터 증가

            yield return StartCoroutine(ability.Execute(args));

            AbilityBusyCounter.UnRegisterWorkAbility(); // 3. 완료 후 카운터 감소
            _workingAbilityCount--;
        }

        #endregion

        #region Registration
        public void RegisterAbility(IAbility ability)
        {
            if (ability == null)
                return;

            _registeredAbilities.Add(ability);

            if (ability.CreatureTrigger != ECreatureTrigger.None)
                RegisterCreatureEventAbility(ability.CreatureTrigger, ability);

            if (ability.GameTrigger != EGameEventTrigger.None)
                RegisterGameEventAbility(ability.GameTrigger, ability);
        }
        public void RegisterCreatureEventAbility(ECreatureTrigger trigger, IAbility ability)
        {
            if (trigger == ECreatureTrigger.OnAdded)
            {
                AbilityArgs args = new AbilityArgs(EGameEventTrigger.None, trigger);
                StartCoroutine(ability.Execute(args));
                return;
            }

            if (!_creatureTriggerAbilities.ContainsKey(trigger))
            {
                _creatureTriggerAbilities.Add(trigger, new List<IAbility>());
            }

            _creatureTriggerAbilities[trigger].Add(ability);
        }

        List<(EGameEventTrigger, Action)> GameActions = new List<(EGameEventTrigger, Action)>();

        public void RegisterGameEventAbility(EGameEventTrigger trigger, IAbility ability)
        {
            if (!_gameTriggerAbilities.ContainsKey(trigger))
            {
                _gameTriggerAbilities.Add(trigger, new List<IAbility>());

                GameActions.Add((trigger, GameTriggerEventBus.Instance.SubscribeTriggerEvent(trigger, () => ExecuteGameEventAbility(trigger))));
            }

            _gameTriggerAbilities[trigger].Add(ability);
        }
        public void RegisterAutoAllAbility(List<IAbility> abilities)
        {
            foreach (var ability in abilities)
            {
                if (ability == null)
                    continue;

                _registeredAbilities.Add(ability);

                if (ability.CreatureTrigger != ECreatureTrigger.None)
                {
                    // EGameEventTrigger에 정의된 모든 Enum 값을 순회
                    foreach (ECreatureTrigger triggerFlag in Enum.GetValues(typeof(ECreatureTrigger)))
                    {
                        // None 값은 등록할 필요가 없으므로 패스
                        if (triggerFlag == ECreatureTrigger.None)
                            continue;

                        // 합쳐진 GameTrigger 안에 현재 순회 중인 triggerFlag가 포함되어 있는지 검사
                        if (ability.CreatureTrigger.HasFlag(triggerFlag))
                        {
                            RegisterCreatureEventAbility(triggerFlag, ability);
                        }
                    }
                }


                if (ability.GameTrigger != EGameEventTrigger.None)
                {
                    // EGameEventTrigger에 정의된 모든 Enum 값을 순회
                    foreach (EGameEventTrigger triggerFlag in Enum.GetValues(typeof(EGameEventTrigger)))
                    {
                        // None 값은 등록할 필요가 없으므로 패스
                        if (triggerFlag == EGameEventTrigger.None)
                            continue;

                        // 합쳐진 GameTrigger 안에 현재 순회 중인 triggerFlag가 포함되어 있는지 검사
                        if (ability.GameTrigger.HasFlag(triggerFlag))
                        {
                            RegisterGameEventAbility(triggerFlag, ability);
                        }
                    }
                }
            }
        }
        #endregion

        public void SendPayload<T>(T payload)
        {
            foreach (var gameAbility in _gameTriggerAbilities)
            {
                foreach (var ability in gameAbility.Value)
                {
                    if (ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(payload);
                    }
                }
            }

            foreach (var creatureAbility in _creatureTriggerAbilities)
            {
                foreach (var ability in creatureAbility.Value)
                {
                    if (ability is IAbilityPayloadReceiver<T> receiver)
                    {
                        receiver.ReceivePayload(payload);
                    }
                }
            }
        }

        public void Clear()
        {
            StopAllCoroutines();

            for (int i = 0; i < _workingAbilityCount; i++)
            {
                AbilityBusyCounter.UnRegisterWorkAbility();
            }

            _workingAbilityCount = 0;

            if (GameTriggerEventBus.Instance != null)
            {
                foreach (var action in GameActions)
                {
                    GameTriggerEventBus.Instance.UnSubscribeTriggerEvent(action.Item1, action.Item2);
                }
            }

            GameActions.Clear();
            _gameTriggerAbilities.Clear();
            _creatureTriggerAbilities.Clear();

            foreach (var ability in _registeredAbilities)
            {
                ability.ReleaseAbility();
            }

            _registeredAbilities.Clear();
        }
    }
}
