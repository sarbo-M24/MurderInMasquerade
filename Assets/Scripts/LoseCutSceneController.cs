using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoseCutsceneController : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private VideoClip loseCutsceneVideo; // Your MP4 file

    [Header("Scene Settings")]
    [SerializeField] private string nextSceneName = "MainMenu"; // Where to go after cutscene
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    [Header("Lose Screen")]
    [SerializeField] private GameObject loseScreenPanel; // Drag your lose screen UI panel here
    [SerializeField] private float delayBeforeMenu = 2f; // Seconds to wait before going to menu

    [Header("Optional UI")]
    [SerializeField] private GameObject skipPrompt; // "Press SPACE to skip" text (optional)

    private RenderTexture renderTexture;
    private bool isVideoFinished = false;

    void Awake()
    {
        // Ensure time is running
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Hide lose screen at start
        if (loseScreenPanel != null)
            loseScreenPanel.SetActive(false);
    }

    void Start()
    {
        SetupVideoPlayer();
        PlayCutscene();
    }

    void SetupVideoPlayer()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("Video Player not assigned!");
            return;
        }

        // Create render texture for video output
        renderTexture = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = renderTexture;

        // Assign render texture to Raw Image
        if (videoDisplay != null)
        {
            videoDisplay.texture = renderTexture;
        }

        // Setup video player
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = loseCutsceneVideo;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;

        // Subscribe to video finished event
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void PlayCutscene()
    {
        if (videoPlayer != null && loseCutsceneVideo != null)
        {
            videoPlayer.Play();
            Debug.Log("Playing lose cutscene...");

            if (skipPrompt != null)
                skipPrompt.SetActive(true);
        }
        else
        {
            Debug.LogError("Video Player or Video Clip not assigned! Loading next scene...");
            LoadNextScene();
        }
    }

    void Update()
    {
        // Allow player to skip cutscene
        if (allowSkip && Input.GetKeyDown(skipKey) && !isVideoFinished)
        {
            SkipCutscene();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        isVideoFinished = true;
        Debug.Log("Lose cutscene finished, showing lose screen...");

        // Hide video and skip prompt
        if (videoDisplay != null)
            videoDisplay.gameObject.SetActive(false);
        if (skipPrompt != null)
            skipPrompt.SetActive(false);

        // Show lose screen panel
        if (loseScreenPanel != null)
        {
            loseScreenPanel.SetActive(true);
        }

        // Start countdown to main menu
        StartCoroutine(LoadMenuAfterDelay());
    }

    void SkipCutscene()
    {
        Debug.Log("Cutscene skipped!");
        videoPlayer.Stop();

        // Trigger the same sequence as if video finished
        OnVideoFinished(videoPlayer);
    }

    IEnumerator LoadMenuAfterDelay()
    {
        Debug.Log($"Waiting {delayBeforeMenu} seconds before returning to menu...");

        // Use realtime so it works even if timeScale changes
        yield return new WaitForSecondsRealtime(delayBeforeMenu);

        Debug.Log("Loading main menu...");
        LoadNextScene();
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // Clean up
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
