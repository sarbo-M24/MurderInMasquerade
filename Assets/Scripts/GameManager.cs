using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton for easy access

    [Header("Characters")]
    public GameObject npcPrefab;
    public GameObject imposterPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPointsParent;

    // We store all points, and a separate list for the empty ones
    private List<Transform> allSpawnPoints = new List<Transform>();
    private List<Transform> emptySpawnPoints = new List<Transform>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CollectSpawnPoints();
        SpawnCharacters();
    }

    void CollectSpawnPoints()
    {
        foreach (Transform child in spawnPointsParent)
        {
            allSpawnPoints.Add(child);
        }
    }

    void SpawnCharacters()
    {
        // 1. Shuffle
        ShuffleSpawnPoints();

        int totalPoints = allSpawnPoints.Count;
        int npcCount = Mathf.Max(0, totalPoints - 4);

        // 2. Spawn NPCs (They take the first batch of spots)
        for (int i = 0; i < npcCount; i++)
        {
            Instantiate(npcPrefab, allSpawnPoints[i].position, Quaternion.identity);
        }

        // 3. Spawn Imposter (Takes the next spot)
        if (npcCount < totalPoints)
        {
            GameObject imposter = Instantiate(imposterPrefab, allSpawnPoints[npcCount].position, Quaternion.identity);
            
            // Tell the imposter which spawn point he is currently standing on
            ImposterController controller = imposter.GetComponent<ImposterController>();
            if (controller != null) 
            {
                controller.currentSpawnPoint = allSpawnPoints[npcCount];
            }
        }

        // 4. Register Empty Spots (The rest of the list)
        // We start loop from npcCount + 1 (because npcCount was the Imposter's spot)
        for (int i = npcCount + 1; i < totalPoints; i++)
        {
            emptySpawnPoints.Add(allSpawnPoints[i]);
        }
    }

    // --- NEW PUBLIC METHOD FOR IMPOSTER ---
    public Transform GetNewTeleportSpot(Transform currentSpotToFree)
    {
        if (emptySpawnPoints.Count == 0) return null;

        // 1. Pick a random empty spot
        int randomIndex = Random.Range(0, emptySpawnPoints.Count);
        Transform newSpot = emptySpawnPoints[randomIndex];

        // 2. The Imposter's OLD spot is now empty, add it to the empty list
        emptySpawnPoints.Add(currentSpotToFree);

        // 3. The NEW spot is now taken, remove it from the empty list
        emptySpawnPoints.RemoveAt(randomIndex);

        return newSpot;
    }

    void ShuffleSpawnPoints()
    {
        for (int i = 0; i < allSpawnPoints.Count; i++)
        {
            Transform temp = allSpawnPoints[i];
            int randomIndex = Random.Range(i, allSpawnPoints.Count);
            allSpawnPoints[i] = allSpawnPoints[randomIndex];
            allSpawnPoints[randomIndex] = temp;
        }
    }
}