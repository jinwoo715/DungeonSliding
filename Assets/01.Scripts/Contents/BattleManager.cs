using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.SlidingPuzzle {
    public class BattleManager : MonoBehaviour
    {
        private ICombatant _playerCombatant;
        private ICombatProvider _enemyCombatProvider;

        private Queue<ActPair> actPairs = new Queue<ActPair>();

        public struct ActPair
        {
            public ActPair(ICombatant attacker, ICombatant target)
            {
                Attacker = attacker;
                Target = target;
            }

            public ICombatant Attacker;
            public ICombatant Target;
        }

        public void SetPlayerCombatant(ICombatant player) => _playerCombatant = player;
        public void SetCombatProvider(ICombatProvider combatProvider) => _enemyCombatProvider = combatProvider;

        public void StartBattleSequence()
        {
            GameSceneManager.Instance.EnterGameFlow(EGameFlowType.Battle);

            TilePoint playerFowardTile = _playerCombatant.Point.GetNextTile(_playerCombatant.Direction);
            
            if(_enemyCombatProvider.TryGetCombatant(playerFowardTile, out ICombatant combatant))
            {
                actPairs.Enqueue(new ActPair(_playerCombatant, combatant));
            }

            List<ICombatant> enemies = _enemyCombatProvider.GetAllCombatant();

            for (int i = 0; i < enemies.Count; i++)
            {
                TilePoint enemyForwardTile = enemies[i].Point.GetNextTile(enemies[i].Direction);

                if(enemyForwardTile == _playerCombatant.Point)
                {
                    actPairs.Enqueue(new ActPair(enemies[i], _playerCombatant));
                }
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

            GameSceneManager.Instance.ExitGameFlow(EGameFlowType.Battle);
        }
    }
}