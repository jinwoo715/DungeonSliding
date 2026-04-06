using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.GamePlay.Entities;
using JW.DungeonSliding.GamePlay.Stats;
using System;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class PlayerStatPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerStatViewer _viewer;

        private IStatReadOnly _statReadOnly;

        private Action OnUnBind;

        public void Init(PlayerInfo playerInfo)
        {
            Bind(playerInfo.PlayerStatReader, playerInfo.PlayerStatModifier, playerInfo.Level, playerInfo.NextAttackEnhancer);
        }
        private void Bind(IStatReadOnly statReadOnly, IStatModifier statModifier, ILevelProgress levelProgress, INextAttackEnhancer nextAttackEnhancer)
        {
            UnBind();

            _statReadOnly = statReadOnly;

            levelProgress.OnLevelUp += ChangePlayerLevel;
            levelProgress.OnChangedXp += ChangePlayerLevelProgress;

            statModifier.OnStatChanged += ChangePlayerStat;

            nextAttackEnhancer.OnChangedNextAttackCount += UpdateExraAttackCount;
            nextAttackEnhancer.OnChangedNextAttackDamage += UpdateAddDamage;

            OnUnBind += () =>
            {
                levelProgress.OnLevelUp -= ChangePlayerLevel;
                levelProgress.OnChangedXp -= ChangePlayerLevelProgress;

                statModifier.OnStatChanged -= ChangePlayerStat;

                nextAttackEnhancer.OnChangedNextAttackCount -= UpdateExraAttackCount;
                nextAttackEnhancer.OnChangedNextAttackDamage -= UpdateAddDamage;
            };
        }
        private void UnBind()
        {
            OnUnBind?.Invoke();
        }

        public void ChangePlayerStat(ECreatureStatType changedStat)
        {
            switch (changedStat)
            {
                case ECreatureStatType.CurrentHP:
                case ECreatureStatType.MaxHp:
                    int currentHP = _statReadOnly.Get(ECreatureStatType.CurrentHP);
                    int maxHP = _statReadOnly.Get(ECreatureStatType.MaxHp);
                    _viewer.UpdateHP(currentHP, maxHP);
                    break;
                case ECreatureStatType.Damage:
                    int damage = _statReadOnly.Get(ECreatureStatType.Damage);
                    _viewer.UpdateDamage(damage);
                    break;
                case ECreatureStatType.CurrentMoveCount:
                case ECreatureStatType.MaxMoveCount:
                    int currentMove = _statReadOnly.Get(ECreatureStatType.CurrentMoveCount);
                    int maxMove = _statReadOnly.Get(ECreatureStatType.MaxMoveCount);
                    _viewer.UpdateMoveCount(currentMove, maxMove);
                    break;
            }
        }
        public void ChangePlayerLevel(int level)
        {
            _viewer.UpdateLevelText(level);
        }
        public void ChangePlayerLevelProgress(int currentXp, int requireXp)
        {
            _viewer.UpdateLevelProgress(currentXp, requireXp);
        }
        public void UpdateAddDamage(int damage)
        {
            _viewer.UpdateNextAttackExtraDamage(damage);
        }
        public void UpdateExraAttackCount(int count)
        {
            _viewer.UpdateAttackCount(count);
        }
    }
}
