using JW.DungeonSliding.Core;
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

        public void InitializePlayer(IRouteService routeService, IMoveRule moveRule, IRequesterRegistry requesterRegistry, ILevelProgress levelProgress)
        {
            _player.Initialize(ECreatureType.Player);

            CreatureBaseStat baseStat = CreatePlayerBaseStat();
            _player.InitData(baseStat);

            _player.Wire(routeService, moveRule, levelProgress);

            _player.RegisterRequester(requesterRegistry);
        }
        private CreatureBaseStat CreatePlayerBaseStat()
        {
            PlayerConfig playerConfig = GameManager.Config.Player;
            return new CreatureBaseStat(playerConfig.HP, playerConfig.DMG, playerConfig.MVCount);
        }

 
    }
}
