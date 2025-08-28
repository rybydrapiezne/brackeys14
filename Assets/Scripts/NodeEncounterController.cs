using System;
using System.Collections.Generic;
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

    public void EnableEncounter(int amountOfChoices, Sprite encounterImage, string encounterText, string encounterName, Choice[] choices, PrerequisiteWrapper[] prerequisites)
    {
        onEncounterStart?.Invoke();
        this.encounterImage.sprite = encounterImage;
        this.encounterText.text = encounterText;
        this.encounterName.text = encounterName;
        encounterCanvas.SetActive(true); 

        if (amountOfChoices > choiceButtons.Count)
        {
            Debug.LogError($"Not enough choice buttons! Required: {amountOfChoices}, Available: {choiceButtons.Count}");
            return;
        }

        for (int i = 0; i < amountOfChoices; i++)
        {
            int index = i;
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i].choiceDescription;
            
            if (choices[i].typeOfPrerequisites == EncounterPrerequisiteType.Normal)
            {
                if (prerequisites[i].ToIPrerequisite() is EncounterNormalPrerequisistes normalPrerequisite)
                {
                    if (normalPrerequisite.ResourceType != ResourceSystem.ResourceType.People)
                    {
                        if (ResourceSystem.getResource(normalPrerequisite.ResourceType) < (int)normalPrerequisite.AmountNeeded)
                        {
                            choiceButtons[i].GetComponent<Button>().interactable = false;
                        }
                    }
                    else
                    {
                        if (ResourceSystem.getResource(normalPrerequisite.ResourceType) < normalPrerequisite.peopleAmount)
                        {
                            choiceButtons[i].GetComponent<Button>().interactable = false;
                        }
                    }
                }
            }
            else if (choices[i].typeOfPrerequisites == EncounterPrerequisiteType.Conditional)
            {
                if (prerequisites[i].ToIPrerequisite() is EncounterConditionalPrerequisites conditionalPrerequisite)
                {
                    switch (conditionalPrerequisite.condition)
                    {
                        case EncounterConditionalPrerequisitesEnum.Greater:
                            choiceButtons[i].GetComponent<Button>().interactable = 
                                ConditionalPrerequisites.GreaterThan(
                                    ResourceSystem.getResource(conditionalPrerequisite.value1),
                                    ResourceSystem.getResource(conditionalPrerequisite.value2),
                                    ResourceSystem.getResource(conditionalPrerequisite.value3),
                                    ResourceSystem.getResource(conditionalPrerequisite.value4),
                                    ResourceSystem.getResource(conditionalPrerequisite.value5),
                                    conditionalPrerequisite.expected);
                            break;
                        case EncounterConditionalPrerequisitesEnum.Less:
                            choiceButtons[i].GetComponent<Button>().interactable = 
                                ConditionalPrerequisites.LowerThan(
                                    ResourceSystem.getResource(conditionalPrerequisite.value1),
                                    ResourceSystem.getResource(conditionalPrerequisite.value2),
                                    ResourceSystem.getResource(conditionalPrerequisite.value3),
                                    ResourceSystem.getResource(conditionalPrerequisite.value4),
                                    ResourceSystem.getResource(conditionalPrerequisite.value5),
                                    conditionalPrerequisite.expected);
                            break;
                        case EncounterConditionalPrerequisitesEnum.Equal:
                            choiceButtons[i].GetComponent<Button>().interactable = 
                                ConditionalPrerequisites.EqualTo(
                                    ResourceSystem.getResource(conditionalPrerequisite.value1),
                                    ResourceSystem.getResource(conditionalPrerequisite.value2),
                                    ResourceSystem.getResource(conditionalPrerequisite.value3),
                                    ResourceSystem.getResource(conditionalPrerequisite.value4),
                                    ResourceSystem.getResource(conditionalPrerequisite.value5),
                                    conditionalPrerequisite.expected);
                            break;
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
        ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)choice.suppliesOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.People, choice.peopleOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)choice.valuablesOutcome);
        ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)choice.gearOutcome);
        onEncounterEnd?.Invoke();
    }
}