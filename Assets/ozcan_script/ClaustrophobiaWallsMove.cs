using System.Collections;
using UnityEngine;

public class ClaustrophobiaWallsMove : MonoBehaviour
{
    [Header("DUVARLAR")]
    public Transform duvar1; // Z -4
    public Transform duvar2; // X +3.5
    public Transform duvar3; // Z -5.5
    public Transform duvar4; // X +1.5

    [Header("HAREKET AYARLARI")]
    public float moveDuration = 3f;

    private bool triggered = false;

    private Vector3 d1Start, d2Start, d3Start, d4Start;
    private Vector3 d1Target, d2Target, d3Target, d4Target;

    void Start()
    {
        // Başlangıç pozisyonlarını kaydet
        d1Start = duvar1.position;
        d2Start = duvar2.position;
        d3Start = duvar3.position;
        d4Start = duvar4.position;

        // Hedef pozisyonları hesapla
        d1Target = d1Start + new Vector3(0f, 0f, -4f);
        d2Target = d2Start + new Vector3(3.5f, 0f, 0f);
        d3Target = d3Start + new Vector3(0f, 0f, -5.5f);
        d4Target = d4Start + new Vector3(1.5f, 0f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(MoveWalls());
        }
    }

    IEnumerator MoveWalls()
    {
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

        // Final pozisyonları garantiye al
        duvar1.position = d1Target;
        duvar2.position = d2Target;
        duvar3.position = d3Target;
        duvar4.position = d4Target;
    }
}
