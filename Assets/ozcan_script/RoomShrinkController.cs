using System.Collections;
using UnityEngine;

public class RoomShrinkController : MonoBehaviour
{
    public Transform wallLeft;
    public Transform wallRight;
    public Transform wallFront;
    public Transform wallBack;

    [Header("Shrink Settings")]
    public float shrinkSpeed = 0.2f;
    public float minHalfSize = 1.0f;

    [Header("Timing")]
    public float waitAfterShrink = 5f;          // DARALMA BİTİNCE BEKLE
    public float returnDuration = 1.0f;         // GERİ AÇILMA SÜRESİ
    public float waitAfterReturn = 5f;           // GERİ AÇILINCA BEKLE

    private bool shrinking = false;
    private bool finished = false;

    // Başlangıç pozisyonları
    private Vector3 leftStart, rightStart, frontStart, backStart;

    void Start()
    {
        leftStart  = wallLeft.position;
        rightStart = wallRight.position;
        frontStart = wallFront.position;
        backStart  = wallBack.position;
    }

    void Update()
    {
        if (!shrinking) return;

        float halfWidth = Vector3.Distance(wallLeft.position, wallRight.position) * 0.5f;
        float halfDepth = Vector3.Distance(wallFront.position, wallBack.position) * 0.5f;

        if (halfWidth <= minHalfSize || halfDepth <= minHalfSize)
        {
            shrinking = false;

            if (!finished)
            {
                finished = true;
                StartCoroutine(WaitThenReturnThenRemove());
            }
            return;
        }

        float step = shrinkSpeed * Time.deltaTime;

        wallLeft.position  += new Vector3(+step, 0f, 0f);
        wallRight.position += new Vector3(-step, 0f, 0f);
        wallFront.position += new Vector3(0f, 0f, -step);
        wallBack.position  += new Vector3(0f, 0f, +step);
    }

    public void StartShrinking()
    {
        if (!wallRight.gameObject.activeSelf)
            wallRight.gameObject.SetActive(true);

        shrinking = true;
        finished = false;
    }

    IEnumerator WaitThenReturnThenRemove()
    {
        // 1) Daralma bittikten sonra bekle
        yield return new WaitForSeconds(waitAfterShrink);

        // 2) Duvarları eski pozisyona geri aç
        Vector3 leftFrom  = wallLeft.position;
        Vector3 rightFrom = wallRight.position;
        Vector3 frontFrom = wallFront.position;
        Vector3 backFrom  = wallBack.position;

        float t = 0f;
        while (t < returnDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / returnDuration);

            wallLeft.position  = Vector3.Lerp(leftFrom,  leftStart,  a);
            wallRight.position = Vector3.Lerp(rightFrom, rightStart, a);
            wallFront.position = Vector3.Lerp(frontFrom, frontStart, a);
            wallBack.position  = Vector3.Lerp(backFrom,  backStart,  a);

            yield return null;
        }

        wallLeft.position  = leftStart;
        wallRight.position = rightStart;
        wallFront.position = frontStart;
        wallBack.position  = backStart;

        // 3) Geri açılma bittikten sonra bekle
        yield return new WaitForSeconds(waitAfterReturn);

        // 4) Wall_Right'ı kaldır
        wallRight.gameObject.SetActive(false);
    }
}
