using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ResourceSystem;

public class GameEnder : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI endGameText;
    [SerializeField] private TextMeshProUGUI endGameTitle;
    [TextArea(3, 10)]
    [SerializeField] private string descriptionPositive;
    [SerializeField] private string titlePositive;
    [TextArea(3, 10)]
    [SerializeField] private string descriptionNegative;
    [SerializeField] private string titleNegative;
    private void Start()
    {
        OnOutOfResource += OutOfResource;
        TurnController.OnLastNodeReached += LastNodeReached;

    }

    private void LastNodeReached(object sender, EventArgs e)
    {
        endGameText.text = descriptionPositive;
        endGameTitle.text = titlePositive;
        endGamePanel.SetActive(true);
        
    }

    private void OutOfResource(object s, OnOutOfResourceArgs a)
    {
        if (a.Resource == ResourceType.People)
        {
            endGameText.text = descriptionNegative;
            endGameTitle.text = titleNegative;
            endGamePanel.SetActive(true);
            
        }
    }
}
