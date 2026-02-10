using DG.Tweening;
using JW.DungeonSliding.GamePlay.Combat;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class StoneStatueEnemy : Enemy
    {
        [SerializeField] protected Transform _avatar;
        [SerializeField] private GameObject _eyeLight;

        public override void StartAttackAnimation()
        {
            _eyeLight.SetActive(true);
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

        public override bool TakeDamage(DamageContext damageInfo)
        {
            base.TakeDamage(damageInfo);

            if (IsActive == false)
                return false;

            Vector3 punchScale = new Vector3(0.02f, 0f, 0.02f);
            _avatar.transform.DOPunchPosition(punchScale, 0.3f, 20);

            EDirectionType toDir = DirectionToTile(damageInfo.Attacker.TilePosition);

            var particle = ParticlePool.Instance.GetParticle("HitDust");
            particle.SetParticle(this.transform.position + Vector3.up * 0.65f + GetHitParticlePosition(toDir), 1.0f);

            float targetRotation = GetEulerYByDirection(toDir);
            particle.transform.rotation = Quaternion.Euler(-20, targetRotation, 0);

            if(!HasStatus(ECreatureStatus.Bind) && !HasStatus(ECreatureStatus.Stun))
                StartCoroutine(CoRotationToPlayer(toDir));
            else EndHittedAnimation();

            return true;
        }
        public IEnumerator CoRotationToPlayer(EDirectionType rotationDir)
        {
            yield return new WaitForSeconds(0.3f);

            var particle = ParticlePool.Instance.GetParticle("RotationDust");
            particle.SetParticle(this.transform.position + Vector3.up * 0.15f, 2.0f);

            yield return CoRotateCharacter(rotationDir);

            EndHittedAnimation();
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
