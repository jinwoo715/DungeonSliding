using JW.DungeonSliding.Core.Flow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat 
{
    public class BattleManager : MonoBehaviour, IAttackRequestListener, IRequesterRegistry
    {
        //TODO BattleManager
        private ICombatantSensor _combatSensor;

        private IAttackRequester _playerRequester;
        private List<IAttackRequester> _enemyRequesters = new();

        private Queue<ActPair> _actPairs = new Queue<ActPair>();
        private Queue<ActPair> _counterActPairs = new Queue<ActPair>();

        public void Init(ICombatantSensor combatantSensor)
        {
            _combatSensor = combatantSensor;
        }

        private void OnDisable()
        {
            //GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameEventTriggerType.OnMoveEnd, StartBattleSequence);
        }

        public void StartBattleSequence()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnBattleStart);

            List<IAttackRequester> requesters = new List<IAttackRequester>();

            requesters.Add(_playerRequester);
            requesters.AddRange(_enemyRequesters);

            for (int i = 0; i < requesters.Count; i++)
            {
                requesters[i].TrySubmitAttackRequest(_combatSensor);
            }

            StartCoroutine(CoStartSequence());
        }
        private IEnumerator CoStartSequence()
        {
            while (_actPairs.Count > 0 || _counterActPairs.Count > 0)
            {
                ActPair act;
                if (_counterActPairs.Count > 0)
                {
                    act = _counterActPairs.Dequeue();
                }
                else
                {
                    act = _actPairs.Dequeue();
                }


                if (act.Attacker.IsActive == false || act.Target.IsActive == false)
                    continue;

                bool isAttackDone = false;
                bool isHitDone = false;

                void OnAttackEnd() => isAttackDone = true;
                void OnHitEnd() => isHitDone = true;

                act.Attacker.OnAttackSequenceEnd += OnAttackEnd;
                act.Target.OnHitSequenceEnd += OnHitEnd;

                act.Attacker.ExcuteAttack(act);

                float timer = 0;
                const float timeOut = 3.0f;

                while (timer < timeOut)
                {
                    if(isAttackDone && isHitDone)
                    {
                        break;
                    }
                    else
                    {
                        timer += Time.deltaTime;
                        yield return null;
                    }
                }

                if(!isAttackDone || !isHitDone)
                {
                    Debug.LogError($"Time Out Battle, Attack : {isAttackDone}, Hit : {isHitDone}");
                }

                act.Attacker.OnAttackSequenceEnd -= OnAttackEnd;
                act.Target.OnHitSequenceEnd -= OnHitEnd;
            }

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnBattleEnd);

            _actPairs.Clear();
            _counterActPairs.Clear();

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnTurnEnd);
        }
        public void EnqueueActPair(ActPair pair)
        {
            _actPairs.Enqueue(pair);
        }
        public void EnqueueCounterActPair(ActPair pair)
        {
            _counterActPairs.Enqueue(pair);
        }

        public void RegisterEnemyAttackRequester(IAttackRequester requester)
        {
            _enemyRequesters.Add(requester);
            requester.OnRequestAttack += EnqueueActPair;
            requester.OnRequestCounterAttack += EnqueueCounterActPair;
        }

        public void UnRegisterEnemyAttackRequester(IAttackRequester requester)
        {
            _enemyRequesters.Remove(requester);
            requester.OnRequestAttack -= EnqueueActPair;
            requester.OnRequestCounterAttack -= EnqueueCounterActPair;
        }

        public void RegisterPlayerAttackRequester(IAttackRequester requester)
        {
            _playerRequester = requester;
            requester.OnRequestAttack += EnqueueActPair;
            requester.OnRequestCounterAttack += EnqueueCounterActPair;
        }

        public void UnRegisterPlayerAttackRequester(IAttackRequester requester)
        {
            _playerRequester = null;
            requester.OnRequestAttack -= EnqueueActPair;
            requester.OnRequestCounterAttack -= EnqueueCounterActPair;
        }
    }
}