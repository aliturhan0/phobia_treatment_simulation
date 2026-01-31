using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class SpiderRoam_PhysicsAvoid : MonoBehaviour
{
    [Header("Roam Area")]
    public Transform roomCenter;
    public float roamRadius = 4f;

    [Header("Move")]
    public float moveSpeed = 0.6f;
    public float turnSpeed = 360f;
    public float minChangeTime = 0.8f;
    public float maxChangeTime = 2.0f;

    [Header("Obstacle Avoidance")]
    public float forwardCheck = 0.35f;
    public float castRadius = 0.12f;
    public LayerMask obstacleMask;
    public Vector2 turnAngleRange = new Vector2(90f, 150f);

    [Header("Ground (only while roaming)")]
    public LayerMask groundMask;
    public float groundRayDistance = 2f;
    public float groundOffset = 0.02f;

    [Header("Physics")]
    public bool freezeXZRotation = true;

    private Rigidbody rb;
    private Vector3 centerPos;
    private Vector3 moveDir;
    private float changeTimer;

    private bool roaming = true; // ✅ vurulunca false yapacağız

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Dynamic + gravity
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (freezeXZRotation)
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        centerPos = roomCenter ? roomCenter.position : transform.position;

        PickNewDirection();
    }

    void FixedUpdate()
    {
        if (!roaming) return; // ✅ savrulurken hiçbir şey zorlamasın

        changeTimer -= Time.fixedDeltaTime;
        if (changeTimer <= 0f) PickNewDirection();

        if (IsObstacleAhead()) TurnAway();

        // Oda sınırı
        Vector3 flatPos = new Vector3(rb.position.x, centerPos.y, rb.position.z);
        Vector3 flatCenter = new Vector3(centerPos.x, centerPos.y, centerPos.z);
        if (Vector3.Distance(flatPos, flatCenter) > roamRadius)
        {
            Vector3 toCenter = (flatCenter - flatPos).normalized;
            moveDir = Vector3.Slerp(moveDir, toCenter, 0.15f).normalized;
        }

        StickToGround(); // ✅ sadece roaming modunda

        Vector3 step = moveDir * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + step);

        if (moveDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }
    }

    private void PickNewDirection()
    {
        changeTimer = Random.Range(minChangeTime, maxChangeTime);
        Vector2 r = Random.insideUnitCircle;
        if (r.sqrMagnitude < 0.0001f) r = Vector2.right;
        r.Normalize();
        moveDir = new Vector3(r.x, 0f, r.y).normalized;
    }

    private bool IsObstacleAhead()
    {
        Vector3 origin = rb.position + Vector3.up * 0.1f;
        Vector3 dir = moveDir.sqrMagnitude > 0.0001f ? moveDir : transform.forward;

        int mask = obstacleMask.value == 0 ? ~0 : obstacleMask.value;

        if (Physics.SphereCast(origin, castRadius, dir, out RaycastHit hit, forwardCheck, mask, QueryTriggerInteraction.Ignore))
        {
            if (((1 << hit.collider.gameObject.layer) & groundMask.value) != 0) return false;
            if (hit.collider.attachedRigidbody == rb) return false;
            return true;
        }
        return false;
    }

    private void TurnAway()
    {
        float sign = Random.value < 0.5f ? -1f : 1f;
        float angle = Random.Range(turnAngleRange.x, turnAngleRange.y);
        Vector3 newDir = Quaternion.Euler(0f, sign * angle, 0f) * moveDir;
        moveDir = new Vector3(newDir.x, 0f, newDir.z).normalized;
        changeTimer = Random.Range(0.3f, 0.8f);
    }

    private void StickToGround()
    {
        if (groundMask.value == 0) return;

        Vector3 rayOrigin = rb.position + Vector3.up * 0.6f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRayDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            // ✅ Y'yi "rb.position =" ile zıplatmak yerine MovePosition ile yumuşat
            Vector3 p = rb.position;
            p.y = hit.point.y + groundOffset;
            rb.MovePosition(p);
        }
    }

    // ✅ Sopayla vurunca çağır
    public void EnablePhysicsOnHit()
    {
        roaming = false;

        // Yere yapıştırma/MovePosition etkileri tamamen dursun.
        // Fizik doğal çalışsın.
        rb.constraints = RigidbodyConstraints.None; // istersek dönsün
        // Takla istemiyorsan şu satırı kullan:
        // rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
}
