using System;
using UnityEngine;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Core;
using System.Collections.Generic;

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
        public string EnemyUID => _enemyData.UID;
        public Transform StatUITransform => _statUITransform;

       

        //TODO Enemy Skill 구현
        #region Enemy Skill
        private Dictionary<EGameTriggerType, List<IEnemyAbility>> gameTriggerAbilities = new();
        private Dictionary<ECreatureTrigger, List<IEnemyAbility>> creatureTriggerAbilities = new ();

        public void SetAbility(List<IEnemyAbility> abilities)
        {
            if (abilities == null) return;

            foreach (var ability in abilities)
            {
                if(ability.GameTrigger != EGameTriggerType.None)
                {
                    if (!gameTriggerAbilities.ContainsKey(ability.GameTrigger))
                    {
                        gameTriggerAbilities.Add(ability.GameTrigger, new List<IEnemyAbility>());

                        GameTriggerEventBus.Instance.SubscribeTriggerEvent(ability.GameTrigger, () => ExcuteGameTriggerAbility(ability.GameTrigger));
                    }

                    gameTriggerAbilities[ability.GameTrigger].Add(ability);
                }

                if(ability.CreatureTrigger != ECreatureTrigger.None)
                {
                    if (!creatureTriggerAbilities.ContainsKey(ability.CreatureTrigger))
                        creatureTriggerAbilities.Add(ability.CreatureTrigger, new List<IEnemyAbility>());

                    creatureTriggerAbilities[ability.CreatureTrigger].Add(ability);
                }
            }
        }
        private void ExcuteCreatureAbility(ECreatureTrigger creatureTrigger)
        {
            if(creatureTriggerAbilities.TryGetValue(creatureTrigger, out var list))
            {
                foreach (var ability in list)
                {
                    ability.Excute();
                }
            }
        }
        private void ExcuteGameTriggerAbility(EGameTriggerType trigger)
        {
            if (gameTriggerAbilities.TryGetValue(trigger, out var list))
            {
                foreach (var ability in list)
                {
                    StartCoroutine(ability.Excute());
                }
            }
        }

        #endregion
        public override bool TakeDamage(DamageContext damageInfo)
        {
            ExcuteCreatureAbility(ECreatureTrigger.OnReceivedAttack);
            return base.TakeDamage(damageInfo);
        }
        public override void Init(ICombatEventListener combatEventListener, ECretureType cretureType)
        {
            base.Init(combatEventListener, cretureType);
         
            _backAttackMultiplier = GameManager.Config.Combat.BackAttackDMGMultiple;
        }
        public void SetData(EnemyData data, int floor)
        {
            IsActive = true;

            _enemyData = data;

            int RandomDir = UnityEngine.Random.Range(0, 4);
            SetRotation((EDirectionType)RandomDir);

            int hp = CalculateHP(_enemyData.BaseHP, floor);
            int dmg = CalculateDamage(_enemyData.BaseDamage, floor);
            int xp = CalculateXp(_enemyData.BaseXP, floor);

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
        public override void AddDamageDealtMultiplier(float value)
        {
            base.AddDamageDealtMultiplier(value);
            OnStatChangedEvent?.Invoke(EEnemyStatType.Damage);
        }
        public void SetEnemyStat(EEnemyStatType stat, int value)
        {
            switch (stat)
            {
                case EEnemyStatType.HP:
                    _enemyStat.HP = value;
                    break;
                case EEnemyStatType.Damage:
                    _enemyStat.Damage = value;
                    break;
            }
            OnStatChangedEvent?.Invoke(stat);
        }
        public void ModifyEnemyStat(EnemyApplyStatContext context)
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
            ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, -damage, EEnemyStatType.None));
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
            ExcuteCreatureAbility(ECreatureTrigger.OnAttack);

            damageContext.Attacker = this;
            damageContext.Damage = Get(EEnemyStatType.Damage);

            return damageContext;
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