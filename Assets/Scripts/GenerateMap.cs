using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject rootNode;
    [SerializeField] private GameObject endNode;
    [SerializeField] private int rows;
    [SerializeField] private float nodeSpacing = 1f;
    public List<GameObject> nodes;
    private readonly List<List<GameObject>> _levelNodes = new();
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
        nodes.Add(this.rootNode);
        GenerateGrid();
        ConnectNodes();
    }

    private void GenerateGrid()
    {
        // Create nodes and back connections
        for(int level = 1; level < rows; level++)
        {
            _levelNodes.Add(new List<GameObject>());
            int nodesInRow = Random.Range(1, 5);
            if (level == 1 || ( nodesInRow == 1 && Random.Range(0, 10) > 3) ) nodesInRow = 2;

            float levelOffset = Random.Range(-nodeSpacing, nodeSpacing);
            for(int rowIndex = 0; rowIndex < nodesInRow; rowIndex++)
            {
                var node = Instantiate(nodePrefab);
                var currentNode = node.GetComponent<Node>();

                currentNode.level = level;
                currentNode.name = currentNode.name + " " + level + "-" + (level - 1);

                _levelNodes[level].Add(node);
                nodes.Add(node);

                int previousLevelNodeCount = _levelNodes[level - 1].Count;
                Node previousNode = _levelNodes[level - 1][Random.Range(0, previousLevelNodeCount)].GetComponent<Node>();
                previousNode.children.Add(currentNode);
                currentNode.parent.Add(previousNode);
                

                node.transform.position = new Vector3(level * nodeSpacing, (rowIndex - ((nodesInRow-1f)/2f)) * nodeSpacing + levelOffset, 0);
            }
        }

        // Create missing forward connections
        GenerateConnections();
    }

    private void GenerateConnections()
    {
        foreach (var node in nodes)
        {
            Node currentNode = node.GetComponent<Node>();
            if (currentNode.level + 1 >= _levelNodes.Count)
            {
                Node nextNode = endNode.GetComponent<Node>();
                currentNode.children.Add(nextNode);
                nextNode.parent.Add(currentNode);
                continue;
            }

            int nextLevelNodeCount = _levelNodes[currentNode.level + 1].Count;

            if (currentNode.children.Count == 0)
            {
                Node nextNode = _levelNodes[currentNode.level + 1][Random.Range(0, nextLevelNodeCount)].GetComponent<Node>();
                currentNode.children.Add(nextNode);
                nextNode.parent.Add(currentNode);
            }
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

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = lineMaterial;
        line.startColor = Color.gray;
        line.endColor = Color.gray;

        line.positionCount = 2;
        line.SetPosition(0, node1.transform.position + lineOffset);
        line.SetPosition(1, node2.transform.position + lineOffset);
    }
}