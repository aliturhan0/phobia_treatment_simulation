using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Oyuncu bu trigger'a düşünce ekran beyazlaşır ve sahne yeniden yüklenir.
/// Bu scripti yerin altına büyük bir Box Collider ile koy.
/// </summary>
public class DususAlgilayici : MonoBehaviour
{
    [Header("Ayarlar")]
    [Tooltip("Ekran beyazlaşma süresi (saniye)")]
    public float beyazlasmaSuresi = 0.5f;

    [Tooltip("Beyaz ekranda bekleme süresi (saniye)")]
    public float bekleSuresi = 1f;

    private bool dustu = false;
    private bool aktif = false;
    private Image fadePanel;

    void Start()
    {
        FadePaneliOlustur();
    }

    void FadePaneliOlustur()
    {
        // VR Kamerayı bul
        Camera vrCamera = Camera.main;
        if (vrCamera == null)
        {
            Debug.LogError("DususAlgilayici: Main Camera bulunamadı!");
            return;
        }

        // Canvas oluştur - WORLD SPACE (VR için gerekli!)
        GameObject fadeCanvas = new GameObject("FadeCanvas_VR");
        Canvas canvas = fadeCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = vrCamera;
        canvas.sortingOrder = 999;

        // Canvas'ı kameranın child'ı yap
        fadeCanvas.transform.SetParent(vrCamera.transform, false);
        fadeCanvas.transform.localPosition = new Vector3(0, 0, 0.3f); // 30cm önde
        fadeCanvas.transform.localRotation = Quaternion.identity;
        fadeCanvas.transform.localScale = Vector3.one * 0.001f; // Küçük scale (1mm = 1 pixel)

        // RectTransform ayarla
        RectTransform canvasRect = fadeCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2000, 2000); // Geniş boyut

        // Beyaz panel oluştur
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(fadeCanvas.transform, false);

        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(1f, 1f, 1f, 0f); // Beyaz, görünmez

        // Tüm Canvas'ı kapla
        RectTransform rect = fadePanel.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Debug.Log("DususAlgilayici: VR Canvas oluşturuldu!");
    }

    void OnTriggerEnter(Collider other)
    {
        if (!aktif) return;
        if (dustu) return;

        if (OyuncuMu(other.gameObject))
        {
            dustu = true;
            Debug.Log("Oyuncu düştü! Ekran beyazlaşıyor...");
            StartCoroutine(DususEfekti());
        }
    }

    public void AktifEt()
    {
        aktif = true;
        Debug.Log("Düşüş algılayıcı aktif!");
    }

    bool OyuncuMu(GameObject obj)
    {
        if (obj.CompareTag("Player")) return true;
        if (obj.transform.root.CompareTag("Player")) return true;

        if (obj.name.Contains("XR") || obj.name.Contains("Camera") ||
            obj.name.Contains("Controller") || obj.name.Contains("Hand"))
            return true;

        return false;
    }

    IEnumerator DususEfekti()
    {
        // 1. Ekranı beyazlaştır (fade in)
        float timer = 0f;
        while (timer < beyazlasmaSuresi)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, timer / beyazlasmaSuresi);
            fadePanel.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        fadePanel.color = new Color(1f, 1f, 1f, 1f); // Tam beyaz

        // 2. Beyaz ekranda bekle
        yield return new WaitForSeconds(bekleSuresi);

        // 3. Sahneyi yeniden yükle (her şey resetlenir)
        Debug.Log("Sahne yeniden yükleniyor...");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}
