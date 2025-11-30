using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Betrayal/Character Data")]
public class CharacterDataSO : ScriptableObject
{
    [Header("Thông tin Nhân vật")]
    public string characterName;
    public Sprite portrait;      // Ảnh đại diện tròn
    public Sprite tokenSprite;   // Hình nhân vật trên map
    public Color tokenColor;     // Màu sắc (để phân biệt cặp nhân vật cùng màu)

    [Header("UI Display")]
    public Sprite characterPortrait; // For HUD display

    [Header("Bảng Chỉ số (Stat Tracks)")]
    // 4 thanh chỉ số riêng biệt
    public AttributeTrack speedTrack;
    public AttributeTrack mightTrack;
    public AttributeTrack sanityTrack;
    public AttributeTrack knowledgeTrack;

    private void OnValidate()
    {
        // Auto-assign characterPortrait if not set
        if (characterPortrait == null && portrait != null)
        {
            characterPortrait = portrait;
        }
    }
}