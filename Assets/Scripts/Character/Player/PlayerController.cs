using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("References")]
    [SerializeField] private PlayerStamina stamina;
    [SerializeField] private PlayerAnimation playerAnimation;

    private Rigidbody rb;

    private float horizontal;
    private float vertical;

    private bool jumpPressed;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        playerAnimation.SetGrounded(isGrounded);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPressed = true;
            playerAnimation.StartJump(jumpForce);
        }
    }

    private void FixedUpdate()
    {
        Move();

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
    }

    private void Move()
    {
        bool isMoving =
            Mathf.Abs(horizontal) > 0.1f ||
            Mathf.Abs(vertical) > 0.1f;

        bool isSprinting =
            Input.GetKey(KeyCode.LeftShift) &&
            isMoving &&
            stamina.HasStamina;

        playerAnimation.SetMovement(
            isMoving,
            isSprinting
        );

        float speed = walkSpeed;

        if (isSprinting)
        {
            speed = sprintSpeed;
            stamina.DrainSprint();
        }

        // --------------------------------
        // CAMERA-RELATIVE MOVEMENT
        // --------------------------------

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        // Remove camera's vertical tilt.
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        // --------------------------------
        // ROTATE PLAYER TOWARD MOVEMENT
        // --------------------------------

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            Quaternion newRotation =
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );

            rb.MoveRotation(newRotation);
        }

        // --------------------------------
        // MOVEMENT
        // --------------------------------

        Vector3 targetVelocity =
            moveDirection * speed;

        Vector3 velocity =
            rb.linearVelocity;

        Vector3 velocityChange =
            targetVelocity -
            new Vector3(
                velocity.x,
                0f,
                velocity.z
            );

        rb.AddForce(
            velocityChange * acceleration,
            ForceMode.Acceleration
        );
    }

    private void Jump()
    {
        rb.AddForce(
            Vector3.up * jumpForce,
            ForceMode.Impulse
        );
    }
}