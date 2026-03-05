using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Combat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability
{
    public class EnemyAbilityFactory
    {
        public static EnemyAbilityFactory Instance { get; private set; }

        public IEnemyAbilityGetter _getter;

        Dictionary<string, EnemyAbilityData> _enemyAbilityByType = new Dictionary<string, EnemyAbilityData>();
        public EnemyAbilityFactory(IEnemyAbilityGetter getter)
        {
            Instance = this;

            _getter = getter;

            var datas = GameManager.Data.EnemyAbilityDatas;

            foreach (var ability in datas)
            {
                _enemyAbilityByType.Add(ability.Name, ability);
            }
        }
        public List<IAbility> GetAbility(string enemyAbilities, ICombatant host, int section)
        {
            if (string.IsNullOrEmpty(enemyAbilities)) return null;

            string[] abilities = enemyAbilities.Split('|');

            List<IAbility> abilityList = new List<IAbility>();

            for (int i = 0; i < abilities.Length; i++)
            {
                Debug.Log(abilities[i]);
                EnemyAbilityData data = _enemyAbilityByType[abilities[i]];

                abilityList.Add(CreateAbility(data, host, section));
            }

            return abilityList;
        }
        private IAbility CreateAbility(EnemyAbilityData data, ICombatant host, int section)
        {
            switch (data.EnemyAbilityType)
            {
                case EEnemyAbilityType.HeavyGravity: return new HeavyGravityAbility(data, _getter, host, section);
                case EEnemyAbilityType.AutoRotate: return new AutoRotateAbility(data, _getter, host, section);
                case EEnemyAbilityType.MoveBanToDirection: return new FacingMoveBanAbility(data, _getter, host, section);
                case EEnemyAbilityType.CopyPlayerStat: return new CopyAbility(data, _getter, host, section);
                case EEnemyAbilityType.Blind: return new BlindAbility(data, _getter, host, section);
                case EEnemyAbilityType.EnhanceAbility: return new EnemyEnhanceAbility(data, _getter, host, section);
                case EEnemyAbilityType.Exaltation: return new ExaltationAbility(data, _getter, host, section);
                case EEnemyAbilityType.CommandRotate: return new CommandRotateAbility(data, _getter, host, section);
                case EEnemyAbilityType.CounterAbility: return new CounterAbility(data, _getter, host, section);
                case EEnemyAbilityType.DefenceFrontAttack: return new DefenceFrontAttackAbility(data, _getter, host, section);
                case EEnemyAbilityType.AutoRotateToPlayer: return new RotateToPlayerAbility(data, _getter, host, section);
                default: return null;
            }
        }
    }
}
