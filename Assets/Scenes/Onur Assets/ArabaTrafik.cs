using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Basit araba trafik sistemi - waypoint'leri takip eder, tekerlekler döner.
/// Bu scripti araba objesine ekle.
/// </summary>
public class ArabaTrafik : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    [Tooltip("Araba hızı (m/s)")]
    public float hiz = 8f;

    [Tooltip("Dönüş hızı")]
    public float donusHizi = 2f;

    [Header("Waypoint Sistemi")]
    [Tooltip("Takip edilecek waypoint'ler (sırayla)")]
    public Transform[] waypointler;

    [Tooltip("Son waypoint'e ulaşınca başa dön?")]
    public bool dongu = true;

    [Tooltip("Son waypoint'e ulaşınca yok ol?")]
    public bool sonundaYokOl = false;

    [Header("Tekerlek Ayarları")]
    [Tooltip("Tekerlek dönüş çarpanı")]
    public float tekerlekDonusHizi = 360f;

    // Otomatik bulunan tekerlekler
    private List<Transform> tekerlekler = new List<Transform>();
    private int mevcutWaypoint = 0;
    private bool hareketEdiyor = true;

    void Start()
    {
        // Tekerlekleri otomatik bul ("Wheel" içeren tüm çocuklar)
        TekerlekleriBul(transform);
        
        if (tekerlekler.Count > 0)
            Debug.Log($"{gameObject.name}: {tekerlekler.Count} tekerlek bulundu!");
    }

    void TekerlekleriBul(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // "Wheel" veya "wheel" içeren objeleri bul
            if (child.name.ToLower().Contains("wheel"))
            {
                tekerlekler.Add(child);
            }
            // Çocukların çocuklarını da kontrol et
            TekerlekleriBul(child);
        }
    }

    void Update()
    {
        if (!hareketEdiyor || waypointler == null || waypointler.Length == 0)
            return;

        // Hedef waypoint
        Transform hedef = waypointler[mevcutWaypoint];
        if (hedef == null) return;

        // Hedefe doğru yön
        Vector3 yon = hedef.position - transform.position;
        yon.y = 0; // Sadece yatay hareket

        // Hedefe ulaştık mı?
        if (yon.magnitude < 1f)
        {
            SonrakiWaypoint();
            return;
        }

        // Dönüş (yumuşak)
        Quaternion hedefRotasyon = Quaternion.LookRotation(yon);
        transform.rotation = Quaternion.Slerp(transform.rotation, hedefRotasyon, donusHizi * Time.deltaTime);

        // İleri hareket
        transform.position += transform.forward * hiz * Time.deltaTime;

        // Tekerlekleri döndür
        TekerlekleriDondur();
    }

    void TekerlekleriDondur()
    {
        float donusMiktari = hiz * tekerlekDonusHizi * Time.deltaTime;
        
        foreach (Transform tekerlek in tekerlekler)
        {
            // X ekseni etrafında döndür (ileri hareket)
            tekerlek.Rotate(donusMiktari, 0, 0, Space.Self);
        }
    }

    void SonrakiWaypoint()
    {
        mevcutWaypoint++;

        if (mevcutWaypoint >= waypointler.Length)
        {
            if (dongu)
            {
                mevcutWaypoint = 0; // Başa dön
            }
            else if (sonundaYokOl)
            {
                Destroy(gameObject); // Yok ol
            }
            else
            {
                hareketEdiyor = false; // Dur
            }
        }
    }

    // Editor'da waypoint çizgilerini göster
    void OnDrawGizmos()
    {
        if (waypointler == null || waypointler.Length < 2) return;

        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypointler.Length - 1; i++)
        {
            if (waypointler[i] != null && waypointler[i + 1] != null)
            {
                Gizmos.DrawLine(waypointler[i].position, waypointler[i + 1].position);
            }
        }

        // Döngü varsa son-ilk arası çiz
        if (dongu && waypointler.Length > 0)
        {
            if (waypointler[waypointler.Length - 1] != null && waypointler[0] != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(waypointler[waypointler.Length - 1].position, waypointler[0].position);
            }
        }
    }
}
