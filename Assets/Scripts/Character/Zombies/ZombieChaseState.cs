using UnityEngine;

public class ZombieChaseState : ZombieState
{
    private Transform target;

    private Vector3 lastKnownPosition;

    private float lostPlayerTimer;

    private const float memoryDuration = 4f;
    private const float chaseSpeed = 4.5f;

    public ZombieChaseState(ZombieStateMachine zombie)
        : base(zombie)
    {
    }

    public override void Enter()
    {
        zombie.Agent.isStopped = false;
        zombie.Agent.speed = chaseSpeed;

        lostPlayerTimer = 0f;

        if (zombie.Detection.CanSeePlayer)
        {
            target = zombie.Detection.DetectedPlayer;
            lastKnownPosition = target.position;
        }

        Debug.Log("Zombie entered Chase State");
    }

    public override void Update()
    {
        if (zombie.Detection.CanSeePlayer)
        {
            target = zombie.Detection.DetectedPlayer;

            lastKnownPosition = target.position;

            lostPlayerTimer = 0f;

            zombie.Agent.SetDestination(target.position);

            return;
        }

        // Player currently isn't visible
        lostPlayerTimer += Time.deltaTime;

        zombie.Agent.SetDestination(lastKnownPosition);

        // Give up after memory expires
        if (lostPlayerTimer >= memoryDuration)
        {
            zombie.ChangeState(zombie.WanderState);
        }
    }

    public override void Exit()
    {
        target = null;

        // Return to normal wandering speed
        zombie.Agent.speed = 2.5f;
    }
}