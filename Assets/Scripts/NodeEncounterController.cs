using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NodeEncounterController : MonoBehaviour
{
    [SerializeField] private Image encounterImage;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private TextMeshProUGUI encounterName;
    [SerializeField] private GameObject encounterCanvas;
    [SerializeField] private GameObject riskySituationCanvas;
    [SerializeField] private Image riskySituationImage;
    [SerializeField] private Slider riskySituationSlider;
    [SerializeField] private Button riskySituationAcceptButton;
    [SerializeField] private TextMeshProUGUI difficultyAmountText;
    [SerializeField] private TextMeshProUGUI resourceBetAmountText;
    [SerializeField] private List<GameObject> choiceButtons;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private List<resourceImageData> resourceImages;
    public static Action onEncounterStart;
    public static Action onEncounterEnd;
    
    private Dictionary<Choice,EncounterPrerequisitesRisky> prerequisitesDictionary=new();
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
            else if (choices[i].typeOfPrerequisites == EncounterPrerequisiteType.Risky)
            {
                if (prerequisites[i].ToIPrerequisite() is EncounterPrerequisitesRisky riskyPrerequisite)
                {
                    prerequisitesDictionary.Add(choices[i],riskyPrerequisite);
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
        if (choice.risky)
        {
            HandleRiskySituation(choice);
        }
        else
        {
            ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)choice.suppliesOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.People, choice.peopleOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)choice.valuablesOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)choice.gearOutcome);
            encounterText.text = choice.resultText;
            HandleEncounterEnd();
        }
    }

    private void HandleRiskySituation(Choice choice)
    {
        encounterCanvas.SetActive(false);
        riskySituationCanvas.SetActive(true);
        riskySituationSlider.maxValue = ResourceSystem.getResource(prerequisitesDictionary[choice].bettingResource);
        difficultyAmountText.text = prerequisitesDictionary[choice].minAmount.ToString();
        foreach (resourceImageData image in resourceImages)
        {
            if (image.resourceType == prerequisitesDictionary[choice].bettingResource)
            {
                riskySituationImage.sprite = image.image;
                break;
            }
        }
        riskySituationAcceptButton.onClick.AddListener(() => Gamble(choice));
        riskySituationSlider.onValueChanged.AddListener(delegate{OnSliderValueChange();});
    }

    private void HandleEncounterEnd()
    {
        foreach (GameObject button in choiceButtons)
        {
            button.SetActive(false);
        }
        closeButton.SetActive(true);
    }

    public void ClosePopup()
    {
        encounterCanvas.SetActive(false);
        onEncounterEnd?.Invoke();
    }

    private void OnSliderValueChange()
    {
        resourceBetAmountText.text=riskySituationSlider.value.ToString();
    }

    private void Gamble(Choice choice)
    {
        ResourceSystem.addResource(prerequisitesDictionary[choice].bettingResource,-(int)riskySituationSlider.value);
        int rand=Random.Range(0,prerequisitesDictionary[choice].minAmount);
        if (rand <= riskySituationSlider.value)
        {
            encounterText.text = prerequisitesDictionary[choice].successText;
            ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)prerequisitesDictionary[choice].suppliesPositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.People, prerequisitesDictionary[choice].peoplePositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)prerequisitesDictionary[choice].valuablesPositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)prerequisitesDictionary[choice].gearPositiveOutcome);
        }
        else
        {
            encounterText.text = prerequisitesDictionary[choice].failureText;
            ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)prerequisitesDictionary[choice].suppliesNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.People, prerequisitesDictionary[choice].peopleNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)prerequisitesDictionary[choice].valuablesNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)prerequisitesDictionary[choice].gearNegativeOutcome);
        }
        encounterCanvas.SetActive(true);
        riskySituationCanvas.SetActive(false);
        HandleEncounterEnd();
    }

    public void Cancel()
    {
        encounterCanvas.SetActive(true);
        riskySituationCanvas.SetActive(false);
    }
}

[Serializable]
public struct resourceImageData
{
    public ResourceSystem.ResourceType resourceType;
    public Sprite image;
}