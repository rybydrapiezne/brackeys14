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
    Queue<Node> queue=new Queue<Node>();


    void Start()
    {
        Nodes = new List<GameObject>();
        Node rootNode=RootNode.GetComponent<Node>();
        rootNode.parent=null;
        rootNode.level=0;
        rootNode.ymin = ymin;
        rootNode.ymax = ymax;
        EndNode.GetComponent<Node>().children=null;
        queue.Enqueue(rootNode);
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
                Node currentNode = node.GetComponent<Node>();
                currentNode.level = currentParentNode.level+1;
                currentNode.name=currentNode.name+" "+currentNode.level+"-"+currentParentNode.level;
                currentNode.ymin = currentParentNode.ymin+j*step;
                currentNode.ymax = currentParentNode.ymin+(j+1)*step;
                Vector3 nodePos=new Vector3(currentParentNode.transform.position.x+nodeSpacing,(currentNode.ymax+currentNode.ymin)/2,0);
                node.transform.position = nodePos;
                currentNode.parent = currentParentNode;
                currentParentNode.children.Add(currentNode);
                Nodes.Add(node);
                queue.Enqueue(currentNode);
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
    
}