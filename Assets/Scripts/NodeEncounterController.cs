using System;
using System.Collections.Generic;
using Mono.Cecil;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NodeEncounterController : MonoBehaviour
{
    [SerializeField] private Image encounterImage;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private TextMeshProUGUI encounterName;
    [SerializeField] private GameObject encounterCanvas;
    [SerializeField] private List<GameObject> choiceButtons;
    public static Action onEncounterStart;
    public static Action onEncounterEnd;

    public void EnableEncounter(int amountOfChoices, Sprite encounterImage, String encounterText,String encounterName, Choice[] choices)
    {
        onEncounterStart?.Invoke();
        this.encounterImage.sprite = encounterImage;
        this.encounterText.text = encounterText;
        this.encounterName.text = encounterName;
        encounterCanvas.SetActive(true); 
        
        for (int i = 0; i < amountOfChoices; i++)
        {
            int index = i;
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i].choiceDescription;
            if (choices[i].Prerequisites.Length > 0)
            {
                if (ResourceSystem.getResource(choices[index].Prerequisites[0].ResourceType) <
                    choices[i].Prerequisites[0].AmountNeeded)
                {
                    choiceButtons[i].GetComponent<Button>().interactable = false;
                }
            }
            choiceButtons[i].SetActive(true);
            
            Button button = choiceButtons[i].GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            
            button.onClick.AddListener(() => HandleChoiceSelected(choices[index]));
        }
    }

    private void HandleChoiceSelected(Choice choice)
    {
        encounterCanvas.SetActive(false);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Food, choice.foodOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Water, choice.waterOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Morale,choice.moraleOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.People, choice.peopleOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables,choice.valuablesOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, choice.gearOutcome);
        onEncounterEnd?.Invoke();
        
    }
}