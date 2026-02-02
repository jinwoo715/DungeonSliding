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

        public void ReturnPool()
        {
            
        }

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

        public override void TakeDamage(DamageContext damageInfo)
        {
            base.TakeDamage(damageInfo);

            if (IsActive == false)
                return;

            Vector3 punchScale = new Vector3(0.02f, 0f, 0.02f);
            _avatar.transform.DOPunchPosition(punchScale, 0.3f, 20);

            EDirectionType toDir = ToTargetDirection(damageInfo.Attacker.TilePosition);

            var particle = ParticlePool.Instance.GetParticle("HitDust");
            particle.SetParticle(this.transform.position + Vector3.up * 0.65f + GetHitParticlePosition(toDir), 1.0f);

            float targetRotation = GetEulerYByDirection(toDir);
            particle.transform.rotation = Quaternion.Euler(-20, targetRotation, 0);

            StartCoroutine(CoRotationToPlayer(toDir));
        }
        public IEnumerator CoRotationToPlayer(EDirectionType rotationDir)
        {
            yield return new WaitForSeconds(0.3f);

            if (rotationDir != Direction)
            {
                float timer = 0;
                const float roationTime = 1f;

                float startRotationY = this.transform.rotation.eulerAngles.y;
                float targetRotation = GetEulerYByDirection(rotationDir);

                var particle = ParticlePool.Instance.GetParticle("RotationDust");
                particle.SetParticle(this.transform.position + Vector3.up * 0.15f, 2.0f);

                while (timer <= 1)
                {
                    float rotationValue = Mathf.Lerp(startRotationY, targetRotation, timer);
                    this.transform.rotation = Quaternion.Euler(new Vector3(0, rotationValue, 0));

                    timer += Time.deltaTime * roationTime;
                    yield return null;
                }

                SetCharacterRotation(rotationDir);
            }

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
