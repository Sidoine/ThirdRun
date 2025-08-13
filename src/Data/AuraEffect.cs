using System;

namespace ThirdRun.Data
{
    /// <summary>
    /// Represents an active aura effect on a unit
    /// </summary>
    public class AuraEffect
    {
        public Aura Aura { get; }
        public float RemainingDuration { get; set; }
        public int Stacks { get; set; }
        public float AppliedTime { get; }

        public AuraEffect(Aura aura, float appliedTime, int initialStacks = 1)
        {
            Aura = aura;
            AppliedTime = appliedTime;
            RemainingDuration = aura.BaseDuration;
            Stacks = Math.Min(initialStacks, aura.MaxStacks);
        }

        /// <summary>
        /// Updates the aura effect with the current game time
        /// Returns true if the aura is still active, false if expired
        /// </summary>
        public bool Update(float currentTime, float deltaTime)
        {
            RemainingDuration -= deltaTime;
            return RemainingDuration > 0;
        }

        /// <summary>
        /// Adds stacks to this aura effect, up to the maximum allowed
        /// Refreshes the duration to the base duration
        /// </summary>
        public void AddStacks(int stacksToAdd)
        {
            Stacks = Math.Min(Stacks + stacksToAdd, Aura.MaxStacks);
            RemainingDuration = Aura.BaseDuration; // Refresh duration
        }

        /// <summary>
        /// Gets the total modifier for a specific characteristic based on current stacks
        /// </summary>
        public int GetCharacteristicModifier(Characteristic characteristic)
        {
            if (Aura.CharacteristicModifiers.TryGetValue(characteristic, out var modifierPerStack))
            {
                return modifierPerStack * Stacks;
            }
            return 0;
        }
    }
}