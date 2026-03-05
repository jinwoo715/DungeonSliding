using DG.Tweening;
using JW.DungeonSliding.GamePlay.Combat;
using JW.DungeonSliding.Map;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class StoneStatueEnemy : Enemy
    {
        [SerializeField] protected Transform _avatar;
        [SerializeField] private GameObject _eyeLight;

        public void ExcuteAttack()
        {
            _eyeLight.SetActive(true);
            _animatorController.SetAnimationTrigger(ConstString.ONE_HAND_ATTACK_ANIM);
        }
        public override void EndAttackAnimation()
        {
            base.EndAttackAnimation();
            _eyeLight.SetActive(false);
        }
        public override void OnDeath()
        {
            base.OnDeath();
            StopAllCoroutines();
        }

        public override void TakeDamage(DamageContext damageInfo)
        {
            if (IsActive == false)
                return;

            base.TakeDamage(damageInfo);

            Vector3 punchScale = new Vector3(0.02f, 0f, 0.02f);
            _avatar.transform.DOPunchPosition(punchScale, 0.3f, 20);

            EDirectionType toDir = GridUtility.GetDirFromTileToTile(Tile.TilePosition, damageInfo.Attacker.Tile.TilePosition);

            var particle = ParticlePool.Instance.GetParticle("HitDust");
            particle.SetParticle(this.transform.position + Vector3.up * 0.65f + GetHitParticlePosition(toDir), 1.0f);

            float targetRotation = GetEulerYByDirection(toDir);
            particle.transform.rotation = Quaternion.Euler(-20, targetRotation, 0);

            if (IsCanRotate() && toDir != Rotate.Direction)
            {
                var rotateDustParticle = ParticlePool.Instance.GetParticle("RotationDust");
                rotateDustParticle.SetParticle(this.transform.position + Vector3.up * 0.15f, 2.0f);

                Rotate.OnRotateEnd += EndHittedAnimation;

                StartCoroutine(Rotate.CoRotateToDirection(toDir));
            }
            else EndHittedAnimation();
        }
        public float GetEulerYByDirection(EDirectionType direction)
        {
            float rotation = (int)direction * 90;
            return rotation;
        }
        public Vector3 GetHitParticlePosition(EDirectionType direction)
        {
            Vector3 particlePosition = Vector3.zero;

            switch (direction)
            {
                case EDirectionType.Left:
                    particlePosition.x = -0.1f;
                    break;
                case EDirectionType.Up:
                    particlePosition.z = 0.1f;
                    break;
                case EDirectionType.Right:
                    particlePosition.x = 0.1f;
                    break;
                case EDirectionType.Down:
                    particlePosition.z = -0.1f;
                    break;
            }

            return particlePosition;
        }

    }
}
