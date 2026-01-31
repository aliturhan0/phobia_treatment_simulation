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

    [Header("DARALMA AYARLARI")]
    public float moveDuration = 3f;
    public float moveAmount = 2f; // ✅ 2 birim yaklaşma

    [Header("MERKEZ (opsiyonel)")]
    public Transform merkez; // boşsa 4 duvarın ortalaması alınır

    [Header("TETIKLEME")]
    public LayerMask playerLayer;

    [Header("XR ORIGIN (TELEPORT)")]
    public XROrigin xrOrigin;         // Inspector'dan XR Origin'i sürükle (önerilen)
    public float waitAfterShrink = 5f; // ✅ daralma bitince kaç sn beklesin

    [Header("SES (DARALIRKEN)")]
    public AudioSource shrinkAudioSource; // içine ses koyduğun AudioSource
    public bool loopSoundWhileShrinking = true;
    public bool stopAudioWhenDone = true;

    private bool started = false;

    // Sahneye ilk girilen başlangıç pozisyonu (kamera dünya konumu)
    private Vector3 startCameraWorldPos;
    private bool startPosSaved = false;

    void Start()
    {
        // xrOrigin otomatik bulmayı dener ama en sağlamı inspector'dan bağlamak
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();

        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            startCameraWorldPos = xrOrigin.Camera.transform.position;
            startPosSaved = true;
            Debug.Log("[Claustro] Başlangıç kamera konumu kaydedildi: " + startCameraWorldPos);
        }
        else
        {
            Debug.LogWarning("[Claustro] XROrigin veya Camera bulunamadı! xrOrigin'i inspector'dan bağla.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (started) return;

        // Player layer kontrolü
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
            return;

        started = true;
        StartCoroutine(ShrinkThenWaitThenReturnToStart());
    }

    IEnumerator ShrinkThenWaitThenReturnToStart()
    {
        // ====== SES BAŞLAT ======
        if (shrinkAudioSource != null)
        {
            shrinkAudioSource.loop = loopSoundWhileShrinking;
            shrinkAudioSource.Play();
        }

        // ====== DARALMA (EKSEN KISITLI) ======
        Vector3 p1 = duvar1.position;
        Vector3 p2 = duvar2.position;
        Vector3 p3 = duvar3.position;
        Vector3 p4 = duvar4.position;

        Vector3 center = (merkez != null) ? merkez.position : (p1 + p2 + p3 + p4) / 4f;

        // Merkeze doğru işaretler
        float d1zDir = Mathf.Sign(center.z - p1.z); // duvar1 Z
        float d3zDir = Mathf.Sign(center.z - p3.z); // duvar3 Z
        float d2xDir = Mathf.Sign(center.x - p2.x); // duvar2 X
        float d4xDir = Mathf.Sign(center.x - p4.x); // duvar4 X

        // Hedefler: SADECE ilgili eksen değişir
        Vector3 t1 = new Vector3(p1.x, p1.y, p1.z + d1zDir * moveAmount);              // duvar1: Z
        Vector3 t3 = new Vector3(p3.x, p3.y, p3.z + d3zDir * moveAmount);              // duvar3: Z
        Vector3 t2 = new Vector3(p2.x + d2xDir * moveAmount, p2.y, p2.z);              // duvar2: X
        Vector3 t4 = new Vector3(p4.x + d4xDir * moveAmount, p4.y, p4.z);              // duvar4: X

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            duvar1.position = Vector3.Lerp(p1, t1, t);
            duvar2.position = Vector3.Lerp(p2, t2, t);
            duvar3.position = Vector3.Lerp(p3, t3, t);
            duvar4.position = Vector3.Lerp(p4, t4, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Final garanti
        duvar1.position = t1;
        duvar2.position = t2;
        duvar3.position = t3;
        duvar4.position = t4;

        // ====== SES DURDUR ======
        if (shrinkAudioSource != null && stopAudioWhenDone)
            shrinkAudioSource.Stop();

        // ====== 5 SN BEKLE ======
        yield return new WaitForSeconds(waitAfterShrink);

        // ====== BAŞLANGIÇ KONUMUNA GERİ IŞINLA ======
        ReturnToStartPosition();
    }

    private void ReturnToStartPosition()
    {
        if (!startPosSaved)
        {
            Debug.LogWarning("[Claustro] Başlangıç konumu kaydedilmemiş, geri ışınlanamıyor.");
            return;
        }

        if (xrOrigin == null)
        {
            Debug.LogWarning("[Claustro] xrOrigin boş! Inspector'dan XR Origin'i bağla.");
            return;
        }

        // Teleport sırasında CC kapatmak (varsa) çok daha stabil
        CharacterController cc = xrOrigin.GetComponentInChildren<CharacterController>();
        if (cc != null) cc.enabled = false;

        // ✅ XR'a doğru teleport: kamerayı hedef dünya konumuna getir
        xrOrigin.MoveCameraToWorldLocation(startCameraWorldPos);

        if (cc != null) cc.enabled = true;

        Debug.Log("[Claustro] Başlangıç alanına geri dönüldü.");
    }
}
