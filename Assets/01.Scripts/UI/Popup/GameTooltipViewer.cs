using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public enum EGamePopupType
    {
        Ability,
        Xp,
    }

    public class GameTooltipViewer : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _contentsText;

        public void SetPosition(Vector2 position)
        {
            this.transform.position = position;
        }

        public void SetData(Sprite icon, string name, string content)
        {
            if (icon == null) _iconImage.gameObject.SetActive(false);
            else
            {
                _iconImage.gameObject.SetActive(true);
                _iconImage.sprite = icon;
            }

            _nameText.text = name;
            _contentsText.text = content;
        }
    }
}
