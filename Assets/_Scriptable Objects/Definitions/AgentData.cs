using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TestAgentData", menuName = "ScriptObj/AgentData")]
public class AgentData : ScriptableObject
{
    [Header("Type")]
    public AgentType type = AgentType.Agent1;

    [Header("Custom Movement Values")]
    public bool overrideNavMeshAgentValues = true;
    public float speed = 3.5f;
    public float angularSpeed = 120f;
    public float acceleration = 8f;
    public float stoppingDistance = 0f;
    public bool autoBraking = true;

    [Header("Delays in Seconds")]
    public float startupCollisionDelay = 0.5f;
    public float exitTriggerCollisionDelay = 2f;

    [Header("Collision Spawn")]
    public float spawnRadius = 5f;
}
