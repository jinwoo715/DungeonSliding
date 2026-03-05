using JW.DungeonSliding.Core;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public struct AttackResultPayload
    {
        public readonly ICombatant Attacker;
        public readonly int Damage;
        public readonly bool IsCounterAttack;

        public AttackResultPayload(ICombatant attacker, int damage, bool isCounterAttacked)
        {
            Attacker = attacker;
            Damage = damage;
            IsCounterAttack = isCounterAttacked;
        }
    }


    public class CombatSystem
    {
        private ICombatant _owner;
        public ICombatant LastTarget { get; private set; }
        public ICombatant LastAttacker { get; private set; }

        private DamageContext _receivedDamageContext;
        private DamageContext _sendDamageContext;

        private NextAttackEnhanceContext _attackEnhanceContext = new NextAttackEnhanceContext();

        public event Action<AttackResultPayload> OnHitted;

        public bool IsCombated => _isAttacked || _isHitted;
        private bool _isAttacked = false;
        private bool _isHitted = false;

        public CombatSystem(ICombatant combatant)
        {
            _owner = combatant;
        }
        public void SetAttackPayload(ActPair payload)
        {
            LastTarget = payload.Target;
        }
        public void ExcuteAttack()
        {
            if (LastTarget == null || !LastTarget.IsActive) return;

            _sendDamageContext = CreateDamageContext();
            LastTarget.TakeDamage(_sendDamageContext);

            //
        }

        public void AddAttackStatus(EStatusEffectType status, int amount)
        {
            _sendDamageContext.AddStatus(status, amount);
        }
        public DamageContext CreateDamageContext()
        {
            return new DamageContext(_owner, _owner.StatReadOnly.Get(ECreatureStatType.Damage), false);
        }
        public void TakeDamage(DamageContext damageContext)
        {
            _receivedDamageContext = damageContext;
            LastAttacker = _receivedDamageContext.Attacker;

            bool isBackAttack = DamageCalculator.IsBackAttack(LastAttacker, _owner);
            int finalDamage = CalculateFinalDamage(damageContext.Damage, isBackAttack);

            _owner.StatModifier.ModifyStat(new Stats.StatModifierContext(ECreatureStatType.CurrentHP, -finalDamage, ECreatureStatType.None));

            CombatEventBus.Excuter.RaiseDamageEvent(new DamageEvent(LastAttacker, _owner, finalDamage, isBackAttack));

            OnHitted?.Invoke(new AttackResultPayload(damageContext.Attacker, finalDamage, damageContext.IsCounterAttack));
        }
        public int CalculateFinalDamage(int baseDamage, bool isBackAttack)
        {
            int damage = baseDamage;

            if (isBackAttack)
            {
                float multiplier = GameManager.Config.Combat.BackAttackDMGMultiple;
                damage = DamageCalculator.CalculateBackAttackDamage(baseDamage, multiplier);
            }
            
            damage = damage * _owner.StatReadOnly.Get(ECreatureStatType.DamageTakeMultiplier);
            return damage;
        }

        public void AddNextAttackDamage(int damage) => _attackEnhanceContext.AddDamage(damage);
        public void AddNextAttackDamageMulti(float damage) => _attackEnhanceContext.AddDamageMulti(damage);
        public void AddNextAttackCount(int damage) => _attackEnhanceContext.AddExtraAttack(damage);

        public void OnCombatEnd()
        {
            _isAttacked = true;
            _isHitted = true;
            LastTarget = null;
            LastAttacker = null;
            _attackEnhanceContext.Reset();
        }
    }
}
