using UnityEngine;
namespace JW.SlidingPuzzle
{
    public class StopObject : EffectObjectBase, IEffectObject
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
