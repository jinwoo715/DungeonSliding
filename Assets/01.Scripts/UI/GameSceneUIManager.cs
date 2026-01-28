using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public interface IUIFader
    {
        IEnumerator FadeOut();
        IEnumerator FadeIn();
    }

    public class GameSceneUIManager : MonoBehaviour, IUIFader
    {
        [SerializeField] private FadeController _fadeController;

        public IEnumerator FadeIn()
        {
            yield return _fadeController.CoFadeIn();
        }

        public IEnumerator FadeOut()
        {
            yield return _fadeController.CoFadeOut();
        }

        public void Init()
        {
            ChildInit();
        }
        private void ChildInit()
        {
            _fadeController.Init();
        }

    }
}
