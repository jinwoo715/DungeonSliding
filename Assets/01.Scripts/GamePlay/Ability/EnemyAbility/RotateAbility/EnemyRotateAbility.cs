using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using System;
using System.Collections;

namespace JW.DungeonSliding.GamePlay.Rotate
{
    public class AutoRotateAbility : EnemyAbilityBase
    {
        public AutoRotateAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

        public override IEnumerator Execute(AbilityArgs args)
        {
            EDirectionType nextDirection = DirectionUtility.GetRightRotateResultDirection(_owner.Rotate.Direction);
            yield return _owner.Rotate.CoRotateToDirection(nextDirection);
        }

        protected override void BindService()
        {
        }
    }

    public class RotateToPlayerAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;

        public RotateToPlayerAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

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

    public class CommandRotateAbility : EnemyAbilityBase
    {
        ICombatantSensor _sensor;
        public CommandRotateAbility(EnemyAbilityData data, IAbilityContextService context, ICombatant owner, int section) : base(data, context, owner, section) { }

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
                enemy.Rotate.CoRotateToDirection(dir);
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
