using UnityEngine;
using UnityEngine.Events;

public class SimpleVRButton : MonoBehaviour
{
    [Header("Settings")]
    public float pressDistance = 0.02f; // How deep it goes
    public float returnSpeed = 5.0f;
    
    [Header("Events")]
    public UnityEvent onPressed;

    private Vector3 startPos;
    private bool isPressed = false;
    private Transform buttonTransform;

    void Start()
    {
        buttonTransform = transform;
        startPos = buttonTransform.localPosition;
    }

    void Update()
    {
        // Debug: Press 'B' on keyboard to test
        if (Input.GetKeyDown(KeyCode.B))
        {
            PressButton();
        }

        // Smooth return animation
        if (!isPressed)
        {
            buttonTransform.localPosition = Vector3.Lerp(buttonTransform.localPosition, startPos, Time.deltaTime * returnSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPressed) return;

        // Check if it's a hand (Player tag or Hand tag)
        if (other.CompareTag("Player") || other.CompareTag("Hand") || other.name.ToLower().Contains("hand"))
        {
            PressButton();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Hand") || other.name.ToLower().Contains("hand"))
        {
            isPressed = false;
        }
    }

    void PressButton()
    {
        isPressed = true;
        // Move visual down
        buttonTransform.localPosition = new Vector3(startPos.x, startPos.y - pressDistance, startPos.z);
        
        // Trigger Event
        onPressed.Invoke();
    }
}
