using DG.Tweening;
using JW.DungeonSliding.Core;
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

        public override void Initialize(ECreatureType cretureType)
        {
            base.Initialize(cretureType);
            Rotate.OnRotateEnd += EndHittedAnimation;
        }
        public virtual void ExcuteAttack()
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
            GameManager.Sound.PlayEffectSound(EEffectSoundType.CollapseStatue);
            StopAllCoroutines();
        }

        private float _rotateDelay = 0.3f;
        public override bool TakeDamage(DamageContext damageInfo)
        {
            if (IsActive == false) return false;

            base.TakeDamage(damageInfo);

            if (damageInfo.AppliedFinalDamage == 0) return false;

            Vector3 punchScale = new Vector3(0.02f, 0f, 0.02f);
            _avatar.transform.DOPunchPosition(punchScale, _rotateDelay, 20);

            EDirectionType toDir = DirectionUtility.GetDirFromTileToTile(TileObject.TilePosition, damageInfo.Attacker.TileObject.TilePosition);

            var particle = ParticlePool.Instance.GetParticle("HitDust");
            particle.SetParticle(this.transform.position + Vector3.up * 0.65f + GetHitParticlePosition(toDir), 1.0f);

            GameManager.Sound.PlayEffectSound(EEffectSoundType.HitStatue);

            if (IsActive == false) return false;

            float targetRotation = GetEulerYByDirection(toDir);
            particle.transform.rotation = Quaternion.Euler(-20, targetRotation, 0);

            if (IsCanRotate() && toDir != Rotate.Direction)
            {
                StartCoroutine(CoDelayRotate(toDir));
            }
            else EndHittedAnimation();

            return true;
        }

        private IEnumerator CoDelayRotate(EDirectionType toDir)
        {
            yield return new WaitForSeconds(_rotateDelay);

           

            StartCoroutine(Rotate.CoRotateToDirection(toDir));
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
