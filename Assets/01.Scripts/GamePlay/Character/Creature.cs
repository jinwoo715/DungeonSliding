using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.GamePlay.Statues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Creature : MonoBehaviour, ICombatant, ICounterAttackable, IBarrierable, IAttackRequester
    {
        [SerializeField] protected AnimatorController _animatorController;

        public bool IsActive { get; protected set; } = true;
        public EDirectionType Direction { get; private set; }
        public Tile TilePosition { get; private set; }
        public ICombatant LastAttacker { get; private set; }
        public ICombatant AttackTarget { get; protected set; }
        public bool IsCombat { get; private set; }

        public event Action OnAttackSequenceEnd;
        public event Action OnHitSequenceEnd;
        public event Action<ActPair> OnCounterAttackTriggered;
        public event Action OnDeathEvent;

        public bool _isAttacked = false;
        public bool _isHitted = false;
        public Action<ActPair> OnCounterEvent { get; set; }
        public bool IsBarrierActive { get; private set; }

        public IStatModifier StatModifier => throw new NotImplementedException();
        public IStatReadOnly StatReadOnly => throw new NotImplementedException();
        public IStatusModifier StatusModifier => _statusManager;
        public IStatusReadOnly StatusReadOnly => _statusManager;

        protected DamageContext damageContext = new DamageContext();
        private ECreatureType _cretureType;
        private ICombatEventListener _combatEventListener;

        private CreatureStat _stat;
        private StatusEffectManager _statusManager;
        private AttackRequester _attackRequester;

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

        #region Position And Rotation
        public void SetPosition(Tile point)
        {
            TilePosition = point;
            this.transform.position = point.GetPosition;
        }

        public float GetEulerYByDirection(EDirectionType direction)
        {
            float rotation = (int)direction * 90;

            return rotation;
        }
        public EDirectionType DirectionToTile(Tile tile)
        {
            float xDistance = tile.X - this.TilePosition.X;
            float zDistance = tile.Z - this.TilePosition.Z;

            if(Mathf.Abs(xDistance) >= Mathf.Abs(zDistance))
            {
                if (xDistance >= 0) return EDirectionType.Right;
                else return EDirectionType.Left;
            }
            else
            {
                if (zDistance >= 0) return EDirectionType.Up;
                else return EDirectionType.Down;
            }
        }
        public EDirectionType ReverseDirection(EDirectionType directionType)
        {
            int reverse = (int)directionType + 2;
            reverse = reverse % 4;

            return (EDirectionType)reverse;
        }

        public bool IsCanRotate()
        {
            return !(_statusManager.HasStatus(ECreatureStatus.Bind) || _statusManager.HasStatus(ECreatureStatus.Stun));
        }
        public IEnumerator CoRotateToDirection(EDirectionType directionType)
        {
            if (directionType != Direction)
            {
                float timer = 0;
                const float rotationDuration = 1f; // 1초 동안 회전

                float startRotationY = this.transform.rotation.eulerAngles.y;
                float targetRotationY = GetEulerYByDirection(directionType);

                while (timer < 1f)
                {
                    timer += Time.deltaTime / rotationDuration; // duration으로 나눠야 정확히 1초 걸림

                    // LerpAngle을 써야 270도에서 0(360)도로 갈 때 최단 거리로 회전함
                    float rotationValue = Mathf.LerpAngle(startRotationY, targetRotationY, timer);
                    this.transform.rotation = Quaternion.Euler(0, rotationValue, 0);

                    yield return null;
                }

                SetRotation(directionType);
            }
        }
        public IEnumerator CoRotateToTarget(ITileObject combatant, Action DoneCallback = null)
        {
            EDirectionType dir = DirectionToTile(combatant.TilePosition);
            yield return StartCoroutine(CoRotateToDirection(dir));
            DoneCallback?.Invoke();
        }

        public void SetRotation(EDirectionType directionType)
        {
            if (directionType == EDirectionType.None)
                return;

            Direction = directionType;

            float rotation = GetEulerYByDirection(directionType);
            this.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
        #endregion

        #region Combat
        //Attack
        public bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener)
        {
            if (_statusManager.HasStatus(ECreatureStatus.Stun)) return false;

            ECreatureType searchType = _cretureType == ECreatureType.Player ? ECreatureType.Enemy : ECreatureType.Player;
         
            if (sensor.GetCombatant(TilePosition.GetNextTile(Direction), searchType, out var target))
            {
                AttackTarget = target;
                attackRequestListener.EnqueueActPair(new ActPair(this, target));
                _isAttacked = true;
                return true;
            }
            else
            {
                AttackTarget = null;
                return false;
            }
        }
        public abstract void StartAttackAnimation();
        protected abstract DamageContext CreateDamageContext();

        //Take Damage
        public virtual bool TakeDamage(DamageContext damageInfo)
        {
            _isHitted = true;

            if (IsBarrierActive)
            {
                ReleaseBarrier();
                EndHittedAnimation();
                return false;
            }

            LastAttacker = damageInfo.Attacker;

            damageInfo.Damage = CalculateRealAppliedDamage(damageInfo.Damage);
            if (damageInfo.Damage <= 0) return false;

            return ApplyDamage(damageInfo);
        }
        private bool ApplyDamage(DamageContext damageInfo)
        {
            int damage = CalculateRealAppliedDamage(damageInfo.Damage);

            if (damage <= 0) return false;

            ApplyDamage(damage);

            _combatEventListener.RaiseDamageEvent(new DamageEvent(LastAttacker, this, damage));

            return true;
        }
        protected abstract int CalculateRealAppliedDamage(int takeDamage);
        public virtual void OnDeath()
        {
            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            
            _combatEventListener.RaiseDeathEvent(new DeathEvent(LastAttacker, this));

            _statusManager.ClearAllStatus();

            OnHitSequenceEnd?.Invoke();
            OnAttackSequenceEnd?.Invoke();
        }
        #endregion

        #region Animation
        protected virtual void ApplyAttack()
        {
            if (!IsActive || AttackTarget == null || !AttackTarget.IsActive) return;

            Debug.Log($"{this.name} : Attacked");
            AttackTarget.TakeDamage(CreateDamageContext());

            damageContext.Reset();
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
        #endregion
        public virtual void OnBattleEnd()
        {
            IsCombat = _isAttacked || _isHitted;
        }
        public void OnTurnEnd()
        {
            _statusManager.TimePassStatueUpdate();
        }
        public void OnTurnStart()
        {
            _isAttacked = false;
            _isHitted = false;
        }

        #region Stat
        public virtual void AddDamageDealtMultiplier(float value)
        {
            DamageDealtMultiplier += value;
        }
        public void AddDamageTakenMultiplier(float value)
        {
            DamageTakenMultiplier += value;
        }
        public void ApplyDamage(int damage)
        {
            _stat.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentHP, StatModifyType.Add, -damage));
        }
        
        #endregion

        #region Status

        public bool TryGet<T>(out T service) where T : class
        {
            // object로 캐스팅하면 컴파일러가 "T가 뭔지 모르겠지만 일단 해봐"라고 허락해줍니다.
            service = (object)this as T;
            return service != null;
        }
        
        public void RequestCounterAttack(ICombatant target)
        {
            OnCounterEvent?.Invoke(new ActPair(this, target));
        }
        public virtual void GainBarrier()
        {
            IsBarrierActive = true;
        }
        public virtual void ReleaseBarrier()
        {
            IsBarrierActive = false;
        }
        public void AddDamageContextStatue(EStatusEffectType effectType, int amount)
        {
            damageContext.StatusEffect = effectType;
            damageContext.StatusAmount = amount;
        }

        public void ExcuteAttack()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
