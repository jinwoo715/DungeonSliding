using JW.Utility;
using System.Collections;
using UnityEngine;
namespace JW.SlidingPuzzle
{
    public class ParticlePool : MonoBehaviour
    {
        private static ParticlePool _particlePool;
        public static ParticlePool Instance => _particlePool;

        [SerializeField] private ParticlePoolObject _rotationDustParticle;
        [SerializeField] private ParticlePoolObject _hitDustParticle;
        [SerializeField] private ParticlePoolObject _destroyStatueParticle;

        private DictionaryPool<ParticlePoolObject> _particles = new DictionaryPool<ParticlePoolObject>();

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

        public ParticlePoolObject GetParticle(string key)
        {
            ParticlePoolObject obj = _particles.GetObject(key);
            return obj;
        }
    }
}
