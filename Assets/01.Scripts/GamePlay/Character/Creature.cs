using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public abstract class Creature : MonoBehaviour, ICombatant
    {
        [SerializeField] protected AnimatorController _animatorController;

        public bool IsActive { get; private set; } = true;
        public EDirectionType Direction { get; private set; }
        public Tile TilePosition { get; private set; }
        public ICombatant LastAttacker { get; private set; }
        public ICombatant AttackTarget { get; private set; }
        public ECreatureStatus StatusFlags { get; private set; }
        private Dictionary<ECreatureStatus, int> _statusDurations = new();
        public float DamageDealtMultiplier { get; private set; } = 1;
        public float DamageTakenMultiplier { get; private set; } = 1;
        protected int _currentHP;

        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;
        private Action<Vector3, int> ShowHitDamageUIEvent;

        public IAttackRequestListener AttackRequestListener { get; private set; }
        public ICombatantSensor _sensor { get; private set; }

        public virtual void Init() 
        {
            IsActive = true;
            BindAnimEvent();
        }
        private void OnEnable()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.BattleEnd, TurnEnd);
        }
        private void OnDisable()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.BattleEnd, TurnEnd);
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
        protected void SetCharacterRotation(EDirectionType directionType)
        {
            if (directionType == EDirectionType.None)
                return;

            Direction = directionType;

            float rotation = GetEulerYByDirection(directionType);
            this.transform.rotation = Quaternion.Euler(0, rotation, 0);
        }
        public float GetEulerYByDirection(EDirectionType direction)
        {
            float rotation = (int)direction * 90;

            return rotation;
        }
        public EDirectionType ToTargetDirection(Tile tile)
        {
            float xDistance = tile.XPos - this.TilePosition.XPos;
            float zDistance = tile.ZPos - this.TilePosition.ZPos;

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
        #endregion

        #region Combat
        //Attack
        public abstract bool TrySubmitAttackRequest();
        public abstract void StartAttackAnimation();
        protected abstract DamageContext CreateDamageContext();

        //Take Damage
        public virtual void TakeDamage(DamageContext damageInfo)
        {
            LastAttacker = damageInfo.Attacker;
            ApplyDamage(damageInfo);
        }
        protected void ApplyDamage(DamageContext damageInfo)
        {
            DamageContext info = CalculateRealAppliedDamage(damageInfo);

            if (info.Damage <= 0) return;

            ReduceHP(info.Damage);

            ShowHitDamageUIEvent?.Invoke(this.transform.position + (Vector3.up * 2), info.Damage);
        }
        protected abstract DamageContext CalculateRealAppliedDamage(DamageContext damageInfo);
        protected abstract void ReduceHP(int damage);
        public virtual void OnDeath()
        {
            IsActive = false;
            _animatorController.SetAnimationTrigger(ConstString.STOP_ALL_TRIGGER_ANIMATION);
            OnHitDoneEvent?.Invoke();
        }
        #endregion


        #region Animation
        protected virtual void ApplyAttack()
        {
            if (!IsActive || AttackTarget == null || !AttackTarget.IsActive) return;

            AttackTarget.TakeDamage(CreateDamageContext());
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

        public virtual void TurnEnd()
        {
            UpdateStatusDuration();
        }

        //Wire
        public void SetCombatSensor(ICombatantSensor combatantSensor)
        {
            if (combatantSensor == null) Debug.LogError("CombatSensor Is Null");
            _sensor = combatantSensor;
        }
        public void SetAttackRequestListener(IAttackRequestListener requestListener)
        {
            if (requestListener == null) Debug.LogError("RequestListener Is Null");
            AttackRequestListener = requestListener;
        }
        public void SetShowHitDamageUIEvent(Action<Vector3, int> ShowHitDamageUIEvent)
        {
            this.ShowHitDamageUIEvent = ShowHitDamageUIEvent;
        }
        public void AddDamageDealtMultiplier(float value)
        {
            DamageDealtMultiplier += value;
        }
        public void AddDamageTakenMultiplier(float value)
        {
            DamageTakenMultiplier += value;
        }

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

            foreach (var status in _statusDurations)
            {
                ECreatureStatus key = status.Key;
                _statusDurations[key]--;
                if (_statusDurations[key] <= 0)
                {
                    StatusFlags &= ~key;
                    _statusDurations.Remove(key);
                }
            }
        }
        #endregion
    }
}
