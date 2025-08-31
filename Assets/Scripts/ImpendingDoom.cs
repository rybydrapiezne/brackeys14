using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ImpendingDoom : MonoBehaviour
{
    private static ImpendingDoom instance;

    public static ImpendingDoom Instance
    {
        get
        {
            return instance;
        }
    }

    public int doomLevel = -4; // "It starts 4 turns after the start of your journey"

    [NonSerialized] public int totalLevels;

    [SerializeField] private Slider doomSlider;
    [SerializeField] private Slider playerSlider;

    [SerializeField] float transitionSpeed = 1f;

    [SerializeField] private AnimationCurve moveCurve;

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
    }

    public void setTotalLevels(int levels)
    {
        totalLevels = levels;
    }

    public void Refresh(int playerLevel)
    {
        if (playerLevel < doomLevel)
        {
            
        }
        StartCoroutine(UpdateElements(playerLevel));
    }

    private IEnumerator UpdateElements(int playerLevel)
    {
        //Debug.Log("doom " + doomLevel + " player " + playerLevel + " total " + totalLevels);
        float playerStartPosition = playerSlider.value;
        float playerTargetPosition = playerLevel / (float)totalLevels;

        float doomStartPosition = doomSlider.value;
        float doomTargetPosition = Mathf.Max(doomLevel, 0) / (float)totalLevels;

        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            playerSlider.value = Mathf.Lerp(playerStartPosition, playerTargetPosition, moveCurve.Evaluate(elapsedTime));
            doomSlider.value = Mathf.Lerp(doomStartPosition, doomTargetPosition, moveCurve.Evaluate(elapsedTime));
            yield return null;
        }

        playerSlider.value = playerTargetPosition;
        doomSlider.value = doomTargetPosition;
    }
}
