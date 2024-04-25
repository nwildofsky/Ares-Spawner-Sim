using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "TestAgentData", menuName = "ScriptObj/AgentData")]
public class AgentData : ScriptableObject
{
    [Header("Type")]
    public AgentType type = AgentType.Bat;

    [Header("Custom Movement Values")]
    public bool overrideNavMeshAgentValues = true;
    [Tooltip("Maximum movement speed when following a path")]
    public float speed = 3.5f;
    [Tooltip("Maximum turning speed in (deg/s) while following a path")]
    public float angularSpeed = 120f;
    [Tooltip("The maximum acceleration of an agent as it follows a path, given in units / sec^2")]
    public float acceleration = 8f;
    [Tooltip("Stop within this distance from the target position")]
    public float stoppingDistance = 0f;
    [Tooltip("The agent will avoid overshooting the destination point by slowing down in time")]
    public bool autoBraking = true;
    [Header("Obstacle Avoidance")]
    [Tooltip("The minimum distance to keep clear between the center of this agent and any other agents or obstacles nearby")]
    public float radius = 0.4f;
    [Tooltip("The height of the agent for purposed of passing under obstacles")]
    public float height = 1f;
    [Tooltip("Higher quality avoidance reduces more the chance of agents overlapping but it is slower to compute than lower quality avoidance")]
    public UnityEngine.AI.ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    [Tooltip("A lower value implies higher importance. This agent will ignore all other agents for which this number is higher")]
    public int priority = 50;

    [Header("Delays in Seconds")]
    public float startupCollisionDelay = 0.5f;
    public float exitTriggerCollisionDelay = 2f;

    [Header("Collision Spawn")]
    public float spawnRadius = 5f;
}
