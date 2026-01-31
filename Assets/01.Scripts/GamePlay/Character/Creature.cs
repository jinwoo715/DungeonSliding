using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using System;
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

        public float DamageDealtMultiplier { get; set; }
        public float DamageTakenMultiplier { get; set; }
        
        public ECreatureStatus CreateStatus => throw new NotImplementedException();
        protected ICombatant _attackTarget;
        
        [SerializeField] protected int _currentHP;

        public event Action OnAttackDoneEvent;
        public event Action OnHitDoneEvent;
        public event Action<int> ShowHitDamageUIEvent;
        public event Action<ICombatant, ICombatant> OnCounterRequestedEvent;
        

        public IAttackRequestListener _attackRequestListener { get; private set; }
        public ICombatantSensor _sensor { get; private set; }

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

            ShowHitDamageUIEvent = null;

            OnAttackDoneEvent = null;
            OnHitDoneEvent = null;
        }

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
        protected void ApplyDamage(DamageInfo damageInfo)
        {
            DamageInfo info = CalculateRealAppliedDamage(damageInfo);

            Debug.Log($"Damage {info.Damage}");

            if (info.Damage <= 0) return;

            ModifyStat(new ApplyStatContext(EPlayerStat.HP, EApplyStatType.Add, -info.Damage, EPlayerStat.None));

            if (_currentHP <= 0)
            {
                OnDeath();
            }

            ShowHitDamageUIEvent?.Invoke(info.Damage);
        }
        protected virtual DamageInfo CalculateRealAppliedDamage(DamageInfo damageInfo)
        {
            return damageInfo;
        }
        protected abstract DamageInfo CreateDamageInfo();
      
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
            OnHitDoneEvent?.Invoke();
        }
        public abstract void ModifyStat(ApplyStatContext context);
        public void GainBarrier()
        {
            throw new NotImplementedException();
        }
        public void RequestCounterAttack(ICombatant target)
        {
            throw new NotImplementedException();
        }
        public void ApplyBind(ECreatureStatus State, int duration)
        {
            throw new NotImplementedException();
        }
        public abstract void RegisterAttack();
        public void SetCombatSensor(ICombatantSensor combatantSensor)
        {
            if (combatantSensor == null) Debug.LogError("CombatSensor Is Null");
            _sensor = combatantSensor;
        }
        public void SetAttackRequestListener(IAttackRequestListener requestListener)
        {
            if (requestListener == null) Debug.LogError("RequestListener Is Null");
            _attackRequestListener = requestListener;
        }
        
    }
}
