using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.UI;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class EnemyTooltipHandler : MonoBehaviour
    {
        private ITooltipService _tooltipService;
        private Collider _currentHoverEnemy;
        private Camera _camera;
        private LayerMask _layer;

        public void Init(ITooltipService service)
        {
            _tooltipService = service;

            _camera = Camera.main;
            _layer = LayerMask.GetMask("Enemy");
        }

        private void Update()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (_tooltipService == null)
                return;

            if (Physics.Raycast(ray, out var hit, 100000, _layer))
            {
                if (_currentHoverEnemy == hit.collider)
                    return;

                _currentHoverEnemy = hit.collider;
                Enemy hoverEnemy = _currentHoverEnemy.GetComponent<Enemy>();

                TooltipRequest request = new TooltipRequest();
                request.Name = hoverEnemy.Name;
                request.Description = hoverEnemy.Description;
                request.Anchor = TextAnchor.UpperRight;

                _tooltipService.ShowTooltip(request);
            }
            else
            {
                _currentHoverEnemy = null;
                _tooltipService.CloseTooltip();
            }
        }
    }
}
