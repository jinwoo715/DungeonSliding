using JW.DungeonSliding.UI;
using JW.DungeonSliding.UI.Visual;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI.Visual
{
    public interface IUIFaderService
    {
        public event Action OnCompleteFadeOut;
        public event Action OnCompleteFadeIn;
        void FadeOut(Action OnEndCallback = null);
        void FadeIn(Action OnEndCallback = null);
    }
}

namespace JW.DungeonSliding
{
    public class Fader : MonoBehaviour, IUIFaderService
    {
        [SerializeField] private CanvasGroup _fadeImage;

        public event Action OnCompleteFadeOut;
        public event Action OnCompleteFadeIn;
        public void FadeIn(Action OnEndCallback = null)
        {
            StartCoroutine(CoFadeIn(OnEndCallback));
        }

        public IEnumerator CoFadeIn(Action OnEndCallback = null)
        {
            float timer = _fadeImage.alpha;
            while (timer >= 0)
            {
                timer -= Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }
            _fadeImage.alpha = 0;
            OnCompleteFadeIn?.Invoke();
            OnEndCallback?.Invoke();
        }

        public void FadeOut(Action OnEndCallback = null)
        {
            StartCoroutine(CoFadeOut(OnEndCallback));
        }

        public IEnumerator CoFadeOut(Action OnEndCallback = null)
        {
            float timer = _fadeImage.alpha;
            while (timer < 1)
            {
                timer += Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }
            OnCompleteFadeOut?.Invoke();
            OnEndCallback?.Invoke();
            _fadeImage.alpha = 1;
        }
    }
}
