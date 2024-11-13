using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    [RequireComponent(typeof(Animator))]
    public class AnimationTrigger : Triggerable {
        public Animator Animator;
        public string Animation;

        public override void Trigger() {
           Animator.Play(Animation);
        }
    }
}