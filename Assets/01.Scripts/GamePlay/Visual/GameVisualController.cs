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
            Debug.Log("Bliend");

            RenderSettings.reflectionIntensity = 0;
            RenderSettings.ambientIntensity = 0;
            _directionLight.SetActive(false);
            _playerSpotLight.SetActive(true);
            _enemyStatUIService.HideAll();

            Debug.Log(RenderSettings.reflectionIntensity);
        }

        public void ExitBlind()
        {
            Debug.Log("Shy");

            RenderSettings.reflectionIntensity = 1;
            RenderSettings.ambientIntensity = 1;
            _directionLight.SetActive(true);
            _playerSpotLight.SetActive(false);
            _enemyStatUIService.ShowAll();
        }
    }
}
