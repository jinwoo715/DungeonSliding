using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class CretureStatController : MonoBehaviour
    {
        [SerializeField] private Transform _statUICanvas;
        [SerializeField] private CretureStatViewer_UI _statViewerPrefab;

        public void CreateStatViewer(Creature target)
        {
            CretureStatViewer_UI statViewer = Instantiate(_statViewerPrefab, _statUICanvas);
            statViewer.Init(target);
        }
    }
}
