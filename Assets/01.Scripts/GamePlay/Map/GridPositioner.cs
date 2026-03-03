using UnityEngine;

namespace JW.DungeonSliding
{
    public class GridPositioner : ITileObject
    {
        public Tile TilePosition { get; private set; }
        private Transform _owner;

        public GridPositioner(Transform owner)
        {
            _owner = owner;
        }

        public void SetPosition(Tile point)
        {
            _owner.position = point.GetPosition;
            TilePosition = point;
        }
    }
}
