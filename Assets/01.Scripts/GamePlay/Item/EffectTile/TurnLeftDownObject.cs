using UnityEngine;

namespace JW.DungeonSliding
{
    public class TurnLeftDownObject : EffectObjectBase, IEffectObject
    {
        public override MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            // ¡æ ¡é,
            // ¡é ¡æ,     ¢Ê
            // ¡è ¡ç,
            // ¡ç ¡è 

            switch (moveContext.Direction)
            {
                case EDirectionType.Left:
                    moveContext.Direction = EDirectionType.Up;
                    break;
                case EDirectionType.Up:
                    moveContext.Direction = EDirectionType.Left;
                    break;
                case EDirectionType.Right:
                    moveContext.Direction = EDirectionType.Down;
                    break;
                case EDirectionType.Down:
                    moveContext.Direction = EDirectionType.Right;
                    break;
            }

            moveContext.ResultType = ESlideResultType.Move;

            return moveContext;
        }
    }
}