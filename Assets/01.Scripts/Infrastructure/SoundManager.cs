using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace JW.DungeonSliding
{
    public enum EEffectSoundType
    {
        CollapseStatue,
        CriticalStatueHit,
        HitStatue,
        RotateStatue,
        HitPlayer,
        PressButton,
        SelectAbility,
        LevelUp
    }
    public enum EBGMSoundType
    {

    }

    [System.Serializable]
    public class AudioUnit
    {
        public AudioClip Clip;
        public EEffectSoundType SoundType;
        public float Time;
        public float Volume;
    }

    public interface ISound
    {
        void PlayEffectSound(EEffectSoundType effectSound);
    }

    public class SoundManager : MonoBehaviour, ISound
    {
        [SerializeField] private SoundSource _origin;
        private Stack<SoundSource> _audioSources = new Stack<SoundSource>();

        private Dictionary<EEffectSoundType, AudioUnit> _clip = new Dictionary<EEffectSoundType, AudioUnit>();

        public void Init(List<AudioUnit> audioClips)
        {
            foreach (var unit in audioClips)
            {
                _clip.Add(unit.SoundType, unit);
            }
        }

        public void PlayEffectSound(EEffectSoundType effectSound)
        {
            if(_clip.TryGetValue(effectSound, out var unit))
            {
                var ss = GetSoundSource();
                ss.PlayAudio(unit);
            }
        }
        public void PlayBackgroundSound()
        {

        }

        public SoundSource GetSoundSource()
        {
            if (_audioSources.Count > 0)
                return _audioSources.Pop();

            return SpawnSoundSource();
        }
        private SoundSource SpawnSoundSource()
        {
            var ss = Instantiate(_origin, this.transform);
            ss.OnReturnSoundSorce += ReturnSoundSource;
            return ss;
        }
        private void ReturnSoundSource(SoundSource ss)
        {
            _audioSources.Push(ss);
        }
    }
}
