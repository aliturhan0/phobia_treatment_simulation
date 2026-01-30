using UnityEngine;

/// <summary>
/// Rüzgar sesi - düşerken hızlanır/şiddetlenir.
/// </summary>
public class YukseklikSesleri : MonoBehaviour
{
    [Header("Ses Ayarları")]
    [Tooltip("Rüzgar ses dosyası (MP3 veya WAV)")]
    public AudioClip ruzgarSesi;

    [Tooltip("Normal ses şiddeti (0-1 arası)")]
    [Range(0f, 1f)]
    public float normalSesYuksekligi = 0.5f;

    [Tooltip("Düşerken maksimum ses şiddeti")]
    [Range(0f, 1f)]
    public float maxSesYuksekligi = 1f;

    [Tooltip("Fade-in süresi (saniye)")]
    public float fadeInSuresi = 2f;

    [Header("Düşüş Efekti")]
    [Tooltip("XR Origin veya Player objesi. Boş bırakırsan otomatik bulur.")]
    public Transform oyuncu;

    [Tooltip("Bu hızda ses maksimuma ulaşır (m/s)")]
    public float maxDususHizi = 20f;

    [Tooltip("Normal pitch (sabit durunca)")]
    public float normalPitch = 1f;

    [Tooltip("Maksimum pitch (düşerken)")]
    public float maxPitch = 1.5f;

    private AudioSource audioSource;
    private float hedefSes = 0f;
    private Vector3 oncekiPozisyon;
    private float dusmeHizi = 0f;
    private bool aktif = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = ruzgarSesi;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;     // 2D ses
        audioSource.volume = 0f;
        audioSource.pitch = normalPitch;

        // Oyuncuyu bul
        if (oyuncu == null)
        {
            // XR Origin'i bul
            GameObject xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin != null) oyuncu = xrOrigin.transform;
            
            // Yoksa Main Camera'yı bul
            if (oyuncu == null && Camera.main != null)
                oyuncu = Camera.main.transform;
        }

        if (oyuncu != null)
            oncekiPozisyon = oyuncu.position;
    }

    void Update()
    {
        if (!aktif) return;

        // Düşme hızını hesapla
        if (oyuncu != null)
        {
            float yFark = oncekiPozisyon.y - oyuncu.position.y; // Pozitif = düşüyor
            dusmeHizi = yFark / Time.deltaTime;
            oncekiPozisyon = oyuncu.position;

            // Düşme hızına göre ses ve pitch ayarla
            if (dusmeHizi > 1f) // Düşüyorsa (1 m/s'den fazla)
            {
                float dususOrani = Mathf.Clamp01(dusmeHizi / maxDususHizi);
                
                // Ses şiddeti artar
                hedefSes = Mathf.Lerp(normalSesYuksekligi, maxSesYuksekligi, dususOrani);
                
                // Pitch artar (daha tiz ses)
                audioSource.pitch = Mathf.Lerp(normalPitch, maxPitch, dususOrani);
            }
            else
            {
                // Normal durum
                hedefSes = normalSesYuksekligi;
                audioSource.pitch = Mathf.Lerp(audioSource.pitch, normalPitch, Time.deltaTime * 2f);
            }
        }

        // Volume fade
        audioSource.volume = Mathf.Lerp(audioSource.volume, hedefSes, Time.deltaTime / fadeInSuresi);

        // Güvenlik: ses durmuşsa tekrar başlat
        if (hedefSes > 0 && !audioSource.isPlaying && ruzgarSesi != null)
        {
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void SesiBaşlat()
    {
        if (ruzgarSesi == null)
        {
            Debug.LogWarning("Rüzgar sesi atanmamış!");
            return;
        }

        if (!audioSource.isPlaying)
        {
            audioSource.clip = ruzgarSesi;
            audioSource.Play();
        }

        if (oyuncu != null)
            oncekiPozisyon = oyuncu.position;
        
        hedefSes = normalSesYuksekligi;
        aktif = true;
        Debug.Log("Rüzgar sesi başladı!");
    }
}
