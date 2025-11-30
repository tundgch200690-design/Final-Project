using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main HUD Manager for Sprint 3 - Controls all in-game UI elements
/// Displays player stats, turn indicators, and handles UI updates
/// </summary>
public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }

    [Header("Player Stats UI")]
    [SerializeField] private PlayerStatsUI playerStatsUI;
    
    [Header("Turn System UI")]
    [SerializeField] private TurnIndicatorUI turnIndicatorUI;
    [SerializeField] private Button endTurnButton;
    
    [Header("Game Controls")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button menuButton;
    
    [Header("Debug Display")]
    [SerializeField] private GameObject debugPanel;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private bool showDebugInfo = true;

    private bool isPaused = false;
    private Player currentPlayer;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializeHUD();
        SubscribeToEvents();
    }

    private void InitializeHUD()
    {
        // Setup button listeners
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePause);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OpenGameMenu);
        }

        // Initialize debug panel
        if (debugPanel != null)
        {
            debugPanel.SetActive(showDebugInfo);
        }

        Debug.Log("[HUDManager] HUD initialized successfully");
    }

    private void SubscribeToEvents()
    {
        // Subscribe to turn changes
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.AddListener(OnTurnChanged);
        }
    }

    private void OnTurnChanged(Player newActivePlayer)
    {
        currentPlayer = newActivePlayer;
        
        // Update player stats UI
        if (playerStatsUI != null)
        {
            playerStatsUI.UpdateDisplay(newActivePlayer);
        }

        // Update turn indicator
        if (turnIndicatorUI != null)
        {
            turnIndicatorUI.UpdateTurnDisplay(newActivePlayer);
        }

        Debug.Log($"[HUDManager] UI updated for {newActivePlayer?.playerName}'s turn");
    }

    private void OnEndTurnClicked()
    {
        if (TurnManager.Instance != null && TurnManager.Instance.IsGameInitialized)
        {
            TurnManager.Instance.EndTurn();
            Debug.Log("[HUDManager] End turn button clicked");
        }
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        
        Debug.Log($"[HUDManager] Game {(isPaused ? "paused" : "resumed")}");
    }

    private void OpenGameMenu()
    {
        // This will be expanded later - for now just log
        Debug.Log("[HUDManager] Game menu requested");
        // TODO: Implement pause menu/settings overlay
    }

    private void Update()
    {
        UpdateDebugDisplay();
        HandleKeyboardInput();
    }

    private void UpdateDebugDisplay()
    {
        if (!showDebugInfo || debugText == null) return;

        string debugInfo = "=== GAME DEBUG INFO ===\n";
        debugInfo += $"Current Player: {currentPlayer?.playerName ?? "NONE"}\n";
        debugInfo += $"Game Initialized: {TurnManager.Instance?.IsGameInitialized ?? false}\n";
        debugInfo += $"Total Players: {TurnManager.Instance?.players.Count ?? 0}\n";
        
        if (currentPlayer != null)
        {
            var controller = currentPlayer.GetComponent<PlayerController>();
            if (controller != null)
            {
                debugInfo += $"Grid Position: {controller.gridPosition}\n";
                debugInfo += $"Moves Left: {controller.currentMovesLeft}\n";
            }

            var stats = currentPlayer.GetComponent<PlayerStats>();
            if (stats != null)
            {
                debugInfo += $"Speed: {stats.GetSpeed()}\n";
                debugInfo += $"Might: {stats.GetMight()}\n";
                debugInfo += $"Sanity: {stats.GetSanity()}\n";
                debugInfo += $"Knowledge: {stats.GetKnowledge()}\n";
                debugInfo += $"Is Dead: {stats.IsDead}\n";
            }
        }
        
        debugInfo += $"Map Rooms: {MapManager.Instance?.GetTotalRoomsPlaced() ?? 0}\n";
        debugInfo += $"Time Scale: {Time.timeScale}\n";

        debugText.text = debugInfo;
    }

    private void HandleKeyboardInput()
    {
        // Keyboard shortcuts for debugging
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnEndTurnClicked();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleDebugDisplay();
        }
    }

    public void ToggleDebugDisplay()
    {
        showDebugInfo = !showDebugInfo;
        if (debugPanel != null)
        {
            debugPanel.SetActive(showDebugInfo);
        }
    }

    // Public methods for external UI updates
    public void ForceUpdateDisplay()
    {
        if (currentPlayer != null)
        {
            OnTurnChanged(currentPlayer);
        }
    }

    public void ShowNotification(string message, float duration = 3f)
    {
        Debug.Log($"[HUDManager] Notification: {message}");
        // TODO: Implement notification popup system
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.RemoveListener(OnTurnChanged);
        }
    }
}