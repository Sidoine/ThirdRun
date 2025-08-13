namespace ThirdRun.Data
{
    /// <summary>
    /// Represents a resource with current value, maximum value, and replenish rate
    /// </summary>
    public class Resource
    {
        public ResourceType Type { get; }
        public float CurrentValue { get; set; }
        public float MaxValue { get; }
        public float ReplenishRate { get; } // Units per second

        public Resource(ResourceType type, float maxValue, float replenishRate, float? startingValue = null)
        {
            Type = type;
            MaxValue = maxValue;
            ReplenishRate = replenishRate;
            CurrentValue = startingValue ?? maxValue; // Start at full by default
        }

        /// <summary>
        /// Replenishes the resource based on the time elapsed
        /// </summary>
        /// <param name="deltaTime">Time elapsed in seconds</param>
        public void Replenish(float deltaTime)
        {
            if (deltaTime <= 0) return;
            
            CurrentValue += ReplenishRate * deltaTime;
            if (CurrentValue > MaxValue)
                CurrentValue = MaxValue;
        }

        /// <summary>
        /// Attempts to consume the specified amount of resource
        /// </summary>
        /// <param name="amount">Amount to consume</param>
        /// <returns>True if the resource was consumed, false if insufficient</returns>
        public bool TryConsume(float amount)
        {
            if (CurrentValue < amount)
                return false;

            CurrentValue -= amount;
            if (CurrentValue < 0)
                CurrentValue = 0;
            
            return true;
        }

        /// <summary>
        /// Attempts to generate the specified amount of resource (negative cost)
        /// </summary>
        /// <param name="amount">Amount to generate</param>
        /// <returns>True if the resource was generated without exceeding max, false otherwise</returns>
        public bool TryGenerate(float amount)
        {
            if (CurrentValue + amount > MaxValue)
                return false;

            CurrentValue += amount;
            return true;
        }

        /// <summary>
        /// Checks if the resource has enough for the specified amount
        /// </summary>
        public bool HasEnough(float amount)
        {
            return CurrentValue >= amount;
        }

        /// <summary>
        /// Checks if generating the specified amount would exceed maximum
        /// </summary>
        public bool WouldExceedMax(float amount)
        {
            return CurrentValue + amount > MaxValue;
        }
    }
}