using UnityEngine;

public class BridgeEffect : MonoBehaviour
{
    public Transform xrOrigin;          // XR Origin'i sürükle
    public AudioSource creakSource;     // gýcýrtý AudioSource
    public float swayRotZ = 1.5f;       // derece (küçük tut)
    public float swayRotY = 1.0f;       // derece
    public float periodSeconds = 4.5f;  // yumuþak

    Quaternion startRot;
    bool active;

    void Start()
    {
        if (xrOrigin) startRot = xrOrigin.localRotation;
    }

    void Update()
    {
        if (!active || !xrOrigin) return;

        float w = (2f * Mathf.PI) / Mathf.Max(0.1f, periodSeconds);
        float s = Mathf.Sin(Time.time * w);

        xrOrigin.localRotation = startRot * Quaternion.Euler(0f, s * swayRotY, s * swayRotZ);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        active = true;
        if (xrOrigin) startRot = xrOrigin.localRotation;
        if (creakSource) creakSource.Play(); // giriþte bir gýcýr
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        active = false;
        if (xrOrigin) xrOrigin.localRotation = startRot;
    }
}
