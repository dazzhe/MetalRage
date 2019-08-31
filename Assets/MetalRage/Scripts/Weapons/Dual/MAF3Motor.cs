using System.Collections;
using UnityEngine;

public class MAF3Motor : MonoBehaviour {
    private Animator _anim;
    private Collider co;

    private void Awake() {
        this._anim = GetComponent<Animator>();
        this.co = GetComponent<Collider>();
    }
}
