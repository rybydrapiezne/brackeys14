using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> parent=new List<Node>();
    public List<Node> children=new List<Node>();
    public List<Material> material;
    public int level;
    public Biome biome;
}
