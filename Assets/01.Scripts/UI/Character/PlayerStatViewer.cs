using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class PlayerStatViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private TMP_Text _moveCountText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Image _levelProgressImage;

        public void UpdateHP(int currentHp, int maxHp)
        {
            _hpText.text = $"{currentHp} : {maxHp}";
        }
        public void UpdateDamage(int damage)
        {
            _damageText.text = $"{damage}";
        }
        public void UpdateMoveCount(int currentMoveCount, int maxMoveCount)
        {
            _moveCountText.text = $"{currentMoveCount} : {maxMoveCount}";
        }
        public void UpdateLevelText(int level)
        {
            _levelText.text = $"Lv {level}";
        }
        public void UpdateLevelProgress(int current, int max)
        {
            Debug.Log($"{current} / {max}");
            _levelProgressImage.fillAmount = (float)current / (float)max;
        }
    }
}
