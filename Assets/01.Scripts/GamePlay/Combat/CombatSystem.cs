using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public class CombatSystem
    {
        private ICombatant _owner;
        private ICombatEventListener _combatEventListener;
        public ICombatant LastTarget { get; private set; }
        public ICombatant LastAttacker { get; private set; }

        private DamageContext _receivedDamageContext;
        private DamageContext _sendDamageContext;

        public bool IsCombated => _isAttacked || _isHitted;
        private bool _isAttacked = false;
        private bool _isHitted = false;

        public CombatSystem(ICombatant combatant)
        {
            _owner = combatant;
        }
        public void SetAttackTarget(ICombatant target)
        {
            LastTarget = target;
        }
        public void ExcuteAttack()
        {
            if (LastTarget == null || !LastTarget.IsActive) return;
        }
        public void AddAttackStatus(EStatusEffectType status, int amount)
        {
            _sendDamageContext.AddStatus(status, amount);
        }

        public void TakeDamage(DamageContext damageContext)
        {
            _receivedDamageContext = damageContext;
            LastAttacker = _receivedDamageContext.Attacker;

            //TODO 데미지 적용 공식 적용

            _combatEventListener.RaiseDamageEvent(new DamageEvent(LastAttacker, _owner, damageContext.Damage));
        }

        public void OnCombatEnd()
        {
            _isAttacked = true;
            _isHitted = true;
            LastTarget = null;
            LastAttacker = null;
        }
    }
}
