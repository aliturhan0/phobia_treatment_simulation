using UnityEngine;
using UnityEditor;

public class SkyboxCreator : EditorWindow
{
    [MenuItem("Ali/Arkaplanı 360 Derece Yap (Fix Skybox)")]
    public static void CreateSkybox()
    {
        string textPath = "Assets/ali_assetler/Art/UI/main_menu_bg.png";
        string matPath = "Assets/ali_assetler/Art/UI/MainMenuSkybox.mat";

        // 1. Load Texture
        Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(textPath);
        if (tex == null)
        {
            Debug.LogError("HATA: Arkaplan resmi bulunamadı! (" + textPath + ")");
            return;
        }

        // 2. Create Material
        Material skyboxMat = new Material(Shader.Find("Skybox/Panoramic"));
        skyboxMat.SetTexture("_MainTex", tex);
        skyboxMat.SetFloat("_Exposure", 1f);
        skyboxMat.SetFloat("_Rotation", 180f); // Rotate to face camera potentially

        // 3. Save Material
        AssetDatabase.CreateAsset(skyboxMat, matPath);
        
        // 4. Assign to Scene
        RenderSettings.skybox = skyboxMat;
        DynamicGI.UpdateEnvironment();

        // 5. Hide flat panel background if it exists
        GameObject mainPanel = GameObject.Find("MainPanel");
        if (mainPanel)
        {
            UnityEngine.UI.Image img = mainPanel.GetComponent<UnityEngine.UI.Image>();
            if (img && img.sprite != null && img.sprite.name == "main_menu_bg")
            {
                // Disable the image component so we see the skybox instead
                // But keep the panel object for buttons
                img.enabled = false; 
                Debug.Log("Düz panel resmi gizlendi, artık Skybox görünüyor.");
            }
        }

        Debug.Log("BAŞARILI: 360 Derece Skybox ayarlandı!");
    }
}
