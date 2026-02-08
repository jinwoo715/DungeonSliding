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

        public event Action<string> SelectAbilityEvent;

        public void Init()
        {
            _selectBtn.onClick.AddListener(() => OnClickSelectButton());
        }

        public void SetData(AbilityDataBase abilityData)
        {
            _data = abilityData;

            _cardImage.sprite = _cardSprite[(int)_data.Rank];

            //TODO Image Sprite
            //_abilityImage.sprite = _data.AbilitySprite;
            _abilityName.text = _data.Name;
            _abilityDescription.text = GetDescription(_data.Description, _data);
        }

        private string GetDescription(string description, AbilityDataBase data)
        {
            StringBuilder sb = new StringBuilder(description);

            if (data is StatAbilityData)
            {
                var sa = data as StatAbilityData;

                var convertList = new Dictionary<string, string>
                {
                    { "{StatValue}", sa.ApplyType == EApplyStatType.Add ? sa.StatValue.ToString() : (sa.StatValue * 100).ToString()},
                    { "{NextAttackValue}", sa.NextAttackType == GamePlay.Combat.ENextAttackType.Multiple ? 
                    (sa.NextAttackValue * 100).ToString() : sa.NextAttackValue.ToString() },

                    { "{NeedStackCount}", sa.NeedStackCount.ToString() },
                    { "{ResetThreshold}", sa.ResetThreshold.ToString() }
                };

                foreach (var replaceData in convertList)
                {
                    sb.Replace(replaceData.Key, replaceData.Value);
                }
            }
            else
            {
                var ra = data as RuleAbilityData;

                var convertList = new Dictionary<string, string>
                {
                    { "{P1}", ra.P1.ToString() },
                    { "{P2}", ra.P2.ToString() },
                };

                foreach (var replaceData in convertList)
                {
                    sb.Replace(replaceData.Key, replaceData.Value);
                }
            }

            return sb.ToString();
        }

        public void OnClickSelectButton()
        {
            SelectAbilityEvent?.Invoke(_data.UID);
        }
    }
}
