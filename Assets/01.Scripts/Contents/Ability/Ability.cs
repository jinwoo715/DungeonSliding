using JW.SlidingPuzzle;
using System;
using UnityEngine;

public class Ability : MonoBehaviour
{
    
}

[CreateAssetMenu(fileName = "Ability", menuName = "Create Ability", order = 0)]
public class AbilityData : ScriptableObject
{
    public string Name;
    public EAbilityTriggerType AbilityTrigger;
    public int TriggerCount;

    public bool IsDisposable;

    public EDurationType DurationType { get; }
    public int DurationCount { get; }

    public EAbilityEffectKind EffectKind;

    public EPlayerStat PlayerStat;
    public int Value;

    public ERuleEffect RuleEffect;
    public ERewardEffect RewardEffect;
}

public interface IAbility
{
    public AbilityData AbilityData { get; }

    public void ExcuteAbility();
    public void TickDuration(EDurationType durationType);
}

public abstract class StatAbility : IAbility
{
    public Action<EPlayerStat, int> ApplyStatAbilityEvent;
    public Action ReleaseAbilityEvent;

    public AbilityData AbilityData { get; private set; }

    public abstract void TickDuration(EDurationType durationType);
    public abstract void ExcuteAbility();

    public abstract void ResetStat();
    public abstract void ReleaseAbility();
}

public class AbilityInstance
{
    public readonly AbilityData Ability;
    public readonly Player Player;

    private int _triggerCounter;
    public bool Consumed { get; private set; }

    public AbilityInstance(AbilityData data)
    {
        Ability = data;
        _triggerCounter = 0;
        Consumed = false;
    }

    public void TriggerAbility()
    {
        if (Consumed) return;

        _triggerCounter++;

        if(_triggerCounter >= Ability.TriggerCount)
        {
            ExcuteAbility();

            if (Ability.IsDisposable)
                Consumed = true;
        }
    }
    public void ExcuteAbility()
    {
        switch (Ability.EffectKind)
        {
            case EAbilityEffectKind.Stat:
                break;
            case EAbilityEffectKind.Rule:
                break;
            case EAbilityEffectKind.Reward:
                break;
        }
    }
}
