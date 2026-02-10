using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
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
        public IPlayerStatReader PlayerStatReader { get; private set; }
        public IVisualController VisualController { get; private set; }

        public BossAbilityManager(ICombatantSensor sensor, IMoveRule rule, IPlayerStatReader reader, IVisualController visualController)
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
        public EEnemyAbilityType AbilityType;
        public EGameTriggerType GameTriggerType;
        public ECretureTrigger CretureTriggerType;
        public bool IsReleaseOnDeath;
        public float P1;
        public float GrowthP1Ratio;
        public float P2;
        public float GrowthP2Ratio;
    }

    public class EnemyAbilityFactory
    {
        public IEnemyAbilityGetter getter;
        public List<IEnemyAbility> GetAbility(string enemyAbilities, ICombatant host, int section)
        {
            string[] abilities = enemyAbilities.Split('|');
            List<IEnemyAbility> abilityList = new List<IEnemyAbility>();

            for (int i = 0; i < abilities.Length; i++)
            {
                EEnemyAbilityType type = (EEnemyAbilityType)Enum.Parse(typeof(EEnemyAbilityType), abilities[i]);
                EnemyAbilityData data = new EnemyAbilityData();

                abilityList.Add(CreateAbility(type, data, host, section));
            }

            return abilityList;
        }
        private IEnemyAbility CreateAbility(EEnemyAbilityType abilityType, EnemyAbilityData data, ICombatant host, int section)
        {
            switch (abilityType)
            {
                case EEnemyAbilityType.HeavyGravity:          return new HeavyGravityAbility(data, getter, host, section);   
                case EEnemyAbilityType.AutoRotate:            return new AutoRotateAbility(data, getter, host, section);
                case EEnemyAbilityType.MoveBanToDirection:    return new FacingMoveBanAbility(data, getter, host, section);
                case EEnemyAbilityType.CopyPlayerStat:        return new CopyAbility(data, getter, host, section);
                case EEnemyAbilityType.Blind:                 return new BlindAbility(data, getter, host, section);
                case EEnemyAbilityType.EnhanceAbility:        return new EnemyEnhanceAbility(data, getter, host, section);
                case EEnemyAbilityType.Exaltation:            return new ExaltationAbility(data, getter, host, section);
                case EEnemyAbilityType.CommandRotate:         return new CommandRotateAbility(data, getter, host, section);
                default: return null;
            }
        }
    }


    public enum EEnemyAbilityType
    {
        HeavyGravity,
        AutoRotate,
        MoveBanToDirection,
        CopyPlayerStat,
        Blind,
        EnhanceAbility,
        GrowthOnBattleEnd,
        Exaltation,
        CommandRotate
    }

    public enum ECretureTrigger
    {
        None,
        OnRotate,
        OnAttack,
        OnHitted,
        OnDeath
    }
    public abstract class EnemyAbilityBase : IEnemyAbility
    {
        public EnemyAbilityData _data;
        protected ICombatant _owner;
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
            _data.P1 = _data.P1 + _data.GrowthP1Ratio * section;
            _data.P2 = _data.P2 + _data.GrowthP2Ratio * section;
        }
        public virtual void ReleaseAbility() { }
    }

    public class AutoRotateAbility : EnemyAbilityBase
    {
        ICreatureRotator _creatureRotator;
        IMoveRule _moveRule;

        public AutoRotateAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter bossAbilityGetter)
        {
            if (_owner.TryGet<ICreatureRotator>(out var service))
                _creatureRotator = service;

            _moveRule = bossAbilityGetter.MoveRule;
        }

        public override IEnumerator Excute()
        {
            _moveRule.SetIsMoveable(false);
            EDirectionType nextDirection = (EDirectionType)(((int)_owner.Direction + 1) % 4);
            yield return _creatureRotator.CoRotateCharacter(nextDirection);
            _moveRule.SetIsMoveable(true);
        }
    }
    public class FacingMoveBanAbility : EnemyAbilityBase
    {
        IMoveRule _moveRule;
        
        public FacingMoveBanAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override IEnumerator Excute()
        {
            _moveRule.SetMoveBanDirection(_owner.Direction);
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

            _addMoveCost += (int)_data.P1;

            _moveRule.AddMoveCost(_addMoveCost);
        }
        public override void ReleaseAbility()
        {
            _moveRule.AddMoveCost(-_addMoveCost);
        }
    }
    public class CopyAbility : EnemyAbilityBase
    {
        IPlayerStatReader _playerStatReader;
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
            _enemyStatModifier.SetEnemyStat(EEnemyStatType.HP, _playerStatReader.Get(EPlayerStatType.CurrentHP));
            _enemyStatModifier.SetEnemyStat(EEnemyStatType.Damage, _playerStatReader.Get(EPlayerStatType.Damage));

            yield return null;
        }
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

    public class BlindAbility : EnemyAbilityBase
    {
        IVisualController _visualController;
        private bool isEnabled = true;

        public BlindAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter getter)
        {
            _visualController = getter.VisualController;
        }

        public override IEnumerator Excute()
        {
            isEnabled = !isEnabled;

            if (isEnabled) _visualController.EnterBlind();
            else _visualController.ExitBlind();

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
            _statModifier.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, _data.P1, EEnemyStatType.None));
            _statModifier.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.Damage, EApplyStatType.Add, _data.P2, EEnemyStatType.None));
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
                    service.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.HP, EApplyStatType.Add, _data.P1, EEnemyStatType.None));
                    service.ModifyEnemyStat(new EnemyApplyStatContext(EEnemyStatType.Damage, EApplyStatType.Add, _data.P2, EEnemyStatType.None));
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
                    int setHp = Mathf.Max(1, service.Get(EEnemyStatType.HP) - (int)_data.P1);
                    int setDamage = Mathf.Max(1, service.Get(EEnemyStatType.Damage) - (int)_data.P2);

                    service.SetEnemyStat(EEnemyStatType.HP, setHp);
                    service.SetEnemyStat(EEnemyStatType.Damage, setDamage);
                }
            }
        }
    }
    public class CommandRotateAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;

        public CommandRotateAbility(EnemyAbilityData data, IEnemyAbilityGetter getter, ICombatant owner, int section) : base(data, getter, owner, section) { }

        public override void Bind(IEnemyAbilityGetter getter)
        {
            _sensor = getter.CombatantSensor;
        }

        public override IEnumerator Excute()
        {
            var enemies = _sensor.AllEnemyCombatants;
            var playerTile = _sensor.PlayerCombatant;

            foreach (var enemy in enemies)
            {
                enemy.CoRotateToTarget(playerTile);
            }

            yield return null;
        }
    }


    public interface IEnemyAbilityGetter
    {
        ICombatantSensor CombatantSensor { get; }
        IMoveRule MoveRule { get; }
        IPlayerStatReader PlayerStatReader { get; }
        IVisualController VisualController { get; }
    }
    public interface ICreatureRotator
    {
        public IEnumerator CoRotateCharacter(EDirectionType directionType);
        public void SetRotation(EDirectionType directionType);
        public IEnumerator CoRotateToTarget(ITilePosition combatant);
    }
    public interface IEnemyAbility
    {
        public IEnumerator Excute();
        public void ReleaseAbility();
    }
}
