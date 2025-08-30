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
public struct Choice : IEquatable<Choice>
{
    [TextArea(2, 5)]
    public string choiceDescription;
    [TextArea(3, 10)]
    public string resultText;
    public bool risky;
    public Amounts suppliesOutcome;
    public int peopleOutcome;
    public Amounts valuablesOutcome;
    public Amounts gearOutcome;
    public EncounterPrerequisiteType typeOfPrerequisites;

    public bool Equals(Choice other)
    {
        return choiceDescription == other.choiceDescription && resultText == other.resultText && risky == other.risky && suppliesOutcome == other.suppliesOutcome && peopleOutcome == other.peopleOutcome && valuablesOutcome == other.valuablesOutcome && gearOutcome == other.gearOutcome && typeOfPrerequisites == other.typeOfPrerequisites;
    }

    public override bool Equals(object obj)
    {
        return obj is Choice other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(choiceDescription, resultText, risky, (int)suppliesOutcome, peopleOutcome, (int)valuablesOutcome, (int)gearOutcome, (int)typeOfPrerequisites);
    }
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
    Conditional,
    Risky
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

[Serializable]
public struct EncounterPrerequisitesRisky : IPrerequisite
{
    public ResourceSystem.ResourceType bettingResource;
    public int minAmount;
    [TextArea(3, 10)]
    public string successText;
    [TextArea(3, 10)]
    public string failureText;
    public Amounts suppliesPositiveOutcome;
    public int peoplePositiveOutcome;
    public Amounts valuablesPositiveOutcome;
    public Amounts gearPositiveOutcome;
    public Amounts suppliesNegativeOutcome;
    public int peopleNegativeOutcome;
    public Amounts valuablesNegativeOutcome;
    public Amounts gearNegativeOutcome;
}
public interface IPrerequisite
{
    
}

