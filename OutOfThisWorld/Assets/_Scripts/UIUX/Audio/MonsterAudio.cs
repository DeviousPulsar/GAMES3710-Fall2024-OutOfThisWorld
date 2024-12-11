using UnityEngine;
using OutOfThisWorld.Monster;

namespace OutOfThisWorld.Audio {
    public class MonsterAudio : MonoBehaviour {
        public MonsterAI Monster;
        public AudioSource Source;
        public AudioClip Enable;
        public AudioClip Persue;
        public AudioClip Rage;

        private SingletonAudioManager _audioManager;
        private MonsterState _lastState;

        void Start() {
            _audioManager = SingletonAudioManager.Get();
        }

        void FixedUpdate() {
            if (Monster.CurrentState != _lastState) {
                if (_lastState == MonsterState.Disabled) {
                    _audioManager.PlaySFX(Source, Enable);
                } else if (Monster.CurrentState == MonsterState.Rage) {
                    _audioManager.PlaySFX(Source, Rage);
                } else if (Monster.CurrentState == MonsterState.Persue) {
                    _audioManager.PlaySFX(Source, Persue);
                } 
            }

            _lastState = Monster.CurrentState;
        }
    }
}
