using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class PlayerData
    {
        public readonly int HP;
        public readonly int Damage;
        public readonly int MoveCount;

        public PlayerData(int hp, int dmg, int move)
        {
            HP = hp;
            Damage = dmg;
            MoveCount = move;
        }
    }
}
