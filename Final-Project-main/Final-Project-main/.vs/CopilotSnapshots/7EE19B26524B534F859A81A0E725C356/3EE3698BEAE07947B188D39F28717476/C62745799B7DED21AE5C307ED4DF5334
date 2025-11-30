using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;

    [Header("Danh sách người chơi")]
    public List<PlayerController> allPlayers = new List<PlayerController>();

    [Header("Trạng thái")]
    public int currentPlayerIndex = 0; // Index của người đang chơi trong list

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Tự động tìm tất cả Player có trong Scene lúc bắt đầu
        // (Sau này làm Multiplayer online sẽ logic khác, đây là logic Local/Offline)
        PlayerController[] foundPlayers = FindObjectsOfType<PlayerController>();
        allPlayers.AddRange(foundPlayers);

        if (allPlayers.Count > 0)
        {
            StartTurn(0); // Bắt đầu với người đầu tiên
        }
    }

    public void StartTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;
        PlayerController activePlayer = allPlayers[currentPlayerIndex];

        Debug.Log($"--- Lượt của: {activePlayer.characterData.characterName} ---");

        // 1. Reset bước di chuyển cho nhân vật đó
        activePlayer.StartTurn();

        // 2. Cập nhật UI hiển thị thông tin người đó
        PlayerStats stats = activePlayer.GetComponent<PlayerStats>();
        if (PlayerHUD.Instance != null)
        {
            PlayerHUD.Instance.UpdateHUD(stats);
        }

        // 3. Khóa di chuyển của những người KHÁC (nếu cần)
        foreach (var p in allPlayers)
        {
            // Chỉ bật script di chuyển cho người hiện tại
            p.enabled = (p == activePlayer);
        }
    }

    public void EndCurrentTurn()
    {
        Debug.Log("Kết thúc lượt!");

        // Chuyển sang người tiếp theo
        currentPlayerIndex++;

        // Nếu vượt quá danh sách thì quay về 0 (Vòng tròn)
        if (currentPlayerIndex >= allPlayers.Count)
        {
            currentPlayerIndex = 0;
            Debug.Log(">>> Bắt đầu vòng chơi mới! <<<");
        }

        StartTurn(currentPlayerIndex);
    }
}