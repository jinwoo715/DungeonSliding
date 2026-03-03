using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.GamePlay.Statues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Creature : MonoBehaviour, ICombatant
    {
        [SerializeField] protected AnimatorController _animatorController;

        public bool IsActive { get; protected set; } = true;

        public event Action OnAttackSequenceEnd;
        public event Action OnHitSequenceEnd;
        public event Action<ActPair> OnCounterAttackTriggered;

        public Action OnDeathEvent;

        public IStatModifier StatModifier => _stat;
        public IStatReadOnly StatReadOnly => _stat;
        public IStatusModifier StatusModifier => _statusManager;
        public IStatusReadOnly StatusReadOnly => _statusManager;
        public ITileObject Tile => _gridTransform;
        public IRotateObject Rotate => _objectRotator;

        private ECreatureType _cretureType;
        private ICombatEventListener _combatEventListener;

        private CreatureStat _stat;
        private StatusEffectManager _statusManager;
        private AttackRequester _attackRequester;
        private CombatSystem _combatSystem;
        private GridPositioner _gridTransform;
        private ObjectRotator _objectRotator;

        public virtual void Init(ICombatEventListener combatEventListener, ECreatureType cretureType)
        {
            _combatEventListener = combatEventListener;
            _cretureType = cretureType;
            IsActive = true;
            BindAnimEvent();
        }
        public void RegisterRequester(Action<IAttackRequester> registerAction)
        {
            registerAction?.Invoke(_attackRequester);
        }
        public void UnRegisterRequester(Action<IAttackRequester> unregisterAction)
        {
            unregisterAction?.Invoke(_attackRequester);
        }

        private void OnEnable()
        {
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnBattleEnd, OnBattleEnd);
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, OnTurnEnd);
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnStart, OnTurnStart);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameTriggerType.OnBattleEnd, OnBattleEnd);
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, OnTurnEnd);
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnStart, OnTurnStart);
        }
        private void OnDestroy()
        {
            UnBindAnimEvent();
        }

        public bool IsCanRotate()
        {
            return !(_statusManager.HasStatus(ECreatureStatus.Bind) || _statusManager.HasStatus(ECreatureStatus.Stun));
        }

        #region Combat
        public virtual void TakeDamage(DamageContext damageInfo)
        {
            _combatSystem.TakeDamage(damageInfo);
            _animatorController.SetAnimationTrigger(ConstString.HIT_ANIM);
        }
        public virtual void ExcuteAttack(ActPair actPair)
        {
            _combatSystem.SetAttackTarget(actPair.Target);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
        protected virtual void ApplyAttack()
        {
            if (!IsActive) return;
            _combatSystem.ExcuteAttack();
        }
        #endregion

        public virtual void OnDeath()
        {
            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            
            _combatEventListener.RaiseDeathEvent(new DeathEvent(_combatSystem.LastAttacker, this));

            _statusManager.ClearAllStatus();

            OnHitSequenceEnd?.Invoke();
            OnAttackSequenceEnd?.Invoke();
        }
        public virtual void EndHittedAnimation()
        {
            OnHitSequenceEnd?.Invoke();
        }
        public virtual void EndAttackAnimation()
        {
            OnAttackSequenceEnd?.Invoke();
        }
        private void BindAnimEvent()
        {
            _animatorController.OnEndAttackAnimationEvent += EndAttackAnimation;
            _animatorController.OnEndHittedAnimationEvent += EndHittedAnimation;
            _animatorController.OnHitTimeingEvent += ApplyAttack;
        }
        private void UnBindAnimEvent()
        {
            _animatorController.OnEndAttackAnimationEvent -= EndAttackAnimation;
            _animatorController.OnEndHittedAnimationEvent -= EndHittedAnimation;
            _animatorController.OnHitTimeingEvent -= ApplyAttack;
        }
        public virtual void OnBattleEnd()
        {
           
        }
        public void OnTurnEnd()
        {
            _statusManager.TimePassStatueUpdate();
        }
        public void OnTurnStart()
        {
            _combatSystem.OnCombatEnd();
            
        }
        public bool TryGet<T>(out T service) where T : class
        {
            service = (object)this as T;
            return service != null;
        }
        public void AddStatusEffect(EStatusEffectType effectType, int amount)
        {
            _combatSystem.AddAttackStatus(effectType, amount);
        }
    }
}
