using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
    public bool gameEnded = false;

    private void Start()
    {
        OnOutOfResource += OutOfResource;
        TurnController.OnLastNodeReached += LastNodeReached;

    }

    public void OnMainMenuButton()
    {
        if (gameEnded)
        {
            StartCoroutine(AudioManager.fadeIn(AudioManager.Instance.desertTheme, 1f));
            StartCoroutine(AudioManager.fadeOut(AudioManager.Instance.mainTheme, 1f));

            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void OnRestartButton()
    {
        if (gameEnded)
        {
            resourceSystemReset(defaultResources);

            SceneManager.LoadScene("MainScene");
        }
    }

    private void LastNodeReached(object sender, EventArgs e)
    {
        endGameText.text = descriptionPositive;
        endGameTitle.text = titlePositive;
        endGamePanel.SetActive(true);
        gameEnded = true;
    }

    private void OutOfResource(object s, OnOutOfResourceArgs a)
    {
        if (a.Resource == ResourceType.People)
        {
            endGameText.text = descriptionNegative;
            endGameTitle.text = titleNegative;
            endGamePanel.SetActive(true);
            gameEnded = true;
        }
    }
}
