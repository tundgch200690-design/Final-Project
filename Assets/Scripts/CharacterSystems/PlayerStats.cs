using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Debug Info")]
    public CharacterDataSO data;

    // Current positions on attribute tracks (0-8)
    [Range(0, 8)] public int currentSpeedIndex;
    [Range(0, 8)] public int currentMightIndex;
    [Range(0, 8)] public int currentSanityIndex;
    [Range(0, 8)] public int currentKnowledgeIndex;

    public bool IsDead { get; private set; } = false;

    public void Initialize(CharacterDataSO charData)
    {
        data = charData;
        IsDead = false;

        // Initialize starting positions
        currentSpeedIndex = data.speedTrack.startingIndex;
        currentMightIndex = data.mightTrack.startingIndex;
        currentSanityIndex = data.sanityTrack.startingIndex;
        currentKnowledgeIndex = data.knowledgeTrack.startingIndex;
    }

    // Get actual values from attribute tracks
    public int GetSpeed() => data.speedTrack.GetValueAt(currentSpeedIndex);
    public int GetMight() => data.mightTrack.GetValueAt(currentMightIndex);
    public int GetSanity() => data.sanityTrack.GetValueAt(currentSanityIndex);
    public int GetKnowledge() => data.knowledgeTrack.GetValueAt(currentKnowledgeIndex);

    // Modify stats
    public void ModifyMight(int amount) => currentMightIndex = ApplyModification(currentMightIndex, data.mightTrack, amount);
    public void ModifySpeed(int amount) => currentSpeedIndex = ApplyModification(currentSpeedIndex, data.speedTrack, amount);
    public void ModifySanity(int amount) => currentSanityIndex = ApplyModification(currentSanityIndex, data.sanityTrack, amount);
    public void ModifyKnowledge(int amount) => currentKnowledgeIndex = ApplyModification(currentKnowledgeIndex, data.knowledgeTrack, amount);

    private int ApplyModification(int currentIndex, AttributeTrack track, int amount)
    {
        if (IsDead) return 0;

        int newIndex = currentIndex + amount;

        // Clamp to valid range (0-8)
        if (newIndex > 8) newIndex = 8;

        // Check for death (index 0)
        if (newIndex <= 0)
        {
            newIndex = 0;
            HandleDeath();
        }

        return newIndex;
    }

    private void HandleDeath()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.LogError($"CHARACTER {data.characterName} IS DEAD!");

        // Disable PlayerController
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;
    }
}