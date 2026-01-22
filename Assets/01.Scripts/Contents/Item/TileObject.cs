using JW.Utility;
using UnityEngine;

namespace JW.SlidingPuzzle {
    public class TileObject : PoolObject
    {
        [SerializeField] private ETileType _tileType;

        public override void OnDespawn()
        {
            Release();
        }

        public override void OnSpawn()
        {

        }
    }
}