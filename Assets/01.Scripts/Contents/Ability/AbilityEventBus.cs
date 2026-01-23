using System;

public class AbilityEventBus
{
    public event Action OnEnterRoomEvent;
    public event Action OnEnemyKillEvent;
    public event Action OnEnemyAttackEvent;

    public void ExcuteAbilityEvent()
    {

    }
}
