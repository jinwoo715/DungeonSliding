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
    public abstract class Creature : MonoBehaviour, ICombatant
    {
        [SerializeField] protected AnimatorController _animatorController;
        [SerializeField] private AbilityExecuter _abilityExcuter;
        [SerializeField] private ObjectRotator _objectRotator;

        private ECreatureType _creatureType;
        
        private AttackRequester _attackRequester;
        private AttackEnhancer _attackEnhancer = new AttackEnhancer();
        private CreatureStat _stat = new CreatureStat();
        private CombatController _combatController;
        private GridPositioner _gridTransform;
        private StatusEffectManager _status;

        public bool IsActive { get; protected set; } = true;

        public event Action OnAttackSequenceEnd;
        public event Action OnHitSequenceEnd;

        public event Action OnDeathEvent;

        private event Action<IAttackRequester, int> _unRegisterRequesterEvent;

        public IStatModifier StatModifier => _stat;
        public IStatReadOnly StatReadOnly => _stat;
        public IStatusModifier StatusModifier => _status;
        public IStatusReadOnly StatusReadOnly => _status;
        public ITileObject TileObject => _gridTransform;
        public IRotateObject Rotate => _objectRotator;
        public IAbilityRegister AbilityRegister => _abilityExcuter;
        public IAbilityExcuter Ability => _abilityExcuter;
        public INextAttackEnhancer NextAttackEnhancer => _attackEnhancer;
        public IAttackRequester AttackRequester => _attackRequester;

        private void OnDestroy()
        {
            UnBindAnimEvent();
        }
        public virtual void Initialize(ECreatureType cretureType)
        {
            IsActive = true;
            _creatureType = cretureType;
            _objectRotator.SetOwner(this.transform);

            _gridTransform = new GridPositioner(this.transform);
            _attackRequester = new AttackRequester(this, _creatureType);
            _combatController = new CombatController(this);
            _status = new StatusEffectManager();
            _attackEnhancer.Init(_stat);

            _combatController.OnPrepareAttack += (value) => _abilityExcuter.ExecuteCreatureTrigger(ECreatureTrigger.OnAttackPrepared, value);
            _combatController.OnPerformedAttack += (value) => _abilityExcuter.ExecuteCreatureTrigger(ECreatureTrigger.OnAttackPerformed, value);
            _combatController.OnBackAttacked += () => Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnBackAttacked);
            _combatController.OnHitted += (value) => Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnAfterHitted, value);
            _combatController.OnBeforeHit += (value) => Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnBeforeHitted, value);

            _stat.OnStatChanged += (value) => { _attackEnhancer.CalculateFinalExtraDamage(); };

            _attackRequester.OnRegisterAttack += () => _abilityExcuter.ExecuteCreatureTrigger(ECreatureTrigger.OnRegisterAttack);

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnBattleEnd, OnBattleEnd);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, OnTurnEnd);

            BindAnimEvent();
        }
        public void InitData(CreatureBaseStat baseStat)
        {
            _stat.Init(baseStat);
        }
        public void RegisterRequester(IAttackRegister requesterRegistry)
        {
            requesterRegistry.RegisterAttackRequester(_attackRequester, (int)_creatureType);
            _unRegisterRequesterEvent += requesterRegistry.UnRegisterAttackRequester;
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
        public bool IsCanRotate()
        {
            return !(_status.HasStatus(ECreatureStatus.Bind) || _status.HasStatus(ECreatureStatus.Stun));
        }

        #region Combat
        public virtual bool TakeDamage(DamageContext damageInfo)
        {
            if (_combatController.TryTakeDamage(damageInfo))
            {
                CheckHPOut();
                return true;
            }
            else
            {
                EndHittedAnimation();
                return false;
            }
        }
        
        public virtual void ExcuteAttack(ActPair actPair)
        {
            _combatController.SetAttackPayload(actPair);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
        public void AddStatusEffect(ECreatureStatus effectType, int amount)
        {
            _combatController.AddAttackStatus(effectType, amount);
        }
        public virtual void OnDeath()
        {
            if (IsActive == false) return;

            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            CombatEventBus.Excuter.RaiseDeathEvent(new DeathEvent(_combatController.LastAttacker, this));

            _status.Reset();
            _stat.Clear();

            OnHitSequenceEnd?.Invoke();
            OnAttackSequenceEnd?.Invoke();

            OnDeathEvent?.Invoke();

            _abilityExcuter.Clear();

            _unRegisterRequesterEvent?.Invoke(this._attackRequester, (int)_creatureType);
        }
        void CheckHPOut()
        {
            if (!IsOutOfHp()) return;
                Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnDeathByHp);

            if (!IsOutOfHp()) return;
                Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnDeath);

            if (!IsOutOfHp()) return;

            OnDeath();
        }
        bool IsOutOfHp()
        {
            int remainHP = _stat.Get(ECreatureStatType.CurrentHP);
            return remainHP == 0;
        }
        private void CheckMoveOut()
        {
            if (!IsOutOfMoveCount()) return;
            Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnDeathByMoveCount);

            if (!IsOutOfMoveCount()) return;
            Ability.ExecuteCreatureTrigger(ECreatureTrigger.OnDeath);

            if (!IsOutOfMoveCount()) return;

            OnDeath();
        }
        public bool IsOutOfMoveCount()
        {
            int remainMove = StatReadOnly.Get(ECreatureStatType.CurrentMoveCount);
            return remainMove == 0;
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
            _combatController.ExcuteAttack(_attackEnhancer);
        }
        #endregion

        #region Sequence Method
        public virtual void OnTurnEnd()
        {
            if (!IsActive)
                return;

            _status.TimePassStatueUpdate();
            CheckMoveOut();
        }
        public virtual void OnBattleEnd()
        {
            BattleResultPayLoad turnResultPayLoad = new BattleResultPayLoad(_combatController.IsCombated);
            _abilityExcuter.SendPayload(turnResultPayLoad);
            _combatController.OnCombatEnd();
            _attackEnhancer.Clear();
        }
        #endregion
        public bool TryGet<T>(out T service) where T : class
        {
            service = (object)this as T;
            return service != null;
        }
    }
}
