using UnityEngine;
namespace JW.DungeonSliding
{
    public class ConstData
    {
        //Player Stat
        public static readonly float MOVE_LERP_SCALE = 4.0f;
        public static readonly int PLAYER_START_HP = 5;
        public static readonly int PLAYER_START_DMG = 5;
        public static readonly int PLAYER_START_MOVECOUNT = 10;

        public static readonly int LEVELUP_XP_OFFSET = 3;

        //Enemy Stat Up Value
        public static readonly float ENEMY_HP_POW = 1.06f;
        public static readonly float ENEMY_DMG_POW = 1.03f;
        public static readonly float ENEMY_XP_POW = 1.015f;
    }

    public class ConstDataKey
    {
        public static readonly string ENEMY_DATA = "EnemyNomalData";
        public static readonly string ENEMY_BOSS_DATA = "EnemyBossData";
        public static readonly string ENEMY_ABILITY_DATA = "EnemyAbilityData";

        public static readonly string RULE_ABILITY_DATA = "RuleAbilityData"; 
        public static readonly string STAT_ABILITY_DATA = "StatAbilityData";
    }

    public class ConstString
    {
        //Animation Trigger
        public static readonly string ONE_HAND_ATTACK_ANIM = "OneHandAttack";
        public static readonly string TWO_HAND_ATTACK_ANIM = "TwoHandAttack";
        public static readonly string HIT_ANIM = "Hitted";

        public static readonly string STOP_ALL_TRIGGER_ANIMATION = "ExitAllTriggerAnimation";

        public static readonly string PLAYER_STATE_KEY = "CharacterState";
    }
}
