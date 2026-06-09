using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public static class IntFormatter
    {
        //public static string Format(int num)
        //{

        //}
    }

    public class PlayerStatViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private TMP_Text _moveCountText;
        [SerializeField] private TMP_Text _criticalText;
        [SerializeField] private TMP_Text _levelText;

        [SerializeField] private TMP_Text _extraAttackCountText;
        [SerializeField] private TMP_Text _extraAttackDamageText;

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
            _levelProgressImage.fillAmount = (float)current / (float)max;
        }
        public void UpdateAttackCount(int count)
        {
            if(count == 0)
                _extraAttackCountText.text = "";
            else
                _extraAttackCountText.text = $"X{count+1}";
        }

        public void UpdateCriticalMultiple(int multiple)
        {
            _criticalText.text = $"{multiple}%";
        }

        public void UpdateNextAttackExtraDamage(int damage)
        {
            if (damage == 0)
                _extraAttackDamageText.text = "";
            else
                _extraAttackDamageText.text = $"(+{damage})";
        }
    }
}
