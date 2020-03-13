using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Game : MonoBehaviour {
    public static SoundSystem SoundSystem { get; private set; }
    public static Dictionary<MechType, Entity> MechPrefabMap = new Dictionary<MechType, Entity>();

    private void Awake() {
        SoundSystem = new SoundSystem();
    }
}
