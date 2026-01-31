using System.Collections;
using UnityEngine;
using Unity.XR.CoreUtils;

public class ClaustroTriggerStart : MonoBehaviour
{
    [Header("DUVARLAR (EKSEN KISITLI)")]
    public Transform duvar1; // SADECE Z
    public Transform duvar2; // SADECE X
    public Transform duvar3; // SADECE Z
    public Transform duvar4; // SADECE X

    [Header("TAVAN")]
    public Transform tavan;  // ✅ TAVAN - SADECE Y ekseni (aşağı iner)
    public float tavanMoveAmount = 2.5f; // Tavan 2.5 birim inecek (aynı kalıyor)

    [Header("DARALMA AYARLARI")]
    public float moveDuration = 8f; // 8 saniye daralma
    public float moveAmount = 7f;   // ✅ 7 birim yaklaşma (DAHA SIKIŞIK!)

    [Header("MERKEZ (opsiyonel)")]
    public Transform merkez; // boşsa 4 duvarın ortalaması alınır

    [Header("TETIKLEME")]
    public LayerMask playerLayer;

    [Header("XR ORIGIN (TELEPORT)")]
    public XROrigin xrOrigin; // Inspector'dan XR Origin'i sürükle

    [Header("BİTİŞ AYARLARI")]
    public float waitAfterShrink = 5f; // ✅ daralma bitince 5 sn bekle

    [Header("KALP SESİ (TÜM SÜRE BOYUNCA)")]
    public AudioSource heartbeatAudioSource; // Kalp sesi AudioSource
    public bool loopHeartbeat = true;        // Sürekli loop

    private bool started = false;
    private Vector3 startCameraWorldPos;
    private bool startPosSaved = false;

    void Start()
    {
        // XR Origin bul
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();

        // Başlangıç konumunu kaydet
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            startCameraWorldPos = xrOrigin.Camera.transform.position;
            startPosSaved = true;
            Debug.Log("[Claustro] Başlangıç konumu kaydedildi: " + startCameraWorldPos);
        }

        // Ses başlangıçta kapalı olsun
        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.loop = loopHeartbeat;
            heartbeatAudioSource.Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (started) return;

        // Player layer kontrolü
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
            return;

        started = true;
        StartCoroutine(ShrinkWaitAndTeleportBack());
    }

    IEnumerator ShrinkWaitAndTeleportBack()
    {
        // ====== KALP SESİ BAŞLAT ======
        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.loop = loopHeartbeat;
            heartbeatAudioSource.Play();
            Debug.Log("[Claustro] 💓 Kalp sesi başladı!");
        }

        // ====== DARALMA (5 SANİYE) ======
        Vector3 p1 = duvar1.position;
        Vector3 p2 = duvar2.position;
        Vector3 p3 = duvar3.position;
        Vector3 p4 = duvar4.position;
        Vector3 pTavan = (tavan != null) ? tavan.position : Vector3.zero;

        Vector3 center = (merkez != null) ? merkez.position : (p1 + p2 + p3 + p4) / 4f;

        // Merkeze doğru işaretler
        float d1zDir = Mathf.Sign(center.z - p1.z);
        float d3zDir = Mathf.Sign(center.z - p3.z);
        float d2xDir = Mathf.Sign(center.x - p2.x);
        float d4xDir = Mathf.Sign(center.x - p4.x);

        // Hedefler - Duvarlar
        Vector3 t1 = new Vector3(p1.x, p1.y, p1.z + d1zDir * moveAmount);
        Vector3 t3 = new Vector3(p3.x, p3.y, p3.z + d3zDir * moveAmount);
        Vector3 t2 = new Vector3(p2.x + d2xDir * moveAmount, p2.y, p2.z);
        Vector3 t4 = new Vector3(p4.x + d4xDir * moveAmount, p4.y, p4.z);
        
        // Hedef - Tavan (aşağı iner)
        Vector3 tTavan = (tavan != null) ? new Vector3(pTavan.x, pTavan.y - tavanMoveAmount, pTavan.z) : Vector3.zero;

        Debug.Log("[Claustro] 🧱 Duvarlar ve tavan daralıyor... (" + moveDuration + " saniye)");

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            duvar1.position = Vector3.Lerp(p1, t1, t);
            duvar2.position = Vector3.Lerp(p2, t2, t);
            duvar3.position = Vector3.Lerp(p3, t3, t);
            duvar4.position = Vector3.Lerp(p4, t4, t);
            
            // Tavan da insin
            if (tavan != null)
                tavan.position = Vector3.Lerp(pTavan, tTavan, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final pozisyonlar
        duvar1.position = t1;
        duvar2.position = t2;
        duvar3.position = t3;
        duvar4.position = t4;
        if (tavan != null)
            tavan.position = tTavan;

        Debug.Log("[Claustro] 🧱 Daralma tamamlandı! " + waitAfterShrink + " saniye bekleniyor...");

        // ====== İÇERDE 5 SANİYE BEKLE (KALP SESİ DEVAM EDİYOR) ======
        yield return new WaitForSeconds(waitAfterShrink);

        // ====== KALP SESİ DURDUR ======
        if (heartbeatAudioSource != null)
        {
            heartbeatAudioSource.Stop();
            Debug.Log("[Claustro] 💓 Kalp sesi durdu.");
        }

        // ====== ODA ESKİ HALİNE DÖNSÜN ======
        Debug.Log("[Claustro] 🔄 Oda eski haline dönüyor...");
        
        // Duvarları geri al
        duvar1.position = p1;
        duvar2.position = p2;
        duvar3.position = p3;
        duvar4.position = p4;
        
        // Tavanı geri al
        if (tavan != null)
            tavan.position = pTavan;

        Debug.Log("[Claustro] ✅ Oda resetlendi!");

        // ====== BAŞLANGIÇ KONUMUNA GERİ IŞINLA ======
        TeleportToStart();
    }

    private void TeleportToStart()
    {
        if (!startPosSaved)
        {
            Debug.LogWarning("[Claustro] Başlangıç konumu kaydedilmemiş!");
            return;
        }

        if (xrOrigin == null)
        {
            Debug.LogWarning("[Claustro] xrOrigin boş!");
            return;
        }

        // CharacterController varsa kapat (teleport için)
        CharacterController cc = xrOrigin.GetComponentInChildren<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Geri ışınla
        xrOrigin.MoveCameraToWorldLocation(startCameraWorldPos);

        if (cc != null) cc.enabled = true;

        Debug.Log("[Claustro] ✅ Başlangıç alanına geri ışınlandı!");
        
        // Reset for next trigger
        started = false;
    }
}
