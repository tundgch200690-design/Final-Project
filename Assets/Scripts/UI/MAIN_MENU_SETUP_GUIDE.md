# Main Menu System - Complete Setup Guide

## Overview
This comprehensive main menu system provides:
- Play (Single Player / Multiplayer selection)
- Settings (Volume, Resolution, Fullscreen)
- Credits (Scrollable with auto-scroll)
- Quit Game

## Components Created

### 1. MainMenuManager
- **Location:** `Assets/Scripts/UI/MainMenuManager.cs`
- **Purpose:** Handles menu navigation and game mode selection
- **Key Methods:**
  - `PlaySinglePlayer()` - Start single player mode
  - `PlayMultiplayer()` - Start multiplayer mode
  - `ReturnToMainMenu()` - Go back to main menu
  - `QuitGame()` - Exit the game

### 2. MainMenuUI
- **Location:** `Assets/Scripts/UI/MainMenuUI.cs`
- **Purpose:** Manages all menu UI panels and transitions
- **Features:**
  - Main menu display
  - Game mode selection (Single Player / Multiplayer)
  - Settings panel integration
  - Credits panel integration

### 3. SettingsMenuUI
- **Location:** `Assets/Scripts/UI/SettingsMenuUI.cs`
- **Purpose:** Handles game settings
- **Features:**
  - Master Volume slider
  - Music Volume slider
  - SFX Volume slider
  - Fullscreen toggle
  - Resolution dropdown
  - Settings persistence using PlayerPrefs

### 4. CreditsUI
- **Location:** `Assets/Scripts/UI/CreditsUI.cs`
- **Purpose:** Displays game credits
- **Features:**
  - Scrollable credits display
  - Manual scroll buttons (up/down)
  - Auto-scroll functionality
  - Customizable credits content

### 5. GameModeManager
- **Location:** `Assets/Scripts/UI/GameModeManager.cs`
- **Purpose:** Manages different game modes
- **Features:**
  - Single Player mode configuration
  - Multiplayer mode configuration
  - AI player settings

### 6. MainMenuAudioManager
- **Location:** `Assets/Scripts/UI/MainMenuAudioManager.cs`
- **Purpose:** Handles menu audio and music
- **Features:**
  - Background menu music
  - Button click sounds
  - Volume control integration
  - Audio persistence

## UI Hierarchy Setup

Create this structure in your MainMenu scene:

```
Canvas
??? MainMenuPanel (Panel)
?   ??? Title (TextMeshPro - "BETRAYAL AT HOUSE")
?   ??? Logo/Image (optional)
?   ??? ButtonsContainer (VerticalLayoutGroup)
?   ?   ??? PlayButton (Button + TextMeshPro)
?   ?   ??? SettingsButton (Button + TextMeshPro)
?   ?   ??? CreditsButton (Button + TextMeshPro)
?   ?   ??? QuitButton (Button + TextMeshPro)
?   ??? Version Text (optional)
?
??? GameModePanel (Panel - HIDDEN initially)
?   ??? Title (TextMeshPro - "Select Game Mode")
?   ??? ButtonsContainer (HorizontalLayoutGroup)
?   ?   ??? SinglePlayerButton (Button + Image + TextMeshPro)
?   ?   ??? MultiplayerButton (Button + Image + TextMeshPro)
?   ??? BackButton (Button + TextMeshPro)
?
??? SettingsPanel (Panel - HIDDEN initially)
?   ??? Title (TextMeshPro - "Settings")
?   ??? ScrollView (for settings if needed)
?   ??? VolumeSection
?   ?   ??? MasterVolumeLabel (Text)
?   ?   ??? MasterVolumeSlider (Slider)
?   ?   ??? MasterVolumeText (TextMeshPro - shows %)
?   ?   ??? MusicVolumeLabel (Text)
?   ?   ??? MusicVolumeSlider (Slider)
?   ?   ??? MusicVolumeText (TextMeshPro)
?   ?   ??? SFXVolumeLabel (Text)
?   ?   ??? SFXVolumeSlider (Slider)
?   ?   ??? SFXVolumeText (TextMeshPro)
?   ??? GraphicsSection
?   ?   ??? FullscreenLabel (Text)
?   ?   ??? FullscreenToggle (Toggle)
?   ?   ??? ResolutionLabel (Text)
?   ?   ??? ResolutionDropdown (Dropdown)
?   ??? ButtonsContainer
?   ?   ??? ApplyButton (Button)
?   ?   ??? BackButton (Button)
?   ??? SettingsMenuUI script here
?
??? CreditsPanel (Panel - HIDDEN initially)
?   ??? Title (TextMeshPro - "Credits")
?   ??? ScrollView
?   ?   ??? Content
?   ?       ??? CreditsText (TextMeshPro)
?   ??? ScrollButtons
?   ?   ??? ScrollUpButton (Button)
?   ?   ??? ScrollDownButton (Button)
?   ??? BackButton (Button)
?   ??? CreditsUI script here
?
??? EventSystem
```

