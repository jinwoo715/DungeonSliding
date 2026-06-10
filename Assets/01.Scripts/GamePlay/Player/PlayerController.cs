using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System;
using System.Collections;
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

        private LevelSystem _levelSystem;

        public event Action OnPlayerDie;

        private bool _isCanMove;

        public ICombatant Player => _player;
        public IStatReadOnly StatReadOnly => _player.StatReadOnly;
        public IStatModifier StatModifier => _player.StatModifier;
        public IMoveable Moveable => _player;
        public INextAttackEnhancer NextAttackEnhancer => _player.NextAttackEnhancer;
        public IMoveRule _moveRule;

        private IMoveContextProvider _moveContextProvider;
        private ITurnPassRequester _turnPassRequester;
        private Coroutine _passTurnRoutine;

        public void Init(
            IRouteService routeService,
            IMoveRule moveRule,
            IAttackRegister requesterRegistry,
            IAbilityEventService abilityEventService,
            IMoveContextProvider moveContextProvider,
            ITurnPassRequester turnPassRequester)
        {
            _levelSystem = new LevelSystem();
            _levelSystem.Initialize();

            _player.Initialize(ECreatureType.Player);

            _moveRule = moveRule;
            _moveContextProvider = moveContextProvider;
            _turnPassRequester = turnPassRequester;

            _player.InitData(CreatePlayerBaseStat());

            _player.Wire(routeService);
            _player.RegisterRequester(requesterRegistry);

            abilityEventService.OnSelectAbility += _player.AbilityRegister.RegisterAbility;
            
            _player.OnGetXp += _levelSystem.AddXp;
            
            _levelSystem.OnLevelUp += abilityEventService.GrantAbilityPoint;
            _levelSystem.OnLevelUp += ApplyLevelUpGrowth;
            _levelSystem.OnLevelUp += _player.HandleLevelUp;

            _player.OnMoveEnd += PayMoveCost;

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnStart, CheckValidTurn);

            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameEventTrigger.OnTurnEnd, CheckPlayerAlive);
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

        private void ApplyLevelUpGrowth(int level)
        {
            PlayerConfig playerConfig = GameManager.Config.Player;

            ApplyLevelUpStat(
                ECreatureStatType.MaxHp,
                playerConfig.LevelUpHP,
                playerConfig.LevelUpHPRatio);

            ApplyLevelUpStat(
                ECreatureStatType.Damage,
                playerConfig.LevelUpDamage,
                playerConfig.LevelUpDamageRatio);

            if (IsMoveGrowthLevel(level, playerConfig.LevelUpMoveInterval))
            {
                ApplyLevelUpStat(
                    ECreatureStatType.MaxMoveCount,
                    playerConfig.LevelUpMove,
                    0);
            }
        }

        private void ApplyLevelUpStat(ECreatureStatType statType, int flatGrowth, int ratioGrowth)
        {
            if (flatGrowth > 0)
            {
                StatModifier.ModifyStat(new StatModifierContext(statType, EApplyStatType.Add, flatGrowth));
            }

            if (ratioGrowth > 0)
            {
                float ratio = ratioGrowth * 0.01f;
                StatModifier.ModifyStat(new StatModifierContext(statType, EApplyStatType.Multiple, ratio));
            }
        }

        private void CheckValidTurn()
        {
            if (_passTurnRoutine != null || !IsBlockedByMoveBan())
                return;

            _passTurnRoutine = StartCoroutine(CoRequestPassTurn());
        }

        private bool IsBlockedByMoveBan()
        {
            EDirectionType banDirection = _moveRule.BanDirection;
            if (banDirection == EDirectionType.None)
                return false;

            Tile playerTile = _player.TileObject.TilePosition;

            for (int i = 0; i < 4; i++)
            {
                EDirectionType direction = (EDirectionType)i;
                ESlideResultType result = _moveContextProvider
                    .GetMoveContext(playerTile, direction, ETileEnterType.None)
                    .ResultType;

                if (direction == banDirection)
                {
                    if (result != ESlideResultType.Move)
                        return false;
                }
                else if (result != ESlideResultType.Stop)
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerator CoRequestPassTurn()
        {
            yield return null;

            _passTurnRoutine = null;
            _turnPassRequester.RequestPassTurn();
        }
        private bool IsMoveGrowthLevel(int level, int interval)
        {
            if (interval <= 0) return true;

            return level % interval == 0;
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
            if(_player.SlideResultType == ESlideResultType.Move)
                StatModifier.ModifyStat(new StatModifierContext(ECreatureStatType.CurrentMoveCount, EApplyStatType.Add, -_moveRule.MoveCost));
        }

        private void CheckPlayerAlive()
        {
            if (!_player.IsActive)
                OnPlayerDie?.Invoke();
        }

        public PlayerInfo GetPlayerInfo()
        {
            return new PlayerInfo(StatReadOnly, StatModifier, _levelSystem, NextAttackEnhancer);
        }
    }
}
