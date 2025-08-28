using UnityEngine;

[CreateAssetMenu(fileName = "NewBiome", menuName = "BiomeData")]
public class BiomeData : ScriptableObject
{
    public string biomeName;
    public Sprite sprite;

    [Space(2)]
    public int minimumLevelRequired = 0;
}
