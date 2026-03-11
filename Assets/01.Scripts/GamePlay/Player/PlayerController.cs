using JW.DungeonSliding.Core;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.Map;
using System;
using UnityEngine;
using static JW.DungeonSliding.GamePlay.GameConfig;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Player _player;

        public ICombatant Player => _player;
        public IStatReadOnly StatReadOnly => _player.StatReadOnly;
        public IStatModifier StatModifier => _player.StatModifier;
        public ITileObject Tile => _player.Tile;
        public IMoveable Moveable => _player;
        public IAbilityRegister AbilityRegister => _player.AbilityRegister;
        public INextAttackEnhancer NextAttackEnhancer => _player.NextAttackEnhancer;

        public void InitializePlayer(IRouteService routeService, IMoveRule moveRule, IRequesterRegistry requesterRegistry, ILevelProgress levelProgress, IAbilityEventService abilityEventService)
        {
            _player.Initialize(ECreatureType.Player);

            CreatureBaseStat baseStat = CreatePlayerBaseStat();
            _player.InitData(baseStat);

            _player.Wire(routeService, moveRule);

            _player.RegisterRequester(requesterRegistry);

            abilityEventService.OnSelectAbility += _player.AbilityRegister.RegisterAbility;

            _player.OnGetXp += levelProgress.AddXp;
            levelProgress.OnLevelUp += _player.HandleLevelUp;
        }
        private CreatureBaseStat CreatePlayerBaseStat()
        {
            PlayerConfig playerConfig = GameManager.Config.Player;
            return new CreatureBaseStat(playerConfig.HP, playerConfig.DMG, playerConfig.MVCount);
        }

 
    }
}
