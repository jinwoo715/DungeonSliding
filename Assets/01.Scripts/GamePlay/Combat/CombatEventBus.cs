using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface ICombatEventPresenter
    {
        public event Action<DamageEvent> OnDamageEvent;
        public event Action<DeathEvent> OnDeathEvent;
    }
    public interface ICombatEventListener
    {
        public void RaiseDamageEvent(DamageEvent e);
        public void RaiseDeathEvent(DeathEvent e);
    }

    public class CombatEventBus : ICombatEventPresenter, ICombatEventListener
    {
        private static CombatEventBus _instance;
        public static ICombatEventListener Excuter
        {
            get
            {
                if (_instance == null)
                    throw new NullReferenceException("CombatEvent Not Initialize");
                return _instance;
            }
        }

        public static ICombatEventPresenter Register
        {
            get
            {
                if (_instance == null)
                    throw new NullReferenceException("CombatEvent Not Initialize");
                return _instance;
            }
        }

        public event Action<DamageEvent> OnDamageEvent;
        public event Action<DeathEvent> OnDeathEvent;

        public CombatEventBus()
        {
            Debug.Log("Init Combat Event Bus");
            _instance = this;
        }

        public void RaiseDamageEvent(DamageEvent e) { OnDamageEvent?.Invoke(e); }
        public void RaiseDeathEvent(DeathEvent e) => OnDeathEvent?.Invoke(e);

        public void Clear()
        {
            OnDamageEvent = null;
            OnDeathEvent = null;
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
