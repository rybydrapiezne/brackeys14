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

    private int levelsCheckCount = 8;
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
                foreach (var child in nodeComponent.children)
                {
                    Color materialColor = child.node.sprite.color;
                    materialColor.a = fadedPathAlpha;
                    child.node.sprite.color = materialColor;

                    materialColor = child.material.color;
                    materialColor.a = fadedPathAlpha;
                    child.material.color = materialColor;
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
                nextNode = currentNodeNode.children[0].node.transform.position.y >
                           currentNodeNode.children[1].node.transform.position.y
                    ? currentNodeNode.children[0].node.gameObject
                    : currentNodeNode.children[1].node.gameObject;
                nodeSelected = true;
            }
            else
            {
                nextNode = currentNodeNode.children[0].node.gameObject;
                nodeSelected = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentNodeNode.children.Count > 1)
            {
                nextNode = currentNodeNode.children[0].node.transform.position.y <
                           currentNodeNode.children[1].node.transform.position.y
                    ? currentNodeNode.children[0].node.gameObject
                    : currentNodeNode.children[1].node.gameObject;
                nodeSelected = true;
            }
            else
            {
                nextNode = currentNodeNode.children[0].node.gameObject;
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
        Queue<PathDestination> nodesToProcess = new Queue<PathDestination>(currentNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            PathDestination currentNode = nodesToProcess.Dequeue();
            if (currentNode.node.level > nextNodeComponent.level + levelsCheckCount)
                break;

            Color materialColor = currentNode.node.sprite.color;
            materialColor.a = fadedPathAlpha;
            currentNode.node.sprite.color = materialColor;

            materialColor = currentNode.material.color;
            materialColor.a = fadedPathAlpha;
            currentNode.material.color = materialColor;
            

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
            PathDestination pd = currentNodeComponent.children.Find(c => Object.Equals(c.node.gameObject, nextNodeComponent.gameObject));

            Color materialColor = pd.node.sprite.color;
            materialColor.a = 1;
            pd.node.sprite.color = materialColor;

            materialColor = pd.material.color;
            materialColor.a = 1;
            pd.material.color = materialColor;
        }

        // Show all possible paths
        nodesToProcess = new Queue<PathDestination>(nextNodeComponent.children);

        while (true)
        {
            if (nodesToProcess.Count == 0) break;

            PathDestination currentNode = nodesToProcess.Dequeue();
            if (currentNode.node.level >= nextNodeComponent.level + levelsCheckCount)
                break;

            Color materialColor = currentNode.node.sprite.color;
            materialColor.a = 1;
            currentNode.node.sprite.color = materialColor;

            materialColor = currentNode.material.color;
            materialColor.a = 1;
            currentNode.material.color = materialColor;
            

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