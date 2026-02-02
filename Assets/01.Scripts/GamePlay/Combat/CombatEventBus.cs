using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface ICombatEventPresenter
    {
        public event Action<DamageEvent> DamageEvent;
        public event Action<DeathEvent> DeathEvent;
    }
    public interface ICombatEventListener
    {
        public void RaiseDamageEvent(DamageEvent e);
        public void RaiseDeathEvent(DeathEvent e);
    }

    public class CombatEventBus : ICombatEventPresenter, ICombatEventListener
    {
        public event Action<DamageEvent> DamageEvent;
        public event Action<DeathEvent> DeathEvent;

        public void RaiseDamageEvent(DamageEvent e) => DamageEvent?.Invoke(e);
        public void RaiseDeathEvent(DeathEvent e) => DeathEvent?.Invoke(e);

        public void Clear()
        {
            DamageEvent = null;
            DeathEvent = null;
        }
    }

    public readonly struct DamageEvent
    {
        public readonly ICombatant Attacker;
        public readonly ICombatant Target;
        public readonly int Damage;
        public readonly bool IsCrit;

        public DamageEvent(ICombatant attacker, ICombatant target, int damage, bool isCrit = false)
        {
            Attacker = attacker;
            Target = target;
            Damage = damage;
            IsCrit = isCrit;
        }
    }
    public readonly struct DeathEvent
    {
        public readonly ICombatant Killer;
        public readonly ICombatant Victim;

        public DeathEvent(ICombatant killer, ICombatant victim)
        {
            Killer = killer;
            Victim = victim;
        }
    }
}
