using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCountListener : MonoBehaviour
{
    public int BatCount {  get; private set; }
    public int CrabCount { get; private set; }
    public int SalamanderCount { get; private set; }
    public int SpiderCount { get; private set; }
    public int Total {  get; private set; }
    public float BatCountAsPercentage { get => BatCount / (float)Total; }
    public float CrabCountAsPercentage { get => CrabCount / (float)Total; }
    public float SalamanderCountAsPercentage { get => SalamanderCount / (float)Total; }
    public float SpiderCountAsPercentage { get => SpiderCount / (float)Total; }

    private void OnEnable()
    {
        EventManager.UI.OnAgentCountChanged += AdjustCount;
    }

    private void OnDisable()
    {
        EventManager.UI.OnAgentCountChanged -= AdjustCount;
    }

    private void AdjustCount(AgentType type, int count)
    {
        switch (type)
        {
            case AgentType.Bat:
                Total += count - BatCount;
                BatCount = count;
                break;
            case AgentType.Crab:
                Total += count - CrabCount;
                CrabCount = count;
                break;
            case AgentType.Salamander:
                Total += count - SalamanderCount;
                SalamanderCount = count;
                break;
            case AgentType.Spider:
                Total += count - SpiderCount;
                SpiderCount = count;
                break;
            default: break;
        }

        EventManager.UI.OnAgentCountHeard?.Invoke();
    }
}
