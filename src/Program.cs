using System;
using System.Threading.Tasks;
using ThirdRun.Utils;

namespace ThirdRun;

class Program
{
    static async Task Main(string[] args)
    {
        // Check if this is an image generation command
        if (args.Length > 0 && args[0] == "--generate-image")
        {
            await HandleImageGenerationCommand(args);
            return;
        }

        // Normal game startup
        using var game = new MonogameRPG.Game1();
        game.Run();
    }

    static async Task HandleImageGenerationCommand(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: ThirdRun --generate-image \"<prompt>\" \"<output-path>\"");
            Console.WriteLine("Example: ThirdRun --generate-image \"warrior character sprite\" \"Content/Characters/generated_warrior.png\"");
            return;
        }

        string prompt = args[1];
        string outputPath = args[2];

        Console.WriteLine("=== ThirdRun Image Generator ===");
        Console.WriteLine($"Prompt: {prompt}");
        Console.WriteLine($"Output: {outputPath}");
        Console.WriteLine();

        var generator = new GeminiImageGenerator();
        
        if (!generator.ValidateConfiguration())
        {
            Console.WriteLine("ERROR: Image generator configuration is invalid.");
            Console.WriteLine("Please ensure Google Cloud credentials are properly configured.");
            return;
        }

        Console.WriteLine("Generating image...");
        bool success = await generator.GenerateImageAsync(prompt, outputPath);
        
        if (success)
        {
            Console.WriteLine("✓ Image generation completed successfully!");
        }
        else
        {
            Console.WriteLine("✗ Image generation failed.");
        }

        generator.Dispose();
    }
}
