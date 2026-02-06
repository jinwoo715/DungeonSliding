using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using System.Collections.Generic;

namespace JW.DungeonSliding.GamePlay.Combat
{
    public interface IAttackRequestListener
    {
        void EnqueueActPair(ActPair pair);
        void EnqueueCounterActPair(ActPair pair);
    }

    public interface IDamageable
    {
        event Action OnHitDoneEvent;

        float DamageTakenMultiplier { get;}  // 받는 피해 배율
        ICombatant LastAttacker { get;}

        void AddDamageTakenMultiplier(float value);
        void TakeDamage(DamageContext damageInfo);
        void OnDeath();
    }
    public interface IAttackable
    {
        event Action OnAttackDoneEvent;
        ICombatant AttackTarget { get;}

        float DamageDealtMultiplier { get; }  // 가하는 피해 배율

        void AddDamageDealtMultiplier(float value);
        bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener);
        void StartAttackAnimation();
    }

    public interface ICounterAttackable
    {
        public event Action<ICombatant, ICombatant> OnCounterRequestedEvent;
        public void RequestCounterAttack(ICombatant target);
    }

    public interface IBarrierable
    {
        public void GainBarrier();
        public void ReleaseBarrier();
    }

    public interface INextAttackEnhancer
    {
        public void AddEnhance(ENextAttackType nextAttackEnhanceType, float value);
        public void ClearEnhance();
    }
    
    public enum ENextAttackType
    {
        None,
        Add,
        Multiple,
        ExtraAttack
    }

    // 3. 상태 및 버프 (선택 사항)
    public interface IStatusEffectable
    {
        public ECreatureStatus StatusFlags { get; }
        bool HasStatus(ECreatureStatus status);
        void ApplyStatus(ECreatureStatus status, int durationTurnCount);
        void RemoveStatus(ECreatureStatus status);
    }

    public interface IPlayerStatModifier
    {
        public void ModifyStat(PlayerApplyStatContext context);
    }

    public interface ICombatant : ITilePosition, IAttackable, IDamageable, IStatusEffectable
    {
        public bool IsActive { get; }
        public EDirectionType Direction { get; }
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