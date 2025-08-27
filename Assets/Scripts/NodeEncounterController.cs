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
                if (choices[i].Prerequisites[0].ResourceType != ResourceSystem.ResourceType.People)
                {
                    if (ResourceSystem.getResource(choices[index].Prerequisites[0].ResourceType) <
                        (int)choices[i].Prerequisites[0].AmountNeeded)
                    {
                        choiceButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
                else
                {
                    if (ResourceSystem.getResource(choices[index].Prerequisites[0].ResourceType) <
                        choices[index].Prerequisites[0].peopleAmount)
                    {
                        choiceButtons[i].GetComponent<Button>().interactable = false;
                    }
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
        ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies,(int) choice.suppliesOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.People,choice.peopleOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables,(int) choice.valuablesOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Gear,(int) choice.gearOutcome);
        onEncounterEnd?.Invoke();
        
    }
}