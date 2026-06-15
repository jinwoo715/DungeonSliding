
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class MapDataSerializer
    {
        private const string DefaultPath = "Assets/08.ScriptableObjects/MapDatas";
        private static readonly Regex ActNamePattern = new Regex(
            @"^Act_(\d+)_",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public void CreateMapAsset(MapDataContext mapDataContext)
        {
            if (string.IsNullOrWhiteSpace(mapDataContext.MapName))
            {
                UnityEditor.EditorUtility.DisplayDialog("맵 저장", "파일 이름을 지정해주세요.", "확인");
                return;
            }

            MapData mapData = ScriptableObject.CreateInstance<MapData>();
            mapData.Width = mapDataContext.XCount;
            mapData.Height = mapDataContext.ZCount;
            mapData.MapTiles = (bool[])mapDataContext.TileArray.Clone();
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

            string savePath = GetSavePath(mapDataContext.MapName);
            if (IsExistSameNameData(savePath))
            {
                bool ok = UnityEditor.EditorUtility.DisplayDialog(
                "맵 저장",
                $"'{mapDataContext.MapName}' 맵이 이미 존재합니다.\n덮어쓸까요?",
                "덮어쓰기",
                "취소"
                );

                if (!ok) return;
            }

            SaveMapData(mapData, savePath);
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

        private void SaveMapData(MapData data, string savePath)
        {
            string directoryPath = Path.GetDirectoryName(savePath)?.Replace('\\', '/');
            EnsureFolderExists(directoryPath);

            MapData savedData = AssetDatabase.LoadAssetAtPath<MapData>(savePath);
            if (savedData != null)
            {
                UnityEditor.EditorUtility.CopySerialized(data, savedData);
                UnityEditor.EditorUtility.SetDirty(savedData);
                Object.DestroyImmediate(data);
            }
            else
            {
                AssetDatabase.CreateAsset(data, savePath);
                savedData = data;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            UnityEditor.EditorUtility.FocusProjectWindow();
            Selection.activeObject = savedData;
        }

        private bool IsExistSameNameData(string path)
        {
            return AssetDatabase.LoadAssetAtPath<MapData>(path) != null;
        }

        private string GetSavePath(string fileName)
        {
            string targetDirectory = DefaultPath;
            Match actMatch = ActNamePattern.Match(fileName);
            if (actMatch.Success && int.TryParse(actMatch.Groups[1].Value, out int actNumber))
            {
                targetDirectory = $"{DefaultPath}/Act{actNumber}";
            }

            return $"{targetDirectory}/{fileName}.asset";
        }

        private void EnsureFolderExists(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath) || AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            string parentPath = Path.GetDirectoryName(folderPath)?.Replace('\\', '/');
            EnsureFolderExists(parentPath);
            AssetDatabase.CreateFolder(parentPath, Path.GetFileName(folderPath));
        }
    }
}
