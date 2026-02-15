using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Creature : MonoBehaviour, ICombatant, ICounterAttackable, IBarrierable
    {
        [SerializeField] protected AnimatorController _animatorController;

        public bool IsActive { get; protected set; } = true;
        public EDirectionType Direction { get; private set; }
        public Tile TilePosition { get; private set; }
        public ICombatant LastAttacker { get; private set; }
        public ICombatant AttackTarget { get; protected set; }
        public bool IsCombat { get; private set; }
        public ECreatureStatus StatusFlags { get; private set; }
        protected Dictionary<ECreatureStatus, int> _statusDurations = new();
        private readonly List<ECreatureStatus> _statusKeys = new();

        public float DamageDealtMultiplier { get; private set; } = 1;
        public float DamageTakenMultiplier { get; private set; } = 1;
        
        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;

        public bool _isAttacked = false;
        public bool _isHitted = false;
        public Action<ActPair> OnCounterEvent { get; set; }

        public bool IsBarrierActive { get; private set; }

        protected DamageContext damageContext = new DamageContext();
        private ECretureType _cretureType;
        private ICombatEventListener _combatEventListener;

        public virtual void Init(ICombatEventListener combatEventListener, ECretureType cretureType) 
        {
            _combatEventListener = combatEventListener;
            _cretureType = cretureType;
            IsActive = true;
            BindAnimEvent();
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
        public IEnumerator CoRotateCharacter(EDirectionType directionType)
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
            if (_statusDurations.ContainsKey(ECreatureStatus.Stun)) return false;

            ECretureType searchType = _cretureType == ECretureType.Player ? ECretureType.Enemy : ECretureType.Player;
         
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
            return ApplyDamage(damageInfo);
        }
        private bool ApplyDamage(DamageContext damageInfo)
        {
            DamageContext info = CalculateRealAppliedDamage(damageInfo);

            if (info.Damage <= 0) return false;

            ReduceHP(info.Damage);

            _combatEventListener.RaiseDamageEvent(new DamageEvent(LastAttacker, this, info.Damage));

            return true;
        }
        protected abstract DamageContext CalculateRealAppliedDamage(DamageContext damageInfo);
        protected abstract void ReduceHP(int damage);
        public virtual void OnDeath()
        {
            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            
            _combatEventListener.RaiseDeathEvent(new DeathEvent(LastAttacker, this));

            ClearStatus();

            OnHitDoneEvent?.Invoke();
            OnAttackDoneEvent?.Invoke();
        }
        #endregion

        #region Animation
        protected virtual void ApplyAttack()
        {
            if (!IsActive || AttackTarget == null || !AttackTarget.IsActive) return;
            
            AttackTarget.TakeDamage(CreateDamageContext());

            damageContext.Reset();
        }
        public virtual void EndHittedAnimation()
        {
            OnHitDoneEvent?.Invoke();
        }
        public virtual void EndAttackAnimation()
        {
            OnAttackDoneEvent?.Invoke();
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
            UpdateStatusDuration();
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
        #endregion

        #region Status
        public bool HasStatus(ECreatureStatus status)
        {
            return status != ECreatureStatus.None && (StatusFlags & status) == status;
        }
        public void ApplyStatus(ECreatureStatus status, int duration)
        {
            if (duration <= 0 || status == ECreatureStatus.None) return;

            StatusFlags |= status;
            _statusDurations[status] = Mathf.Max(_statusDurations.GetValueOrDefault(status), duration);
        }
        public void RemoveStatus(ECreatureStatus status)
        {
            StatusFlags &= ~status;

            _statusDurations.Remove(status);
        }
        private void UpdateStatusDuration()
        {
            if (_statusDurations.Count == 0) return;

            _statusKeys.Clear();
            foreach (var kv in _statusDurations)
                _statusKeys.Add(kv.Key);

            foreach (var key in _statusKeys)
            {
                _statusDurations[key]--;

                if (_statusDurations[key] <= 0)
                {
                    _statusDurations.Remove(key);
                    StatusFlags &= ~key;
                }
            }
        }

        public void ClearStatus()
        {
            _statusDurations.Clear();
            StatusFlags = ECreatureStatus.None;
        }

        public bool TryGet<T>(out T service) where T : class
        {
            // object로 캐스팅하면 컴파일러가 "T가 뭔지 모르겠지만 일단 해봐"라고 허락해줍니다.
            service = (object)this as T;
            return service != null;
        }

        public IEnumerator CoRotateToTarget(ITilePosition combatant, Action DoneCallback = null)
        {
            EDirectionType dir = DirectionToTile(combatant.TilePosition);
            yield return StartCoroutine(CoRotateCharacter(dir));
            DoneCallback?.Invoke();
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

        #endregion
    }
}
