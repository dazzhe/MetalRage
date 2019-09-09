using System.Collections;
using UnityEngine;

public class RadioChatIcon : MonoBehaviour {
    [SerializeField]
    private float lifeTime = 5f;

    private Animator animator;

    private void Awake() {
        this.animator = GetComponent<Animator>();
    }

    private IEnumerator Start() {
        this.animator.SetTrigger("Show");
        yield return new WaitForSeconds(this.lifeTime);
        this.animator.SetTrigger("Hide");
    }
}
