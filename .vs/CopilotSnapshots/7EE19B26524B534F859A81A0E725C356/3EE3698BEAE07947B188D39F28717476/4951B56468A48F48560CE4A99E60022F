using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Debug Info")]
    public CharacterDataSO data;

    // Các biến này lưu vị trí cái kẹp nhựa (Index từ 0 đến 8)
    [Range(0, 8)] public int currentSpeedIndex;
    [Range(0, 8)] public int currentMightIndex;
    [Range(0, 8)] public int currentSanityIndex;
    [Range(0, 8)] public int currentKnowledgeIndex;

    public bool IsDead { get; private set; } = false;

    public void Initialize(CharacterDataSO charData)
    {
        data = charData;
        IsDead = false;

        // Khởi tạo vị trí kẹp nhựa ban đầu
        currentSpeedIndex = data.speedTrack.startingIndex;
        currentMightIndex = data.mightTrack.startingIndex;
        currentSanityIndex = data.sanityTrack.startingIndex;
        currentKnowledgeIndex = data.knowledgeTrack.startingIndex;
    }

    // Lấy giá trị thực tế (Value) từ vị trí kẹp (Index)
    public int GetSpeed() => data.speedTrack.GetValueAt(currentSpeedIndex);
    public int GetMight() => data.mightTrack.GetValueAt(currentMightIndex);
    public int GetSanity() => data.sanityTrack.GetValueAt(currentSanityIndex);
    public int GetKnowledge() => data.knowledgeTrack.GetValueAt(currentKnowledgeIndex);

    // --- CÁC HÀM THAY ĐỔI CHỈ SỐ ---
    public void ModifyMight(int amount) => currentMightIndex = ApplyModification(currentMightIndex, data.mightTrack, amount);
    public void ModifySpeed(int amount) => currentSpeedIndex = ApplyModification(currentSpeedIndex, data.speedTrack, amount);
    public void ModifySanity(int amount) => currentSanityIndex = ApplyModification(currentSanityIndex, data.sanityTrack, amount);
    public void ModifyKnowledge(int amount) => currentKnowledgeIndex = ApplyModification(currentKnowledgeIndex, data.knowledgeTrack, amount);

    // Logic cốt lõi xử lý 9 mốc
    private int ApplyModification(int currentIndex, AttributeTrack track, int amount)
    {
        if (IsDead) return 0;

        int newIndex = currentIndex + amount;

        // 1. Giới hạn trên: Không bao giờ vượt quá Index 8 (Mốc cao nhất bên phải)
        if (newIndex > 8) newIndex = 8;

        // 2. Kiểm tra CHẾT: Nếu về Index 0 (Đầu lâu)
        if (newIndex <= 0)
        {
            newIndex = 0; // Gán chặt về 0
            HandleDeath();
        }

        return newIndex;
    }

    private void HandleDeath()
    {
        if (IsDead) return;
        IsDead = true;
        Debug.LogError($"NHÂN VẬT {data.characterName} ĐÃ CHẾT VÌ CHỈ SỐ VỀ 0!");

        // Disable PlayerController để không đi được nữa
        var controller = GetComponent<PlayerController>();
        if (controller != null) controller.enabled = false;
    }
}