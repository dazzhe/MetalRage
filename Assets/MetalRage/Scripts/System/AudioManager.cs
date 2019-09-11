using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : SingletonBehaviour<AudioManager> {
    [SerializeField]
    private AudioClip[] SEs = default;

    private new AudioSource audio;

    private void Start() {
        this.audio = GetComponent<AudioSource>();
    }

    public void PlaySE(int clipIndex) {
        this.audio.PlayOneShot(this.SEs[clipIndex]);
    }
}
