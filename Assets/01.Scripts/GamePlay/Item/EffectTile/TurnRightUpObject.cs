using UnityEngine;

namespace JW.DungeonSliding
{
    public class TurnRightUpObject : EffectObjectBase, IEffectObject
    {
        // /
        public override MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            // ¡æ ¡è
            // ¡é ¡ç      ¢É
            // ¡è ¡æ
            // ¡é ¡ç

            switch (moveContext.Direction)
            {
                case EDirectionType.Left:
                    moveContext.Direction = EDirectionType.Down;
                    break;
                case EDirectionType.Up:
                    moveContext.Direction = EDirectionType.Right;
                    break;
                case EDirectionType.Right:
                    moveContext.Direction = EDirectionType.Up;
                    break;
                case EDirectionType.Down:
                    moveContext.Direction = EDirectionType.Left;
                    break;
            }

            moveContext.ResultType = ESlideResultType.Move;

            return moveContext;
        }
    }
}