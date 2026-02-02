using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private List<MapData> _mapDatas;
        [SerializeField] private List<TextAsset> _textDatas;
        [SerializeField] private List<AbilityData> _abilities;

        [SerializeField] private GameConfig _gameConfig;

        public List<MapData> MapData { get; private set; }
        public Dictionary<string, string> _textDataByName = new Dictionary<string, string>();
        public List<AbilityData> AllAbility => _abilities;
        public GameConfig GameConfig => _gameConfig;

        internal void Init()
        {
            MapData = _mapDatas;
            for (int i = 0; i < _textDatas.Count; i++)
            {
                string textName = _textDatas[i].name;
                _textDataByName[textName] = _textDatas[i].text;
            }
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
}
