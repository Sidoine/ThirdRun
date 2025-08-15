# ThirdRun - MonoGame RPG Project

## Technical Stack
- **Programming Language**: C# (.NET 8.0)
- **Game Framework**: MonoGame Framework DesktopGL (version 3.8.2.1105)
- **Test Framework**: xUnit (version 2.4.2) with Microsoft.NET.Test.Sdk
- **Additional Libraries**:
  - FontStashSharp.MonoGame (version 1.3.10) - Font rendering
  - StbTrueTypeSharp (version 1.26.12) - TrueType font support
  - MonoGame.Content.Builder.Task - Content pipeline

## Sprite Generation
- **Tool**: ImageGenTool (https://www.nuget.org/packages/ImageGenTool)
- **API Key**: GEMINI_API_KEY secret is available for AI-powered image generation
- **Installation**: Install as a global .NET tool: `dotnet tool install --global ImageGenTool`
- **Usage**: All sprites MUST be generated using this tool - no other image generation method is allowed
- **Content Directory**: Generated sprites should be placed in appropriate subdirectories under `/Content/`
- **Requirements**: Images must be generated with ImageGenTool and no other way

## Project Structure
```
/src/
├── Data/           # Game data models and logic
│   ├── Abilities/  # Character abilities and skills system
│   ├── Characters/ # Character classes and properties
│   ├── Items/      # Equipment, weapons, armor, potions
│   ├── Map/        # World map and tile system
│   └── Monsters/   # Monster definitions and behavior
├── Graphics/       # Rendering and visual components
│   ├── Characters/ # Character sprites and animations
│   ├── Map/        # Map rendering and tile graphics
│   ├── Monsters/   # Monster sprites
│   └── UI/         # User interface rendering
├── UI/             # User interface system
│   ├── Components/ # Reusable UI components (buttons, tooltips, etc.)
│   └── Panels/     # Game panels (inventory, character details, etc.)
└── Utils/          # Utility classes and helpers

/ThirdRun.Tests/    # Unit and integration tests
/Content/           # Game assets (textures, fonts, sounds)
/doc/               # Documentation files (implementation guides, design docs)
```

## Game Description
Ce projet est un jeu de rôle en 2D où le groupe de personnages joue de manière autonome. Le groupe se déplace dans un monde généré pour combattre des monstres et collecter des objets.

### Core Systems
- **Character Classes**: Guerrier, voleur, mage, prêtre, paladin, druide, chasseur
- **Equipment System**: Armes, armures, potions avec système d'équipement
- **Combat System**: Combat automatique contre des monstres
- **Ability System**: Compétences débloquées selon la classe et le niveau
- **Inventory Management**: Gestion des objets avec drag & drop
- **Map Generation**: Génération procédurale du monde de jeu

## Coding Conventions
- Use nullable reference types (`<Nullable>enable</Nullable>`)
- Follow C# naming conventions (PascalCase for classes, camelCase for fields)
- Prefer composition over inheritance for game components
- Use dependency injection pattern where appropriate
- Write comprehensive unit tests for game logic (non-graphics)
- Separate rendering logic from game logic

## MonoGame-Specific Patterns
- **Game Loop**: Main game logic in `Game1.cs` with Update/Draw methods
- **Content Management**: Load assets using `ContentManager`
- **Graphics Pipeline**: Use `SpriteBatch` for 2D rendering
- **Input Handling**: MonoGame input system for keyboard/mouse
- **UI Framework**: Custom UI system with panels and components

## Testing Strategy
- Unit tests for game logic and data models
- Integration tests for character abilities and equipment
- UI component tests for panels and drag-drop functionality
- Mock MonoGame dependencies (ContentManager, GraphicsDevice) for testing
- Test coverage includes: abilities, characters, combat, inventory, items, maps

## Development Guidelines
- Keep game state separate from rendering
- Use interfaces for testable components
- Implement proper dispose patterns for MonoGame resources
- Handle content loading asynchronously when possible
- Maintain clean separation between UI logic and game logic

## Documentation
- All implementation documentation and design documents should be placed in the `/doc/` directory
- This includes feature implementation guides, architecture decisions, and technical specifications
