
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
        Stop,       //ÀÏ¹ÝÀûÀÎ ¸ØÃã
        EnemyStop,  //¾Õ¿¡ ÀûÀÌ ÀÖ¾î¼­ ¸ØÃã
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
    public enum EAnimationTriggerType
    {

    }
    public enum ECretureStatType
    {
        HP,
        Damage
    }

    public enum EEditModeType
    {
        Tile,
        Player,
        Enemy,
        Effect
    }
}