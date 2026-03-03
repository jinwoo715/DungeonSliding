using JW.DungeonSliding.GamePlay.Combat;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class EnemyInputDamageCalculator
    {
        public static int CalculateDamage(ICombatant attacker, ICombatant victim, int baseDamage, float criMultiple)
        {
            if (IsBackAttack(attacker, victim))
                return Mathf.RoundToInt(baseDamage * criMultiple);
            else
                return baseDamage;
        }

        private static bool IsBackAttack(ICombatant attacker, ICombatant victim)
        {
            var behindTile = victim.Tile.TilePosition.GetNextTileByDir(victim.Rotate.ReverseDirection(victim.Rotate.Direction));
            return behindTile != null && attacker.Tile.TilePosition == behindTile;
        }
    }
}
