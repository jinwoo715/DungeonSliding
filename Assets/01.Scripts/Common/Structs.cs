using System;
using UnityEngine;

namespace JW.SlidingPuzzle
{
    [System.Serializable]
    public struct EffectObjectData
    {
        public EEffectObjectType EffectObjectType;
        public TilePoint Point;

        //Teleport Tile¸¸ »ç¿ë
        public TilePoint TeleportPoint;

        public EffectObjectData(TilePoint point, EEffectObjectType type)
        {
            Point = point;
            EffectObjectType = type;
            TeleportPoint = TilePoint.Invalid;
        }
    }

    public struct MoveContext
    {
        public EDirectionType Direction;
        public ESlideResultType ResultType;
        public TilePoint DestTile;
        public ETileEnterType EnterType;

        public int Damage;

        public MoveContext(TilePoint point, EDirectionType direction, ETileEnterType enterType) 
        {
            Damage = 0;
            DestTile = point;
            Direction = direction;
            ResultType = ESlideResultType.Move;
            EnterType = enterType;
        }
    }

    [System.Serializable]
    public struct TilePoint
    {
        public int XPos; 
        public int ZPos;

        public TilePoint(int x, int z)
        {
            XPos = x;
            ZPos = z;
        }

        public bool IsValid => (XPos >= 0 && ZPos >= 0);
        public static TilePoint Invalid => new TilePoint(-1, -1);
        public static Vector3 Direction(TilePoint destination, TilePoint start)
        {
            return (destination.GetPosition - start.GetPosition).normalized;
        }
        public void Expire()
        {
            XPos = -1;
            ZPos = -1;
        }
        
        public TilePoint GetNextTile(EDirectionType directionType)
        {
            int[] xDir = { -1, 0, 1, 0 };
            int[] zDir = { 0, 1, 0, -1 };

            return new TilePoint(XPos + xDir[(int)directionType], ZPos + zDir[(int)directionType]);
        }

        public Vector3 GetPosition => new Vector3(XPos, 0, ZPos);
        
        public static bool IsNearPosition(TilePoint a, TilePoint b)
        {
            return Vector3.Distance(a.GetPosition, b.GetPosition) <= 1;
        }
       
        public static bool operator ==(TilePoint a, TilePoint b)
        {
            return a.XPos == b.XPos && a.ZPos == b.ZPos;
        }

        public static bool operator != (TilePoint a, TilePoint b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            if (obj is TilePoint other)
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

    [System.Serializable]
    public struct DamageInfo
    {
        public ICombatant Attacker;
        public int Damage;
        public bool IsCritical;
        public EStatusEffectType StatusEffect;
        public int StatusAmount;
        public DamageInfo(ICombatant attacker, int amount, bool isCritical, EStatusEffectType effectType = EStatusEffectType.None, int statusAmount = 0)
        {
            Attacker = attacker;
            Damage = amount;
            IsCritical = isCritical;
            StatusEffect = effectType;
            StatusAmount = statusAmount;
        }
    }
    public readonly struct RewardData
    {
        public readonly int Xp;

        public RewardData(int xp)
        {
            Xp = xp;
        }
    }
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
}
