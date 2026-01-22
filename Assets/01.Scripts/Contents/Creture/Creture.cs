using System;
using System.Collections;
using UnityEngine;

namespace JW.SlidingPuzzle
{
    public abstract class Creture : MonoBehaviour, ICombatant
    {
        [SerializeField] protected AnimatorController _animatorController;

        public bool IsActive { get; private set; } = true;
        public EDirectionType Direction { get; private set; }
        public TilePoint Point { get; private set; }

        protected ICombatant _attackTarget;

        protected CretureStat _originCretureStat;
        protected CretureStat _currentCretureStat;

        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;

        public event Action<int, int> ChangeRemainHP;
        public event Action<int> ShowHitDamageUIEvent;

        public virtual void Init() 
        {
            IsActive = true;

            UnBindAnimEvent();

            BindAnimEvent();
        }
        private void OnDisable()
        {
            UnBindAnimEvent();

            _attackTarget = null;

            ChangeRemainHP = null;
            ShowHitDamageUIEvent = null;

            OnAttackDoneEvent = null;
            OnHitDoneEvent = null;
        }
        public virtual void SetCretureStat(CretureStat stat)
        {
            _originCretureStat = stat;
            _currentCretureStat = stat;
        }
        public void SetPosition(TilePoint point)
        {
            Point = point;
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
            float rotation = 0;
            switch (direction)
            {
                case EDirectionType.Left:
                    rotation = 270;
                    break;
                case EDirectionType.Up:
                    rotation = 0;
                    break;
                case EDirectionType.Right:
                    rotation = 90;
                    break;
                case EDirectionType.Down:
                    rotation = 180;
                    break;
            }

            return rotation;
        }
        public EDirectionType ToTargetDirection(TilePoint tile)
        {
            float xDistance = tile.XPos - this.Point.XPos;
            float zDistance = tile.ZPos - this.Point.ZPos;

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
        protected void ApplyDamage(DamageInfo damageInfo)
        {
            DamageInfo info = CalculateRealAppliedDamage(damageInfo);
            _currentCretureStat.HP -= info.Damage;

            if (_currentCretureStat.HP <= 0)
            {
                OnDeath();
            }

            ShowHitDamageUIEvent?.Invoke(info.Damage);
            ChangeRemainHP?.Invoke(_originCretureStat.HP, _currentCretureStat.HP);
        }
        protected virtual DamageInfo CalculateRealAppliedDamage(DamageInfo damageInfo)
        {
            return damageInfo;
        }
        protected virtual DamageInfo CreateDamageInfo()
        {
            DamageInfo damageInfo = new DamageInfo(this, _currentCretureStat.Damage, false);
            return damageInfo;
        }
        private void BindAnimEvent()
        {
            _animatorController.OnEndAttackAnimationEvent += EndAttackAnimation;
            _animatorController.OnEndHittedAnimationEvent += EndHittedAnimation;
            _animatorController.OnHitTimeingEvent += ApplyAttackDamage;
        }
        private void UnBindAnimEvent()
        {
            _animatorController.OnEndAttackAnimationEvent -= EndAttackAnimation;
            _animatorController.OnEndHittedAnimationEvent -= EndHittedAnimation;
            _animatorController.OnHitTimeingEvent -= ApplyAttackDamage;
        }

        //Bind Animation Event Trigger
        private void ApplyAttackDamage()
        {
            if (!IsActive || _attackTarget == null || !_attackTarget.IsActive) return;
            
            _attackTarget.GetHit(CreateDamageInfo());
        }
        public virtual void EndHittedAnimation()
        {
            Debug.Log("EndHittedAnimation");
            OnHitDoneEvent?.Invoke();
        }
        public virtual void EndAttackAnimation()
        {
            Debug.Log("EndAttackAnimation");
            OnAttackDoneEvent?.Invoke();
        }

        //Interface
        public virtual void Attack(ICombatant target)
        {
            _attackTarget = target;
        }
        public virtual void GetHit(DamageInfo damageInfo)
        {
            ApplyDamage(damageInfo);
        }
        public virtual void OnDeath()
        {
            IsActive = false;
        }

    }
}
