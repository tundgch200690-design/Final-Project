using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance { get; private set; }

    [Header("Scene Names")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private string singlePlayerCharacterSelectionScene = "CharacterSelection";
    [SerializeField] private string multiplayerCharacterSelectionScene = "CharacterSelection";
    [SerializeField] private string gamePlaySceneName = "GameScene";

    [Header("Game Mode")]
    private GameMode currentGameMode = GameMode.None;

    public enum GameMode
    {
        None,
        SinglePlayer,
        Multiplayer
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySinglePlayer()
    {
        currentGameMode = GameMode.SinglePlayer;
        Debug.Log("[MainMenuManager] Starting Single Player mode");
        
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.SinglePlayer);
        }
        
        SceneManager.LoadScene(singlePlayerCharacterSelectionScene);
    }

    public void PlayMultiplayer()
    {
        currentGameMode = GameMode.Multiplayer;
        Debug.Log("[MainMenuManager] Starting Multiplayer mode");
        
        if (GameModeManager.Instance != null)
        {
            GameModeManager.Instance.SetGameMode(GameMode.Multiplayer);
        }
        
        SceneManager.LoadScene(multiplayerCharacterSelectionScene);
    }

    public void OpenSettings()
    {
        Debug.Log("[MainMenuManager] Opening Settings");
        // Settings will be handled by UI panels in the same scene
    }

    public void OpenCredits()
    {
        Debug.Log("[MainMenuManager] Opening Credits");
        // Credits will be handled by UI panels in the same scene
    }

    public void ReturnToMainMenu()
    {
        currentGameMode = GameMode.None;
        Time.timeScale = 1f; // Ensure game is not paused
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("[MainMenuManager] Quitting game");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
    }
}
