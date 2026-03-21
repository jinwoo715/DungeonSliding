using System;
using UnityEngine;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Core;
using System.Collections.Generic;
using JW.DungeonSliding.GamePlay.Ability;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Enemy : Creature, IRewardSender
    {
        [SerializeField] private Transform _statUITransform;
        [SerializeField] private EnemyStat _enemyStat;

        [SerializeField] private EnemyData _enemyData;

        public event Action<EEnemyStatType> OnStatChangedEvent;
        public string Name => _enemyData.Name;
        public string Description => _enemyData.Description;
        public string EnemyUID => _enemyData.UID;
        public Transform StatUITransform => _statUITransform;

        public event Action<Enemy> OnEnemyReturnEvent;

        private int _xp;

        public override void Initialize(ECreatureType cretureType)
        {
            base.Initialize(cretureType);
        }
        
        public void SetData(EnemyData data, int floor)
        {
            IsActive = true;

            _enemyData = data;

            int RandomDir = UnityEngine.Random.Range(0, 4);
            Rotate.SetRotation((EDirectionType)RandomDir);

            int hp = CalculateHP(_enemyData.BaseHP, floor);
            int dmg = CalculateDamage(_enemyData.BaseDamage, floor);
            int xp = CalculateXp(_enemyData.BaseXP, floor);
            _xp = CalculateXp(_enemyData.BaseXP, floor);

            CreatureBaseStat stat = new CreatureBaseStat(hp, dmg, 100);
            InitData(stat);

            OnStatChangedEvent?.Invoke(EEnemyStatType.HP);
            OnStatChangedEvent?.Invoke(EEnemyStatType.Damage);
        }

        public override void OnDeath()
        {
            base.OnDeath();
            OnEnemyReturnEvent?.Invoke(this);
        }

        #region Calculate Stat
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
        public RewardData CreateReward()
        {
            return new RewardData(ERewardType.KillReward, _xp);
        }
        #endregion
    }
}