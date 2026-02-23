using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JW.DungeonSliding
{
    public class StartButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image _doorImage;
        [SerializeField] private Button _button;
        [SerializeField] private Sprite[] _sprite;
        public void Initialize(Action clickEvent)
        {
            _button.onClick.AddListener(() => clickEvent?.Invoke());
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _doorImage.sprite = _sprite[1];
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            _doorImage.sprite = _sprite[0];
        }
    }
}
