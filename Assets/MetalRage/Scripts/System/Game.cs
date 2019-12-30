using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    public static SoundSystem SoundSystem { get; private set; }

    private void Awake() {
        SoundSystem = new SoundSystem();
    }
}
