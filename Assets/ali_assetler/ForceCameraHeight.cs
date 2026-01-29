using UnityEngine;

public class ForceCameraHeight : MonoBehaviour
{
    [Header("Settings")]
    public Transform cameraOffsetObject; // Assign 'Camera Offset' here
    public float targetHeight = 1.5f;    // Desired height
    
    void LateUpdate()
    {
        if (cameraOffsetObject)
        {
            // Lock the Y position to targetHeight, but let X and Z move freely
            Vector3 currentPos = cameraOffsetObject.localPosition;
            
            // IF it's not at the target height, Force it.
            if (Mathf.Abs(currentPos.y - targetHeight) > 0.01f)
            {
                cameraOffsetObject.localPosition = new Vector3(currentPos.x, targetHeight, currentPos.z);
            }
        }
    }
}
