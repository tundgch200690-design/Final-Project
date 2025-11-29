using UnityEngine;

// 1. Thay đổi Enum: Thêm [System.Flags] và gán giá trị theo lũy thừa 2 (1, 2, 4...)
[System.Flags]
public enum FloorLayer
{
    None = 0,
    Basement = 1,  // 001
    Ground = 2,  // 010
    Upper = 4,  // 100

    // Tiện ích: Gộp Ground và Upper (thường gặp)
    GroundAndUpper = Ground | Upper,
    // Tiện ích: Tất cả các tầng (hiếm gặp nhưng có thể dùng cho thang máy)
    All = Basement | Ground | Upper
}

[System.Flags]
public enum DoorDirection
{
    None = 0,
    North = 1,
    East = 2,
    South = 4,
    West = 8
}

[CreateAssetMenu(fileName = "NewRoomTile", menuName = "Betrayal/Room Tile")]
public class RoomTileSO : ScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string roomName;
    public Sprite roomSprite;

    // 2. Sử dụng Enum mới
    // Trong Inspector của Unity, bạn giờ có thể tích chọn nhiều ô cùng lúc!
    public FloorLayer allowedFloors;

    [Header("Logic Kết nối")]
    public DoorDirection doors;

    [Header("Sự kiện")]
    public bool hasOmen;
    public bool hasItem;
    public bool hasEvent;

    // --- CÁC HÀM TIỆN ÍCH (HELPER METHODS) ---

    // Kiểm tra xem phòng này có được phép đặt ở tầng 'floorToCheck' không
    public bool CanPlaceOnFloor(FloorLayer floorToCheck)
    {
        // Phép toán Bitwise AND (&): Nếu kết quả khác 0 nghĩa là có khớp
        return (allowedFloors & floorToCheck) != 0;
    }

    public bool HasDoor(DoorDirection dir)
    {
        return (doors & dir) != 0;
    }
}