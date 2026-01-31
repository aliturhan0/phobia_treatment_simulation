using System.Collections;
using UnityEngine;

public class ClaustrophobiaController : MonoBehaviour
{
    [Header("DUVARLAR")]
    public Transform duvar1; // Z -4
    public Transform duvar2; // X +3.5
    public Transform duvar3; // Z -5.5
    public Transform duvar4; // X +1.5

    public float moveDuration = 3f;
    private bool started = false;

    // XR Button'dan çağrılacak
    public void StartClaustrophobia()
    {
        if (started) return;
        started = true;

        Debug.Log("BAŞLAT BUTONUNA BASILDI");

        StartCoroutine(MoveWalls());
    }

    IEnumerator MoveWalls()
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
    }
}
