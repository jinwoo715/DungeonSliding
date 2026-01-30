using JW.DungeonSliding.GamePlay.Entities;
using System;
using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface IAttackRequestListener
    {
        
        public void RegisterActpair(ActPair pair);

    }

    public interface IDamageable
    {
        public event Action OnHitDoneEvent;
        public event Action<ICombatant, ICombatant> OnCounterRequestedEvent;
        public int CurrentHP { get; }
        public float DamageTakenMultiplier { get; set; }  // 받는 피해 배율
        public ICombatant LastAttacker { get;}
        public void GainBarrier();
        public void GetHit(DamageInfo damageInfo);
        public void RequestCounterAttack(ICombatant target);
        public void OnDeath();
    }
    public interface IAttackable
    {
        public event Action OnAttackDoneEvent;
        public IAttackRequestListener _attackRequestListener { get; }
        public ICombatant AttackTarget { get;}
        public NextAttackBuff AttackBuff { get; }
        float DamageDealtMultiplier { get; set; }  // 가하는 피해 배율
        public void SetAttackRequestListener(IAttackRequestListener requestListener);
        public void RegisterAttack();
        public void Attack(ICombatant target);
    }

    // 3. 상태 및 버프 (선택 사항)
    public interface IStatusEffectable
    {
        public ECreatureStatus CreateStatus { get; }
        void ApplyBind(ECreatureStatus State, int duration);
    }

    public interface IStatModifier
    {
        public void ModifyStat(ApplyStatContext context);
    }

    public interface ICombatant : ITilePosition, IAttackable, IDamageable, IStatusEffectable, IStatModifier
    {
        public bool IsActive { get; }
        public EDirectionType Direction { get; }
        public void SetCombatSensor(ICombatantSensor combatantSensor);
    }

    public interface ICombatProvider
    {
        public bool TryGetCombatant(Tile tilePoint, out ICombatant combatant);
        public List<ICombatant> GetAllActiveCombatant();
    }
    public interface ICombatantSensor
    {
        public ICombatant PlayerCombatant { get; }
        public List<ICombatant> AllEnemyCombatants { get; }
        public bool GetCombatant(Tile tile, ECretureType targetType, out ICombatant combatant);
        public int GetNearCambatantCount(ICombatant except);
    }
}