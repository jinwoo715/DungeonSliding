using JW.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{
    public class TileGenerator : MonoBehaviour
    {
        [SerializeField] private TileObject _routeTilePrefab;

        private bool[] _tileArrayInfo;
        private int _width;
        private int _height;

        private List<TileObject> _activeTiles = new List<TileObject>();
        private DictionaryPool<TileObject> _tilePoolDic = new DictionaryPool<TileObject>();

        private IBoard _board;

        public void Init(IBoard board)
        {
            _board = board;

            _tilePoolDic.CreatePool(ETileType.Route.ToString(), _routeTilePrefab, this.transform);
        }

        public void SetMap(bool[] tileArrayInfo, int height, int width)
        {
            _tileArrayInfo = new bool[tileArrayInfo.Length];
            _tileArrayInfo = tileArrayInfo;
            _width = width;
            _height = height;

            SetAllTile();
        }

        public void ClearAllAcitveTile()
        {
            for (int i = 0; i < _activeTiles.Count; i++)
            {
                _activeTiles[i].Release();
            }

            _activeTiles.Clear();
        }

        private void SetAllTile()
        {
            for (int z = 0; z < _height; z++)
            {
                for (int x = 0; x < _width; x++)
                {
//                    ETileType tileType = (ETileType)_tileArrayInfo[z * _width + x];

                    Tile tilePoint = new Tile(x, z);

                    if (_tileArrayInfo[z * _width + x])
                    {
                        _board.RegisterTileBoard(tilePoint, true);
                        TileObject tile = _tilePoolDic.GetObject("Route");
                        _activeTiles.Add(tile);
                        tile.transform.localPosition = tilePoint.GetPosition;
                    }
                    else
                    {
                        _board.RegisterTileBoard(tilePoint, false);
                    }

                }
            }
        }
    }
}
