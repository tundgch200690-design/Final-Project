using UnityEngine;

/// <summary>
/// Player MonoBehaviour - represents an actual player in the game scene
/// Contains references to PlayerStats and PlayerController components
/// </summary>
public class Player : MonoBehaviour
{
    [Header("Player Information")]
    public string playerName;
    public Sprite avatar;
    
    [Header("Character Data Reference")]
    [SerializeField] private CharacterDataSO characterData;
    
    // Cache components to avoid repeated GetComponent calls
    private PlayerStats cachedStats;
    private PlayerController cachedController;
    
    // Properties for easy access to components
    public PlayerStats Stats 
    { 
        get 
        { 
            if (cachedStats == null) 
                cachedStats = GetComponent<PlayerStats>();
            return cachedStats; 
        } 
    }
    
    public PlayerController Controller 
    { 
        get 
        { 
            if (cachedController == null) 
                cachedController = GetComponent<PlayerController>();
            return cachedController; 
        } 
    }
    
    public CharacterDataSO CharacterData 
    { 
        get => characterData; 
        set => characterData = value; 
    }

    // COMPATIBILITY PROPERTIES - for backward compatibility with old code
    public int might => GetStat("might");
    public int speed => GetStat("speed");
    public int sanity => GetStat("sanity");
    public int knowledge => GetStat("knowledge");

    private void Awake()
    {
        // Ensure we have required components
        if (GetComponent<PlayerStats>() == null)
        {
            gameObject.AddComponent<PlayerStats>();
        }
        
        if (GetComponent<PlayerController>() == null)
        {
            gameObject.AddComponent<PlayerController>();
        }

        // Cache components immediately
        cachedStats = GetComponent<PlayerStats>();
        cachedController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        // ALWAYS initialize if we have character data
        if (characterData != null)
        {
            Initialize(characterData);
        }
        else
        {
            Debug.LogWarning($"[Player] {gameObject.name} has no CharacterDataSO assigned!");
        }
    }

    /// <summary>
    /// Initialize this player with character data
    /// </summary>
    public void Initialize(CharacterDataSO data)
    {
        if (data == null)
        {
            Debug.LogError($"[Player] Trying to initialize {gameObject.name} with null CharacterDataSO!");
            return;
        }

        characterData = data;
        playerName = data.characterName;
        avatar = data.portrait;
        
        // Initialize components - ensure they exist first
        if (Stats != null)
        {
            Stats.Initialize(data);
            Debug.Log($"[Player] {playerName} stats initialized successfully");
        }
        else
        {
            Debug.LogError($"[Player] PlayerStats component missing on {gameObject.name}!");
        }
        
        if (Controller != null)
        {
            Controller.characterData = data;
            Debug.Log($"[Player] {playerName} controller initialized successfully");
        }
        else
        {
            Debug.LogError($"[Player] PlayerController component missing on {gameObject.name}!");
        }
    }

    /// <summary>
    /// Get current stat value by name
    /// </summary>
    public int GetStat(string statName)
    {
        if (Stats == null) 
        {
            Debug.LogWarning($"[Player] PlayerStats is null for {gameObject.name}");
            return 0;
        }

        switch (statName.ToLower())
        {
            case "might": return Stats.GetMight();
            case "speed": return Stats.GetSpeed();
            case "sanity": return Stats.GetSanity();
            case "knowledge": return Stats.GetKnowledge();
            default: return 0;
        }
    }

    /// <summary>
    /// Modify stat by amount
    /// </summary>
    public void ModifyStat(string statName, int amount)
    {
        if (Stats == null)
        {
            Debug.LogWarning($"[Player] PlayerStats is null for {gameObject.name}");
            return;
        }

        switch (statName.ToLower())
        {
            case "might": Stats.ModifyMight(amount); break;
            case "speed": Stats.ModifySpeed(amount); break;
            case "sanity": Stats.ModifySanity(amount); break;
            case "knowledge": Stats.ModifyKnowledge(amount); break;
        }
    }
}