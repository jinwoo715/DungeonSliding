
namespace JW.DungeonSliding
{
    [System.Serializable]
    public struct EnemyStat
    {
        public int HP;
        public int Damage;
        public int XP;

        public EnemyStat(int hp, int damage, int xp)
        {
            HP = hp;
            Damage = damage;
            XP = xp;
        }
    }
}
