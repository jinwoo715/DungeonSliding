using System;
using UnityEngine;

namespace JW.DungeonSliding
{
    public interface IGameResultService
    {
        event Action<GameResultPayload> OnGameWin;
        event Action<GameResultPayload> OnGameLose;
    }

    public class GameWinPresenter
    {
        private GameResultViewer _viewer;

        public void Init(GameResultViewer viewer, IGameResultService gameResultService)
        {
            gameResultService.OnGameWin += ShowWinResult;
            _viewer = viewer;
        }
        public void ShowWinResult(GameResultPayload gameResultPayload)
        {
            _viewer.gameObject.SetActive(true);

            GamePlayInfoPayload gamePlay = gameResultPayload.GamePlay;
            PlayerInfoPayload playerInfoPayload = gameResultPayload.PlayerInfoPayload;
            PlayerAbilityInfo playerAbility = gameResultPayload.PlayerAbility;

            _viewer.AppendEnter();

            SetGameProgressInfo(gamePlay);
            SetPlayerStatInfo(playerInfoPayload);
            SetAbilityInfo(playerAbility);

            _viewer.PrintResult();
        }
        private void SetGameProgressInfo(GamePlayInfoPayload gamePlay)
        {
            string payTurn = gamePlay.TotalSlideCount + "ео";

            int min = Mathf.FloorToInt(gamePlay.TotalPlayTime / 60);
            int second = Mathf.FloorToInt(gamePlay.TotalPlayTime % 60);

            string time = $"{min} : {second}";

            _viewer.AppendInfo(payTurn);
            _viewer.AppendInfo(time);
        }
        private void SetPlayerStatInfo(PlayerInfoPayload playerInfoPayload) 
        {
            string hp = $"{playerInfoPayload.HP} / {playerInfoPayload.MaxHP}";
            string damage = $"{playerInfoPayload.Damage}";
            string move = $"{playerInfoPayload.Move} / {playerInfoPayload.MaxMove}";
            string critical = $"{playerInfoPayload.Critical}%";

            _viewer.AppendInfo(hp);
            _viewer.AppendInfo(damage);
            _viewer.AppendInfo(move);
            _viewer.AppendInfo(critical);

         
        }
        private void SetAbilityInfo(PlayerAbilityInfo abilityInfo)
        {
            _viewer.AbilityClear();

            foreach (var ability in abilityInfo.Lists)
            {
                _viewer.SetAbilityData(ability);
            }
        }
    }
}
