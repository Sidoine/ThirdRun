using System.Collections.Generic;
using System.Linq;

namespace ThirdRun.Data
{
    /// <summary>
    /// Manages all resources for a unit
    /// </summary>
    public class ResourceManager
    {
        private readonly Dictionary<ResourceType, Resource> _resources;

        public ResourceManager()
        {
            _resources = new Dictionary<ResourceType, Resource>();
            
            // Initialize default resources
            InitializeDefaultResources();
        }

        /// <summary>
        /// Initializes the default resources using the ResourceType configuration
        /// </summary>
        private void InitializeDefaultResources()
        {
            _resources[ResourceType.Energy] = new Resource(ResourceType.Energy);
            _resources[ResourceType.Mana] = new Resource(ResourceType.Mana);
        }

        /// <summary>
        /// Gets a resource by type
        /// </summary>
        public Resource? GetResource(ResourceType resourceType)
        {
            return _resources.TryGetValue(resourceType, out var resource) ? resource : null;
        }

        /// <summary>
        /// Gets the current value of a resource
        /// </summary>
        public float GetCurrentValue(ResourceType resourceType)
        {
            var resource = GetResource(resourceType);
            return resource?.CurrentValue ?? 0f;
        }

        /// <summary>
        /// Gets the maximum value of a resource
        /// </summary>
        public float GetMaxValue(ResourceType resourceType)
        {
            var resource = GetResource(resourceType);
            return resource?.MaxValue ?? 0f;
        }

        /// <summary>
        /// Checks if there's enough of a resource for the specified amount
        /// </summary>
        public bool HasEnoughResource(ResourceType resourceType, float amount)
        {
            var resource = GetResource(resourceType);
            return resource?.HasEnough(amount) ?? false;
        }

        /// <summary>
        /// Attempts to consume a resource
        /// </summary>
        public bool TryConsumeResource(ResourceType resourceType, float amount)
        {
            var resource = GetResource(resourceType);
            return resource?.TryConsume(amount) ?? false;
        }

        /// <summary>
        /// Attempts to generate a resource (negative cost)
        /// </summary>
        public bool TryGenerateResource(ResourceType resourceType, float amount)
        {
            var resource = GetResource(resourceType);
            return resource?.TryGenerate(amount) ?? false;
        }

        /// <summary>
        /// Checks if generating a resource would exceed its maximum
        /// </summary>
        public bool WouldExceedMaxResource(ResourceType resourceType, float amount)
        {
            var resource = GetResource(resourceType);
            return resource?.WouldExceedMax(amount) ?? false;
        }

        /// <summary>
        /// Updates all resources, replenishing them based on delta time
        /// </summary>
        public void UpdateResources(float deltaTime)
        {
            if (deltaTime <= 0) return;
            
            foreach (var resource in _resources.Values)
            {
                resource.Replenish(deltaTime);
            }
        }

        /// <summary>
        /// Gets all resource types that are currently managed
        /// </summary>
        public IEnumerable<ResourceType> GetAllResourceTypes()
        {
            return _resources.Keys.ToList();
        }

        /// <summary>
        /// Adds or updates a resource type with specified starting value
        /// </summary>
        public void AddResource(ResourceType resourceType, float? startingValue = null)
        {
            _resources[resourceType] = new Resource(resourceType, startingValue);
        }
    }
}