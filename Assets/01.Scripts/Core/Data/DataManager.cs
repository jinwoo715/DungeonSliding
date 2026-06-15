using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Data
{
    public interface IDataService
    {
        List<EnemyData> GetEnemyData();
        List<EnemyData> GetBossData();
    }

    public class DataManager : IDataService
    {
        public List<EnemyData> EnemyData { get; private set; }
        public List<EnemyData> EnemyBossData { get; private set; }

        public Dictionary<string, EnemyAbilityData> _enemyAbility = new Dictionary<string, EnemyAbilityData>();

        public List<AbilityDataBase> StatAbilities { get; private set; } = new List<AbilityDataBase>();
        public List<AbilityDataBase> RuleStatAbilities { get; private set; } = new List<AbilityDataBase>();
        public List<AbilityDataBase> RuleAbilities { get; private set; } = new List<AbilityDataBase>();
        public GameConfig Config { get; private set; }

        #region Getter

        public EnemyAbilityData EnemyAbility(string name)
        {
            return _enemyAbility[name];
        }
        public List<EnemyAbilityData> EnemyAbilities(string name)
        {
            if (string.IsNullOrEmpty(name)) return null;

            string[] abilities = name.Split('|');

            List<EnemyAbilityData> abilityList = new List<EnemyAbilityData>();

            for (int i = 0; i < abilities.Length; i++)
            {
                EnemyAbilityData data = _enemyAbility[abilities[i]];

                abilityList.Add(data);
            }

            return abilityList;
        }

        #endregion

        public void Initialize()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new EmptyStringValueTypeResolver()
            };

            ReloadAbilityData(settings);

            string enemyAbility = GameManager.Resource.GetTextData(ConstDataKey.ENEMY_ABILITY_DATA);
            var enemyAbilities = JsonConvert.DeserializeObject<List<EnemyAbilityData>>(enemyAbility, settings);

            for (int i = 0; i < enemyAbilities.Count; i++)
            {
                var data = enemyAbilities[i];
                _enemyAbility.Add(data.Name, data);
            }

            EnemyData = new List<EnemyData>();
            string enemyDatas = GameManager.Resource.GetTextData(ConstDataKey.ENEMY_DATA);
            EnemyData = JsonConvert.DeserializeObject<List<EnemyData>>(enemyDatas, settings);

            EnemyBossData = new List<EnemyData>();
            string bossDatas = GameManager.Resource.GetTextData(ConstDataKey.ENEMY_BOSS_DATA);
            EnemyBossData = JsonConvert.DeserializeObject<List<EnemyData>>(bossDatas, settings);

            Config = GameManager.Resource.GameConfig;
        }

        public void ReloadAbilityData()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new EmptyStringValueTypeResolver()
            };

            ReloadAbilityData(settings);
        }

        private void ReloadAbilityData(JsonSerializerSettings settings)
        {
            StatAbilities = new List<AbilityDataBase>();
            RuleStatAbilities = new List<AbilityDataBase>();
            RuleAbilities = new List<AbilityDataBase>();

            AbilityDatabaseSO abilityDatabase = GameManager.Resource.AbilityDatabase;
            List<AbilityDataBase> pureRuleAbilities = new List<AbilityDataBase>();

            if (abilityDatabase != null)
            {
                StatAbilities = abilityDatabase.CreateStatAbilityRuntimeData();
                RuleStatAbilities = abilityDatabase.CreateRuleStatAbilityRuntimeData();
                pureRuleAbilities = abilityDatabase.CreatePureRuleAbilityRuntimeData();
            }

            if (StatAbilities.Count == 0)
            {
                StatAbilities.AddRange(
                    LoadAbilityJson<StatAbilityData>(ConstDataKey.STAT_ABILITY_DATA, settings));
            }

            if (RuleStatAbilities.Count == 0)
            {
                RuleStatAbilities.AddRange(
                    LoadAbilityJson<RuleStatAbilityData>(ConstDataKey.RULE_STAT_ABILITY_DATA, settings));
            }

            if (pureRuleAbilities.Count == 0)
            {
                pureRuleAbilities.AddRange(
                    LoadAbilityJson<RuleAbilityData>(ConstDataKey.RULE_ABILITY_DATA, settings));
            }

            RuleAbilities.Clear();
            RuleAbilities.AddRange(RuleStatAbilities);
            RuleAbilities.AddRange(pureRuleAbilities);

            if (StatAbilities.Count == 0 || RuleAbilities.Count == 0)
            {
                throw new InvalidOperationException(
                    $"Ability data load failed. Stat: {StatAbilities.Count}, Rule: {RuleAbilities.Count}");
            }

            Debug.Log(
                $"Ability data loaded. Stat: {StatAbilities.Count}, RuleStat: {RuleStatAbilities.Count}, Rule: {RuleAbilities.Count}");
        }

        private List<T> LoadAbilityJson<T>(string dataKey, JsonSerializerSettings settings)
        {
            string json = GameManager.Resource.GetTextData(dataKey);
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            return JsonConvert.DeserializeObject<List<T>>(json, settings) ?? new List<T>();
        }

        public List<EnemyData> GetEnemyData() => EnemyData;
        public List<EnemyData> GetBossData() => EnemyBossData;
    }
}
