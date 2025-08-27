using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewEncounter", menuName = "Encounters/EncounterData")]
public class EncounterData : ScriptableObject
{
    public string encounterName;
    [TextArea(3, 10)]
    public string description;
    public Choice[] choices;
    public Sprite encounterImage;
}

[Serializable]
public struct Choice
{
    [TextArea(2, 5)]
    public string choiceDescription;
    public Amounts suppliesOutcome;
    public int peopleOutcome;
    public Amounts valuablesOutcome;
    public Amounts gearOutcome;
    public EncounterPrerequisistes[] Prerequisites;

}

