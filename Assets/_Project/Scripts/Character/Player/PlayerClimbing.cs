using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerClimbing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform climbCheck;

    [Header("Climb Detection")]
    [SerializeField] private LayerMask climbableLayer;
    [SerializeField] private float detectionDistance = 0.8f;

    [Header("Climbing")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private float wallDistance = 0.55f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float snapSpeed = 10f;
    [SerializeField] private PlayerStamina stamina;

    [Header("Ledge Detection")]
    [SerializeField] private float ledgeCheckHeight = 1.5f;
    [SerializeField] private float ledgeForwardDistance = 0.8f;
    [SerializeField] private float ledgeDownDistance = 2f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Mantle")]
    [SerializeField] private float mantleForwardOffset = 0.6f;
    [SerializeField] private float mantleHeightOffset = 0.05f;
    [SerializeField] private float mantleDuration = 0.5f;

    private Rigidbody rb;

    private bool isClimbing;
    private bool isMantling;

    private RaycastHit wallHit;

    public bool IsClimbing => isClimbing;
    public bool IsMantling => isMantling;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isMantling)
            return;

        bool hasWall = CheckForWall();

        if (!isClimbing)
        {
            if (Input.GetKeyDown(KeyCode.Space) && hasWall)
            {
                StartClimbing();
            }

            return;
        }

        // Check for the top before stopping the climb.
        if (Input.GetAxisRaw("Vertical") > 0f &&
            TryFindLedge(out Vector3 mantlePosition))
        {
            StartCoroutine(Mantle(mantlePosition));
            return;
        }

        if (!hasWall)
        {
            StopClimbing();
        }
    }

    private void FixedUpdate()
    {
        if (!isClimbing || isMantling)
            return;

        HandleClimbing();
        AlignWithWall();
    }

    private bool CheckForWall()
    {
        return Physics.Raycast(
            climbCheck.position,
            transform.forward,
            out wallHit,
            detectionDistance,
            climbableLayer,
            QueryTriggerInteraction.Ignore
        );
    }

    private void StartClimbing()
    {
        isClimbing = true;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        // Remove any existing spin.
        rb.angularVelocity = Vector3.zero;
    }

    private void StopClimbing()
    {
        isClimbing = false;

        rb.useGravity = true;
    }

    private void HandleClimbing()
    {
        if (!stamina.HasStamina)
        {
            StopClimbing();
            return;
        }

        stamina.DrainClimb();

        float verticalInput = Input.GetAxisRaw("Vertical");

        rb.linearVelocity =
            Vector3.up * verticalInput * climbSpeed;
    }

    private void AlignWithWall()
    {
        // Face toward the wall.
        Vector3 directionTowardWall = -wallHit.normal;
        directionTowardWall.y = 0f;

        if (directionTowardWall.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(directionTowardWall);

            Quaternion smoothRotation =
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );

            rb.MoveRotation(smoothRotation);
        }

        // Keep the player a consistent distance from the wall.
        Vector3 desiredPosition =
            wallHit.point + wallHit.normal * wallDistance;

        desiredPosition.y = rb.position.y;

        Vector3 smoothPosition =
            Vector3.Lerp(
                rb.position,
                desiredPosition,
                snapSpeed * Time.fixedDeltaTime
            );

        rb.MovePosition(smoothPosition);
    }

    private bool TryFindLedge(out Vector3 mantlePosition)
    {
        mantlePosition = Vector3.zero;

        /*
         * First check:
         *
         * Is there still a wall above us?
         *
         * If there is, we're not at the top yet.
         */

        Vector3 upperOrigin =
            climbCheck.position +
            Vector3.up * ledgeCheckHeight;

        bool wallAbove = Physics.Raycast(
            upperOrigin,
            transform.forward,
            ledgeForwardDistance,
            climbableLayer,
            QueryTriggerInteraction.Ignore
        );

        if (wallAbove)
            return false;

        /*
         * There is no wall above.
         * Now cast downward from above and slightly
         * beyond the wall to find the top surface.
         */

        Vector3 downOrigin =
            upperOrigin +
            transform.forward * ledgeForwardDistance;

        if (!Physics.Raycast(
            downOrigin,
            Vector3.down,
            out RaycastHit topHit,
            ledgeDownDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        mantlePosition =
            topHit.point +
            transform.forward * mantleForwardOffset +
            Vector3.up * mantleHeightOffset;

        return true;
    }

    private IEnumerator Mantle(Vector3 targetPosition)
    {
        isMantling = true;
        isClimbing = false;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Vector3 startPosition = rb.position;

        float timer = 0f;

        while (timer < mantleDuration)
        {
            timer += Time.fixedDeltaTime;

            float t = Mathf.Clamp01(
                timer / mantleDuration
            );

            // Smooth acceleration/deceleration.
            t = t * t * (3f - 2f * t);

            Vector3 position =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

            rb.MovePosition(position);

            yield return new WaitForFixedUpdate();
        }

        rb.position = targetPosition;
        rb.linearVelocity = Vector3.zero;

        rb.useGravity = true;

        isMantling = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (climbCheck == null)
            return;

        // Main wall detection.
        Gizmos.DrawLine(
            climbCheck.position,
            climbCheck.position +
            transform.forward * detectionDistance
        );

        // Upper ledge check.
        Vector3 upperOrigin =
            climbCheck.position +
            Vector3.up * ledgeCheckHeight;

        Gizmos.DrawLine(
            upperOrigin,
            upperOrigin +
            transform.forward * ledgeForwardDistance
        );

        // Downward top-surface check.
        Vector3 downOrigin =
            upperOrigin +
            transform.forward * ledgeForwardDistance;

        Gizmos.DrawLine(
            downOrigin,
            downOrigin +
            Vector3.down * ledgeDownDistance
        );
    }
}