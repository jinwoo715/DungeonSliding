using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Data
{
    public class DataManager
    {
        public List<EnemyData> EnemyData { get; private set; }
        public List<EnemyAbilityData> EnemyAbilityDatas { get; private set; }
        public List<AbilityDataBase> Abilities { get; private set; }
        public GameConfig Config { get; private set; }

        public void Initialize()
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new EmptyStringValueTypeResolver()
            };

            Abilities = new List<AbilityDataBase>();

            string rule = GameManager.Resource.GetTextData(ConstDataKey.RULE_ABILITY_DATA);
            string stat = GameManager.Resource.GetTextData(ConstDataKey.STAT_ABILITY_DATA);

            Abilities.AddRange(JsonConvert.DeserializeObject<List<RuleAbilityData>>(rule, settings));
            Abilities.AddRange(JsonConvert.DeserializeObject<List<StatAbilityData>>(stat, settings));

            EnemyAbilityDatas = new List<EnemyAbilityData>();
            string enemyAbility = GameManager.Resource.GetTextData(ConstDataKey.ENEMY_ABILITY_DATA);
            EnemyAbilityDatas.AddRange(JsonConvert.DeserializeObject<List<EnemyAbilityData>>(enemyAbility, settings));

            EnemyData = new List<EnemyData>();
            string enemyDatas = GameManager.Resource.GetTextData(ConstDataKey.ENEMY_DATA);
            EnemyData.AddRange(JsonConvert.DeserializeObject<List<EnemyData>>(enemyDatas, settings));

            Config = GameManager.Resource.GameConfig;
        }

    }
}
