using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Core.Resource
{
    public class ResourceManager : MonoBehaviour
    {
        [SerializeField] private List<MapData> _mapDatas;
        public List<MapData> MapData { get; private set; }

        internal void Init()
        {
            throw new NotImplementedException();
        }
    }
}
