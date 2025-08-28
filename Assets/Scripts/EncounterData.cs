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
    public PrerequisiteWrapper[] prerequisites;
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
    public EncounterPrerequisiteType typeOfPrerequisites;

}


[Serializable]
public struct EncounterNormalPrerequisistes:IPrerequisite
{
    public ResourceSystem.ResourceType ResourceType;
    public Amounts AmountNeeded;
    public int peopleAmount;
}
[Serializable]
public enum EncounterPrerequisiteType
{
    None,
    Normal,
    Conditional
}
[Serializable]
public enum EncounterConditionalPrerequisitesEnum
{
    Greater,
    Less,
    Equal
}
[Serializable]
public struct EncounterConditionalPrerequisites:IPrerequisite
{
    public ResourceSystem.ResourceType value1;
    public ResourceSystem.ResourceType value2;
    public ResourceSystem.ResourceType value3;
    public ResourceSystem.ResourceType value4;
    public ResourceSystem.ResourceType value5;
    public int expected;
    public EncounterConditionalPrerequisitesEnum condition;


}

public interface IPrerequisite
{
    
}

