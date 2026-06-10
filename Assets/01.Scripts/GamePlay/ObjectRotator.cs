using JW.DungeonSliding.Core;
using System;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding.GamePlay.Entities
{
    public class ObjectRotator : MonoBehaviour, IRotateObject
    {
        private Transform _owner;

        public event Action OnRotateEnd;
        public void SetOwner(Transform owner)
        {
            _owner = owner;
        }

        public EDirectionType Direction { get; private set; }
        public float GetEulerYByDirection(EDirectionType direction)
        {
            float rotation = (int)direction * 90;

            return rotation;
        }
        public EDirectionType ReverseDirection(EDirectionType directionType)
        {
            int reverse = (int)directionType + 2;
            reverse = reverse % 4;

            return (EDirectionType)reverse;
        }
        public IEnumerator CoRotateToDirection(EDirectionType directionType)
        {
            var rotateDustParticle = ParticlePool.Instance.GetParticle("RotationDust");
            rotateDustParticle.SetParticle(this.transform.position + Vector3.up * 0.15f, 2.0f);

            GameManager.Sound.PlayEffectSound(EEffectSoundType.RotateStatue);

            if (directionType != Direction)
            {
                float timer = 0;
                const float rotationDuration = 1f; // 1초 동안 회전

                float startRotationY = _owner.rotation.eulerAngles.y;
                float targetRotationY = GetEulerYByDirection(directionType);

                while (timer < 1f)
                {
                    timer += Time.deltaTime / rotationDuration; // duration으로 나눠야 정확히 1초 걸림

                    // LerpAngle을 써야 270도에서 0(360)도로 갈 때 최단 거리로 회전함
                    float rotationValue = Mathf.LerpAngle(startRotationY, targetRotationY, timer);
                    _owner.rotation = Quaternion.Euler(0, rotationValue, 0);

                    yield return null;
                }
            }
            SetRotation(directionType);
            OnRotateEnd?.Invoke();
        }
        public void SetRotation(EDirectionType directionType)
        {
            if (directionType == EDirectionType.None)
                return;

            Direction = directionType;

            float rotation = GetEulerYByDirection(directionType);
            _owner.rotation = Quaternion.Euler(0, rotation, 0);
        }

        public void RotateToDirection(EDirectionType directionType)
        {
            StartCoroutine(CoRotateToDirection(directionType));
        }
    }
}
