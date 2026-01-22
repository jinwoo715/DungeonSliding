using UnityEngine;

namespace JW.SlidingPuzzle {
    public class CretureStatViewer_UI : MonoBehaviour
    {
        private Transform _targetTransform;
        [SerializeField] private Vector3 _offset;

        [SerializeField] private CretureStat_UI _hpUI;
        [SerializeField] private CretureStat_UI _damageUI;

        public void Init(Creture target)
        {
            _targetTransform = target.transform;
            //target.ShowChangeHPEvent += UpdateStat;
        }

        private void LateUpdate()
        {
            if (_targetTransform == null)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                transform.forward = Camera.main.transform.forward;
                this.transform.position = _targetTransform.position + _offset;
            }
        }

        public void UpdateStat(ECretureStatType statType, int value)
        {
            if(statType == ECretureStatType.HP)
            {
                _hpUI.UpdateValue(value);
            }
            else
            {
                _damageUI.UpdateValue(value);
            }
        }
    }
}
