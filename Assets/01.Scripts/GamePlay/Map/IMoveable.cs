namespace JW.DungeonSliding.Map
{
    public interface IMoveable : ITilePosition
    {
        public int SlideTileCount();
        public bool IsRoute(EDirectionType dir);
        public ESlideResultType SlideResultType { get; }
        public EDirectionType MoveDir { get; }
        public void MoveStep(EDirectionType dir, int stepCount = 1);
        public void Move(EDirectionType directionType);
    }
}