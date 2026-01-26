using JW.Utility;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class ParticlePooledObject : PoolObject
    {
        [SerializeField] private ParticleSystem _particle;

        public override void OnDespawn()
        {
            _particle.Stop();
        }

        public override void OnSpawn()
        {
            _particle.Play();
        }

        public void SetParticle(Vector3 position, float time)
        {
            this.transform.position = position;
            StartCoroutine(CoTimeOut(time));
        }

        private IEnumerator CoTimeOut(float time)
        {
            yield return new WaitForSeconds(time);
            Release();
        }
    }
}
