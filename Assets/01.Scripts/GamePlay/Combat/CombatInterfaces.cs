using JW.DungeonSliding.GamePlay.Entities;
using System;
using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface IAttackRequestListener
    {
        public void ReceiveAttackRequest(IAttackable attacker, IDamageable target);
    }

    public interface IDamageable
    {
        public event Action OnHitDoneEvent;
        public event Action<ICombatant, ICombatant> OnCounterRequestedEvent;
        public int CurrentHP { get; }
        public float DamageTakenMultiplier { get; set; }  // 받는 피해 배율
        public ICombatant LastAttacker { get; set; }
        public void GainBarrier();
        public void GetHit(DamageInfo damageInfo);
        public void RequestCounterAttack(ICombatant target);
        public void OnDeath();
    }
    public interface IAttackable
    {
        public event Action OnAttackDoneEvent;
        public ICombatant AttackTarget { get; set; }
        public NextAttackBuff CurrentAttackBuff { get; }
        float DamageDealtMultiplier { get; set; }  // 가하는 피해 배율
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
    }

    public interface ICombatProvider
    {
        public bool TryGetCombatant(Tile tilePoint, out ICombatant combatant);
        public List<ICombatant> GetAllCombatant();
    }
    public interface ICombatantSensor
    {
        public int GetNearCambatantCount(ICombatant except);
    }
}