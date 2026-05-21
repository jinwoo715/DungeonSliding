using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.Map
{
    [CreateAssetMenu(fileName = "MapBundle", menuName = "Data/MapBundle", order = 1)]
    public class MapBundle : ScriptableObject
    {
        [SerializeField] private List<ActMapDataBundle> Bundles;

        public int TotalFloorCount()
        {
            int floor = 0;

            for (int i = 0; i < Bundles.Count; i++)
            {
                floor += Bundles[i].ActFloorCount;
            }

            return floor;
        }

        public List<int> GetBossStages()
        {
            List<int> floors = new List<int>();

            int currentFloor = 0;

            for (int i = 0; i < Bundles.Count; i++)
            {
                currentFloor += Bundles[i].ActFloorCount;
                floors.Add(currentFloor);
            }

            return floors;
        }

        public ActMapDataBundle GetActMapBundle(int actNum)
        {
            if (Bundles.Count <= actNum) return null;

            return Bundles[actNum];
        }
    }
}
