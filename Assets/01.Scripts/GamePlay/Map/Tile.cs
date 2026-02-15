using System;
using UnityEngine;
namespace JW.DungeonSliding
{
    [System.Serializable]
    public struct Tile
    {
        public int X;
        public int Z;

        public Tile(int x, int z)
        {
            X = x;
            Z = z;
        }

        public bool IsValid => (X >= 0 && Z >= 0);
        public static Tile Invalid => new Tile(-1, -1);
        public static Vector3 Direction(Tile destination, Tile start)
        {
            return (destination.GetPosition - start.GetPosition).normalized;
        }
        public void Expire()
        {
            X = -1;
            Z = -1;
        }

        public Tile GetNextTile(EDirectionType directionType)
        {
            int[] xDir = { 0, 1, 0, -1 };
            int[] zDir = { 1, 0, -1, 0 };

            return new Tile(X + xDir[(int)directionType], Z + zDir[(int)directionType]);
        }

        public Vector3 GetPosition => new Vector3(X, 0, Z);

        public static bool IsNearPosition(Tile a, Tile b)
        {
            return Vector3.Distance(a.GetPosition, b.GetPosition) <= 1;
        }

        public static bool operator ==(Tile a, Tile b)
        {
            return a.X == b.X && a.Z == b.Z;
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
            return HashCode.Combine(X, Z);
        }
    }
}