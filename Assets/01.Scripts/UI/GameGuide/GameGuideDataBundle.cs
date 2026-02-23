using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    [CreateAssetMenu(fileName = "GuideDataBundle", menuName = "Data/GuideDataBundle", order = 1)]
    public class GameGuideDataBundle : ScriptableObject
    {
        public EGuideType GuideType;
        public List<GameGuideData> Datas;
    }
}
