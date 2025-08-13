namespace ThirdRun.Data
{
    /// <summary>
    /// Represents the resource cost or generation of an ability
    /// </summary>
    public class ResourceCost
    {
        public ResourceType ResourceType { get; }
        public float Amount { get; }

        /// <summary>
        /// Creates a resource cost
        /// </summary>
        /// <param name="resourceType">The type of resource</param>
        /// <param name="amount">The amount - positive for cost, negative for generation</param>
        public ResourceCost(ResourceType resourceType, float amount)
        {
            ResourceType = resourceType;
            Amount = amount;
        }

        /// <summary>
        /// True if this represents a cost (positive amount)
        /// </summary>
        public bool IsCost => Amount > 0;

        /// <summary>
        /// True if this represents resource generation (negative amount)
        /// </summary>
        public bool IsGeneration => Amount < 0;

        /// <summary>
        /// Gets the absolute amount (always positive)
        /// </summary>
        public float AbsoluteAmount => System.Math.Abs(Amount);
    }
}