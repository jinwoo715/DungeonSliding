using UnityEngine;

namespace JW.DungeonSliding
{
    public abstract class EffectObjectBase : MonoBehaviour, IEffectTile, ITilePosition
    {
        protected EffectObjectData _effectObjectData;

        public EEffectObjectType EffectType => _effectObjectData.EffectObjectType;
        public Tile TilePosition => _effectObjectData.Point;

        public void Init(EffectObjectData effectObjectData)
        {
            _effectObjectData = effectObjectData;
        }

        public virtual MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            moveContext.EnterEffectTile();
            return moveContext;
        }

        public void SetPosition(Tile point)
        {
            this.transform.localPosition = point.GetPosition;
        }
    }
}