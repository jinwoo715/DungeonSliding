using JW.Utility;
using System.Collections;
using UnityEngine;
namespace JW.DungeonSliding
{
    public class ParticlePool : MonoBehaviour
    {
        private static ParticlePool _particlePool;
        public static ParticlePool Instance => _particlePool;

        [SerializeField] private ParticlePooledObject _rotationDustParticle;
        [SerializeField] private ParticlePooledObject _hitDustParticle;
        [SerializeField] private ParticlePooledObject _destroyStatueParticle;

        private DictionaryPool<ParticlePooledObject> _particles = new DictionaryPool<ParticlePooledObject>();

        private void Awake()
        {
            _particlePool = this;

            Init();
        }

        private void Init()
        {
            _particles.CreatePool("RotationDust", _rotationDustParticle, this.transform, 1);
            _particles.CreatePool("HitDust", _hitDustParticle, this.transform, 1);
            _particles.CreatePool("DestroyDust", _destroyStatueParticle, this.transform, 1);
        }

        public ParticlePooledObject GetParticle(string key)
        {
            ParticlePooledObject obj = _particles.GetObject(key);
            return obj;
        }
    }
}
