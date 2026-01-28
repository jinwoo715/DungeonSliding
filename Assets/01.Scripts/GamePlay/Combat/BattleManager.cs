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

        private IGameModeChanger _gameModeChanger;

        private Queue<ActPair> actPairs = new Queue<ActPair>();

        public void Init(ICombatantSensor combatantSensor, IGameModeChanger gameModeChanger)
        {
            _combatSensor = combatantSensor;
            _gameModeChanger = gameModeChanger;
        }
        
        public void StartBattleSequence()
        {
            _gameModeChanger.EnterGameMode(EGameModeType.Battle);
            _combatSensor.PlayerCombatant.RegisterAttack();

            List<ICombatant> enemies = _combatSensor.AllEnemyCombatants;

            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].RegisterAttack();
            }

            StartCoroutine(CoStartSequence());
        }
        private IEnumerator CoStartSequence()
        {
            while (actPairs.Count > 0)
            {
                ActPair act = actPairs.Dequeue();

                if (act.Attacker.IsActive == false || act.Target.IsActive == false)
                    continue;

                bool isAttackDone = false;
                bool isHitDone = false;

                void OnAttackEnd() => isAttackDone = true;
                void OnHitEnd() => isHitDone = true;

                act.Attacker.OnAttackDoneEvent += OnAttackEnd;
                act.Target.OnHitDoneEvent += OnHitEnd;

                act.Attacker.Attack(act.Target);

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

                Debug.Log("End Sequence");
            }

            _gameModeChanger.ExitGameMode(EGameModeType.Battle);
        }

        public void RegisterActpair(ActPair pair)
        {
            actPairs.Enqueue(pair);
        }
    }
}