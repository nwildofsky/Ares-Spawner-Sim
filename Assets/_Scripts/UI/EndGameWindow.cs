using Michsky.MUIP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameWindow : MonoBehaviour
{
    public ModalWindowManager window;

    private void OnEnable()
    {
        EventManager.Game.OnGameEnd += EndGameActions;
    }

    private void OnDisable()
    {
        EventManager.Game.OnGameEnd -= EndGameActions;
    }

    private void EndGameActions()
    {
        window.Open();
        EventManager.UI.OnAgentCountHeard?.Invoke();
    }
}
