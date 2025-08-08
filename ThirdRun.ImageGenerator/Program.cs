using System;
using System.Threading.Tasks;
using ThirdRun.Utils;

namespace ThirdRun.ImageGenerator;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: ThirdRun.ImageGenerator \"<prompt>\" \"<output-path>\"");
            Console.WriteLine("Example: ThirdRun.ImageGenerator \"warrior character sprite\" \"../Content/Characters/generated_warrior.png\"");
            return;
        }

        string prompt = args[0];
        string outputPath = args[1];

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
