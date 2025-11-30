using UnityEngine;

public class GameModeManager : MonoBehaviour
{
    public static GameModeManager Instance { get; private set; }

    public enum GameMode
    {
        None,
        SinglePlayer,
        Multiplayer
    }

    private GameMode currentGameMode = GameMode.None;

    [Header("Single Player Settings")]
    [SerializeField] private int singlePlayerAICount = 2;
    [SerializeField] private bool autoGenerateAIPlayers = false;

    [Header("Multiplayer Settings")]
    [SerializeField] private int minMultiplayerPlayers = 3;
    [SerializeField] private int maxMultiplayerPlayers = 6;

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

    public void SetGameMode(GameMode mode)
    {
        currentGameMode = mode;
        Debug.Log($"[GameModeManager] Game mode set to: {mode}");

        switch (mode)
        {
            case GameMode.SinglePlayer:
                InitializeSinglePlayer();
                break;
            case GameMode.Multiplayer:
                InitializeMultiplayer();
                break;
            case GameMode.None:
                break;
        }
    }

    private void InitializeSinglePlayer()
    {
        Debug.Log($"[GameModeManager] Single Player mode initialized with {singlePlayerAICount} AI players");
        
        if (CharacterSelectionManager.Instance != null && autoGenerateAIPlayers)
        {
            CharacterSelectionManager.Instance.ClearSelection();
            
            // Optionally add AI players automatically
            // This would require additional setup
        }
    }

    private void InitializeMultiplayer()
    {
        Debug.Log($"[GameModeManager] Multiplayer mode initialized (Min: {minMultiplayerPlayers}, Max: {maxMultiplayerPlayers})");
        
        if (CharacterSelectionManager.Instance != null)
        {
            CharacterSelectionManager.Instance.ClearSelection();
        }
    }

    public GameMode GetCurrentGameMode()
    {
        return currentGameMode;
    }

    public bool IsSinglePlayer()
    {
        return currentGameMode == GameMode.SinglePlayer;
    }

    public bool IsMultiplayer()
    {
        return currentGameMode == GameMode.Multiplayer;
    }

    public int GetSinglePlayerAICount()
    {
        return singlePlayerAICount;
    }

    public int GetMinMultiplayerPlayers()
    {
        return minMultiplayerPlayers;
    }

    public int GetMaxMultiplayerPlayers()
    {
        return maxMultiplayerPlayers;
    }

    public void SetSinglePlayerAICount(int count)
    {
        singlePlayerAICount = Mathf.Clamp(count, 1, 5);
        Debug.Log($"[GameModeManager] Single player AI count set to: {singlePlayerAICount}");
    }
}
