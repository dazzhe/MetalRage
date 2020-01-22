using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField]
    private GameConfig config = default;

    public static SoundSystem SoundSystem { get; private set; }
    public static GameConfig Config { get; private set; }

    private void Awake() {
        SoundSystem = new SoundSystem();
        Config = this.config;
    }
}
