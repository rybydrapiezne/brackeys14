using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>, IDictionary<Tuple<TKey1, TKey2>, TValue>
{

    public TValue this[TKey1 key1, TKey2 key2]
    {
        get { return base[Tuple.Create(key1, key2)]; }
        set { base[Tuple.Create(key1, key2)] = value; }
    }

    public void Add(TKey1 key1, TKey2 key2, TValue value)
    {
        base.Add(Tuple.Create(key1, key2), value);
    }

    public bool ContainsKey(TKey1 key1, TKey2 key2)
    {
        return base.ContainsKey(Tuple.Create(key1, key2));
    }
}

public class GenerateMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject rootNode;
    [SerializeField] private GameObject endNode;
    
    [Space(5)]
    [SerializeField] private Vector2Int totalBiomes = new Vector2Int(10, 3); // Total biomes to go through (total biomes = biomesHorizontally.x * biomesHorizontally.y)
    [SerializeField] private Vector2 biomeSpacing = Vector2.one;
    [SerializeField] private Vector2 nodeInBiomeSpacing = Vector2.one;
    [SerializeField] private int biomePathLength = 2; // Number of nodes horizontally in a biome
    
    [Space(2)]
    [SerializeField] private float nodeRandomOffset = 1f;

    [Space(5)]
    public List<GameObject> nodes;
    private readonly List<List<GameObject>> _levelNodes = new();
    private readonly Dictionary<int, int, Dictionary<int, int, GameObject>> biomes = new(); // Biome X,Y -> Node X,Y GameObject
    private Material lineMaterial;


    private void Start()
    {
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        nodes = new List<GameObject>();
        var rootNodeNode = rootNode.GetComponent<Node>();
        rootNodeNode.parent = null;
        rootNodeNode.level = 0;
        _levelNodes.Add(new List<GameObject>());
        _levelNodes[0].Add(this.rootNode);
        endNode.GetComponent<Node>().children = null;
        endNode.transform.position = new Vector3((totalBiomes.x + 1) * (biomeSpacing.x + nodeInBiomeSpacing.x), 0, 0);
        rootNode.transform.position = new Vector3((biomePathLength-1) * nodeInBiomeSpacing.x, 0, 0);
        nodes.Add(this.rootNode);

        GenerateBiomes();

        // Create connecting lines
        ConnectNodes();
    }

    private void GenerateBiomes()
    {
        // Create nodes and back connections
        for (int biomeX = 0; biomeX < totalBiomes.x; biomeX++)
        {
            int level = biomeX * biomePathLength;

            for (int nodesX = 0; nodesX < biomePathLength; nodesX++)
                _levelNodes.Add(new List<GameObject>());

            for (int biomeY = 0; biomeY < totalBiomes.y; biomeY++)
            {
                biomes[biomeX, biomeY] = new();
                GenerateGrid(biomeX, biomeY, level);
            }

            // Fix missing forward connections from previous biome, intersecting with other biomes
            foreach (var node in _levelNodes[level])
            {
                Node currentNode = node.GetComponent<Node>();
                if (currentNode.children.Count == 0)
                {
                    Node nextNode = _levelNodes[level + 1].GetRandom().GetComponent<Node>();
                    currentNode.children.Add(nextNode);
                    nextNode.parent.Add(currentNode);
                }
            }
        }

        // Connect last level nodes to endNode
        foreach (var node in _levelNodes.Last())
        {
            Node currentNode = node.GetComponent<Node>();
            if (currentNode.children.Count == 0)
            {
                Node nextNode = endNode.GetComponent<Node>();
                currentNode.children.Add(nextNode);
                nextNode.parent.Add(currentNode);
            }
        }
    }

    private void GenerateGrid(int biomeX, int biomeY, int outerLevel)
    {
        Vector2 biomeLocation = new Vector2(biomeX+1, biomeY - (totalBiomes.y - 1) / 2) * (biomeSpacing + nodeInBiomeSpacing);
        List<GameObject> previousBiomeRowNodes = new();

        for (int nodesX = 0; nodesX < biomePathLength; nodesX++)
        {
            int nodesInBiomeRow = (Random.value < 0.75f) ? 1 : 2; ; // 1 or 2, with greater probability of 1
            List<GameObject> currentBiomeRowNodes = new();

            for (int rowIndex = 0; rowIndex < nodesInBiomeRow; rowIndex++)
            {
                int innerLevel = outerLevel + nodesX + 1;
                var node = Instantiate(nodePrefab);
                var currentNode = node.GetComponent<Node>();

                currentNode.level = innerLevel;
                currentNode.name = "Node " + biomeX + "," + biomeY + "-" + nodesX + ", " + rowIndex + "-" + innerLevel;

                _levelNodes[innerLevel].Add(node);
                biomes[biomeX, biomeY][nodesX, rowIndex] = node;
                nodes.Add(node);
                currentBiomeRowNodes.Add(node);

                Node previousNode; // From all previous nodes if first in biome, or only biome ones if further

                if (nodesX == 0)
                    previousNode = _levelNodes[innerLevel - 1].GetRandom().GetComponent<Node>();
                else
                    previousNode = previousBiomeRowNodes.GetRandom().GetComponent<Node>();

                previousNode.children.Add(currentNode);
                currentNode.parent.Add(previousNode);


                node.transform.position = biomeLocation + (new Vector3(nodesX, rowIndex - (nodesInBiomeRow - 1) / 2f, 0) * nodeInBiomeSpacing) + new Vector2(Random.Range(-nodeRandomOffset, nodeRandomOffset), Random.Range(-nodeRandomOffset, nodeRandomOffset));
            }

            // Fix missing forward connections from previous level, biome-wise
            if (nodesX < biomePathLength) {
                foreach (var previousNodeGO in previousBiomeRowNodes)
                {
                    Node previousNode = previousNodeGO.GetComponent<Node>();
                    if (previousNode.children.Count == 0)
                    {
                        Node currentNode = currentBiomeRowNodes.GetRandom().GetComponent<Node>();
                        previousNode.children.Add(currentNode);
                        currentNode.parent.Add(previousNode);
                    }
                }
            }

            previousBiomeRowNodes = currentBiomeRowNodes;
        }
    }

    private void ConnectNodes()
    {
        foreach (var node in nodes)
        {
            var currNode = node.GetComponent<Node>();
            if (currNode.children != null)
                foreach (var child in currNode.children)
                    DrawConnection(node, child.gameObject);
        }
    }

    private Vector3 lineOffset = new Vector3(0, 0, 1);

    private void DrawConnection(GameObject node1, GameObject node2)
    {
        var lineObject = new GameObject($"Line_{node1.name}_to_{node2.name}");
        var line = lineObject.AddComponent<LineRenderer>();

        line.startWidth = 0.2f;
        line.endWidth = 0.2f;
        line.material = lineMaterial;
        line.startColor = Color.gray;
        line.endColor = Color.gray;

        line.positionCount = 2;
        line.SetPosition(0, node1.transform.position + lineOffset);
        line.SetPosition(1, node2.transform.position + lineOffset);
    }
}