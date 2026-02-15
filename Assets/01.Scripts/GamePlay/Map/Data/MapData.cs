using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Data/Map", order = 1)]
    public class MapData : ScriptableObject
    {
        public int Width;
        public int Height;
        public int[] MapTiles;

        public List<CreatureTemplete> CretureTempletes;
        public EffectObjectData[] effectTileDatas;
    }
}
