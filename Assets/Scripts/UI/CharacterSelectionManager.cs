using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    public static CharacterSelectionManager Instance { get; private set; }

    [Header("Character Selection")]
    [SerializeField] private List<Player> availableCharacters = new List<Player>();
    private List<Player> selectedPlayers = new List<Player>();

    [Header("Game Settings")]
    [SerializeField] private int minPlayers = 3;
    [SerializeField] private int maxPlayers = 6;

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

    public void SelectCharacter(Player character)
    {
        if (character == null) return;

        if (selectedPlayers.Contains(character))
        {
            selectedPlayers.Remove(character);
            Debug.Log($"Deselected: {character.playerName}");
        }
        else if (selectedPlayers.Count < maxPlayers)
        {
            selectedPlayers.Add(character);
            Debug.Log($"Selected: {character.playerName}");
        }
        else
        {
            Debug.LogWarning($"Maximum players ({maxPlayers}) reached!");
        }
    }

    public void DeselectCharacter(Player character)
    {
        if (character != null && selectedPlayers.Contains(character))
        {
            selectedPlayers.Remove(character);
            Debug.Log($"Deselected: {character.playerName}");
        }
    }

    public bool IsCharacterSelected(Player character)
    {
        return selectedPlayers.Contains(character);
    }

    public void StartGame()
    {
        if (selectedPlayers.Count < minPlayers)
        {
            Debug.LogError($"Need at least {minPlayers} players to start! Current: {selectedPlayers.Count}");
            return;
        }

        if (selectedPlayers.Count > maxPlayers)
        {
            Debug.LogError($"Too many players! Maximum: {maxPlayers}");
            return;
        }

        // Populate TurnManager with selected players
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.players.Clear();
            foreach (Player player in selectedPlayers)
            {
                TurnManager.Instance.players.Add(player);
            }
            Debug.Log($"Game started with {selectedPlayers.Count} players!");
        }
    }

    public List<Player> GetSelectedPlayers()
    {
        return new List<Player>(selectedPlayers);
    }

    public List<Player> GetAvailableCharacters()
    {
        return availableCharacters;
    }

    public void ClearSelection()
    {
        selectedPlayers.Clear();
    }

    public int GetSelectedPlayerCount()
    {
        return selectedPlayers.Count;
    }

    public int GetMinPlayers()
    {
        return minPlayers;
    }

    public int GetMaxPlayers()
    {
        return maxPlayers;
    }
}
