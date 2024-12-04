using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class AnimationTrigger : Triggerable {
        public Animator Animator;
        public string Animation;
        public string UndoAnimation;

        public override void Trigger() {
            Animator.Play(Animation);
        }

        public override void Undo() {
            Animator.Play(UndoAnimation);
        }
    }
}