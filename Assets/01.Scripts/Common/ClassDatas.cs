using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    [CreateAssetMenu(fileName = "MapData", menuName = "Data/Map", order = 1)]
    public class MapData : ScriptableObject
    {
        public int Width;
        public int Height;
        public int[] MapTiles;

        public Tile PlayerPosition;
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
        public Dictionary<Tile, EnemySettingData> EnemyData = new Dictionary<Tile, EnemySettingData>();
    }

    [System.Serializable]
    public class EnemyData
    {
        public string UID;
        public string Name;
        public string Description;
        public int BaseHP;
        public int BaseDamage;
        public int BaseXP;
        public string AbilityList;
    }

    [System.Serializable]
    public class EnemyBossData : EnemyData
    {
        public EEnemyAbilityType AbilityType;
        public float P1;
        public float P2;
    }


    [System.Serializable]
    public class EnemySettingData
    {
        public string EnemyUID;
        public Tile Point;

        public EnemySettingData(string enemyUID, Tile point)
        {
            EnemyUID = enemyUID;
            Point = point;
        }
    }

    [System.Serializable]
    public class EffectObjectSettingData
    {
        public int EffectObjUID;
        public Tile Point;
        public Tile TeleportPoint;
        public EffectObjectSettingData(int enemyUID, Tile point)
        {
            EffectObjUID = enemyUID;
            Point = point;
        }
    }

    [System.Serializable]
    public class EnemyDataSheet
    {
        public string EnemyUID;
        public string EnemyName;
        public int BaseHP;
        public int BaseAttack;
        public int Xp;
    }
}
