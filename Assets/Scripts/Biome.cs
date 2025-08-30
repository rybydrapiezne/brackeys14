using UnityEngine;

public class Biome : MonoBehaviour
{
    public BiomeType biomeName;
    public Sprite sprite;
    public string description;

    [Space(2)]
    public int minimumLevelRequired = 0;
}
