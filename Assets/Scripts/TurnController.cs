using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceSystem;
public class TurnController : MonoBehaviour
{
    [SerializeField] private GameObject currentNode;
    [SerializeField] private Camera cam;
    [SerializeField] private float transitionSpeed = 2f;
    [SerializeField] private int suppliesTraverseCost;
    [SerializeField] private List<EncounterData> encounters;
    [SerializeField] private GameObject playerCaravan;
    private Node currentNodeNode;
    private bool isMoving;
    private bool encounterOngoing = false;
    [SerializeField] private AnimationCurve camMoveCurve;

    private int levelsCheckCount = 5;
    [SerializeField] private float fadedPathAlpha = 0.3f;

    private void Start()
    {
        NodeEncounterController.onEncounterStart += EncounterStarted;
        NodeEncounterController.onEncounterEnd += EncounterEnded;
        currentNodeNode = currentNode.GetComponent<Node>();
        cam.transform.position = new Vector3(
            currentNode.transform.position.x,
            currentNode.transform.position.y,
            cam.transform.position.z
        );
        playerCaravan.transform.position=currentNode.transform.position;

        foreach (var node in GenerateMap.Instance.nodes)
        {
            Node nodeComponent = node.GetComponent<Node>();
            if(nodeComponent.level >  levelsCheckCount){
                foreach (var material in nodeComponent.material)
                {
                    Color materialColor = material.color;
                    materialColor.a = fadedPathAlpha;
                    material.color = materialColor;
                }
            }
        }
    }

    private void Update()
    {
        if (!isMoving&&!encounterOngoing) SelectPath();
    }

    private void EncounterStarted()
    {
        encounterOngoing = true;
    }

    private void EncounterEnded()
    {
        encounterOngoing = false;
    }
    private void SelectPath()
    {
        var nextNode = currentNode;
        var nodeSelected = false;

        // Path choosing
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentNodeNode.children.Count > 1)
            {
                nextNode = currentNodeNode.children[0].transform.position.y >
                           currentNodeNode.children[1].transform.position.y
                    ? currentNodeNode.children[0].gameObject
                    : currentNodeNode.children[1].gameObject;
                nodeSelected = true;
            }
            else
            {
                nextNode = currentNodeNode.children[0].gameObject;
                nodeSelected = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentNodeNode.children.Count > 1)
            {
                nextNode = currentNodeNode.children[0].transform.position.y <
                           currentNodeNode.children[1].transform.position.y
                    ? currentNodeNode.children[0].gameObject
                    : currentNodeNode.children[1].gameObject;
                nodeSelected = true;
            }
            else
            {
                nextNode = currentNodeNode.children[0].gameObject;
                nodeSelected = true;
            }
        }

        if (nodeSelected && nextNode != currentNode)
        {
            // Fading unaccessable paths
            Node nextNodeComponent = nextNode.GetComponent<Node>();
            if (nextNodeComponent.children.Count > 0)
                fadeUnavailablePaths(currentNodeNode, nextNodeComponent);
            

            // Starting traverse animation
            StartCoroutine(TraverseToNextNode(nextNode));
        }
    }

    private void fadeUnavailablePaths(Node currentNodeComponent, Node nextNodeComponent)
    {
        // Hide unavailable paths
        Queue<Node> nodesToProcess = new Queue<Node>(currentNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            Node currentNode = nodesToProcess.Dequeue();
            if (currentNode.level >= nextNodeComponent.level + levelsCheckCount)
                break;

            foreach (var material in currentNode.material)
            {
                Color materialColor = material.color;
                materialColor.a = fadedPathAlpha;
                material.color = materialColor;
            }

            if (currentNode.children != null)
            {
                foreach (var child in currentNode.children)
                {
                    nodesToProcess.Enqueue(child);
                }
            }
        }

        // Show all possible paths
        foreach (var material in nextNodeComponent.material)
        {
            // currently cannot determine target, rewrite required
            Color materialColor = material.color;
            materialColor.a = 1;
            material.color = materialColor;
        }

        nodesToProcess = new Queue<Node>(nextNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            Node currentNode = nodesToProcess.Dequeue();
            if (currentNode.level >= nextNodeComponent.level + levelsCheckCount)
                break;

            foreach (var material in currentNode.material)
            {
                Color materialColor = material.color;
                materialColor.a = 1;
                material.color = materialColor;
            }

            if (currentNode.children != null)
            {
                foreach (var child in currentNode.children)
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
            cam.transform.position.z
        );
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
    }
}