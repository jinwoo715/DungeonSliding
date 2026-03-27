using UnityEngine;
namespace JW.DungeonSliding
{
    public class TeleportObject : EffectObjectBase, IEffectTile
    {
        public override MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            if (moveContext.EnterType != ETileEnterType.Slide)
                return moveContext;

            base.OnEnterTile(ref moveContext);

            moveContext.ResultType = ESlideResultType.Teleport;
            moveContext.DestTile = _effectObjectData.TeleportPoint;
            moveContext.EnterType = ETileEnterType.Teleport;

            return moveContext;
        }
    }
}