using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Events;

public class MainMenuBuilder : EditorWindow
{
    [MenuItem("Ali/Ana Menüyü Otomatik Kur (Create Main Menu)")]
    public static void CreateMainMenu()
    {
        // 1. Setup Canvas (VR optimized)
        GameObject canvasObj = new GameObject("VR_MainMenu_Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        RectTransform canvasRT = canvasObj.GetComponent<RectTransform>();
        canvasRT.position = new Vector3(0, 1.5f, 3f); // 3 meters in front, eye level
        canvasRT.sizeDelta = new Vector2(1920, 1080);
        canvasRT.localScale = Vector3.one * 0.002f;   // Scale down for VR

        // 2. Load Assets (and force Sprite settings)
        Sprite bgSprite = LoadSpriteAuto("Assets/ali_assetler/Art/UI/main_menu_bg.png");
        Sprite iconHeight = LoadSpriteAuto("Assets/ali_assetler/Art/UI/icon_height.png");
        Sprite iconSpider = LoadSpriteAuto("Assets/ali_assetler/Art/UI/icon_spider.png");
        Sprite iconNatureCity = LoadSpriteAuto("Assets/ali_assetler/Art/UI/icon_nature_city.png");

        // 3. Setup Manager
        GameObject managerObj = new GameObject("MenuManager");
        MainMenuManager manager = managerObj.AddComponent<MainMenuManager>();

        // 4. Create Main Panel
        GameObject mainPanel = CreatePanel(canvasObj.transform, "MainPanel", bgSprite);
        
        // Header Text
        GameObject headerObj = new GameObject("Header_Text");
        headerObj.transform.SetParent(mainPanel.transform, false);
        Text headerText = headerObj.AddComponent<Text>();
        headerText.text = "SANAL GERÇEKLİK TERAPİSİ";
        headerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        headerText.fontSize = 80;
        headerText.alignment = TextAnchor.UpperCenter;
        headerText.color = Color.white;
        headerObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
        headerObj.GetComponent<RectTransform>().sizeDelta = new Vector2(1000, 200);

        // Buttons (Main)
        GameObject btnHeight = CreateButton(mainPanel.transform, "Btn_Height", "Yükseklik Fobisi", iconHeight, new Vector2(-400, 0));
        GameObject btnSpider = CreateButton(mainPanel.transform, "Btn_Spider", "Örümcek Fobisi", iconSpider, new Vector2(400, 0));

        // 5. Create Dropdown Container (Height Options)
        GameObject heightDropdown = new GameObject("HeightOptions_Container");
        heightDropdown.transform.SetParent(mainPanel.transform, false); // Parent to Main Panel
        RectTransform dropRT = heightDropdown.AddComponent<RectTransform>();
        dropRT.anchoredPosition = new Vector2(-400, -250); // Position below Height button
        dropRT.sizeDelta = new Vector2(400, 300);
        
        // Buttons (Sub)
        // Adjust positions relative to container
        GameObject btnNature = CreateButton(heightDropdown.transform, "Btn_Nature", "Doğa Ortamı", iconNatureCity, new Vector2(0, 100));
        GameObject btnCity = CreateButton(heightDropdown.transform, "Btn_City", "Şehir Ortamı", iconNatureCity, new Vector2(0, -100));
        
        // Hide by default
        heightDropdown.SetActive(false);

        // 6. Assign References to Manager
        manager.heightOptionsContainer = heightDropdown;

        // 7. Link Buttons to Script
        UnityEventTools.AddPersistentListener(btnHeight.GetComponent<Button>().onClick, manager.ToggleHeightOptions);
        UnityEventTools.AddPersistentListener(btnSpider.GetComponent<Button>().onClick, manager.LoadSpiderScene);

        // Sub Panel Buttons
        UnityEventTools.AddPersistentListener(btnNature.GetComponent<Button>().onClick, manager.LoadNatureScene);
        UnityEventTools.AddPersistentListener(btnCity.GetComponent<Button>().onClick, manager.LoadCityScene);
        // "Back" button is no longer needed in dropdown logic as we toggle logic

        // 8. Ensure EventSystem
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        Debug.Log("VR Main Menu Created Successfully!");
        Selection.activeGameObject = canvasObj;
    }

    // HELPER: Forces the texture to be a Sprite and returns it
    private static Sprite LoadSpriteAuto(string path)
    {
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            if (importer.textureType != TextureImporterType.Sprite)
            {
                Debug.Log("Converting to Sprite: " + path);
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
            }
        }
        else
        {
            Debug.LogError("Texture not found at: " + path);
            return null;
        }

        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    private static GameObject CreatePanel(Transform parent, string name, Sprite sprite)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        Image img = panel.AddComponent<Image>();
        img.sprite = sprite;
        img.color = new Color(1, 1, 1, 0.9f); // Slight transparency
        RectTransform rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        return panel;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Sprite icon, Vector2 pos)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        Image img = btnObj.AddComponent<Image>();
        img.sprite = icon; // Can be null, will be white box
        img.color = Color.white;
        
        Button btn = btnObj.AddComponent<Button>();
        // Add flashy colors
        ColorBlock colors = btn.colors;
        colors.highlightedColor = Color.cyan;
        colors.pressedColor = Color.green;
        btn.colors = colors;

        RectTransform rt = btnObj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 300);
        rt.anchoredPosition = pos;

        // Text Label below button
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(btnObj.transform, false);
        Text txt = textObj.AddComponent<Text>();
        txt.text = label;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = 40;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.color = Color.white;
        
        // Position text below
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchoredPosition = new Vector2(0, -200); 
        textRT.sizeDelta = new Vector2(400, 100);

        return btnObj;
    }
}
