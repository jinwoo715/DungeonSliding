using JW.DungeonSliding.Core.Flow;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat 
{
    public class BattleManager : MonoBehaviour, IAttackRequestListener
    {
        private ICombatantSensor _combatSensor;

        private Queue<ActPair> _actPairs = new Queue<ActPair>();
        private Queue<ActPair> _counterActPairs = new Queue<ActPair>();
        private HashSet<(ICombatant, ICombatant)> _counterActs = new ();

        public void Init(ICombatantSensor combatantSensor)
        {
            _combatSensor = combatantSensor;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.OnMoveEnd, StartBattleSequence);
        }

        private void OnDisable()
        {
            GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameTriggerType.OnMoveEnd, StartBattleSequence);
        }

        public void StartBattleSequence()
        {
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnBattleStart);

            _combatSensor.PlayerCombatant.TrySubmitAttackRequest(_combatSensor, this);

            List<ICombatant> enemies = _combatSensor.AllEnemyCombatants;

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].TrySubmitAttackRequest(_combatSensor, this);
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

                    if (!_counterActs.Add((act.Attacker, act.Target)))
                        continue;
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

                act.Attacker.OnAttackDoneEvent += OnAttackEnd;
                act.Target.OnHitDoneEvent += OnHitEnd;
                act.Target.OnCounterEvent += EnqueueCounterActPair;

                act.Attacker.StartAttackAnimation();

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

                act.Attacker.OnAttackDoneEvent -= OnAttackEnd;
                act.Target.OnHitDoneEvent -= OnHitEnd;
            }

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnBattleEnd);

            _actPairs.Clear();
            _counterActPairs.Clear();
            _counterActs.Clear();

            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnTurnEnd);
        }

        public void EnqueueActPair(ActPair pair)
        {
            _actPairs.Enqueue(pair);
        }

        public void EnqueueCounterActPair(ActPair pair)
        {
            _counterActPairs.Enqueue(pair);
        }
    }
}