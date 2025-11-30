using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{   
    public static MapManager Instance;

    [Header("Cài đặt Hệ thống")]
    public float tileSize = 2.0f;
    public int maxIterations = 500;

    [Header("Cấu hình Tile Cố định (Anchor Rooms)")]
    public RoomTileSO upperLanding;
    public RoomTileSO basementLanding;

    [Header("Bộ 3 Phòng Ground")]
    public RoomTileSO foyer;
    public RoomTileSO grandStaircase;
    public RoomTileSO entranceHall;

    [Header("Kho Dữ liệu Ngẫu nhiên")]
    public List<RoomTileSO> tileDeck;

    [Header("Cấu hình Generation")]
    [Tooltip("Số lần thử lại khi không tìm được bài khớp")]
    public int retryAttempts = 3;
    [Tooltip("Tự động sinh bản đồ khi Start")]
    public bool autoGenerateOnStart = true;

    [Header("Cấu hình Visualization")]
    [Tooltip("Hiển thị gizmo grid và door connections")]
    public bool showDebugGizmos = true;
    [Tooltip("Hiển thị door visuals trên mỗi tile")]
    public bool showDoorVisuals = true;
    [Tooltip("Hiển thị grid layout")]
    public bool showGridLayout = true;
    [SerializeField] private Color gridColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    // Trạng thái Initialization
    private bool isMapGenerated = false;
    public bool IsMapGenerated => isMapGenerated;

    // Dữ liệu lưu trữ
    private Dictionary<Vector2Int, RoomTilePlacement> mapGrid = new Dictionary<Vector2Int, RoomTilePlacement>();
    private Dictionary<Vector2Int, GameObject> roomGameObjects = new Dictionary<Vector2Int, GameObject>();
    private List<Vector2Int> availableSpots = new List<Vector2Int>();
    private List<RoomTileSO> remainingTiles = new List<RoomTileSO>();

    // Định nghĩa khoảng cách tầng
    private readonly Vector2Int UPPER_OFFSET = new Vector2Int(0, 50);
    private readonly Vector2Int BASEMENT_OFFSET = new Vector2Int(0, -50);

    private void Awake()
    {
        if (Instance == null) 
        {
            Instance = this;
            Debug.Log("[MapManager] Initialized as Singleton");
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (autoGenerateOnStart)
        {
            InitializeGame();
        }
        else
        {
            // Initialize remaining tiles for dynamic discovery
            remainingTiles = new List<RoomTileSO>(tileDeck);
            // Remove anchor rooms from deck
            if (remainingTiles.Contains(foyer)) remainingTiles.Remove(foyer);
            if (remainingTiles.Contains(grandStaircase)) remainingTiles.Remove(grandStaircase);
            if (remainingTiles.Contains(entranceHall)) remainingTiles.Remove(entranceHall);
            if (remainingTiles.Contains(upperLanding)) remainingTiles.Remove(upperLanding);
            if (remainingTiles.Contains(basementLanding)) remainingTiles.Remove(basementLanding);
        }
    }

    /// <summary>
    /// Initialize game with all anchor rooms and dynamic discovery for remaining
    /// </summary>
    public void InitializeGame()
    {
        if (isMapGenerated)
        {
            Debug.LogWarning("[MapManager] Game already initialized!");
            return;
        }

        Debug.Log("[MapManager] ===== INITIALIZING GAME =====");
        
        // Initialize remaining tiles for dynamic discovery
        remainingTiles = new List<RoomTileSO>(tileDeck);
        
        // 1. Setup UPPER FLOOR
        if (upperLanding != null) 
        {
            PlaceTileLogic(UPPER_OFFSET, upperLanding, 0);
            Debug.Log($"[MapManager] Placed anchor: {upperLanding.roomName} at {UPPER_OFFSET}");
        }

        // 2. Setup BASEMENT
        if (basementLanding != null) 
        {
            PlaceTileLogic(BASEMENT_OFFSET, basementLanding, 0);
            Debug.Log($"[MapManager] Placed anchor: {basementLanding.roomName} at {BASEMENT_OFFSET}");
        }

        // 3. Setup GROUND FLOOR - 3 anchor rooms
        if (grandStaircase != null) 
        {
            PlaceTileLogic(new Vector2Int(-1, 0), grandStaircase, 0);
            Debug.Log($"[MapManager] Placed anchor: {grandStaircase.roomName} at (-1, 0)");
        }

        if (foyer != null) 
        {
            PlaceTileLogic(new Vector2Int(0, 0), foyer, 0);
            Debug.Log($"[MapManager] Placed anchor: {foyer.roomName} at (0, 0)");
        }

        if (entranceHall != null) 
        {
            PlaceTileLogic(new Vector2Int(1, 0), entranceHall, 0);
            Debug.Log($"[MapManager] Placed anchor: {entranceHall.roomName} at (1, 0)");
        }
        
        // Remove anchor rooms from deck to avoid duplicates during discovery
        if (remainingTiles.Contains(foyer)) remainingTiles.Remove(foyer);
        if (remainingTiles.Contains(grandStaircase)) remainingTiles.Remove(grandStaircase);
        if (remainingTiles.Contains(entranceHall)) remainingTiles.Remove(entranceHall);
        if (remainingTiles.Contains(upperLanding)) remainingTiles.Remove(upperLanding);
        if (remainingTiles.Contains(basementLanding)) remainingTiles.Remove(basementLanding);

        isMapGenerated = true;
        Debug.Log("[MapManager] ===== GAME INITIALIZED =====");
    }

    /// <summary>
    /// Sinh bản đồ đầy đủ (gọi từ GameManager nếu cần)
    /// </summary>
    public void GenerateMap()
    {
        if (isMapGenerated)
        {
            Debug.LogWarning("[MapManager] Map already generated! Call ClearMap() first if you want to regenerate.");
            return;
        }

        Debug.Log("[MapManager] ===== STARTING FULL MAP GENERATION =====");
        Debug.Log("[MapManager] Step 1: Setting up anchor rooms...");
        
        SetupFixedMap();
        
        Debug.Log("[MapManager] Step 2: Generating random rooms...");
        GenerateRandomMap();
        
        Debug.Log("[MapManager] Step 3: Finalizing map...");
        LogMapSummary();

        isMapGenerated = true;
        Debug.Log("[MapManager] ===== MAP GENERATION COMPLETE =====");
    }

    /// <summary>
    /// Try to discover a new room at the target position
    /// Returns true if a room was placed, false otherwise
    /// </summary>
    public bool TryDiscoverRoom(Vector2Int targetPos)
    {
        // If room already exists, no discovery needed
        if (mapGrid.ContainsKey(targetPos))
        {
            return true;
        }

        FloorLayer currentLayer = DetermineFloorLayer(targetPos);
        (RoomTileSO selectedTile, int rotation) = FindMatchingTileWithRotation(targetPos, currentLayer);

        if (selectedTile != null)
        {
            PlaceTileLogic(targetPos, selectedTile, rotation);
            remainingTiles.Remove(selectedTile);
            Debug.Log($"[MapManager] Discovered new room: {selectedTile.roomName} at {targetPos}");
            return true;
        }

        Debug.LogWarning($"[MapManager] Could not find matching room for position {targetPos}");
        return false;
    }

    /// <summary>
    /// Công khai: Xóa bản đồ hiện tại (để sinh lại)
    /// </summary>
    public void ClearMap()
    {
        Debug.Log("[MapManager] Clearing map...");
        
        // Xóa tất cả GameObject rooms
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Reset dữ liệu
        mapGrid.Clear();
        availableSpots.Clear();
        remainingTiles.Clear();
        isMapGenerated = false;

        Debug.Log("[MapManager] Map cleared. Ready for regeneration.");
    }

    // --- SETUP BẢN ĐỒ (ANCHOR ROOMS) ---
    private void SetupFixedMap()
    {
        // 1. Setup UPPER FLOOR
        if (upperLanding != null) 
        {
            PlaceTileLogic(UPPER_OFFSET, upperLanding, 0);
            Debug.Log($"[MapManager] Placed anchor: {upperLanding.roomName} at {UPPER_OFFSET}");
        }

        // 2. Setup BASEMENT
        if (basementLanding != null) 
        {
            PlaceTileLogic(BASEMENT_OFFSET, basementLanding, 0);
            Debug.Log($"[MapManager] Placed anchor: {basementLanding.roomName} at {BASEMENT_OFFSET}");
        }

        // 3. Setup GROUND FLOOR - 3 anchor rooms
        if (grandStaircase != null) 
        {
            PlaceTileLogic(new Vector2Int(-1, 0), grandStaircase, 0);
            Debug.Log($"[MapManager] Placed anchor: {grandStaircase.roomName} at (-1, 0)");
        }

        if (foyer != null) 
        {
            PlaceTileLogic(new Vector2Int(0, 0), foyer, 0);
            Debug.Log($"[MapManager] Placed anchor: {foyer.roomName} at (0, 0)");
        }

        if (entranceHall != null) 
        {
            PlaceTileLogic(new Vector2Int(1, 0), entranceHall, 0);
            Debug.Log($"[MapManager] Placed anchor: {entranceHall.roomName} at (1, 0)");
        }

        // Remove anchor rooms from deck to avoid duplicates
        if (tileDeck.Contains(upperLanding)) tileDeck.Remove(upperLanding);
        if (tileDeck.Contains(basementLanding)) tileDeck.Remove(basementLanding);
        if (tileDeck.Contains(foyer)) tileDeck.Remove(foyer);
        if (tileDeck.Contains(grandStaircase)) tileDeck.Remove(grandStaircase);
        if (tileDeck.Contains(entranceHall)) tileDeck.Remove(entranceHall);
    }

    private void GenerateRandomMap()
    {
        remainingTiles = new List<RoomTileSO>(tileDeck);

        int iterationCount = 0;
        int retryCount = 0;

        while (remainingTiles.Count > 0 && availableSpots.Count > 0 && iterationCount < maxIterations)
        {
            iterationCount++;

            if (availableSpots.Count == 0)
            {
                Debug.LogWarning("[MapManager] No available spots left but tiles remain!");
                break;
            }

            int randSpotIndex = Random.Range(0, availableSpots.Count);
            Vector2Int targetPos = availableSpots[randSpotIndex];

            FloorLayer currentLayer = DetermineFloorLayer(targetPos);
            (RoomTileSO selectedTile, int rotation) = FindMatchingTileWithRotation(targetPos, currentLayer);

            if (selectedTile != null)
            {
                PlaceTileLogic(targetPos, selectedTile, rotation);
                remainingTiles.Remove(selectedTile);
                retryCount = 0;
            }
            else
            {
                retryCount++;

                if (retryCount >= retryAttempts)
                {
                    availableSpots.RemoveAt(randSpotIndex);
                    retryCount = 0;
                }
            }
        }

        LogGenerationStats(iterationCount);
    }

    // --- API CHO PLAYER ---

    public bool HasRoom(Vector2Int coord)
    {
        return mapGrid.ContainsKey(coord);
    }

    public Vector3 GridToWorld(Vector2Int coord)
    {
        return new Vector3(coord.x * tileSize, coord.y * tileSize, -1f);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        return new Vector2Int(x, y);
    }

    public RoomTileSO GetRoomTile(Vector2Int coord)
    {
        if (mapGrid.ContainsKey(coord))
            return mapGrid[coord].tile;
        return null;
    }

    public int GetTotalRoomsPlaced()
    {
        return mapGrid.Count;
    }

    public int GetRemainingTiles()
    {
        return remainingTiles.Count;
    }

    public Dictionary<Vector2Int, RoomTilePlacement> GetMapGrid()
    {
        return mapGrid;
    }

    // --- LOGIC CỐT LÕI ---

    private FloorLayer DetermineFloorLayer(Vector2Int pos)
    {
        if (pos.y >= UPPER_OFFSET.y - 20) return FloorLayer.Upper;
        if (pos.y <= BASEMENT_OFFSET.y + 20) return FloorLayer.Basement;
        return FloorLayer.Ground;
    }

    private (RoomTileSO, int) FindMatchingTileWithRotation(Vector2Int pos, FloorLayer layer)
    {
        List<RoomTileSO> shuffledDeck = remainingTiles.OrderBy(x => Random.value).ToList();

        foreach (RoomTileSO tile in shuffledDeck)
        {
            if (!tile.CanPlaceOnFloor(layer)) continue;

            for (int rotation = 0; rotation < 4; rotation++)
            {
                DoorDirection rotatedDoors = tile.RotateDoors(rotation);

                if (IsPlacementValid(pos, rotatedDoors))
                {
                    return (tile, rotation);
                }
            }
        }

        return (null, -1);
    }

    private void PlaceTileLogic(Vector2Int pos, RoomTileSO tile, int rotation)
    {
        if (mapGrid.ContainsKey(pos)) return;

        RoomTilePlacement placement = new RoomTilePlacement(tile, rotation);
        mapGrid.Add(pos, placement);

        SpawnRoomVisual(pos, placement);

        if (availableSpots.Contains(pos)) availableSpots.Remove(pos);
        UpdateAvailableSpots(pos, placement);
    }

    private void UpdateAvailableSpots(Vector2Int pos, RoomTilePlacement placement)
    {
        DoorDirection rotatedDoors = placement.GetRotatedDoors();

        CheckAndAddSpot(pos + Vector2Int.up, DoorDirection.North, rotatedDoors);
        CheckAndAddSpot(pos + Vector2Int.right, DoorDirection.East, rotatedDoors);
        CheckAndAddSpot(pos + Vector2Int.down, DoorDirection.South, rotatedDoors);
        CheckAndAddSpot(pos + Vector2Int.left, DoorDirection.West, rotatedDoors);
    }

    private void CheckAndAddSpot(Vector2Int neighborPos, DoorDirection myDoorDir, DoorDirection myRotatedDoors)
    {
        if ((myRotatedDoors & myDoorDir) == 0) return;
        if (mapGrid.ContainsKey(neighborPos)) return;
        if (!availableSpots.Contains(neighborPos)) availableSpots.Add(neighborPos);
    }

    private bool IsPlacementValid(Vector2Int pos, DoorDirection rotatedDoors)
    {
        if (!CheckConnection(pos + Vector2Int.up, DoorDirection.North, rotatedDoors)) return false;
        if (!CheckConnection(pos + Vector2Int.right, DoorDirection.East, rotatedDoors)) return false;
        if (!CheckConnection(pos + Vector2Int.down, DoorDirection.South, rotatedDoors)) return false;
        if (!CheckConnection(pos + Vector2Int.left, DoorDirection.West, rotatedDoors)) return false;
        return true;
    }

    private bool CheckConnection(Vector2Int neighborPos, DoorDirection myDir, DoorDirection myRotatedDoors)
    {
        if (!mapGrid.ContainsKey(neighborPos)) return true;

        RoomTilePlacement neighborPlacement = mapGrid[neighborPos];
        DoorDirection neighborRotatedDoors = neighborPlacement.GetRotatedDoors();
        DoorDirection neighborOppositeDir = GetOppositeDirection(myDir);

        bool iHaveDoor = (myRotatedDoors & myDir) != 0;
        bool neighborHasDoor = (neighborRotatedDoors & neighborOppositeDir) != 0;

        return iHaveDoor == neighborHasDoor;
    }

    private DoorDirection GetOppositeDirection(DoorDirection dir)
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

    private void SpawnRoomVisual(Vector2Int coord, RoomTilePlacement placement)
    {
        RoomTileSO data = placement.tile;
        int rotation = placement.rotationSteps;

        GameObject newRoomObj = new GameObject(data.roomName);
        newRoomObj.transform.position = new Vector3(coord.x * tileSize, coord.y * tileSize, 0);
        newRoomObj.transform.rotation = Quaternion.Euler(0, 0, rotation * 90f);

        SpriteRenderer sr = newRoomObj.AddComponent<SpriteRenderer>();
        if (data.roomSprite != null) sr.sprite = data.roomSprite;

        // Set sorting order based on tile position to prevent sprite overlap issues
        // Formula: y-position (inverted, higher y = higher sort order) ensures correct layering
        int sortingOrder = -coord.y;
        sr.sortingOrder = sortingOrder;

        newRoomObj.AddComponent<BoxCollider2D>();

        // Debug Text
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(newRoomObj.transform);
        textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = $"{data.roomName}\n(R:{rotation})";
        tm.characterSize = 0.15f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = Color.black;

        // Add door visualizer
        if (showDoorVisuals)
        {
            RoomTileVisualizer visualizer = newRoomObj.AddComponent<RoomTileVisualizer>();
            visualizer.Initialize(placement, sortingOrder);
        }

        // Room GameObject tracking for gizmo rendering
        roomGameObjects[coord] = newRoomObj;

        newRoomObj.transform.SetParent(this.transform);
        newRoomObj.name = $"{data.roomName} ({coord.x},{coord.y}) [R{rotation}]";
    }

    // --- GIZMO & DEBUG VISUALIZATION ---

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        if (!isMapGenerated) return;

        // Draw grid
        if (showGridLayout)
        {
            DrawGridGizmo();
        }

        // Draw door connections
        DrawDoorConnectionGizmo();
    }

    private void DrawGridGizmo()
    {
        Gizmos.color = gridColor;

        // Draw grid lines for all placed rooms
        foreach (var kvp in mapGrid)
        {
            Vector2Int coord = kvp.Key;
            Vector3 pos = GridToWorld(coord);

            // Draw grid cell
            float halfSize = tileSize * 0.5f;
            Vector3[] corners = new Vector3[]
            {
                pos + new Vector3(-halfSize, -halfSize, 0),
                pos + new Vector3(halfSize, -halfSize, 0),
                pos + new Vector3(halfSize, halfSize, 0),
                pos + new Vector3(-halfSize, halfSize, 0)
            };

            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
        }
    }

    private void DrawDoorConnectionGizmo()
    {
        foreach (var kvp in mapGrid)
        {
            Vector2Int coord = kvp.Key;
            RoomTilePlacement placement = kvp.Value;
            DoorDirection doors = placement.GetRotatedDoors();

            Vector3 roomPos = GridToWorld(coord);

            // Draw lines for each door direction
            DrawDoorDirectionLine(coord, roomPos, DoorDirection.North, Vector3.up);
            DrawDoorDirectionLine(coord, roomPos, DoorDirection.South, Vector3.down);
            DrawDoorDirectionLine(coord, roomPos, DoorDirection.East, Vector3.right);
            DrawDoorDirectionLine(coord, roomPos, DoorDirection.West, Vector3.left);
        }
    }

    private void DrawDoorDirectionLine(Vector2Int coord, Vector3 roomPos, DoorDirection dir, Vector3 direction)
    {
        RoomTilePlacement placement = mapGrid[coord];
        DoorDirection doors = placement.GetRotatedDoors();

        if ((doors & dir) == 0) return; // No door in this direction

        Vector2Int neighborCoord = coord + DirectionToVector2Int(dir);

        // Check if neighbor exists
        if (!mapGrid.ContainsKey(neighborCoord))
        {
            // Draw red line to empty space
            Gizmos.color = new Color(1, 0, 0, 0.5f);
        }
        else
        {
            // Check door matching
            RoomTilePlacement neighborPlacement = mapGrid[neighborCoord];
            DoorDirection neighborDoors = neighborPlacement.GetRotatedDoors();
            DoorDirection oppositeDir = GetOppositeDirection(dir);

            bool neighborHasDoor = (neighborDoors & oppositeDir) != 0;
            bool thisHasDoor = (doors & dir) != 0;

            if (thisHasDoor && neighborHasDoor)
            {
                Gizmos.color = Color.green;
            }
            else if (!thisHasDoor && !neighborHasDoor)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            else
            {
                Gizmos.color = Color.red;
            }
        }

        // Draw line from room center towards door
        Vector3 endPos = roomPos + direction * (tileSize * 0.4f);
        Gizmos.DrawLine(roomPos, endPos);
    }

    private Vector2Int DirectionToVector2Int(DoorDirection dir)
    {
        switch (dir)
        {
            case DoorDirection.North: return Vector2Int.up;
            case DoorDirection.South: return Vector2Int.down;
            case DoorDirection.East: return Vector2Int.right;
            case DoorDirection.West: return Vector2Int.left;
            default: return Vector2Int.zero;
        }
    }

    // --- LOGGING ---

    private void LogGenerationStats(int iterationCount)
    {
        Debug.Log($"[MapManager] Total Rooms Placed: {mapGrid.Count}");
        Debug.Log($"[MapManager] Remaining Tiles: {remainingTiles.Count}");
        Debug.Log($"[MapManager] Iterations Used: {iterationCount}/{maxIterations}");
    }

    private void LogMapSummary()
    {
        if (remainingTiles.Count > 0)
        {
            Debug.LogWarning($"[MapManager] ⚠️ {remainingTiles.Count} tiles could NOT be placed:");
            foreach (var tile in remainingTiles)
            {
                Debug.LogWarning($"   - {tile.roomName}");
            }
        }
        else
        {
            Debug.Log("[MapManager] ✓ ALL TILES SUCCESSFULLY PLACED!");
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (!isMapGenerated)
        {
            GUI.Label(new Rect(10, 10, 400, 80),
                "<color=yellow>MAP GENERATING...</color>\nRooms: " + mapGrid.Count + 
                "\nUnused: " + remainingTiles.Count + 
                "\nSpots: " + availableSpots.Count);
        }
        else
        {
            GUI.Label(new Rect(10, 10, 400, 80),
                "<color=green>MAP GENERATED</color>\nRooms: " + mapGrid.Count + 
                "\nUnused: " + remainingTiles.Count);
        }
    }
#endif
}