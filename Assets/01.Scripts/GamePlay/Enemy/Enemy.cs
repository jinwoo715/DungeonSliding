using System;
using UnityEngine;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Enemy : Creature, IEnemyStatReadOnly
    {
        [SerializeField] private Transform _statUITransform;

        public Action<Enemy> ReturnEvent;
        public Action<Enemy> OnDeathEvent;

        private EnemyData _enemyData;
        private int _rewardXp = 0;

        [SerializeField] private CretureStat _baseStat;

        public int Xp => _rewardXp;
        public int EnemyUID => _enemyData.EnemyUID;

        public Transform UITransform => _statUITransform;

        public event Action<EEnemyStatType> OnStatChanged;

        public void SetData(EnemyData data, int floor)
        {
            _enemyData = data;

            int hp = CalculateHP(_enemyData.BaseHP, floor);
            int dmg = CalculateDamage(_enemyData.BaseDamage, floor);
            _rewardXp = CalculateXp(_enemyData.Xp, floor);

            SetCretureStat(new CretureStat(hp, dmg));
        }

        public virtual void SetCretureStat(CretureStat stat)
        {
            _baseStat = stat;
            _currentHP = _baseStat.HP;
            OnStatChanged?.Invoke(EEnemyStatType.HP);
            OnStatChanged?.Invoke(EEnemyStatType.Damage);
        }
        public override void Init()
        {
            base.Init();
            int RandomDir = UnityEngine.Random.Range(0, 4);
            SetCharacterRotation((EDirectionType)RandomDir);
        }
        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            int damage = damageInfo.Damage;
            bool critical = damageInfo.IsCritical;

            if(damageInfo.Attacker.Direction == this.Direction)
            {
                damage *= 2;
                critical = true;
            }

            DamageContext info = new DamageContext(damageInfo.Attacker, damage, critical);

            return info;
        }
        public int CalculateHP(int baseHP, int floor)
        { 
            return ScaleStat(baseHP, ConstData.ENEMY_HP_POW, floor, ceil: true);   // HP는 올림 추천
        }
        public int CalculateDamage(int baseDamage, int floor)
        {
            return ScaleStat(baseDamage, ConstData.ENEMY_DMG_POW, floor);
        }       
        public int CalculateXp(int baseXp, int floor)
        {
            return ScaleStat(baseXp, ConstData.ENEMY_XP_POW, floor);               // XP도 반올림
        }
        public int ScaleStat(int baseValue, float pow, int floor, bool ceil = false)
        {
            if (floor <= 1) return baseValue;

            float v = baseValue * Mathf.Pow(pow, floor - 1);

            int scaled = ceil ? Mathf.CeilToInt(v) : Mathf.RoundToInt(v);
            return Mathf.Max(baseValue, scaled);
        }

        public override bool TrySubmitAttackRequest()
        {
            if (_sensor.GetCombatant(TilePosition.GetNextTile(Direction), ECretureType.Player, out var target))
            {
                AttackRequestListener.EnqueueActPair(new ActPair(this, target));
                return true;
            }
            else return false;
        }

        public void ModifyStat(EnemyApplyStatContext context)
        {
            switch (context.EnemyStat)
            {
                case EEnemyStatType.HP:
                    _currentHP += (int)context.Value;

                    _currentHP = Mathf.Clamp(_currentHP, 0, _baseStat.HP);

                    OnStatChanged?.Invoke(EEnemyStatType.HP);

                    if (_currentHP <= 0)
                    {
                        OnDeath();
                    }

                    break;
                case EEnemyStatType.Damage:
                    _baseStat.Damage += (int)context.Value;
                    OnStatChanged?.Invoke(EEnemyStatType.Damage);

                    break;
            }

        }

        public int Get(EEnemyStatType stat)
        {
            int returnValue = 0;
            switch (stat)
            {
                case EEnemyStatType.HP:
                    returnValue = _currentHP;
                    break;
                case EEnemyStatType.Damage:
                    returnValue = _baseStat.Damage;
                    break;
            }

            return returnValue;
        }

        protected override DamageContext CreateDamageContext()
        {
            DamageContext damageInfo = new DamageContext(this, Get(EEnemyStatType.Damage), false);
            return damageInfo;
        }

        public override void ReduceHP(int damage)
        {
            ModifyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, -damage, EEnemyStatType.None));
        }
    }
}