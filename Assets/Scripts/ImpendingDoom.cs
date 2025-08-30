using UnityEngine;

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

    public int totalLevels;

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

    public void UpdateElements(int doomLevel, int playerLevel)
    {
        Debug.Log("doom " + doomLevel + " player " + playerLevel + " total " + totalLevels);
        // progress (0;1)
        //doomLevel / totalLevels;
        //playerLevel / totalLevels;
    }
}
