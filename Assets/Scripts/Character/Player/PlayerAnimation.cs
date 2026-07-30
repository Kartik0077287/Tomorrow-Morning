using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerClimbing climbing;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private static readonly int MoveSpeedHash =
        Animator.StringToHash("Move Speed");

    private static readonly int IsGroundedHash =
        Animator.StringToHash("Is Grounded");

    private static readonly int IsClimbingHash =
        Animator.StringToHash("Is Climbing");

    private static readonly int ClimbSpeedHash =
        Animator.StringToHash("Climb Speed");

    private float targetMoveSpeed;

    public void SetMovement(bool isMoving, bool isSprinting)
    {
        if (!isMoving)
            targetMoveSpeed = 0f;
        else if (isSprinting)
            targetMoveSpeed = 1f;
        else
            targetMoveSpeed = 0.5f;
    }

    private void Update()
    {
        UpdateMovement();
        UpdateClimbing();
    }

    private void UpdateMovement()
    {
        float current =
            animator.GetFloat(MoveSpeedHash);

        float smooth =
            Mathf.Lerp(
                current,
                targetMoveSpeed,
                smoothSpeed * Time.deltaTime
            );

        animator.SetFloat(MoveSpeedHash, smooth);
    }

    private void UpdateClimbing()
    {
        animator.SetBool(
            IsClimbingHash,
            climbing.IsClimbing
        );

        if (climbing.IsClimbing)
        {
            float verticalInput =
                Input.GetAxisRaw("Vertical");

            animator.SetFloat(
                ClimbSpeedHash,
                verticalInput
            );
        }
        else
        {
            animator.SetFloat(
                ClimbSpeedHash,
                0f
            );
        }
    }

    public void SetGrounded(bool grounded)
    {
        animator.SetBool(
            IsGroundedHash,
            grounded
        );
    }
}