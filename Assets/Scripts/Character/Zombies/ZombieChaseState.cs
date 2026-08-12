using UnityEngine;

public class ZombieChaseState : ZombieState
{
    private Transform target;
    private Vector3 lastKnownPosition;
    private float lostPlayerTimer;
    private float nextAttackTime;

    private const float memoryDuration = 4f;
    private const float attackRange = 1.5f;
    private const float attackDamage = 10f;
    private const float attackCooldown = 1f;

    public ZombieChaseState(ZombieStateMachine zombie) : base(zombie)
    {
    }

    public override void Enter()
    {
        zombie.Agent.isStopped = false;
        zombie.Agent.speed = zombie.ChaseSpeed;
        lostPlayerTimer = 0f;
        nextAttackTime = 0f;

        if (zombie.Detection.CanSeePlayer)
        {
            target = zombie.Detection.DetectedPlayer;
            lastKnownPosition = target.position;
        }
    }

    public override void Update()
    {
        if (zombie.Detection.CanSeePlayer)
        {
            target = zombie.Detection.DetectedPlayer;
            lastKnownPosition = target.position;
            lostPlayerTimer = 0f;

            float distance = Vector3.Distance(zombie.transform.position, target.position);
            if (distance <= attackRange)
            {
                zombie.Agent.isStopped = true;
                AttackPlayer();
            }
            else
            {
                zombie.Agent.isStopped = false;
                zombie.Agent.SetDestination(target.position);
            }

            return;
        }

        zombie.Agent.isStopped = false;
        zombie.Agent.speed = zombie.ChaseSpeed;
        lostPlayerTimer += Time.deltaTime;
        zombie.Agent.SetDestination(lastKnownPosition);

        if (lostPlayerTimer >= memoryDuration)
            zombie.ChangeState(zombie.WanderState);
    }

    private void AttackPlayer()
    {
        if (Time.time < nextAttackTime || target == null)
            return;

        PlayerHealth health = target.GetComponentInParent<PlayerHealth>();
        if (health == null)
            return;

        health.TakeDamage(attackDamage);
        nextAttackTime = Time.time + attackCooldown;
    }

    public override void Exit()
    {
        target = null;
        zombie.Agent.isStopped = false;
    }
}
