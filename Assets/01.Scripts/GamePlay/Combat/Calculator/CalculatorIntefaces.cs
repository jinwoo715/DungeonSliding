using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IDamageInputCalulator
    {
        public int CalculateDamage(int baseValue);
    }

    public interface IDamageOutputCalulator
    {
        public int CalculateDamage(int baseValue);
    }
}
