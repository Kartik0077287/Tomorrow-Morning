using UnityEngine;

[RequireComponent(typeof(PlayerAnimation))]
public class PlayerCombat : MonoBehaviour
{
    [Header("Knife Combo")]
    [SerializeField] private float comboResetTime = 0.8f;
    [SerializeField] private float minimumAttackInterval = 0.15f;

    private PlayerAnimation playerAnimation;
    private bool nextAttackIsInward = true;
    private float lastAttackTime = float.NegativeInfinity;

    private void Awake()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (Time.time - lastAttackTime > comboResetTime)
            nextAttackIsInward = true;

        if (Input.GetMouseButtonDown(0))
            TryAttack();
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime < minimumAttackInterval)
            return;

        if (nextAttackIsInward)
            playerAnimation.TriggerKnifeInward();
        else
            playerAnimation.TriggerKnifeOutward();

        nextAttackIsInward = !nextAttackIsInward;
        lastAttackTime = Time.time;
    }
}
