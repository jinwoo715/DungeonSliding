using JW.DungeonSliding.GamePlay.Ability;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class AbilitySelectCard : MonoBehaviour
    {
        [SerializeField] private Button _selectBtn;

        [SerializeField] private Image _cardImage;
        [SerializeField] private Image _abilityImage;
        
        [SerializeField] private TMP_Text _abilityName;
        [SerializeField] private TMP_Text _abilityDescription;

        [SerializeField] private Sprite[] _cardSprite;

        private AbilityDataBase _data;

        public event Action<AbilityDataBase> SelectAbilityEvent;

        public void Init()
        {
            _selectBtn.onClick.AddListener(() => OnClickSelectButton());
        }

        public void SetData(AbilityDataBase abilityData)
        {
            _data = abilityData;

            _cardImage.sprite = _cardSprite[(int)_data.Rank];

            _abilityImage.sprite = _data.AbilitySprite;
            _abilityName.text = _data.Name;

            _abilityDescription.text = GetDescription(abilityData);
        }

        private string GetDescription(AbilityDataBase abilityData)
        {
            if (abilityData is RuleStatAbilityData rsa)
                return AbilityTextFormatter.ConvertRuleStatAbilityDescription(rsa);

            if (abilityData is StatAbilityData sa)
                return AbilityTextFormatter.ConvertStatAbilityDescription(sa);

            if (abilityData is RuleAbilityData ra)
                return AbilityTextFormatter.ConvertRuleAbilityDescription(ra);

            return string.Empty;
        }

        public void OnClickSelectButton()
        {
            SelectAbilityEvent?.Invoke(_data);
        }
    }
}
