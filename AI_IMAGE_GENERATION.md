# AI Image Generation Feature

This project now includes AI-powered image generation capabilities using Google's Gemini AI platform.

## Features

- **Copilot Agent Environment**: Configured to use .NET Core 9 and Google Gemini API
- **Command Line Interface**: Generate images directly from the command line
- **Game Integration**: Add generated images to the game's content pipeline
- **Automatic Fallback**: Works with or without Google Cloud credentials

## Usage

### Command Line Image Generation

Generate images using the command line interface:

```bash
dotnet run --generate-image "prompt description" "output/path/image.png"
```

**Examples:**
```bash
# Generate a warrior character
dotnet run --generate-image "medieval warrior character sprite for 2D RPG" "Content/Characters/generated_warrior.png"

# Generate a magic spell effect
dotnet run --generate-image "blue fireball spell effect" "Content/Effects/blue_fireball.png"

# Generate a dungeon background
dotnet run --generate-image "dark stone dungeon corridor" "Content/Backgrounds/dungeon_corridor.png"
```

### Programmatic Usage

Use the `GeminiImageGenerator` class in your code:

```csharp
using ThirdRun.Utils;

var generator = new GeminiImageGenerator("your-project-id", "us-central1");

bool success = await generator.GenerateImageAsync(
    "fantasy RPG character portrait", 
    "Content/Characters/portrait.png"
);

if (success)
{
    Console.WriteLine("Image generated successfully!");
}
```

## Configuration

### Google Cloud Setup (Optional)

For production use with actual AI generation:

1. Create a Google Cloud Project
2. Enable the AI Platform API
3. Set up Application Default Credentials:
   ```bash
   gcloud auth application-default login
   ```
4. Update the project ID in your code

### Development Mode

The system works without Google Cloud credentials by creating placeholder files for testing and development. This allows developers to work on the feature without requiring API access.

## Architecture

- **GeminiImageGenerator**: Core service for image generation
- **Program.cs**: CLI interface for image generation commands
- **Automatic Authentication**: Detects available credentials and falls back gracefully
- **Test Coverage**: Comprehensive unit tests with mocked dependencies

## Files Added

- `src/Utils/GeminiImageGenerator.cs` - Main image generation service
- `ThirdRun.Tests/GeminiImageGeneratorTests.cs` - Unit tests
- Updated `src/Program.cs` - CLI interface

## Environment Requirements

- .NET 9.0 or later
- Google.Cloud.AIPlatform.V1 NuGet package (automatically installed)
- Optional: Google Cloud credentials for production use

## Testing

Run the test suite to verify functionality:

```bash
dotnet test
```

All tests pass both with and without Google Cloud credentials, making the feature robust for development and production environments.