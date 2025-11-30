using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CharacterSelectionButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image avatarImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI statsText;
    [SerializeField] private Button selectionButton;
    [SerializeField] private Image selectionHighlight;

    private Player character;
    private Action onSelectionChanged;
    private bool hasInitialized = false;

    private void Start()
    {
        // Only setup button listener if not already done during Initialize
        if (!hasInitialized && selectionButton != null)
        {
            selectionButton.onClick.AddListener(OnButtonClicked);
        }
    }

    public void Initialize(Player playerCharacter, Action onSelectionCallback)
    {
        character = playerCharacter;
        onSelectionChanged = onSelectionCallback;
        hasInitialized = true;

        // Find components if not already assigned
        if (selectionButton == null)
        {
            selectionButton = GetComponent<Button>();
        }

        if (avatarImage == null)
        {
            // Try to find image component on root or in children
            Image[] images = GetComponentsInChildren<Image>();
            if (images.Length > 0)
            {
                avatarImage = images[0]; // First image is usually the avatar
            }
        }

        if (nameText == null)
        {
            // Try to find by name or get first TextMeshProUGUI
            nameText = GetComponentInChildren<TextMeshProUGUI>();
        }

        // Setup button listener
        if (selectionButton != null)
        {
            selectionButton.onClick.AddListener(OnButtonClicked);
        }

        if (character != null)
        {
            // Set avatar
            if (avatarImage != null && character.avatar != null)
            {
                avatarImage.sprite = character.avatar;
            }

            // Set name
            if (nameText != null)
            {
                nameText.text = character.playerName;
            }

            // Set stats display - FIXED: Use GetStat method instead of direct properties
            if (statsText != null)
            {
                int might = character.GetStat("might");
                int speed = character.GetStat("speed");
                int sanity = character.GetStat("sanity");
                int knowledge = character.GetStat("knowledge");
                
                statsText.text = $"M:{might} S:{speed} Sa:{sanity} K:{knowledge}";
            }
        }

        UpdateHighlight();
    }

    private void OnButtonClicked()
    {
        if (character == null) return;

        CharacterSelectionManager manager = CharacterSelectionManager.Instance;
        if (manager == null) return;

        if (manager.IsCharacterSelected(character))
        {
            manager.DeselectCharacter(character);
        }
        else
        {
            manager.SelectCharacter(character);
        }

        UpdateHighlight();
        onSelectionChanged?.Invoke();
    }

    private void UpdateHighlight()
    {
        if (selectionHighlight == null) return;

        bool isSelected = CharacterSelectionManager.Instance != null && 
                         CharacterSelectionManager.Instance.IsCharacterSelected(character);
        
        selectionHighlight.gameObject.SetActive(isSelected);
    }

    private void OnDestroy()
    {
        if (selectionButton != null)
        {
            selectionButton.onClick.RemoveListener(OnButtonClicked);
        }
    }
}
