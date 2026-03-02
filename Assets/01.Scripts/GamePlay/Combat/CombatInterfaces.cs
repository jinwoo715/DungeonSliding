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
        event Action OnHitSequenceEnd;
        event Action OnDeathEvent;

        ICombatant LastAttacker { get;}

        void AddDamageTakenMultiplier(float value);
        bool TakeDamage(DamageContext damageInfo);
        void ApplyDamage(int damage);
        void OnDeath();
    }

    public interface IAttackRequester
    {
        bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener);
    }

    public interface IAttackable
    {
        event Action<ActPair> OnCounterAttackTriggered;
        event Action OnAttackSequenceEnd;
        ICombatant AttackTarget { get;}

        void AddDamageContextStatue(EStatusEffectType effectType, int amount);
        void AddDamageDealtMultiplier(float value);
        void StartAttackAnimation();
        void ExcuteAttack();
    }

    public interface ICounterAttackable
    {
        public void RequestCounterAttack(ICombatant target);
    }

    public interface IBarrierable
    {
        public bool IsBarrierActive { get; }
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

    public interface IStatusModifier
    {
        void ApplyStatus(ECreatureStatus status, int durationTurnCount);
        void RemoveStatus(ECreatureStatus status);
        void TimePassStatueUpdate();
        void ClearAllStatus();
    }
    public interface IStatusReadOnly
    {
        bool HasStatus(ECreatureStatus status);
    }

    public interface IPlayerStatModifier
    {
        public void ModifyStat(PlayerApplyStatContext context);
        public void SetCurrentHP(PlayerApplyStatContext context);
        public void SetCurrentMoveCount(PlayerApplyStatContext context);
    }

    public interface ICombatant : IAttackable, IDamageable, ICreatureRotator, ITileObject
    {
        public bool IsActive { get; }
        public bool IsCombat { get; }
        
        bool TryGet<T>(out T service) where T : class;

        IStatModifier StatModifier { get; }
        IStatReadOnly StatReadOnly { get; }

        IStatusModifier StatusModifier { get; }
        IStatusReadOnly StatusReadOnly { get; }

        IAttackRequester AttackRequester { get; }
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
        public bool GetCombatant(Tile tile, ECreatureType targetType, out ICombatant combatant);
        public int GetNearEnemyCount(Tile pivot);
    }

}