using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float acceleration = 20f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundRadius = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private PlayerClimbing climbing;

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

        if (Input.GetButtonDown("Jump"))
            jumpPressed = true;

        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundRadius,
            groundLayer);
    }

    private void FixedUpdate()
    {
        if (climbing.IsClimbing || climbing.IsMantling)
            return;

        Move();

        if (jumpPressed)
        {
            Jump();
            jumpPressed = false;
        }
    }

    private void Move()
    {
        if (climbing.IsClimbing || climbing.IsMantling)
            return;

        float speed = Input.GetKey(KeyCode.LeftShift)
            ? sprintSpeed
            : walkSpeed;

        Vector3 move =
            transform.forward * vertical +
            transform.right * horizontal;

        move.Normalize();

        Vector3 targetVelocity = move * speed;

        Vector3 velocity = rb.linearVelocity;
        Vector3 velocityChange = targetVelocity - new Vector3(velocity.x, 0, velocity.z);

        rb.AddForce(velocityChange * acceleration, ForceMode.Acceleration);
    }

    private void Jump()
    {
        if (climbing.IsClimbing || climbing.IsMantling)
            return;

        if (!isGrounded)
            return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}