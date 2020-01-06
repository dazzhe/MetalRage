using Unity.Entities;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField]
    private ConfigContainer config = default;

    public static SoundSystem SoundSystem { get; private set; }
    public static ConfigContainer Config { get; private set; }

    private void Awake() {
        SoundSystem = new SoundSystem();
        Config = this.config;
        World.Active.CreateManager<GameLoopSystem>();
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(World.Active);
    }
}
