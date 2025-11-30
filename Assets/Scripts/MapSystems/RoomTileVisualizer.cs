using UnityEngine;

public class RoomTileVisualizer : MonoBehaviour
{
    private RoomTilePlacement placement;
    private int baseSortingOrder = 0;

    // Bạn cần tạo 1 Prefab nhỏ hình cái cửa (hoặc mũi tên xanh) và gán vào đây
    // Nếu không có, code sẽ chỉ vẽ Gizmos (ẩn khi chơi thật)
    public GameObject doorIndicatorPrefab;

    public void Initialize(RoomTilePlacement placement, int sortingOrder = 0)
    {
        this.placement = placement;
        this.baseSortingOrder = sortingOrder;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (placement == null) return;

        DoorDirection rotatedDoors = placement.GetRotatedDoors();
        float roomSize = MapManager.Instance.tileSize;
        float offset = roomSize * 0.4f; // Đẩy cửa ra sát mép phòng

        // Tạo visual cho từng cửa
        if ((rotatedDoors & DoorDirection.North) != 0) SpawnDoorVisual(Vector3.up * offset, 0);
        if ((rotatedDoors & DoorDirection.South) != 0) SpawnDoorVisual(Vector3.down * offset, 180);
        if ((rotatedDoors & DoorDirection.East) != 0) SpawnDoorVisual(Vector3.right * offset, -90);
        if ((rotatedDoors & DoorDirection.West) != 0) SpawnDoorVisual(Vector3.left * offset, 90);
    }

    private void SpawnDoorVisual(Vector3 localPos, float angle)
    {
        GameObject door;

        // Nếu có Prefab cửa đẹp thì dùng nó, nếu không thì tạm tạo Primitive
        if (doorIndicatorPrefab != null)
        {
            door = Instantiate(doorIndicatorPrefab, transform);
            door.transform.localPosition = localPos;
            door.transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Vẽ 1 ô vuông nhỏ màu vàng làm cửa (Dùng tạm Primitive nếu chưa có Sprite)
            door = GameObject.CreatePrimitive(PrimitiveType.Quad);
            Destroy(door.GetComponent<Collider>()); // Xóa collider để không cản đường

            door.transform.SetParent(transform);
            door.transform.localPosition = localPos;
            door.transform.localRotation = Quaternion.Euler(0, 0, angle);
            door.transform.localScale = Vector3.one * 0.2f; // Cửa nhỏ thôi

            // Đổi màu cửa
            door.GetComponent<Renderer>().material.color = Color.yellow;
        }

        // Set sorting order để cửa hiển thị trên phòng
        SpriteRenderer doorRenderer = door.GetComponent<SpriteRenderer>();
        if (doorRenderer != null)
        {
            doorRenderer.sortingOrder = baseSortingOrder + 1;
        }

        door.name = $"Door_{angle}";
    }
}