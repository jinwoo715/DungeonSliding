using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class BarrierVisualViewer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private Material _originMat;
        private Material _barrierMat;

        [Header("Fade Tile Multiplier")]
        [SerializeField] private float _fadeOutLerpMultiplier;
        [SerializeField] private float _fadeInlerpMultiplier;

        private Coroutine _fadeInCoroutine;
        private Coroutine _fadeOutCoroutine;

        private void Start()
        {
            _barrierMat = new Material(_originMat);
            _barrierMat.SetFloat("_TotalAlpha", 0f);

            _renderer.material = _barrierMat;
        }

        public void ExcuteBarrier()
        {
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);

            _fadeInCoroutine = StartCoroutine(FadeIn());
        }
        public void BreakBarrier()
        {
            if (_fadeInCoroutine != null) StopCoroutine(_fadeInCoroutine);
            if (_fadeOutCoroutine != null) StopCoroutine(_fadeOutCoroutine);

            _fadeOutCoroutine = StartCoroutine(FadeOut());
        }

        private IEnumerator FadeOut()
        {
            float currentTime = _barrierMat.GetFloat("_TotalAlpha");

            while(currentTime > 0)
            {
                currentTime -= Time.deltaTime * _fadeOutLerpMultiplier;

                float t = currentTime / 1;

                _barrierMat.SetFloat("_TotalAlpha", currentTime);

                yield return null;
            }

            _barrierMat.SetFloat("_TotalAlpha", 0f);
        }
        private IEnumerator FadeIn()
        {
            float currentTime = _barrierMat.GetFloat("_TotalAlpha");

            while (currentTime < 1)
            {
                currentTime += Time.deltaTime * _fadeInlerpMultiplier;

                _barrierMat.SetFloat("_TotalAlpha", currentTime);

                yield return null;
            }

            _barrierMat.SetFloat("_TotalAlpha", 1f);
        }
    }
}
