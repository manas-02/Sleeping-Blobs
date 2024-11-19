using UnityEngine;

public class HideBlobsOnStart : MonoBehaviour
{
    void Start()
    {
        // This script should be attached to the prefab instances in the scene.
        gameObject.SetActive(false);
    }
}
