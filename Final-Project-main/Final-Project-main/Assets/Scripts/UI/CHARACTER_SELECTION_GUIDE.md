# Character Selection Menu - Implementation Guide

## Overview
This system allows players to select characters before starting the game. The selected characters will then be displayed in the PlayerHUD as turns progress.

## Components Created

### 1. CharacterSelectionManager
- **Location:** `Assets/Scripts/UI/CharacterSelectionManager.cs`
- **Purpose:** Manages player character selection and validation
- **Key Methods:**
  - `SelectCharacter(Player character)` - Add/remove a character from selection
  - `StartGame()` - Validates selection and populates TurnManager
  - `GetSelectedPlayers()` - Returns list of selected players

### 2. CharacterSelectionUI
- **Location:** `Assets/Scripts/UI/CharacterSelectionUI.cs`
- **Purpose:** Manages the character selection UI panel
- **Features:**
  - Displays available characters
  - Shows selection count
  - Enables/disables start button based on valid player count

### 3. CharacterSelectionButton
- **Location:** `Assets/Scripts/UI/CharacterSelectionButton.cs`
- **Purpose:** Individual character button in the selection menu
- **Features:**
  - Shows character avatar, name, and stats
  - Visual highlight when selected
  - Toggle selection on click

### 4. GameFlowManager
- **Location:** `Assets/Scripts/UI/GameFlowManager.cs`
- **Purpose:** Manages scene transitions
- **Methods:**
  - `LoadCharacterSelection()` - Load selection menu
  - `LoadGamePlay()` - Load game with selected players
  - `ReturnToCharacterSelection()` - Go back to menu and clear selection

### 5. Modified TurnManager
- **File:** `Assets/Scripts/Mechanics/TurnManager.cs`
- **Changes:**
  - Added `InitializeGameWithPlayers(List<Player> selectedPlayers)` method
  - Modified initialization to handle manual player assignment

## Setup Instructions

### Step 1: Create Character Selection Scene

1. Create a new scene named "CharacterSelection"
2. Create a Canvas in the scene
3. Add the following UI structure to the Canvas:

```
Canvas
??? Background (Image)
??? Title (Text)
??? CharacterGrid (GridLayoutGroup)
?   ??? Character Button Prefab (will be instantiated)
??? SelectionInfo
?   ??? SelectedCountText (TextMeshPro)
??? StartButton (Button)
```

### Step 2: Create Character Selection Button Prefab

1. Create a prefab with the following hierarchy:
```
CharacterSelectionButton
??? Avatar (Image)
??? Name (TextMeshPro)
??? Stats (TextMeshPro)
??? Highlight (Image - for selection visual)
```

2. Add these components:
   - Button component (for the root)
   - CharacterSelectionButton script
   - LayoutElement (for sizing)

### Step 3: Create CharacterSelectionManager in Scene

1. Create an empty GameObject named "CharacterSelectionManager"
2. Add the `CharacterSelectionManager` script
3. In the Inspector:
   - Set Min Players: 3
   - Set Max Players: 6
   - Assign your available Player ScriptableObjects to the "Available Characters" list

### Step 4: Setup CharacterSelectionUI

1. Create an empty GameObject named "SelectionUIController"
2. Add the `CharacterSelectionUI` script
3. Assign references:
   - Character Button Container: The GridLayoutGroup transform
   - Start Game Button: The button in your UI
   - Selected Count Text: The TextMeshPro displaying count
   - Character Button Prefab: Your prefab from Step 2

### Step 5: Create GameFlowManager (Optional but Recommended)

1. Create an empty GameObject in your project (can be in any scene)
2. Add the `GameFlowManager` script
3. Make it a prefab for easy reuse

## Usage Flow

### Character Selection Scene
1. Player sees available characters
2. Clicks on 3-6 characters to select them
3. Selection count updates in real-time
4. Start button becomes enabled when 3-6 characters selected
5. Click Start Game to load the main game scene

### Game Scene
1. TurnManager initializes with selected players
2. PlayerHUD displays the current player's information
3. Turns progress through selected players as usual

## Integration with Existing System

### PlayerHUD
No changes needed! The existing PlayerHUD code will automatically display whichever player's turn it is:
```csharp
OnPlayerChanged(Player newPlayer) // Called when turn changes
UpdateHUD() // Updates avatar and stats based on currentPlayer
```

### TurnManager
The modified version maintains backward compatibility:
- Old flow: Assign players in Inspector ? Game starts automatically
- New flow: Use CharacterSelectionManager ? Manual StartGame call

## Example: Creating Your Selection Menu UI

### Canvas Setup Example
```
Canvas
??? BG (Image, stretch to fill)
??? Title (Text: "Select Your Characters")
??? Instructions (Text: "Choose 3-6 characters to play")
??? ScrollView (Optional, if many characters)
?   ??? Content
?       ??? CharacterGrid (GridLayoutGroup, 2-3 columns)
??? SelectionPanel
?   ??? SelectionCountText (displays: "Selected: 3/6")
??? ButtonsPanel
    ??? StartGameButton (Button)
    ??? QuitButton (Button)
```

### CharacterSelectionButton Prefab Example
- Size: 200x300 px
- Contains:
  - Avatar Image (top, 160x160)
  - Name Text (below avatar)
  - Stats Text (below name, smaller font)
  - Highlight Image overlay (semi-transparent, hidden by default)
  - Button component covers entire area

## Scene Setup Summary

### CharacterSelection Scene
- Managers:
  - CharacterSelectionManager (singleton)
  - GameFlowManager (if in this scene)
- UI:
  - CharacterSelectionUI
  - Character buttons (instantiated dynamically)

### GameScene
- Should already have:
  - TurnManager
  - PlayerHUD
  - Other game logic

## Testing Checklist

- [ ] Character Selection scene loads correctly
- [ ] Available characters display as buttons
- [ ] Can select/deselect characters (3-6)
- [ ] Selection count updates in real-time
- [ ] Start button disabled with <3 or >6 players
- [ ] Start button enabled with 3-6 selected
- [ ] Clicking Start Game loads game scene
- [ ] TurnManager.players list populated correctly
- [ ] PlayerHUD shows correct current player
- [ ] Turn cycling works with selected players only
- [ ] Avatar and stats display correctly in HUD

## Troubleshooting

### Characters not appearing in selection menu
- Check that CharacterSelectionManager has Player assets assigned
- Verify CharacterSelectionButton prefab is assigned to CharacterSelectionUI

### Start button not enabling
- Ensure you have selected 3-6 characters
- Check min/max player settings in CharacterSelectionManager

### HUD not updating
- Verify TurnManager.OnTurnChanged event is firing
- Check that TurnManager has players in its list
- Ensure PlayerHUD is subscribed to the event

### Scene not loading
- Check scene names in CharacterSelectionUI and GameFlowManager
- Verify scenes are added to Build Settings

## Customization Options

### Min/Max Players
Edit in CharacterSelectionManager Inspector:
```
minPlayers = 3
maxPlayers = 6
```

### Visual Styling
Customize in your prefabs:
- Avatar image size
- Text colors and fonts
- Selection highlight appearance
- Button animations

### Button Layout
Modify in CharacterSelectionUI:
- Grid columns
- Button spacing
- Scroll view options

## Notes

- The system is designed to work alongside your existing TurnManager
- Character selection data persists through scene load via DontDestroyOnLoad
- You can return to character selection and start a new game anytime
- Each player object should have avatar and stat values defined
