using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutOfThisWorld {
    public class Audio : MonoBehaviour {
        public AudioManager audioManager;

        private void Awake()
        {
            audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        }

        private void Start()
        {
            audioManager.PlaySFX(audioManager.background);
        }
    }
}
