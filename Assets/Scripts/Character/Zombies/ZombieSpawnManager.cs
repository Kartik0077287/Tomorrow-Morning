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

    [Header("Day Spawn Settings")]
    [SerializeField] private int dayMaxZombies = 8;
    [SerializeField] private float daySpawnInterval = 10f;

    [Header("Night Spawn Settings")]
    [SerializeField] private int nightMaxZombies = 20;
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
    private int currentZombieCount;

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
        if (currentZombieCount >= CurrentMaxZombies)
            return;

        for (int i = 0; i < attemptsPerSpawn; i++)
        {
            Vector3 candidate = GetRandomPosition();

            // Find nearest valid NavMesh position
            if (!NavMesh.SamplePosition(
                candidate,
                out NavMeshHit navHit,
                navMeshSampleRadius,
                NavMesh.AllAreas))
            {
                continue;
            }

            Vector3 spawnPosition = navHit.position;

            // Make sure NavMesh sampling didn't move us
            // inside the minimum player radius.
            float distanceFromPlayer =
                Vector3.Distance(player.position, spawnPosition);

            if (distanceFromPlayer < minSpawnDistance ||
                distanceFromPlayer > maxSpawnDistance)
            {
                continue;
            }

            // Player must not be able to see the spawn.
            if (IsVisibleToPlayer(spawnPosition))
                continue;

            // During daytime the position must be shadowed.
            if (dayNightCycle.IsDay && !IsInShadow(spawnPosition))
                continue;

            SpawnZombie(spawnPosition);
            return;
        }
    }
    private int CurrentMaxZombies =>
    dayNightCycle.IsDay
        ? dayMaxZombies
        : nightMaxZombies;

    private float CurrentSpawnInterval =>
        dayNightCycle.IsDay
            ? daySpawnInterval
            : nightSpawnInterval;
    private Vector3 GetRandomPosition()
    {
        Vector2 direction = Random.insideUnitCircle.normalized;

        float distance = Random.Range(
            minSpawnDistance,
            maxSpawnDistance);

        return player.position +
               new Vector3(
                   direction.x,
                   0f,
                   direction.y) * distance;
    }

    private bool IsVisibleToPlayer(Vector3 position)
    {
        Vector3 viewportPoint =
            playerCamera.WorldToViewportPoint(
                position + Vector3.up);

        bool insideCamera =
            viewportPoint.z > 0f &&
            viewportPoint.x > 0f &&
            viewportPoint.x < 1f &&
            viewportPoint.y > 0f &&
            viewportPoint.y < 1f;

        // Not even inside the camera view.
        if (!insideCamera)
            return false;

        Vector3 cameraPosition =
            playerCamera.transform.position;

        Vector3 target =
            position + Vector3.up;

        Vector3 direction =
            target - cameraPosition;

        float distance = direction.magnitude;

        // Something blocks the camera's view.
        if (Physics.Raycast(
            cameraPosition,
            direction.normalized,
            distance,
            visibilityBlockingLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        // Inside camera AND nothing blocks it.
        return true;
    }

    private bool IsInShadow(Vector3 position)
    {
        if (sun == null)
            return false;

        Vector3 origin =
            position + Vector3.up * 0.5f;

        // Directional light rays travel along sun.forward.
        // We check toward the light source.
        Vector3 directionToSun =
            -sun.transform.forward;

        return Physics.Raycast(
            origin,
            directionToSun,
            sunlightCheckDistance,
            shadowBlockingLayers,
            QueryTriggerInteraction.Ignore);
    }

    private void SpawnZombie(Vector3 position)
    {
        GameObject zombie = Instantiate(
            zombiePrefab,
            position,
            Quaternion.Euler(
                0f,
                Random.Range(0f, 360f),
                0f));

        currentZombieCount++;

        ZombieSpawnTracker tracker =
            zombie.AddComponent<ZombieSpawnTracker>();

        tracker.Initialize(this);
    }

    public void NotifyZombieDestroyed()
    {
        currentZombieCount =
            Mathf.Max(0, currentZombieCount - 1);
    }
}