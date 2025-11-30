# Dynamic Room Discovery System - Implementation Guide

## Overview
This system replaces the pre-generated map with a dynamic room discovery system. Rooms are only created as the player discovers them by attempting to move into unexplored areas.

## Key Changes Made

### 1. MapManager.cs Modifications

#### New Method: `InitializeGame()`
```csharp
public void InitializeGame()
```
- **Purpose**: Initialize the game with only the starting room (Foyer)
- **When Called**: Automatically during Start() if `autoGenerateOnStart` is true
- **Behavior**:
  - Places only the Foyer at position (0, 0)
  - Initializes remainingTiles with full tileDeck
  - Removes all anchor rooms from the discovery pool
  - Sets isMapGenerated to true

#### New Method: `TryDiscoverRoom(Vector2Int targetPos)`
```csharp
public bool TryDiscoverRoom(Vector2Int targetPos)
```
- **Purpose**: Attempt to discover a new room at the given position
- **Returns**: 
  - `true` if room was placed or already exists
  - `false` if no compatible room could be found
- **Logic**:
  1. Checks if room already exists at targetPos
  2. Determines correct floor layer for the position
  3. Finds a matching tile from remainingTiles that fits the position's doors
  4. Places the tile with correct rotation
  5. Removes tile from remainingTiles
  6. Logs discovery event

#### Modified Method: `Start()`
- Now calls `InitializeGame()` instead of full `GenerateMap()`
- Initializes remainingTiles for dynamic discovery
- Removes anchor rooms from the discovery pool

#### Legacy Method: `GenerateMap()`
- Still available for full pre-generation if needed
- Sets `autoGenerateOnStart` to false to disable it in new games

### 2. PlayerController.cs Modifications

#### Modified Method: `TryMove(Vector2Int targetPos)`
**New Logic**:
```csharp
// Try to discover room if it doesn't exist yet
if (!MapManager.Instance.HasRoom(targetPos))
{
    bool discoveredRoom = MapManager.Instance.TryDiscoverRoom(targetPos);
    if (!discoveredRoom)
    {
        Debug.Log($"Cannot move to {targetPos} - no compatible room found.");
        return;
    }
}
```

**Behavior**:
1. Checks if target position has a room
2. If no room exists, calls TryDiscoverRoom()
3. Only allows movement if room discovery succeeds
4. Movement blocked if no compatible room can be generated

### 3. GameInitializer.cs Modifications

#### Updated `InitializeGameSequence()`
- Calls `InitializeGame()` instead of waiting for `GenerateMap()`
- Logs now show "Starting rooms placed" instead of "Rooms placed"
- Displays "Tiles available for discovery" instead of "Unused tiles"
- System is now ready faster (only 1 room instead of full map)

## How It Works

### Game Flow

1. **Startup**
   - MapManager.Awake() initializes as Singleton
   - MapManager.Start() calls InitializeGame()
   - Only Foyer is placed at (0, 0)
   - remainingTiles = full tileDeck minus anchor rooms
   - isMapGenerated = true

2. **Player Movement**
   - Player clicks on adjacent tile
   - PlayerController.TryMove(targetPos) is called
   - If targetPos has no room:
     - TryDiscoverRoom(targetPos) is called
     - System finds matching tile from remainingTiles
     - Tile is placed with correct rotation
     - Doors are connected logically
   - Player moves to discovered room

3. **Room Discovery Rules**
   - Only adjacent rooms can be discovered (distance = 1)
   - Doors must match: if you have a door, neighbor must have matching door
   - Floor layer determines which tiles are available
   - Random tile selection from compatible tiles
   - Each tile is used at most once

### Floor Layer Management

```
Upper Floor:  y >= 30 (UPPER_OFFSET.y - 20)
Ground Floor: -20 < y < 30
Basement:     y <= -30 (BASEMENT_OFFSET.y + 20)
```

- Each discovered room's floor layer is determined by its Y coordinate
- Only tiles marked for that floor can be placed
- Prevents Ground rooms from appearing in Upper/Basement areas

