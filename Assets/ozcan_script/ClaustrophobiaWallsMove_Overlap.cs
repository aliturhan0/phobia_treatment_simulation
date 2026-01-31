using System.Collections;
using UnityEngine;

public class ClaustrophobiaWallsMove_Overlap : MonoBehaviour
{
    [Header("DUVARLAR")]
    public Transform duvar1; // Z -4
    public Transform duvar2; // X +3.5
    public Transform duvar3; // Z -5.5
    public Transform duvar4; // X +1.5

    [Header("HAREKET")]
    public float moveDuration = 3f;

    [Header("OYUNCU ALGILAMA (XR GARANTI)")]
    public LayerMask playerLayer;                 // Player layer
    public Vector3 boxHalfExtents = new Vector3(0.5f, 0.25f, 0.5f);
    public float boxUpOffset = 0.2f;              // kutuyu biraz yukarı al
    public bool debugLog = true;

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        Vector3 center = transform.position + Vector3.up * boxUpOffset;

        // Platformun üstünde Player layer var mı?
        Collider[] hits = Physics.OverlapBox(center, boxHalfExtents, Quaternion.identity, playerLayer);

        if (hits != null && hits.Length > 0)
        {
            triggered = true;
            if (debugLog) Debug.Log("[Overlap] Player algilandi -> Duvarlar basliyor");
            StartCoroutine(MoveWallsWorld());
        }
    }

    IEnumerator MoveWallsWorld()
    {
        Vector3 d1Start = duvar1.position;
        Vector3 d2Start = duvar2.position;
        Vector3 d3Start = duvar3.position;
        Vector3 d4Start = duvar4.position;

        Vector3 d1Target = d1Start + new Vector3(0f, 0f, -4f);
        Vector3 d2Target = d2Start + new Vector3(3.5f, 0f, 0f);
        Vector3 d3Target = d3Start + new Vector3(0f, 0f, -5.5f);
        Vector3 d4Target = d4Start + new Vector3(1.5f, 0f, 0f);

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            duvar1.position = Vector3.Lerp(d1Start, d1Target, t);
            duvar2.position = Vector3.Lerp(d2Start, d2Target, t);
            duvar3.position = Vector3.Lerp(d3Start, d3Target, t);
            duvar4.position = Vector3.Lerp(d4Start, d4Target, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        duvar1.position = d1Target;
        duvar2.position = d2Target;
        duvar3.position = d3Target;
        duvar4.position = d4Target;

        if (debugLog) Debug.Log("[Overlap] Duvar hareketi bitti");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.up * boxUpOffset, Quaternion.identity, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, boxHalfExtents * 2f);
    }
}
