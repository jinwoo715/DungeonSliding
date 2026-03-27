using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Configs/CombatConfig")]
    public class GameConfig : ScriptableObject
    {
        [System.Serializable]
        public class CombatConfig
        {
            [SerializeField] private float _backAttackDamageMultiplier;
            public float BackAttackDMGMultiple => _backAttackDamageMultiplier;
        }

        [System.Serializable]
        public class PlayerConfig
        {
            [SerializeField] private float _moveSpeed;

            [SerializeField] private int _baseHP;
            [SerializeField] private int _baseDamage;
            [SerializeField] private int _baseMoveCount;

            [SerializeField] private int _levelUpHp;
            [SerializeField] private int _levelUpDamage;

            [SerializeField] private int _levelUpHpRatio;
            [SerializeField] private int _levelUpDamageRatio;

            [SerializeField] private int _levelUpNeedValueRatio;

            public int HP => _baseHP;
            public int DMG => _baseDamage;
            public int MVCount => _baseMoveCount;
        }

        [System.Serializable]
        public class EnemyConfig
        {
            [SerializeField] private float _floorUpHpValue;
            [SerializeField] private float _floorUpDMGValue;
        }

        [System.Serializable]
        public class AbilityConfig
        {
            [SerializeField] private int _GainAbilityPerLevel;
            [SerializeField] private float _nomalCardRatio;
            [SerializeField] private float _rareCardRatio;
            [SerializeField] private float _epicCardRatio;
            [SerializeField] private float _legendaryCardRatio;

            public int AbilityLevel => _GainAbilityPerLevel;
        }

        [System.Serializable]
        public class UIConfig
        {
            [SerializeField] private float _fadeOutTime;
            [SerializeField] private float _fadeInTime;
            [SerializeField] private float _fadeOutWaitTime;
        }

        [System.Serializable]
        public class MapConfig
        {
            public int _baseWidth;
            public int _baseHeight;
            public int _baseEnemyCount;
            public float _baseWallFill;
        }

        [System.Serializable]
        public class ActConfig
        {
            [SerializeField] private List<int> _bossStages;
            [SerializeField] private int _actPerFloorCount;
            [SerializeField] private int _totalFloor;
            public int ActPerFloor => _actPerFloorCount;
            public int TotalFloor => _totalFloor;
            public List<int> BossStages => _bossStages;
        }

        public CombatConfig Combat;
        public PlayerConfig Player;
        public EnemyConfig Enemy;
        public AbilityConfig Ability;
        public UIConfig UI;
        public ActConfig Act;
    }
}
