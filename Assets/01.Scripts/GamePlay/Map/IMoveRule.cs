using JW.DungeonSliding.GamePlay;
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
    public class BossAbilityManager : IEnemyAbilityGetter
    {
        public ICombatantSensor CombatantSensor { get; private set; }
        public IMoveRule MoveRule { get; private set; }
        public IStatReadOnly PlayerStatReader { get; private set; }
        public IVisualController VisualController { get; private set; }

        public BossAbilityManager(ICombatantSensor sensor, IMoveRule rule, IStatReadOnly reader, IVisualController visualController)
        {
            CombatantSensor = sensor;
            MoveRule = rule;
            PlayerStatReader = reader;
            VisualController = visualController;
        }
    }

    

    public interface IBossAbility
    {
        public void SetAbilityGetter(IEnemyAbilityGetter bossAbilityGetter);
    }

    [System.Serializable]
    public class EnemyAbilityData
    {
        public string UID;
        public string Name;
        public string Description;
        public EEnemyAbilityType EnemyAbilityType;
        public EGameTriggerType GameTriggerType;
        public ECreatureTrigger CretureTriggerType;
        public bool IsReleaseOnDeath;
        public float BaseP1;
        public float GrowthP1Ratio;
        public float BaseP2;
        public float GrowthP2Ratio;
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
    //Environment       //change
    //Direction light   //off
    //player spot light //on
    //enemy ui          //off
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
    public interface IAbility
    {
        public EGameTriggerType GameTrigger { get; }
        public ECreatureTrigger CreatureTrigger { get; }
        public IEnumerator Excute();
        public void ReleaseAbility();
    }

    public interface IAbilityPayloadReceiver<T>
    {
        void ReceivePayload(T payload);
    }

    public enum ECreatureTrigger
    {
        None,
        OnRotate,

        // 공격 시퀀스
        OnAttackPrepared,   // 공격 직전 (버프 주입, 데미지 계산 전)
        OnAttackPerformed,  // 공격 실행 완료 (흡혈, 처치 시 효과) - OnAttacked 대신 더 명확한 표현

        // 피격 시퀀스
        OnBeforeHitted,     // 데미지 계산 전 (방어 버프, 회피 판정) - OnReceivedAttack 대응
        OnAfterHitted,      // 데미지 계산 및 체력 감소 후 (반격, 피격 시 연출) - OnHitted 대응

        OnDeathByHp,
        OnDeathByMoveCount,

        OnDeath
    }

    public abstract class EnemyAbilityBase : IAbility
    {
        public EnemyAbilityData _data;
        protected ICombatant _owner;

        public EGameTriggerType GameTrigger => _data.GameTriggerType;
        public ECreatureTrigger CreatureTrigger => _data.CretureTriggerType;

        public EnemyAbilityBase(EnemyAbilityData data, IEnemyAbilityGetter bossAbilityGetter, ICombatant boss, int section)
        {
            _owner = boss;
            _data = data;
            Bind(bossAbilityGetter);
            CalculateParam(section);
        }
        public abstract IEnumerator Excute();
        public abstract void Bind(IEnemyAbilityGetter bossAbilityGetter);
        private void CalculateParam(int section)
        {
            _data.BaseP1 = _data.BaseP1 + _data.GrowthP1Ratio * section;
            _data.BaseP2 = _data.BaseP2 + _data.GrowthP2Ratio * section;
        }
        public virtual void ReleaseAbility() { }
    }

    public class AutoRotateAbility : EnemyAbilityBase
    {
        IRotateObject _creatureRotator;
        IMoveRule _moveRule;

        public AutoRotateAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if (_owner.TryGet<IRotateObject>(out var service))
                _creatureRotator = service;

            _moveRule = bossAbilityGetter.MoveRule;
        }

        public override IEnumerator Excute()
        {
            //if (!_owner.IsCombat)
            {
                _moveRule.SetIsMoveable(false);
                EDirectionType nextDirection = (EDirectionType)(((int)_owner.Rotate.Direction + 1) % 4);
                yield return _creatureRotator.CoRotateToDirection(nextDirection);
                _moveRule.SetIsMoveable(true);
            }
        }
    }
    public class FacingMoveBanAbility : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        
        public FacingMoveBanAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override IEnumerator Excute()
        {
            _moveRule.SetMoveBanDirection(_owner.Rotate.Direction);
            yield return null;
        }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            _moveRule = bossAbilityGetter.MoveRule;
        }
    }
    public class HeavyGravityAbility : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        private int _addMoveCost = 0;
        public HeavyGravityAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }
        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            _moveRule = bossAbilityGetter.MoveRule;
        }
        public override IEnumerator Excute()
        {
            yield return null;

            _addMoveCost += (int)_data.BaseP1;

            _moveRule.AddMoveCost(_addMoveCost);
        }
        public override void ReleaseAbility()
        {
            _moveRule.AddMoveCost(-_addMoveCost);
        }
    }
    public class CopyAbility : EnemyAbilityBase
    {
        IStatReadOnly _playerStatReader;
        IEnemyStatModifier _enemyStatModifier;
        public CopyAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            _playerStatReader = bossAbilityGetter.PlayerStatReader;
            if (_owner.TryGet<IEnemyStatModifier>(out var enemyStatModifier))
            {
                _enemyStatModifier = enemyStatModifier;
            }
        }

        public override IEnumerator Excute()
        {
            //_enemyStatModifier.SetEnemyStat(EEnemyStatType.HP, _playerStatReader.Get(EPlayerStatType.CurrentHP));
            //_enemyStatModifier.SetEnemyStat(EEnemyStatType.Damage, _playerStatReader.Get(EPlayerStatType.Damage));

            yield return null;
        }
    }
    public class BlindAbility : EnemyAbilityBase
    {
        IVisualController _visualController;
        private bool isBlined = false;
        private int blindTurn = 0;

        public BlindAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter getter)
        {
            _visualController = getter.VisualController;
        }

        public override IEnumerator Excute()
        {
            Debug.Log("Excute");
            if(isBlined == true)
            {
                isBlined = false;
                _visualController.ExitBlind();
            }
            else
            {
                blindTurn++;
                if(blindTurn >= _data.BaseP1)
                {
                    blindTurn = 0;
                    isBlined = true;
                    _visualController.EnterBlind();
                }
            }
            yield return null;
        }
    }
    public class EnemyEnhanceAbility : EnemyAbilityBase
    {
        IEnemyStatModifier _statModifier;

        public EnemyEnhanceAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if(_owner.TryGet<IEnemyStatModifier>(out var service))
            {
                _statModifier = service;
            }
        }

        public override IEnumerator Excute()
        {
            _statModifier.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, _data.BaseP1, EEnemyStatType.None));
            _statModifier.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.Damage, EApplyStatType.Add, _data.BaseP2, EEnemyStatType.None));
            yield return null;
        }
    }
    public class ExaltationAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        public ExaltationAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }
        public override void Bind(IEnemyAbilityGetter getter)
        {
            _sensor = getter.CombatantSensor;
        }

        public override IEnumerator Excute()
        {
            var enemies = _sensor.AllEnemyCombatants;

            foreach (var enemy in enemies)
            {
                if (enemy.TryGet<IEnemyStatModifier>(out var service))
                {
                    service.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, _data.BaseP1, EEnemyStatType.None));
                    service.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.Damage, EApplyStatType.Add, _data.BaseP2, EEnemyStatType.None));
                }
            }

            yield return null;
        }

        public override void ReleaseAbility()
        {
            var enemies = _sensor.AllEnemyCombatants;

            foreach (var enemy in enemies)
            {
                if (enemy.TryGet<IEnemyStatModifier>(out var service))
                {
                    int setHp = Mathf.Max(1, service.Get(EEnemyStatType.HP) - (int)_data.BaseP1);
                    int setDamage = Mathf.Max(1, service.Get(EEnemyStatType.Damage) - (int)_data.BaseP2);

                    service.SetEnemyStat(EEnemyStatType.HP, setHp);
                    service.SetEnemyStat(EEnemyStatType.Damage, setDamage);
                }
            }
        }
    }
    public class CommandRotateAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        IMoveRule _moveRule;
        public CommandRotateAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter getter)
        {
            _sensor = getter.CombatantSensor;
            _moveRule = getter.MoveRule;
        }

        public override IEnumerator Excute()
        {
            _moveRule.SetIsMoveable(false);
            var enemies = _sensor.AllEnemyCombatants;
            var playerTile = _sensor.PlayerCombatant;

            int remain = 0;

            foreach (var enemy in enemies)
            {
                remain++;

                EDirectionType dir = GridUtility.GetDirFromTileToTile(enemy.Tile.TilePosition, playerTile.Tile.TilePosition);
                enemy.Rotate.CoRotateToDirection(dir);
            }

            while (remain > 0)
            {
                yield return null;
            }

            _moveRule.SetIsMoveable(true);
        }
    }
    public class CounterAbility : EnemyAbilityBase
    {
        IDamageable _damageable;
        ICounterAttackable _counterAttackable;

        public CounterAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }
        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if(_owner.TryGet<IDamageable>(out var damageable))
            {
                _damageable = damageable;
            }

            if (_owner.TryGet<ICounterAttackable>(out var counter))
            {
                _counterAttackable = counter;
            }
        }

        public override IEnumerator Excute()
        {
            if (_counterAttackable != null)
            {
                int chanceValue = UnityEngine.Random.Range(0, 101);

                if (chanceValue <= _data.BaseP1)
                {
                    //_counterAttackable.RequestCounterAttack(_damageable.LastAttacker);
                }
            }

            yield return null;
        }
    }
    public class DefenceFrontAttackAbility : EnemyAbilityBase
    {
        private IBarrierable _barrierable;
        public DefenceFrontAttackAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }
        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if(_owner.TryGet<IBarrierable>(out var barrierable))
            {
                _barrierable = barrierable;
            }
        }

        public override IEnumerator Excute()
        {
            _barrierable.GainBarrier();
            yield return null;
        }
    }
    public class KnockBackAttackAbility : EnemyAbilityBase
    {
        IAttackable _attackable;
        public KnockBackAttackAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }
        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if(_owner.TryGet<IAttackable>(out var service))
            {
                _attackable = service;
            }
        }

        public override IEnumerator Excute()
        {
            //_attackable.AddDamageContextStatue(EStatusEffectType.KnockBack, (int)_data.BaseP1);
            yield return null;
        }
    }
    public class RotateToPlayerAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        IMoveRule _moveRule;
        public RotateToPlayerAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            _sensor = bossAbilityGetter.CombatantSensor;
            _moveRule = bossAbilityGetter.MoveRule;
        }

        public override IEnumerator Excute()
        {
            _moveRule.SetIsMoveable(false);

            var playerTile = _sensor.PlayerCombatant;

            EDirectionType dir = GridUtility.GetDirFromTileToTile(_owner.Tile.TilePosition, playerTile.Tile.TilePosition);
            yield return _owner.Rotate.CoRotateToDirection(dir);

            _moveRule.SetIsMoveable(true);
        }
    }

}
