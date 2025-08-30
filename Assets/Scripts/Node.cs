using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> parent = new List<Node>();
    public List<PathDestination> children = new List<PathDestination>();
    public int level;
    public Biome biome;
    public SpriteRenderer sprite;

    public Color baseColor = Color.white;
    public Color hoverColor = new Color(.5f, 1f, .5f);

    public List<PathDestination> futureDestinations = new();

    private void OnMouseUp()
    {
        TurnController.Instance.SelectPath(gameObject);
        AudioManager.Instance.click.Play();
    }

    void OnMouseEnter()
    {
        if (level > FogOfWarManager.Instance.currentLevel + 1) return;

        if(biome != null)
            OnHoverPopup.Instance.showPopup(biome.description);

        AudioManager.Instance.hover.Play();

        sprite.color = hoverColor;
        futureDestinations = new();

        if (children == null) return;

        Queue<PathDestination> queue = new Queue<PathDestination>(children);
        while (true)
        {
            if (queue.Count == 0) break;

            PathDestination currentNode = queue.Dequeue();
            if (currentNode.node.level >= level + TurnController.Instance.levelsCheckCount)
                break;

            futureDestinations.Add(currentNode);

            currentNode.node.sprite.color = hoverColor;
            currentNode.material.color = hoverColor;


            if (currentNode.node.children != null)
            {
                foreach (var child in currentNode.node.children)
                {
                    queue.Enqueue(child);
                }
            }
        }
    }

    void OnMouseExit()
    {
        OnHoverPopup.Instance.hidePopup();

        sprite.color = baseColor;

        foreach (var fd in futureDestinations)
        {
            fd.node.sprite.color = fd.node.baseColor;
            foreach (var parentNode in fd.node.parent)
            {
                foreach (var child in parentNode.children)
                {
                    if(parentNode.baseColor == child.node.baseColor)
                        child.material.color = parentNode.baseColor;
                    else
                        child.material.color = TurnController.Instance.fadedPathColor;
                }
            }
        }
    }
}
