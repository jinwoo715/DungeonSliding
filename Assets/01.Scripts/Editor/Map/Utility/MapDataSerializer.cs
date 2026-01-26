
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MapDataSerializer
    {
        private string defaultPath = "Assets/00.Resources/ScriptableObjects/MapDatas";

        public void CreateMapAsset(MapDataContext mapDataContext)
        {
            MapData mapData = ScriptableObject.CreateInstance<MapData>();
            mapData.Width = mapDataContext.XCount;
            mapData.Height = mapDataContext.ZCount;
            mapData.MapTiles = (int[])mapDataContext.TileArray.Clone();

            mapData.PlayerPosition = mapDataContext.PlayerPoint;

            var sheet = mapDataContext.EnemyTempleteSheet ?? new List<EnemyTempleteSheet>();

            mapData.EnemyTemplete = new EnemyTemplete[sheet.Count];

            for (int i = 0; i < sheet.Count; i++)
            {
                EnemyTempleteSheet templeteSheet = mapDataContext.EnemyTempleteSheet[i];
                EnemyTemplete templete = new EnemyTemplete();

                foreach (var settingData in templeteSheet.EnemyData)
                {
                    templete.EnemyData.Add(settingData.Value);
                }

                mapData.EnemyTemplete[i] = templete;
            }

            mapData.effectTileDatas = new List<EffectObjectData>();

            if (mapDataContext.EffectObjData != null)
            {
                foreach (var effectObj in mapDataContext.EffectObjData)
                {
                    mapData.effectTileDatas.Add(effectObj.Value);
                }
            }

            if (IsExistSameNameData(GetSavePath(mapDataContext.MapName)))
            {
                bool ok = UnityEditor.EditorUtility.DisplayDialog(
                "맵 저장",
                $"'{mapDataContext.MapName}' 맵이 이미 존재합니다.\n덮어쓸까요?",
                "덮어쓰기",
                "취소"
                );

                if (!ok) return;
            }

            SaveMapData(mapData, mapDataContext.MapName);
        }

        private void SaveMapData(MapData data, string fileName)
        {
            AssetDatabase.CreateAsset(data, GetSavePath(fileName));
            AssetDatabase.SaveAssets();

            UnityEditor.EditorUtility.FocusProjectWindow();
            Selection.activeObject = data;
        }

        private bool IsExistSameNameData(string path)
        {
            return File.Exists(path);
        }

        private string GetSavePath(string fileName)
        {
            return Path.Combine(defaultPath, $"{fileName}.asset");
        }
    }
}
