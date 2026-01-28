namespace JW.DungeonSliding.GamePlay.Ability
{
    public interface IAbility
    {
        public IAbilityEntity Entity { get; }

        public void ExcuteAbility();
        public void ProcTrigger(EGameTriggerType triggerType);
    }

}
