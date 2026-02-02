using JW.DungeonSliding.GamePlay.Stats;
using UnityEngine;

namespace JW.DungeonSliding.UI
{
    public class PlayerStatUIContoller : MonoBehaviour
    {
        [SerializeField] private PlayerStatViewer _viewer;

        private IPlayerStatProvider _statReadOnly;
        
        public void Init(IPlayerStatProvider statReadOnly)
        {
            Bind(statReadOnly);
        }
        private void Bind(IPlayerStatProvider statReadOnly)
        {
            UnBind();

            _statReadOnly = statReadOnly;
            _statReadOnly.OnStatChanged += ChangePlayerStat;
        }
        private void UnBind()
        {
            if(_statReadOnly != null)
            {
                _statReadOnly.OnStatChanged -= ChangePlayerStat;
                _statReadOnly = null;
            }
        }

        public void ChangePlayerStat(EPlayerStat changedStat)
        {
            switch (changedStat)
            {
                case EPlayerStat.HP:

                    _viewer.UpdateHP(_statReadOnly.Get(EPlayerStat.HP), _statReadOnly.Get(EPlayerStat.MaxHp));

                    break;
                case EPlayerStat.MaxHp:
                    _viewer.UpdateHP(_statReadOnly.Get(EPlayerStat.HP), _statReadOnly.Get(EPlayerStat.MaxHp));

                    break;
                case EPlayerStat.Damage:
                    _viewer.UpdateDamage(_statReadOnly.Get(EPlayerStat.Damage));

                    break;
                case EPlayerStat.MoveCount:
                    _viewer.UpdateMoveCount(_statReadOnly.Get(EPlayerStat.MoveCount), _statReadOnly.Get(EPlayerStat.MaxMoveCount));

                    break;
                case EPlayerStat.MaxMoveCount:
                    _viewer.UpdateMoveCount(_statReadOnly.Get(EPlayerStat.MoveCount), _statReadOnly.Get(EPlayerStat.MaxMoveCount));

                    break;
            }
        }
    }
}
