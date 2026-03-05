using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class DamageCalculator
    {
        public static int CalculateBackAttackDamage(int baseDamage, float criMultiple)
        {
            return Mathf.RoundToInt(baseDamage * criMultiple);
        }

        public static bool IsBackAttack(ICombatant attacker, ICombatant victim)
        {
            if (attacker == null || victim == null) return false;

            var victimDir = victim.Rotate.Direction;
            var reverseDir = victim.Rotate.ReverseDirection(victimDir);
            var behindTile = victim.Tile.TilePosition.GetNextTileByDir(reverseDir);

            return attacker.Tile.TilePosition == behindTile;
        }
    }
}
