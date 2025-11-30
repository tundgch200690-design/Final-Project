using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance;

    [Header("UI Components")]
    public Image avatarImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI mightText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI sanityText;
    public TextMeshProUGUI knowledgeText;

    [Header("Legacy Text Fallback")]
    public Text legacyNameText;
    public Text legacyMightText;
    public Text legacySpeedText;
    public Text legacySanityText;
    public Text legacyKnowledgeText;

    private Player currentPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.AddListener(OnPlayerChanged);

            if (TurnManager.Instance.CurrentPlayer != null)
            {
                OnPlayerChanged(TurnManager.Instance.CurrentPlayer);
            }
        }
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged.RemoveListener(OnPlayerChanged);
        }
    }

    public void OnPlayerChanged(Player newPlayer)
    {
        currentPlayer = newPlayer;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (currentPlayer == null) return;

        if (avatarImage != null && currentPlayer.avatar != null)
        {
            avatarImage.sprite = currentPlayer.avatar;
        }

        UpdateText(nameText, legacyNameText, currentPlayer.playerName);

        // FIXED: Use GetStat method instead of direct property access
        UpdateText(mightText, legacyMightText, $"Might: {currentPlayer.GetStat("might")}");
        UpdateText(speedText, legacySpeedText, $"Speed: {currentPlayer.GetStat("speed")}");
        UpdateText(sanityText, legacySanityText, $"Sanity: {currentPlayer.GetStat("sanity")}");
        UpdateText(knowledgeText, legacyKnowledgeText, $"Knowledge: {currentPlayer.GetStat("knowledge")}");
    }

    private void UpdateText(TextMeshProUGUI tmpText, Text legacyText, string value)
    {
        if (tmpText != null)
        {
            tmpText.text = value;
        }
        else if (legacyText != null)
        {
            legacyText.text = value;
        }
    }
}