using UnityEngine;
using UnityEngine.UI;
using TMPro; // Nhớ dùng thư viện này nếu xài TextMeshPro

public class PlayerHUD : MonoBehaviour
{
    public static PlayerHUD Instance; // Singleton để dễ gọi từ nơi khác

    [Header("UI References")]
    public Image avatarImage;
    public TextMeshProUGUI nameText;

    [Header("Stats Text")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI mightText;
    public TextMeshProUGUI sanityText;
    public TextMeshProUGUI knowledgeText;

    [Header("Controls")]
    public Button endTurnButton;

    private PlayerStats currentPlayer;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Gán sự kiện click cho nút End Turn
        endTurnButton.onClick.AddListener(OnEndTurnClicked);
    }

    // Hàm này được gọi khi đến lượt người chơi mới
    public void UpdateHUD(PlayerStats player)
    {
        currentPlayer = player;

        // Cập nhật thông tin tĩnh
        avatarImage.sprite = player.data.portrait;
        nameText.text = player.data.characterName;

        // Cập nhật chỉ số động
        RefreshStats();
    }

    // Gọi hàm này mỗi khi chỉ số thay đổi (bị đánh, buff...)
    public void RefreshStats()
    {
        if (currentPlayer == null) return;

        speedText.text = $"Speed: {currentPlayer.GetSpeed()}";
        mightText.text = $"Might: {currentPlayer.GetMight()}";
        sanityText.text = $"Sanity: {currentPlayer.GetSanity()}";
        knowledgeText.text = $"Knowledge: {currentPlayer.GetKnowledge()}";

        // Đổi màu đỏ nếu chỉ số sắp chết (Index thấp)
        // (Đây là bài tập mở rộng cho bạn sau này)
    }

    private void OnEndTurnClicked()
    {
        // Gọi sang TurnManager để kết thúc lượt
        TurnManager.Instance.EndCurrentTurn();
    }
}