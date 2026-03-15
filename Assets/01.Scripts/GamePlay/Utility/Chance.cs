using UnityEngine;

namespace JW.DungeonSliding
{
    public static class Chance
    {
        public static bool IsChanceSuccess(float chance) 
        {
            int ranNum = UnityEngine.Random.Range(1, 101);
            return chance >= ranNum;
        }
    }
}
