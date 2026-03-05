using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.GamePlay.Statues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Creature : MonoBehaviour, ICombatant, INextAttackEnhancer
    {
        [SerializeField] protected AnimatorController _animatorController;
        [SerializeField] private AbilityExecuter _abilityExcuter;
        
        private CreatureStat _stat;
        private StatusEffectManager _statusManager;
        private AttackRequester _attackRequester;
        private CombatSystem _combatSystem;
        private GridPositioner _gridTransform;
        private ObjectRotator _objectRotator;

        public bool IsActive { get; protected set; } = true;

        public event Action OnAttackSequenceEnd;
        public event Action OnHitSequenceEnd;

        public event Action OnDeathEvent;

        private Action<IAttackRequester> _unRegisterRequesterEvent;

        public IStatModifier StatModifier => _stat;
        public IStatReadOnly StatReadOnly => _stat;
        public IStatusModifier StatusModifier => _statusManager;
        public IStatusReadOnly StatusReadOnly => _statusManager;
        public ITileObject Tile => _gridTransform;
        public IRotateObject Rotate => _objectRotator;

        private ECreatureType _creatureType;

        public virtual void Initialize(ECreatureType cretureType, IAttackRequestListener attackRequestListener)
        {
            IsActive = true;
            _creatureType = cretureType;

            _objectRotator = new ObjectRotator(this.transform);
            _gridTransform = new GridPositioner(this.transform);
            _combatSystem = new CombatSystem(this);
            _attackRequester = new AttackRequester(this, _creatureType, attackRequestListener);
            _statusManager = new StatusEffectManager();

            BindAnimEvent();
        }

        public void SetData(CreatureBaseStat baseStat)
        {
            _stat = new CreatureStat(baseStat);
        }
        public void RegisterRequester(Action<IAttackRequester> registerAction, Action<IAttackRequester> unregisterAction)
        {
            registerAction?.Invoke(_attackRequester);
            _unRegisterRequesterEvent = unregisterAction;
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
        private void OnEnable()
        {
            GameTriggerEventBus.Instance?.SubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, OnTurnEnd);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance?.UnSubscribeTriggerEvent(EGameTriggerType.OnTurnEnd, OnTurnEnd);
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
            _combatSystem.SetAttackPayload(actPair.Target);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
        public void AddStatusEffect(EStatusEffectType effectType, int amount)
        {
            _combatSystem.AddAttackStatus(effectType, amount);
        }
        public virtual void OnDeath()
        {
            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            CombatEventBus.Excuter.RaiseDeathEvent(new DeathEvent(_combatSystem.LastAttacker, this));

            _statusManager.Reset();
            _stat.Reset();

            OnHitSequenceEnd?.Invoke();
            OnAttackSequenceEnd?.Invoke();

            OnDeathEvent?.Invoke();

            _unRegisterRequesterEvent?.Invoke(this._attackRequester);
        }

        bool IsCheckDie()
        {
            int currentHp = _stat.Get(ECreatureStatType.CurrentHP);
            int currentMove = _stat.Get(ECreatureStatType.CurrentMoveCount);

            if (currentHp <= 0)
                ExecuteCreatureEvent(ECreatureTrigger.OnDeathByHp);

            if (currentMove <= 0)
                ExecuteCreatureEvent(ECreatureTrigger.OnDeathByMoveCount);

            currentHp = _stat.Get(ECreatureStatType.CurrentHP);
            currentMove = _stat.Get(ECreatureStatType.CurrentMoveCount);

            if (currentHp <= 0 || currentMove <= 0)
                ExecuteCreatureEvent(ECreatureTrigger.OnDeath);

            currentHp = _stat.Get(ECreatureStatType.CurrentHP);
            currentMove = _stat.Get(ECreatureStatType.CurrentMoveCount);

            if (currentHp <= 0 || currentMove <= 0)
                return true;

            return false;
        }

        private void ExecuteCreatureEvent(ECreatureTrigger type)
        {
            _abilityExcuter.ExecuteCreatureTrigger(type);
        }

        #endregion

        #region Animation CallBack
        public virtual void EndHittedAnimation()
        {
            OnHitSequenceEnd?.Invoke();
        }
        public virtual void EndAttackAnimation()
        {
            OnAttackSequenceEnd?.Invoke();
        }
        protected virtual void ApplyAttack()
        {
            if (!IsActive) return;
            _combatSystem.ExcuteAttack();
        }
        #endregion

        #region Sequence Method
        public void OnTurnEnd()
        {
            _statusManager.TimePassStatueUpdate();
            _combatSystem.OnCombatEnd();

            if(IsCheckDie())
            {
                OnDeath();
            }
        }
        #endregion

        public bool TryGet<T>(out T service) where T : class
        {
            service = (object)this as T;
            return service != null;
        }

        public void AddDamage(int damage) => _combatSystem.AddNextAttackDamage(damage);
        public void AddDamageMulti(float multi) => _combatSystem.AddNextAttackDamageMulti(multi);
        public void AddAttackCount(int count) => _combatSystem.AddNextAttackCount(count);
    }
}
