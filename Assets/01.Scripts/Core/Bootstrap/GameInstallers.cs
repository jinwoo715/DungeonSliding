using JW.DungeonSliding.Core.Flow;
using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Ability;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Context;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Move;
using JW.DungeonSliding.GamePlay.Stage;
using JW.DungeonSliding.GamePlay.Stats;
using JW.DungeonSliding.UI;

namespace JW.DungeonSliding.GamePlay.Bootstrap
{
    public static class WorldInstaller
    {
        public static void Install(WorldReferences world, ICombatant player, EnemyAbilityFactory enemyAbilityFactory)
        {
            world.MapManager.Init();
            world.StageController.Init(world.MapManager, world.ObstacleController, player.TileObject, world.EnemyManager);
            world.ObstacleController.Init(world.MapManager);
            world.EnemyManager.Init(enemyAbilityFactory);
        }
    }

    public class PlayerInstaller
    {
        public void Install(PlayerReferences player, IRouteService routeService, IMoveRule moveRule, IAttackRegister requesterRegistry, IAbilityEventService abilityEventService)
        {
            player.InputCoordinator.Init();
            player.Controller.Init(routeService, moveRule, requesterRegistry, abilityEventService);
        }
    }

    public class AbilityInstaller
    {
        public void InstallPlayerAbility(
            PlayerAbilitySystem abilitySystem,
            PlayerAbilityContext context,
            PlayerController player,
            FieldCombatantFinder finder,
            RouteBuilder routeBuilder)
        {
            player.RegisterContext(context);
            context.SetOwner(player.Player);
            context.Register<ICombatantSensor>(finder);
            context.Register<IRouteService>(routeBuilder);

            abilitySystem.Init(context, player.Level);
        }

        public void InstallEnemyAbility(
            EnemyAbilityFactory factory,
            EnemyAbilityContext context,
            FieldCombatantFinder finder,
            MoveRule moveRule,
            PlayerController player,
            GameVisualController visualController,
            RouteBuilder routeBuilder)
        {
            context.Register<ICombatantSensor>(finder);
            context.Register<IMoveRule>(moveRule);
            context.Register<IStatReadOnly>(player.StatReadOnly);
            context.Register<IVisualController>(visualController);
            context.Register<IRouteService>(routeBuilder);

            factory.Init(context);
        }
    }

    public class UIInstaller
    {
        public void Install(UIReferences ui, IStageViewer stageViewer, IAbilityEventService abilityService, IPlayerInfoViewer playerInfoViewer)
        {
            ui.HasAbilityPresenter.Init(ui.AbilityTooltipPresenter, abilityService);
            ui.UIManager.Init();
            ui.StageViewer.Init(stageViewer.TotalFloor, stageViewer.BossFloors);
            ui.AbilityUIController.Init(abilityService);
            ui.PlayerStatPresenter.Init(playerInfoViewer.GetPlayerInfo());
            ui.EnemyTooltipClicker.Init(ui.EnemyTooltipPresenter);
        }
    }
}
