using System;
using UnityEngine;
namespace JW.DungeonSliding
{
    [System.Serializable]
    public struct Tile
    {
        public int XPos;
        public int ZPos;

        public Tile(int x, int z)
        {
            XPos = x;
            ZPos = z;
        }

        public bool IsValid => (XPos >= 0 && ZPos >= 0);
        public static Tile Invalid => new Tile(-1, -1);
        public static Vector3 Direction(Tile destination, Tile start)
        {
            return (destination.GetPosition - start.GetPosition).normalized;
        }
        public void Expire()
        {
            XPos = -1;
            ZPos = -1;
        }

        public Tile GetNextTile(EDirectionType directionType)
        {
            int[] xDir = { 0, 1, 0, -1 };
            int[] zDir = { 1, 0, -1, 0 };

            return new Tile(XPos + xDir[(int)directionType], ZPos + zDir[(int)directionType]);
        }

        public Vector3 GetPosition => new Vector3(XPos, 0, ZPos);

        public static bool IsNearPosition(Tile a, Tile b)
        {
            return Vector3.Distance(a.GetPosition, b.GetPosition) <= 1;
        }

        public static bool operator ==(Tile a, Tile b)
        {
            return a.XPos == b.XPos && a.ZPos == b.ZPos;
        }

        public static bool operator !=(Tile a, Tile b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is Tile other)
            {
                return this == other;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(XPos, ZPos);
        }
    }
}