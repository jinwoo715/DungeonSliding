using JW.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{
    public class TileGenerator : MonoBehaviour
    {
        [SerializeField] private TileObject _routeTilePrefab;
        [SerializeField] private TileObject _wallTilePrefab;

        private int[] _tileArrayInfo;
        private int _width;
        private int _height;

        private List<TileObject> _activeTiles = new List<TileObject>();
        private DictionaryPool<TileObject> _tilePoolDic = new DictionaryPool<TileObject>();

        private IBoard _board;

        public void Init(IBoard board)
        {
            _board = board;

            _tilePoolDic.CreatePool(ETileType.Route.ToString(), _routeTilePrefab, this.transform);
            _tilePoolDic.CreatePool(ETileType.Wall.ToString(), _wallTilePrefab, this.transform);
        }

        public void SetMap(int[] tileArrayInfo, int height, int width)
        {
            _tileArrayInfo = tileArrayInfo;
            _width = width;
            _height = height;

            ClearAllAcitveTile();
            SetAllTile();
        }

        private void ClearAllAcitveTile()
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
                    ETileType tileType = (ETileType)_tileArrayInfo[z * _width + x];

                    if (tileType == ETileType.Empty) continue;

                    Tile tilePoint = new Tile(x, z);
                    TileObject tile = _tilePoolDic.GetObject(tileType.ToString());

                    _activeTiles.Add(tile);

                    if (tileType == ETileType.Route)
                    {
                        _board.RegisterTileBoard(tilePoint, true);
                    }
                    else
                    {
                        _board.RegisterTileBoard(tilePoint, false);
                    }

                    tile.transform.localPosition = tilePoint.GetPosition;
                }
            }
        }
    }
}
