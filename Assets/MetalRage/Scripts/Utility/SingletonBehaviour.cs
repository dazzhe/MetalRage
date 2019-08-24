using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour {
    private static bool enableDdol = true;

    protected static T instance;
    protected static Thread mainThread;

    protected static bool IsQuitting { get; private set; } = false;
    protected static bool EnableDdol {
        get => enableDdol;
        set {
            if (value) {
                DontDestroyOnLoad(instance.gameObject);
            } else {
                SceneManager.MoveGameObjectToScene(instance.gameObject, SceneManager.GetActiveScene());
            }
        }
    }

    public static T Instance {
        get {
            if (Thread.CurrentThread != mainThread && mainThread != null) {
                Debug.LogWarning(
                    $"{typeof(T).ToString()}.Instance called" +
                     " from background threads returns null if it is never called before.");
                return instance;
            }
            if (IsQuitting) {
                return null;
            }
            if (instance == null) {
                instance = FindObjectOfType(typeof(T)) as T;
                if (instance == null) {
                    instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                }
            }
            return instance;
        }
        private set => instance = value;
    }

    protected bool AnotherInstanceExists() {
        return Instance != this && Instance != null;
    }

    protected void Awake() {
        if (AnotherInstanceExists()) {
            DestroyImmediate(this.gameObject);
            return;
        }
        instance = this as T;
        mainThread = Thread.CurrentThread;
        DontDestroyOnLoad(this.gameObject);
        IsQuitting = false;
    }

    protected void OnDestroy() {
        IsQuitting = true;
    }
}
