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
        public static EDirectionType ReverseDirection(EDirectionType currentDir)
        {
            switch (currentDir)
            {
                case EDirectionType.Up:
                    return EDirectionType.Down;
                case EDirectionType.Right:
                    return EDirectionType.Left;
                case EDirectionType.Down:
                    return EDirectionType.Up;
                case EDirectionType.Left:
                    return EDirectionType.Right;
                default:
                    return EDirectionType.None;
            }
        }
        public static EDirectionType GetRightRotateResultDirection(EDirectionType currentDir)
        {
            int rightDir = (int)currentDir + 1;
            rightDir = rightDir % 4;

            return (EDirectionType)rightDir;
        }
        public static EDirectionType GetDirFromTileToTile(Tile baseTile, Tile targetTile)
        {
            float xDistance = targetTile.X - baseTile.X;
            float zDistance = targetTile.Z - baseTile.Z;

            if (Mathf.Abs(xDistance) >= Mathf.Abs(zDistance))
            {
                if (xDistance >= 0) return EDirectionType.Right;
                else return EDirectionType.Left;
            }
            else
            {
                if (zDistance >= 0) return EDirectionType.Up;
                else return EDirectionType.Down;
            }
        }
        public static EDirectionType GetReverseDirection(EDirectionType baseDirection)
        {
            switch (baseDirection)
            {
                case EDirectionType.Up:
                    return EDirectionType.Down;
                case EDirectionType.Right:
                    return EDirectionType.Left;
                case EDirectionType.Down:
                    return EDirectionType.Up;
                case EDirectionType.Left:
                    return EDirectionType.Right;
                default:
                    return EDirectionType.None;
            }
        }
    }
}
