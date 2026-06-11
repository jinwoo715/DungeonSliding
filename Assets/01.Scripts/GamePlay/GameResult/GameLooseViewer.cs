using JW.DungeonSliding.GamePlay.Ability;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public enum ELooseReasonType
    {
        HP,
        MoveCount
    }

    public struct GameResultPayload
    {
        public GamePlayInfoPayload GamePlay;
        public PlayerInfoPayload PlayerInfoPayload;
        public PlayerAbilityInfo PlayerAbility;
    }

    [System.Serializable]
    public struct GamePlayInfoPayload
    {
        public int CurrentFloor;
        public int MaxFloor;
        public int TotalSlideCount;
        public float TotalPlayTime;
    }
    public struct PlayerInfoPayload
    {
        public int HP;
        public int MaxHP;
        public int Damage;
        public int Critical;
        public int Move;
        public int MaxMove;
    }

    public struct PlayerAbilityInfo
    {
        public List<AbilityDataBase> Lists;
    }



    public class GameLooseViewer : GameResultViewerBase
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TMP_Text _looseReasonText;

        [SerializeField] private Button _toLobbyButton;
        [SerializeField] private Button _retryButton;

        public void ShowLoosePanel(GameResultPayload gameResultPayload)
        {
            _group.alpha = 1;
        }
    }
}
