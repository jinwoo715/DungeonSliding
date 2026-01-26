namespace JW.DungeonSliding.Map
{
    public interface IMoveable : ITilePosition
    {
        public int CurrentMoveCount { get; }
        public int SlideTileCount();
        public bool IsRoute(EDirectionType dir);
        public ESlideResultType SlideResultType { get; }
        public EDirectionType MoveDir { get; }
        public void MoveStep(EDirectionType dir, int stepCount = 1);
    }
}