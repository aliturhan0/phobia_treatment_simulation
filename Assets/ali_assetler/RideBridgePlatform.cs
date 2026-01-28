using UnityEngine;

public class RideBridgePlatform : MonoBehaviour
{
    public Transform bridgeSwayRoot;   // BridgeSwayRoot'u buraya ver
    public Transform xrOriginRoot;     // XR Origin (XR Rig) root'u buraya ver

    Transform originalParent;

    void OnTriggerEnter(Collider other)
    {
        if (!xrOriginRoot || !bridgeSwayRoot) return;
        if (!other.CompareTag("Player")) return;

        originalParent = xrOriginRoot.parent;
        xrOriginRoot.SetParent(bridgeSwayRoot, true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!xrOriginRoot) return;
        if (!other.CompareTag("Player")) return;

        xrOriginRoot.SetParent(originalParent, true);
    }
}
