
namespace JW.DungeonSliding
{
    public enum EDirectionType
    {
        Left,
        Up,
        Right,
        Down,
        None,
    }

    public enum ESlideResultType
    {
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
        Stun,
        KnockBack
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

    public enum ECretureStatType
    {
        HP,
        Damage
    }


    //Ability

    public enum EAbilityRank
    {
        Nomal,
        Rare,
        Legend
    }
    public enum EGameTriggerType
    {
        None,

        EnterRoom,

        BackAttack,
        Attack,
        Hitted, 
        Kill,

        MoveStart,
        MoveEnd,

        LevelUp,

        BattleStart,
        BattleEnd,

        OnStepEffectTile,

        OnDeathByHP,
        OnDeathByMoveCount,

        FadeOutStart,
        FadeOutFin,

        FadeInStart,
        FadeInFin,

        ClearStage,
    }

    public enum EAbilityEffectKind
    {
        Stat,
        Rule,
    }

    public enum ERuleAbilityType
    {
        // --- 생존/방어 ---
        Revive,              // 죽으면 1회 부활
        RoomBarrier,         // 방 입장 시 베리어 획득

        // --- 이동/기믹 ---
        WallBound,            // 벽 앞에서 반대쪽 이동

        // --- 공격/강화 ---
        AroundEnemyBonus,    // 주변 적 비례 추가 데미지
        DoubleAttack,        // 확률적 2번 공격
        CounterAttack,       // 피격 시 확률 반격
        DistanceDamageBonus, // 이동한 타일 수만큼 공격 강화

        // --- 제어/위험 ---
        EnemyBind,           // 적 움직임 1턴 속박
        Berserker,           // 피해 2배 / 피격 2배

        // --- 자원 역전 (부활과 구분 필요) ---
        LastResortMove,      // Move 0일 때 HP 소모 회복
        LastResortHP         // HP 0일 때 Move 소모 회복
    }

    public enum EPlayerStat
    {
        None,
        HP,
        MaxHp,
        Damage,
        MoveCount,
        MaxMoveCount,
        NextAttackDamage,
        RerollCount,
    }

    public enum EApplyStatType
    {
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

    public enum ERewardEffect
    {
        None,
        RandomAbility,
        HPBarrier,
        MoveCountBarrier,
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