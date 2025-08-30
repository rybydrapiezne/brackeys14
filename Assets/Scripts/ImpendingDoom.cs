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

    [NonSerialized] public int totalLevels;

    [SerializeField] private Slider doomSlider;
    [SerializeField] private Slider playerSlider;

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

    public IEnumerator UpdateElements(int doomLevel, int playerLevel, float transitionSpeed)
    {
        //Debug.Log("doom " + doomLevel + " player " + playerLevel + " total " + totalLevels);
        float playerStartPosition = playerSlider.value;
        float playerTargetPosition = playerLevel / (float)totalLevels;

        float doomStartPosition = doomSlider.value;
        float doomTargetPosition = doomLevel / (float)totalLevels;

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
