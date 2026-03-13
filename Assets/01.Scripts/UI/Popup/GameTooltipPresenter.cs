using UnityEngine;
using UnityEngine.UI;

namespace JW.DungeonSliding.UI
{
    public interface ITooltipService
    {
        void ShowTooltip(TooltipRequest tooltipContext);
        void CloseTooltip();
    }

    public struct TooltipRequest
    {
        public Sprite IconSprite;
        public string Name;
        public string Description;
        public TextAnchor Anchor;
    }

    public class GameTooltipPresenter : MonoBehaviour, ITooltipService
    {
        [SerializeField] private GameTooltipViewer _viewer;
        [SerializeField] private LayoutGroup _layoutGroup;

        public void ShowTooltip(TooltipRequest tooltipContext)
        {
            _viewer.SetData(tooltipContext.IconSprite, tooltipContext.Name, tooltipContext.Description);
            _viewer.gameObject.SetActive(true);

            _layoutGroup.childAlignment = tooltipContext.Anchor;
        }
       
        public void CloseTooltip()
        {
            _viewer.gameObject.SetActive(false);
        }
    }
}
