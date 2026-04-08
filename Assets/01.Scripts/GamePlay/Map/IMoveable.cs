using System;

namespace JW.DungeonSliding.Map
{
    public interface IMoveable
    {
        event Action OnMoveEnd;
        event Action OnSlideEnd;

        public ITileObject TileObject { get; }
        public int SlideTileCount();
        public ESlideResultType SlideResultType { get; }
        public void SetMoveResult(ESlideResultType result);
        public void KnockBack(EDirectionType dir);
        public void SlideRoute(EDirectionType directionType);
    }
}