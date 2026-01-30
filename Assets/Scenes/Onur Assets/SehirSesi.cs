using UnityEngine;

/// <summary>
/// Şehir ambiyans sesi - aşağıdan gelen trafik/şehir sesi.
/// Bu scripti sahnenin alt kısmına (yerden) koy.
/// Oyuncu yükseldikçe ses mesafeden dolayı azalır (3D ses).
/// </summary>
public class SehirSesi : MonoBehaviour
{
    [Header("Ses Ayarları")]
    [Tooltip("Şehir ambiyans ses dosyası")]
    public AudioClip sehirSesi;

    [Tooltip("Ses şiddeti")]
    [Range(0f, 1f)]
    public float sesYuksekligi = 0.5f;

    [Header("3D Ses Ayarları")]
    [Tooltip("Sesin tam duyulduğu mesafe")]
    public float minMesafe = 5f;

    [Tooltip("Sesin duyulmaz olduğu mesafe")]
    public float maxMesafe = 100f;

    [Header("Başlangıç")]
    [Tooltip("Oyun başlayınca otomatik çalsın mı?")]
    public bool otomatikBasla = true;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = sehirSesi;
        audioSource.loop = true;           // Sürekli çal
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;     // 3D ses (mesafeye göre azalır)
        audioSource.minDistance = minMesafe;
        audioSource.maxDistance = maxMesafe;
        audioSource.volume = sesYuksekligi;
        audioSource.rolloffMode = AudioRolloffMode.Linear; // Doğrusal azalma

        if (otomatikBasla && sehirSesi != null)
        {
            Invoke("SesiBaşlat", 0.5f);
        }
    }

    public void SesiBaşlat()
    {
        if (sehirSesi == null)
        {
            Debug.LogWarning("Şehir sesi atanmamış!");
            return;
        }

        audioSource.Play();
        Debug.Log("Şehir sesi başladı!");
    }

    public void SesiDurdur()
    {
        audioSource.Stop();
    }
}
