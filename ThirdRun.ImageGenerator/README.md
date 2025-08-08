# ThirdRun Image Generator

A dedicated console application for generating AI-powered images for the ThirdRun game using Google's Gemini AI platform.

## Usage

```bash
cd ThirdRun.ImageGenerator
dotnet run "prompt description" "output/path/image.png"
```

## Examples

```bash
# Generate a warrior character sprite
dotnet run "medieval warrior character sprite for 2D RPG" "../Content/Characters/generated_warrior.png"

# Generate game assets
dotnet run "fantasy potion bottle icon" "../Content/Items/potion.png"
```

## Requirements

- .NET 9.0 or later
- Google Cloud credentials (optional, falls back to placeholder mode)
- Access to Google Cloud AI Platform API (for production use)

## Configuration

The application automatically detects available Google Cloud credentials. If no credentials are found, it operates in placeholder mode, creating sample files for development and testing purposes.

For production use with real AI generation, set up Google Cloud credentials as described in the main project documentation.