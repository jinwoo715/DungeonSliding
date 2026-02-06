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

        public void ChangePlayerStat(EPlayerStatType changedStat)
        {
            switch (changedStat)
            {
                case EPlayerStatType.HP:

                    _viewer.UpdateHP(_statReadOnly.Get(EPlayerStatType.HP), _statReadOnly.Get(EPlayerStatType.MaxHp));

                    break;
                case EPlayerStatType.MaxHp:
                    _viewer.UpdateHP(_statReadOnly.Get(EPlayerStatType.HP), _statReadOnly.Get(EPlayerStatType.MaxHp));

                    break;
                case EPlayerStatType.Damage:
                    _viewer.UpdateDamage(_statReadOnly.Get(EPlayerStatType.Damage));

                    break;
                case EPlayerStatType.MoveCount:
                    _viewer.UpdateMoveCount(_statReadOnly.Get(EPlayerStatType.MoveCount), _statReadOnly.Get(EPlayerStatType.MaxMoveCount));

                    break;
                case EPlayerStatType.MaxMoveCount:
                    _viewer.UpdateMoveCount(_statReadOnly.Get(EPlayerStatType.MoveCount), _statReadOnly.Get(EPlayerStatType.MaxMoveCount));

                    break;
            }
        }
    }
}
