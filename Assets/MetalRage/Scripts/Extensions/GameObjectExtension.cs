using UnityEngine;

public static class GameObjectExtension {
    public static void SetLayerRecursively(this GameObject self, int layer) {
        self.layer = layer;
        foreach (Transform n in self.transform) {
            SetLayerRecursively(n.gameObject, layer);
        }
    }
}