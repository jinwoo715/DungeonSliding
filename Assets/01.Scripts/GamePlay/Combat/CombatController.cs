using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public struct AttackResultPayLoad
    {
        public readonly ICombatant Target;
        public readonly int AppliedDamage;

        public AttackResultPayLoad(ICombatant target, int appliedDamage)
        {
            Target = target;
            AppliedDamage = appliedDamage;
        }
    }
    public struct HitResultPayload
    {
        public readonly ICombatant Attacker;
        public readonly int Damage;
        public readonly bool IsCounterAttack;

        public HitResultPayload(ICombatant attacker, int damage, bool isCounterAttacked)
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
    public struct TakeAttackPayLoad
    {
        public readonly ICombatant Attacker;
        public readonly int Damage;
        public TakeAttackPayLoad(ICombatant attacker, int damage)
        {
            Attacker = attacker;
            Damage = damage;
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

        private DamageContext _receivedDamageContext = new DamageContext();
        private DamageContext _sendDamageContext = new DamageContext();

        public event Action<AttackPreparePayLoad> OnPrepareAttack;
        public event Action<HitResultPayload> OnHitted;
        public event Action<TakeAttackPayLoad> OnBeforeHit;
        public event Action<AttackResultPayLoad> OnPerformedAttack;
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

            CreateDamageContext(nextAttackEnhancer);

            bool isBackAttack = DirectionUtility.IsBackAttack(_owner, LastTarget);

            if(isBackAttack == true || nextAttackEnhancer.IsGuaranteedCritical)
            {
                float criticalMultiplier = _owner.StatReadOnly.Get(ECreatureStatType.CriticalMultiplier);
                criticalMultiplier = criticalMultiplier / 100;

                _sendDamageContext.Damage = Mathf.RoundToInt(_sendDamageContext.Damage * criticalMultiplier);
                _sendDamageContext.IsCritical = true;
            }

            for (int i = 0; i <= nextAttackEnhancer.FinalExtraAttackCount; i++)
            {
                LastTarget.TakeDamage(_sendDamageContext);
            }

            OnPerformedAttack?.Invoke(new AttackResultPayLoad(LastTarget, _sendDamageContext.AppliedFinalDamage));
            _isAttacked = true;

            if (isBackAttack)
                OnBackAttacked?.Invoke();
        }

        public void AddAttackStatus(ECreatureStatus status, int amount)
        {
            _sendDamageContext.AddStatus(status, amount);
        }

        //TODO 데미지 계산 공식 수정
        public void CreateDamageContext(INextAttackEnhancer nextAttackEnhancer)
        {
            int damage = _owner.StatReadOnly.Get(ECreatureStatType.Damage);

            damage += nextAttackEnhancer.FinalEnhanceDamage;

            float dealMultiplier = _owner.StatReadOnly.Get(ECreatureStatType.DamageDealtMultiplier) / (float)100;

            damage = Mathf.RoundToInt(damage * dealMultiplier);

            _sendDamageContext.Attacker = _owner;
            _sendDamageContext.Damage = damage;
        }

        //TODO 피격 판정 수정
        public bool TryTakeDamage(DamageContext damageContext)
        {
            _receivedDamageContext = damageContext;
            LastAttacker = _receivedDamageContext.Attacker;

            OnBeforeHit?.Invoke(new TakeAttackPayLoad(damageContext.Attacker, damageContext.Damage));

            if (_owner.StatusReadOnly.HasStatus(ECreatureStatus.Barrier))
            {
                _owner.StatusModifier.RemoveStatus(ECreatureStatus.Barrier);
                damageContext.AppliedFinalDamage = 0;
                return false;
            }

            int finalDamage = CalculateFinalDamage(damageContext.Damage);
            damageContext.AppliedFinalDamage = finalDamage;


            _owner.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -finalDamage));

            CombatEventBus.Excuter.RaiseDamageEvent(new DamageEvent(LastAttacker, _owner, finalDamage, damageContext.IsCritical));

            OnHitted?.Invoke(new HitResultPayload(damageContext.Attacker, finalDamage, damageContext.IsCounterAttack));

            if(damageContext.Status.TryGetValue(ECreatureStatus.Execution, out int value))
            {
                int maxHP = _owner.StatReadOnly.Get(ECreatureStatType.MaxHp);
                int remainHP = _owner.StatReadOnly.Get(ECreatureStatType.CurrentHP);

                float excuteValue = (maxHP * value * 0.01f);

                if(remainHP <= excuteValue)
                    _owner.StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, EApplyStatType.Add, -remainHP));
            }

            //TODO상태이상 로직 분리
            foreach (var status in damageContext.Status)
            {
                var key = status.Key;

                if (key == ECreatureStatus.Execution)
                    continue;

                _owner.StatusModifier.ApplyStatus(status.Key, status.Value);
            }

            _isHitted = true;

            return true;
        }
        public int CalculateFinalDamage(int baseDamage)
        {
            int finalDamage = baseDamage;
            float takeMultiplier = _owner.StatReadOnly.Get(ECreatureStatType.DamageTakeMultiplier) / (float)100;

            finalDamage = Mathf.RoundToInt(baseDamage * takeMultiplier);
            return finalDamage;
        }

        public void OnCombatEnd()
        {
            _isAttacked = false;
            _isHitted = false;
            LastTarget = null;
            LastAttacker = null;
            _sendDamageContext.Clear();
            _receivedDamageContext.Clear();
        }
    }
}
