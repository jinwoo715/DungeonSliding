using UnityEngine;
namespace JW.SlidingPuzzle
{
    public class TeleportObject : EffectObjectBase, IEffectObject
    {
        private TilePoint deltaPoint;

        public override MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            if (moveContext.EnterType != ETileEnterType.Slide)
                return moveContext;

            moveContext.ResultType = ESlideResultType.Teleport;
            moveContext.DestTile = _effectObjectData.TeleportPoint;
            moveContext.EnterType = ETileEnterType.Teleport;

            return moveContext;
        }
    }
}