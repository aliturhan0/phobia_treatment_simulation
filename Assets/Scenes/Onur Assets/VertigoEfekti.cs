using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Aşağı bakınca vertigo efekti - URP gerektirmez!
/// UI Image ile basit vignette efekti.
/// </summary>
public class VertigoEfekti : MonoBehaviour
{
    [Header("Kamera Referansı")]
    [Tooltip("VR Kamera. Boş bırakırsan Main Camera kullanılır.")]
    public Transform vrKamera;

    [Header("Efekt Ayarları")]
    [Tooltip("Efektin başladığı açı (derece). 20 = hafif aşağı bakış")]
    public float baslamaAcisi = 0f; // TEST: 0 derece - her zaman çalışsın

    [Tooltip("Efektin maksimum olduğu açı (derece). 70 = neredeyse dik aşağı")]
    public float maksimumAci = 45f; // TEST: 45 derece yeterli

    [Tooltip("Vignette efekti şiddeti (0-1)")]
    [Range(0f, 1f)]
    public float maksimumSiddet = 0.6f;

    [Tooltip("Efekt geçiş hızı")]
    public float gecisHizi = 3f;

    [Header("Vignette Rengi")]
    public Color vignetteRenk = Color.black;

    private Image vignetteImage;
    private float hedefAlpha = 0f;
    private float mevcutAlpha = 0f;
    private bool aktif = false;

    void Start()
    {
        // Kamerayı bul
        if (vrKamera == null && Camera.main != null)
        {
            vrKamera = Camera.main.transform;
        }

        // Vignette UI oluştur
        VignetteOlustur();
    }

    void VignetteOlustur()
    {
        // VR Kamerayı bul
        Camera vrCamera = null;
        if (vrKamera != null)
        {
            vrCamera = vrKamera.GetComponent<Camera>();
        }
        if (vrCamera == null)
        {
            vrCamera = Camera.main;
        }
        if (vrCamera == null)
        {
            Debug.LogError("VertigoEfekti: Main Camera bulunamadı!");
            return;
        }

        // Canvas oluştur - WORLD SPACE (VR için gerekli!)
        GameObject canvasObj = new GameObject("VertigoCanvas_VR");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = vrCamera;
        canvas.sortingOrder = 100;

        // Canvas'ı kameranın child'ı yap
        canvasObj.transform.SetParent(vrCamera.transform, false);
        canvasObj.transform.localPosition = new Vector3(0, 0, 0.3f); // 30cm önde
        canvasObj.transform.localRotation = Quaternion.identity;
        canvasObj.transform.localScale = Vector3.one * 0.001f; // Küçük scale

        // RectTransform ayarla
        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2000, 2000); // Geniş boyut

        // Vignette Image oluştur
        GameObject vignetteObj = new GameObject("VignetteImage");
        vignetteObj.transform.SetParent(canvasObj.transform, false);

        vignetteImage = vignetteObj.AddComponent<Image>();
        
        // Vignette sprite oluştur (gradient texture)
        vignetteImage.sprite = VignetteSprite();
        vignetteImage.color = new Color(vignetteRenk.r, vignetteRenk.g, vignetteRenk.b, 0f);
        vignetteImage.raycastTarget = false;

        // Tüm Canvas'ı kapla
        RectTransform rect = vignetteImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Debug.Log("VertigoEfekti: VR Canvas hazır!");
    }

    Sprite VignetteSprite()
    {
        // Radial gradient texture oluştur
        int size = 256;
        Texture2D tex = new Texture2D(size, size);
        
        Vector2 center = new Vector2(size / 2f, size / 2f);
        float maxDist = size / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), center);
                float normalizedDist = dist / maxDist;
                
                // Kenarlardan merkeze doğru azalan alpha
                float alpha = Mathf.Clamp01(normalizedDist - 0.3f) / 0.7f;
                alpha = Mathf.Pow(alpha, 1.5f); // Daha yumuşak geçiş
                
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }
        
        tex.Apply();
        
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
    }

    void Update()
    {
        if (!aktif || vrKamera == null) return;

        // Kameranın aşağı bakış açısını hesapla
        float asagiBakisAcisi = Vector3.Angle(vrKamera.forward, Vector3.down);
        float asagiBakisDerecesi = 90f - asagiBakisAcisi; // 0 = düz, 90 = tam aşağı

        // Efekt şiddetini hesapla
        if (asagiBakisDerecesi > baslamaAcisi)
        {
            float oran = Mathf.Clamp01((asagiBakisDerecesi - baslamaAcisi) / (maksimumAci - baslamaAcisi));
            hedefAlpha = oran * maksimumSiddet;
        }
        else
        {
            hedefAlpha = 0f;
        }

        // Smooth geçiş
        mevcutAlpha = Mathf.Lerp(mevcutAlpha, hedefAlpha, Time.deltaTime * gecisHizi);

        // Vignette'i güncelle
        if (vignetteImage != null)
        {
            vignetteImage.color = new Color(vignetteRenk.r, vignetteRenk.g, vignetteRenk.b, mevcutAlpha);
        }
    }

    /// <summary>
    /// Vertigo efektini aktif et (ışınlanınca çağrılır)
    /// </summary>
    public void AktifEt()
    {
        aktif = true;
        Debug.Log("Vertigo efekti aktif!");
    }

    /// <summary>
    /// Vertigo efektini kapat
    /// </summary>
    public void Kapat()
    {
        aktif = false;
        hedefAlpha = 0f;
    }
}
