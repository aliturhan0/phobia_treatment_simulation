using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FallRespawnManager : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot;        // XR Origin Root (Main Player Object)
    public Transform cameraOffset;      // Camera Offset (Shake this, NOT the root)
    public Transform respawnPoint;      // Target position
    public AudioSource windAudioSource; // Wind Sound (Looping)
    [Header("Visual Settings")]
    public CanvasGroup whiteScreenGroup;// White Fade Panel
    public CanvasGroup speedLinesGroup; // Speed Lines UI
    public ParticleSystem windParticleSystem; // Assign Speed Lines / Dust particles here
    public float maxParticleEmission = 50f;   // Max particles per second

    [Header("Haptics")]
    public ActionBasedController leftController;
    public ActionBasedController rightController;

    [Header("Fall Settings")]
    public float fallSpeedThreshold = 4.0f; // Increased to prevent false trigger on bridge
    public float maxWindSpeed = 15.0f;      // Speed at which wind is max volume
    public float shakeIntensity = 0.02f;    // How much camera shakes

    [Header("Respawn Settings")]
    public float fadeSpeed = 5.0f;
    public float spawnHeightOffset = 1.0f;

    private Vector3 lastPosition;
    private float currentVerticalSpeed;
    private Vector3 originalCameraPos;
    private bool isDead = false;

    void Start()
    {
        if (playerRoot) lastPosition = playerRoot.position;

        // --- VR FIX START: Sadece Efekt Canvaslari Icin ---
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            if (whiteScreenGroup) FixCanvasForVR(whiteScreenGroup.GetComponentInParent<Canvas>(), mainCam);
            if (speedLinesGroup) FixCanvasForVR(speedLinesGroup.GetComponentInParent<Canvas>(), mainCam);
        }
        // --------------------------------------------------

        if (whiteScreenGroup)
        {
            whiteScreenGroup.alpha = 0f;
            whiteScreenGroup.blocksRaycasts = false;
        }
        if (windAudioSource)
        {
            windAudioSource.volume = 0f;
            if (!windAudioSource.isPlaying) windAudioSource.Play();
        }
        if (windParticleSystem)
        {
            windParticleSystem.Stop();
        }
    }

    // Sadece verilen canvas'i VR uyumlu yapar, digerlerine dokunmaz
    void FixCanvasForVR(Canvas cvs, Camera cam)
    {
        if (cvs != null)
        {
            cvs.renderMode = RenderMode.ScreenSpaceCamera;
            cvs.worldCamera = cam;
            cvs.planeDistance = 0.5f; // Gozun 50cm onunde
            cvs.sortingOrder = 999;   // En on sirada

            // Fix Edge Gap: Force the content to be larger than the screen
            // We iterate through immediate children (which should be the Panels)
            foreach (Transform child in cvs.transform)
            {
                RectTransform rect = child.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // Stretch anchors way beyond 0-1
                    rect.anchorMin = new Vector2(-0.2f, -0.2f);
                    rect.anchorMax = new Vector2(1.2f, 1.2f);
                    rect.offsetMin = Vector2.zero;
                    rect.offsetMax = Vector2.zero;
                }
            }
        }
    }

    void Update()
    {
        if (!playerRoot || isDead) return;

        // 1. Calculate Vertical Speed (Manual calculation is safer than Rigidbody for VR rigs)
        float verticalDelta = lastPosition.y - playerRoot.position.y;
        currentVerticalSpeed = verticalDelta / Time.deltaTime;
        
        lastPosition = playerRoot.position;

        // 2. Handle Fall Effects (Wind & Shake & Particles & UI & Haptics)
        if (currentVerticalSpeed > fallSpeedThreshold)
        {
            // Calculate intensity (0 to 1) based on speed
            float intensity = Mathf.Clamp01((currentVerticalSpeed - fallSpeedThreshold) / (maxWindSpeed - fallSpeedThreshold));

            // Wind Volume
            if (windAudioSource)
            {
                windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, intensity, Time.deltaTime * 5f);
            }

            // Camera Shake (Jitter)
            if (cameraOffset)
            {
                Vector3 randomOffset = Random.insideUnitSphere * shakeIntensity * intensity;
                cameraOffset.localPosition = Vector3.Lerp(cameraOffset.localPosition, randomOffset, Time.deltaTime * 10f);
            }

            // Haptics (Vibration)
            if (intensity > 0.1f)
            {
                if (leftController) leftController.SendHapticImpulse(intensity * 0.5f, 0.1f);
                if (rightController) rightController.SendHapticImpulse(intensity * 0.5f, 0.1f);
            }

            // UI Speed Lines (Anime Effect)
            if (speedLinesGroup)
            {
                speedLinesGroup.alpha = Mathf.Lerp(speedLinesGroup.alpha, intensity, Time.deltaTime * 10f);
            }

            // Particle System (Wind/Speed Lines)
            if (windParticleSystem)
            {
                if (!windParticleSystem.isPlaying) windParticleSystem.Play();
                
                var emission = windParticleSystem.emission;
                emission.rateOverTime = Mathf.Lerp(0f, maxParticleEmission, intensity);
            }
        }
        else
        {
            // Not falling fast enough -> Fade out effects
            if (windAudioSource)
            {
                windAudioSource.volume = Mathf.Lerp(windAudioSource.volume, 0f, Time.deltaTime * 2f);
            }
            if (cameraOffset)
            {
                cameraOffset.localPosition = Vector3.Lerp(cameraOffset.localPosition, Vector3.zero, Time.deltaTime * 5f);
            }
            if (speedLinesGroup)
            {
                speedLinesGroup.alpha = Mathf.Lerp(speedLinesGroup.alpha, 0f, Time.deltaTime * 5f);
            }
            if (windParticleSystem)
            {
                var emission = windParticleSystem.emission;
                emission.rateOverTime = Mathf.Lerp(emission.rateOverTime.constant, 0f, Time.deltaTime * 5f);
                
                if (emission.rateOverTime.constant < 0.1f) windParticleSystem.Stop();
            }
        }
    }

    // This TRIGGER is now the "DEATH ZONE" (Ground Hit)
    void OnTriggerEnter(Collider other)
    {
        if (isDead) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(DeathSequence());
        }
    }

    IEnumerator DeathSequence()
    {
        isDead = true;

        // 1. Instant White Screen (Impact Flash)
        if (whiteScreenGroup)
        {
            // Instantly go white (Flash)
            whiteScreenGroup.alpha = 1f; 
        }

        // 2. Teleport
        if (playerRoot && respawnPoint)
        {
            CharacterController cc = playerRoot.GetComponent<CharacterController>();
            if (cc) cc.enabled = false;
            
            // Apply offset to ensure we are above the collider
            playerRoot.position = respawnPoint.position + Vector3.up * spawnHeightOffset;
            
            // playerRoot.rotation = respawnPoint.rotation; 
            lastPosition = playerRoot.position;
            if (cameraOffset) cameraOffset.localPosition = Vector3.zero;

            if (cc) 
            {
                // Fix for "Step Offset" error preventing re-enable
                // Ensure step offset is reasonable relative to height
                if (cc.stepOffset >= cc.height)
                {
                    cc.stepOffset = cc.height * 0.25f;
                }
                
                // Wrap in try-catch to ensure flow continues even if Unity complains
                try {
                    cc.enabled = true;
                } catch {}
            }
        }

        // 3. Reset Audio
        if (windAudioSource) windAudioSource.volume = 0f;

        yield return new WaitForSeconds(3.0f); // Wait 3 seconds in white screen

        // 4. Fade Out
        if (whiteScreenGroup)
        {
            while (whiteScreenGroup.alpha > 0f)
            {
                whiteScreenGroup.alpha -= Time.deltaTime * fadeSpeed;
                yield return null;
            }
        }

        isDead = false;
    }
}