using UnityEngine;

public class ZombieIdleState : ZombieState
{
    private float idleTimer;
    private float idleDuration;

    public ZombieIdleState(ZombieStateMachine zombie)
        : base(zombie)
    {
    }

    public override void Enter()
    {
        zombie.Agent.isStopped = true;

        idleDuration = Random.Range(2f, 5f);
        idleTimer = 0f;
    }

    public override void Update()
    {
        // Player detected
        if (zombie.Detection.CanSeePlayer)
        {
            zombie.ChangeState(zombie.ChaseState);
            return;
        }

        idleTimer += Time.deltaTime;

        if (idleTimer >= idleDuration)
        {
            zombie.ChangeState(zombie.WanderState);
        }
    }

    public override void Exit()
    {
    }
}