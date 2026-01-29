using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
// Note: If using newer Toolkit, namespace might be UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement

public class RoomScaleEnforcer : MonoBehaviour
{
    [Header("Assign XR Origin")]
    public GameObject xrOrigin;
    
    private ActionBasedContinuousMoveProvider moveProvider;

    void Start()
    {
        if (xrOrigin == null) xrOrigin = GameObject.Find("XR Origin");
        
        if (xrOrigin)
        {
            moveProvider = xrOrigin.GetComponentInChildren<ActionBasedContinuousMoveProvider>();
            if (moveProvider == null)
            {
                // Try finding component on parent/children widely
                moveProvider = xrOrigin.GetComponent<ActionBasedContinuousMoveProvider>();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && moveProvider != null)
        {
            Debug.Log("Entered Room-Scale Zone: Disabling Joystick Movement.");
            moveProvider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && moveProvider != null)
        {
            Debug.Log("Exited Room-Scale Zone: Enabling Joystick Movement.");
            moveProvider.enabled = true;
        }
    }
}
