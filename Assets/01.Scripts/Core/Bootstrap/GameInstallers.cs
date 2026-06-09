using JW.DungeonSliding.Core.Data;
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
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Bootstrap
{
    public static class WorldInstaller
    {
        public static void Install(WorldReferences world, ICombatant player, EnemyAbilityFactory enemyAbilityFactory, IDataService service)
        {
            world.MapManager.Init();
            world.StageController.Init(world.MapManager, world.ObstacleController, player, world.EnemyManager);
            world.ObstacleController.Init(world.MapManager);
            world.EnemyManager.Init(enemyAbilityFactory, service);
        }
    }

    public class PlayerInstaller
    {
        public void Install(PlayerReferences player, IRouteService routeService, IMoveRule moveRule, IAttackRegister requesterRegistry, IAbilityEventService abilityEventService)
        {
            player.InputCoordinator.Init();
            player.Controller.Init(routeService, moveRule, requesterRegistry, abilityEventService);
            player.Level = new LevelSystem();
            player.Level.Initialize();
        }
    }

    public class AbilityInstaller
    {
        public void InstallPlayerAbility(
            PlayerAbilitySystem abilitySystem,
            PlayerAbilityFactory abilityFactory,
            PlayerAbilityContext context,
            PlayerController player,
            FieldCombatantFinder finder,
            RouteBuilder routeBuilder)
        {
            player.RegisterContext(context);
            context.SetOwner(player.Player);
            context.Register<ICombatantSensor>(finder);
            context.Register<IRouteService>(routeBuilder);

            abilityFactory.SetContext(context);
            abilitySystem.Init(abilityFactory, context);
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

            factory.SetContext(context);
        }
    }

    public class UIInstaller
    {
        public void Install(UIReferences ui, IStageService stageService, IStageViewer stageViewer, IAbilityEventService abilityService, IPlayerInfoViewer playerInfoViewer)
        {
            ui.HasAbilityPresenter.Init(ui.AbilityTooltipPresenter, abilityService);
            ui.UIManager.Init();
            GetStagePresenter(ui.StageViewer).Init(stageService, stageViewer, ui.StageViewer);
            ui.AbilityUIController.Init(abilityService);
            ui.PlayerStatPresenter.Init(playerInfoViewer.GetPlayerInfo());
            ui.EnemyStatPresenter.Init();
            ui.EnemyTooltipClicker.Init(ui.EnemyTooltipPresenter);
        }

        private StagePresenter GetStagePresenter(StageViewer stageViewer)
        {
            StagePresenter presenter = stageViewer.GetComponent<StagePresenter>();

            if (presenter == null)
                presenter = stageViewer.gameObject.AddComponent<StagePresenter>();

            return presenter;
        }
    }
}
