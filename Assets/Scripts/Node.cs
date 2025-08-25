using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node parent;
    public List<Node> children=new List<Node>();
    public int level;
    public float ymin;
    public float ymax;
}
