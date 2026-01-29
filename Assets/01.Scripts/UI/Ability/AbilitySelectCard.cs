using JW.DungeonSliding.GamePlay.Ability;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class AbilitySelectCard : MonoBehaviour
    {
        [SerializeField] private Button _selectBtn;
        
        [SerializeField] private Image _abilityImage;
        
        [SerializeField] private TMP_Text _abilityName;
        [SerializeField] private TMP_Text _abilityDescription;

        private AbilityData _data;

        public event Action<int> SelectAbilityEvent;

        public void Init()
        {
            _selectBtn.onClick.AddListener(() => OnClickSelectButton());
        }

        public void SetData(AbilityData abilityData)
        {
            _data = abilityData;

            _abilityImage.sprite = _data.AbilitySprite;
            _abilityName.text = _data.Name;
            _abilityDescription.text = _data.Description;
        }

        public void OnClickSelectButton()
        {
            SelectAbilityEvent?.Invoke(_data.AbilityUID);
        }
    }
}
