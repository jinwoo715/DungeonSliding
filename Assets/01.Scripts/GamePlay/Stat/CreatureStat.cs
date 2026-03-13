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
       
        public CreatureStat()
        {
            _stats.Add(ECreatureStatType.MaxHp, new StatValue(0));
            _stats.Add(ECreatureStatType.Damage, new StatValue(0));
            _stats.Add(ECreatureStatType.MaxMoveCount, new StatValue(0));
        }

        public void Init(CreatureBaseStat baseStat)
        {
            Clear();

            _stats[ECreatureStatType.MaxHp].SetBase(baseStat.HP);
            _stats[ECreatureStatType.Damage].SetBase(baseStat.Damage);
            _stats[ECreatureStatType.MaxMoveCount].SetBase(baseStat.Move);

            _currentHP = baseStat.HP;
            _currentMove = baseStat.Move;

            _damageDealMultiplier = 100;
            _damageTakeMultiplier = 100;

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
            if (modifierContext.TargetStat == ECreatureStatType.CurrentHP)
            {
                ModifyCurrentHp(modifierContext);
            }
            else if (modifierContext.TargetStat == ECreatureStatType.CurrentMoveCount)
            {
                ModifyCurrentMoveCount(modifierContext);
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
                    int beforeValue = value.Final(this);

                    value.ModifiyStat(modifierContext);

                    int afterValue = value.Final(this);
                    int diffValue = afterValue - beforeValue;

                    if (modifierContext.TargetStat == ECreatureStatType.MaxHp)
                    {
                        _currentHP += diffValue;
                        _currentHP = Mathf.Clamp(_currentHP, 0, Get(ECreatureStatType.MaxHp));
                        OnStatChanged?.Invoke(ECreatureStatType.CurrentHP);
                    }
                    else if (modifierContext.TargetStat == ECreatureStatType.MaxMoveCount)
                    {
                        _currentMove += diffValue;
                        _currentMove = Mathf.Clamp(_currentMove, 0, Get(ECreatureStatType.MaxMoveCount));
                        OnStatChanged?.Invoke(ECreatureStatType.CurrentMoveCount);
                    }
                }
            }

            OnStatChanged?.Invoke(modifierContext.TargetStat);
        }
        private int CalculateFixedAddValue(StatModifierContext modifierContext)
        {
            int calValue = 0;

            switch (modifierContext.ModifyType)
            {
                case EApplyStatType.Add:
                    calValue += Mathf.RoundToInt(modifierContext.Value);

                    break;
                case EApplyStatType.Multiple:
                    Debug.LogError($"Current Stat Modify Type Error");
                    break;

                case EApplyStatType.Ratio:

                    int baseValue = Get(modifierContext.RatioBaseStat);
                    int resultValue = Mathf.RoundToInt(baseValue * modifierContext.Value);
                    calValue += resultValue;

                    break;
            }

            return calValue;
        }
        private void ModifyCurrentHp(StatModifierContext modifierContext)
        {
            _currentHP += CalculateFixedAddValue(modifierContext);

            int maxHp = Get(ECreatureStatType.MaxHp);
            _currentHP = Mathf.Clamp(_currentHP, 0, maxHp);
        }
        private void ModifyCurrentMoveCount(StatModifierContext modifierContext)
        {
            _currentMove += CalculateFixedAddValue(modifierContext);

            int maxMove = Get(ECreatureStatType.MaxMoveCount);
            _currentMove = Mathf.Clamp(_currentMove, 0, maxMove);
        }

        public void Clear()
        {
            foreach (var stat in _stats)
            {
                stat.Value.Clear();
            }

            _currentHP = 0;
            _currentMove = 0;

            _damageDealMultiplier = 100;
            _damageTakeMultiplier = 100;
        }
    }
}
