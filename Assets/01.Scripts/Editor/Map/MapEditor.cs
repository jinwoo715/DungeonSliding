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
        private EnemyPlacementService _cretureService;
        private EffectObjectPlacementService _effectObjService;

        private MapDataSerializer _mapDataSerializer;

        private int xCountField;
        private int zCountField;

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
            SetNames();
            Init();
        }
        private void Init()
        {
            _textureProvider = new TextureProvider();
            _sessionState = new MapEditorSessionState();
            _mapEditState = new MapEditState();

            _mapGridView = new MapGridView(_mapEditState, _textureProvider);

            _playerService = new PlayerPlacementService();
            _cretureService = new EnemyPlacementService();
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
        }
        private void OnGUI()
        {
            EnsureInitialized();

            UpdateAreaSize();
            DrawHeader();

            switch (_sessionState.EditMode)
            {
                case EEditModeType.Tile:
                    DrawTileEditUI();
                    break;
                case EEditModeType.Creture:
                    DrawEnemyEdit();
                    break;
                case EEditModeType.Effect:
                    DrawEffectTile();
                    break;
            }

            _mapGridView.DrawGridWithOverlays(_sessionState.CretureTemplateIndex);
        }
        private void EnsureInitialized()
        {
            if (_effectTileNames == null || _tileNames == null || _editModeNames == null)
                SetNames();

            if (_mapEditState == null ||
                _mapGridView == null ||
                _sessionState == null ||
                _playerService == null ||
                _cretureService == null ||
                _effectObjService == null ||
                _mapDataSerializer == null)
            {
                Init();
            }
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
                    Debug.Log("로드");
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

            EditorGUIUtility.labelWidth = prev;

            EditorGUILayout.Space(20);

            EndFixedWidthBox();
        }
        private void DrawEnemyEdit()
        {
            BeginFixedWidthBox();

            EditorGUILayout.LabelField("Creture Type");
            
            string[] cretureTypes = { "Player", "Enemy", "Boss" };

            _sessionState.EditorCretureType = (EEditorCretureType)GUILayout.Toolbar(
               (int)_sessionState.EditorCretureType, cretureTypes, GUILayout.Height(30) );

            EditorGUILayout.Space(10);
            string[] templeteNums = new string[_mapEditState.GetEnemyTempleteCount()];
            for (int i = 0; i < templeteNums.Length; i++)
            {
                templeteNums[i] = $"Templete {i+1}";
            }

            _enemyTempleteScroll = EditorGUILayout.BeginScrollView(_enemyTempleteScroll); 
            _sessionState.CretureTemplateIndex = GUILayout.SelectionGrid(
            _sessionState.CretureTemplateIndex,
            templeteNums,
            1, // columns = 1 => 세로로 쌓임
            GUILayout.Height(30 * templeteNums.Length)
            );

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                _sessionState.CretureTemplateIndex = _mapEditState.AddEnemyTemplete();
            }
            if (GUILayout.Button("-"))
            {
                _sessionState.CretureTemplateIndex = _mapEditState.RemoveEnemyTemplete(_sessionState.CretureTemplateIndex);
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
                    _mapEditState.SetTileType(point);
                    break;

                case EEditModeType.Effect:
                    _effectObjService.ProcessEffectObject(_mapEditState, point, _sessionState.SelectedEffectType);
                    break;

                case EEditModeType.Creture:
                    int templeteNum = _sessionState.CretureTemplateIndex;
                    EEditorCretureType cretureType = _sessionState.EditorCretureType;
                    _cretureService.ProcessCreturePoint(_mapEditState, point, templeteNum, cretureType);
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
