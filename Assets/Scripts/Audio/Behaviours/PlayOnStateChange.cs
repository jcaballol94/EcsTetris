using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    public class PlayOnStateChange : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip clip;

        private void OnEnable()
        {
            source.PlayOneShot(clip);
        }

        private void OnDisable()
        {
            source.PlayOneShot(clip);
        }
    }
}
