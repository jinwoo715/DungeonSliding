using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GameGuidePresenter : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private GameGuideCategoryViewer _categoryViewer;
        [SerializeField] private GameGuideContentViewer _contentViewer;
        [SerializeField] private Button _closeButton;

        [Header("Data")]
        [SerializeField] private List<GameGuideDataBundle> _gameGuideDataBundle;

        public void Initialize()
        {
            _categoryViewer.Initialize(OnClickCategory);
            _closeButton.onClick.AddListener(CloseGameGuide);
        }

        public void ShowGameGuide()
        {
            _panel.SetActive(true);
        }

        private void CloseGameGuide()
        {
            _categoryViewer.Init();
            _panel.SetActive(false);
        }

        private void OnClickCategory(int categoryNum)
        {
            _contentViewer.SetData(_gameGuideDataBundle[categoryNum]);
        }
    }
}
