using UnityEngine;

/// <summary>
/// Visualizes door connections between adjacent tiles
/// Shows which doors match correctly (green) and which don't (red)
/// </summary>
public class DoorConnectionVisualizer : MonoBehaviour
{
    [Header("Connection Visual Settings")]
    [SerializeField] private float lineWidth = 0.05f;
    [SerializeField] private Color validConnectionColor = Color.green;
    [SerializeField] private Color invalidConnectionColor = Color.red;
    [SerializeField] private bool showConnectionLines = true;

    private MapManager mapManager;

    private void Start()
    {
        mapManager = MapManager.Instance;
    }

    /// <summary>
    /// Draw debug lines showing door connections between tiles
    /// </summary>
    public void DrawConnectionDebug()
    {
        if (!showConnectionLines || mapManager == null) return;

        // This would be called from MapManager to visualize all connections
        // For now, we'll use OnDrawGizmos in MapManager instead
    }

    /// <summary>
    /// Check if two adjacent tiles have matching doors (call from MapManager)
    /// </summary>
    public static bool CheckDoorConnection(
        RoomTilePlacement tile1, Vector2Int pos1,
        RoomTilePlacement tile2, Vector2Int pos2,
        out bool hasBothDoors, out bool hasNoDoors)
    {
        hasBothDoors = false;
        hasNoDoors = false;

        Vector2Int diff = pos2 - pos1;
        
        // Determine direction from tile1 to tile2
        DoorDirection direction = Vector2IntToDirection(diff);
        DoorDirection oppositeDir = GetOppositeDirection(direction);

        DoorDirection doors1 = tile1.GetRotatedDoors();
        DoorDirection doors2 = tile2.GetRotatedDoors();

        bool tile1HasDoor = (doors1 & direction) != 0;
        bool tile2HasDoor = (doors2 & oppositeDir) != 0;

        hasBothDoors = tile1HasDoor && tile2HasDoor;
        hasNoDoors = !tile1HasDoor && !tile2HasDoor;

        // Connection is valid if both have door OR both don't have door
        return hasBothDoors || hasNoDoors;
    }

    private static DoorDirection Vector2IntToDirection(Vector2Int diff)
    {
        if (diff.y > 0) return DoorDirection.North;
        if (diff.y < 0) return DoorDirection.South;
        if (diff.x > 0) return DoorDirection.East;
        if (diff.x < 0) return DoorDirection.West;
        return DoorDirection.None;
    }

    private static DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.North: return DoorDirection.South;
            case DoorDirection.South: return DoorDirection.North;
            case DoorDirection.East: return DoorDirection.West;
            case DoorDirection.West: return DoorDirection.East;
            default: return DoorDirection.None;
        }
    }
}
