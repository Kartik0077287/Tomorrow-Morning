using UnityEngine;
using UnityEngine.AI;

public class ZombieWanderState : ZombieState
{
    private float wanderRadius = 10f;

    public ZombieWanderState(ZombieStateMachine zombie)
        : base(zombie)
    {
    }

    public override void Enter()
    {
        zombie.Agent.isStopped = false;

        Vector3 randomPoint = GetRandomNavMeshPoint();

        zombie.Agent.SetDestination(randomPoint);

        Debug.Log("Zombie entered Wander State");
    }

    public override void Update()
    {
        if (zombie.Agent.pathPending)
            return;

        if (zombie.Agent.remainingDistance <= zombie.Agent.stoppingDistance)
        {
            zombie.ChangeState(zombie.IdleState);
        }
    }

    public override void Exit()
    {
    }

    private Vector3 GetRandomNavMeshPoint()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection =
                Random.insideUnitSphere * wanderRadius;

            randomDirection += zombie.transform.position;

            if (NavMesh.SamplePosition(
                randomDirection,
                out NavMeshHit hit,
                wanderRadius,
                NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        return zombie.transform.position;
    }
}