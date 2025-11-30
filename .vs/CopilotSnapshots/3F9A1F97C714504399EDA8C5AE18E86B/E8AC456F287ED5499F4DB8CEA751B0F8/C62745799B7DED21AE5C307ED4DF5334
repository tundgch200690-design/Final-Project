using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Sprint 3 Player System")]
    public List<Player> players = new List<Player>();

    [Header("Turn State")]
    public int currentPlayerIndex = 0;
    private bool isGameInitialized = false;

    [Header("Events")]
    public UnityEvent<Player> OnTurnChanged = new UnityEvent<Player>();

    public bool IsGameInitialized => isGameInitialized;
    public Player CurrentPlayer => (players.Count > 0 && currentPlayerIndex >= 0 && currentPlayerIndex < players.Count) ? players[currentPlayerIndex] : null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        yield return new WaitForEndOfFrame();

        if (players.Count >= 3 && players.Count <= 6)
        {
            isGameInitialized = true;
            StartFirstTurn();
        }
        else
        {
            Debug.LogError($"[TurnManager] Invalid player count: {players.Count}. Need 3-6 players.");
        }
    }

    private void StartFirstTurn()
    {
        currentPlayerIndex = 0;
        if (CurrentPlayer != null)
        {
            Debug.Log($"[TurnManager] Game started! {CurrentPlayer.playerName}'s turn");
            OnTurnChanged?.Invoke(CurrentPlayer);
        }
    }

    public void EndTurn()
    {
        if (!isGameInitialized || players.Count == 0) return;

        Player previousPlayer = CurrentPlayer;
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        Player newPlayer = CurrentPlayer;

        if (newPlayer != null)
        {
            Debug.Log($"[TurnManager] Now {newPlayer.playerName}'s turn! (Index: {currentPlayerIndex})");
            OnTurnChanged?.Invoke(newPlayer);
        }
    }

    public void SimpleEndTurn()
    {
        Debug.Log("Simple End Turn clicked!");
        EndTurn();
    }

    [ContextMenu("Debug End Turn")]
    public void DebugEndTurn()
    {
        EndTurn();
    }

    // Add this temporary method to TurnManager for easy testing
    public void QuickTest()
    {
        Debug.Log("=== SPRINT 3 QUICK TEST ===");
        Debug.Log($"Players in list: {players.Count}");
        Debug.Log($"Game initialized: {isGameInitialized}");
        Debug.Log($"Current player: {CurrentPlayer?.playerName ?? "NONE"}");
        
        if (players.Count > 0)
        {
            SimpleEndTurn();
        }
    }
}