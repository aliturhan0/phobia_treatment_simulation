using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadOnTrigger : MonoBehaviour
{
    [Header("Which scene to load (must be in Build Settings)")]
    public string sceneName;

    [Header("Optional: prevent multiple triggers")]
    public bool loadOnlyOnce = true;

    private bool _loaded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (loadOnlyOnce && _loaded) return;

        // Player tag check
        if (!other.CompareTag("Player")) return;

        _loaded = true;

        // Load the target scene
        SceneManager.LoadScene(sceneName);
    }
}
