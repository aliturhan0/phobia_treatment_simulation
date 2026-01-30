using UnityEngine;

public class IsinlanmaNoktasi : MonoBehaviour
{
    [Header("Hedef Ayarları")]
    [Tooltip("Oyuncunun ışınlanacağı nokta (boş bir GameObject koy binanın tepesine)")]
    public Transform hedefNokta;

    [Header("XR Rig Referansı (Opsiyonel)")]
    [Tooltip("XR Origin objesi. Boş bırakırsan otomatik bulur.")]
    public Transform xrRig;

    [Header("Kamera Referansı (Opsiyonel)")]
    [Tooltip("Main Camera. Boş bırakırsan otomatik bulur.")]
    public Transform vrKamera;

    [Header("Ses Sistemi (Opsiyonel)")]
    [Tooltip("Bina tepesindeki YukseklikSesleri scripti. Boş bırakırsan otomatik bulur.")]
    public YukseklikSesleri yukseklikSesleri;

    [Tooltip("Kalp atışı scripti. Boş bırakırsan otomatik bulur.")]
    public KalpAtisi kalpAtisi;

    [Header("Düşüş Sistemi (Opsiyonel)")]
    [Tooltip("Düşüş algılayıcı scripti. Boş bırakırsan otomatik bulur.")]
    public DususAlgilayici dususAlgilayici;

    [Header("Efektler (Opsiyonel)")]
    [Tooltip("Vertigo efekti scripti. Boş bırakırsan otomatik bulur.")]
    public VertigoEfekti vertigoEfekti;

    void OnTriggerEnter(Collider other)
    {
        // 1. DURUM: Eğer XR Rig'i elinle sürükleyip koyduysan
        if (xrRig != null && hedefNokta != null)
        {
            Isinla(xrRig);
            return;
        }

        // 2. DURUM: Elle koymadıysan, çarpan şeyin 'Player' olup olmadığına bak
        if (other.CompareTag("Player") && hedefNokta != null)
        {
            Transform playerRoot = other.transform.root;
            Isinla(playerRoot);
        }
    }

    void Isinla(Transform player)
    {
        // Varsa CharacterController'ı bul ve geçici olarak sustur
        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Kamerayı bul (atanmamışsa otomatik bul)
        Transform kamera = vrKamera;
        if (kamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null) kamera = mainCam.transform;
        }

        // OFFSET HESAPLA: Kamera ile XR Origin arasındaki yatay fark
        // (Kafa nereye bakarsa baksın, XR Origin'i doğru yere koymalıyız)
        if (kamera != null)
        {
            // Kameranın XR Origin'e göre yatay offset'i (Y hariç)
            Vector3 kameraOffset = kamera.position - player.position;
            kameraOffset.y = 0; // Sadece yatay offset (X ve Z)

            // Hedef pozisyondan offset'i çıkar
            Vector3 duzeltilmisPozisyon = hedefNokta.position - kameraOffset;
            player.position = duzeltilmisPozisyon;
        }
        else
        {
            // Kamera bulunamadıysa direkt ışınla
            player.position = hedefNokta.position;
        }

        // ROTASYON: Hedef nereye bakıyorsa oraya çevir
        player.rotation = hedefNokta.rotation;

        // CharacterController'ı geri aç
        if (cc != null) cc.enabled = true;

        // Rüzgar sesini başlat
        if (yukseklikSesleri != null)
        {
            yukseklikSesleri.SesiBaşlat();
        }
        else
        {
            // Otomatik bul
            YukseklikSesleri bulunanSes = FindObjectOfType<YukseklikSesleri>();
            if (bulunanSes != null)
            {
                bulunanSes.SesiBaşlat();
            }
        }

        // Kalp atışını başlat
        if (kalpAtisi != null)
        {
            kalpAtisi.KorkuyuBaslat();
        }
        else
        {
            // Otomatik bul
            KalpAtisi bulunanKalp = FindObjectOfType<KalpAtisi>();
            if (bulunanKalp != null)
            {
                bulunanKalp.KorkuyuBaslat();
            }
        }

        // Düşüş algılayıcıyı aktif et
        if (dususAlgilayici != null)
        {
            dususAlgilayici.AktifEt();
        }
        else
        {
            // Otomatik bul
            DususAlgilayici bulunanDusus = FindObjectOfType<DususAlgilayici>();
            if (bulunanDusus != null)
            {
                bulunanDusus.AktifEt();
            }
        }

        // Vertigo efektini aktif et
        if (vertigoEfekti != null)
        {
            vertigoEfekti.AktifEt();
        }
        else
        {
            // Otomatik bul
            VertigoEfekti bulunanVertigo = FindObjectOfType<VertigoEfekti>();
            if (bulunanVertigo != null)
            {
                bulunanVertigo.AktifEt();
            }
        }

        Debug.Log("Operasyon Tamam: Oyuncu çatıya yollandı!");
    }
}