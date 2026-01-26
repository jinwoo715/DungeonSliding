#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using JW.EditorUtility;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;
using Unity.Plastic.Newtonsoft.Json;

namespace JW.DungeonSliding
{
    public class MapEditor : EditorWindow
    {
        private TextureProvider _textureProvider = new TextureProvider();
        private MapEditorSessionState _sessionState = new MapEditorSessionState();
        private MapEditState _mapEditState;
        private MapGridView _mapGridView;

        private PlayerPlacementService _playerService;
        private EnemyPlacementService _enemyService;
        private EffectObjectPlacementService _effectObjService;

        private MapDataSerializer _mapDataSerializer;

        private int xCountField;
        private int zCountField;

        private int[] _enemyUids;
        private string[] _enemyNames;
        private string[] _effectTileNames;
        private string[] _tileNames;
        private string[] _editModeNames;

        private Vector2 _enemyTempleteScroll;

        private MapData _loadedMapAsset;

        [MenuItem("Tools/Data/Map")]
        public static void Map()
        {
            var window = GetWindow<MapEditor>("Map Editor");
            window.minSize = new Vector2(900, 600);
        }
        private void OnEnable()
        {
            Init();
            SetNames();
        }
        private void Init()
        {
            _textureProvider = new TextureProvider();
            _sessionState = new MapEditorSessionState();
            _mapEditState = new MapEditState();

            _mapGridView = new MapGridView(_mapEditState, _textureProvider);

            _playerService = new PlayerPlacementService();
            _enemyService = new EnemyPlacementService();
            _effectObjService = new EffectObjectPlacementService();

            _mapDataSerializer = new MapDataSerializer();

            _textureProvider.Init();

            _mapGridView.OnClickGridEvent = OnClickGridCell;
        }
        private void SetNames()
        {
            _effectTileNames = Enum.GetNames(typeof(EEffectObjectType));
            _tileNames = Enum.GetNames(typeof(ETileType));
            _editModeNames = Enum.GetNames(typeof(EEditModeType));

            string jsonName = "EnemyData.json";
            string jsonPath = Path.Combine("00.Resources/Data/", jsonName);
            string data = LoadLocalAsset.GetJsonData(jsonPath);

            var enemyDatas = JsonConvert.DeserializeObject<List<EnemyDataSheet>>(data);
            _enemyNames = new string[enemyDatas.Count];
            _enemyUids = new int[enemyDatas.Count];

            for (int i = 0; i < enemyDatas.Count; i++)
            {
                _enemyNames[i] = enemyDatas[i].EnemyName;
                _enemyUids[i] = enemyDatas[i].EnemyUID;
            }
        }
        private void OnGUI()
        {
            UpdateAreaSize();
            DrawHeader();

            switch (_sessionState.EditMode)
            {
                case EEditModeType.Tile:
                    DrawTileEditUI();
                    break;
                case EEditModeType.Player:
                    break;
                case EEditModeType.Enemy:
                    DrawEnemyEdit();
                    break;
                case EEditModeType.Effect:
                    DrawEffectTile();
                    break;
            }

            _mapGridView.DrawGridWithOverlays(_sessionState.EnemyTemplateIndex);
        }
        private void UpdateAreaSize()
        {
            _mapEditState.UpdateFieldArea(position.width, position.height);
        }

        private void DrawHeader()
        {
            BeginFixedWidthBox();

            DrawMapName();

            EditorGUILayout.Space(10);

            DrawSaveButton();

            EditorGUILayout.Space(10);

            DrawLoadMapField();

            EditorGUILayout.Space(10);

            DrawEditModeToolbar();

            EditorGUILayout.Space(10);

            EndFixedWidthBox();
        }
        private void DrawMapName()
        {
            EditorGUILayout.LabelField("Map Name");
            _mapEditState.MapName = EditorGUILayout.TextField(_mapEditState.MapName);
        }
        private void DrawSaveButton()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 30
            };

