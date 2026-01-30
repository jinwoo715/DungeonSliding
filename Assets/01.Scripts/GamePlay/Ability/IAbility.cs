namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbility
    {
        public void ExcuteAbility();
        public void ProcTrigger(EGameTriggerType triggerType);
    }
}
