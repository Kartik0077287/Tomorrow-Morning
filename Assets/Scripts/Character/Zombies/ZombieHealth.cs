using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float despawnDelay = 3f;

    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f || IsDead)
            return;

        CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);

        if (IsDead)
            StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        ZombieStateMachine stateMachine = GetComponent<ZombieStateMachine>();
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Collider zombieCollider = GetComponent<Collider>();

        if (stateMachine != null)
        {
            stateMachine.enabled = false;
            stateMachine.TriggerDeath();
        }

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.enabled = false;
        }

        if (zombieCollider != null)
            zombieCollider.enabled = false;

        yield return new WaitForSeconds(despawnDelay);
        Destroy(gameObject);
    }
}
