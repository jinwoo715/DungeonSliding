using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Stats
{
    public struct CreatureBaseStat
    {
        public readonly int HP;
        public readonly int Damage;
        public readonly int Move;

        public CreatureBaseStat(int hp, int damage, int move)
        {
            HP = hp;
            Damage = damage;
            Move = move;
        }
    }


    public class CreatureStat : IStatReadOnly, IStatModifier
    {
        Dictionary<ECreatureStatType, StatValue> _stats = new();
        private HashSet<ECreatureStatType> _evaluatingStats = new();

        private int _currentHP;
        private int _currentMove;

        private int _damageDealMultiplier;
        private int _damageTakeMultiplier;

        public event Action<ECreatureStatType> OnStatChanged;
       
        public void Init(CreatureBaseStat baseStat)
        {
            _stats.Add(ECreatureStatType.MaxHp, new StatValue(baseStat.HP));
            _stats.Add(ECreatureStatType.Damage, new StatValue(baseStat.Damage));
            _stats.Add(ECreatureStatType.MaxMoveCount, new StatValue(baseStat.Move));

            _currentHP = baseStat.HP;
            _currentMove = baseStat.Move;

            _damageDealMultiplier = 1;
            _damageTakeMultiplier = 1;

            OnStatChanged?.Invoke(ECreatureStatType.CurrentHP);
            OnStatChanged?.Invoke(ECreatureStatType.MaxHp);
            OnStatChanged?.Invoke(ECreatureStatType.Damage);
            OnStatChanged?.Invoke(ECreatureStatType.CurrentMoveCount);
            OnStatChanged?.Invoke(ECreatureStatType.MaxMoveCount);
        }

        public int Get(ECreatureStatType stat)
        {
            if (stat == ECreatureStatType.CurrentHP) return _currentHP;
            if (stat == ECreatureStatType.CurrentMoveCount) return _currentMove;
            if (stat == ECreatureStatType.DamageTakeMultiplier) return _damageTakeMultiplier;
            if (stat == ECreatureStatType.DamageDealtMultiplier) return _damageDealMultiplier;
            if (_stats.TryGetValue(stat, out StatValue value))
            {
                // 1. 이미 계산 중인 스탯을 또 물어본다면? (순환 참조 발견!)
                if (_evaluatingStats.Contains(stat))
                {
                    Debug.LogError($"[치명적 에러] 스탯 순환 참조가 발생했습니다! 무한 루프를 방지하기 위해 0을 반환합니다. 원인 스탯: {stat}");
                    return 0; // 또는 value.Base 등 기본값만 반환시켜 루프를 끊음
                }

                _evaluatingStats.Add(stat);

                int finalResult = value.Final(this);

                _evaluatingStats.Remove(stat);

                return finalResult;
            }

            return 0;
        }

        //TODO 메서드 나눠야함
        public void ModifyStat(StatModifierContext modifierContext)
        {
            Debug.Log(modifierContext.ModifyType);
            Debug.Log(modifierContext.TargetStat);
            if (modifierContext.TargetStat == ECreatureStatType.CurrentHP)
            {
                _currentHP += Mathf.RoundToInt(modifierContext.Value);

                int maxHp = Get(ECreatureStatType.MaxHp);
                _currentHP = Mathf.Clamp(_currentHP, 0, maxHp);
            }
            else if (modifierContext.TargetStat == ECreatureStatType.CurrentMoveCount)
            {
                _currentMove += Mathf.RoundToInt(modifierContext.Value);

                int maxMove = Get(ECreatureStatType.MaxMoveCount);
                _currentMove = Mathf.Clamp(_currentMove, 0, maxMove);
            }
            else if (modifierContext.TargetStat == ECreatureStatType.DamageTakeMultiplier)
            {
                _damageTakeMultiplier += Mathf.RoundToInt(modifierContext.Value);

            }
            else if (modifierContext.TargetStat == ECreatureStatType.DamageDealtMultiplier)
            {
                _damageDealMultiplier += Mathf.RoundToInt(modifierContext.Value);
            }
            else
            {
                if (_stats.TryGetValue(modifierContext.TargetStat, out StatValue value))
                {
                    value.ModifiyStat(modifierContext);

                    if (modifierContext.TargetStat == ECreatureStatType.MaxHp)
                    {
                        _currentHP = Mathf.Clamp(_currentHP, 0, Get(ECreatureStatType.MaxHp));
                        OnStatChanged?.Invoke(ECreatureStatType.CurrentHP); // 현재 체력도 변했다고 방송!
                    }
                    else if (modifierContext.TargetStat == ECreatureStatType.MaxMoveCount)
                    {
                        _currentMove = Mathf.Clamp(_currentMove, 0, Get(ECreatureStatType.MaxMoveCount));
                        OnStatChanged?.Invoke(ECreatureStatType.CurrentMoveCount);
                    }
                }
            }

            OnStatChanged?.Invoke(modifierContext.TargetStat);
        }

        public void Clear()
        {
            foreach (var stat in _stats)
            {
                stat.Value.Clear();
            }

            _currentHP = 0;
            _currentMove = 0;

            _damageDealMultiplier = 1;
            _damageTakeMultiplier = 1;
        }
    }
}
