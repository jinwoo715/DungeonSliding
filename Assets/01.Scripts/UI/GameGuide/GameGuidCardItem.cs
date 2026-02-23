using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class GameGuidCardItem : MonoBehaviour
    {
        [SerializeField] private Image _guideImage;
        [SerializeField] private GameObject _dotLine;
        [SerializeField] private GameObject _imageObject;
        [SerializeField] private TMP_Text _guidText;

        public void SetData(Sprite sprite, string description)
        {
            bool imageValid = sprite == null ? false : true;

            _imageObject.gameObject.SetActive(imageValid);
            _dotLine.SetActive(imageValid);

            if(imageValid) _guideImage.sprite = sprite;

            _guidText.text = description;
        }
    }
}
