using JW.DungeonSliding.GamePlay.Entities;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class CretureStatController : MonoBehaviour
    {
        [SerializeField] private Transform _statUICanvas;
        [SerializeField] private EnemyStatViewItem _statViewerPrefab;

        //public void CreateStatViewer(Creature target)
        //{
        //    EnemyStatViewItem statViewer = Instantiate(_statViewerPrefab, _statUICanvas);
        //    statViewer.Init(target);
        //}
    }
}
