using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceSystem;
using Random = UnityEngine.Random;

public class TurnController : MonoBehaviour
{
    private static TurnController instance;
    public static TurnController Instance
    {
        get
        {
            return instance;
        }
    }

    [SerializeField] private GameObject currentNode;
    [SerializeField] private Camera cam;
    [SerializeField] private Vector3 camOffset = new Vector3(100, 0, -10);
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private int suppliesTraverseCost;
    [SerializeField] private List<EncounterData> encounters;
    [SerializeField] private GameObject playerCaravan;
    private Node currentNodeNode;
    private bool isMoving;
    private bool encounterOngoing = false;
    [SerializeField] private AnimationCurve camMoveCurve;

    public int levelsCheckCount = 8;
    [SerializeField] public Color fadedPathColor = new Color(1f, 1f, 1f, 0.3f);
    public static event EventHandler OnLastNodeReached;

    private int doomLevel = -4; // "It starts 4 turns after the start of your journey"

    private void Awake()
    {
        if (instance)
        {
            DestroyImmediate(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        NodeEncounterController.onEncounterStart += EncounterStarted;
        NodeEncounterController.onEncounterEnd += EncounterEnded;
        currentNodeNode = currentNode.GetComponent<Node>();
        cam.transform.position = new Vector3(
            currentNode.transform.position.x,
            0,
            0
        ) + camOffset;
        playerCaravan.transform.position=currentNode.transform.position;

        foreach (var node in GenerateMap.Instance.nodes)
        {
            Node nodeComponent = node.GetComponent<Node>();
            if(nodeComponent.level >  levelsCheckCount){
                foreach (var child in nodeComponent.children)
                {
                    child.node.baseColor = fadedPathColor;

                    child.node.sprite.color = child.node.baseColor;
                    child.material.color = child.node.baseColor;
                }
            }
        }
    }

    private void Update()
    {
        
    }

    private void EncounterStarted()
    {
        encounterOngoing = true;
    }

    private void EncounterEnded()
    {
        encounterOngoing = false;
    }
    public void SelectPath(GameObject nextNode)
    {
        if (isMoving || encounterOngoing || currentNodeNode.children.Find(c => Equals(c.node.gameObject, nextNode)) == null ) return;
        
        // Fading unaccessable paths
        Node nextNodeComponent = nextNode.GetComponent<Node>();
        if (nextNodeComponent.children != null)
            fadeUnavailablePaths(currentNodeNode, nextNodeComponent);

        // Impending doom
        doomLevel++;

        StartCoroutine(ImpendingDoom.Instance.UpdateElements(Mathf.Max(doomLevel, 0), nextNodeComponent.level, transitionSpeed));

        // Starting traverse animation
        StartCoroutine(TraverseToNextNode(nextNode));
    }

    private void fadeUnavailablePaths(Node currentNodeComponent, Node nextNodeComponent)
    {
        // Hide unavailable paths
        Queue<PathDestination> nodesToProcess = new Queue<PathDestination>(currentNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            PathDestination currentNode = nodesToProcess.Dequeue();
            if (currentNode.node.level > nextNodeComponent.level + levelsCheckCount)
                break;

            currentNode.node.baseColor = fadedPathColor;

            currentNode.node.sprite.color = currentNode.node.baseColor;
            currentNode.material.color = currentNode.node.baseColor;
            

            if (currentNode.node.children != null)
            {
                foreach (var child in currentNode.node.children)
                {
                    nodesToProcess.Enqueue(child);
                }
            }
        }

        // Show selected path
        {
            PathDestination pd = currentNodeComponent.children.Find(c => Equals(c.node.gameObject, nextNodeComponent.gameObject));

            Color materialColor = pd.node.sprite.color;
            materialColor.a = 1;
            pd.node.baseColor = materialColor;

            pd.node.sprite.color = pd.node.baseColor;
            pd.material.color = pd.node.baseColor;
        }

        // Show all possible paths
        nodesToProcess = new Queue<PathDestination>(nextNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            PathDestination currentNode = nodesToProcess.Dequeue();
            if (currentNode.node.level >= nextNodeComponent.level + levelsCheckCount)
                break;

            currentNode.node.baseColor = Color.white;

            currentNode.node.sprite.color = currentNode.node.baseColor;
            currentNode.material.color = currentNode.node.baseColor;


            if (currentNode.node.children != null)
            {
                foreach (var child in currentNode.node.children)
                {
                    nodesToProcess.Enqueue(child);
                }
            }
        }
    }

    private IEnumerator TraverseToNextNode(GameObject nextNode)
    {
        isMoving = true;
        playerCaravan.GetComponent<PlayerCaravanController>().startCaravanMovement(nextNode.transform.position);
        addResource(ResourceType.Supplies,-suppliesTraverseCost);
        var startPosition = cam.transform.position;
        var targetPosition = new Vector3(
            nextNode.transform.position.x,
            0,
            0
        ) + camOffset;
        var elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, camMoveCurve.Evaluate(elapsedTime));
            yield return null;
        }

        cam.transform.position = targetPosition;

        currentNode = nextNode;

        currentNodeNode = currentNode.GetComponent<Node>();
        int encounter=Random.Range(0,encounters.Count);
        EncounterData currentEncounter = encounters[encounter];
        while (playerCaravan.GetComponent<PlayerCaravanController>().isMoving)
        {
            yield return null;
        }
        currentNode.GetComponent<NodeEncounterController>().EnableEncounter(currentEncounter.choices.Length,
            currentEncounter.encounterImage,currentEncounter.description,currentEncounter.encounterName,currentEncounter.choices,currentEncounter.prerequisites);
        isMoving = false;
        if(currentNode.CompareTag("EndNode"))
            OnLastNodeReached?.Invoke(null,null);
    }
}