using UnityEngine;

namespace JW.DungeonSliding
{
    public static class Chance
    {
        public static bool IsChanceSuccess(float chance) 
        {
            int ranNum = UnityEngine.Random.Range(1, 101);
            Debug.Log($"Canc : {chance}");
            Debug.Log($"Ran : {ranNum}");
            return chance >= ranNum;
        }
        public static int GetRandomNum(int excludeMax)
        {
            int ranNum = UnityEngine.Random.Range(0, excludeMax);
            return ranNum;
        }
    }
}
