using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Stats;
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
    public struct AttackPreparePayLoad
    {
        public ICombatant Target;

        public AttackPreparePayLoad(ICombatant target)
        {
            Target = target;
        }
    }
    public struct BattleResultPayLoad
    {
        public bool IsCombatted;
        public BattleResultPayLoad(bool isCombatted)
        {
            IsCombatted = isCombatted;
        }
    }

    public interface ICriticalSetter
    {
        public bool IsCritical { get; }
        public float CriticalValue { get; }
        public void AddCriticalValue(float value);
        public void SetCritical(float value);
    }

    public class CombatController
    {
        private ICombatant _owner;
        public ICombatant LastTarget { get; private set; }
        public ICombatant LastAttacker { get; private set; }

        private DamageContext _receivedDamageContext;
        private DamageContext _sendDamageContext;

        public event Action<AttackPreparePayLoad> OnPrepareAttack;
        public event Action<AttackResultPayload> OnHitted;
        public event Action OnPerformedAttack;
        public event Action OnBackAttacked;

        public bool IsCombated => _isAttacked || _isHitted;
        private bool _isAttacked = false;
        private bool _isHitted = false;

        public CombatController(ICombatant combatant)
        {
            _owner = combatant;
        }
        public void SetAttackPayload(ActPair payload)
        {
            LastTarget = payload.Target;

            AttackPreparePayLoad preparePayLoad = new AttackPreparePayLoad(payload.Target);
            OnPrepareAttack?.Invoke(preparePayLoad);
        }
        public void ExcuteAttack(INextAttackEnhancer nextAttackEnhancer)
        {
            if (LastTarget == null || !LastTarget.IsActive) return;

            _sendDamageContext = CreateDamageContext(nextAttackEnhancer);

            for (int i = 0; i <= nextAttackEnhancer.FinalExtraAttackCount; i++)
            {
                LastTarget.TakeDamage(_sendDamageContext);
            }

            OnPerformedAttack?.Invoke();

            bool isBackAttack = DirectionUtility.IsBackAttack(_owner, LastTarget);

            if (isBackAttack)
                OnBackAttacked?.Invoke();
        }

        public void AddAttackStatus(EStatusEffectType status, int amount)
        {
            _sendDamageContext.AddStatus(status, amount);
        }

        //TODO 데미지 계산 공식 수정
        public DamageContext CreateDamageContext(INextAttackEnhancer nextAttackEnhancer)
        {
            int damage = _owner.StatReadOnly.Get(ECreatureStatType.Damage);

            damage += nextAttackEnhancer.FinalEnhanceDamage;

            float dealMultiplier = _owner.StatReadOnly.Get(ECreatureStatType.DamageDealtMultiplier) / (float)100;
            damage = Mathf.RoundToInt(damage * dealMultiplier);

            return new DamageContext(_owner, damage, false);
        }

        //TODO 피격 판정 수정
        public void TakeDamage(DamageContext damageContext)
        {
            _receivedDamageContext = damageContext;
            LastAttacker = _receivedDamageContext.Attacker;

            bool isBackAttack = DirectionUtility.IsBackAttack(LastAttacker, _owner);

            float takeDealMultiplier = _owner.StatReadOnly.Get(ECreatureStatType.DamageTakeMultiplier) / (float)100;
            int finalDamage = CalculateFinalDamage(damageContext.Damage, isBackAttack);

            finalDamage = Mathf.RoundToInt(finalDamage * takeDealMultiplier);

            _owner.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -finalDamage));

            CombatEventBus.Excuter.RaiseDamageEvent(new DamageEvent(LastAttacker, _owner, finalDamage, isBackAttack));

            OnHitted?.Invoke(new AttackResultPayload(damageContext.Attacker, finalDamage, damageContext.IsCounterAttack));

            if(damageContext.Status.TryGetValue(EStatusEffectType.Execution, out int value))
            {
                int maxHP = _owner.StatReadOnly.Get(ECreatureStatType.MaxHp);
                int remainHP = _owner.StatReadOnly.Get(ECreatureStatType.CurrentHP);

                int remainRatio = Mathf.RoundToInt(((float)remainHP / maxHP) * 100);

                if(remainRatio <= value)
                    _owner.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -remainHP));
            }
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

        public void OnCombatEnd()
        {
            _isAttacked = true;
            _isHitted = true;
            LastTarget = null;
            LastAttacker = null;
        }
    }
}
