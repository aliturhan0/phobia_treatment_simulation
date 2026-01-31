using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickHit_NoNavMesh_Final : MonoBehaviour
{
    public string spiderTag = "Spider";

    [Header("Force")]
    public float baseImpulse = 1.5f;
    public float velocityMultiplier = 0.35f;
    public float maxImpulse = 3.0f;
    public float maxUp = 0.12f;

    [Header("Cooldown")]
    public float perSpiderCooldown = 0.15f;

    [Header("Spider Death")]
    public int hitsToDestroy = 2;
    public float destroyDelay = 0.05f;

    [Tooltip("Kırmızı ölüm particle prefab")]
    public GameObject deathParticlePrefab;

    [Tooltip("Pop sesi")]
    public AudioClip popSound;

    [Range(0f, 1f)]
    public float popVolume = 0.8f;

    private Vector3 _lastPos;
    private Vector3 _velocity;

    private readonly Dictionary<int, float> _lastHitTime = new();
    private readonly Dictionary<int, int> _hitCount = new();

    void Start()
    {
        _lastPos = transform.position;
    }

    void FixedUpdate()
    {
        _velocity = (transform.position - _lastPos) / Time.fixedDeltaTime;
        _lastPos = transform.position;
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // Child collider kontrolü
        if (!other.CompareTag(spiderTag))
        {
            Transform p = other.transform.parent;
            while (p != null && !p.CompareTag(spiderTag)) p = p.parent;
            if (p == null) return;
            other = p.gameObject;
        }

        int id = other.GetInstanceID();

        // Cooldown
        if (_lastHitTime.TryGetValue(id, out float t) && Time.time - t < perSpiderCooldown)
            return;
        _lastHitTime[id] = Time.time;

        // Hit say
        int hits = _hitCount.ContainsKey(id) ? _hitCount[id] + 1 : 1;
        _hitCount[id] = hits;

        bool isFinalHit = hits >= hitsToDestroy;

        // 💀 ÖLÜM
        if (isFinalHit)
        {
            // 🔴 Particle
            if (deathParticlePrefab != null)
            {
                Instantiate(
                    deathParticlePrefab,
                    collision.GetContact(0).point,
                    Quaternion.identity
                );
            }

            // 🔊 Pop sesi
            if (popSound != null)
            {
                AudioSource.PlayClipAtPoint(
                    popSound,
                    collision.GetContact(0).point,
                    popVolume
                );
            }

            _hitCount.Remove(id);
            _lastHitTime.Remove(id);

            Destroy(other, destroyDelay);
            return;
        }

        // --- HAYATTA KALDIYSA SAVRULSUN ---

        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) return;

        var roam = other.GetComponent<SpiderRoam_PhysicsAvoid>();
        if (roam != null) roam.EnablePhysicsOnHit();
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Vector3 dir = _velocity.sqrMagnitude > 0.0001f ? _velocity.normalized : transform.forward;
        dir.y = Mathf.Clamp(dir.y, -0.05f, maxUp);
        dir.Normalize();

        float speed = _velocity.magnitude;
        float impulse = Mathf.Clamp(baseImpulse + speed * velocityMultiplier, 0f, maxImpulse);

        Vector3 hitPoint = collision.GetContact(0).point;
        rb.AddForceAtPosition(dir * impulse, hitPoint, ForceMode.Impulse);

        rb.angularVelocity *= 0.6f;
    }
}
