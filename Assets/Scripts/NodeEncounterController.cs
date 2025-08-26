using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class NodeEncounterController : MonoBehaviour
{
    [SerializeField] private Image encounterImage;
    [SerializeField] private TextMeshProUGUI encounterText;
    [SerializeField] private GameObject encounterCanvas;
    
    [SerializeField] private List<GameObject> choiceButtons;

    public void EnableEncounter(int amountOfChoices, Sprite encounterImage, String encounterText, Choice[] choicesDescription)
    {
        this.encounterImage.sprite = encounterImage;
        this.encounterText.text = encounterText;
        encounterCanvas.SetActive(true); 

        for (int i = 0; i < amountOfChoices; i++)
        {
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choicesDescription[i].choiceDescription;
            choiceButtons[i].SetActive(true);
        }
    }
}
