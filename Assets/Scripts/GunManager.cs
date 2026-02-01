using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // For restarting the game

public class GunManager : MonoBehaviour
{
    public static GunManager instance;
    public static bool isGameOver = false; // The master switch
    [Header("UI References")]
    public GameObject losePanel; // Drag your 'LosePanel' here
    public GameObject gunUIElement;
    public GameObject crosshairUI;
    public Image timerCircle;      // Drag your TimerCircle Image here
    public GameObject loseMessage; // Drag your LoseText GameObject here

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
        if (partsCollected >= totalPartsRequired)
        {
            StartTimer();
        }
    }

    void StartTimer()
    {
        isGunComplete = true;
        timerActive = true;
        currentTime = timeLimit;

        gunUIElement.SetActive(true);
        crosshairUI.SetActive(true);
        timerCircle.gameObject.SetActive(true);
    }

    void GameOver()
    {
        isGameOver = true; // Trigger the freeze
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
    
}