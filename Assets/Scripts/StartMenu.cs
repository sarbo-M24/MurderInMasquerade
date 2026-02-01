using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string cutsceneSceneName = "CutsceneScene";

    [Header("Input Settings")]
    [SerializeField] private KeyCode startKey = KeyCode.Space;
    [SerializeField] private KeyCode alternateKey = KeyCode.Return; // Enter key as backup

    void Update()
    {
        // Check for key press
        if (Input.GetKeyDown(startKey) || Input.GetKeyDown(alternateKey))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        Debug.Log("Starting game! Loading cutscene...");
        SceneManager.LoadScene(cutsceneSceneName);
    }
}
