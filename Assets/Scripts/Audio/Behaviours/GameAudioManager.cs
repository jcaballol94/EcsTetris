using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    [RequireComponent(typeof(AudioSource))]
    public class GameAudioManager : Singleton<GameAudioManager>
    {
        public enum EFFECTS
        {
            Hit,
            Move,
            Rotate,
            RemoveLine,
            Place,
            Drop,
            Hold,
            Tetris
        }
        [System.Serializable]
        public struct SoundEffect
        {
            public EFFECTS effect;
            public AudioClip clip;
        }

        private AudioSource m_audioSource;
        [SerializeField] private List<SoundEffect> m_sounds = new List<SoundEffect>();

        protected override void Awake()
        {
            base.Awake();
            m_audioSource = GetComponent<AudioSource>();
        }

        public void PlayEffect(EFFECTS effect)
        {
            var sound = m_sounds.Find(e => e.effect == effect);
            m_audioSource.Stop();
            if (sound.clip)
                m_audioSource.PlayOneShot(sound.clip);
        }
    }
}
