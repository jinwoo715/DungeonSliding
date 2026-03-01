namespace JW.DungeonSliding
{
    public interface ITileObject : IReadOnlyTilePosition
    {
        public void SetPosition(Tile point);
    }

    public interface IReadOnlyTilePosition
    {
        Tile TilePosition { get; }
    }
}