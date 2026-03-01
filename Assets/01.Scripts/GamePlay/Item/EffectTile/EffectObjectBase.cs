using JW.DungeonSliding.GamePlay;
using UnityEngine;

namespace JW.DungeonSliding
{
    public abstract class EffectObjectBase : MonoBehaviour, IEffectTile, ITileObject
    {
        protected EffectObjectData _effectObjectData;
        public EEffectObjectType EffectType => _effectObjectData.EffectObjectType;
        public Tile TilePosition => _effectObjectData.Point;

        public MeshRenderer _mesh;

        public Texture _offTexture;
        public Texture _onTexture;

        private void OnEnable()
        {
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, TurnOnTile);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, TurnOnTile);
        }

        public void Init(EffectObjectData effectObjectData)
        {
            _effectObjectData = effectObjectData;
        }

        public virtual MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            moveContext.EnterEffectTile(TurnOffTile);
            return moveContext;
        }

        public void SetPosition(Tile point)
        {
            this.transform.localPosition = point.GetPosition;
        }
        private void TurnOffTile()
        {
            _mesh.material.mainTexture = _offTexture;
        }
        private void TurnOnTile()
        {
            _mesh.material.mainTexture = _onTexture;
        }
    }
}