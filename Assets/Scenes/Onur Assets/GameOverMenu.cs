using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// VR Seviye Başlangıç Menüsü.
/// Canvas sahne açıldığında AÇIK, oyun DONDURULMUŞ başlar.
/// "BAŞLA" butonu oyunu başlatır.
/// "ANA MENÜYE DÖN" butonu ana menüye döner.
/// </summary>
public class GameOverMenu : MonoBehaviour
{
    [Header("Sahne Ayarları")]
    [Tooltip("Ana menü sahnesinin ismi (Build Settings'te olmalı)")]
    public string mainMenuSahnesi = "MainMenu";

    [Header("Canvas Referansı")]
    [Tooltip("Başlangıç/Pause menüsü Canvas'ı")]
    public GameObject menuCanvas;

    void Start()
    {
        // Başlangıçta menü AÇIK ve oyun DONDURULMUŞ
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(true);
        }
        
        // Oyunu dondur
        Time.timeScale = 0f;
        Debug.Log("Sahne başladı - Oyun donduruldu, menü açık.");
    }

    /// <summary>
    /// Oyunu başlat - BAŞLA butonu için
    /// </summary>
    public void Basla()
    {
        // Menüyü gizle
        if (menuCanvas != null)
        {
            menuCanvas.SetActive(false);
        }
        
        // Oyunu çöz
        Time.timeScale = 1f;
        Debug.Log("Oyun başladı!");
    }

    /// <summary>
    /// Ana menüye dön
    /// </summary>
    public void AnaMenuyeDon()
    {
        // Önce timeScale'i düzelt (yoksa sonraki sahne de donuk kalır!)
        Time.timeScale = 1f;
        
        Debug.Log("Ana menüye dönülüyor: " + mainMenuSahnesi);
        SceneManager.LoadScene(mainMenuSahnesi);
    }

    /// <summary>
    /// Sahneyi yeniden başlat (düşünce çağrılır)
    /// </summary>
    public void SahneyiSifirla()
    {
        // Önce timeScale'i düzelt
        Time.timeScale = 1f;
        
        Debug.Log("Sahne sıfırlanıyor...");
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
