using UnityEngine;

/// <summary>
/// Helper class for tile placement calculations and validation
/// Handles spacing, grid alignment, and door matching verification
/// </summary>
public static class TilePlacementHelper
{
    /// <summary>
    /// Calculate world position for a grid coordinate
    /// </summary>
    public static Vector3 GridToWorldPosition(Vector2Int gridCoord, float tileSize)
    {
        return new Vector3(gridCoord.x * tileSize, gridCoord.y * tileSize, -1f);
    }

    /// <summary>
    /// Calculate grid coordinate from world position
    /// </summary>
    public static Vector2Int WorldToGridPosition(Vector3 worldPos, float tileSize)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        return new Vector2Int(x, y);
    }

    /// <summary>
    /// Calculate door position in world space for a room
    /// </summary>
    public static Vector3 GetDoorPositionInWorld(Vector2Int gridCoord, DoorDirection doorDirection, float tileSize)
    {
        Vector3 roomCenter = GridToWorldPosition(gridCoord, tileSize);
        float doorOffset = tileSize * 0.45f;

        return doorDirection switch
        {
            DoorDirection.North => roomCenter + Vector3.up * doorOffset,
            DoorDirection.South => roomCenter + Vector3.down * doorOffset,
            DoorDirection.East => roomCenter + Vector3.right * doorOffset,
            DoorDirection.West => roomCenter + Vector3.left * doorOffset,
            _ => roomCenter
        };
    }

    /// <summary>
    /// Calculate door size relative to tile size
    /// </summary>
    public static float GetDoorSize(float tileSize)
    {
        return tileSize * 0.3f;
    }

    /// <summary>
    /// Validate door connection between two adjacent tiles
    /// </summary>
    public static bool ValidateDoorConnection(
        RoomTilePlacement tile1, Vector2Int pos1,
        RoomTilePlacement tile2, Vector2Int pos2,
        out string validationMessage)
    {
        validationMessage = "";
        
        // Get relative position
        Vector2Int diff = pos2 - pos1;
        if (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) != 1)
        {
            validationMessage = "Tiles are not adjacent!";
            return false;
        }

        // Determine direction
        DoorDirection dir = DirectionFromVector(diff);
        DoorDirection oppositeDir = GetOppositeDirection(dir);

        DoorDirection doors1 = tile1.GetRotatedDoors();
        DoorDirection doors2 = tile2.GetRotatedDoors();

        bool tile1HasDoor = (doors1 & dir) != 0;
        bool tile2HasDoor = (doors2 & oppositeDir) != 0;

        if (tile1HasDoor != tile2HasDoor)
        {
            validationMessage = $"Door mismatch! {tile1.tile.roomName} {(tile1HasDoor ? "has" : "no")} door, " +
                              $"{tile2.tile.roomName} {(tile2HasDoor ? "has" : "no")} door";
            return false;
        }

        validationMessage = tile1HasDoor ? "Connected (both have door)" : "Connected (both sealed)";
        return true;
    }

    /// <summary>
    /// Get all neighbors of a position
    /// </summary>
    public static Vector2Int[] GetAdjacentPositions(Vector2Int pos)
    {
        return new Vector2Int[]
        {
            pos + Vector2Int.up,      // North
            pos + Vector2Int.down,    // South
            pos + Vector2Int.right,   // East
            pos + Vector2Int.left     // West
        };
    }

    /// <summary>
    /// Get door direction between two positions
    /// </summary>
    public static DoorDirection GetDoorDirectionBetween(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        return DirectionFromVector(diff);
    }

    // ===== HELPER METHODS =====

    private static DoorDirection DirectionFromVector(Vector2Int diff)
    {
        if (diff == Vector2Int.up) return DoorDirection.North;
        if (diff == Vector2Int.down) return DoorDirection.South;
        if (diff == Vector2Int.right) return DoorDirection.East;
        if (diff == Vector2Int.left) return DoorDirection.West;
        return DoorDirection.None;
    }

    private static DoorDirection GetOppositeDirection(DoorDirection dir)
    {
        return dir switch
        {
            DoorDirection.North => DoorDirection.South,
            DoorDirection.South => DoorDirection.North,
            DoorDirection.East => DoorDirection.West,
            DoorDirection.West => DoorDirection.East,
            _ => DoorDirection.None
        };
    }
}
