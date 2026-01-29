using UnityEngine;
namespace JW.DungeonSliding
{
    public class StopObject : EffectObjectBase, IEffectTile
    {
        public override MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            if (moveContext.EnterType != ETileEnterType.Slide)
                return moveContext;

            moveContext.ResultType = ESlideResultType.Stop;

            return moveContext;
        }
    }
}
