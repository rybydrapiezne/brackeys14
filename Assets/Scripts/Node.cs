using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> parent = new List<Node>();
    public List<PathDestination> children = new List<PathDestination>();
    public int level;
    public Biome biome;
    public SpriteRenderer sprite;
}
