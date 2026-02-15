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
        bool TakeDamage(DamageContext damageInfo);
        void OnDeath();
    }
    public interface IAttackable
    {
        Action<ActPair> OnCounterEvent { get; set; }
        event Action OnAttackDoneEvent;
        ICombatant AttackTarget { get;}
        float DamageDealtMultiplier { get; }  // 가하는 피해 배율

        void AddDamageContextStatue(EStatusEffectType effectType, int amount);
        void AddDamageDealtMultiplier(float value);
        bool TrySubmitAttackRequest(ICombatantSensor sensor, IAttackRequestListener attackRequestListener);
        void StartAttackAnimation();
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

    // 3. 상태 및 버프 (선택 사항)
    public interface IStatusEffectable
    {
        public ECreatureStatus StatusFlags { get; }
        bool HasStatus(ECreatureStatus status);
        void ApplyStatus(ECreatureStatus status, int durationTurnCount);
        void RemoveStatus(ECreatureStatus status);
        void ClearStatus();
    }

    public interface IPlayerStatModifier
    {
        public void ModifyStat(PlayerApplyStatContext context);
        public void SetCurrentHP(PlayerApplyStatContext context);
        public void SetCurrentMoveCount(PlayerApplyStatContext context);
    }

    public interface ICombatant : ITilePosition, IAttackable, IDamageable, IStatusEffectable, ICreatureRotator
    {
        public bool IsActive { get; }
        public bool IsCombat { get; }
        public EDirectionType Direction { get; }
        bool TryGet<T>(out T service) where T : class;
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
        public int GetNearEnemyCount(Tile pivot);
    }

}