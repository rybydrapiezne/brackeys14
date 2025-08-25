using System.Collections;
using UnityEngine;

public class TurnController : MonoBehaviour
{
    [SerializeField] private GameObject currentNode;
    [SerializeField] private Camera cam;
    [SerializeField] private float transitionSpeed = 2f;
    private Node currentNodeNode;
    private bool isMoving = false;

    void Start()
    {
        currentNodeNode = currentNode.GetComponent<Node>();
        cam.transform.position = new Vector3(
            currentNode.transform.position.x,
            currentNode.transform.position.y,
            cam.transform.position.z
        );
    }

    void Update()
    {
        if (!isMoving)
        {
            SelectPath();
        }
    }

    private void SelectPath()
    {
        GameObject nextNode = currentNode;
        bool nodeSelected = false;

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentNodeNode.children.Count > 1)
            {
                nextNode = currentNodeNode.children[0].transform.position.y > 
                           currentNodeNode.children[1].transform.position.y ?
                           currentNodeNode.children[0].gameObject : currentNodeNode.children[1].gameObject;
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
                           currentNodeNode.children[1].transform.position.y ?
                    currentNodeNode.children[0].gameObject : currentNodeNode.children[1].gameObject;
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
            StartCoroutine(TraverseToNextNode(nextNode));
        }
    }

    IEnumerator TraverseToNextNode(GameObject nextNode)
    {
        isMoving = true;
        Vector3 startPosition = cam.transform.position;
        Vector3 targetPosition = new Vector3(
            nextNode.transform.position.x,
            nextNode.transform.position.y,
            cam.transform.position.z
        );
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime * transitionSpeed;
            cam.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        cam.transform.position = targetPosition;
        
        currentNode = nextNode;
        currentNodeNode = currentNode.GetComponent<Node>();
        isMoving = false;
    }
}