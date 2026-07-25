using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }

    public ZombieState CurrentState { get; private set; }

    public ZombieIdleState IdleState { get; private set; }
    public ZombieWanderState WanderState { get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        IdleState = new ZombieIdleState(this);
        WanderState = new ZombieWanderState(this);
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    private void Update()
    {
        CurrentState?.Update();
    }

    public void ChangeState(ZombieState newState)
    {
        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState.Enter();
    }
}