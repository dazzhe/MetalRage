using UnityEngine;
using System.Collections;
using System.Linq;

[CreateAssetMenu(fileName = "ConfigContainer", menuName = "MetalRage/ConfigContainer")]
public class ConfigContainer : ScriptableObject {
    [SerializeField]
    private ScriptableObject[] configs;

    public T GetConfig<T>() where T : ScriptableObject {
        return this.configs.Where(c => c.GetType() == typeof(T)).FirstOrDefault() as T;
    }
}
