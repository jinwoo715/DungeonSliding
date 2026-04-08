using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System;
using UnityEngine;
using static JW.DungeonSliding.GamePlay.GameConfig;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public struct PlayerInfo
    {
        public readonly IStatReadOnly PlayerStatReader;
        public readonly IStatModifier PlayerStatModifier;
        public readonly ILevelProgress Level;
        public readonly INextAttackEnhancer NextAttackEnhancer;

        public PlayerInfo(IStatReadOnly playerStatReader, IStatModifier playerStatModifier, ILevelProgress level, INextAttackEnhancer nextAttackEnhancer)
        {
            PlayerStatReader = playerStatReader;
            PlayerStatModifier = playerStatModifier;
            Level = level;
            NextAttackEnhancer = nextAttackEnhancer;
        }
    }

    public interface IPlayerInfoViewer
    {
        PlayerInfo GetPlayerInfo();
    }

    public class PlayerController : MonoBehaviour, IPlayerInfoViewer
    {
        [SerializeField] private Player _player;

        private LevelSystem _levelSystem = new LevelSystem();

        public event Action OnPlayerDie;

        private bool _isCanMove;

        public ILevelProgress Level => _levelSystem;
        public ICombatant Player => _player;
        public IStatReadOnly StatReadOnly => _player.StatReadOnly;
        public IStatModifier StatModifier => _player.StatModifier;
        public IMoveable Moveable => _player;
        public INextAttackEnhancer NextAttackEnhancer => _player.NextAttackEnhancer;
        public IMoveRule _moveRule;

        public void Init(IRouteService routeService, IMoveRule moveRule, IAttackRegister requesterRegistry, IAbilityEventService abilityEventService)
        {
            _player.Initialize(ECreatureType.Player);

            CreatureBaseStat baseStat = CreatePlayerBaseStat();
            _player.InitData(baseStat);

            _player.Wire(routeService);

            _player.RegisterRequester(requesterRegistry);

            abilityEventService.OnSelectAbility += _player.AbilityRegister.RegisterAbility;

            _player.OnGetXp += _levelSystem.AddXp;
            _levelSystem.OnLevelUp += _player.HandleLevelUp;

            _player.OnMoveEnd += PayMoveCost;
        }
        public void RegisterContext(IAbilityContextService abilityContextService)
        {
            abilityContextService.Register<IMoveable>(Moveable);
            abilityContextService.Register<IStatModifier>(StatModifier);
            abilityContextService.Register<IStatReadOnly>(StatReadOnly);
            abilityContextService.Register<IStatusModifier>(_player.StatusModifier);
            abilityContextService.Register<INextAttackEnhancer>(NextAttackEnhancer);
            abilityContextService.Register<IRotateObject>(_player.Rotate);
            abilityContextService.Register<ITileObject>(_player.TileObject);
            abilityContextService.Register<IAttackRequester>(_player.AttackRequester);
        }
        private CreatureBaseStat CreatePlayerBaseStat()
        {
            PlayerConfig playerConfig = GameManager.Config.Player;
            return new CreatureBaseStat(playerConfig.HP, playerConfig.DMG, playerConfig.MVCount);
        }
        
        public void OnPlayerMove(EDirectionType directionType)
        {
            if (_isCanMove && _moveRule.IsCanMove(directionType))
                Moveable.SlideRoute(directionType);
        }

        public void OnChangeMoveState(bool isMoveable)
        {
            _isCanMove = isMoveable;
        }

        private void PayMoveCost()
        {
            StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, -_moveRule.MoveCost));
        }

        private void CheckPlayerAlive()
        {
            if (!_player.IsActive)
                OnPlayerDie?.Invoke();
        }

        public PlayerInfo GetPlayerInfo()
        {
            return new PlayerInfo(StatReadOnly, StatModifier, Level, NextAttackEnhancer);
        }
    }
}
