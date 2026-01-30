using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ali
{
    /// <summary>
    /// Butona basınca ana menüye döner.
    /// Bu scripti butona ekle ve OnClick'e bağla.
    /// </summary>
    public class MainMenuButton : MonoBehaviour
    {
        /// <summary>
        /// Butonun OnClick olayına bu fonksiyonu bağla
        /// </summary>
        public void GoToMainMenu()
        {
            Debug.Log("Ana Menüye dönülüyor...");
            SceneManager.LoadScene(0);
        }
    }
}
