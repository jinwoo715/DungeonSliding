using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding
{
    public class StageViewer : MonoBehaviour
    {
        [SerializeField] private Transform _dividLineParent;
        [SerializeField] private GameObject _dividLine;

        [SerializeField] private RectTransform _rectTransform;

        [SerializeField] private Transform _bossMarkParent;
        [SerializeField] private GameObject _bossMarkObject;

        [SerializeField] private RectTransform _arrow;

        public List<float> yOffsets = new List<float>();

        public float _floorYOffset;
        public float _startYOffset;

        public int totalCount;
        public int bossCount;

        [ContextMenu("Test")]
        public void Test()
        {
            //Init(totalCount, bossCount);
        }

        public int floor = 0;

        [ContextMenu("Test2")]
        public void TestFloor()
        {
            UpdateFloor(floor);
        }

        public void Init(int totalFloorCount, List<int> bossTerm)
        {
            _floorYOffset = _rectTransform.sizeDelta.y / (totalFloorCount-1);
            _startYOffset = _rectTransform.position.y - (_rectTransform.sizeDelta.y / 2);

            int index = 0;

            for (int i = 1; i <= totalFloorCount; i++)
            {
                var line = Instantiate(_dividLine, _dividLineParent);
                line.transform.position = new Vector3(_rectTransform.position.x, _startYOffset + _floorYOffset * (i-1));

                if (i == bossTerm[index])
                {
                    var mark = Instantiate(_bossMarkObject, _bossMarkParent);
                    mark.transform.position = new Vector3(_rectTransform.position.x, _startYOffset + _floorYOffset * (i-1));
                    index++;
                }
            }
        }

        public void UpdateFloor(int floor)
        {
            _arrow.transform.position = new Vector3(_rectTransform.position.x - 72.9f, _startYOffset + _floorYOffset * floor);
        }
    }
}
