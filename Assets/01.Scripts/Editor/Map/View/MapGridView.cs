using JW.DungeonSliding;
using System;
using UnityEditor;
using UnityEngine;

public class MapGridView
{
    public Action<int, int> OnClickGridEvent;
    private GUIStyle _tileButtonGUI;

    private float _iconImageSize;

    private readonly ITextureProvider _textureProvider;
    private readonly MapEditState _mapState;

    public MapGridView(MapEditState mapEditState, ITextureProvider textureProvider)
    {
        _mapState = mapEditState;
        _textureProvider = textureProvider;
    }

    private void OnClickGrid(int x, int z)
    {
        OnClickGridEvent?.Invoke(x,z);
    }

    private void InitButtonGUI()
    {
        if (_tileButtonGUI != null)
            return;

        _tileButtonGUI = new GUIStyle(GUI.skin.button);
        _tileButtonGUI.padding = new RectOffset(0, 0, 0, 0);
        _tileButtonGUI.margin = new RectOffset(0, 0, 0, 0);
        _tileButtonGUI.imagePosition = ImagePosition.ImageOnly;
    }

    public void DrawGridWithOverlays(int currentTemplete)
    {
        InitButtonGUI();

        if (_mapState.xCount <= 0 || _mapState.zCount <= 0)
            return;

        UpdateIconSize();

        GUI.BeginGroup(new Rect(_mapState.GridFieldX, 0, _mapState._gridFieldWidth, _mapState.WindowHeight));

        DrawTileGrid();

        DrawPlayerIcon();

        DrawEnemyIcons(currentTemplete);

        DrawEffectObjects();

        GUI.EndGroup();
    }

    private void UpdateIconSize()
    {
        float xSize = _mapState._gridFieldWidth / _mapState.xCount;
        float ySize = _mapState.WindowHeight / _mapState.zCount;

        float size = xSize <= ySize ? xSize : ySize;
        _iconImageSize = size;

        _tileButtonGUI.fixedWidth = size;
        _tileButtonGUI.fixedHeight = size;
    }

    private void DrawTileGrid()
    {
        float zStartPoint = _iconImageSize * (_mapState.zCount-1);
        for (int z = 0; z < _mapState.zCount; z++)
        {
            for (int x = 0; x < _mapState.xCount; x++)
            {
                Texture2D buttonTexture = _textureProvider.GetTileTexture((ETileType)_mapState.GetTileType(x, z));
                Rect buttonRect = new Rect(x * _iconImageSize, zStartPoint - (z * _iconImageSize), _iconImageSize, _iconImageSize);

                if (GUI.Button(buttonRect, buttonTexture, _tileButtonGUI))
                {
                    OnClickGrid(x, z);
                }
            }
        }
    }
    private void DrawPlayerIcon()
    {
        Tile playerPoint = _mapState.PlayerPoint;

        if (playerPoint.XPos == -1 || playerPoint.ZPos == -1)
            return;

        GUI.DrawTexture(GetIconRect(playerPoint), _textureProvider.GetPlayerIcon());
    }

    private void DrawEnemyIcons(int currentTemplete)
    {
        EnemyTempleteSheet enemyTemplete = _mapState.GetEnemyTemplete(currentTemplete);

        if (enemyTemplete == null)
            return;

        foreach (var enemy in enemyTemplete.EnemyData)
        {
            GUI.DrawTexture(GetIconRect(enemy.Value.Point), _textureProvider.GetEnemyIcon(enemy.Value.EnemyUID));
        }
    }
    private void DrawEffectObjects()
    {
        if (_mapState.EffectObjects == null) 
            return;

        foreach (var obj in _mapState.EffectObjects)
        {
            GUI.DrawTexture(GetIconRect(obj.Value.Point), _textureProvider.GetEffectIcon(obj.Value.EffectObjectType));
        }
    }

    private Rect GetIconRect(Tile point)
    {
        float zStartPoint = _iconImageSize * (_mapState.zCount - 1);
        float iconRatio = 0.5f;
        float iconSize = _iconImageSize * iconRatio;

        float x = point.XPos * _iconImageSize + iconSize * 0.5f;
        float y = zStartPoint - (point.ZPos * _iconImageSize) + (iconSize * 0.5f);

        return new Rect(x, y, iconSize, iconSize);
    }
}
