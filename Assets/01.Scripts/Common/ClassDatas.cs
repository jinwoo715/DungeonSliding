using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding
{
    [System.Serializable]
    public class ActMapDataBundle
    {
        public string BundleName;
        public int ActFloorCount;
        public List<MapData> MapDatas;
    }

    public class MapProvider
    {
        [SerializeField] private List<ActMapDataBundle> Bundles;

        public MapProvider(List<ActMapDataBundle> bundles)
        {
            Bundles = bundles;
        }

        public ActMapDataBundle GetActMapBundle(int actNum)
        {
            if (Bundles.Count < actNum) return null;

            return Bundles[actNum];
        }

    }

    [System.Serializable]
    public class CreatureTemplete
    {
        public List<Tile> NomalEnemyPos = new List<Tile>();
        public List<Tile> BossEnemyPos = new List<Tile>();
        public Tile PlayerPos = Tile.Invalid;
    }

    public class CretureTempleteEditor
    {
        public void SetCreturePos(CreatureTemplete cretureTemplete, EEditorCretureType creatureType, Tile tile)
        {
            if(TryRemovePos(cretureTemplete, tile, out var removeType))
            {
                if(creatureType != removeType)
                {
                    SetCreaturePos(cretureTemplete, creatureType, tile);
                }
            }
            else
            {
                SetCreaturePos(cretureTemplete, creatureType, tile);
            }
        }

        private void SetCreaturePos(CreatureTemplete cretureTemplete, EEditorCretureType creatureType, Tile tile)
        {
            switch (creatureType)
            {
                case EEditorCretureType.Player:
                    SetPlayerPos(cretureTemplete, tile);
                    break;
                case EEditorCretureType.NomalEnemy:
                    SetEnemyPos(cretureTemplete, tile);
                    break;
                case EEditorCretureType.BossEnemy:
                    SetBossPos(cretureTemplete, tile);
                    break;
            }
        }

        public void SetPlayerPos(CreatureTemplete cretureTemplete, Tile tile) 
        {
            cretureTemplete.PlayerPos = tile;
        }
        public void SetEnemyPos(CreatureTemplete cretureTemplete, Tile tile) 
        {
            cretureTemplete.NomalEnemyPos.Add(tile);
        }
        public void SetBossPos(CreatureTemplete cretureTemplete, Tile tile) 
        {
            cretureTemplete.BossEnemyPos.Add(tile);
        }
        public bool TryRemovePos(CreatureTemplete cretureTemplete, Tile tile, out EEditorCretureType removeType)
        {
            if (cretureTemplete.PlayerPos == tile)
            {
                cretureTemplete.PlayerPos = Tile.Invalid;
                removeType = EEditorCretureType.Player;
                return true;
            }

            if (cretureTemplete.NomalEnemyPos.Contains(tile))
            {
                cretureTemplete.NomalEnemyPos.Remove(tile);
                removeType = EEditorCretureType.NomalEnemy;
                return true;
            }

            if (cretureTemplete.BossEnemyPos.Contains(tile))
            {
                cretureTemplete.BossEnemyPos.Remove(tile);
                removeType = EEditorCretureType.BossEnemy;
                return true;
            }

            removeType = default;
            return false;
        }
    }



    [System.Serializable]
    public class EnemyTemplete2
    {
        public List<Tile> NomalEnemyPos = new List<Tile>();
        public List<Tile> BossEnemyPos = new List<Tile>();
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
    public class EnemySettingData
    {
        public Tile Point;

        public EnemySettingData(Tile point)
        {
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
