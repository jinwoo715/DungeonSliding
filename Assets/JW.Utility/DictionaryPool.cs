using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
namespace JW.Utility 
{

    public class ObjectPool<T> where T : PoolObject
    {
        public T OriginData;
        public Stack<T> Pool = new Stack<T>();
        public Transform Parent;

        public ObjectPool(T origin, int count, Transform parent)
        {
            Parent = parent;
            OriginData = origin;

            for (int i = 0; i < count; i++)
            {
                Pool.Push(CreateObj());
            }
        }

        public T GetObject()
        {
            T obj;

            if (Pool.Count > 0) obj = Pool.Pop(); 
            else obj = CreateObj();

            obj.gameObject.SetActive(true);
            obj.OnSpawn();
            return obj;
        }

        private T CreateObj()
        {
            T instance = MonoBehaviour.Instantiate(OriginData, Parent);
            instance.SetReleaseEvent(ReturnObject);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public void ReturnObject(PoolObject returnObj)
        {
            returnObj.OnDespawn();
            returnObj.gameObject.SetActive(false);
            Pool.Push((T)returnObj);
        }
    }

    public class DictionaryPool<T> where T : PoolObject
    {
        private Dictionary<string, ObjectPool<T>> _dictionaryPool = new Dictionary<string, ObjectPool<T>>();

        public void CreatePool(string key, T originPrefab, Transform parent, int initCount = 5)
        {
            if (!_dictionaryPool.ContainsKey(key))
            {
                _dictionaryPool.Add(key, new ObjectPool<T>(originPrefab, initCount, parent));
            }
        }

        public T GetObject(string key)
        {
            return _dictionaryPool[key].GetObject();
        }
    }

    public abstract class PoolObject : MonoBehaviour
    {
        public Action<PoolObject> _releaseEvent;
        public void SetReleaseEvent(Action<PoolObject> releaseEvent) => _releaseEvent = releaseEvent;
        public void Release() => _releaseEvent?.Invoke(this);

        public abstract void OnDespawn();
        public abstract void OnSpawn();
    }
}