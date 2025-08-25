using System;
using System.Collections.Generic;
using static ResourceSystem.ResourceType;

public static class ResourceSystem
{
    public static event Action<ResourceType> OnResourceLimitReached;
    public static event Action<ResourceType> OnOutOfResource;
    public static event Action<ResourceType> OnResourceChanged;

    public enum ResourceType
    {
        Water,
        Food,
        Morale
    }

    private static Dictionary<ResourceType, int> maxResources = new Dictionary<ResourceType, int>
    {
        [Water] = 6,
        [Food] = 10,
        [Morale] = 12,
    };

    private static Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>
    {
        [Water] = 3,
        [Food] = 6,
        [Morale] = 5,
    };

    public static bool addResource(ResourceType resource, int amount)
    {
        bool success = true;
        if (amount >= 0)
        { // resource limit reached
            if (resources[resource] + amount > maxResources[resource])
            {
                resources[resource] = maxResources[resource];
                success = false;
                OnResourceLimitReached?.Invoke(resource);
            }
        } else
        { // out of resource
            if (resources[resource] + amount <= 0)
            {
                resources[resource] = 0;
                success = false;
                OnOutOfResource?.Invoke(resource);
            }
        }
        
        if(success) resources[resource] += amount;

        OnResourceChanged?.Invoke(resource);
        return success;
    }

    public static int getResource(ResourceType resource)
    {
        return resources[resource];
    }
}
