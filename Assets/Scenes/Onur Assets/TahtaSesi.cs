using UnityEngine;

/// <summary>
/// Oyuncu tahtanın üstünde durduğu sürece gıcırtı sesi çalar (loop).
/// Tahtadan inince ses durur.
/// </summary>
public class TahtaSesi : MonoBehaviour
{
    [Header("Ses Ayarları")]
    [Tooltip("Tahta gıcırtı sesi (loop olarak çalacak)")]
    public AudioClip tahtaSesi;

    [Tooltip("Ses şiddeti (0-1 arası)")]
    [Range(0f, 1f)]
    public float sesYuksekligi = 0.6f;

    [Header("Fade Ayarları")]
    [Tooltip("Fade-in/out süresi (saniye)")]
    public float fadeSuresi = 0.5f;

    private AudioSource audioSource;
    private bool oyuncuUstunde = false;
    private float hedefSes = 0f;
    private bool hazir = false;

    void Start()
    {
        // AudioSource komponenti ekle
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = tahtaSesi;
        audioSource.loop = true;           // Sürekli tekrar
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 1f;     // 3D ses
        audioSource.minDistance = 0.5f;
        audioSource.maxDistance = 10f;
        audioSource.volume = 0f;           // Başta sessiz
        
        // 1 saniye bekle
        Invoke("AktifEt", 1f);
    }

    void AktifEt()
    {
        hazir = true;
    }

    void Update()
    {
        if (!hazir) return;
        
        // Fade in/out efekti
        if (oyuncuUstunde && audioSource.volume < hedefSes)
        {
            audioSource.volume += Time.deltaTime / fadeSuresi;
            audioSource.volume = Mathf.Min(audioSource.volume, hedefSes);
        }
        else if (!oyuncuUstunde && audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime / fadeSuresi;
            audioSource.volume = Mathf.Max(audioSource.volume, 0);
            
            if (audioSource.volume <= 0)
                audioSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!hazir) return;
        
        SesiBaşlat();
        Debug.Log("Oyuncu tahtaya bastı!");
    }

    void OnTriggerStay(Collider other)
    {
        // Oyuncu hala üstündeyse ses çalmaya devam et
        if (!hazir) return;
        oyuncuUstunde = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (!hazir) return;
        
        SesiDurdur();
        Debug.Log("Oyuncu tahtadan indi!");
    }

    void SesiBaşlat()
    {
        if (tahtaSesi == null)
        {
            Debug.LogWarning("Tahta sesi atanmamış!");
            return;
        }
        
        if (!audioSource.isPlaying)
        {
            audioSource.clip = tahtaSesi;
            audioSource.Play();
        }
        
        hedefSes = sesYuksekligi;
        oyuncuUstunde = true;
    }

    void SesiDurdur()
    {
        oyuncuUstunde = false;
    }
}
