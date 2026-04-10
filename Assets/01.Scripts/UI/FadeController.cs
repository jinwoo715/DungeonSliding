using JW.DungeonSliding.GamePlay;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class FadeController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _fadeImage;

        public void SetAlpha(int alpha)
        {
            _fadeImage.alpha = alpha;
        }
        public IEnumerator CoFadeIn() 
        {
            float timer = _fadeImage.alpha;
            while (timer >= 0)
            {
                timer -= Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }
            _fadeImage.alpha = 0;
        }
        public IEnumerator CoFadeOut()
        {
            float timer = _fadeImage.alpha;
            while (timer < 1)
            {
                timer += Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }

            yield return new WaitForSeconds(0.5f);

            _fadeImage.alpha = 1;
        }
    }
}
