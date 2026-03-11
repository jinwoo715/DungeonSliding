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
        private float _backAttackMultiplier = 2f;

        public string Name => _enemyData.Name;
        public string Description => _enemyData.Description;
        public string EnemyUID => _enemyData.UID;
        public Transform StatUITransform => _statUITransform;

        //TODO Enemy Skill 구현
        #region Enemy Skill
        private Dictionary<EGameEventTrigger, List<IAbility>> gameTriggerAbilities = new();
        private Dictionary<ECreatureTrigger, List<IAbility>> creatureTriggerAbilities = new ();

        public void SetAbility(List<IAbility> abilities)
        {
            if (abilities == null) return;

            foreach (var ability in abilities)
            {
                if(ability.GameTrigger != EGameEventTrigger.None)
                {
                    if (!gameTriggerAbilities.ContainsKey(ability.GameTrigger))
                    {
                        gameTriggerAbilities.Add(ability.GameTrigger, new List<IAbility>());
                    }

                    gameTriggerAbilities[ability.GameTrigger].Add(ability);
                }

                if(ability.CreatureTrigger != ECreatureTrigger.None)
                {
                    if (!creatureTriggerAbilities.ContainsKey(ability.CreatureTrigger))
                        creatureTriggerAbilities.Add(ability.CreatureTrigger, new List<IAbility>());

                    creatureTriggerAbilities[ability.CreatureTrigger].Add(ability);
                }
            }
        }

        #endregion

        public override void Initialize(ECreatureType cretureType)
        {
            base.Initialize(cretureType);
         
            _backAttackMultiplier = GameManager.Config.Combat.BackAttackDMGMultiple;
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

            _enemyStat = new EnemyStat(hp, dmg, xp);

            OnStatChangedEvent?.Invoke(EEnemyStatType.HP);
            OnStatChangedEvent?.Invoke(EEnemyStatType.Damage);
        }

        public override void OnDeath()
        {
            base.OnDeath();
            //GameTriggerEventBus.Instance?.ExcuteAbilityEvent(EGameEventTriggerType.OnKillEnemy);
            //OnDeathEvent?.Invoke(this);
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