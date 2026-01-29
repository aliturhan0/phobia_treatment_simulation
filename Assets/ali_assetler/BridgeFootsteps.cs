using UnityEngine;
using UnityEngine.InputSystem; // NEW: For Joystick Input checking

public class BridgeFootsteps : MonoBehaviour
{
    public CharacterController characterController;
    public AudioSource footstepSource;
    
    [Header("Input Check (Fixes Swaying)")]
    public InputActionProperty moveInputSource; // Assign 'XRI LeftHand Locomotion/Move' here

    [Header("Surface Settings")]
    public string surfaceTag = "WoodenStep"; // STRICT: Only this tag triggers footsteps

    public float stepInterval = 0.5f; // Time between steps
    public float minSpeed = 0.5f;    // Increased to 0.5, ignores micro-movements

    [Header("Pitch Randomization")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.1f;

    private float stepTimer = 0f;

    void Start()
    {
        // Auto-assign CharacterController if empty
        if (characterController == null) 
            characterController = GetComponentInParent<CharacterController>();

        // Auto-assign AudioSource if empty
        if (footstepSource == null) 
            footstepSource = GetComponent<AudioSource>();

        if (characterController == null) Debug.LogError("BridgeFootsteps: CharacterController bulunamadı!");
        if (footstepSource == null) Debug.LogError("BridgeFootsteps: AudioSource bulunamadı!");
    }

    void Update()
    {
        if (!characterController || !footstepSource) return;

        // 1. INPUT CHECK
        bool isInputMoving = true; 
        if (moveInputSource.action != null)
        {
            float inputMag = moveInputSource.action.ReadValue<Vector2>().magnitude;
            if (inputMag < 0.1f) isInputMoving = false;
        }

        // 2. VELOCITY CHECK
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        bool isPhysicallyMoving = horizontalVelocity.magnitude > minSpeed;

        // 3. SURFACE CHECK
        bool isOnBridge = false;
        RaycastHit hit;
        
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 2.0f, Color.red);

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2.0f))
        {
            // Restore Original Behavior: Check for "Bridge" tag
            if (hit.collider.CompareTag(surfaceTag) || hit.collider.CompareTag("Bridge"))
            {
                isOnBridge = true;
            }
        }

        // Only play if INPUT + MOVING + ON BRIDGE
        if (isInputMoving && isPhysicallyMoving && isOnBridge)
        {
            stepTimer -= Time.deltaTime;

            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0.1f;
        }
    }

    void PlayFootstep()
    {
        // Don't overlap if it's the same long clip (basic check)
        if (footstepSource.isPlaying && footstepSource.clip.length > 1f) return;

        footstepSource.pitch = Random.Range(minPitch, maxPitch);
        footstepSource.PlayOneShot(footstepSource.clip); 
    }
}
