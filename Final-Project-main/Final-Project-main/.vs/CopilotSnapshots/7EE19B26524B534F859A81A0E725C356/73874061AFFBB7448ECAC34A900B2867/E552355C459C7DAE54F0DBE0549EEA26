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

/// <summary>
/// Represents a room tile placement with rotation information
/// </summary>
[System.Serializable]
public class RoomTilePlacement
{
    public RoomTileSO tile;
    public int rotationSteps; // 0, 1, 2, 3 (90° increments)

    public RoomTilePlacement(RoomTileSO tile, int rotationSteps = 0)
    {
        this.tile = tile;
        this.rotationSteps = rotationSteps;
    }

    public DoorDirection GetRotatedDoors()
    {
        return tile.RotateDoors(rotationSteps);
    }
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

    /// <summary>
    /// Xoay cửa của phòng theo chiều kim đồng hồ (0°, 90°, 180°, 270°)
    /// rotationSteps: 0 = 0°, 1 = 90°, 2 = 180°, 3 = 270°
    /// </summary>
    public DoorDirection RotateDoors(int rotationSteps)
    {
        rotationSteps = ((rotationSteps % 4) + 4) % 4;
        
        DoorDirection rotatedDoors = doors;
        for (int i = 0; i < rotationSteps; i++)
        {
            rotatedDoors = RotateDoorsOnce(rotatedDoors);
        }
        return rotatedDoors;
    }

    /// <summary>
    /// Xoay cửa 90° theo chiều kim đồng hồ
    /// North → East → South → West → North
    /// </summary>
    private DoorDirection RotateDoorsOnce(DoorDirection doors)
    {
        DoorDirection result = DoorDirection.None;

        if ((doors & DoorDirection.North) != 0) result |= DoorDirection.East;
        if ((doors & DoorDirection.East) != 0) result |= DoorDirection.South;
        if ((doors & DoorDirection.South) != 0) result |= DoorDirection.West;
        if ((doors & DoorDirection.West) != 0) result |= DoorDirection.North;

        return result;
    }

    /// <summary>
    /// Kiểm tra xem có hướng xoay nào khớp với cửa yêu cầu không
    /// </summary>
    public int GetValidRotation(DoorDirection requiredDoors)
    {
        for (int rotation = 0; rotation < 4; rotation++)
        {
            DoorDirection rotatedDoors = RotateDoors(rotation);
            if (rotatedDoors == requiredDoors)
            {
                return rotation;
            }
        }
        return -1;
    }

    /// <summary>
    /// Lấy danh sách tất cả các hướng xoay có thể của phòng này
    /// </summary>
    public DoorDirection[] GetAllRotations()
    {
        DoorDirection[] rotations = new DoorDirection[4];
        for (int i = 0; i < 4; i++)
        {
            rotations[i] = RotateDoors(i);
        }
        return rotations;
    }
}