## Character Portrait Resource Bars Implementation

This implementation adds health and resource bars to character portraits as requested in issue #70.

### Features Added

#### Visual Elements
- **Health Bar**: Displays above the energy bar using lime green color (Color.LimeGreen)
- **Energy Bar**: Displays below the health bar using green color (Color.Green) as specified
- **Bar Dimensions**: 50px wide × 4px height, positioned 8px from portrait bottom
- **Background Colors**: Dark red for health bar background, dark gray for energy bar background

#### Implementation Details

```csharp
// Bar positioning constants
private const int BarHeight = 4;
private const int BarWidth = 50;
private const int BarSpacing = 2;  // Space between health and energy bars
private const int BarOffset = 8;   // Distance from bottom of portrait

// Resource colors as specified in requirements
private static readonly Color HealthBarColor = Color.LimeGreen;      // Health bar foreground
private static readonly Color HealthBarBackground = Color.DarkRed;   // Health bar background
private static readonly Color EnergyBarColor = Color.Green;          // Energy bar (green as required)
private static readonly Color EnergyBarBackground = Color.DarkGray;  // Energy bar background
```

#### Bar Calculation Logic
- Uses `character.CurrentHealth / character.GetEffectiveMaxHealth()` for health percentage
- Uses `energyResource.CurrentValue / energyResource.MaxValue` for energy percentage  
- Applies `MathHelper.Clamp(percent, 0f, 1f)` to ensure valid range
- Draws background rectangle first, then foreground based on percentage

#### Integration Points
- Extends existing `CharacterPortrait.Draw()` method
- Uses established `Helpers.GetPixel()` pattern for rendering
- Leverages existing `ResourceManager` and `ResourceType.Energy` system
- Follows existing UI color schemes and positioning conventions

### Code Structure

The implementation adds:
1. **Constants**: Bar dimensions and positioning
2. **Color Definitions**: Resource-specific colors with green for energy as required
3. **DrawResourceBars()**: Method to render both health and energy bars
4. **DrawBar()**: Generic bar rendering method with percentage calculation
5. **Draw() Override**: Calls base implementation then adds resource bars

### Testing

Added comprehensive test coverage with 8 new tests:
- ✅ Verifies characters have Energy resource with correct default values
- ✅ Tests health and energy value validation for bar calculations
- ✅ Validates percentage calculation logic (0%, 50%, 75%, 100%)
- ✅ Confirms resource colors match requirements (Energy = Green)
- ✅ Tests resource value changes (damage/energy consumption scenarios)

### Visual Result

When running the game, each character portrait will show:
```
┌─────────────────┐
│   [Portrait]    │  ← Character class icon (warrior, mage, etc.)
│                 │
├─────────────────┤
│████████░░      │  ← Health bar (lime green filled, dark red background)
│██████░░░░      │  ← Energy bar (green filled, dark gray background) 
└─────────────────┘
```

The bars dynamically update to reflect:
- Current health vs. maximum health (including aura bonuses)
- Current energy vs. maximum energy (100 by default)
- Real-time changes during combat and ability usage

This implementation fully satisfies the issue requirements:
- ✅ Health bar displayed on portrait
- ✅ Energy resource bar displayed with green color as specified  
- ✅ Resource types have defined colors
- ✅ Minimal code changes following existing patterns
- ✅ Comprehensive test coverage