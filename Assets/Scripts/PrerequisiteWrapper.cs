using System;
using UnityEngine;

[Serializable]
public class PrerequisiteWrapper : IPrerequisite
{
    public EncounterPrerequisiteType prerequisiteType;
    public EncounterNormalPrerequisistes normalPrerequisite;
    public EncounterConditionalPrerequisites conditionalPrerequisite;
    public EncounterPrerequisitesRisky riskyPrerequisite;

    public IPrerequisite ToIPrerequisite()
    {
        if (prerequisiteType == EncounterPrerequisiteType.Normal)
            return normalPrerequisite;
        if (prerequisiteType == EncounterPrerequisiteType.Conditional)
            return conditionalPrerequisite;
        if (prerequisiteType == EncounterPrerequisiteType.Risky)
            return riskyPrerequisite;
        return null;
    }
}