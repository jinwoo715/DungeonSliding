using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private List<MapData> _mapDatas;
        [SerializeField] private List<TextAsset> _textDatas;
        [SerializeField] private List<AbilityDataBase> _abilities;

        [SerializeField] private GameConfig _gameConfig;

        [SerializeField] private PlayerData _playerData;

        [Header("Ability")]
        [SerializeField] private TextAsset _ruleAbilityJson;
        [SerializeField] private TextAsset _statAbilityJson;

        public List<MapData> MapData { get; private set; }
        public Dictionary<string, string> _textDataByName = new Dictionary<string, string>();
        public List<AbilityDataBase> AllAbilityDatas => _abilities;

        public GameConfig GameConfig => _gameConfig;
        public PlayerData PlayerData => _playerData;

        public List<RuleAbilityData> ruleAbilityDatas;
        public List<StatAbilityData> statAbilityDatas;

        internal void Init()
        {
            MapData = _mapDatas;
            for (int i = 0; i < _textDatas.Count; i++)
            {
                string textName = _textDatas[i].name;
                _textDataByName[textName] = _textDatas[i].text;
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new EmptyStringValueTypeResolver()
            };

            ruleAbilityDatas = JsonConvert.DeserializeObject<List<RuleAbilityData>>(_ruleAbilityJson.text, settings);
            statAbilityDatas = JsonConvert.DeserializeObject<List<StatAbilityData>>(_statAbilityJson.text, settings);

            _abilities = new List<AbilityDataBase>();
            //_abilities.AddRange(statAbilityDatas);
            _abilities.AddRange(ruleAbilityDatas);

        }
        public string GetTextData(string textName)
        {
            if (_textDataByName.ContainsKey(textName))
            {
                return _textDataByName[textName];
            }
            else
            {
                return null;
            }
        }
    }
    public class ForceDefaultConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t != typeof(string); // string 빼고 다 건드림
        public override object ReadJson(JsonReader r, Type t, object ev, JsonSerializer s)
        {
            // 빈 문자열("")이나 null이 들어오면? "씨발 무시하고 기본값 써!"
            if (r.TokenType == JsonToken.String && string.IsNullOrWhiteSpace(r.Value?.ToString()))
            {
                return t.IsValueType ? Activator.CreateInstance(t) : null;
            }
            try { return s.Deserialize(r, t); }
            catch { return t.IsValueType ? Activator.CreateInstance(t) : null; }
        }
        public override void WriteJson(JsonWriter w, object v, JsonSerializer s) => s.Serialize(w, v);
    }
}
