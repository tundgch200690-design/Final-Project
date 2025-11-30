# Map Generation Visualization Guide

## Overview
This system provides complete visual feedback for map generation with tile placement, door matching, and grid visualization.

## Components Created

### 1. **RoomTileVisualizer.cs**
Renders door indicators on each room tile showing which directions have openings.

**Features:**
- Green squares = doors (open passages)
- Red squares = walls (sealed)
- Visual labels showing ? for doors, ? for walls
- Door size relative to tile size
- Dynamically updates when tile rotates

**Usage:**
Automatically added to room GameObjects when `showDoorVisuals = true` in MapManager.

---

### 2. **DoorConnectionVisualizer.cs**
Validates and visualizes door connections between adjacent tiles.

**Features:**
- Checks if two rooms have matching doors
- Validates door-to-door or wall-to-wall connections
- Static validation methods for checking connections

**Usage:**
```csharp
bool isValid = DoorConnectionVisualizer.CheckDoorConnection(
    tile1, pos1,
    tile2, pos2,
    out bool hasBothDoors,
    out bool hasNoDoors);
```

---

### 3. **TilePlacementHelper.cs**
Utility class for tile positioning and validation calculations.

**Key Methods:**
- `GridToWorldPosition()` - Convert grid coords to world position
- `WorldToGridPosition()` - Convert world position to grid coords
- `GetDoorPositionInWorld()` - Get world position of a door on a tile
- `ValidateDoorConnection()` - Validate two adjacent tiles
- `GetAdjacentPositions()` - Get all 4 neighboring grid positions

**Usage:**
```csharp
Vector3 worldPos = TilePlacementHelper.GridToWorldPosition(new Vector2Int(5, 3), 2.0f);
bool isValid = TilePlacementHelper.ValidateDoorConnection(tile1, pos1, tile2, pos2, out msg);
```

---

### 4. **MapDebugVisualizer.cs**
On-screen debug window showing map generation status and tile information.

**Features:**
- Real-time generation status
- List of placed tiles with positions and rotations
- Door matching information for each tile
- Validation reporting
- Toggle-able displays

**Usage:**
Add to any GameObject with MapManager present. Access in Editor with `OnGUI()` panel.

```csharp
visualizer.ValidateAllConnections(); // Log all door connections
```

---

## Visualization Settings in MapManager

### Inspector Options:
```
C?u hình Visualization:
?? showDebugGizmos (boolean)     - Enable gizmo rendering
?? showDoorVisuals (boolean)     - Show door indicators on tiles
?? showGridLayout (boolean)      - Show grid cell outlines
?? gridColor (color)             - Grid line color (default: gray)
```

### Gizmo Color Coding:
- **Green lines** - Valid door connections (both rooms have door OR both sealed)
- **Red lines** - Invalid door connections (mismatch)
- **Gray lines** - Adjacent rooms without doors in between
- **Gray grid** - Grid cell boundaries

---

## Setup Instructions

### In Unity Scene:

1. **Add to MapManager GameObject:**
   ```
   Add Component ? MapDebugVisualizer
   ```

2. **Configure MapManager Inspector:**
   ```
   C?u hình Visualization:
   - showDebugGizmos: ? (enabled)
   - showDoorVisuals: ? (enabled)
   - showGridLayout: ? (enabled)
   ```

3. **Run Scene:**
   - See grid lines in Scene view
   - See green/red door indicators on each room
   - See door connection lines in gizmos
   - Debug window shows tile list and validation

---

## Visualization Examples

### Door Indicators (On Tiles)
```
    ?
    ?
? ? ? ? ?
    ?
    ?
```
Each direction shows:
- **?** = Door present (green)
- **?** = Wall present (red)

### Gizmo Grid
```
+???+???+???+
?   ?   ?   ?
+???+???+???+
?   ?   ?   ?
+???+???+???+
```
Gray lines show tile grid boundaries.

### Door Connections
```
Room A ??? Room B
    (green line = both have doors)

Room A ??? Room C
    (red line = mismatch in doors)
```

---

## Debug Methods

### Validate All Connections:
```csharp
// In Editor Console:
MapDebugVisualizer visualizer = FindObjectOfType<MapDebugVisualizer>();
visualizer.ValidateAllConnections();
```

**Output Example:**
```
? Foyer ? North | Connected (both have door)
? Grand Staircase ? East | Connected (both sealed)
? Entrance Hall ? South | Door mismatch!
```

---

## Performance Notes

- **Gizmo rendering**: Only in Editor, no runtime overhead
- **Door visuals**: Uses simple 2D sprites, minimal performance impact
- **Debug window**: Only draws in Editor mode (`#if UNITY_EDITOR`)
- **Validation**: Static methods, can be called anytime without instantiation

---

## Troubleshooting

### Issue: No door visuals on tiles
**Solution**: Check `showDoorVisuals = true` in MapManager inspector

### Issue: Gizmo lines not showing
**Solution**: 
1. Enable gizmos in top-right corner of Scene view
2. Check `showDebugGizmos = true`
3. Press Play (gizmos only draw during generation)

### Issue: Red lines showing door mismatches
**Solution**: Check room tile door configuration in RoomTileSO assets
- Ensure anchor rooms have correct door placement
- Verify tileDeck entries have appropriate door directions

---

## Adding Custom Visualizations

### Create Custom Door Visual:
```csharp
public class CustomDoorVisualizer : RoomTileVisualizer
{
    protected override void CreateDoorVisuals()
    {
        // Custom door rendering code
        base.CreateDoorVisuals();
    }
}
```

### Add Validation Logging:
```csharp
foreach (var kvp in MapManager.Instance.GetMapGrid())
{
    Vector2Int coord = kvp.Key;
    RoomTilePlacement placement = kvp.Value;
    DoorDirection doors = placement.GetRotatedDoors();
    Debug.Log($"Room at {coord}: {string.Join(",", doors)}");
}
```

---

**Now your map generation has complete visual feedback!** ??
