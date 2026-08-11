using UnityEngine;

public class ZombieDetection : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float closeDetectionRadius = 2f;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 120f;

    [Header("Layers")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Eye")]
    [SerializeField] private Transform eyePoint;

    public Transform DetectedPlayer { get; private set; }

    public bool CanSeePlayer => DetectedPlayer != null;

    private void Update()
    {
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        DetectedPlayer = null;

        Collider[] players = Physics.OverlapSphere(
            transform.position,
            detectionRadius,
            playerLayer);

        if (players.Length == 0)
            return;

        Transform player = players[0].transform;


        float distanceToPlayer = Vector3.Distance(
            transform.position,
            player.position);

        // Close detection ignores field of view
        if (distanceToPlayer <= closeDetectionRadius)
        {
            DetectedPlayer = player;
            return;
        }

        Vector3 targetPosition = player.position + Vector3.up;

        Vector3 directionToPlayer =
            (targetPosition - eyePoint.position).normalized;

        // Check FOV
        float angle = Vector3.Angle(
            transform.forward,
            directionToPlayer);

        if (angle > viewAngle * 0.5f)
            return;

        // Check line of sight
        float distance = Vector3.Distance(
            eyePoint.position,
            targetPosition);

        if (!Physics.Raycast(
            eyePoint.position,
            directionToPlayer,
            distance,
            obstacleLayer))
        {
            DetectedPlayer = player;

            Debug.Log("PLAYER DETECTED");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Vector3 leftBoundary =
            DirectionFromAngle(-viewAngle / 2f);

        Vector3 rightBoundary =
            DirectionFromAngle(viewAngle / 2f);

        Gizmos.DrawLine(
            transform.position,
            transform.position + leftBoundary * detectionRadius);

        Gizmos.DrawLine(
            transform.position,
            transform.position + rightBoundary * detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeDetectionRadius);
    }

    private Vector3 DirectionFromAngle(float angle)
    {
        angle += transform.eulerAngles.y;

        return new Vector3(
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0,
            Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}