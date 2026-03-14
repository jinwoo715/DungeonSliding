using JW.DungeonSliding.GamePlay.Ability;
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
        void TakeDamage(DamageContext damageInfo);
    }

    public interface IAttackRequester
    {
        public event Action<ActPair> OnRequestAttack;
        public event Action<ActPair> OnRequestCounterAttack;
        bool TrySubmitAttackRequest(ICombatantSensor sensor);
        void RequestCounterAttack(ICombatant target);
    }

    public interface IAttackable
    {
        event Action OnAttackSequenceEnd;
        void AddStatusEffect(EStatusEffectType effectType, int amount);
        void ExcuteAttack(ActPair actPair);
    }

    public interface IBarrierable
    {
        public bool IsBarrierActive { get; }
        public void GainBarrier();
        public void ReleaseBarrier();
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
        public event Action<ECreatureStatus> OnAppliedStatus;
        public event Action<ECreatureStatus> OnReleasedStatus;
        void ApplyStatus(ECreatureStatus status, int durationTurnCount);
        void RemoveStatus(ECreatureStatus status);
        void TimePassStatueUpdate();
        void Reset();
    }
    public interface IStatusReadOnly
    {
        bool HasStatus(ECreatureStatus status);
    }

    public interface IPlayerStatModifier
    {
        public void ModifyStat(ApplyStatContext context);
        public void SetCurrentHP(ApplyStatContext context);
        public void SetCurrentMoveCount(ApplyStatContext context);
    }

    public interface ICombatant : IAttackable, IDamageable
    {
        public bool IsActive { get; }
        bool TryGet<T>(out T service) where T : class;

        IStatModifier StatModifier { get; }
        IStatReadOnly StatReadOnly { get; }

        IStatusModifier StatusModifier { get; }
        IStatusReadOnly StatusReadOnly { get; }

        ITileObject Tile { get; }
        IRotateObject Rotate { get; }

        IAbilityExcuter Ability {get;}

        INextAttackEnhancer NextAttackEnhancer { get; }
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