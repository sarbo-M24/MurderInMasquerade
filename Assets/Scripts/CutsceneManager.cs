using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    [Header("Video Settings")]
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private VideoClip cutsceneVideo; // Your MP4 file

    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "GameplayScene"; // Your main game scene name
    [SerializeField] private bool allowSkip = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    private RenderTexture renderTexture;
    private bool isVideoFinished = false;

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
        videoPlayer.clip = cutsceneVideo;
        videoPlayer.isLooping = false;
        videoPlayer.skipOnDrop = true;

        // Subscribe to video finished event
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void PlayCutscene()
    {
        if (videoPlayer != null && cutsceneVideo != null)
        {
            videoPlayer.Play();
            Debug.Log("Playing cutscene...");
        }
        else
        {
            Debug.LogError("Video Player or Video Clip not assigned! Loading gameplay scene...");
            LoadGameplayScene();
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
        Debug.Log("Cutscene finished, loading gameplay scene...");
        LoadGameplayScene();
    }

    void SkipCutscene()
    {
        Debug.Log("Cutscene skipped!");
        videoPlayer.Stop();
        LoadGameplayScene();
    }

    void LoadGameplayScene()
    {
        SceneManager.LoadScene(gameplaySceneName);
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
