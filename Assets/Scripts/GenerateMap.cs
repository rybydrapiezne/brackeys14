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
    [SerializeField] private float ymin;
    [SerializeField] private float ymax;
    [SerializeField] private int splits;
    public List<GameObject> Nodes;
    Queue<Node> queue=new Queue<Node>();
    private List<List<GameObject>> _levelNodes=new List<List<GameObject>>();


    void Start()
    {
        
        Nodes = new List<GameObject>();
        Node rootNode=RootNode.GetComponent<Node>();
        rootNode.parent=null;
        rootNode.level=0;
        _levelNodes.Add(new List<GameObject>());
        _levelNodes[0].Add(RootNode);
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
        int currentSplits = 0;
        while (queue.Count > 0)
        {
            Node currentParentNode = queue.Dequeue();
            int paths;
            if (currentParentNode.level == rows)
            {
                currentParentNode.children.Add(EndNode.GetComponent<Node>());
                continue;
            }
            int probability = Random.Range(0, 30);
            if (probability <= 2 && currentSplits < splits)
            {
                paths = 2;
                currentSplits++;
            }
            else
            {
                paths = 1;
            }
            if (queue.Count == 0)
            {
                paths = 2;
            }
            float step=(currentParentNode.ymax-currentParentNode.ymin)/paths;
            for (int j = 0; j < paths; j++)
            {

                GameObject node = Instantiate(NodePrefab);
                Node currentNode = node.GetComponent<Node>();
                if (_levelNodes.Count-1<currentParentNode.level+1)
                {
                    _levelNodes.Add(new List<GameObject>());
                }
                currentNode.level = currentParentNode.level+1;
                _levelNodes[currentParentNode.level+1].Add(node);
                currentNode.name=currentNode.name+" "+currentNode.level+"-"+currentParentNode.level;
                node.transform.position = new Vector3(currentParentNode.transform.position.x+nodeSpacing, 0, 0);
                currentNode.parent.Add(currentParentNode);
                currentParentNode.children.Add(currentNode);
                Nodes.Add(node);
                queue.Enqueue(currentNode);
            }
        }

        for(int i=0;i<_levelNodes.Count;i++)
        {
            float currYmin=ymin*_levelNodes[i].Count/2;
            float currYmax=ymax*_levelNodes[i].Count/2;
            float step=(currYmax-currYmin)/_levelNodes[i].Count;

            for (int j = 0; j < _levelNodes[i].Count; j++)
            {
                Node currNode=_levelNodes[i][j].GetComponent<Node>();
                currNode.ymin=currYmin+j*step;
                currNode.ymax=currYmin+(j+1)*step;
                Vector3 nodePos=new Vector3(_levelNodes[i][j].transform.position.x,(currNode.ymax+currNode.ymin)/2,0);
                _levelNodes[i][j].transform.position=nodePos;
                
            }
        }
        GenerateConnections();
    }

    void GenerateConnections()
    {
        for(int i=0;i<_levelNodes.Count-1;i++)
        {
            for (int j = 0; j < _levelNodes[i].Count; j++)
            {
                if (_levelNodes[i][j].GetComponent<Node>().children.Count == 1 &&  _levelNodes[i][j].GetComponent<Node>().parent.Count == 1)
                {
                    int probability = Random.Range(0, 40);
                    if (probability <= 2)
                    {
                        if (j != 0 && j != _levelNodes[i].Count - 1)
                        {
                            var direction = Random.Range(0, 2);
                            if (direction == 0)
                            {
                                _levelNodes[i][j].GetComponent<Node>().children.Add(_levelNodes[i + 1][j + 1].GetComponent<Node>());
                                _levelNodes[i+1][j+1].GetComponent<Node>().parent.Add(_levelNodes[i][j].GetComponent<Node>());
                            }
                            else
                            {
                                _levelNodes[i][j].GetComponent<Node>().children.Add(_levelNodes[i - 1][j - 1].GetComponent<Node>());
                                _levelNodes[i-1][j-1].GetComponent<Node>().parent.Add(_levelNodes[i][j].GetComponent<Node>());
                            }
                        }
                        else
                        {
                            if (j==0)
                            {
                                _levelNodes[i][j].GetComponent<Node>().children.Add(_levelNodes[i + 1][j + 1].GetComponent<Node>());
                                _levelNodes[i+1][j+1].GetComponent<Node>().parent.Add(_levelNodes[i][j].GetComponent<Node>());
                            }
                            else
                            {
                                _levelNodes[i][j].GetComponent<Node>().children.Add(_levelNodes[i - 1][j - 1].GetComponent<Node>());
                                _levelNodes[i-1][j-1].GetComponent<Node>().parent.Add(_levelNodes[i][j].GetComponent<Node>());
                            }
                        }
                    }
                }
                
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