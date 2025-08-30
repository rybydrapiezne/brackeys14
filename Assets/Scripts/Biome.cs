using UnityEngine;

public class Biome : MonoBehaviour
{
    public BiomeType biomeName;
    public Sprite sprite;
    public string description;
    public float dangerMultiplier;

    [Space(2)]
    public int minimumLevelRequired = 0;
}
