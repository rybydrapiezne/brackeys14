using EasyTextEffects;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ResourceSystem;

public class GameEnder : MonoBehaviour
{
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private TextMeshProUGUI endGameDescription;
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
            resourceSystemReset(defaultResources);

            AudioManager.Instance.changeMusic(AudioManager.Instance.desertTheme, 1f);

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
        endGameDescription.text = descriptionPositive;
        endGameTitle.text = titlePositive;
        endGameTitle.GetComponent<TextEffect>().Refresh();
        endGamePanel.SetActive(true);
        AudioManager.Instance.walk.Stop();
        gameEnded = true;
    }

    private void OutOfResource(object s, OnOutOfResourceArgs a)
    {
        if (a.Resource == ResourceType.People && !gameEnded)
        {
            endGameDescription.text = descriptionNegative;
            endGameTitle.text = titleNegative;
            endGameTitle.GetComponent<TextEffect>().Refresh();
            endGamePanel.SetActive(true);
            AudioManager.Instance.walk.Stop();
            gameEnded = true;
        }
    }
}
