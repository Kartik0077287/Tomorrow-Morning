using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Camera Distance")]
    [SerializeField] private float defaultDistance = 4f;
    [SerializeField] private float minimumDistance = 0.5f;

    [Header("Aim Camera")]
    [SerializeField] private float aimDistance = 2f;
    [SerializeField] private float aimShoulderOffset = 0.75f;
    [SerializeField] private float aimHeightOffset = 0.2f;

    [Header("Collision")]
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.15f;
    [SerializeField] private LayerMask collisionLayers;

    [Header("Smoothing")]
    [SerializeField] private float moveInSpeed = 20f;
    [SerializeField] private float moveOutSpeed = 8f;

    private float currentDistance;
    private Vector3 currentOffset;
    private bool isAiming;

    private void Start()
    {
        currentDistance = defaultDistance;
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }

    private void LateUpdate()
    {
        Vector3 direction = -cameraPivot.forward;
        float requestedDistance = isAiming ? aimDistance : defaultDistance;
        Vector3 requestedOffset = isAiming
            ? cameraPivot.right * aimShoulderOffset + cameraPivot.up * aimHeightOffset
            : Vector3.zero;

        float targetDistance = requestedDistance;
        Vector3 castOrigin = cameraPivot.position + requestedOffset;

        if (Physics.SphereCast(
            castOrigin,
            cameraRadius,
            direction,
            out RaycastHit hit,
            requestedDistance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            targetDistance = Mathf.Clamp(
                hit.distance - collisionOffset,
                minimumDistance,
                requestedDistance);
        }

        float speed = targetDistance < currentDistance ? moveInSpeed : moveOutSpeed;
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, speed * Time.deltaTime);
        currentOffset = Vector3.Lerp(currentOffset, requestedOffset, moveInSpeed * Time.deltaTime);
        transform.position = cameraPivot.position + currentOffset + direction * currentDistance;
    }
}
