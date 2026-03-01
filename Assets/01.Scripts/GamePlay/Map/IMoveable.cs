namespace JW.DungeonSliding.Map
{
    public interface IMoveable : ITileObject
    {
        public int SlideTileCount();
        public ESlideResultType SlideResultType { get; }
        public void SetMoveResult(ESlideResultType result);
        public void KnockBack(EDirectionType dir);
        public void SlideRoute(EDirectionType directionType);
    }
}