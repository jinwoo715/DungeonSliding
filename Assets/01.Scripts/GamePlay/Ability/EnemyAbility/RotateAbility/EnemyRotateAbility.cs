using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.Map;
using System;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Ability.Enemy
{
    public class AutoRotate : EnemyAbilityBase
    {
        IRouteService _moveable;
        public AutoRotate(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            if (_moveable.LastMoveTileCount == 0)
                yield break;

            Debug.Log("?S????SD??SFD?SDF");

            EDirectionType nextDirection = DirectionUtility.GetRightRotateResultDirection(_owner.Rotate.Direction);
            yield return _owner.Rotate.CoRotateToDirection(nextDirection);

            Debug.Log("Rotate End");
        }
          
        protected override void BindService()
        {
            BindService(ref _moveable);
        }
    }
    public class AutoRotateToPlayer : EnemyAbilityBase
    {
        ICombatantSensor _sensor;

        public AutoRotateToPlayer(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            var playerTile = _sensor.PlayerCombatant;

            EDirectionType dir = DirectionUtility.GetDirFromTileToTile(_owner.Tile.TilePosition, playerTile.Tile.TilePosition);
            yield return _owner.Rotate.CoRotateToDirection(dir);
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
        }
    }
    public class CommandRotate : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        public CommandRotate(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            var enemies = _sensor.AllEnemyCombatants;
            var playerTile = _sensor.PlayerCombatant;

            int remain = 0;
            Action OnEndRotate = () => remain--;

            foreach (var enemy in enemies)
            {
                remain++;

                enemy.Rotate.OnRotateEnd += OnEndRotate;

                EDirectionType dir = DirectionUtility.GetDirFromTileToTile(enemy.Tile.TilePosition, playerTile.Tile.TilePosition);

                enemy.Rotate.RotateToDirection(dir);
            }

            while (remain > 0)
            {
                yield return null;
            }

            foreach (var enemy in enemies)
            {
                enemy.Rotate.OnRotateEnd -= OnEndRotate;
            }
        }

        protected override void BindService()
        {
            BindService(ref _sensor);
        }
    }
}
