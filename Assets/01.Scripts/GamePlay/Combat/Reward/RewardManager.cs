using JW.DungeonSliding.GamePlay.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    public class RewardManager
    {
        private ICombatEventPresenter _combatEventPresenter;

        public void Init(ICombatEventPresenter combatEventPresenter)
        {
            _combatEventPresenter = combatEventPresenter;
            _combatEventPresenter.OnDeathEvent += RequestReward;
        }

        public void RequestReward(DeathEvent deathEvent)
        {
            Debug.Log(deathEvent.Killer);
            Debug.Log(deathEvent.Victim);

            if (TryGetSender(deathEvent.Victim, out var sender) && TryGetReceiver(deathEvent.Killer, out var receiver))
            {
                receiver.AddReward(sender.CreateReward());
            }
        }

        private bool TryGetSender(ICombatant combatant, out IRewardSender rewardSender)
        {
            Debug.Log(combatant is IRewardSender s);

            if (combatant is IRewardSender sender)
            {
                rewardSender = sender;
                return true;
            }
            else
            {
                rewardSender = null;
                return false;
            }
        }
        private bool TryGetReceiver(ICombatant combatant, out IRewardReceiver rewardReceiver)
        {
            Debug.Log(combatant is IRewardReceiver r);

            if (combatant is IRewardReceiver receiver)
            {
                rewardReceiver = receiver;
                return true;
            }
            else
            {
                rewardReceiver = null;
                return false;
            }
        }
    }
}
