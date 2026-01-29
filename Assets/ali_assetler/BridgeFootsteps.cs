using UnityEngine;
using UnityEngine.InputSystem; // NEW: For Joystick Input checking

public class BridgeFootsteps : MonoBehaviour
{
    public CharacterController characterController;
    public AudioSource footstepSource;
    
    [Header("Input Check (Fixes Swaying)")]
    public InputActionProperty moveInputSource; // Assign 'XRI LeftHand Locomotion/Move' here

    public float stepInterval = 0.5f; // Time between steps
    public float minSpeed = 0.5f;    // Increased to 0.5, ignores micro-movements

    [Header("Pitch Randomization")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.1f;

    private float stepTimer = 0f;

    void Update()
    {
        if (!characterController || !footstepSource) return;

        // 1. INPUT CHECK: Is user actually pushing the joystick?
        bool isInputMoving = true; // Default true if no input source assigned (fallback)
        if (moveInputSource.action != null)
        {
            float inputMag = moveInputSource.action.ReadValue<Vector2>().magnitude;
            if (inputMag < 0.1f) isInputMoving = false;
        }

        // 2. VELOCITY CHECK: Is character actually moving?
        Vector3 horizontalVelocity = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
        bool isPhysicallyMoving = horizontalVelocity.magnitude > minSpeed;

        // 3. SURFACE CHECK: Are we on the Bridge?
        bool isOnBridge = false;
        RaycastHit hit;
        
        // Debug draw to see the ray in Scene view
        Debug.DrawRay(transform.position + Vector3.up * 0.5f, Vector3.down * 2.0f, Color.red);

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2.0f))
        {
            // Logic: Tag IS "Bridge" OR Name contains "Bridge" (Case insensitive)
            if (hit.collider.CompareTag("Bridge") || hit.collider.name.ToLower().Contains("bridge"))
            {
                isOnBridge = true;
            }
            else
            {
                // DEBUG: Print what we are hitting if NOT bridge
                // Debug.Log("Standing on: " + hit.collider.name + " (Tag: " + hit.collider.tag + ")");
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
