using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GameGuideContentViewer : MonoBehaviour
    {
        [SerializeField] private GameGuidCardItem[] _cards;

        [SerializeField] private Button _prevButton;
        [SerializeField] private Button _nextButton;

        private GameGuideDataBundle _currentDataBundle;

        public void SetData(GameGuideDataBundle bundle)
        {
            _currentDataBundle = bundle;

            for (int i = 0; i < _cards.Length; i++)
            {
                _cards[i].gameObject.SetActive(false);
            }

            int maxCount = Mathf.Min(_currentDataBundle.Datas.Count, 3);

            for (int i = 0; i < maxCount; i++)
            {
                GameGuideData data = _currentDataBundle.Datas[i];

                _cards[i].gameObject.SetActive(true);
                _cards[i].SetData(data._sprite, data._description);
            }

            if(_currentDataBundle.Datas.Count < 3)
            {
                _prevButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(false);
            }
            else
            {
                _prevButton.gameObject.SetActive(false);
                _nextButton.gameObject.SetActive(true);
            }
        }
    }
}
