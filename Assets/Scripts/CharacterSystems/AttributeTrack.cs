using UnityEngine;

[System.Serializable]
public class AttributeTrack
{
    // Quy định cứng: Luôn có 9 mốc. Index 0 luôn là Chết.
    // Mặc định tạo sẵn mảng 9 số 0 để bạn điền vào.
    public int[] values = new int[9];

    [Tooltip("Vị trí bắt đầu (Số màu xanh lá). Nhập từ 1 đến 8.")]
    [Range(1, 8)]
    public int startingIndex;

    public int GetValueAt(int index)
    {
        // Đảm bảo an toàn không vượt quá giới hạn mảng
        if (values == null || values.Length == 0) return 0;
        int clampedIndex = Mathf.Clamp(index, 0, values.Length - 1);
        return values[clampedIndex];
    }
}