using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates the game initialization sequence:
/// 1. Wait for MapManager to generate map with anchor rooms
/// 2. Wait for TurnManager to initialize players
/// 3. Signal game ready
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [Header("Initialization")]
    [Tooltip("Show detailed logs during initialization")]
    public bool showDetailedLogs = true;

    private bool isGameReady = false;
    public bool IsGameReady => isGameReady;

    private void Start()
    {
        StartCoroutine(InitializeGameSequence());
    }

    private IEnumerator InitializeGameSequence()
    {
        Log("========== GAME INITIALIZATION START ==========");

        // ===== STEP 1: MAP INITIALIZATION =====
        Log("STEP 1: Initializing map with anchor rooms...");
        
        if (MapManager.Instance != null && !MapManager.Instance.IsMapGenerated)
        {
            MapManager.Instance.InitializeGame();
        }

        while (MapManager.Instance != null && !MapManager.Instance.IsMapGenerated)
        {
            yield return new WaitForSeconds(0.1f);
        }

        if (MapManager.Instance != null)
        {
            int totalRooms = MapManager.Instance.GetTotalRoomsPlaced();
            int unusedTiles = MapManager.Instance.GetRemainingTiles();
            Log($"✓ Map initialized successfully!");
            Log($"  - Anchor rooms placed: {totalRooms}");
            Log($"  - Tiles available for discovery: {unusedTiles}");
            Log($"  - Player spawned at: Entrance Hall (1, 0)");
        }

        // ===== STEP 2: WAIT FOR TURN MANAGER =====
        Log("STEP 2: Waiting for turn system to initialize...");

        while (TurnManager.Instance != null && !TurnManager.Instance.IsGameInitialized)
        {
            yield return new WaitForSeconds(0.1f);
        }

        Log($"✓ Turn system initialized!");
        // FIXED: Use players instead of allPlayers
        if (TurnManager.Instance != null)
        {
            Log($"  - Active players: {TurnManager.Instance.players.Count}");
        }

        // ===== STEP 3: GAME READY =====
        Log("STEP 3: Finalizing game startup...");
        yield return new WaitForSeconds(0.5f); // Small delay for visual feedback

        isGameReady = true;
        Log("========== GAME READY TO PLAY! ==========");

        // Signal any listeners that game is ready
        OnGameReady();
    }

    private void OnGameReady()
    {
        // Here you can trigger UI transitions, fade in effects, etc.
        Debug.Log("[GameInitializer] Broadcasting game ready signal...");
        
        // Example: You could fade in the UI here
        // UIManager.Instance.FadeIn();
    }

    private void Log(string message)
    {
        if (showDetailedLogs)
        {
            Debug.Log($"[GameInitializer] {message}");
        }
    }
}
