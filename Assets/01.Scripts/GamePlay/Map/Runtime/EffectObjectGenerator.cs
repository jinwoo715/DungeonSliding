using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{
    public class EffectObjectGenerator : MonoBehaviour
    {
        [SerializeField] private List<EffectObjectBase> _effectObjectPrefabs;

        Dictionary<EEffectObjectType, Stack<EffectObjectBase>> _effectObjectPool = new Dictionary<EEffectObjectType, Stack<EffectObjectBase>>();
        List<EffectObjectBase> _activeEffectObject = new List<EffectObjectBase>();

        private IBoard _board;

        public void SetBoard(IBoard board)
        {
            _board = board;
        }

        public void SetMap(List<EffectObjectData> effectTileDatas)
        {
            for (int i = 0; i < _activeEffectObject.Count; i++)
            {
                ReturnEffectObject(_activeEffectObject[i]);
            }

            for (int i = 0; i < effectTileDatas.Count; i++)
            {
                EffectObjectBase obj = GetEffectObject(effectTileDatas[i].EffectObjectType);
                obj.Init(effectTileDatas[i]);
                obj.gameObject.SetActive(true);
                obj.SetPosition(effectTileDatas[i].Point);
                _activeEffectObject.Add(obj);
                _board.RegisterEffectObject(effectTileDatas[i].Point, obj);
            }
        }

        public EffectObjectBase GetEffectObject(EEffectObjectType effectObjectType)
        {
            if(!_effectObjectPool.ContainsKey(effectObjectType))
            {
                CreateEffectObjectPool(effectObjectType);
            }

            if(_effectObjectPool[effectObjectType].Count > 0)
            {
                return _effectObjectPool[effectObjectType].Pop();
            }
            else
            {
                return CreateEffectObject(effectObjectType);
            }

        }

        private void CreateEffectObjectPool(EEffectObjectType effectObjectType)
        {
            _effectObjectPool[effectObjectType] = new Stack<EffectObjectBase>();
        }
        private EffectObjectBase CreateEffectObject(EEffectObjectType effectObjectType)
        {
            EffectObjectBase effectObject = Instantiate(_effectObjectPrefabs[(int)effectObjectType]);
            return effectObject;
        }

        private void ReturnEffectObject(EffectObjectBase effectObjectBase)
        {
            effectObjectBase.gameObject.SetActive(false);
            _effectObjectPool[effectObjectBase.EffectType].Push(effectObjectBase);
            _board.UnRegisterEffectObject(effectObjectBase.TilePosition);
        }
    }
}