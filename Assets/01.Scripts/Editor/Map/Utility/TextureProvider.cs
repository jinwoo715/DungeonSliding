#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class TextureProvider : ITextureProvider
    {
        private Dictionary<ETileType, Texture2D> _tileTextureDic = new Dictionary<ETileType, Texture2D>();

        private Dictionary<EEditorCretureType, Texture2D> _creatureTextureDic = new Dictionary<EEditorCretureType, Texture2D>();
        private Dictionary<EEffectObjectType, Texture2D> _effectObjectTextureDic = new Dictionary<EEffectObjectType, Texture2D>();
        
        public void Init()
        {
            string[] cretureNames = Enum.GetNames(typeof(EEditorCretureType));
            string basePath = "04.Sprites/Editor/Creature";

            for (int i = 0; i < cretureNames.Length; i++)
            {
                string iconName = $"{cretureNames[i]}.png";
                string path = Path.Combine(basePath, iconName);

                _creatureTextureDic.Add((EEditorCretureType)i, LoadTexture(path));
            }

            string[] effectObjectNames = Enum.GetNames(typeof(EEffectObjectType));
            string effectPath = Path.Combine("04.Sprites/Editor/EffectObject/");

            for (int i = 0; i < effectObjectNames.Length; i++)
            {
                string effectObjPath = effectPath + effectObjectNames[i] + ".png";
                _effectObjectTextureDic.Add((EEffectObjectType)i, LoadTexture(effectObjPath));
            }

            string[] tileNames = Enum.GetNames(typeof(ETileType));
            string tilePath = Path.Combine("04.Sprites/");

            for (int i = 0; i < tileNames.Length; i++)
            {
                string tileName = tilePath + tileNames[i] + ".png";
                _tileTextureDic.Add((ETileType)i, LoadTexture(tileName));
            }
        }

        private Texture2D LoadTexture(string path)
        {
            string assetPath = $"Assets/{path.Replace('\\', '/')}";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

            if (sprite != null)
                return sprite.texture;

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            if (texture != null)
                return texture;

            Debug.LogWarning($"Map editor texture not found: {assetPath}");
            return Texture2D.grayTexture;
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
            return GetCreatureIcon(EEditorCretureType.Player);
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
