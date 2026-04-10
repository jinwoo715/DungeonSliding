using JW.DungeonSliding.GamePlay.Context;
using System;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class GamePopupPresenter : MonoBehaviour, IPopupService
    {
        [SerializeField] private GamePopupViewer OneButtonPopup;

        public void Init()
        {
            OneButtonPopup.Init();
            OneButtonPopup.OnClickEvent += ClosePopup;
        }

        public void ShowOneButtonPopup(string name, string desc, ButtonSet buttonSet)
        {
            OneButtonPopup.SetData(name, desc, buttonSet);
            OneButtonPopup.gameObject.SetActive(true);
        }

        public void ClosePopup()
        {
            OneButtonPopup.gameObject.SetActive(false);
        }
    }
}
