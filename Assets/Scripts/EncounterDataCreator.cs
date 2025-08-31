using UnityEditor;
using UnityEngine;

public class EncounterCreationTools
{
    [MenuItem("Tools/Encounters/Create Oasis Safe")]
    public static void CreateOasisSafeAsset()
    {
        EncounterData oasisSafe = ScriptableObject.CreateInstance<EncounterData>();

        oasisSafe.encounterName = "Oasis";
        oasisSafe.description = "Amid endless sea of sand, you see your salvation. A couple of palms bends over a clear pool, offering shade, water, and a brief escape from the desert’s heat. It seems no one is here beside some small animals…";
        oasisSafe.biome = BiomeType.Desert | BiomeType.Dunes | BiomeType.WarlordsTerritory;
        oasisSafe.depth = 0f;
        oasisSafe.dangerLevel = 0;
        oasisSafe.isUnique = false;
        oasisSafe.choices = new Choice[]
        {
            new Choice
            {
                choiceDescription = "There’s no time to waste. Take a short rest and keep going.",
                resultText = "You take a short time and resupply on water. However you know you have to keep going.",
                risky = false,
                suppliesOutcome = Amounts.PositiveLargeAmount,
                peopleOutcome = 0,
                valuablesOutcome = Amounts.None,
                gearOutcome = Amounts.None,
                typeOfPrerequisites = EncounterPrerequisiteType.None,
                luckOutcome = 0,
                fogOfWarOutcome = 0,
                impendingDoomOutcome = 0
            },
            new Choice
            {
                choiceDescription = "It’s a safe spot. Take time to resupply.",
                resultText = "You spend the night here and have the best rest in recent days. Your caravan is full of supplies. However you know that you let the impending doom to catch up to you.",
                risky = false,
                suppliesOutcome = Amounts.PositiveVeryLargeAmount,
                peopleOutcome = 0,
                valuablesOutcome = Amounts.None,
                gearOutcome = Amounts.None,
                typeOfPrerequisites = EncounterPrerequisiteType.None,
                luckOutcome = 0,
                fogOfWarOutcome = 0,
                impendingDoomOutcome = 1
            }
        };

        AssetDatabase.CreateAsset(oasisSafe, "Assets/EncounterData/Oasis_Safe.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Encounters/Create Oasis Bandits")]
    public static void CreateOasisBanditsAsset()
    {
        EncounterData oasisBandits = ScriptableObject.CreateInstance<EncounterData>();

        oasisBandits.encounterName = "Oasis";
        oasisBandits.description = "Amid endless sea of sand, you see your salvation. A couple of palms bends over a clear pool, offering shade, water, and a brief escape from the desert’s heat. As you get closer you see the oasis is already occupied. Eight figures in black hoods rise to meet you, weapons in hand.";
        oasisBandits.biome = BiomeType.Desert | BiomeType.Dunes;
        oasisBandits.depth = 0f;
        oasisBandits.dangerLevel = 50;
        oasisBandits.isUnique = false;
        oasisBandits.choices = new Choice[]
        {
            new Choice
            {
                choiceDescription = "It’s too dangerous. Steer clear, marching on through the desert losing all hope for much needed rest.",
                resultText = "They stare at you triumphantly while you travel past the oasis keeping a significant distance. At least you didn’t get in any trouble.",
                risky = false,
                suppliesOutcome = Amounts.None,
                peopleOutcome = 0,
                valuablesOutcome = Amounts.None,
                gearOutcome = Amounts.None,
                typeOfPrerequisites = EncounterPrerequisiteType.None,
                luckOutcome = 0,
                fogOfWarOutcome = 0,
                impendingDoomOutcome = 0
            },
            new Choice
            {
                choiceDescription = "Offer them some of your valuables in exchange for water and rest under the palms.",
                resultText = "", // Result text is now in the prerequisite struct
                risky = true,
                suppliesOutcome = Amounts.None,
                peopleOutcome = 0,
                valuablesOutcome = Amounts.None,
                gearOutcome = Amounts.None,
                typeOfPrerequisites = EncounterPrerequisiteType.Risky,
                luckOutcome = 0,
                fogOfWarOutcome = 0,
                impendingDoomOutcome = 0
            },
            new Choice
            {
                choiceDescription = "Draw your weapons, ready to fight.",
                resultText = "", // Result text is now in the prerequisite struct
                risky = true,
                suppliesOutcome = Amounts.None,
                peopleOutcome = 0,
                valuablesOutcome = Amounts.None,
                gearOutcome = Amounts.None,
                typeOfPrerequisites = EncounterPrerequisiteType.Risky,
                luckOutcome = 0,
                fogOfWarOutcome = 0,
                impendingDoomOutcome = 0
            }
        };

        // Populate the prerequisites for the risky choices
        oasisBandits.prerequisites = new PrerequisiteWrapper[]
        {
            new PrerequisiteWrapper
            {
                riskyPrerequisite = new EncounterPrerequisitesRisky
                {
                    bettingResource = ResourceSystem.ResourceType.Valuables,
                    minAmount = 30, // Assuming a reasonable value for 'difficulty 30'
                    successText = "Bandits look at each other and nod. They accept your valuables and step aside. You resupply and rest shortly. Though you must keep your eyes on them throughout the entire time.",
                    failureText = "Bandits laugh at each other when you show them what you’re willing to pay. They take your offering but don’t let you pass and threaten you with their daggers. Some people are not worth bargaining with…",
                    suppliesPositiveOutcome = Amounts.PositiveLargeAmount,
                    peoplePositiveOutcome = 0,
                    valuablesPositiveOutcome = Amounts.None,
                    gearPositiveOutcome = Amounts.None,
                    luckPositiveOutcome = 0,
                    fogPositiveOutcome = 0,
                    doomPositiveOutcome = 0,
                    suppliesNegativeOutcome = Amounts.None,
                    peopleNegativeOutcome = 0,
                    valuablesNegativeOutcome = Amounts.None,
                    gearNegativeOutcome = Amounts.None,
                    luckNegativeOutcome = 0,
                    fogNegativeOutcome = 0,
                    doomNegativeOutcome = 0,
                }
            },
            new PrerequisiteWrapper
            {
                riskyPrerequisite = new EncounterPrerequisitesRisky
                {
                    bettingResource = ResourceSystem.ResourceType.Gear,
                    minAmount = 45, // Assuming a reasonable value for 'difficulty 45'
                    enemiesCount = 8,
                    successText = "After a hard struggle, the bandits scatter into the dunes, leaving their dead and their belongings behind. You claim the water, gather some gear, and even find a few valuables among their packs.",
                    failureText = "The fight turns bloody and chaotic beneath the palms. The cost is heavy. Two of your men lie lifeless in the sand, and you decide to retreat into the scorching heat of the desert.",
                    suppliesPositiveOutcome = Amounts.PositiveLargeAmount,
                    peoplePositiveOutcome = 0,
                    valuablesPositiveOutcome = Amounts.PositiveSmallAmount,
                    gearPositiveOutcome = Amounts.PositiveSmallAmount,
                    luckPositiveOutcome = 0,
                    fogPositiveOutcome = 0,
                    doomPositiveOutcome = 0,
                    suppliesNegativeOutcome = Amounts.None,
                    peopleNegativeOutcome = -2,
                    valuablesNegativeOutcome = Amounts.None,
                    gearNegativeOutcome = Amounts.None,
                    luckNegativeOutcome = 0,
                    fogNegativeOutcome = 0,
                    doomNegativeOutcome = 0,
                }
            }
        };

        AssetDatabase.CreateAsset(oasisBandits, "Assets/EncounterData/Oasis_Bandits.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
}