#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace JW.SlidingPuzzle
{
    public class EffectObjectPlacementService
    {
        public void ProcessEffectObject(MapEditState mapEditState, TilePoint point, EEffectObjectType effectObjIndex)
        {
            if (!mapEditState.IsRoute(point))
                return;

            if (mapEditState.PlayerPoint == point)
                return;

            if (mapEditState.IsExistEnemy(point))
                return;

            if (mapEditState.TryGetEffectObject(point, out EffectObjectData exist))
            {
                if (exist.EffectObjectType == effectObjIndex)
                {
                    if (exist.EffectObjectType == EEffectObjectType.Teleport)
                        UnlinkTeleport(mapEditState, exist);

                    mapEditState.RemoveEffectObject(point);
                }
                return;
            }

            if (effectObjIndex == EEffectObjectType.Teleport)
            {
                HandleAddTeleport(mapEditState, point);
                return;
            }

            mapEditState.SetEffectObj(new EffectObjectData(point, effectObjIndex));
        }

        private void UnlinkTeleport(MapEditState mapEditState, EffectObjectData exist)
        {
            if (exist.TeleportPoint.IsValid)
            {
                if(mapEditState.TryGetEffectObject(exist.TeleportPoint, out var other))
                {
                    other.TeleportPoint.Expire();
                    mapEditState.SetEffectObj(other);
                }
            }
        }

        private void HandleAddTeleport(MapEditState mapEditState, TilePoint tile)
        {
            int tCount = mapEditState.GetTeleports(out var t1, out var t2);

            if (tCount >= 2)
            {
                UnityEngine.Debug.Log("Teleport는 1쌍(2개)까지만 배치할 수 있어요.");
                return;
            }

            if (tCount == 0)
            {
                mapEditState.SetEffectObj(new EffectObjectData(tile, EEffectObjectType.Teleport));
                return;
            }

            if (tCount == 1)
            {
                EffectObjectData teleport0 = new EffectObjectData(tile, EEffectObjectType.Teleport) { TeleportPoint = t1};
                mapEditState.SetEffectObj(teleport0);

                if (mapEditState.TryGetEffectObject(t1, out var oldTeleport))
                {
                    oldTeleport.TeleportPoint = tile;
                    mapEditState.SetEffectObj(oldTeleport);
                }
            }
         
        }
    }
}
#endif
