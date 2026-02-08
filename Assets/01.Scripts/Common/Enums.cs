
namespace JW.DungeonSliding
{
    public enum EDirectionType
    {
        Up,
        Right,
        Down,
        Left,
        None,
    }

    public enum ESlideResultType
    {
        None,
        Move,
        Stop,       //일반적인 멈춤
        EnemyStop,  //앞에 적이 있어서 멈춤
        Teleport
    }

    public enum ETileEnterType
    {
        None,
        Slide,
        Teleport
    }

    public enum EStatusEffectType
    {
        None,
        KnockBack,
        Blind
    }

    public enum EEffectObjectType
    {
        Stop,
        TurnRightUp,
        TurnRightDown,
        Teleport
    }

    public enum ETileType
    {
        Route,
        Wall,
        Empty,
    }
    public enum ECharacterStateType
    {
        Idle,
        Run,
    }
    public enum ECretureType
    {
        Player,
        Enemy
    }

    public enum EEnemyStatType
    {
        None,
        HP,
        Damage
    }


    //Ability

    public enum EAbilityRank
    {
        Nomal,
        Rare,
        Epic,
        Legend
    }
    public enum EGameTriggerType
    {
        None                = 0,

        Instant             = 1 << 0,

        OnEnterRoom         = 1 << 1,

        OnBackAttack        = 1 << 2,
        OnAttack            = 1 << 3,
        OnDamaged           = 1 << 4, 
        OnKillEnemy         = 1 << 5,

        OnSlideStart        = 1 << 6,
        OnSlideEnd          = 1 << 7,

        OnSlideBlocked      = 1 << 8,

        OnShowAbility       = 1 << 9,
        OnHideAbility       = 1 << 10,

        OnMoveEnd           = 1 << 11,

        OnLevelUp           = 1 << 12,

        OnBattleStart       = 1 << 13,
        OnBattleEnd         = 1 << 14,

        OnStepEffectTile    = 1 << 15,

        OnDeathByHP         = 1 << 16,
        OnDeathByMoveCount  = 1 << 17,
        OnDeath             = 1 << 18,

        OnClearStage        = 1 << 19,

        OnTurnEnd           = 1 << 20,
    }

    public enum EAbilityEffectKind
    {
        Stat,
        Rule,
    }

  
    public enum EAbilityApplyStatType
    {
        None,
        PlayerStat,
        NextAttack
    }

    public enum ERuleAbilityType
    {
        // --- 생존/방어 ---
        Revive,              // 죽으면 1회 부활
        Barrier,         // 방 입장 시 베리어 획득

        // --- 이동/기믹 ---
        WallBounce,            // 벽 앞에서 벽 쪽으로 이동 시 반대쪽 1칸 이동

        // --- 공격/강화 ---
        SurroundEnemy,    // 주변 적 비례 추가 데미지
        DoubleAttack,        // 확률적 2번 공격
        CounterAttack,       // 피격 시 확률 반격
        DistanceDamageBonus, // 이동한 타일 수만큼 공격 강화

        // --- 제어/위험 ---
        EnemyBind,           // 적 움직임 1턴 속박
        Berserker,           // 피해 2배 / 피격 2배

        // --- 자원 역전 (부활과 구분 필요) ---
        ConvertHPToMoveCount,      // Move 0일 때 HP 소모 Move회복
        ConvertMoveCountToHp,         // HP 0일 때 Move 소모 HP 회복

        RerollPlus          //리롤 횟수 1회
    }

    public enum EPlayerStatType
    {
        None,
        CurrentHP,
        MaxHp,
        Damage,
        CurrentMoveCount,
        MaxMoveCount,
    }

    public enum EApplyStatType
    {
        None,
        Add,
        Multiple,
        Ratio
    }

    public enum ERuleEffect
    {
        None,
        ExtraAttack,    //추가 공격
        CounterAttack,  //공격 받으면 공격하기
        WallKnockBack,  //벽 방향으로 이동하면 반대쪽으로 한 칸 밀리기
    }

    //Map Editor
    public enum EEditModeType
    {
        Tile,
        Player,
        Enemy,
        Effect
    }
}