## Configuration

### Inspector Settings

1. **autoGenerateOnStart** (Default: false for dynamic, true for old system)
   - `true`: Uses InitializeGame() - recommended for dynamic discovery
   - `false`: Must manually call InitializeGame() or GenerateMap()

2. **tileDeck** (Required)
   - Assign all non-anchor room tiles here
   - Anchor rooms (Foyer, etc.) are automatically excluded
   - Tiles are drawn from this pool as rooms are discovered

3. **Anchor Room Assignments** (Optional if using dynamic)
   - Still used for SetupFixedMap() if GenerateMap() is called
   - Not used in InitializeGame() except for Foyer

## Example Usage

### Dynamic Discovery (Recommended)
```csharp
// In Inspector:
// - autoGenerateOnStart = true
// - foyer = [Foyer Asset]
// - tileDeck = [all room assets]

// Game starts:
// 1. InitializeGame() is called
// 2. Only Foyer at (0,0) is created
// 3. Player moves to adjacent tile
// 4. TryDiscoverRoom() places a random compatible room
```

### Full Pre-Generation (Legacy)
```csharp
// In Inspector:
// - autoGenerateOnStart = true
// - Call MapManager.GenerateMap() OR set autoGenerateOnStart to true in Start()

// Game starts:
// 1. SetupFixedMap() places 5 anchor rooms
// 2. GenerateRandomMap() fills rest of map
// 3. All rooms created at startup
```

## Debugging

### Log Messages

**Startup**:
```
[MapManager] Initialized as Singleton
[MapManager] ===== GAME INITIALIZED =====
[MapManager] Placed starting room: Foyer at (0, 0)
```

**Room Discovery**:
```
[MapManager] Discovered new room: DiningRoom at (1, 0)
[MapManager] Discovered new room: Kitchen at (1, 1)
```

**Failures**:
```
[MapManager] Could not find matching room for position (5, 0)
```

### Debug Visualization

In MapManager Inspector:
- **showDebugGizmos**: Shows grid and door connections
- **showDoorVisuals**: Shows yellow door indicators
- **showGridLayout**: Shows grid cell outlines

## Performance Considerations

### Advantages of Dynamic Discovery
- **Faster Startup**: Only one room loaded initially
- **Lower Memory**: Rooms created on-demand
- **Infinite Map Potential**: No hard limit on map size
- **Better Pacing**: Rooms reveal as player explores

### Disadvantages
- **Runtime Overhead**: Room creation during gameplay
- **Potential Lag Spikes**: Large room assets may cause frame drops
- **Unpredictable Layout**: No guarantee of optimal pathfinding

## Troubleshooting

### Problem: Player can't move anywhere
**Solution**: 
1. Check that Foyer is assigned in Inspector
2. Verify tileDeck has non-anchor tiles
3. Check that floor layer filters aren't too restrictive

### Problem: Same room appears multiple times
**Solution**:
1. Ensure anchor rooms are removed from tileDeck
2. Check TryDiscoverRoom is removing tiles from remainingTiles
3. Verify tileDeck doesn't have duplicates

### Problem: Rooms appear in wrong floor
**Solution**:
1. Check FloorLayer enum flags on room assets
2. Verify DetermineFloorLayer() logic with Y positions
3. Ensure CanPlaceOnFloor() is implemented correctly

## Migration from Old System

If you had autoGenerateOnStart = true before:

1. Keep it as true (InitializeGame will be called)
2. Remove calls to GenerateMap() from startup
3. Ensure PlayerController uses TryDiscoverRoom
4. Test that Foyer appears at startup
5. Test player movement triggers room discovery

## Future Enhancements

Potential improvements to this system:
1. **Room Variants**: Multiple versions of same room type
2. **Special Rooms**: Rooms that appear only in specific conditions
3. **Procedural Doors**: Doors generated based on adjacency
4. **Player Hints**: UI showing possible discovery directions
5. **Difficulty Scaling**: Tile selection based on player progress
