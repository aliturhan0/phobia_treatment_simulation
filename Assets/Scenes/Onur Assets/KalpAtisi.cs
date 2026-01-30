using UnityEngine;

/// <summary>
/// Yükseklik korkusu için kalp atışı sesi.
/// Işınlanınca başlar ve sabit hızda atar.
/// </summary>
public class KalpAtisi : MonoBehaviour
{
    [Header("Ses Ayarları")]
    [Tooltip("Kalp atışı ses dosyası")]
    public AudioClip kalpSesi;

    [Tooltip("Ses şiddeti")]
    [Range(0f, 1f)]
    public float sesYuksekligi = 0.7f;

    [Header("Kalp Hızı Ayarları")]
    [Tooltip("Kalp atış hızı (bpm) - 60=normal, 100=heyecanlı, 120=korku")]
    public float kalpHizi = 100f;

    [Header("Fade Ayarları")]
    [Tooltip("Sesin açılma süresi")]
    public float fadeSuresi = 1f;

    private AudioSource audioSource;
    private bool aktif = false;
    private float hedefSes = 0f;
    private float sonAtisZamani = 0f;
    private bool hazir = false;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;     // 2D ses
        audioSource.volume = 0f;

        Invoke("AktifEt", 1f);
    }

    void AktifEt()
    {
        hazir = true;
    }

    void Update()
    {
        if (!hazir || kalpSesi == null) return;

        // Volume fade
        if (aktif && audioSource.volume < hedefSes)
        {
            audioSource.volume += Time.deltaTime / fadeSuresi;
            audioSource.volume = Mathf.Min(audioSource.volume, hedefSes);
        }

        // Kalp atışı zamanlama (sabit hız)
        if (aktif)
        {
            float atisAraligi = 60f / kalpHizi;
            
            if (Time.time - sonAtisZamani >= atisAraligi)
            {
                audioSource.PlayOneShot(kalpSesi, audioSource.volume);
                sonAtisZamani = Time.time;
            }
        }
    }

    /// <summary>
    /// Kalp atışını başlat
    /// </summary>
    public void KorkuyuBaslat()
    {
        aktif = true;
        hedefSes = sesYuksekligi;
        sonAtisZamani = Time.time; // Hemen ilk atışı yap
        Debug.Log("Kalp atışı başladı! BPM: " + kalpHizi);
    }

    /// <summary>
    /// Kalp atışını durdur
    /// </summary>
    public void KorkuyuDurdur()
    {
        aktif = false;
        hedefSes = 0f;
    }
}
