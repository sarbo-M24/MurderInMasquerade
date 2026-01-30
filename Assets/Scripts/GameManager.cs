using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Characters")]
    public GameObject npcPrefab;
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
        ShuffleSpawnPoints();

        int totalPoints = allSpawnPoints.Count;
        int npcCount = Mathf.Max(0, totalPoints - 4);

        // Spawn NPCs
        for (int i = 0; i < npcCount; i++)
        {
            Instantiate(npcPrefab, allSpawnPoints[i].position, Quaternion.identity);
        }

        // Spawn Imposter
        if (npcCount < totalPoints)
        {
            GameObject imposter = Instantiate(imposterPrefab, allSpawnPoints[npcCount].position, Quaternion.identity);
            
            ImposterController controller = imposter.GetComponent<ImposterController>();
            if (controller != null) 
            {
                controller.currentSpawnPoint = allSpawnPoints[npcCount];
            }
        }

        // Register Empty Spots
        for (int i = npcCount + 1; i < totalPoints; i++)
        {
            emptySpawnPoints.Add(allSpawnPoints[i]);
        }
    }

    // --- UPDATED METHOD ---
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

        // If no hidden spots exist (player looking at everything), stay put (return null)
        if (validHiddenSpots.Count == 0) return null;

        // Pick random from HIDDEN spots
        int randomIndex = Random.Range(0, validHiddenSpots.Count);
        Transform newSpot = validHiddenSpots[randomIndex];

        // Manage the lists
        emptySpawnPoints.Add(currentSpotToFree);       // Old spot is now empty
        emptySpawnPoints.Remove(newSpot);              // New spot is now taken

        return newSpot;
    }

    // Helper to check if a specific world position is on screen
    bool IsPointOnScreen(Camera cam, Vector3 worldPos)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(worldPos);
        
        // Viewport coordinates: (0,0) is bottom-left, (1,1) is top-right.
        // Z > 0 means it's in front of the camera, not behind.
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