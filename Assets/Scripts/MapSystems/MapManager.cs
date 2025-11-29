using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{   
    public static MapManager Instance;

    [Header("Cài đặt Hệ thống")]
    public float tileSize = 2.0f;
    public int maxIterations = 50;

    [Header("Cấu hình Tile Cố định")]
    public RoomTileSO upperLanding;
    public RoomTileSO basementLanding;

    [Header("Bộ 3 Phòng Ground")]
    public RoomTileSO foyer;          // (0,0)
    public RoomTileSO grandStaircase; // (-1,0)
    public RoomTileSO entranceHall;   // (1,0)

    [Header("Kho Dữ liệu Ngẫu nhiên")]
    public List<RoomTileSO> tileDeck;

    // Dữ liệu lưu trữ
    private Dictionary<Vector2Int, RoomTileSO> mapGrid = new Dictionary<Vector2Int, RoomTileSO>();
    private List<Vector2Int> availableSpots = new List<Vector2Int>();

    // Định nghĩa khoảng cách tầng
    private readonly Vector2Int UPPER_OFFSET = new Vector2Int(0, 50);
    private readonly Vector2Int BASEMENT_OFFSET = new Vector2Int(0, -50);

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetupFixedMap();
        GenerateRandomMap();
    }

    // --- SETUP BẢN ĐỒ ---
    private void SetupFixedMap()
    {
        if (upperLanding != null) PlaceTileLogic(UPPER_OFFSET, upperLanding);
        if (basementLanding != null) PlaceTileLogic(BASEMENT_OFFSET, basementLanding);

        if (grandStaircase != null) PlaceTileLogic(new Vector2Int(-1, 0), grandStaircase);
        if (foyer != null) PlaceTileLogic(new Vector2Int(0, 0), foyer);
        if (entranceHall != null) PlaceTileLogic(new Vector2Int(1, 0), entranceHall);
    }

    private void GenerateRandomMap()
    {
        int safetyCount = 0;
        while (tileDeck.Count > 0 && availableSpots.Count > 0 && safetyCount < maxIterations)
        {
            safetyCount++;
            int randSpotIndex = Random.Range(0, availableSpots.Count);
            Vector2Int targetPos = availableSpots[randSpotIndex];

            FloorLayer currentLayer = DetermineFloorLayer(targetPos);
            RoomTileSO selectedTile = FindMatchingTile(targetPos, currentLayer);

            if (selectedTile != null)
            {
                PlaceTileLogic(targetPos, selectedTile);
                tileDeck.Remove(selectedTile);
            }
            else
            {
                availableSpots.RemoveAt(randSpotIndex);
            }
        }
    }

    // --- API CHO PLAYER (QUAN TRỌNG) ---

    // 1. Kiểm tra có phòng không
    public bool HasRoom(Vector2Int coord)
    {
        return mapGrid.ContainsKey(coord);
    }

    // 2. Grid -> World (Để đặt nhân vật)
    public Vector3 GridToWorld(Vector2Int coord)
    {
        // Z = -1 để nhân vật nổi lên trên sàn nhà
        return new Vector3(coord.x * tileSize, coord.y * tileSize, -1f);
    }

    // 3. World -> Grid (Để xử lý click chuột)
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / tileSize);
        int y = Mathf.RoundToInt(worldPos.y / tileSize);
        return new Vector2Int(x, y);
    }

    // --- LOGIC CỐT LÕI ---

    private FloorLayer DetermineFloorLayer(Vector2Int pos)
    {
        if (pos.y >= UPPER_OFFSET.y - 20) return FloorLayer.Upper;
        if (pos.y <= BASEMENT_OFFSET.y + 20) return FloorLayer.Basement;
        return FloorLayer.Ground;
    }

    private RoomTileSO FindMatchingTile(Vector2Int pos, FloorLayer layer)
    {
        List<RoomTileSO> shuffledDeck = tileDeck.OrderBy(x => Random.value).ToList();
        foreach (RoomTileSO tile in shuffledDeck)
        {
            if (!tile.CanPlaceOnFloor(layer)) continue;
            if (IsPlacementValid(pos, tile)) return tile;
        }
        return null;
    }

    private void PlaceTileLogic(Vector2Int pos, RoomTileSO tile)
    {
        if (mapGrid.ContainsKey(pos)) return;

        mapGrid.Add(pos, tile);
        SpawnRoomVisual(pos, tile);

        if (availableSpots.Contains(pos)) availableSpots.Remove(pos);
        UpdateAvailableSpots(pos, tile);
    }

    private void UpdateAvailableSpots(Vector2Int pos, RoomTileSO tile)
    {
        CheckAndAddSpot(pos + Vector2Int.up, DoorDirection.North, tile);
        CheckAndAddSpot(pos + Vector2Int.right, DoorDirection.East, tile);
        CheckAndAddSpot(pos + Vector2Int.down, DoorDirection.South, tile);
        CheckAndAddSpot(pos + Vector2Int.left, DoorDirection.West, tile);
    }

    private void CheckAndAddSpot(Vector2Int neighborPos, DoorDirection myDoorDir, RoomTileSO myTile)
    {
        if (!myTile.HasDoor(myDoorDir)) return;
        if (mapGrid.ContainsKey(neighborPos)) return;
        if (!availableSpots.Contains(neighborPos)) availableSpots.Add(neighborPos);
    }

    private bool IsPlacementValid(Vector2Int pos, RoomTileSO newTile)
    {
        if (!CheckConnection(pos + Vector2Int.up, DoorDirection.North, newTile)) return false;
        if (!CheckConnection(pos + Vector2Int.right, DoorDirection.East, newTile)) return false;
        if (!CheckConnection(pos + Vector2Int.down, DoorDirection.South, newTile)) return false;
        if (!CheckConnection(pos + Vector2Int.left, DoorDirection.West, newTile)) return false;
        return true;
    }

    private bool CheckConnection(Vector2Int neighborPos, DoorDirection myDir, RoomTileSO newTile)
    {
        if (!mapGrid.ContainsKey(neighborPos)) return true;

        RoomTileSO neighbor = mapGrid[neighborPos];
        DoorDirection neighborOppositeDir = GetOppositeDirection(myDir);

        bool iHaveDoor = newTile.HasDoor(myDir);
        bool neighborHasDoor = neighbor.HasDoor(neighborOppositeDir);

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

    private void SpawnRoomVisual(Vector2Int coord, RoomTileSO data)
    {
        GameObject newRoomObj = new GameObject(data.roomName);
        newRoomObj.transform.position = new Vector3(coord.x * tileSize, coord.y * tileSize, 0);

        SpriteRenderer sr = newRoomObj.AddComponent<SpriteRenderer>();
        if (data.roomSprite != null) sr.sprite = data.roomSprite;

        // --- QUAN TRỌNG: Thêm Collider để chuột click được ---
        newRoomObj.AddComponent<BoxCollider2D>();
        // ----------------------------------------------------

        // Debug Text
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(newRoomObj.transform);
        textObj.transform.localPosition = new Vector3(0, 0, -0.1f);
        TextMesh tm = textObj.AddComponent<TextMesh>();
        tm.text = data.roomName;
        tm.characterSize = 0.15f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.color = Color.black;

        newRoomObj.transform.SetParent(this.transform);
        newRoomObj.name = $"{data.roomName} ({coord.x},{coord.y})";
    }
}