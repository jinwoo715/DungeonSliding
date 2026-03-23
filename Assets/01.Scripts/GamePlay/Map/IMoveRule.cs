using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IMoveRule
    {
        public int MoveCost { get; }
        public bool IsCanMove(EDirectionType directionType);
        public void SetIsMoveable(bool value);
        public void SetMoveBanDirection(EDirectionType directionType);
        public void SetMoveCost(int cost);
        public void AddMoveCost(int cost);
    }
    public class MoveRule : IMoveRule
    {
        public int MoveCost { get; private set; } = 1;

        private EDirectionType _moveBanDirection = EDirectionType.None;
        private bool _isMoveable = true;

        public bool IsCanMove(EDirectionType directionType)
        {
            return _isMoveable == true && directionType != _moveBanDirection;
        }

        public void SetIsMoveable(bool value) => _isMoveable = value;
        public void SetMoveBanDirection(EDirectionType directionType)
        {
            _moveBanDirection = directionType;
        }
        public void SetMoveCost(int cost)
        {
            MoveCost = cost;
        }

        public void AddMoveCost(int cost)
        {
            MoveCost += cost;
        }
    }

    public class EnemyAbilityContext : IAbilityContextService
    {
        private readonly Dictionary<Type, object> _services = new();

        public void Register<T>(T service) where T : class
            => _services[typeof(T)] = service;

        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = (T)obj;
                return true;
            }

            service = null;
            return service != null;
        }
    }

    [System.Serializable]
    public class EnemyAbilityData
    {
        public string UID;
        public string Name;
        public string Description;
        public string AbilityType;
        public EGameEventTrigger GameTriggerType;
        public ECreatureTrigger CretureTriggerType;
        public EGameEventTrigger ReleaseGameTrigger;
        public ECreatureTrigger ReleaseCreatureTrigger;
        public float BaseP1;
        public float GrowthP1;
        public float BaseP2;
        public float GrowthP2;
    }

    public enum EEnemyAbilityType
    {
        HeavyGravity,
        AutoRotate,
        MoveBanToDirection,
        CopyPlayerStat,
        Blind,
        EnhanceAbility,
        CounterAbility,
        Exaltation,
        CommandRotate,
        DefenceFrontAttack,
        KnockBackAttackAbility,
        AutoRotateToPlayer,
    }

    public interface IVisualController
    {
        void EnterBlind();
        void ExitBlind();
    }
    public interface IEnemyAbilityGetter
    {
        ICombatantSensor CombatantSensor { get; }
        IMoveRule MoveRule { get; }
        IStatReadOnly PlayerStatReader { get; }
        IVisualController VisualController { get; }
    }
    public interface IRotateObject
    {
        public event Action OnRotateEnd;
        public EDirectionType Direction { get; }
        public EDirectionType ReverseDirection(EDirectionType directionType);
        public IEnumerator CoRotateToDirection(EDirectionType directionType);
        public void SetRotation(EDirectionType directionType);
    }

 

    public enum ECreatureTrigger
    {
        None,

        OnAdded,

        OnRotate,

        // 공격 시퀀스
        OnRegisterAttack,
        OnAttackPrepared,   // 공격 직전 (버프 주입, 데미지 계산 전)
        OnAttackPerformed,  // 공격 실행 완료 (흡혈, 처치 시 효과) - OnAttacked 대신 더 명확한 표현
        OnBackAttacked,
        OnKilled,

        // 피격 시퀀스
        OnBeforeHitted,     // 데미지 계산 전 (방어 버프, 회피 판정) - OnReceivedAttack 대응
        OnAfterHitted,      // 데미지 계산 및 체력 감소 후 (반격, 피격 시 연출) - OnHitted 대응
        OnHittedBackAttack,

        OnSlided,

        OnSteppedEffectTile,
        OnBlockedByWall,
        OnLevelUp,

        OnDeathByHp,
        OnDeathByMoveCount,

        OnDeath
    }

    
}
