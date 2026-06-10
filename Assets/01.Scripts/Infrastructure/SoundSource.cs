using System;
using System.Collections;
using UnityEngine;

namespace JW.DungeonSliding
{
    public class SoundSource : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        public event Action<SoundSource> OnReturnSoundSorce;
        public void PlayAudio(AudioUnit audioUnit)
        {
            _source.clip = audioUnit.Clip;
            _source.volume = audioUnit.Volume;
            _source.Play();
            StartCoroutine(CoPlayTimer(audioUnit.Time));
        }
        private IEnumerator CoPlayTimer(float timer)
        {
            yield return new WaitForSeconds(timer);
            StopAudio();
        }

        public void StopAudio()
        {
            _source.Stop();
            _source.clip = null;
            _source.volume = 1;

            OnReturnSoundSorce?.Invoke(this);
        }
    }
}
