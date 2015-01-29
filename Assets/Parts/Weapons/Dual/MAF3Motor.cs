using UnityEngine;
using System.Collections;

public class MAF3Motor : MonoBehaviour {
	Animator _anim;
	private Collider co;
	void Awake () {
		GetComponent<Hit>().defence = 0.2f;
		_anim = this.GetComponent<Animator>();
		co = this.GetComponent<Collider>();
	}

	void MakeShot(){
		//audio.Play();
		StartCoroutine (this.EmitFire());
	}

	void ShieldClose(){
		_anim.SetBool("isOpen",false);
		co.enabled = true;
		co.isTrigger = true;
	}

	void ShieldOpen(){
		_anim.SetBool("isOpen",true);
		co.enabled = false;
		co.isTrigger = false;
	}
	
	IEnumerator EmitFire(){
		particleSystem.Play(); 
		yield return new WaitForSeconds(0.1f);
		particleSystem.Stop();
	}
}
