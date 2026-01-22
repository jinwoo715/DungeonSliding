#if UNITY_EDITOR

using System;

namespace JW.SlidingPuzzle 
{
    public class EnemyPlacementService
    {
        public void ProcessEnemyPoint(MapEditState mapEditState, TilePoint point, int templeteNum, int enemyUid)
        {
            if (!mapEditState.IsRoute(point)) return;

            if (mapEditState.PlayerPoint == point) return;

            EnemySettingData data = mapEditState.GetEnemy(templeteNum, point);

            if(data == null)
            {
                mapEditState.SetEnemy(templeteNum, point, enemyUid);
            }
            else
            {
                if(data.EnemyUID == enemyUid)
                {
                    mapEditState.RemoveEnemy(templeteNum, point);
                }
                else
                {
                    mapEditState.SetEnemy(templeteNum, point, enemyUid);
                }
            }
        }
    }
}

#endif