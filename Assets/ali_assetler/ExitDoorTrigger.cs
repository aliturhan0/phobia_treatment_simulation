using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ali
{
    /// <summary>
    /// Çıkış kapısına koyulur. Oyuncu yaklaşınca panel EKRANDA açılır.
    /// Panel üzerindeki buton ana menüye döndürür.
    /// </summary>
    public class ExitDoorTrigger : MonoBehaviour
    {
        [Header("Panel Ayarları")]
        [Tooltip("Oyuncu yaklaşınca açılacak Canvas objesi")]
        public Canvas exitCanvas;

        void Start()
        {
            // Başlangıçta paneli gizle
            if (exitCanvas) exitCanvas.gameObject.SetActive(false);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                ShowPanel();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                HidePanel();
            }
        }

        void ShowPanel()
        {
            if (exitCanvas == null) return;

            // VR Kamerasını bul
            Camera vrCam = Camera.main;
            if (vrCam == null) vrCam = FindObjectOfType<Camera>();

            // Canvas'ı ekrana (kameraya) bağla
            exitCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            exitCanvas.worldCamera = vrCam;
            exitCanvas.planeDistance = 0.5f; // Gözün 50cm önünde
            exitCanvas.sortingOrder = 999;

            // Paneli tam ekran yap
            foreach (Transform child in exitCanvas.transform)
            {
                RectTransform rect = child.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // Tam ekran kaplama
                    rect.anchorMin = Vector2.zero;
                    rect.anchorMax = Vector2.one;
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
            }

            exitCanvas.gameObject.SetActive(true);
        }

        void HidePanel()
        {
            if (exitCanvas) exitCanvas.gameObject.SetActive(false);
        }

        /// <summary>
        /// Butona bağlanacak fonksiyon - Ana Menüye döner
        /// </summary>
        public void ReturnToMainMenu()
        {
            Debug.Log("[ExitDoor] Ana Menüye dönülüyor...");
            SceneManager.LoadScene(0);
        }
    }
}
