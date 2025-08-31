using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class NodeEncounterController : MonoBehaviour
{
    [SerializeField] private Image encounterImageBackground;
    [SerializeField] private Image encounterImageForeground;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private TextMeshProUGUI encounterName;
    [SerializeField] private GameObject encounterCanvas;
    [SerializeField] private GameObject riskySituationCanvas;
    [SerializeField] private Image riskySituationImage;
    [SerializeField] private Slider riskySituationSlider;
    [SerializeField] private Button riskySituationAcceptButton;
    [SerializeField] private TextMeshProUGUI difficultyAmountText;
    [SerializeField] private TextMeshProUGUI resourceBetAmountText;
    [SerializeField] private TextMeshProUGUI successRateText;
    private int currentDifficulty = 0;
    [SerializeField] private List<GameObject> choiceButtons;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private List<resourceImageData> resourceImages;
    [SerializeField] private List<biomeImageData> biomeImages;
    public static Action onEncounterStart;
    public static Action onEncounterEnd;
    private Dictionary<Choice,EncounterPrerequisitesRisky> prerequisitesDictionary=new();
    
    public void EnableEncounter(int amountOfChoices, Sprite encounterImage, string encounterText, string encounterName, Choice[] choices, PrerequisiteWrapper[] prerequisites,BiomeType biomeType)
    {
        onEncounterStart?.Invoke();
        encounterImageBackground.sprite = biomeImages.FirstOrDefault(biomeImageData => biomeImageData.biomeType == biomeType).image;
        encounterImageForeground.sprite = encounterImage;
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
                    prerequisitesDictionary.Add(choices[i], riskyPrerequisite);
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
        AudioManager.Instance.click.Play();
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
            ImpendingDoom.Instance.doomLevel += choice.impendingDoomOutcome;
            ApplyLuck(choice.luckOutcome);
            FogOfWarManager.Instance.MoveFogForward(choice.fogOfWarOutcome);
            encounterText.text = choice.resultText;
            if(choice.addDebt)
                TurnController.Instance.hasTakenContract = true;
            HandleEncounterEnd();
        }
    }

    private void HandleRiskySituation(Choice choice)
    {
        if (prerequisitesDictionary.ContainsKey(choice) && prerequisitesDictionary[choice].enemiesCount > 0)
        {
            EncounterPrerequisitesRisky tempPrerequisites = prerequisitesDictionary[choice];
            tempPrerequisites.minAmount = tempPrerequisites.minAmount - 5 * (ResourceSystem.getResource(ResourceSystem.ResourceType.People) - tempPrerequisites.enemiesCount);
            prerequisitesDictionary[choice] = tempPrerequisites;
        }
        currentDifficulty = prerequisitesDictionary[choice].minAmount;
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
        riskySituationSlider.onValueChanged.AddListener(delegate { OnSliderValueChange(); });
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
        AudioManager.Instance.click.Play();
        encounterCanvas.SetActive(false);
        onEncounterEnd?.Invoke();
    }

    private void OnSliderValueChange()
    {
        if (currentDifficulty != 0)
        {
            float successRate = 100 * ((float)riskySituationSlider.value / (float)currentDifficulty);
            successRateText.text = $"SUCCESS RATE: {(int)successRate}%";    
        }
        resourceBetAmountText.text = riskySituationSlider.value.ToString();
    }

    private void Gamble(Choice choice)
    {
        AudioManager.Instance.click.Play();
        ResourceSystem.addResource(prerequisitesDictionary[choice].bettingResource, -(int)riskySituationSlider.value);
        int rand = Random.Range(0, prerequisitesDictionary[choice].minAmount);
        
        if (rand <= riskySituationSlider.value)
        {
            encounterText.text = prerequisitesDictionary[choice].successText;
            ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)prerequisitesDictionary[choice].suppliesPositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.People, prerequisitesDictionary[choice].peoplePositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)prerequisitesDictionary[choice].valuablesPositiveOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)prerequisitesDictionary[choice].gearPositiveOutcome);
            ApplyLuck(prerequisitesDictionary[choice].luckPositiveOutcome);
            FogOfWarManager.Instance.MoveFogForward(prerequisitesDictionary[choice].fogPositiveOutcome);
            ImpendingDoom.Instance.doomLevel += prerequisitesDictionary[choice].doomPositiveOutcome;
        }
        else
        {
            encounterText.text = prerequisitesDictionary[choice].failureText;
            ResourceSystem.addResource(ResourceSystem.ResourceType.Supplies, (int)prerequisitesDictionary[choice].suppliesNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.People, prerequisitesDictionary[choice].peopleNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Valuables, (int)prerequisitesDictionary[choice].valuablesNegativeOutcome);
            ResourceSystem.addResource(ResourceSystem.ResourceType.Gear, (int)prerequisitesDictionary[choice].gearNegativeOutcome);
            ApplyLuck(prerequisitesDictionary[choice].luckNegativeOutcome);
            FogOfWarManager.Instance.MoveFogForward(prerequisitesDictionary[choice].fogNegativeOutcome);
            ImpendingDoom.Instance.doomLevel += prerequisitesDictionary[choice].doomNegativeOutcome;
        }
        encounterCanvas.SetActive(true);
        riskySituationCanvas.SetActive(false);
        HandleEncounterEnd();
    }

    public void Cancel()
    {
        AudioManager.Instance.click.Play();
        encounterCanvas.SetActive(true);
        riskySituationCanvas.SetActive(false);
    }

    public static void ApplyLuck(float multiplier)
    {
        if (multiplier == 0) return;

        TurnController tc = TurnController.Instance;
        if (tc == null)
        {
            Debug.LogError("TurnController not found in scene!");
            return;
        }

        LuckStatus newStatus = new LuckStatus
        {
            turnsLeft = 3,
            multiplier = multiplier
        };

        tc.AddLuckStatus(newStatus);
    }
    
}

[Serializable]
public struct resourceImageData
{
    public ResourceSystem.ResourceType resourceType;
    public Sprite image;
}

[Serializable]
public struct biomeImageData
{
    public BiomeType biomeType;
    public Sprite image;
}