using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "BiomeData")]
public class BiomeData : ScriptableObject
{
    public BiomeType biomeName;
    public Sprite sprite;
    public string description;

    [Space(2)]
    public int minimumLevelRequired = 0;
}
