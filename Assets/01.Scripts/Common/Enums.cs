
namespace JW.SlidingPuzzle
{
    public enum EGameFlowType
    {
        None = 0,
        Battle = 1 << 0,
        Ability = 1 << 1,
    }

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
        Battle
    }

    public enum ECretureStatType
    {
        HP,
        Damage
    }


    //Ability
    public enum EAbilityTriggerType
    {
        Instant,
        EnterRoom,
        Attack,
        Hitted, 
        Kill,
        MoveStart,
        MoveEnd,
        LevelUp,
        BattleStart,
        BattleEnd,
        OnStepEffectTile
    }
   
    public enum EDurationType
    {
        None,
        TurnCount,
        AttackCount,
        MoveCount,
        HitTakenCount
    }

    public enum EAbilityEffectKind
    {
        Stat,
        Rule,
        Reward
    }

    public enum EPlayerStat
    {
        HP,
        Damage,
        MoveCount
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