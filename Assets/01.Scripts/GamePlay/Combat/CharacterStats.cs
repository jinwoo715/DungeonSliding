
namespace JW.DungeonSliding
{
    public struct CretureStat
    {
        public int HP;
        public int Damage;

        public CretureStat(int hp, int damage)
        {
            HP = hp;
            Damage = damage;
        }
    }

    public struct PlayerStat
    {
        public CretureStat Base;
        public int MoveCount;

        public PlayerStat(CretureStat cretureStat, int moveCount)
        {
            Base = cretureStat;
            MoveCount = moveCount;
        }
    }
}
