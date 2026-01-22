using UnityEngine;

namespace JW.SlidingPuzzle
{
    public class CretureStatController : MonoBehaviour
    {
        [SerializeField] private Transform _statUICanvas;
        [SerializeField] private CretureStatViewer_UI _statViewerPrefab;

        public void CreateStatViewer(Creture target)
        {
            CretureStatViewer_UI statViewer = Instantiate(_statViewerPrefab, _statUICanvas);
            statViewer.Init(target);
        }
    }
}
