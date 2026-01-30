using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Sahne yüklendiğinde lighting'i düzeltir.
/// Bu scripti sahneye boş bir GameObject'e ekle.
/// </summary>
public class LightingFixer : MonoBehaviour
{
    void Start()
    {
        // Lighting'i yenile
        DynamicGI.UpdateEnvironment();
        
        // Skybox'ı yenile
        RenderSettings.skybox = RenderSettings.skybox;
        
        Debug.Log("Lighting düzeltildi!");
    }
}
