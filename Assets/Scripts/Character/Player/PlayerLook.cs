using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerBody;
    [SerializeField] private PlayerClimbing climbing;

    [Header("Mouse")]
    [SerializeField] private float sensitivity = 150f;

    [Header("Climbing Camera")]
    [SerializeField] private float climbYawLimit = 100f;

    private float xRotation;
    private float climbYaw;

    private bool wasClimbing;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            sensitivity *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            sensitivity *
            Time.deltaTime;

        bool climbingNow =
            climbing != null &&
            (climbing.IsClimbing || climbing.IsMantling);

        // Just started climbing
        if (climbingNow && !wasClimbing)
        {
            climbYaw = 0f;
        }

        // Vertical camera look
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);

        if (climbingNow)
        {
            // Camera can look left/right without rotating player.
            climbYaw += mouseX;

            climbYaw = Mathf.Clamp(
                climbYaw,
                -climbYawLimit,
                climbYawLimit
            );

            transform.localRotation =
                Quaternion.Euler(
                    xRotation,
                    climbYaw,
                    0f
                );
        }
        else
        {
            // Reset camera's local horizontal rotation.
            climbYaw = 0f;

            transform.localRotation =
                Quaternion.Euler(
                    xRotation,
                    0f,
                    0f
                );

            // Normal movement rotates the player.
            playerBody.Rotate(Vector3.up * mouseX);
        }

        wasClimbing = climbingNow;
    }
}