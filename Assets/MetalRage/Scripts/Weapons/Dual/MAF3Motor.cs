using System.Collections;
using UnityEngine;

public class MAF3Motor : MonoBehaviour {
    private Animator _anim;
    private Collider co;

    private void Awake() {
        GetComponent<Hit>().defence = 0.2f;
        this._anim = GetComponent<Animator>();
        this.co = GetComponent<Collider>();
    }

    private void MakeShot() {
        //audio.Play();
        StartCoroutine(EmitFire());
    }

    private void ShieldClose() {
        this._anim.SetBool("isOpen", false);
        this.co.enabled = true;
        this.co.isTrigger = true;
    }

    private void ShieldOpen() {
        this._anim.SetBool("isOpen", true);
        this.co.enabled = false;
        this.co.isTrigger = false;
    }

    private IEnumerator EmitFire() {
        GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(0.1f);
        GetComponent<ParticleSystem>().Stop();
    }
}
