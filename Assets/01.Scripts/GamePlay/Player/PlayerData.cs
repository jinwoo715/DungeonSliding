using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    [CreateAssetMenu(fileName = "Player", menuName = "Data/Player", order = 0)]
    public class PlayerData : ScriptableObject
    {
        public int HP;
        public int Damage;
        public int MoveCount;
    }
}
