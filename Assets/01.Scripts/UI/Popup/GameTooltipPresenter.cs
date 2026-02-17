using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public interface ITooltipService
    {
        void ShowTooltip(TooltipRequest tooltipContext);
        void CloseTooltip();
    }

    public enum ETooltipPositionAnchorType
    {
        UpperLeft,
        UpperRight,
        MiddleLeft,
        MiddleRight,
        BottomLeft,
        BottomRight
    }

    public struct TooltipRequest
    {
        public Sprite IconSprite;
        public string Name;
        public string Description;
        public Vector2 Position;
        public ETooltipPositionAnchorType Anchor;
    }

    public class GameTooltipPresenter : MonoBehaviour, ITooltipService
    {
        [SerializeField] private GameTooltipViewer _viewer;

        public void ShowTooltip(TooltipRequest tooltipContext)
        {
            Debug.Log(tooltipContext.Position);

            Vector2 tooltipPosition = tooltipContext.Position;
            tooltipPosition.x = 500;
            tooltipPosition.y = 400;

            _viewer.SetPosition(tooltipPosition);

            _viewer.SetData(tooltipContext.IconSprite, tooltipContext.Name, tooltipContext.Description);
            _viewer.gameObject.SetActive(true);
        }

        private Vector2 GetTooltipPosition(ETooltipPositionAnchorType anchorType)
        {
            Vector2 anchorPosition = Vector2.zero;

            switch (anchorType)
            {
                case ETooltipPositionAnchorType.UpperLeft:
                    break;
                case ETooltipPositionAnchorType.UpperRight:
                    break;
                case ETooltipPositionAnchorType.MiddleLeft:
                    break;
                case ETooltipPositionAnchorType.MiddleRight:
                    break;
                case ETooltipPositionAnchorType.BottomLeft:
                    break;
                case ETooltipPositionAnchorType.BottomRight:
                    break;
            }

            return anchorPosition;
        }

        public void CloseTooltip()
        {
            _viewer.gameObject.SetActive(false);
        }
    }
}
