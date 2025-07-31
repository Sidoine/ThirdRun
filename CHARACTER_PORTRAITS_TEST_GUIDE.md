# Character Portraits Feature - Visual Test Guide

## Expected UI Layout

The character portraits feature has been implemented with the following UI changes:

### Left Side Panel - Character Portraits
- **Location**: Far left side of the screen (0-92 pixels from left)
- **Content**: Vertical row of character portrait buttons
- **Portrait Size**: 60x60 pixels each
- **Spacing**: 8 pixels between portraits
- **Background**: Semi-transparent dark background (20, 20, 20, 150)
- **Portrait Button Colors**:
  - Default: Semi-transparent dark (40, 40, 40, 200)
  - Hover: Semi-transparent green (80, 120, 80, 220)

### Portrait Textures
Each character class should display with the appropriate texture:
- **Guerrier**: Characters/warrior.png
- **Mage**: Characters/mage.png  
- **Prêtre**: Characters/priest.png
- **Chasseur**: Characters/hunter.png

### Inventory Panel Adjustment
- **Location**: Moved right to accommodate portraits (92-412 pixels from left)
- **Previous**: Started at x=0
- **Current**: Starts at x=92 (after portrait panel)

### Character Details Panel
When a portrait is clicked, a modal window appears:
- **Size**: 400x500 pixels
- **Position**: Centered on screen
- **Background**: Dark semi-transparent (30, 30, 30, 240)
- **Border**: Gray border (100, 100, 100, 255)
- **Close Button**: "X" button in top-right corner

### Character Details Content
The modal displays:
1. **Character Info**: Name and class
2. **Statistics**: Health, attack power, experience
3. **Equipment**: 
   - Weapon with damage stats (if equipped)
   - Armor with defense stats (if equipped)
4. **Techniques**: List of unlocked abilities
5. **Inventory Count**: Number of items in inventory

## Testing Instructions

To manually test this feature:

1. **Launch the game** - Character portraits should appear on left side
2. **Verify layout** - Inventory panel should be moved right, not overlapping portraits
3. **Hover portraits** - Should show green highlight on mouseover
4. **Click portrait** - Should open character details modal
5. **Verify details** - All character information should display correctly
6. **Close modal** - Click X button to close
7. **Multiple characters** - Test with each character if multiple exist

## Known Limitations

- Character portrait textures must exist in Content/Characters/ folder
- If texture loading fails, portrait button will show without image
- Character details panel displays over other UI elements (modal behavior)

## Files Modified

- `src/UI/Panels/CharacterPortraitsPanel.cs` - New portrait panel
- `src/UI/Panels/CharacterDetailsPanel.cs` - New details modal
- `src/UI/Panels/Root.cs` - Updated layout with portraits
- `src/UI/UIManager.cs` - Added character details state
- `src/UI/Components/Container.cs` - Added Update method for state management