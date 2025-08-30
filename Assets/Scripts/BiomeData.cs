using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "BiomeData")]
public class BiomeData : ScriptableObject
{
    public BiomeType biomeName;
    public Sprite sprite;
    [TextArea(3, 10)]
    public string description;
    public float dangerMultiplier;

    [Space(2)]
    public int minimumLevelRequired = 0;
}
