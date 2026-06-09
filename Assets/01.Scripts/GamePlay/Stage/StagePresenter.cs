using JW.DungeonSliding.GamePlay.Stage;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class StagePresenter : MonoBehaviour
    {
        private IStageService _stageService;
        private StageViewer _viewer;

        public void Init(IStageService stageService, IStageViewer stageData, StageViewer viewer)
        {
            Unbind();

            _stageService = stageService;
            _viewer = viewer;

            _viewer.Init(stageData.TotalFloor, stageData.BossFloors);
            _viewer.UpdateFloor(stageData.CurrentFloor);

            _stageService.OnChangeFloorEvent += _viewer.UpdateFloor;
        }

        private void OnDestroy()
        {
            Unbind();
        }

        private void Unbind()
        {
            if (_stageService != null && _viewer != null)
                _stageService.OnChangeFloorEvent -= _viewer.UpdateFloor;

            _stageService = null;
            _viewer = null;
        }
    }
}
