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
     // Drag your LoseText GameObject here

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
        isGameOver = false;
        Time.timeScale = 1;

        // Add null checks for everything
        if (losePanel != null)
            losePanel.SetActive(false);
        else
            Debug.LogWarning("losePanel not assigned!");

        if (winPanel != null)
            winPanel.SetActive(false);
        else
            Debug.LogWarning("winPanel not assigned!");

        if (gunUIElement != null)
            gunUIElement.SetActive(false);
        else
            Debug.LogWarning("gunUIElement not assigned!");

        if (crosshairUI != null)
            crosshairUI.SetActive(false);
        else
            Debug.LogWarning("crosshairUI not assigned!");

        if (timerCircle != null)
            timerCircle.gameObject.SetActive(false);
        else
            Debug.LogWarning("timerCircle not assigned!");

       
    }


    void Update()
    {
        // DIAGNOSTIC: Check if time is actually moving
        Debug.Log($"Time.deltaTime: {Time.deltaTime} | Time.timeScale: {Time.timeScale}");

        if (timerActive && !hasFired)
        {
            Debug.Log($"Timer Active! Current time: {currentTime}");

            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                timerCircle.fillAmount = currentTime / timeLimit;

                Debug.Log($"After decrement: {currentTime} | Fill: {timerCircle.fillAmount}");
            }
            else
            {
                Debug.Log("Timer hit zero, calling GameOver");
                GameOver();
            }
        }
        else
        {
            Debug.Log($"Timer not active. timerActive={timerActive}, hasFired={hasFired}");
        }
    }

    public void CollectPart()
    {
        partsCollected++;

        Debug.Log("Collected part " + partsCollected + "/" + totalPartsRequired);

        // NEW CONDITION: Trigger event at 3 parts
        if (partsCollected == 3 && !hasTriggeredPhaseTwo)
        {
            Debug.Log("Triggering Phase Two at 3 parts");
            TriggerPhaseTwo();
        }

        if (partsCollected >= totalPartsRequired)
        {
            Debug.Log("All parts collected! Starting timer...");
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
        Debug.Log("StartTimer() called!");
        Debug.Log("Gun UI Element: " + (gunUIElement != null ? "Assigned" : "NULL"));
        Debug.Log("Crosshair UI: " + (crosshairUI != null ? "Assigned" : "NULL"));
        Debug.Log("Timer Circle: " + (timerCircle != null ? "Assigned" : "NULL"));

        isGunComplete = true;
        timerActive = true;
        currentTime = timeLimit;

        // Start the crossfade logic
        StartCoroutine(CrossfadeMusic());

        if (gunUIElement != null)
            gunUIElement.SetActive(true);
        else
            Debug.LogError("gunUIElement is NULL!");

        if (crosshairUI != null)
            crosshairUI.SetActive(true);
        else
            Debug.LogError("crosshairUI is NULL!");

        if (timerCircle != null)
            timerCircle.gameObject.SetActive(true);
        else
            Debug.LogError("timerCircle is NULL!");

        Debug.Log("Timer is now active: " + timerActive);
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
        timerActive = false;

        // Unlock cursor so player can interact
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // DON'T set Time.timeScale to 0 before loading scene
        // Time.timeScale = 0; // REMOVE THIS LINE

        // Load the lose scene - timeScale will reset automatically
        SceneManager.LoadScene("Lose");

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

        if (winPanel != null)
            winPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Wait 2 seconds then go to main menu
        StartCoroutine(LoadMainMenuAfterDelay(2f));
    }

    IEnumerator LoadMainMenuAfterDelay(float delay)
    {
        Debug.Log($"You Win! Returning to menu in {delay} seconds...");

        // Wait for specified time
        yield return new WaitForSeconds(delay);

        Debug.Log("Loading main menu...");
        SceneManager.LoadScene("MainMenu");
    }


}