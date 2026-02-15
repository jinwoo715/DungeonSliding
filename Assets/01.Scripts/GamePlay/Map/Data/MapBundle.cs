using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{
    [CreateAssetMenu(fileName = "MapBundle", menuName = "Data/MapBundle", order = 1)]
    public class MapBundle : ScriptableObject
    {
        [SerializeField] private List<MapDataBundle> Bundles;

        public MapDataBundle GetActMapBundle(int actNum)
        {
            if (Bundles.Count < actNum) return null;

            return Bundles[actNum];
        }
    }
}
