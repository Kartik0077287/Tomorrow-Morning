using UnityEngine;
using UnityEngine.AI;

public class ZombieSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject zombiePrefab;
    [SerializeField] private Transform player;
    [SerializeField] private Camera playerCamera;

    [Header("Spawn Distance")]
    [SerializeField] private float minSpawnDistance = 50f;
    [SerializeField] private float maxSpawnDistance = 100f;

    [Header("Spawn Rate")]
    [SerializeField] private float daySpawnInterval = 10f;
    [SerializeField] private float nightSpawnInterval = 4f;
    [SerializeField] private int attemptsPerSpawn = 15;

    [Header("Navigation")]
    [SerializeField] private float navMeshSampleRadius = 5f;

    [Header("Visibility")]
    [SerializeField] private LayerMask visibilityBlockingLayers;

    [Header("Day / Night")]
    [SerializeField] private DayNightCycle dayNightCycle;

    [Header("Shadow Detection")]
    [SerializeField] private Light sun;
    [SerializeField] private LayerMask shadowBlockingLayers;
    [SerializeField] private float sunlightCheckDistance = 500f;

    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer < CurrentSpawnInterval)
            return;

        spawnTimer = 0f;
        TrySpawnZombie();
    }

    private void TrySpawnZombie()
    {
        for (int i = 0; i < attemptsPerSpawn; i++)
        {
            Vector3 candidate = GetRandomPosition();

            if (!NavMesh.SamplePosition(
                candidate,
                out NavMeshHit navHit,
                navMeshSampleRadius,
                NavMesh.AllAreas))
            {
                continue;
            }

            Vector3 spawnPosition = navHit.position;
            float distanceFromPlayer = Vector3.Distance(player.position, spawnPosition);

            if (distanceFromPlayer < minSpawnDistance ||
                distanceFromPlayer > maxSpawnDistance)
            {
                continue;
            }

            if (IsVisibleToPlayer(spawnPosition))
                continue;

            if (dayNightCycle.IsDay && !IsInShadow(spawnPosition))
                continue;

            SpawnZombie(spawnPosition);
            return;
        }
    }

    private float CurrentSpawnInterval =>
        dayNightCycle.IsDay
            ? daySpawnInterval
            : nightSpawnInterval;

    private Vector3 GetRandomPosition()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;
        float distance = Random.Range(minSpawnDistance, maxSpawnDistance);

        return player.position +
               new Vector3(direction.x, 0f, direction.y) * distance;
    }

    private bool IsVisibleToPlayer(Vector3 position)
    {
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(position + Vector3.up);

        bool insideCamera =
            viewportPoint.z > 0f &&
            viewportPoint.x > 0f &&
            viewportPoint.x < 1f &&
            viewportPoint.y > 0f &&
            viewportPoint.y < 1f;

        if (!insideCamera)
            return false;

        Vector3 cameraPosition = playerCamera.transform.position;
        Vector3 target = position + Vector3.up;
        Vector3 direction = target - cameraPosition;
        float distance = direction.magnitude;

        if (Physics.Raycast(
            cameraPosition,
            direction.normalized,
            distance,
            visibilityBlockingLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    private bool IsInShadow(Vector3 position)
    {
        if (sun == null)
            return false;

        Vector3 origin = position + Vector3.up * 0.5f;
        Vector3 directionToSun = -sun.transform.forward;

        return Physics.Raycast(
            origin,
            directionToSun,
            sunlightCheckDistance,
            shadowBlockingLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void SpawnZombie(Vector3 position)
    {
        Instantiate(
            zombiePrefab,
            position,
            Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
    }

    public void NotifyZombieDestroyed()
    {
    }
}
