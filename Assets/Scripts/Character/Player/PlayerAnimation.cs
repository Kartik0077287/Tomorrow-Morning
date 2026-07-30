using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float smoothSpeed = 10f;

    private static readonly int MoveSpeedHash =
        Animator.StringToHash("Move Speed");

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
        float current = animator.GetFloat(MoveSpeedHash);

        float smooth = Mathf.Lerp(
            current,
            targetMoveSpeed,
            smoothSpeed * Time.deltaTime
        );

        animator.SetFloat(MoveSpeedHash, smooth);
    }
}