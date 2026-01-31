using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Characters")]
    [Tooltip("Drag NPC1, NPC2, and NPC3 here")]
    public GameObject[] npcPrefabs; // CHANGED: Now an array of prefabs
    public GameObject imposterPrefab;

    [Header("Spawn Settings")]
    public Transform spawnPointsParent;

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
        // 1. Shuffle locations
        ShuffleSpawnPoints();

        int totalPoints = allSpawnPoints.Count;
        int npcCount = Mathf.Max(0, totalPoints - 4);

        // 2. Spawn Random NPCs
        for (int i = 0; i < npcCount; i++)
        {
            // Pick a random prefab from the list of 3
            int randomIndex = Random.Range(0, npcPrefabs.Length);
            GameObject selectedPrefab = npcPrefabs[randomIndex];

            Instantiate(selectedPrefab, allSpawnPoints[i].position, Quaternion.identity);
        }

        // 3. Spawn Imposter
        if (npcCount < totalPoints)
        {
            GameObject imposter = Instantiate(imposterPrefab, allSpawnPoints[npcCount].position, Quaternion.identity);
            
            ImposterController controller = imposter.GetComponent<ImposterController>();
            if (controller != null) 
            {
                controller.currentSpawnPoint = allSpawnPoints[npcCount];
            }
        }

        // 4. Register Empty Spots
        for (int i = npcCount + 1; i < totalPoints; i++)
        {
            emptySpawnPoints.Add(allSpawnPoints[i]);
        }
    }

    public Transform GetHiddenTeleportSpot(Transform currentSpotToFree)
    {
        if (emptySpawnPoints.Count == 0) return null;

        Camera cam = Camera.main;
        List<Transform> validHiddenSpots = new List<Transform>();

        // Filter: Find empty spots that are NOT currently visible
        foreach(Transform spot in emptySpawnPoints)
        {
            if (!IsPointOnScreen(cam, spot.position))
            {
                validHiddenSpots.Add(spot);
            }
        }

        if (validHiddenSpots.Count == 0) return null;

        int randomIndex = Random.Range(0, validHiddenSpots.Count);
        Transform newSpot = validHiddenSpots[randomIndex];

        emptySpawnPoints.Add(currentSpotToFree);
        emptySpawnPoints.Remove(newSpot);

        return newSpot;
    }

    bool IsPointOnScreen(Camera cam, Vector3 worldPos)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(worldPos);
        bool onScreen = (viewPos.x > 0 && viewPos.x < 1 && 
                         viewPos.y > 0 && viewPos.y < 1 && 
                         viewPos.z > 0);
        return onScreen;
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