using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ZombieDetection))]
[RequireComponent(typeof(Animator))]
public class ZombieStateMachine : MonoBehaviour
{
    public NavMeshAgent Agent { get; private set; }
    public ZombieDetection Detection { get; private set; }
    public Animator Animator { get; private set; }

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

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");

    public float WanderSpeed => dayNightCycle.IsDay ? dayWanderSpeed : nightWanderSpeed;
    public float ChaseSpeed => dayNightCycle.IsDay ? dayChaseSpeed : nightChaseSpeed;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Detection = GetComponent<ZombieDetection>();
        Animator = GetComponent<Animator>();

        IdleState = new ZombieIdleState(this);
        WanderState = new ZombieWanderState(this);
        ChaseState = new ZombieChaseState(this);

        if (dayNightCycle == null)
            dayNightCycle = FindFirstObjectByType<DayNightCycle>();
    }

    private void Start()
    {
        ChangeState(IdleState);
    }

    private void Update()
    {
        CurrentState?.Update();

        float animationSpeed = 0f;
        if (Agent.enabled && !Agent.isStopped)
        {
            float velocity = Agent.velocity.magnitude;
            animationSpeed = velocity < 0.1f ? 0f : velocity <= WanderSpeed + 0.1f ? 1f : 2f;
        }

        Animator.SetFloat(SpeedHash, animationSpeed, 0.1f, Time.deltaTime);
    }

    public void ChangeState(ZombieState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    public void TriggerAttack()
    {
        Animator.SetTrigger(AttackHash);
    }

    public void TriggerDeath()
    {
        Animator.SetTrigger(DieHash);
    }
}
