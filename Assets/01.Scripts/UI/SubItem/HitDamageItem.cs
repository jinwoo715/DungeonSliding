using System.Collections;
using TMPro;
using UnityEngine;
using JW.Utility;
namespace JW.DungeonSliding
{
    public class HitDamageItem : PoolObject
    {
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private Color _originColor;
        [SerializeField] private Color _fadeOutColor;

        private Coroutine _moveCoroutine;

        public void Init(int value, float timer)
        {
            _damageText.text = value.ToString();
            _damageText.color = _originColor;

            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);

            _moveCoroutine = StartCoroutine(CoMoveDamageText(timer));
        }

        public override void OnDespawn()
        {
            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
        }

        public override void OnSpawn()
        {
        }

        private IEnumerator CoMoveDamageText(float timer)
        {
            float time = 0;

            float invTimer = 1 / timer;

            while (time <= 1)
            {
                time += Time.deltaTime * invTimer;

                this.transform.position += Vector3.up * Time.deltaTime * 30f;

                _damageText.color = Color.Lerp(_originColor, _fadeOutColor, timer);

                yield return null;
            }

            Release();
        }
    }
}
