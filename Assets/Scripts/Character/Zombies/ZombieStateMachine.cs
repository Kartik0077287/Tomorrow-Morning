using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ZombieDetection))]
public class ZombieStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public ZombieDetection Detection { get; private set; }

    public ZombieState CurrentState { get; private set; }

    public ZombieIdleState IdleState { get; private set; }
    public ZombieWanderState WanderState { get; private set; }
    public ZombieChaseState ChaseState { get; private set; }

    [Header("Day / Night")]
    [SerializeField] private DayNightCycle dayNightCycle;

    [Header("Wander Speed")]
    [SerializeField] private float dayWanderSpeed = 1.5f;
    [SerializeField] private float nightWanderSpeed = 2.5f;

    [Header("Chase Speed")]
    [SerializeField] private float dayChaseSpeed = 2.5f;
    [SerializeField] private float nightChaseSpeed = 4.5f;

    public float WanderSpeed =>
        dayNightCycle.IsDay
            ? dayWanderSpeed
            : nightWanderSpeed;

    public float ChaseSpeed =>
        dayNightCycle.IsDay
            ? dayChaseSpeed
            : nightChaseSpeed;
    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Detection = GetComponent<ZombieDetection>();

        IdleState = new ZombieIdleState(this);
        WanderState = new ZombieWanderState(this);
        ChaseState = new ZombieChaseState(this);

        if (dayNightCycle == null)
        {
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();
        }
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
        if (CurrentState == newState)
            return;

        CurrentState?.Exit();

        CurrentState = newState;

        CurrentState.Enter();
    }
}