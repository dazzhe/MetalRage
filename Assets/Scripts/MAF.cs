using UnityEngine;
using System.Collections;

public class MAF : MonoBehaviour {
    private PhotonView myPV;
	public bool isOpen;
    Animator _anim;
	private Collider co;
	WeaponControl weaponctrl;
	public GameObject unit;
	
	// Use this for initialization
	void Awake () {
        myPV = PhotonView.Get(this);
		weaponctrl = unit.GetComponent<WeaponControl>();
        _anim = this.GetComponent<Animator>();
		co = this.GetComponent<Collider>();
		isOpen = true;
		GetComponent<Hit>().defence = 0.2f;
	}
	
	// Update is called once per frame
	void Update () {
        if (myPV.isMine){
            if (Input.GetButtonDown("Fire2"))
                myPV.RPC("ShieldAnimation",PhotonTargets.AllBuffered);
        }
	}
	
	[RPC]
	void MakeShot(){
		//audio.Play();
		StartCoroutine (this.EmitFire());
	}

    [RPC]
    void ShieldAnimation(){
        if (isOpen){
            isOpen = false;
			weaponctrl.canShot = false;
			_anim.SetBool("isOpen",false);
			co.enabled = true;
			co.isTrigger = true;
        }
        else {
            isOpen = true;
			weaponctrl.canShot = true;
            _anim.SetBool("isOpen",true);
			co.enabled = false;
			co.isTrigger = false;
		}
    }

	IEnumerator EmitFire(){
		particleSystem.Play(); 
		yield return new WaitForSeconds(0.1f);
		particleSystem.Stop();
	}
}
