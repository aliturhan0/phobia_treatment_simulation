using UnityEngine;

public class BridgeSwayCreak : MonoBehaviour
{
    [Header("Sway (VR-safe, side-to-side)")]
    public float rotY = 2.0f;          // Yaw: sað-sol dönme (derece)
    public float rotZ = 1.0f;          // Roll: sað-sol yatma (derece)
    public float posZ = 0.03f;         // Sað-sol kayma (metre)  -> küçük tut
    public float periodSeconds = 5.0f; // Sallanma periyodu (sn)

    [Header("Gust / Irregularity")]
    public float secondWaveMul = 0.35f; // 2. dalga katkýsý (0-1)
    public float secondWaveRatio = 1.7f;

    [Header("Creak")]
    public AudioSource creakSource;    // Ayný objeye AudioSource ekle, buraya sürükle býrak
    public int creakEveryPeak = 1;     // 1=her tepe, 2=iki tepede bir
    public float creakCooldown = 0.35f;
    public Vector2 creakPitchRange = new Vector2(0.95f, 1.08f);

    Vector3 startPos;
    Quaternion startRot;

    float lastSign = 0f;
    int peakCount = 0;
    float creakTimer = 0f;

    void Awake()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;

        if (!creakSource) creakSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        creakTimer -= Time.deltaTime;

        float w = (2f * Mathf.PI) / Mathf.Max(0.1f, periodSeconds);
        float s1 = Mathf.Sin(Time.time * w);
        float s2 = Mathf.Sin(Time.time * w * secondWaveRatio + 0.7f);
        float s = s1 + s2 * secondWaveMul;

        // Side-to-side motion (posX yok -> uçlar daha az "kaymýþ" görünür)
        transform.localPosition = startPos + new Vector3(0f, 0f, s * posZ);
        transform.localRotation = startRot * Quaternion.Euler(0f, s * rotY, s * rotZ);

        // Creak at direction changes (peak-ish)
        float sign = Mathf.Sign(s1); // ana dalgaya göre tepe tespiti
        if (sign != 0f && sign != lastSign)
        {
            lastSign = sign;
            peakCount++;

            if (creakSource && creakEveryPeak > 0 && (peakCount % creakEveryPeak == 0) && creakTimer <= 0f)
            {
                creakSource.pitch = Random.Range(creakPitchRange.x, creakPitchRange.y);
                creakSource.Play();
                creakTimer = creakCooldown;
            }
        }
    }
}
