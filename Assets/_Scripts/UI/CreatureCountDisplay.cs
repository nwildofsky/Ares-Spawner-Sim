using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCountDisplay : MonoBehaviour
{
    public CreatureCountListener data;
    public AgentType type;
    public RectTransform progressBar;
    public float minWidth;
    public float stablePercentWidth;
    public float maxWidth;

    private void Awake()
    {
        progressBar.sizeDelta = new Vector2(minWidth, progressBar.sizeDelta.y);
    }

    private void OnEnable()
    {
        EventManager.UI.OnAgentCountHeard += AdjustWidth;
    }

    private void OnDisable()
    {
        EventManager.UI.OnAgentCountHeard -= AdjustWidth;
    }

    private void AdjustWidth()
    {
        float percentage = 0f;

        switch (type)
        {
            case AgentType.Bat:
                percentage = data.BatCountAsPercentage;
                break;
            case AgentType.Crab:
                percentage = data.CrabCountAsPercentage;
                break;
            case AgentType.Salamander:
                percentage = data.SalamanderCountAsPercentage;
                break;
            case AgentType.Spider:
                percentage = data.SpiderCountAsPercentage;
                break;
            default: break;
        }

        if (percentage <= .25f)
        {
            progressBar.sizeDelta = new Vector2(Mathf.Lerp(minWidth, stablePercentWidth, percentage / 0.25f), progressBar.sizeDelta.y);
        }
        else if (percentage > 0.25f)
        {
            progressBar.sizeDelta = new Vector2(Mathf.Lerp(stablePercentWidth, maxWidth, percentage), progressBar.sizeDelta.y);
        }
    }
}
