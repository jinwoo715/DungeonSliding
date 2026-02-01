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

        private Action<int> SelectCardEvent;
        private Action RerollEvent;

        public void Init()
        {
            _rerollButton.onClick.AddListener(() => OnClickRerollButton());

            for (int i = 0; i < _abilityCards.Length; i++)
            {
                _abilityCards[i].Init();
                _abilityCards[i].SelectAbilityEvent += OnClickAbilityCard;
            }

            _rerollButton.onClick.AddListener(() => RerollEvent?.Invoke());
        }

        internal void Bind(Action<int> selectAbility, Action reroll)
        {
            SelectCardEvent = selectAbility;
            RerollEvent = reroll;
        }

        public void SetAilityDatas(AbilityData[] datas, int rerollCount)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                _abilityCards[i].SetData(datas[i]);
            }

            if (rerollCount <= 0)
                _rerollButton.enabled = false;

            _rerollButtonText.text = rerollCount.ToString();
        }
        public void OnClickAbilityCard(int abilityUid)
        {
            SelectCardEvent?.Invoke(abilityUid);
        }
        public void OnClickRerollButton()
        {
            RerollEvent?.Invoke();
        }
    }
}
