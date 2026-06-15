using UnityEngine;

namespace JW.DungeonSliding
{
    public class GameLoosePresenter
    {
        private GameResultViewer _viewer;

        public void Init(GameResultViewer viewer, IGameResultService gameResultService)
        {
            gameResultService.OnGameLose += ShowResult;
            _viewer = viewer;
        }
        public void ShowResult(GameResultPayload gameResultPayload)
        {
            _viewer.gameObject.SetActive(true);

            Debug.Log("ÄÑÁü");

            GamePlayInfoPayload gamePlay = gameResultPayload.GamePlay;
            PlayerInfoPayload playerInfoPayload = gameResultPayload.PlayerInfoPayload;
            PlayerAbilityInfo playerAbility = gameResultPayload.PlayerAbility;

            SetGameProgressInfo(gamePlay);
            _viewer.PrintResult();

            SetAbilityInfo(playerAbility);
        }

        private void SetGameProgressInfo(GamePlayInfoPayload gamePlay)
        {
            string payTurn = gamePlay.TotalSlideCount + "ÅÏ";

            int min = Mathf.FloorToInt(gamePlay.TotalPlayTime / 60);
            int second = Mathf.FloorToInt(gamePlay.TotalPlayTime % 60);
            int currentFloor = gamePlay.CurrentFloor;
            int maxFloor = gamePlay.MaxFloor;

            string time = $"{min} : {second}";
            string floor = $"{currentFloor} / {maxFloor}Ãþ";

            Debug.Log($"{payTurn} / {time} / {floor}");

            _viewer.AppendInfo(payTurn);
            _viewer.AppendInfo(time);
            _viewer.AppendInfo(floor);
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