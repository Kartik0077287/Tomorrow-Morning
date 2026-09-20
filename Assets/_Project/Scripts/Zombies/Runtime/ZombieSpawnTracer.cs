using UnityEngine;

public class ZombieSpawnTracker : MonoBehaviour
{
    private ZombieSpawnManager spawnManager;

    public void Initialize(ZombieSpawnManager manager)
    {
        spawnManager = manager;
    }

    private void OnDestroy()
    {
        if (spawnManager != null)
        {
            spawnManager.NotifyZombieDestroyed();
        }
    }
}