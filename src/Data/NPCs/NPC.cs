using Microsoft.Xna.Framework;
using MonogameRPG;

namespace ThirdRun.Data.NPCs
{
    public class NPC : Unit
    {
        public string Name { get; set; }
        public NPCType Type { get; set; }

        public NPC(string name, NPCType type, Vector2 position, MonogameRPG.Map.Map map, MonogameRPG.Map.WorldMap worldMap) : base(map, worldMap)
        {
            Name = name;
            Type = type;
            Position = position;
            
            // NPCs are friendly, so they have basic stats but won't fight
            MaxHealth = 100;
            CurrentHealth = MaxHealth;
            AttackPower = 0; // NPCs don't attack
        }
        
        public string GetGreeting()
        {
            return Type switch
            {
                NPCType.Merchant => $"Greetings traveler! I am {Name}, a merchant.",
                NPCType.Guard => $"Welcome to town! I am {Name}, town guard.",
                NPCType.Innkeeper => $"Come rest at my inn! I am {Name}.",
                NPCType.Blacksmith => $"Need weapons or armor? I am {Name}, the blacksmith.",
                _ => $"Hello! I am {Name}."
            };
        }
    }
}