using UnityEngine;
using OutOfThisWorld.Audio;

namespace OutOfThisWorld {
    public class SFXTrigger : Triggerable {
        public AudioClip SFX;
        public AudioSource Source;
        public bool PlaySFXOnUndo;

        public override void Trigger() {
            SingletonAudioManager.Get().PlaySFX(Source, SFX);
        }

        public override void Undo() {
            if(PlaySFXOnUndo) {
                SingletonAudioManager.Get().PlaySFX(Source, SFX);
            }
        }
    }
}