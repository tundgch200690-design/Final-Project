using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the current player's stats (Speed, Might, Sanity, Knowledge)
/// Updates automatically when turns change
/// </summary>
public class PlayerStatsUI : MonoBehaviour
{
    [Header("Player Info Display")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private Image playerPortrait;

    [Header("Stat Value Displays")]
    [SerializeField] private TextMeshProUGUI speedValueText;
    [SerializeField] private TextMeshProUGUI mightValueText;
    [SerializeField] private TextMeshProUGUI sanityValueText;
    [SerializeField] private TextMeshProUGUI knowledgeValueText;

    [Header("Stat Bar Displays (Optional)")]
    [SerializeField] private Slider speedBar;
    [SerializeField] private Slider mightBar;
    [SerializeField] private Slider sanityBar;
    [SerializeField] private Slider knowledgeBar;

    [Header("Stat Index Displays")]
    [SerializeField] private TextMeshProUGUI speedIndexText;
    [SerializeField] private TextMeshProUGUI mightIndexText;
    [SerializeField] private TextMeshProUGUI sanityIndexText;
    [SerializeField] private TextMeshProUGUI knowledgeIndexText;

    [Header("Status Indicators")]
    [SerializeField] private GameObject deadIndicator;
    [SerializeField] private TextMeshProUGUI movesLeftText;

    [Header("Visual Settings")]
    [SerializeField] private Color normalStatColor = Color.white;
    [SerializeField] private Color lowStatColor = Color.red;
    [SerializeField] private Color highStatColor = Color.green;
    [SerializeField] private int lowStatThreshold = 3;
    [SerializeField] private int highStatThreshold = 6;

    public void UpdateDisplay(Player player)
    {
        if (player == null)
        {
            ClearDisplay();
            return;
        }

        UpdatePlayerInfo(player);
        UpdateStats(player);
        UpdateStatusIndicators(player);
    }

    private void UpdatePlayerInfo(Player player)
    {
        // Update player name
        if (playerNameText != null)
        {
            playerNameText.text = player.playerName;
        }

        // Update player portrait using the new property
        if (playerPortrait != null)
        {
            // Try characterPortrait first, fallback to portrait
            Sprite portraitSprite = player.CharacterData?.characterPortrait ?? player.CharacterData?.portrait;
            
            if (portraitSprite != null)
            {
                playerPortrait.sprite = portraitSprite;
                playerPortrait.gameObject.SetActive(true);
            }
            else
            {
                playerPortrait.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateStats(Player player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        if (stats == null) return;

        // Get current stat values
        int speedValue = stats.GetSpeed();
        int mightValue = stats.GetMight();
        int sanityValue = stats.GetSanity();
        int knowledgeValue = stats.GetKnowledge();

        // Update stat value texts
        UpdateStatText(speedValueText, speedValue, "Speed");
        UpdateStatText(mightValueText, mightValue, "Might");
        UpdateStatText(sanityValueText, sanityValue, "Sanity");
        UpdateStatText(knowledgeValueText, knowledgeValue, "Knowledge");

        // Update stat index texts (showing position on track)
        UpdateIndexText(speedIndexText, stats.currentSpeedIndex, "Speed Index");
        UpdateIndexText(mightIndexText, stats.currentMightIndex, "Might Index");
        UpdateIndexText(sanityIndexText, stats.currentSanityIndex, "Sanity Index");
        UpdateIndexText(knowledgeIndexText, stats.currentKnowledgeIndex, "Knowledge Index");

        // Update stat bars if available
        UpdateStatBar(speedBar, speedValue);
        UpdateStatBar(mightBar, mightValue);
        UpdateStatBar(sanityBar, sanityValue);
        UpdateStatBar(knowledgeBar, knowledgeValue);
    }

    private void UpdateStatusIndicators(Player player)
    {
        PlayerStats stats = player.GetComponent<PlayerStats>();
        PlayerController controller = player.GetComponent<PlayerController>();

        // Update dead indicator
        if (deadIndicator != null)
        {
            deadIndicator.SetActive(stats != null && stats.IsDead);
        }

        // Update moves left display
        if (movesLeftText != null && controller != null)
        {
            movesLeftText.text = $"Moves: {controller.currentMovesLeft}";
            
            // Color coding for moves left
            if (controller.currentMovesLeft <= 0)
            {
                movesLeftText.color = lowStatColor;
            }
            else
            {
                movesLeftText.color = normalStatColor;
            }
        }
    }

    private void UpdateStatText(TextMeshProUGUI textComponent, int statValue, string statName)
    {
        if (textComponent == null) return;

        textComponent.text = statValue.ToString();
        
        // Apply color coding based on stat value
        if (statValue <= lowStatThreshold)
        {
            textComponent.color = lowStatColor;
        }
        else if (statValue >= highStatThreshold)
        {
            textComponent.color = highStatColor;
        }
        else
        {
            textComponent.color = normalStatColor;
        }
    }

    private void UpdateIndexText(TextMeshProUGUI textComponent, int indexValue, string statName)
    {
        if (textComponent == null) return;

        textComponent.text = $"[{indexValue}]";
        
        // Color coding for index (red if 0, normal otherwise)
        textComponent.color = indexValue == 0 ? lowStatColor : normalStatColor;
    }

    private void UpdateStatBar(Slider barComponent, int statValue)
    {
        if (barComponent == null) return;

        // Assuming max stat value is around 8-10
        barComponent.maxValue = 8f;
        barComponent.value = statValue;
    }

    private void ClearDisplay()
    {
        if (playerNameText != null)
            playerNameText.text = "No Player";

        if (playerPortrait != null)
            playerPortrait.gameObject.SetActive(false);

        // Clear all stat displays
        ClearStatText(speedValueText);
        ClearStatText(mightValueText);
        ClearStatText(sanityValueText);
        ClearStatText(knowledgeValueText);

        ClearStatText(speedIndexText);
        ClearStatText(mightIndexText);
        ClearStatText(sanityIndexText);
        ClearStatText(knowledgeIndexText);

        if (movesLeftText != null)
            movesLeftText.text = "Moves: --";

        if (deadIndicator != null)
            deadIndicator.SetActive(false);
    }

    private void ClearStatText(TextMeshProUGUI textComponent)
    {
        if (textComponent != null)
        {
            textComponent.text = "--";
            textComponent.color = normalStatColor;
        }
    }

    // Public method for manual refresh
    public void RefreshDisplay()
    {
        if (TurnManager.Instance != null && TurnManager.Instance.CurrentPlayer != null)
        {
            UpdateDisplay(TurnManager.Instance.CurrentPlayer);
        }
    }
}