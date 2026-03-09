using JW.DungeonSliding.GamePlay.Ability;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public class AbilitySelectView : MonoBehaviour
    {
        [SerializeField] private Button _rerollButton;
        [SerializeField] private TMP_Text _rerollButtonText;

        [SerializeField] private AbilitySelectCard[] _abilityCards;

        private Action<AbilityDataBase> SelectCardEvent;
        private Action RerollEvent;

        public void Init()
        {
            _rerollButton.onClick.AddListener(() => OnClickRerollButton());

            for (int i = 0; i < _abilityCards.Length; i++)
            {
                _abilityCards[i].Init();
                _abilityCards[i].SelectAbilityEvent += OnClickAbilityCard;
            }
        }

        internal void Bind(Action<AbilityDataBase> selectAbility, Action reroll)
        {
            SelectCardEvent = selectAbility;
            RerollEvent = reroll;
        }

        public void SetAilityDatas(AbilityDataBase[] datas, int rerollCount)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                _abilityCards[i].SetData(datas[i]);
            }

            if (rerollCount <= 0)
                _rerollButton.interactable = false;
            else
                _rerollButton.interactable = true;

            _rerollButtonText.text = rerollCount.ToString();
        }
        public void OnClickAbilityCard(AbilityDataBase abilityData)
        {
            SelectCardEvent?.Invoke(abilityData);
        }
        public void OnClickRerollButton()
        {
            RerollEvent?.Invoke();
        }
    }
}
