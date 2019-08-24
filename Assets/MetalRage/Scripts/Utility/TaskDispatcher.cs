using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskDispatcher : MonoBehaviour {
    private readonly Queue<Action> actions = new Queue<Action>();

    private void Awake() {
        this.hideFlags = HideFlags.HideInHierarchy;
    }

    private void Update() {
        while (this.actions.Count > 0) {
            this.actions.Dequeue().Invoke();
        }
    }

    public void Invoke(Action action) {
        lock (this.actions) {
            this.actions.Enqueue(action);
        }
    }

    public void Invoke(Func<IEnumerator> action) {
        Invoke(() => StartCoroutine(action()));
    }
}
