using JW.DungeonSliding.Core.Flow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat 
{
    public interface IBattleResult
    {
        BattleResultPayload GatBattleResult();
        bool IsBattleTurn();
    }
    public struct BattleResultPayload
    {
        public int CombatCount;
        public BattleResultPayload(int combatCount)
        {
            CombatCount = combatCount;
        }
    }

    public class BattleManager : MonoBehaviour, IAttackRequestListener, IRequesterRegistry, IBattleResult
    {
        //TODO BattleManager
        private ICombatantSensor _combatSensor;

        private SortedDictionary<int, List<IAttackRequester>> _requesterByPriority = new();

        private Queue<ActPair> _actPairs = new Queue<ActPair>();
        private Queue<ActPair> _counterActPairs = new Queue<ActPair>();

        private int _combatCount = 0;

        public void Init(ICombatantSensor combatantSensor)
        {
            _combatSensor = combatantSensor;
        }
        public void StartBattleSequence()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameEventTrigger.OnBattleStart);

            foreach (var requesters in _requesterByPriority)
            {
                List<IAttackRequester> requestList = requesters.Value;

                for (int i = 0; i < requestList.Count; i++)
                {
                    requestList[i].TrySubmitAttackRequest(_combatSensor);
                }
            }

            StartCoroutine(CoStartSequence());
        }
        private IEnumerator CoStartSequence()
        {
            while (_actPairs.Count > 0 || _counterActPairs.Count > 0)
            {
                _combatCount++;

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
            _combatCount = 0;
        }
        public void EnqueueActPair(ActPair pair)
        {
            _actPairs.Enqueue(pair);
        }
        public void EnqueueCounterActPair(ActPair pair)
        {
            _counterActPairs.Enqueue(pair);
        }
        public void RegisterAttackRequester(IAttackRequester requester, int priority)
        {
            if (!_requesterByPriority.ContainsKey(priority))
                _requesterByPriority.Add(priority, new List<IAttackRequester>());

            _requesterByPriority[priority].Add(requester);

            requester.OnRequestAttack += EnqueueActPair;
            requester.OnRequestCounterAttack += EnqueueCounterActPair;
        }
        public void UnRegisterAttackRequester(IAttackRequester requester, int priority)
        {
            if (_requesterByPriority.TryGetValue(priority, out var list))
            {
                if (list.Contains(requester))
                {
                    list.Remove(requester);
                    requester.OnRequestAttack -= EnqueueActPair;
                    requester.OnRequestCounterAttack -= EnqueueCounterActPair;
                }
            }
        }

        public BattleResultPayload GatBattleResult()
        {
            return new BattleResultPayload(_combatCount);
        }
        public bool IsBattleTurn()
        {
            return _combatCount != 0;
        }
    }
}