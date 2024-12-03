using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld.Monster {
    public class MonsterTarget : MonoBehaviour {
        public float Value = 1f;

        void Update() {
            if (Value <= 0) {
                Destroy(this);
            }
        }
    }
}