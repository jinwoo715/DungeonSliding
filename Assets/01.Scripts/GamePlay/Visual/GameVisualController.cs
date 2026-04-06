using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class GameVisualController : MonoBehaviour, IVisualController
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private GameObject _directionLight;
        [SerializeField] private GameObject _playerSpotLight;

        private IEnemyStatUIService _enemyStatUIService;

        public void Init(IEnemyStatUIService enemyStatUIService)
        {
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
