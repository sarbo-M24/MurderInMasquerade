using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For restarting the game
using System.Collections; // Required for Coroutines

public class GunManager : MonoBehaviour
{
    [Header("Phase Two Event (3/4 Parts)")]
    public SpriteRenderer imposterRenderer; // Drag the Imposter's SpriteRenderer here
    public Sprite imposterNewSprite;        // Drag the new sprite here
    public GameObject[] maskPrefabs;        // Add your mask prefabs to this list
    public Transform maskSpawnPoint;        // Drag the empty 'MaskSpawnPoint' here
    private bool hasTriggeredPhaseTwo = false;
    public static GunManager instance;
    public static bool isGameOver = false; // The master switch
    [Header("UI References")]
    public GameObject losePanel; // Drag your 'LosePanel' here
    public GameObject winPanel; // Drag your new 'WinPanel' here
    public GameObject gunUIElement;
    public GameObject crosshairUI;
    public Image timerCircle;      // Drag your TimerCircle Image here
    public GameObject loseMessage; // Drag your LoseText GameObject here

    [Header("Audio References")]
    public AudioSource mainLoopSource;
    public AudioSource gunMusicSource;
    public float fadeDuration = 2.0f; // How long the crossfade takes

    [Header("Timer Settings")]
    public float timeLimit = 10f;  // Seconds allowed to shoot
    private float currentTime;
    private bool timerActive = false;

    [Header("Collection State")]
    public int partsCollected = 0;
    public int totalPartsRequired = 4;
    [HideInInspector] public bool isGunComplete = false;
    [HideInInspector] public bool hasFired = false;

    void Awake()
    {
        if (instance == null) instance = this;
        isGameOver = false; // Reset on every restart
        // Hide everything at start
        Time.timeScale = 1; // Ensure time is moving when the scene starts
        losePanel.SetActive(false);
        gunUIElement.SetActive(false);
        crosshairUI.SetActive(false);
        timerCircle.gameObject.SetActive(false);
        loseMessage.SetActive(false);
    }

    void Update()
    {
        if (timerActive && !hasFired)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                // Update the circle fill (value between 0 and 1)
                timerCircle.fillAmount = currentTime / timeLimit;
            }
            else
            {
                GameOver();
            }
        }
    }

    public void CollectPart()
    {
        partsCollected++;

        Debug.Log("Collected part " + partsCollected + "/4");

        // NEW CONDITION: Trigger event at 3 parts
        if (partsCollected == 3 && !hasTriggeredPhaseTwo)
        {
            TriggerPhaseTwo();
        }

        if (partsCollected >= totalPartsRequired)
        {
            StartTimer();
        }
    }

    void TriggerPhaseTwo()
    {
        hasTriggeredPhaseTwo = true;

        // Find the imposter dynamically
        GameObject imposterObj = GameObject.FindGameObjectWithTag("Imposter");
        if (imposterObj != null)
        {
            SpriteRenderer imposterRend = imposterObj.GetComponentInChildren<SpriteRenderer>();

            if (imposterRend != null && imposterNewSprite != null)
            {
                imposterRend.sprite = imposterNewSprite;
                Debug.Log("The Imposter has changed appearance!");
            }
        }
        else
        {
            Debug.LogWarning("Imposter not found!");
        }

        // 2. Spawn a Random Mask
        if (maskPrefabs.Length > 0 && maskSpawnPoint != null)
        {
            int randomIndex = Random.Range(0, maskPrefabs.Length);
            Instantiate(maskPrefabs[randomIndex], maskSpawnPoint.position, maskSpawnPoint.rotation);
            Debug.Log("A random mask has appeared.");
        }
    }


    void StartTimer()
    {
        isGunComplete = true;
        timerActive = true;
        currentTime = timeLimit;

        // Start the crossfade logic
        StartCoroutine(CrossfadeMusic());

        gunUIElement.SetActive(true);
        crosshairUI.SetActive(true);
        timerCircle.gameObject.SetActive(true);
    }

    IEnumerator CrossfadeMusic()
    {
        float timer = 0;
        gunMusicSource.Play(); // Start playing the second track (at 0 volume)

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / fadeDuration;

            // Main loop goes from 1 to 0
            mainLoopSource.volume = Mathf.Lerp(1f, 0f, percent);
            // Gun music goes from 0 to 1
            gunMusicSource.volume = Mathf.Lerp(0f, 1f, percent);

            yield return null; // Wait for the next frame
        }

        mainLoopSource.Stop(); // Fully stop the old music
    }

    // Call this in your WinGame and GameOver functions
    public void StopAllMusic()
    {
        mainLoopSource.Stop();
        gunMusicSource.Stop();
    }

    public void GameOver()
    {
        isGameOver = true; // Trigger the freeze
        StopAllMusic(); // Stops the loops
        losePanel.SetActive(true); // Show the whole panel (text + button)
        timerActive = false;
        loseMessage.SetActive(true);
        // Unlock cursor so player can click a restart button if you add one
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0; // Freeze the game
        Debug.Log("Time's up! You Lose.");
    }

    public void RetryGame()
    {
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void WinGame()
{
    isGameOver = true;
    StopAllMusic(); // Stops the loops
    winPanel.SetActive(true);
    
    Cursor.lockState = CursorLockMode.None;
    Cursor.visible = true;

    Time.timeScale = 0; // Freeze the world
}
    
}