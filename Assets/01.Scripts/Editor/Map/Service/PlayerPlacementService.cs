using JW.SlidingPuzzle;

public class PlayerPlacementService
{

    public void ProcessPlayerPoint(MapEditState mapEditState, TilePoint point)
    {
        if (!mapEditState.IsRoute(point))
            return;

        if (mapEditState.PlayerPoint == point)
        {
            mapEditState.SetPlayerPoint(new TilePoint(-1, -1));
            return;
        }

        if (mapEditState.IsExistEnemy(point))
        {
            return;
        }

        mapEditState.SetPlayerPoint(point);
    }
}
