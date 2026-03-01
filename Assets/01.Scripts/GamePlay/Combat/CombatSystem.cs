using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class CombatSystem : IAttackable, IDamageable
    {
        public Action<ActPair> OnCounterEvent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICombatant AttackTarget => throw new NotImplementedException();

        public float DamageDealtMultiplier => throw new NotImplementedException();

        public float DamageTakenMultiplier => throw new NotImplementedException();

        public ICombatant LastAttacker => throw new NotImplementedException();

        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;

        public void AddDamageContextStatue(EStatusEffectType effectType, int amount)
        {
            throw new NotImplementedException();
        }

        public void AddDamageDealtMultiplier(float value)
        {
            throw new NotImplementedException();
        }

        public void AddDamageTakenMultiplier(float value)
        {
            throw new NotImplementedException();
        }

        public void ApplyDamage(int damage)
        {
            throw new NotImplementedException();
        }

        public void OnDeath()
        {
            throw new NotImplementedException();
        }

        public void StartAttackAnimation()
        {
            throw new NotImplementedException();
        }

        public bool TakeDamage(DamageContext damageInfo)
        {
            throw new NotImplementedException();
        }

        public bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener)
        {
            throw new NotImplementedException();
        }
    }
}
