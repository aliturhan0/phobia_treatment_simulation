using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// VR için Game Over / Pause menüsü.
/// Bu scripti boş bir GameObject'e ekle.
/// Oyuncu düştüğünde veya level bittiğinde MenuyuGoster() çağır.
/// </summary>
public class VRMenuCanvas : MonoBehaviour
{
    [Header("Sahne İsimleri")]
    [Tooltip("Ana menü sahnesinin ismi")]
    public string mainMenuSahnesi = "MainMenu";

    [Header("Görünüm Ayarları")]
    [Tooltip("Canvas'ın kameradan uzaklığı (metre)")]
    public float canvasUzakligi = 2f;

    [Tooltip("Canvas boyutu")]
    public float canvasBoyutu = 1f;

    [Header("Renk Ayarları")]
    public Color arkaplanRengi = new Color(0, 0, 0, 0.8f);
    public Color butonRengi = new Color(0.2f, 0.6f, 1f, 1f);
    public Color butonHoverRengi = new Color(0.3f, 0.7f, 1f, 1f);

    private GameObject canvasObj;
    private Canvas canvas;
    private bool menuAcik = false;

    void Start()
    {
        // Menüyü oluştur ama gizle
        MenuOlustur();
        MenuyuGizle();
    }

    void MenuOlustur()
    {
        // VR Kamerayı bul
        Camera vrCamera = Camera.main;
        if (vrCamera == null)
        {
            Debug.LogError("VRMenuCanvas: Main Camera bulunamadı!");
            return;
        }

        // === CANVAS OLUŞTUR ===
        canvasObj = new GameObject("VRMenuCanvas");
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = vrCamera;

        // Canvas'ı kameranın önüne yerleştir (child değil, dünyada sabit)
        canvasObj.transform.position = vrCamera.transform.position + vrCamera.transform.forward * canvasUzakligi;
        canvasObj.transform.rotation = vrCamera.transform.rotation;
        canvasObj.transform.localScale = Vector3.one * 0.001f * canvasBoyutu;

        // RectTransform ayarla
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(1200, 800);

        // Raycaster ekle (VR tıklama için)
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // === ARKAPLAN PANELİ ===
        GameObject panelObj = new GameObject("Panel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = arkaplanRengi;

        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // === BAŞLIK ===
        GameObject baslikObj = new GameObject("Baslik");
        baslikObj.transform.SetParent(panelObj.transform, false);
        
        TextMeshProUGUI baslikText = baslikObj.AddComponent<TextMeshProUGUI>();
        baslikText.text = "OYUN BİTTİ";
        baslikText.fontSize = 80;
        baslikText.alignment = TextAlignmentOptions.Center;
        baslikText.color = Color.white;
        baslikText.fontStyle = FontStyles.Bold;

        RectTransform baslikRect = baslikObj.GetComponent<RectTransform>();
        baslikRect.anchorMin = new Vector2(0, 0.7f);
        baslikRect.anchorMax = new Vector2(1, 0.95f);
        baslikRect.offsetMin = Vector2.zero;
        baslikRect.offsetMax = Vector2.zero;

        // === BUTONLAR ===
        // Tekrar Dene Butonu
        CreateButton(panelObj.transform, "TekrarDeneBtn", "TEKRAR DENE", 
            new Vector2(0.2f, 0.35f), new Vector2(0.8f, 0.55f), TekrarDene);

        // Main Menü Butonu
        CreateButton(panelObj.transform, "MainMenuBtn", "ANA MENÜ", 
            new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.3f), MainMenuyeDon);

        Debug.Log("VRMenuCanvas: Menü oluşturuldu!");
    }

    void CreateButton(Transform parent, string isim, string yazi, Vector2 anchorMin, Vector2 anchorMax, UnityEngine.Events.UnityAction onClick)
    {
        // Buton objesi
        GameObject btnObj = new GameObject(isim);
        btnObj.transform.SetParent(parent, false);

        // Image (buton arkaplanı)
        Image btnImage = btnObj.AddComponent<Image>();
        btnImage.color = butonRengi;

        // Button component
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImage;
        btn.onClick.AddListener(onClick);

        // Hover rengi
        ColorBlock colors = btn.colors;
        colors.normalColor = butonRengi;
        colors.highlightedColor = butonHoverRengi;
        colors.pressedColor = new Color(0.1f, 0.4f, 0.8f, 1f);
        btn.colors = colors;

        // RectTransform
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchorMin = anchorMin;
        btnRect.anchorMax = anchorMax;
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        // Buton yazısı
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);

        TextMeshProUGUI btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = yazi;
        btnText.fontSize = 50;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;
        btnText.fontStyle = FontStyles.Bold;

        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
    }

    // === PUBLIC METODLAR ===

    /// <summary>
    /// Menüyü göster (oyun bittiğinde çağır)
    /// </summary>
    public void MenuyuGoster()
    {
        if (canvasObj == null) return;

        // Canvas'ı kameranın önüne getir
        Camera vrCamera = Camera.main;
        if (vrCamera != null)
        {
            canvasObj.transform.position = vrCamera.transform.position + vrCamera.transform.forward * canvasUzakligi;
            canvasObj.transform.rotation = vrCamera.transform.rotation;
        }

        canvasObj.SetActive(true);
        menuAcik = true;
        Debug.Log("VRMenuCanvas: Menü açıldı!");
    }

    /// <summary>
    /// Menüyü gizle
    /// </summary>
    public void MenuyuGizle()
    {
        if (canvasObj == null) return;
        canvasObj.SetActive(false);
        menuAcik = false;
    }

    /// <summary>
    /// Sahneyi yeniden yükle
    /// </summary>
    public void TekrarDene()
    {
        Debug.Log("Sahne yeniden yükleniyor...");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void MainMenuyeDon()
    {
        Debug.Log("Ana menüye dönülüyor: " + mainMenuSahnesi);
        SceneManager.LoadScene(mainMenuSahnesi);
    }

    /// <summary>
    /// Menü açık mı?
    /// </summary>
    public bool MenuAcikMi()
    {
        return menuAcik;
    }
}
