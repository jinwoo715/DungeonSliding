using JW.DungeonSliding.GamePlay;
using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class PlayerStatPresenter : MonoBehaviour
    {
        [SerializeField] private PlayerStatViewer _viewer;

        private IStatReadOnly _statReadOnly;
        private IStatModifier _statModifier;
        private ILevelProgress _levelProgress;
        public void Init(IStatReadOnly statReadOnly, IStatModifier statModifier, ILevelProgress levelProgress)
        {
            Bind(statReadOnly, statModifier, levelProgress);
        }
        private void Bind(IStatReadOnly statReadOnly, IStatModifier statModifier, ILevelProgress levelProgress)
        {
            UnBind();

            _statReadOnly = statReadOnly;
            _statModifier = statModifier;
            _levelProgress = levelProgress;

            _levelProgress.OnLevelUp += ChangePlayerLevel;
            _levelProgress.OnChangedXp += ChangePlayerLevelProgress;

            _statModifier.OnStatChanged += ChangePlayerStat;
        }
        private void UnBind()
        {
            if(_statReadOnly != null)
            {
                _statModifier.OnStatChanged -= ChangePlayerStat;
                _statReadOnly = null;
                _statModifier = null;
            }
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
        public void ChangePlayerLevel()
        {
            int level = _levelProgress.CurrentLevel;
            _viewer.UpdateLevelText(level);
        }
        public void ChangePlayerLevelProgress()
        {
            int currentXP = _levelProgress.CurrentXp;
            int requireXp = _levelProgress.RequiredXp;
            _viewer.UpdateLevelProgress(currentXP, requireXp);
        }
    }
}
