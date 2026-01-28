using JW.Utility;
using UnityEngine;

namespace JW.DungeonSliding {
    public class TileObject : PoolObject
    {
        [SerializeField] private ETileType _tileType;
        public override void OnDespawn() {}
        public override void OnSpawn() {}
    }
}