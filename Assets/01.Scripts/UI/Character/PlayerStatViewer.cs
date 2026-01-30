using TMPro;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class PlayerStatViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _hpText;
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private TMP_Text _moveCountText;

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
    }
}
