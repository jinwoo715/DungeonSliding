using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.UI;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class EnemyTooltipClicker : MonoBehaviour
    {
        private ITooltipService _tooltipService;

        public void Initialize(ITooltipService service)
        {
            _tooltipService = service;
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if(Physics.Raycast(ray, out var collider, 10000))
                {
                    Debug.Log(collider.transform.name);
                }

                if(Physics.Raycast(ray, out var hit, 100000, LayerMask.GetMask("Enemy")))
                {
                    Enemy enemy = hit.transform.GetComponent<Enemy>();

                    TooltipRequest request = new TooltipRequest();
                    request.Name = enemy.Name;
                    request.Description = enemy.Description;
                    request.Anchor = TextAnchor.UpperRight;

                    _tooltipService.ShowTooltip(request);
                }
                else
                {
                    _tooltipService.CloseTooltip();
                }
            }
        }
    }
}
