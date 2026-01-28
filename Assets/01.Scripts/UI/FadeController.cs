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

        public Action OnEndFadeOutEvent;
        public Action OnEndFadeInEvent;

        public void Init()
        {
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.FadeOutStart, FadeOut);
            GameTriggerEventBus.Instance.SubscribeTriggerEvent(EGameTriggerType.FadeInStart, FadeIn);
        }

        public void FadeIn()
        {
            StartCoroutine(CoFadeIn());
        }

        public IEnumerator CoFadeIn() 
        {
            Debug.Log("Fade In");
            float timer = _fadeImage.alpha;
            while (timer >= 0)
            {
                timer -= Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }
            _fadeImage.alpha = 0;
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.FadeInFin);
            OnEndFadeOutEvent?.Invoke();
        }

        public void FadeOut()
        {
            StartCoroutine(CoFadeOut());
        }

        public IEnumerator CoFadeOut()
        {
            Debug.Log("Fade Out");
            float timer = _fadeImage.alpha;
            while (timer < 1)
            {
                timer += Time.deltaTime * UIConstDatas.FADE_TIMER_MULTIPLIER;
                _fadeImage.alpha = timer;
                yield return null;
            }
            _fadeImage.alpha = 1;
            GameTriggerEventBus.Instance.ExcuteAbilityEvent(EGameTriggerType.FadeOutFin);
            OnEndFadeOutEvent?.Invoke();
        }
    }
}
