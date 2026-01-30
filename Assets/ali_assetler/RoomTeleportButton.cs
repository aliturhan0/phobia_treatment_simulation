using UnityEngine;

namespace Ali
{
    /// <summary>
    /// Butona basınca oyuncuyu belirlenen noktaya ışınlar.
    /// Her buton için bu scripti ekle ve hedefi ayarla.
    /// </summary>
    public class RoomTeleportButton : MonoBehaviour
    {
        [Header("Hedef Ayarları")]
        [Tooltip("Oyuncunun ışınlanacağı nokta (Empty GameObject)")]
        public Transform teleportTarget;

        [Header("Oyuncu (Otomatik Bulunur)")]
        public GameObject xrOrigin;

        void Start()
        {
            // XR Origin'i otomatik bul
            if (xrOrigin == null)
            {
                xrOrigin = GameObject.Find("XR Origin");
                if (xrOrigin == null)
                    xrOrigin = GameObject.Find("XR Origin (XR Rig)");
            }
        }

        /// <summary>
        /// Butonun OnClick() olayına bu fonksiyonu bağla
        /// </summary>
        public void TeleportToRoom()
        {
            if (xrOrigin == null)
            {
                Debug.LogError("[RoomTeleport] XR Origin bulunamadı!");
                return;
            }

            if (teleportTarget == null)
            {
                Debug.LogError("[RoomTeleport] Hedef nokta atanmamış!");
                return;
            }

            Debug.Log("[RoomTeleport] Işınlanıyor: " + teleportTarget.name);

            // CharacterController varsa kapat
            CharacterController cc = xrOrigin.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;

            // Oyuncuyu ışınla
            xrOrigin.transform.position = teleportTarget.position;
            xrOrigin.transform.rotation = teleportTarget.rotation;

            // CharacterController'ı geri aç
            if (cc) cc.enabled = true;
        }
    }
}
