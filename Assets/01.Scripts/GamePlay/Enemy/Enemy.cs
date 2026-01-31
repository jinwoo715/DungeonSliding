using System;
using UnityEngine;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class Enemy : Creature, IEnemyStatReadOnly
    {
        public Action<Enemy> ReturnEvent;
        public Action<Enemy> OnDeathEvent;

        private EnemyData _enemyData;
        private int _rewardXp = 0;

        [SerializeField] private CretureStat _baseStat;

        public int Xp => _rewardXp;
        public int EnemyUID => _enemyData.EnemyUID;

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
        }
        public override void Init()
        {
            base.Init();
            int RandomDir = UnityEngine.Random.Range(0, 4);
            SetCharacterRotation((EDirectionType)RandomDir);
        }
        protected override DamageInfo CalculateRealAppliedDamage(DamageInfo damageInfo)
        {
            int damage = damageInfo.Damage;
            bool critical = damageInfo.IsCritical;

            if(damageInfo.Attacker.Direction == this.Direction)
            {
                damage *= 2;
                critical = true;
            }

            DamageInfo info = new DamageInfo(damageInfo.Attacker, damage, critical);

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

        public override void RegisterAttack()
        {
            if (_sensor.GetCombatant(TilePosition.GetNextTile(Direction), ECretureType.Player, out var target))
            {
                _attackRequestListener.RegisterActpair(new ActPair(this, target));
            }
        }

        public override void ModifyStat(ApplyStatContext context)
        {
            switch (context.PlayerStat)
            {
                case EPlayerStat.HP:
                    _currentHP += (int)context.Value;
                    OnStatChanged?.Invoke(EEnemyStatType.HP);
                    break;
                case EPlayerStat.Damage:
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

        protected override DamageInfo CreateDamageInfo()
        {
            DamageInfo damageInfo = new DamageInfo(this, Get(EEnemyStatType.Damage), false);
            return damageInfo;
        }
    }
}