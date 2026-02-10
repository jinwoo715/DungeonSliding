using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class GameVisualController : IVisualController
    {
        private Camera _camera;
        
        private GameObject _directionLight;
        
        private GameObject _playerSpotLight;
        
        private IEnemyStatUIService _enemyStatUIService;

        public GameVisualController(Camera cam, GameObject dirLight, GameObject playerLight, IEnemyStatUIService enemyStatUIService)
        {
            _camera = cam;
            _directionLight = dirLight;
            _playerSpotLight = playerLight;
            _enemyStatUIService = enemyStatUIService;
        }

        public void EnterBlind()
        {
            RenderSettings.reflectionIntensity = 0;
            RenderSettings.ambientIntensity = 0;
            _directionLight.SetActive(false);
            _playerSpotLight.SetActive(false);
            _enemyStatUIService.HideAll();
        }

        public void ExitBlind()
        {
            RenderSettings.reflectionIntensity = 1;
            RenderSettings.ambientIntensity = 1;
            _directionLight.SetActive(true);
            _playerSpotLight.SetActive(true);
            _enemyStatUIService.ShowAll();
        }
    }
}
