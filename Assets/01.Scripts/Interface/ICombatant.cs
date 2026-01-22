using System;
using System.Collections;

namespace JW.SlidingPuzzle
{
    public interface ICombatant : ITilePoint
    {
        public bool IsActive { get; }
        public EDirectionType Direction { get; }
        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;
        public void Attack(ICombatant target);
        public void GetHit(DamageInfo damageInfo);
        public void OnDeath();
    }

    public interface ITilePoint
    {
        public TilePoint Point { get;}
        public void SetPosition(TilePoint point);
    }
}