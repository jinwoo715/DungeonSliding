using System.Collections.Generic;
using UnityEngine;

namespace JW.SlidingPuzzle
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Data/Map", order = 1)]
    public class MapData : ScriptableObject
    {
        public int Width;
        public int Height;
        public int[] MapTiles;

        public TilePoint PlayerPosition;
        public EnemyTemplete[] EnemyTemplete;
        public List<EffectObjectData> effectTileDatas;
    }

    [System.Serializable]
    public class EnemyTemplete
    {
        public List<EnemySettingData> EnemyData = new List<EnemySettingData>();
    }

    [System.Serializable]
    public class EnemyTempleteSheet
    {
        public Dictionary<TilePoint, EnemySettingData> EnemyData = new Dictionary<TilePoint, EnemySettingData>();
    }

    [System.Serializable]
    public class EnemyData
    {
        public int EnemyUID;
        public string EnemyName;
        public int BaseHP;
        public int BaseDamage;
        public int Xp;
    }

    [System.Serializable]
    public class EnemySettingData
    {
        public int EnemyUID;
        public TilePoint Point;

        public EnemySettingData(int enemyUID, TilePoint point)
        {
            EnemyUID = enemyUID;
            Point = point;
        }
    }

    [System.Serializable]
    public class EffectObjectSettingData
    {
        public int EffectObjUID;
        public TilePoint Point;
        public TilePoint TeleportPoint;
        public EffectObjectSettingData(int enemyUID, TilePoint point)
        {
            EffectObjUID = enemyUID;
            Point = point;
        }
    }

    [System.Serializable]
    public class EnemyDataSheet
    {
        public int EnemyUID;
        public string EnemyName;
        public int BaseHP;
        public int BaseAttack;
        public int Xp;
    }
}
