using System;
using System.Collections.Generic;
using static ResourceSystem.ResourceType;


public static class ResourceSystem
{
    public static event EventHandler<OnResourceLimitReachedArgs> OnResourceLimitReached;
    public static event EventHandler<OnOutOfResourceArgs> OnOutOfResource;
    public static event EventHandler<OnResourceChangedArgs> OnResourceChanged;

    public enum ResourceType
    {
        None,
        Supplies,
        People,
        Valuables,
        Gear
    }

    private static Dictionary<ResourceType, int> maxResources = new Dictionary<ResourceType, int>
    {
        [None]=0,
        [Supplies] = 100,
        [People] = 20,
        [Valuables]=1000,
        [Gear]=1000
    };

    private static Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>
    {
        [None]=0,
        [Supplies] = 100,
        [People] = 10,
        [Valuables]=0,
        [Gear]=50
        
    };

    public static bool addResource(ResourceType resource, int amount)
    {
        bool success = true;
        int desired = resources[resource] + amount;
        if (amount >= 0)
        { // resource limit reached
            int max = maxResources[resource];
            if (desired > max)
            {
                resources[resource] = max;
                success = false;
                OnResourceLimitReached?.Invoke(null, new OnResourceLimitReachedArgs(resource, amount, desired - max));
            }
        } else
        { // out of resource
            if (desired < 0)
            {
                resources[resource] = 0;
                success = false;
                OnOutOfResource?.Invoke(null, new OnOutOfResourceArgs(resource, amount, - desired));
            }
        }

        if (success) resources[resource] += amount;
        
        OnResourceChanged?.Invoke(null, new OnResourceChangedArgs(resource, amount));

        return success;
    }

    public static int getResource(ResourceType resource)
    {
        return resources[resource];
    }
}

public class ResourceEventArgs : EventArgs
{
    public ResourceSystem.ResourceType Resource { get; }
    public int AmountChanged { get; }

    public ResourceEventArgs(ResourceSystem.ResourceType resource, int amountChanged)
    {
        Resource = resource;
        AmountChanged = amountChanged;
    }
}


public class OnResourceLimitReachedArgs : ResourceEventArgs
{
    /// <summary>How much the attempted addition exceeded the max (positive integer).</summary>
    public int AmountOverLimit { get; }

    public OnResourceLimitReachedArgs(ResourceSystem.ResourceType resource, int amountChanged, int amountOverLimit) : base(resource, amountChanged)
    {
        AmountOverLimit = amountOverLimit;
    }
}

public class OnOutOfResourceArgs : ResourceEventArgs
{
    /// <summary>How much of the attempted usage of resources is missing (positive integer)</summary>
    public int AmountMissing { get; }

    public OnOutOfResourceArgs(ResourceSystem.ResourceType resource, int amountChanged, int amountMissing) : base(resource, amountChanged)
    {
        AmountMissing = amountMissing;
    }
}

public class OnResourceChangedArgs : ResourceEventArgs
{
    public OnResourceChangedArgs(ResourceSystem.ResourceType resource, int amountChanged) : base(resource, amountChanged)
    {
    }
}