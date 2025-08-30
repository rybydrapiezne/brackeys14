using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ResourceSystem;

public class GameEnder : MonoBehaviour
{

    private void Start()
    {
        OnOutOfResource += outOfResource;
        TurnController.OnLastNodeReached += lastNodeReached;

    }

    private void lastNodeReached(object sender, EventArgs e)
    {
        SceneManager.LoadSceneAsync("FinishedRunScene");
    }

    private void outOfResource(object s, OnOutOfResourceArgs a)
    {
        if (a.Resource == ResourceType.People)
        {
            SceneManager.LoadSceneAsync("FailedRunScene");//TODO change this to index
        }
    }
}
