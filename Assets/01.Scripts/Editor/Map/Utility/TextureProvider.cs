#if UNITY_EDITOR
using JW.EditorUtility;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class TextureProvider : ITextureProvider
    {
        private Dictionary<ETileType, Texture2D> _tileTextureDic = new Dictionary<ETileType, Texture2D>();
        private Texture2D _playerIconTexture;

        private Dictionary<EEditorCretureType, Texture2D> _creatureTextureDic = new Dictionary<EEditorCretureType, Texture2D>();
        private Dictionary<EEffectObjectType, Texture2D> _effectObjectTextureDic = new Dictionary<EEffectObjectType, Texture2D>();
        
        public void Init()
        {
            string playerIconName = "Player.png";

            string[] cretureNames = Enum.GetNames(typeof(EEditorCretureType));
            string basePath = "00.Resources/Sprites/Editor/Creature";

            for (int i = 0; i < cretureNames.Length; i++)
            {
                string iconName = $"{cretureNames[i]}.png";
                string path = Path.Combine(basePath, iconName);

                Debug.Log(path);

                _creatureTextureDic.Add((EEditorCretureType)i, LoadLocalAsset.GetSingleTexture(path));
            }


            //string jsonName = "EnemyData.json";
            //string jsonPath = Path.Combine("00.Resources/Data/", jsonName);
            //string data = LoadLocalAsset.GetJsonData(jsonPath);

            //var enemyDatas = JsonConvert.DeserializeObject<List<EnemyDataSheet>>(data);

            //for (int i = 0; i < enemyDatas.Count; i++)
            //{
            //    string imageName = enemyDatas[i].EnemyName + ".png";
            //    string imagePath = Path.Combine("00.Resources/Sprites/Editor/Enemy", imageName);
            //    _creatureTextureDic.Add(enemyDatas[i].EnemyName, LoadLocalAsset.GetSingleTexture(imagePath));
            //}

            string[] effectObjectNames = Enum.GetNames(typeof(EEffectObjectType));
            string effectPath = Path.Combine("00.Resources/Sprites/Editor/EffectObject/");

            for (int i = 0; i < effectObjectNames.Length; i++)
            {
                string effectObjPath = effectPath + effectObjectNames[i] + ".png";
                _effectObjectTextureDic.Add((EEffectObjectType)i, LoadLocalAsset.GetSingleTexture(effectObjPath));
            }

            string[] tileNames = Enum.GetNames(typeof(ETileType));
            string tilePath = Path.Combine("00.Resources/Sprites/");

            for (int i = 0; i < tileNames.Length; i++)
            {
                string tileName = tilePath + tileNames[i] + ".png";
                _tileTextureDic.Add((ETileType)i, LoadLocalAsset.GetSingleTexture(tileName));
            }
        }

        public Texture2D GetEffectIcon(EEffectObjectType type)
        {
            return _effectObjectTextureDic[type];
        }
        public Texture2D GetCreatureIcon(EEditorCretureType type)
        {
            return _creatureTextureDic[type];
        }
        public Texture2D GetPlayerIcon()
        {
            return _playerIconTexture;
        }
        public Texture2D GetTileTexture(ETileType type)
        {
            try
            {
                return _tileTextureDic[type];
            }
            catch(Exception err)
            {
                Debug.Log(err);
                return null;
            }

        }
    }

    public interface ITextureProvider
    {
        Texture2D GetTileTexture(ETileType type);
        Texture2D GetPlayerIcon();
        Texture2D GetCreatureIcon(EEditorCretureType type);
        Texture2D GetEffectIcon(EEffectObjectType type);
    }
}
#endif