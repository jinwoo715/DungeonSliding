namespace JW.DungeonSliding.Map
{
    public interface IMoveable : ITilePosition
    {
        public int SlideTileCount();
        public ESlideResultType SlideResultType { get; }
        public void KnockBack(EDirectionType dir);
        public void SlideRoute(EDirectionType directionType);
    }
}