## Step-by-Step Setup

### Step 1: Create MainMenu Scene
1. Create a new scene named "MainMenu"
2. Add a Canvas (set to Screen Space - Overlay)
3. Add an EventSystem (auto-created with Canvas)

### Step 2: Create Main Menu Manager GameObject
1. Create an empty GameObject named "MainMenuManager"
2. Add `MainMenuManager` script
3. Set scene names in Inspector:
   - Main Menu Scene Name: "MainMenu"
   - Single Player Character Selection Scene: "CharacterSelection"
   - Multiplayer Character Selection Scene: "CharacterSelection"
   - Game Play Scene Name: "GameScene"

### Step 3: Create Menu UI Container
1. Create a new empty GameObject named "MenuUIController"
2. Add `MainMenuUI` script
3. Assign all required references in Inspector

### Step 4: Build Main Menu Panel
Create the following in your Canvas:

**MainMenuPanel:**
```
Panels > MainMenuPanel (Image background)
??? Title: "BETRAYAL AT HOUSE" (TextMeshPro, size 60-80)
??? ButtonsContainer (VerticalLayoutGroup)
?   ??? PlayButton (Button)
?   ?   ??? Text: "PLAY" (TextMeshPro)
?   ??? SettingsButton
?   ?   ??? Text: "SETTINGS"
?   ??? CreditsButton
?   ?   ??? Text: "CREDITS"
?   ??? QuitButton
?       ??? Text: "QUIT"
```

**Recommended Button Sizes:**
- Button: 300x80 pixels
- Text size: 40-50
- Spacing between buttons: 20 pixels

### Step 5: Build Game Mode Selection Panel
**GameModePanel:**
```
Panels > GameModePanel (Image background)
??? Title: "SELECT GAME MODE"
??? ButtonsContainer (HorizontalLayoutGroup)
?   ??? SinglePlayerButton
?   ?   ??? Image: [optional game mode icon]
?   ?   ??? Text: "SINGLE PLAYER"
?   ??? MultiplayerButton
?       ??? Image: [optional game mode icon]
?       ??? Text: "MULTIPLAYER"
??? BackButton
    ??? Text: "BACK"
```

### Step 6: Build Settings Panel
**SettingsPanel:**
```
SettingsPanel (Panel)
??? Title: "SETTINGS"
??? VolumeSection
?   ??? Master Volume Row
?   ?   ??? Label: "Master Volume"
?   ?   ??? MasterVolumeSlider (0-1 range)
?   ?   ??? MasterVolumeText: "Master: 100%"
?   ??? Music Volume Row
?   ?   ??? Label: "Music Volume"
?   ?   ??? MusicVolumeSlider
?   ?   ??? MusicVolumeText: "Music: 80%"
?   ??? SFX Volume Row
?       ??? Label: "SFX Volume"
?       ??? SFXVolumeSlider
?       ??? SFXVolumeText: "SFX: 80%"
??? GraphicsSection
?   ??? Fullscreen Row
?   ?   ??? Label: "Fullscreen"
?   ?   ??? FullscreenToggle
?   ??? Resolution Row
?       ??? Label: "Resolution"
?       ??? ResolutionDropdown
??? ButtonsContainer
?   ??? ApplyButton (Button) - Text: "APPLY"
?   ??? BackButton (Button) - Text: "BACK"
??? Add SettingsMenuUI script
```

**Slider Setup:**
- Min Value: 0
- Max Value: 1
- Whole Numbers: False
- Default Value: 0.8 (80%)

### Step 7: Build Credits Panel
**CreditsPanel:**
```
CreditsPanel (Panel)
??? Title: "CREDITS"
??? ScrollView (Scroll Rect)
?   ??? Content (VerticalLayoutGroup)
?       ??? CreditsText (TextMeshPro)
?           - Set Text to your credits content
??? ScrollButtons
?   ??? ScrollUpButton (Button) - Text: "? UP"
?   ??? ScrollDownButton (Button) - Text: "DOWN ?"
??? BackButton (Button) - Text: "BACK"
??? Add CreditsUI script
```

### Step 8: Create Audio Setup
1. Create an empty GameObject named "MenuAudioManager"
2. Add AudioSource components (x2):
   - musicAudioSource (set to PlayOnAwake: false)
   - sfxAudioSource (set to PlayOnAwake: false)
3. Add `MainMenuAudioManager` script
4. Assign audio clips:
   - Menu Music
   - Button Click Sound
   - Select Sound
   - Back Sound

