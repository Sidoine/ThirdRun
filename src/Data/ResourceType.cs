using Microsoft.Xna.Framework;

namespace ThirdRun.Data
{
    /// <summary>
    /// Represents a resource type with its configuration including color, max value, and replenish rate
    /// </summary>
    public class ResourceType
    {
        public string Name { get; }
        public Color Color { get; }
        public float MaxValue { get; }
        public float ReplenishRate { get; }

        private ResourceType(string name, Color color, float maxValue, float replenishRate)
        {
            Name = name;
            Color = color;
            MaxValue = maxValue;
            ReplenishRate = replenishRate;
        }

        // Static instances for each resource type
        public static readonly ResourceType Energy = new ResourceType("Energy", Color.Green, 100f, 10f);
        
        // Future resource types can be added here (e.g. Mana, Rage, Focus, etc.)
        // public static readonly ResourceType Mana = new ResourceType("Mana", Color.Blue, 100f, 5f);
        
        public override bool Equals(object? obj)
        {
            return obj is ResourceType other && Name == other.Name;
        }
        
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        
        public override string ToString()
        {
            return Name;
        }
    }
}