            if (GUILayout.Button("Save Map", buttonStyle))
            {
                _mapDataSerializer.CreateMapAsset(_mapEditState.GetMapDataContext());
            }
        }
        private void DrawEditModeToolbar()
        {
            _sessionState.EditMode = (EEditModeType)GUILayout.Toolbar(
                (int)_sessionState.EditMode,
                _editModeNames,
                GUILayout.Height(30)
            );
        }

        private void DrawLoadMapField()
        {
            EditorGUILayout.LabelField("Load Map Asset");
            _loadedMapAsset = (MapData)EditorGUILayout.ObjectField(
                _loadedMapAsset,
                typeof(MapData),
                false
            );

            if (GUILayout.Button("로드"))
            {
                if (_loadedMapAsset != null)
                {
                    _mapEditState.LoadFromMapData(_loadedMapAsset);
                    xCountField = _loadedMapAsset.Width;
                    zCountField = _loadedMapAsset.Height;
                    Repaint();
                }
            }
        }

        private void DrawTileEditUI()
        {
            BeginFixedWidthBox();

            EditorGUILayout.LabelField("Map Tile Data");
            EditorGUILayout.Space(20);

            float prev = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60;

            xCountField = EditorGUILayout.IntField("Width", xCountField);
            zCountField = EditorGUILayout.IntField("Height", zCountField);

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 30
            };

            if (GUILayout.Button("타일 생성", buttonStyle))
            {
                _mapEditState.InitTileMap(xCountField, zCountField);
            }

            EditorGUILayout.Space(10);

            EditorGUILayout.LabelField("타일 종류");
            _sessionState.SelectedTileType = (ETileType)GUILayout.Toolbar(
                (int)_sessionState.SelectedTileType,
                _tileNames,
                GUILayout.Height(30)
            );

            EditorGUIUtility.labelWidth = prev;

            EditorGUILayout.Space(20);

            EndFixedWidthBox();
        }
        private void DrawEnemyEdit()
        {
            BeginFixedWidthBox();

            EditorGUILayout.LabelField("Enemy Data");

            EditorGUILayout.Space(10);

            _sessionState.SelectedEnemyIndex = GUILayout.Toolbar(
               _sessionState.SelectedEnemyIndex,
               _enemyNames,
               GUILayout.Height(30)
           );

            string[] templeteNums = new string[_mapEditState.GetEnemyTempleteCount()];
            for (int i = 0; i < templeteNums.Length; i++)
            {
                templeteNums[i] = $"Templete {i+1}";
            }

            _enemyTempleteScroll = EditorGUILayout.BeginScrollView(_enemyTempleteScroll); 
            _sessionState.EnemyTemplateIndex = GUILayout.SelectionGrid(
            _sessionState.EnemyTemplateIndex,
            templeteNums,
            1, // columns = 1 => 세로로 쌓임
            GUILayout.Height(30 * templeteNums.Length)
            );

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                _sessionState.EnemyTemplateIndex = _mapEditState.AddEnemyTemplete();
            }
            if (GUILayout.Button("-"))
            {
                _sessionState.EnemyTemplateIndex = _mapEditState.RemoveEnemyTemplete(_sessionState.EnemyTemplateIndex);
            }

            GUILayout.EndHorizontal();

            //_scroll = EditorGUILayout.BeginScrollView(_scroll);

            GUILayout.Space(5);

            EditorGUILayout.EndScrollView();
            EndFixedWidthBox();
        }
        private void DrawEffectTile()
        {
            BeginFixedWidthBox();

            EditorGUILayout.LabelField("Effect Tile");

            _sessionState.SelectedEffectType = (EEffectObjectType)GUILayout.SelectionGrid(
            (int)_sessionState.SelectedEffectType,
            _effectTileNames,
            2, // columns = 1 => 세로로 쌓임
            GUILayout.Height(30 * _effectTileNames.Length / 2)
            );

            EndFixedWidthBox();
        }

        private void OnClickGridCell(int x, int z)
        {
            Tile point = new Tile(x, z);

            switch (_sessionState.EditMode)
            {
                case EEditModeType.Tile:
                    _mapEditState.SetTileType(point, _sessionState.SelectedTileType);
                    break;

                case EEditModeType.Player:
                    _playerService.ProcessPlayerPoint(_mapEditState, point);
                    break;

                case EEditModeType.Enemy:
                    int enemyTemplete = _sessionState.EnemyTemplateIndex;
                    int enemyUid = _enemyUids[_sessionState.SelectedEnemyIndex];
                    _enemyService.ProcessEnemyPoint(_mapEditState, point, enemyTemplete, enemyUid);
                    break;

                case EEditModeType.Effect:
                    _effectObjService.ProcessEffectObject(_mapEditState, point, _sessionState.SelectedEffectType);
                    break;
            }
        }

        private void BeginFixedWidthBox()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(_mapEditState._dataFieldWidth));
            EditorGUILayout.BeginVertical(); // ← 여기서 Width 제거
        }
        private void EndFixedWidthBox()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif