using TMPro;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class EnemyTooltipViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;

        public void ShowTooltip(string name, string description)
        {
            _name.text = name;
            _description.text = description;
        }

    }
}
