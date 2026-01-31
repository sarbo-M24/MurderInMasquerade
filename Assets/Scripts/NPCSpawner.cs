using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject[] npcPrefabs;
    [SerializeField] private Transform checkpointsParent;

    [Header("Spawn Options")]
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool destroyCheckpointsAfterSpawn = false;

    private Transform[] spawnPoints;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnNPCs();
        }
    }

    public void SpawnNPCs()
    {
        if (checkpointsParent == null)
        {
            Debug.LogError("Checkpoints Parent is not assigned!");
            return;
        }

        if (npcPrefabs == null || npcPrefabs.Length == 0)
        {
            Debug.LogError("No NPC prefabs assigned to spawn!");
            return;
        }

        // Get all child transforms from the parent
        int childCount = checkpointsParent.childCount;

        if (childCount == 0)
        {
            Debug.LogWarning("No checkpoint children found under the parent object!");
            return;
        }

        spawnPoints = new Transform[childCount];

        for (int i = 0; i < childCount; i++)
        {
            spawnPoints[i] = checkpointsParent.GetChild(i);
        }

        // Spawn random prefabs at each checkpoint position
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject randomPrefab = npcPrefabs[Random.Range(0, npcPrefabs.Length)];

            GameObject spawnedNPC = Instantiate(
                randomPrefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            // Optional: Parent the spawned NPC to this spawner or keep it in the scene root
            // spawnedNPC.transform.SetParent(transform);
        }

        if (destroyCheckpointsAfterSpawn)
        {
            // Destroy checkpoint GameObjects after spawning
            foreach (Transform spawnPoint in spawnPoints)
            {
                Destroy(spawnPoint.gameObject);
            }
        }

        Debug.Log($"Successfully spawned {spawnPoints.Length} NPCs!");
    }

    // Optional: Call this method to clear all spawned NPCs
    public void ClearSpawnedNPCs()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
