using UnityEngine;

namespace JW.DungeonSliding.GamePlay
{
    [CreateAssetMenu(fileName = "CombatConfig", menuName = "Configs/CombatConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("Combat Settings")]
        [SerializeField] private float _backAttackDamageMultiplier;

        [Header("Player Settings")]
        [SerializeField] private float _moveSpeed;

        [Header("UI")]
        [SerializeField] private float _fadeOutTime;
        [SerializeField] private float _fadeInTime;
        [SerializeField] private float _fadeOutWaitTime;

        public float BackAttackDamageMultiplier => _backAttackDamageMultiplier;
        public float MoveSpeed => _moveSpeed;
        public float FadeOutTime => _fadeOutTime;
        public float FadeInTime => _fadeInTime;
        public float FadeOutWaitTime => _fadeOutWaitTime;
    }
}
