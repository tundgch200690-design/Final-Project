using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Displays turn information and provides visual feedback for turn changes
/// </summary>
public class TurnIndicatorUI : MonoBehaviour
{
    [Header("Turn Display")]
    [SerializeField] private TextMeshProUGUI currentTurnText;
    [SerializeField] private TextMeshProUGUI turnAnnouncementText;
    [SerializeField] private Image currentPlayerIndicator;

    [Header("Player List Display")]
    [SerializeField] private Transform playerListParent;
    [SerializeField] private GameObject playerListItemPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float turnChangeAnimationDuration = 1.5f;
    [SerializeField] private AnimationCurve fadeInCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Colors")]
    [SerializeField] private Color activePlayerColor = Color.green;
    [SerializeField] private Color inactivePlayerColor = Color.gray;
    [SerializeField] private Color deadPlayerColor = Color.red;

    private System.Collections.Generic.List<PlayerListItem> playerListItems = new System.Collections.Generic.List<PlayerListItem>();

    private void Start()
    {
        InitializePlayerList();
    }

    private void InitializePlayerList()
    {
        if (TurnManager.Instance == null || playerListParent == null || playerListItemPrefab == null)
            return;

        // Clear existing items
        ClearPlayerList();

        // Create list items for each player
        foreach (Player player in TurnManager.Instance.players)
        {
            CreatePlayerListItem(player);
        }
    }

    private void CreatePlayerListItem(Player player)
    {
        GameObject itemObj = Instantiate(playerListItemPrefab, playerListParent);
        PlayerListItem item = itemObj.GetComponent<PlayerListItem>();
        
        if (item == null)
        {
            item = itemObj.AddComponent<PlayerListItem>();
        }

        item.Initialize(player);
        playerListItems.Add(item);
    }

    public void UpdateTurnDisplay(Player activePlayer)
    {
        if (activePlayer == null) return;

        // Update main turn text
        if (currentTurnText != null)
        {
            currentTurnText.text = $"{activePlayer.playerName}'s Turn";
        }

        // Update player indicator color
        if (currentPlayerIndicator != null)
        {
            currentPlayerIndicator.color = activePlayer.GetComponent<PlayerStats>()?.IsDead == true 
                ? deadPlayerColor 
                : activePlayerColor;
        }

        // Update player list
        UpdatePlayerList(activePlayer);

        // Show turn change animation
        StartCoroutine(PlayTurnChangeAnimation(activePlayer));
    }

    private void UpdatePlayerList(Player activePlayer)
    {
        foreach (PlayerListItem item in playerListItems)
        {
            if (item.AssignedPlayer == activePlayer)
            {
                item.SetActive(true);
            }
            else
            {
                item.SetActive(false);
            }
        }
    }

    private IEnumerator PlayTurnChangeAnimation(Player newPlayer)
    {
        if (turnAnnouncementText == null) yield break;

        // Setup announcement text
        turnAnnouncementText.text = $"{newPlayer.playerName}'s Turn!";
        turnAnnouncementText.gameObject.SetActive(true);
        
        // Fade in animation
        float elapsedTime = 0f;
        Color originalColor = turnAnnouncementText.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        
        turnAnnouncementText.color = transparentColor;

        while (elapsedTime < turnChangeAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / turnChangeAnimationDuration;
            
            // Fade in first half, fade out second half
            float alpha = progress <= 0.5f 
                ? fadeInCurve.Evaluate(progress * 2) 
                : fadeInCurve.Evaluate(2 - progress * 2);
            
            turnAnnouncementText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            
            yield return null;
        }

        // Hide announcement
        turnAnnouncementText.gameObject.SetActive(false);
        turnAnnouncementText.color = originalColor;
    }

    private void ClearPlayerList()
    {
        foreach (PlayerListItem item in playerListItems)
        {
            if (item != null)
                DestroyImmediate(item.gameObject);
        }
        playerListItems.Clear();
    }

    // Public method to refresh the entire display
    public void RefreshDisplay()
    {
        if (TurnManager.Instance != null && TurnManager.Instance.CurrentPlayer != null)
        {
            UpdateTurnDisplay(TurnManager.Instance.CurrentPlayer);
        }
    }
}

/// <summary>
/// Individual player list item component
/// </summary>
public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image statusIcon;
    
    public Player AssignedPlayer { get; private set; }

    public void Initialize(Player player)
    {
        AssignedPlayer = player;
        
        if (playerNameText != null)
        {
            playerNameText.text = player.playerName;
        }
    }

    public void SetActive(bool isActive)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isActive ? Color.green : Color.gray;
        }

        // Update status icon based on player state
        if (statusIcon != null && AssignedPlayer != null)
        {
            PlayerStats stats = AssignedPlayer.GetComponent<PlayerStats>();
            if (stats != null && stats.IsDead)
            {
                statusIcon.color = Color.red;
            }
            else
            {
                statusIcon.color = isActive ? Color.green : Color.white;
            }
        }
    }
}