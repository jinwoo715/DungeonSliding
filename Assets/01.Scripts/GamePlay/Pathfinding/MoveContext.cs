using System;

namespace JW.DungeonSliding
{
    public struct MoveContext
    {
        public EDirectionType Direction;
        public ESlideResultType ResultType;
        public Tile DestTile;
        public ETileEnterType EnterType;
        public bool OnEnterEffectTile;
        public Action OnStepEvent;

        public MoveContext(Tile point, EDirectionType direction, ETileEnterType enterType)
        {
            DestTile = point;
            Direction = direction;
            ResultType = ESlideResultType.Move;
            EnterType = enterType;
            OnEnterEffectTile = false;
            OnStepEvent = null;
        }
        public void EnterEffectTile(Action stepAction)
        {
            OnEnterEffectTile = true;
            OnStepEvent = stepAction;
        }
    }
}