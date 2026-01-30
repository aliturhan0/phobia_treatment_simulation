using UnityEngine;
using UnityEngine.InputSystem; 
// using UnityEngine.XR; // REMOVED to avoid ambiguity with InputSystem
using System.Collections;

public class SafetyRailManager : MonoBehaviour
{
    [Header("Assign the PARENT object of everything")]
    public GameObject safetyRailParent;

    [Header("Animation Settings")]
    public float animationDuration = 1.0f; // Slower for mechanical feel

    [Header("Controller Input")]
    public InputActionProperty toggleButtonInput;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isSafe = false;
    private bool isAnimating = false;
    private Vector3 originalScale;

    void Start()
    {
        // 1. AUTO-FIND LOGIC (Just in case user forgets assignment)
        if (safetyRailParent == null)
        {
            safetyRailParent = GameObject.Find("SafetyRails");
        }

        if (safetyRailParent)
        {
            originalScale = safetyRailParent.transform.localScale;
            
            // Safety Check: If scale is weirdly 0 in editor, force it to 1
            if (originalScale.magnitude < 0.01f) 
            {
                originalScale = Vector3.one;
            }

            // Start Hidden (Scale 0)
            safetyRailParent.transform.localScale = Vector3.zero;
            safetyRailParent.SetActive(false);
        }
        else
        {
            Debug.LogError("ASSIGN SAFETY RAIL PARENT!");
        }
    }

    void OnEnable()
    {
        if (toggleButtonInput.action != null)
            toggleButtonInput.action.Enable();
    }

    void OnDisable()
    {
        if (toggleButtonInput.action != null)
            toggleButtonInput.action.Disable();
    }

    private bool wasPressedLastFrame = false;

    void Update()
    {
        // Debug Key: 'M'
        if (Input.GetKeyDown(KeyCode.M)) ToggleSafety();

        bool pressed = false;

        // 1. Try New Input System Action
        if (toggleButtonInput.action != null && toggleButtonInput.action.WasPressedThisFrame())
        {
            pressed = true;
        }

        // 2. Fallback: Try Direct XR Device (Right Controller 'A' Button / Primary)
        // using Fully Qualified Names to avoid Ambiguity with InputSystem
        if (!pressed)
        {
            UnityEngine.XR.InputDevice device = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(UnityEngine.XR.XRNode.RightHand);
            if (device.isValid)
            {
                if (device.TryGetFeatureValue(UnityEngine.XR.CommonUsages.primaryButton, out bool primaryPressed))
                {
                    // Check for "Down" event manually
                    if (primaryPressed && !wasPressedLastFrame)
                    {
                        pressed = true;
                    }
                    wasPressedLastFrame = primaryPressed;
                }
            }
        }

        if (pressed)
        {
            ToggleSafety();
        }
    }

    public void ToggleSafety()
    {
        if (isAnimating || safetyRailParent == null) return;
        StartCoroutine(AnimateScale(!isSafe));
    }

    IEnumerator AnimateScale(bool targetState)
    {
        isAnimating = true;
        isSafe = targetState;

        // Audio
        if (audioSource)
        {
            AudioClip clip = isSafe ? openSound : closeSound;
            if (clip) audioSource.PlayOneShot(clip);
        }

        Vector3 startScale = safetyRailParent.transform.localScale;
        Vector3 endScale = isSafe ? originalScale : Vector3.zero;

        if (isSafe) safetyRailParent.SetActive(true);

        float timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.SmoothStep(0, 1, timer / animationDuration);
            safetyRailParent.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }
        
        safetyRailParent.transform.localScale = endScale;

        if (!isSafe) safetyRailParent.SetActive(false);

        isAnimating = false;
    }
}
