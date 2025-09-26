using System.Collections.Generic;

namespace ThirdRun.Data
{
    /// <summary>
    /// Represents an aura that can be applied to units, providing characteristic modifications
    /// </summary>
    public class Aura
    {
        public string Name { get; }
        public string Description { get; }
        public float BaseDuration { get; }
        public int MaxStacks { get; }
        public bool IsDebuff { get; }
        
        /// <summary>
        /// Characteristic modifiers applied per stack of this aura
        /// </summary>
        public Dictionary<Characteristic, int> CharacteristicModifiers { get; }

        public Aura(string name, string description, float baseDuration, int maxStacks, bool isDebuff = false)
        {
            Name = name;
            Description = description;
            BaseDuration = baseDuration;
            MaxStacks = maxStacks;
            IsDebuff = isDebuff;
            CharacteristicModifiers = new Dictionary<Characteristic, int>();
        }

        /// <summary>
        /// Adds a characteristic modifier that will be applied per stack
        /// </summary>
        public void AddModifier(Characteristic characteristic, int valuePerStack)
        {
            CharacteristicModifiers[characteristic] = valuePerStack;
        }
    }
}