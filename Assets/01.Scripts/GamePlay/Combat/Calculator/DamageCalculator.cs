using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class DamageCalculator
    {
        public static int CalculateBackAttackDamage(int baseDamage, float criMultiple)
        {
            return Mathf.RoundToInt(baseDamage * criMultiple);
        }
    }

    public static class DirectionUtility
    {
        public static bool IsBackAttack(ICombatant attacker, ICombatant victim)
        {
            if (attacker == null || victim == null) return false;

            var victimDir = victim.Rotate.Direction;
            var reverseDir = victim.Rotate.ReverseDirection(victimDir);
            var behindTile = victim.Tile.TilePosition.GetNextTileByDir(reverseDir);

            return attacker.Tile.TilePosition == behindTile;
        }
        public static bool IsSideAttack(ICombatant attacker, ICombatant target)
        {
            if (attacker == null || target == null) return false;

            int attackDir = (int)attacker.Rotate.Direction;
            int targetDir = (int)target.Rotate.Direction;

            int diff = Mathf.Abs(attackDir - targetDir);
            bool isSide = (diff == 1 || diff == 3);

            return isSide;
        }
        public static bool IsFacingAttack(ICombatant attacker, ICombatant target)
        {
            if (attacker == null || target == null) return false;

            int attackDir = (int)attacker.Rotate.Direction;
            int targetDir = (int)target.Rotate.Direction;

            return (attackDir - targetDir) == 2;
        }
    }
}
