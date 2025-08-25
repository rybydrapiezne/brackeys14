using UnityEngine;
using TMPro;
using static ResourceSystem;

public class ResourceSystemExample : MonoBehaviour
{
    public TextMeshProUGUI waterText;

    private void Start()
    {
        OnResourceChanged += updateText;
        OnOutOfResource += outOfResource;
        OnResourceLimitReached += limitReached;
        updateText(null, new OnResourceChangedArgs(ResourceType.Water, 0));
    }

    public void addWater(int amount)
    {
        if(!addResource(ResourceType.Water, amount))
        {
            if (amount < 0) Debug.Log("out of water!");
            else Debug.Log("too much water!");
        }
    }

    private void updateText(object s, OnResourceChangedArgs a)
    {
        waterText.text = "Water amount: " + getResource(ResourceType.Water);
        Debug.LogFormat("Updated Resource: {0}, Amount: {1}", a.Resource, a.AmountChanged);
    }

    private void outOfResource(object s, OnOutOfResourceArgs a)
    {
        Debug.LogFormat("Out of Resource: {0}, Amount: {1}, Missing: {2}", a.Resource, a.AmountChanged, a.AmountMissing);
    }

    private void limitReached(object s, OnResourceLimitReachedArgs a)
    {
        Debug.LogFormat("Resource over limit: {0}, Amount: {1}, Over limit amount: {2}", a.Resource, a.AmountChanged, a.AmountOverLimit);
    }
}
