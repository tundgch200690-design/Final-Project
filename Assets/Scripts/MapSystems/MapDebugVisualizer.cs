using UnityEngine;

/// <summary>
/// Visual debug window showing map generation and tile placement status
/// Displays door matching information and tile positions in-game
/// </summary>
public class MapDebugVisualizer : MonoBehaviour
{
    [Header("Debug Display Settings")]
    [SerializeField] private bool showDebugWindow = true;
    [SerializeField] private Rect windowRect = new Rect(10, 10, 400, 300);
    [SerializeField] private bool showTileList = true;
    [SerializeField] private bool showDoorMatchingInfo = true;

    private MapManager mapManager;
    private Vector2 scrollPosition = Vector2.zero;

    private void Start()
    {
        mapManager = MapManager.Instance;
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (!showDebugWindow || mapManager == null) return;

        GUI.Box(windowRect, "MAP DEBUG INFO", GUI.skin.window);

        GUILayout.BeginArea(new Rect(windowRect.x + 5, windowRect.y + 25, windowRect.width - 10, windowRect.height - 30));

        // Generation Status
        GUILayout.Label($"<b>Generation Status</b>", new GUIStyle(GUI.skin.label) { richText = true });
        GUILayout.Label($"Map Generated: {(mapManager.IsMapGenerated ? "<color=green>YES</color>" : "<color=red>NO</color>")}",
            new GUIStyle(GUI.skin.label) { richText = true });
        GUILayout.Label($"Rooms Placed: {mapManager.GetTotalRoomsPlaced()}");
        GUILayout.Label($"Unused Tiles: {mapManager.GetRemainingTiles()}");
        GUILayout.Space(10);

        // Settings
        GUILayout.Label("<b>Debug Settings</b>", new GUIStyle(GUI.skin.label) { richText = true });
        showDoorMatchingInfo = GUILayout.Toggle(showDoorMatchingInfo, "Show Door Matching");
        showTileList = GUILayout.Toggle(showTileList, "Show Tile List");
        GUILayout.Space(10);

        // Tile List
        if (showTileList)
        {
            GUILayout.Label("<b>Placed Tiles</b>", new GUIStyle(GUI.skin.label) { richText = true });
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(150));

            foreach (var kvp in mapManager.GetMapGrid())
            {
                Vector2Int coord = kvp.Key;
                RoomTilePlacement placement = kvp.Value;
                DoorDirection doors = placement.GetRotatedDoors();

                string doorString = "";
                if ((doors & DoorDirection.North) != 0) doorString += "N";
                if ((doors & DoorDirection.South) != 0) doorString += "S";
                if ((doors & DoorDirection.East) != 0) doorString += "E";
                if ((doors & DoorDirection.West) != 0) doorString += "W";

                GUILayout.Label($"{placement.tile.roomName} @ ({coord.x}, {coord.y}) | Rot:{placement.rotationSteps} | Doors:{doorString}");
            }

            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
    }
#endif

    /// <summary>
    /// Log detailed validation of all door connections
    /// </summary>
    public void ValidateAllConnections()
    {
        Debug.Log("[MapDebugVisualizer] Validating all door connections...");

        int validConnections = 0;
        int invalidConnections = 0;

        var mapGrid = mapManager.GetMapGrid();

        foreach (var kvp in mapGrid)
        {
            Vector2Int coord = kvp.Key;
            RoomTilePlacement placement = kvp.Value;

            // Check all 4 directions
            Vector2Int[] neighbors = new Vector2Int[]
            {
                coord + Vector2Int.up,
                coord + Vector2Int.down,
                coord + Vector2Int.right,
                coord + Vector2Int.left
            };

            DoorDirection[] directions = new DoorDirection[]
            {
                DoorDirection.North,
                DoorDirection.South,
                DoorDirection.East,
                DoorDirection.West
            };

            for (int i = 0; i < 4; i++)
            {
                if (mapGrid.ContainsKey(neighbors[i]))
                {
                    bool isValid = TilePlacementHelper.ValidateDoorConnection(
                        placement, coord,
                        mapGrid[neighbors[i]], neighbors[i],
                        out string message);

                    if (isValid)
                        validConnections++;
                    else
                        invalidConnections++;

                    string status = isValid ? "<color=green>?</color>" : "<color=red>?</color>";
                    Debug.Log($"{status} {placement.tile.roomName} ? {directions[i]} | {message}",
                        mapManager.gameObject);
                }
            }
        }

        Debug.Log($"<color=green>VALIDATION COMPLETE</color>: {validConnections} valid, {invalidConnections} invalid",
            mapManager.gameObject);
    }
}
