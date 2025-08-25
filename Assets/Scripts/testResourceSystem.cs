using UnityEngine;
using TMPro;
using static ResourceSystem;

public class testResourceSystem : MonoBehaviour
{
    public TextMeshProUGUI waterText;

    private void Start()
    {
        OnResourceChanged += updateText;
        updateText(ResourceType.Water);
    }

    public void addWater(int amount)
    {
        if(!addResource(ResourceType.Water, amount))
        {
            if (amount < 0) Debug.Log("out of water!");
            else Debug.Log("too much water!");
        }
    }

    private void updateText(ResourceType resource)
    {
        waterText.text = "Water amount: " + getResource(ResourceType.Water);
    }
}
