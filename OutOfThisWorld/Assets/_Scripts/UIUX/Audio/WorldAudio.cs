using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld.Audio {
    public class WorldAudio : MonoBehaviour {
        public AudioClip BGM;
        private SingletonAudioManager _audioManager;

        private void Start() {
            _audioManager = SingletonAudioManager.Get();
            _audioManager.PlayBackgroundMusic(BGM);
        }
    }
}
