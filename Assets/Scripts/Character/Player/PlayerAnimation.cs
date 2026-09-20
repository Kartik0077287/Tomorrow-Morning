using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody playerRigidbody;

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 10f;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int VerticalVelocityHash = Animator.StringToHash("VerticalVelocity");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int KnifeInwardHash = Animator.StringToHash("KnifeInward");
    private static readonly int KnifeOutwardHash = Animator.StringToHash("KnifeOutward");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");
    private static readonly int ReloadHash = Animator.StringToHash("Reload");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    private float targetMoveSpeed;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();
    }

    public void SetMovement(bool isMoving, bool isSprinting)
    {
        targetMoveSpeed = !isMoving ? 0f : isSprinting ? 1f : 0.5f;
    }

    private void Update()
    {
        float smooth = Mathf.Lerp(animator.GetFloat(SpeedHash), targetMoveSpeed, smoothSpeed * Time.deltaTime);
        animator.SetFloat(SpeedHash, smooth);
        animator.SetFloat(VerticalVelocityHash, playerRigidbody != null ? playerRigidbody.linearVelocity.y : 0f);
    }

    public void SetGrounded(bool grounded)
    {
        animator.SetBool(IsGroundedHash, grounded);
    }

    public void StartJump(float initialVerticalVelocity)
    {
        animator.SetBool(IsGroundedHash, false);
        animator.SetFloat(VerticalVelocityHash, initialVerticalVelocity);
        animator.CrossFadeInFixedTime("Jump", 0.02f);
    }

    public void TriggerKnifeInward() => animator.SetTrigger(KnifeInwardHash);
    public void TriggerKnifeOutward() => animator.SetTrigger(KnifeOutwardHash);
    public void TriggerShoot() => animator.SetTrigger(ShootHash);
    public void TriggerReload() => animator.SetTrigger(ReloadHash);
    public void TriggerDeath() => animator.SetTrigger(DeathHash);
}
