using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject rootNode;
    [SerializeField] private GameObject endNode;
    [SerializeField] private int rows;
    [SerializeField] private float nodeSpacing = 1f;
    [SerializeField] private float ymin;
    [SerializeField] private float ymax;
    [SerializeField] private int splits;
    public List<GameObject> nodes;
    private readonly Queue<Node> _queue = new();
    private readonly List<List<GameObject>> _levelNodes = new();


    private void Start()
    {
        nodes = new List<GameObject>();
        var rootNodeNode = rootNode.GetComponent<Node>();
        rootNodeNode.parent = null;
        rootNodeNode.level = 0;
        _levelNodes.Add(new List<GameObject>());
        _levelNodes[0].Add(this.rootNode);
        rootNodeNode.ymin = ymin;
        rootNodeNode.ymax = ymax;
        endNode.GetComponent<Node>().children = null;
        _queue.Enqueue(rootNodeNode);
        nodes.Add(this.rootNode);
        GenerateGrid();
        ConnectNodes();
    }

    private void GenerateGrid()
    {
        var currentSplits = 0;
        while (_queue.Count > 0)
        {
            var currentParentNode = _queue.Dequeue();
            int paths;
            if (currentParentNode.level == rows)
            {
                currentParentNode.children.Add(endNode.GetComponent<Node>());
                continue;
            }

            var probability = Random.Range(0, 30);
            if (probability <= 2 && currentSplits < splits)
            {
                paths = 2;
                currentSplits++;
            }
            else
            {
                paths = 1;
            }

            if (_queue.Count == 0) paths = 2;
            var step = (currentParentNode.ymax - currentParentNode.ymin) / paths;
            for (var j = 0; j < paths; j++)
            {
                var node = Instantiate(nodePrefab);
                var currentNode = node.GetComponent<Node>();
                if (_levelNodes.Count - 1 < currentParentNode.level + 1) _levelNodes.Add(new List<GameObject>());
                currentNode.level = currentParentNode.level + 1;
                _levelNodes[currentParentNode.level + 1].Add(node);
                currentNode.name = currentNode.name + " " + currentNode.level + "-" + currentParentNode.level;
                node.transform.position = new Vector3(currentParentNode.transform.position.x + nodeSpacing, 0, 0);
                currentNode.parent.Add(currentParentNode);
                currentParentNode.children.Add(currentNode);
                nodes.Add(node);
                _queue.Enqueue(currentNode);
            }
        }

        for (var i = 0; i < _levelNodes.Count; i++)
        {
            var currYmin = ymin * _levelNodes[i].Count / 2;
            var currYmax = ymax * _levelNodes[i].Count / 2;
            var step = (currYmax - currYmin) / _levelNodes[i].Count;

            for (var j = 0; j < _levelNodes[i].Count; j++)
            {
                var currNode = _levelNodes[i][j].GetComponent<Node>();
                currNode.ymin = currYmin + j * step;
                currNode.ymax = currYmin + (j + 1) * step;
                var nodePos = new Vector3(_levelNodes[i][j].transform.position.x, (currNode.ymax + currNode.ymin) / 2,
                    0);
                _levelNodes[i][j].transform.position = nodePos;
            }
        }

        GenerateConnections();
    }

    private void GenerateConnections()
    {
        for (var i = 0; i < _levelNodes.Count - 1; i++)
        for (var j = 0; j < _levelNodes[i].Count; j++)
            if (_levelNodes[i][j].GetComponent<Node>().children.Count == 1 &&
                _levelNodes[i][j].GetComponent<Node>().parent.Count == 1)
            {
                var probability = Random.Range(0, 40);
                if (probability <= 2)
                {
                    if (j != 0 && j != _levelNodes[i].Count - 1)
                    {
                        var direction = Random.Range(0, 2);
                        if (direction == 0)
                        {
                            _levelNodes[i][j].GetComponent<Node>().children
                                .Add(_levelNodes[i + 1][j + 1].GetComponent<Node>());
                            _levelNodes[i + 1][j + 1].GetComponent<Node>().parent
                                .Add(_levelNodes[i][j].GetComponent<Node>());
                        }
                        else
                        {
                            _levelNodes[i][j].GetComponent<Node>().children
                                .Add(_levelNodes[i + 1][j - 1].GetComponent<Node>());
                            _levelNodes[i + 1][j - 1].GetComponent<Node>().parent
                                .Add(_levelNodes[i][j].GetComponent<Node>());
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            _levelNodes[i][j].GetComponent<Node>().children
                                .Add(_levelNodes[i + 1][j + 1].GetComponent<Node>());
                            _levelNodes[i + 1][j + 1].GetComponent<Node>().parent
                                .Add(_levelNodes[i][j].GetComponent<Node>());
                        }
                        else
                        {
                            _levelNodes[i][j].GetComponent<Node>().children
                                .Add(_levelNodes[i + 1][j - 1].GetComponent<Node>());
                            _levelNodes[i + 1][j - 1].GetComponent<Node>().parent
                                .Add(_levelNodes[i][j].GetComponent<Node>());
                        }
                    }
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

    private void DrawConnection(GameObject node1, GameObject node2)
    {
        var lineObject = new GameObject($"Line_{node1.name}_to_{node2.name}");
        var line = lineObject.AddComponent<LineRenderer>();

        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;

        line.positionCount = 2;
        line.SetPosition(0, node1.transform.position);
        line.SetPosition(1, node2.transform.position);
    }
}