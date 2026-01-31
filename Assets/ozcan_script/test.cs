using System.Collections;
using UnityEngine;

public class WallMove_TestHard : MonoBehaviour
{
    public Transform duvar1; // z -4
    public Transform duvar2; // x +3.5
    public Transform duvar3; // z -5.5
    public Transform duvar4; // x +1.5

    public float moveDuration = 3f;

    bool started;

    void Update()
    {
        if (started) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            started = true;
            Debug.Log("T BASILDI -> DUVAR HAREKET TESTI BASLADI");

            StartCoroutine(MoveWallsWorld());
        }
    }

    IEnumerator MoveWallsWorld()
    {
        // WORLD position ile taşıyoruz (en net)
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

        Debug.Log("TEST BITTI -> DUVARLAR HEDEFE ULASTI");
    }
}
