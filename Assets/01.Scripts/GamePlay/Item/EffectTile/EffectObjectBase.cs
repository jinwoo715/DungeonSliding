using JW.DungeonSliding.GamePlay;
using UnityEngine;

namespace JW.DungeonSliding
{
    public abstract class EffectObjectBase : MonoBehaviour, IEffectTile, ITileObject
    {
        [SerializeField] private GameObject _tileMark;

        protected EffectObjectData _effectObjectData;
        public EEffectObjectType EffectType => _effectObjectData.EffectObjectType;
        public Tile TilePosition => _effectObjectData.Point;

        public bool IsStepped { get; private set; }

        public MeshRenderer _mesh;

        public Texture _offTexture;
        public Texture _onTexture;

        private void OnEnable()
        {
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, TurnOnTile);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, TurnOnTile);
        }

        public void Init(EffectObjectData effectObjectData)
        {
            _effectObjectData = effectObjectData;
        }

        public virtual MoveContext OnEnterTile(ref MoveContext moveContext)
        {
            IsStepped = true;
            moveContext.EnterEffectTile(TurnOffTile);
            return moveContext;
        }

        public void SetPosition(Tile point)
        {
            this.transform.localPosition = point.GetPosition;
        }
        private void TurnOffTile()
        {
  //          _mesh.material.mainTexture = _offTexture;
            _tileMark.SetActive(false);
        }
        private void TurnOnTile()
        {
            //            _mesh.material.mainTexture = _onTexture;
            IsStepped = false;
            _tileMark.SetActive(true);
        }

        public void OnStepped()
        {
            _tileMark.SetActive(false);
        }
    }
}