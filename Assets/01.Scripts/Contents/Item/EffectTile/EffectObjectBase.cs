using UnityEngine;

namespace JW.SlidingPuzzle
{
    public abstract class EffectObjectBase : MonoBehaviour, IEffectObject, ITilePoint
    {
        protected EffectObjectData _effectObjectData;

        public EEffectObjectType EffectType => _effectObjectData.EffectObjectType;
        public TilePoint Point => _effectObjectData.Point;

        public void Init(EffectObjectData effectObjectData)
        {
            _effectObjectData = effectObjectData;
        }

        public abstract MoveContext OnEnterTile(ref MoveContext moveContext);

        public void SetPosition(TilePoint point)
        {
            this.transform.localPosition = point.GetPosition;
        }
    }
}