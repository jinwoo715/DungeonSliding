using System.Collections.Generic;
using UnityEngine;

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
        [SerializeField] private float _arrowXOffset = 72.9f;

        private readonly List<GameObject> _createdLines = new();
        private readonly List<GameObject> _createdBossMarks = new();

        private float _floorYOffset;
        private float _startYOffset;
        private int _totalFloorCount;

        public void Init(int totalFloorCount, List<int> bossFloors)
        {
            Debug.Log($"{totalFloorCount}, {bossFloors.Count}");

            ClearCreatedObjects();

            _totalFloorCount = Mathf.Max(0, totalFloorCount);

            if (_totalFloorCount <= 0)
            {
                if (_arrow != null)
                    _arrow.gameObject.SetActive(false);

                return;
            }

            if (_arrow != null)
                _arrow.gameObject.SetActive(true);

            _floorYOffset = _totalFloorCount > 1 ? _rectTransform.sizeDelta.y / (_totalFloorCount - 1) : 0;


            _startYOffset = _rectTransform.position.y - (_rectTransform.sizeDelta.y / 2);

            HashSet<int> bossFloorSet = bossFloors != null
                ? new HashSet<int>(bossFloors)
                : new HashSet<int>();

            for (int floor = 1; floor <= _totalFloorCount; floor++)
            {
                Vector3 floorPosition = GetFloorPosition(floor);

                var line = Instantiate(_dividLine, _dividLineParent);
                line.transform.position = floorPosition;

                _createdLines.Add(line);

                if (bossFloorSet.Contains(floor))
                {
                    var mark = Instantiate(_bossMarkObject, _bossMarkParent);
                    mark.transform.position = floorPosition;
                    _createdBossMarks.Add(mark);
                }
            }

            UpdateFloor(1);
        }

        public void UpdateFloor(int floor)
        {
            if (_arrow == null || _totalFloorCount <= 0)
                return;

            int clampedFloor = Mathf.Clamp(floor, 1, _totalFloorCount);
            Vector3 floorPosition = GetFloorPosition(clampedFloor);
            _arrow.transform.position = new Vector3(_rectTransform.position.x - _arrowXOffset, floorPosition.y);
        }

        private Vector3 GetFloorPosition(int floor)
        {
            int floorIndex = Mathf.Clamp(floor - 1, 0, Mathf.Max(0, _totalFloorCount - 1));
            return new Vector3(_rectTransform.position.x, _startYOffset + _floorYOffset * floorIndex);
        }

        private void ClearCreatedObjects()
        {
            DestroyAll(_createdLines);
            DestroyAll(_createdBossMarks);
        }

        private void DestroyAll(List<GameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i] != null)
                    Destroy(objects[i]);
            }

            objects.Clear();
        }
    }
}
