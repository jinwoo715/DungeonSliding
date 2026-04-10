using JW.DungeonSliding.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class MainSceneManager : MonoBehaviour
    {
        [SerializeField] private Button _infoButton;
        [SerializeField] private StartButton _startButton;
        [SerializeField] private GameGuidePresenter _gameGuidePresenter;
        [SerializeField] private Fader _fader;

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
            Action callback = () => GameManager.Scene.LoadScene(SceneType.GameScene);
            _fader.FadeOut(callback);
            Debug.Log("Game Start!");
        }
    }
}
