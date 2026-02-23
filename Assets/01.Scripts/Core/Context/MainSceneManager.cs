using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class MainSceneManager : MonoBehaviour
    {
        [SerializeField] private Button _infoButton;
        [SerializeField] private StartButton _startButton;
        [SerializeField] private GameGuidePresenter _gameGuidePresenter;

        private void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            _startButton.Initialize(StartGame);
            _gameGuidePresenter.Initialize();
            _infoButton.onClick.AddListener(() => _gameGuidePresenter.ShowGameGuide());
        }

        public void StartGame()
        {
            Debug.Log("Game Start!");
        }
    }
}
