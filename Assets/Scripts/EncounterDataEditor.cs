using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(EncounterData))]
public class EncounterDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SerializedObject so = serializedObject;
        so.Update();

        SerializedProperty encounterName = so.FindProperty("encounterName");
        SerializedProperty description = so.FindProperty("description");
        SerializedProperty dangerLevel = so.FindProperty("dangerLevel");
        SerializedProperty biome = so.FindProperty("biome");
        SerializedProperty depth = so.FindProperty("depth");
        SerializedProperty choices = so.FindProperty("choices");
        SerializedProperty encounterImage = so.FindProperty("encounterImage");
        SerializedProperty prerequisites = so.FindProperty("prerequisites");
        SerializedProperty isUnique = so.FindProperty("isUnique");

        EditorGUILayout.PropertyField(encounterName);
        EditorGUILayout.PropertyField(description);
        EditorGUILayout.PropertyField(dangerLevel);
        EditorGUILayout.PropertyField(isUnique);
        EditorGUILayout.PropertyField(encounterImage);
        EditorGUILayout.PropertyField(biome);
        
        EditorGUILayout.LabelField("Depth", EditorStyles.boldLabel);
        float newDepth = EditorGUILayout.Slider(depth.floatValue, 0f, 1f);
        newDepth = Mathf.Round(newDepth * 10f) / 10f;
        if (newDepth != depth.floatValue)
        {
            depth.floatValue = newDepth;
        }

        EditorGUILayout.PropertyField(choices, true);

        if (choices.arraySize > 0 && prerequisites.arraySize != choices.arraySize)
        {
            prerequisites.arraySize = choices.arraySize;
            for (int i = 0; i < prerequisites.arraySize; i++)
            {
                if (prerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("prerequisiteType").enumValueIndex == 0)
                {
                    prerequisites.GetArrayElementAtIndex(i).FindPropertyRelative("prerequisiteType").enumValueIndex = 
                        choices.GetArrayElementAtIndex(i).FindPropertyRelative("typeOfPrerequisites").enumValueIndex;
                }
            }
        }

        EditorGUILayout.LabelField("Prerequisites", EditorStyles.boldLabel);
        for (int i = 0; i < choices.arraySize; i++)
        {
            SerializedProperty choice = choices.GetArrayElementAtIndex(i);
            SerializedProperty typeOfPrerequisites = choice.FindPropertyRelative("typeOfPrerequisites");
            SerializedProperty prerequisite = prerequisites.GetArrayElementAtIndex(i);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField($"Prerequisite for Choice {i + 1}: {typeOfPrerequisites.enumDisplayNames[typeOfPrerequisites.enumValueIndex]}");

            prerequisite.FindPropertyRelative("prerequisiteType").enumValueIndex = typeOfPrerequisites.enumValueIndex;

            if (typeOfPrerequisites.enumValueIndex == (int)EncounterPrerequisiteType.Normal)
            {
                EditorGUILayout.PropertyField(prerequisite.FindPropertyRelative("normalPrerequisite"), true);
            }
            else if (typeOfPrerequisites.enumValueIndex == (int)EncounterPrerequisiteType.Conditional)
            {
                EditorGUILayout.PropertyField(prerequisite.FindPropertyRelative("conditionalPrerequisite"), true);
            }
            else if (typeOfPrerequisites.enumValueIndex == (int)EncounterPrerequisiteType.Risky)
            {
                EditorGUILayout.PropertyField(prerequisite.FindPropertyRelative("riskyPrerequisite"), true);
            }
            else
            {
                EditorGUILayout.LabelField("No prerequisites required.");
            }

            EditorGUILayout.EndVertical();
        }

        so.ApplyModifiedProperties();
    }
}