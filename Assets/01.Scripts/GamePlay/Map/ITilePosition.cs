namespace JW.DungeonSliding
{
    public interface ITilePosition : IReadOnlyTilePosition
    {
        public void SetPosition(Tile point);
    }

    public interface IReadOnlyTilePosition
    {
        Tile TilePosition { get; }
    }
}