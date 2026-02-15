#if UNITY_EDITOR

using System;

namespace JW.DungeonSliding 
{
    public enum EEnemyType
    {
        Nomal,
        Boss
    }

    public class EnemyPlacementService
    {
        private CretureTempleteEditor _cretureTempleteEditor = new CretureTempleteEditor();

        public void ProcessCreturePoint(MapEditState mapEditState, Tile point, int templeteNum, EEditorCretureType cretureType)
        {
            if (!mapEditState.IsRoute(point)) return;

            if (mapEditState.IsEffectTile(point)) return;

            CreatureTemplete templete = mapEditState.GetCretureTemplete(templeteNum);

            _cretureTempleteEditor.SetCreturePos(templete, cretureType, point);
        }
    }
}

#endif