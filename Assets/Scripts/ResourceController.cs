using System;
using TMPro;
using UnityEngine;
using static ResourceSystem;

public class ResourceController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI suppliesAmount;
    [SerializeField] private TextMeshProUGUI peopleAmount;
    [SerializeField] private TextMeshProUGUI valuablesAmount;
    [SerializeField] private TextMeshProUGUI gearAmount;
    void Start()
    {
        OnResourceChanged += UpdateText;
        OnOutOfResource += OutOfResource;
        OnResourceLimitReached += LimitReached;
        UpdateText(null, new OnResourceChangedArgs(ResourceType.Supplies, 0));
    }

    private void UpdateText(object s, OnResourceChangedArgs args)
    {
        suppliesAmount.text=getResource(ResourceType.Supplies).ToString();
        peopleAmount.text=getResource(ResourceType.People).ToString();
        valuablesAmount.text=getResource(ResourceType.Valuables).ToString();
        gearAmount.text=getResource(ResourceType.Gear).ToString();
    }

    private void OutOfResource(object s, OnOutOfResourceArgs args)
    {
        
    }

    private void LimitReached(object s, OnResourceLimitReachedArgs args)
    {
        
    }
}
