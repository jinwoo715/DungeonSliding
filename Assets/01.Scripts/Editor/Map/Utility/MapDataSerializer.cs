
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
            mapData.CretureTempletes = mapDataContext.CreatureTempletes;
            if (!IsPlayerValid(mapData.CretureTempletes))
            {
                bool ok = UnityEditor.EditorUtility.DisplayDialog(
                "맵 저장",
                $"Player 위치가 없는 Templete이 있습니다.",
                "확인"
                );

                return;
            }

            if (IsEnemyEmpty(mapData.CretureTempletes))
            {
                bool ok = UnityEditor.EditorUtility.DisplayDialog(
                "맵 저장",
                $"Enemy가 비어있는 Templete이 있습니다.",
                "확인"
                );

                return;
            }

            if (IsBossEmpty(mapData.CretureTempletes))
            {
                bool ok = UnityEditor.EditorUtility.DisplayDialog(
                "맵 저장",
                $"Boss가 비어있는 Templete이 있습니다.",
                "확인"
                );

                return;
            }
            mapData.effectTileDatas = new EffectObjectData[mapDataContext.EffectObjData.Count];

            if (mapDataContext.EffectObjData != null)
            {
                int index = 0;
                foreach (var effectObj in mapDataContext.EffectObjData)
                {
                    mapData.effectTileDatas[index] = effectObj.Value;
                    index++;
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


        private bool IsPlayerValid(List<CreatureTemplete> creatureTempletes)
        {
            foreach (var templete in creatureTempletes)
            {
                if (!templete.PlayerPos.IsValid) return false;
            }

            return true;
        }

        private bool IsEnemyEmpty(List<CreatureTemplete> creatureTempletes)
        {
            foreach (var templete in creatureTempletes)
            {
                if (templete.NomalEnemyPos.Count == 0) return true;
            }

            return false;
        }

        private bool IsBossEmpty(List<CreatureTemplete> creatureTempletes)
        {
            foreach (var templete in creatureTempletes)
            {
                if (templete.BossEnemyPos.Count == 0) return true;
            }

            return false;
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
