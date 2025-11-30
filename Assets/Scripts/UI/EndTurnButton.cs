using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// End Turn button handler for Sprint 3
/// Simple one-line connection to TurnManager.EndTurn()
/// </summary>
[RequireComponent(typeof(Button))]
public class EndTurnButton : MonoBehaviour
{
    [Header("Button Configuration")]
    [Tooltip("Enable visual feedback based on game state")]
    public bool enableVisualFeedback = true;
    
    private Button endTurnButton;
    
    private void Awake()
    {
        endTurnButton = GetComponent<Button>();
    }
    
    private void Start()
    {
        // One-line fix: Connect button to TurnManager.EndTurn()
        endTurnButton.onClick.AddListener(() => TurnManager.Instance?.EndTurn());
        
        Debug.Log("[EndTurnButton] End Turn button connected to TurnManager.EndTurn()");
        
        // Fixed: Subscribe to UnityEvent instead of System.Action
        if (enableVisualFeedback && TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.AddListener(OnTurnChanged);
        }
    }
    
    private void OnDestroy()
    {
        // Fixed: Clean up UnityEvent subscriptions
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.RemoveListener(OnTurnChanged);
        }
    }
    
    /// <summary>
    /// Optional visual feedback when turn changes
    /// </summary>
    private void OnTurnChanged(Player newActivePlayer)
    {
        if (enableVisualFeedback && endTurnButton != null)
        {
            // Enable button only if game is properly initialized
            endTurnButton.interactable = TurnManager.Instance != null && 
                                        TurnManager.Instance.IsGameInitialized;
        }
    }
    
    /// <summary>
    /// Alternative method for UI Button events (drag-and-drop in Inspector)
    /// </summary>
    public void OnEndTurnButtonClicked()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.EndTurn();
            Debug.Log("[EndTurnButton] End Turn called via Inspector method");
        }
        else
        {
            Debug.LogError("[EndTurnButton] TurnManager.Instance is null!");
        }
    }
    
    /// <summary>
    /// Test method for debugging
    /// </summary>
    [ContextMenu("Test End Turn")]
    public void TestEndTurn()
    {
        OnEndTurnButtonClicked();
    }
}