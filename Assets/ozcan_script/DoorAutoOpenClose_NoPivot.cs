using System.Collections;
using UnityEngine;

public class DoorAutoOpenClose_NoPivot : MonoBehaviour
{
    [Header("Door Settings")]
    public float openAngle = 90f;          // Ters açılırsa -90 yap
    public float rotateSpeed = 120f;       // Derece/saniye
    public float holdOpenSeconds = 1.0f;   // Açık kalma süresi
    public string playerTag = "Player";

    [Header("Room Shrink")]
    public RoomShrinkController room;      // Duvarları daraltacak script

    private bool busy = false;
    private Quaternion closedRot;
    private Quaternion openRot;

    void Awake()
    {
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (busy) return;
        if (!other.CompareTag(playerTag)) return;

        StartCoroutine(OpenThenClose());
    }

    IEnumerator OpenThenClose()
    {
        busy = true;

        // Kapıyı aç
        yield return RotateTo(openRot);

        // Açık bekle
        yield return new WaitForSeconds(holdOpenSeconds);

        // Kapıyı kapat
        yield return RotateTo(closedRot);

        // Kapı kapandıktan sonra 5 saniye bekle
        yield return new WaitForSeconds(5f);

        // Duvarları daralt
        if (room != null)
            room.StartShrinking();

        busy = false;
    }

    IEnumerator RotateTo(Quaternion target)
    {
        while (Quaternion.Angle(transform.localRotation, target) > 0.5f)
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                target,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.localRotation = target;
    }
}
