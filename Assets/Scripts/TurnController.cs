using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private FogOfWarManager fog;
    private Node currentNodeNode;
    private bool hasTakenContract = false;
    private bool isMoving;
    private bool encounterOngoing = false;
    [SerializeField] private AnimationCurve camMoveCurve;

    public int levelsCheckCount = 8;
    [SerializeField] public Color fadedPathColor = new Color(1f, 1f, 1f, 0.3f);
    public static event EventHandler OnLastNodeReached;

    private int doomLevel = -4; // "It starts 4 turns after the start of your journey"
    private List<LuckStatus> luckStasuses = new();

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
        playerCaravan.transform.position = currentNode.transform.position;

        foreach (var node in GenerateMap.Instance.nodes)
        {
            Node nodeComponent = node.GetComponent<Node>();
            if (nodeComponent.level > levelsCheckCount)
            {
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
        if (isMoving || encounterOngoing || currentNodeNode.children.Find(c => Equals(c.node.gameObject, nextNode)) == null) return;

        // Fading unaccessable paths
        Node nextNodeComponent = nextNode.GetComponent<Node>();
        if (nextNodeComponent.children != null)
            fadeUnavailablePaths(currentNodeNode, nextNodeComponent);

        // Impending doom
        ImpendingDoom.Instance.doomLevel++;

        ImpendingDoom.Instance.Refresh(nextNodeComponent.level);

        // Move fog
        fog.MoveFog(currentNodeNode.level + fog.initialDepth + 1);

        for (int i = 0; i < luckStasuses.Count; i++)
        {
            LuckStatus temp = luckStasuses[i];
            temp.turnsLeft--;
            luckStasuses[i] = temp;
        }

        luckStasuses.RemoveAll(ls => ls.turnsLeft <= 0);

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
        addResource(ResourceType.Supplies, -suppliesTraverseCost);
        if (getResource(ResourceType.Supplies) == 0)
        {
            addResource(ResourceType.People, -1);
        }

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


        BiomeType currentBiome = currentNodeNode.biome.biomeName;

        
        traverseByBiome(nextNode, currentBiome);

        // Filter encounters by current biome
        var filteredEncounters = encounters.Where(e => (e.biome & currentBiome) != 0).ToList();
        int dangerAttraction = getDangerAttraction(currentNodeNode, luckStasuses);

        List<int> weights = new List<int>();
        foreach (var e in filteredEncounters)
        {
            int weight = Mathf.Max(1, 100 - Mathf.Abs(dangerAttraction - e.dangerLevel));
            weights.Add(weight);
        }

        int totalWeight = weights.Sum();
        int randomValue = Random.Range(0, totalWeight);
        int cumulative = 0;
        EncounterData currentEncounter = filteredEncounters[0];

        for (int i = 0; i < filteredEncounters.Count; i++)
        {
            cumulative += weights[i];
            if (randomValue < cumulative)
            {
                currentEncounter = filteredEncounters[i];
                break;
            }
        }

        while (playerCaravan.GetComponent<PlayerCaravanController>().isMoving)
        {
            yield return null;
        }
        currentNode.GetComponent<NodeEncounterController>().EnableEncounter(currentEncounter.choices.Length,
            currentEncounter.encounterImage, currentEncounter.description, currentEncounter.encounterName, currentEncounter.choices, currentEncounter.prerequisites);
        isMoving = false;
        if (currentNode.CompareTag("EndNode"))
            OnLastNodeReached?.Invoke(null, null);
    }

    private void traverseByBiome(GameObject nextNode, BiomeType biome)
    {

        if (biome.HasFlag(BiomeType.Desert))
             addResource( ResourceType.Supplies, (int)Amounts.NegativeSmallAmount);

        if (biome.HasFlag(BiomeType.Riverlands))
        {
             addResource( ResourceType.Supplies, (int)Amounts.PositiveVerySmallAmount);
             addResource( ResourceType.Gear, (int)Amounts.NegativeVerySmallAmount);
        }

        if (biome.HasFlag(BiomeType.Canyon))
             addResource( ResourceType.Gear, (int)Amounts.NegativeVerySmallAmount);

        if (biome.HasFlag(BiomeType.Plateau))
        {
            bool success =  addResource( ResourceType.Gear, (int)Amounts.NegativeSmallAmount);
            if (!success)
            {
                doomLevel++;
                Node nextNodeComponent = nextNode.GetComponent<Node>();
                ImpendingDoom.Instance.Refresh(nextNodeComponent.level);
            }
        }

        if (biome.HasFlag(BiomeType.Steppes)) { }

        if (biome.HasFlag(BiomeType.Dunes))
        {
             addResource( ResourceType.Supplies, (int)Amounts.NegativeVerySmallAmount);
             addResource( ResourceType.Gear, (int)Amounts.NegativeVerySmallAmount);
        }

        if (biome.HasFlag(BiomeType.WarlordsTerritory))
        {
            bool success =  addResource( ResourceType.Valuables, (int)Amounts.NegativeSmallAmount);
            if (!success)  addResource( ResourceType.People, -1);
        }

        if (biome.HasFlag(BiomeType.ScorchedLand))
             addResource( ResourceType.Supplies, (int)Amounts.NegativeMediumAmount);

        if (biome.HasFlag(BiomeType.Wastelands))
             addResource( ResourceType.Supplies, (int)Amounts.NegativeSmallAmount);
    }


}