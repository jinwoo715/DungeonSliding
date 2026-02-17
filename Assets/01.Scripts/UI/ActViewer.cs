using JW.DungeonSliding.GamePlay.Context;
using TMPro;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class ActViewer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _actText;
        [SerializeField] private TMP_Text _floorText;
        
        IActService _actService;
        
        public void Initialize(IActService actService)
        {
            _actService = actService;

            _actService.OnChangeActEvent += UpdateAct;
            _actService.OnChangeFloorEvent += UpdateFloor;
        }

        public void UpdateAct(int act, int totalAct)
        {
            Debug.Log($"{act} {totalAct}");
            _actText.text = $"Act {act+1} / {totalAct}";
        }
        public void UpdateFloor(int floor, int totalFloor)
        {
            _floorText.text = $"{floor} / {totalFloor}";
        }
    }
}
