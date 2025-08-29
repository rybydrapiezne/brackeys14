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

public static class BetterCollections
{
    #region GetRandom

    /// <summary>
    /// Returns random element from collection
    /// </summary>
    public static T GetRandom<T>(this T[] collection) => collection[UnityEngine.Random.Range(0, collection.Length)];

    /// <summary>
    /// Returns random element from collection
    /// </summary>
    public static T GetRandom<T>(this IList<T> collection) => collection[UnityEngine.Random.Range(0, collection.Count)];

    /// <summary>
    /// Returns random element from collection
    /// </summary>
    public static T GetRandom<T>(this IEnumerable<T> collection) => collection.ElementAt(UnityEngine.Random.Range(0, collection.Count()));

    #endregion
}

public class GenerateMap : MonoBehaviour
{
    private static GenerateMap instance;
    public static GenerateMap Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject rootNode;
    [SerializeField] private GameObject endNode;
    [SerializeField] private GameObject lineSegmentPrefab;
    [SerializeField] private Material baseMaterial;
    [SerializeField] private float lineSegmentSpacing = 10f; // Custom spacing between line segment prefabs
    [SerializeField] private float lineSegmentRandomOffset = 2f; // Random offset for line segments
    [Space(5)]
    [SerializeField] private Vector2Int totalBiomes = new Vector2Int(10, 3); // Total biomes to go through (total biomes = biomesHorizontally.x * biomesHorizontally.y)
    [SerializeField] private Vector2 biomeSpacing = new Vector2(163, 99);
    [SerializeField] private Vector2 nodeInBiomeSpacing = new Vector2(70, 40);
    [SerializeField] private int biomePathLength = 2; // Number of nodes horizontally in a biome

    [Space(2)]
    [SerializeField] private float biomeSpriteHorizontalOffset = 196f;

    [Space(2)]
    [SerializeField] private float nodeRandomOffset = 1f;

    [Space(5)]
    public List<GameObject> nodes;
    private readonly List<List<GameObject>> _levelNodes = new();

    [Space(5)]
    public GameObject biomePrefab;
    public List<BiomeData> biomes;

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;

        nodes = new List<GameObject>();
        var rootNodeNode = rootNode.GetComponent<Node>();
        rootNodeNode.parent = null;
        rootNodeNode.level = 0;
        _levelNodes.Add(new List<GameObject>());
        _levelNodes[0].Add(this.rootNode);
        endNode.GetComponent<Node>().children = null;
        endNode.transform.position = new Vector3((totalBiomes.x + 1) * biomeSpacing.x, 0, 0);
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

            List<BiomeData> availableBiomes = biomes.FindAll(b => level >= b.minimumLevelRequired);

            for (int nodesX = 0; nodesX < biomePathLength; nodesX++)
                _levelNodes.Add(new List<GameObject>());

            for (int biomeY = 0; biomeY < totalBiomes.y; biomeY++)
            {
                // Create biome
                GameObject biome = Instantiate(biomePrefab);
                biome.transform.position = (Vector3)(new Vector2(biomeX, biomeY - ((totalBiomes.y-1)/2f)) * biomeSpacing) + new Vector3(biomeSpriteHorizontalOffset, 0, 100);
                Biome biomeComponent = biome.GetComponent<Biome>();

                BiomeData pickedBiome = availableBiomes.GetRandom();

                biome.GetComponent<SpriteRenderer>().sprite = pickedBiome.sprite;
                biomeComponent.biomeName = pickedBiome.biomeName;
                biomeComponent.sprite = pickedBiome.sprite;
                biomeComponent.minimumLevelRequired = pickedBiome.minimumLevelRequired;

                // Create nodes for biome
                GenerateGrid(biomeX, biomeY, level, biomeComponent);
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

    private void GenerateGrid(int biomeX, int biomeY, int outerLevel, Biome currentBiome)
    {
        Vector2 biomeLocation = new Vector2(biomeX+1, biomeY - (totalBiomes.y - 1) / 2) * biomeSpacing;
        List<GameObject> previousBiomeRowNodes = new();

        for (int nodesX = 0; nodesX < biomePathLength; nodesX++)
        {
            int nodesInBiomeRow = (Random.value < 0.75f) ? 1 : 2; // 1 or 2, with greater probability of 1
            List<GameObject> currentBiomeRowNodes = new();

            for (int rowIndex = 0; rowIndex < nodesInBiomeRow; rowIndex++)
            {
                int innerLevel = outerLevel + nodesX + 1;
                var node = Instantiate(nodePrefab);
                var currentNode = node.GetComponent<Node>();

                currentNode.level = innerLevel;
                currentNode.name = "Node " + biomeX + "," + biomeY + "-" + nodesX + ", " + rowIndex + "-" + innerLevel;

                _levelNodes[innerLevel].Add(node);
                nodes.Add(node);
                currentBiomeRowNodes.Add(node);
                currentNode.biome = currentBiome;

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
            if (nodesX < biomePathLength)
            {
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
        Vector3 startPos = node1.transform.position + lineOffset;
        Vector3 endPos = node2.transform.position + lineOffset;
        Material lineMaterial = new Material(baseMaterial);
        node1.GetComponent<Node>().material.Add(lineMaterial);
        Vector3 direction = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);
        
        float segmentLength = lineSegmentSpacing;
        if (segmentLength <= 0 && lineSegmentPrefab.GetComponent<SpriteRenderer>())
        {
            segmentLength = lineSegmentPrefab.GetComponent<SpriteRenderer>().bounds.size.x;
        }
        segmentLength = Mathf.Max(segmentLength, 0.1f);
        int segmentCount = Mathf.Max(1, Mathf.FloorToInt(distance / segmentLength));

        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0);

        for (int i = 0; i < segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            Vector3 basePosition = Vector3.Lerp(startPos, endPos, t);
            float randomOffset = Random.Range(-lineSegmentRandomOffset, lineSegmentRandomOffset);
            Vector3 position = basePosition + perpendicular * randomOffset;
            GameObject segment = Instantiate(lineSegmentPrefab, position, Quaternion.identity, lineObject.transform);
            segment.GetComponent<SpriteRenderer>().material = lineMaterial;
            
        }
    }
}