### Step 9: Setup Managers in Scene
1. Create an empty GameObject named "GameManagers"
2. Add `GameModeManager` script
3. Configure settings:
   - Single Player AI Count: 2
   - Min Multiplayer Players: 3
   - Max Multiplayer Players: 6

### Step 10: Add Scripts to UI
1. MainMenuUI script ? Add to MenuUIController GameObject
2. SettingsMenuUI script ? Add to SettingsPanel
3. CreditsUI script ? Add to CreditsPanel

## Inspector Assignments for MainMenuUI

```
Main Menu UI
??? Play Button: [Drag PlayButton here]
??? Settings Button: [Drag SettingsButton here]
??? Credits Button: [Drag CreditsButton here]
??? Quit Button: [Drag QuitButton here]
??? Main Menu Panel: [Drag MainMenuPanel here]
??? Game Mode Panel Prefab: [Drag GameModePanel here]
??? Settings Panel: [Drag SettingsPanel here]
??? Credits Panel: [Drag CreditsPanel here]
??? Single Player Button: [Drag SinglePlayerButton here]
??? Multiplayer Button: [Drag MultiplayerButton here]
??? Back From Game Mode Button: [Drag BackButton in GameModePanel here]
```

## Customization

### Custom Credits
Edit in CreditsUI component:
```
Credits Content: 
  Change the default text to your team information
```

Or call at runtime:
```csharp
CreditsUI credits = GetComponent<CreditsUI>();
credits.SetCreditsText("Your custom credits text here");
```

### Custom Audio
Assign your audio clips in MainMenuAudioManager:
- Menu Music: Background music that loops
- Button Click Sound: Play on any button click
- Select Sound: Play when selecting a game mode
- Back Sound: Play when going back

### Visual Styling
- Button colors and animations in Button component
- Panel backgrounds using Image components
- Text colors and fonts using TextMeshPro
- Layout spacing using LayoutGroup settings

## Keyboard Navigation (Optional Enhancement)

Add keyboard input handling:
```csharp
// In MainMenuUI.cs, in Update()
if (Input.GetKeyDown(KeyCode.Escape))
{
    // Go back or show main menu
}
```

## Mobile Support

For mobile/touch support:
- Ensure buttons are large enough (80x300 minimum)
- Add mobile-friendly slider handles
- Test with touch input
- Consider landscape/portrait orientation

## Scene Management Checklist

- [ ] "MainMenu" scene created and saved
- [ ] "CharacterSelection" scene exists
- [ ] "GameScene" scene exists
- [ ] All scenes added to Build Settings (File > Build Settings)
- [ ] Scene names match those in MainMenuManager Inspector

## Testing Checklist

- [ ] Main menu displays correctly
- [ ] Play button shows game mode selection
- [ ] Single Player button loads character selection
- [ ] Multiplayer button loads character selection
- [ ] Settings button opens settings panel
- [ ] All sliders work (0-1 range)
- [ ] Fullscreen toggle works
- [ ] Resolution dropdown shows available resolutions
- [ ] Apply button saves settings to PlayerPrefs
- [ ] Credits button shows scrollable credits
- [ ] Scroll up/down buttons work in credits
- [ ] Auto-scroll works (if enabled)
- [ ] Back buttons return to main menu
- [ ] Quit button exits game
- [ ] Menu music plays on load
- [ ] Button sounds play on interaction

## Troubleshooting

### Buttons not responding
- Ensure Canvas has EventSystem
- Check button Interactable is enabled
- Verify MainMenuUI has references assigned

### Settings not saving
- Check PlayerPrefs keys match between SettingsMenuUI and MainMenuAudioManager
- Verify PlayerPrefs.Save() is called
- Check that scenes match in Build Settings

### Audio not playing
- Verify AudioSources are in the scene
- Check volume sliders aren't at 0
- Ensure audio clips are assigned to MainMenuAudioManager
- Check browser/system volume in WebGL

### Scene transitions not working
- Verify scene names in MainMenuManager
- Check all scenes are in Build Settings
- Look for errors in Console

## Performance Optimization

- Disable panels when not visible (already done in MainMenuUI)
- Use object pooling for repeated menu transitions
- Unload previous scene when transitioning
- Minimize update calls in menus

## Future Enhancements

- Add controller/gamepad support
- Add language selection
- Add difficulty selection for single player
- Add graphical settings (brightness, contrast)
- Add keybinding customization
- Add pause menu in-game
- Add loading screen between scenes
- Add main menu animations

## Integration with Existing Systems

The main menu integrates with:
- **CharacterSelectionManager** - For player selection
- **TurnManager** - For game initialization
- **GameModeManager** - For game mode handling
- **PlayerHUD** - Displayed after character selection

No modifications needed to existing systems!
