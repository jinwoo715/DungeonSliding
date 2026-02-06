using System;
using UnityEngine;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Core;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Enemy : Creature, IEnemyStatModifier, IRewardSender
    {
        [SerializeField] private Transform _statUITransform;

        private EnemyData _enemyData;
        [SerializeField] private EnemyStat _enemyStat;

        public event Action<Enemy> OnDeathEvent;
        public event Action<EEnemyStatType> OnStatChangedEvent;
        
        private float _backAttackMultiplier;

        public int EnemyUID => _enemyData.EnemyUID;
        public Transform StatUITransform => _statUITransform;

        public override void Init(ICombatEventListener combatEventListener, ECretureType cretureType)
        {
            base.Init(combatEventListener, cretureType);
         
            _backAttackMultiplier = GameManager.Configs.GameConfig.BackAttackDamageMultiplier;
        }
        public void SetData(EnemyData data, int floor)
        {
            IsActive = true;

            _enemyData = data;

            int RandomDir = UnityEngine.Random.Range(0, 4);
            SetCharacterRotation((EDirectionType)RandomDir);

            int hp = CalculateHP(_enemyData.BaseHP, floor);
            int dmg = CalculateDamage(_enemyData.BaseAttack, floor);
            int xp = CalculateXp(_enemyData.Xp, floor);

            _enemyStat = new EnemyStat(hp, dmg, xp);

            OnStatChangedEvent?.Invoke(EEnemyStatType.HP);
            OnStatChangedEvent?.Invoke(EEnemyStatType.Damage);
        }
        protected override DamageContext CalculateRealAppliedDamage(DamageContext damageInfo)
        {
            int damage = damageInfo.Damage;
            bool critical = damageInfo.IsCritical;

            if(IsBackAttack(damageInfo.Attacker))
            {
                GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.OnBackAttack);
                damage = (int)(damage * _backAttackMultiplier);
                critical = true;
            }

            DamageContext info = new DamageContext(damageInfo.Attacker, damage, critical);

            return info;
        }
        bool IsBackAttack(ICombatant attacker)
        {
            var behindTile = TilePosition.GetNextTile(ReverseDirection(Direction));
            return behindTile != null && attacker.TilePosition == behindTile;
        }
        public void ModifyStat(EnemyApplyStatContext context)
        {
            switch (context.EnemyStat)
            {
                case EEnemyStatType.HP:
                    
                    _enemyStat.HP += (int)context.Value;
                    _enemyStat.HP = Mathf.Max(0, _enemyStat.HP);

                    OnStatChangedEvent?.Invoke(EEnemyStatType.HP);

                    if (_enemyStat.HP <= 0)
                    {
                        OnDeath();
                    }

                    break;

                case EEnemyStatType.Damage:
                    _enemyStat.Damage += (int)context.Value;
                    OnStatChangedEvent?.Invoke(EEnemyStatType.Damage);

                    break;
            }

        }
        public int Get(EEnemyStatType stat)
        {
            int returnValue = 0;
            switch (stat)
            {
                case EEnemyStatType.HP:
                    returnValue = _enemyStat.HP;
                    break;
                case EEnemyStatType.Damage:
                    returnValue = _enemyStat.Damage;
                    break;
            }

            return returnValue;
        }
        protected override void ReduceHP(int damage)
        {
            ModifyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, -damage, EEnemyStatType.None));
        }
        public override void OnDeath()
        {
            base.OnDeath();
            GameTriggerEventBus.Instance?.ExcuteAbilityEvent(EGameTriggerType.OnKillEnemy);
            OnDeathEvent?.Invoke(this);
        }
        public override void StartAttackAnimation() { }
        protected override DamageContext CreateDamageContext()
        {
            DamageContext damageInfo = new DamageContext(this, Get(EEnemyStatType.Damage), false);
            return damageInfo;
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
            return new RewardData(_enemyStat.XP);
        }
        #endregion
    }
}