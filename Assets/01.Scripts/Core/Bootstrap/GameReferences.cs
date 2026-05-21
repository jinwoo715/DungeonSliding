using JW.DungeonSliding.Core.Inputs;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stage;
using JW.DungeonSliding.Map;
using JW.DungeonSliding.UI;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Bootstrap
{
    [System.Serializable]
    public class WorldReferences
    {
        public MapManager MapManager;
        public StageController StageController;
        public EnemyManager EnemyManager;
        public ObstacleObjectController ObstacleController;
    }

    [System.Serializable]
    public class PlayerReferences
    {
        public PlayerController Controller;
        public InputCoordinator InputCoordinator;
        public LevelSystem Level;
    }

    [System.Serializable]
    public class UIReferences
    {
        public GameSceneUIManager UIManager;

        [Header("Controller")]
        public AbilitySelectPresenter AbilityUIController;
        public EnemyTooltipController EnemyTooltipClicker;

        [Header("Presenter")]
        public HasAbilityItemPresenter HasAbilityPresenter;
        public EnemyStatPresenter EnemyStatPresenter;
        public PlayerStatPresenter PlayerStatPresenter;
        public GameTooltipPresenter AbilityTooltipPresenter;
        public GameTooltipPresenter EnemyTooltipPresenter;

        [Header("Viewer")]
        public StageViewer StageViewer;
    }
}
