using System;
using TMPro;
using UnityEngine;
using static ResourceSystem;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI waterAmount;
    [SerializeField] private TextMeshProUGUI moraleAmount;
    [SerializeField] private TextMeshProUGUI foodAmount;
    void Start()
    {
        OnResourceChanged += UpdateText;
        OnOutOfResource += OutOfResource;
        OnResourceLimitReached += LimitReached;
        UpdateText(null, new OnResourceChangedArgs(ResourceType.Water, 0));
    }

    private void UpdateText(object s, OnResourceChangedArgs args)
    {
        waterAmount.text=getResource(ResourceType.Water).ToString();
        moraleAmount.text=getResource(ResourceType.Morale).ToString();
        foodAmount.text=getResource(ResourceType.Food).ToString();
    }

    private void OutOfResource(object s, OnOutOfResourceArgs args)
    {
        
    }

    private void LimitReached(object s, OnResourceLimitReachedArgs args)
    {
        
    }
}
