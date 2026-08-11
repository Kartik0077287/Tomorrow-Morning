using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraPivot;

    [Header("Camera Distance")]
    [SerializeField] private float defaultDistance = 4f;
    [SerializeField] private float minimumDistance = 0.5f;

    [Header("Collision")]
    [SerializeField] private float cameraRadius = 0.3f;
    [SerializeField] private float collisionOffset = 0.15f;
    [SerializeField] private LayerMask collisionLayers;

    [Header("Smoothing")]
    [SerializeField] private float moveInSpeed = 20f;
    [SerializeField] private float moveOutSpeed = 8f;

    private float currentDistance;

    private void Start()
    {
        currentDistance = defaultDistance;
    }

    private void LateUpdate()
    {
        Vector3 direction = -cameraPivot.forward;

        float targetDistance = defaultDistance;

        if (Physics.SphereCast(
            cameraPivot.position,
            cameraRadius,
            direction,
            out RaycastHit hit,
            defaultDistance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            targetDistance = hit.distance - collisionOffset;

            targetDistance = Mathf.Clamp(
                targetDistance,
                minimumDistance,
                defaultDistance
            );
        }

        float speed = targetDistance < currentDistance
            ? moveInSpeed
            : moveOutSpeed;

        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            speed * Time.deltaTime
        );

        transform.position =
            cameraPivot.position +
            direction * currentDistance;
    }
}