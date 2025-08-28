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
    private Node currentNodeNode;
    private bool isMoving;
    private bool encounterOngoing = false;

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

        if (nodeSelected && nextNode != currentNode) StartCoroutine(TraverseToNextNode(nextNode));
    }

    private IEnumerator TraverseToNextNode(GameObject nextNode)
    {
        isMoving = true;
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
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        cam.transform.position = targetPosition;

        currentNode = nextNode;
        currentNodeNode = currentNode.GetComponent<Node>();
        int encounter=Random.Range(0,encounters.Count);
        EncounterData currentEncounter = encounters[encounter];
        currentNode.GetComponent<NodeEncounterController>().EnableEncounter(currentEncounter.choices.Length,
            currentEncounter.encounterImage,currentEncounter.description,currentEncounter.encounterName,currentEncounter.choices);
        isMoving = false;
    }
}