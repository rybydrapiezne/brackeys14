using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateMap : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject NodePrefab;
    [SerializeField] private GameObject RootNode;
    [SerializeField] private GameObject EndNode;
    [SerializeField] private int rows;
    [SerializeField] private float nodeSpacing = 1f;
    [SerializeField] private int ymin;
    [SerializeField] private float ymax;
    public List<GameObject> Nodes;
    //private Node _currentParentNode;
    Queue<Node> queue=new Queue<Node>();


    void Start()
    {
        Nodes = new List<GameObject>();
        RootNode.GetComponent<Node>().parent=null;
        RootNode.GetComponent<Node>().level=0;
        RootNode.GetComponent<Node>().ymin = ymin;
        RootNode.GetComponent<Node>().ymax = ymax;
        EndNode.GetComponent<Node>().children=null;
        queue.Enqueue(RootNode.GetComponent<Node>());
        Nodes.Add(RootNode);
        GenerateGrid();
        ConnectNodes();
    }

    void GenerateGrid()
    {
        while (queue.Count > 0)
        {
            Node currentParentNode = queue.Dequeue();
            if (currentParentNode.level == rows)
            {
                currentParentNode.children.Add(EndNode.GetComponent<Node>());
                continue;
            }
            int paths = Random.Range(1, 3);
            if (queue.Count == 0)
            {
                paths = 2;
            }
            float step=(currentParentNode.ymax-currentParentNode.ymin)/paths;
            for (int j = 0; j < paths; j++)
            {
                GameObject node = Instantiate(NodePrefab);
                node.GetComponent<Node>().level = currentParentNode.level+1;
                node.GetComponent<Node>().ymin = currentParentNode.ymin+j*step;
                node.GetComponent<Node>().ymax = currentParentNode.ymin+(j+1)*step;
                Vector3 nodePos=new Vector3(currentParentNode.transform.position.x+nodeSpacing,(node.GetComponent<Node>().ymax+node.GetComponent<Node>().ymin)/2,0);
                node.transform.position = nodePos;
                node.GetComponent<Node>().parent = currentParentNode;
                currentParentNode.children.Add(node.GetComponent<Node>());
                Nodes.Add(node);
                queue.Enqueue(node.GetComponent<Node>());
            }
        }
    }

    void ConnectNodes()
    {
        foreach (GameObject node in Nodes)
        {
            Node currNode = node.GetComponent<Node>();
            if (currNode.children != null)
            {
                foreach (Node child in currNode.children)
                {
                    DrawConnection(node, child.gameObject);
                }
            }
        }
    }

    void DrawConnection(GameObject node1, GameObject node2)
    {
        GameObject lineObject = new GameObject($"Line_{node1.name}_to_{node2.name}");
        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.white;
        line.endColor = Color.white;
        
        line.positionCount = 2;
        line.SetPosition(0, node1.transform.position);
        line.SetPosition(1, node2.transform.position);
    }

    void Update()
    {
        // Optional: Add logic for dynamic updates (e.g., pathfinding visualization)
    }
}