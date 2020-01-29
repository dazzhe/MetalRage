using UnityEngine;
using System.Collections;

public class MechEvents : MonoBehaviour {
    [SerializeField]
    private SoundDefinition walkSound = default;

    public void PlayWalkSound() {
        Game.SoundSystem.Play(this.walkSound, this.transform);
    }
}
