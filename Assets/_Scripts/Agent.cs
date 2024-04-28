using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Agent : MonoBehaviour
{
    [SerializeField]
    private AgentData data;
    private NavMeshAgent navAgent;
    public DOTweenAnimation appearAnim;
    public AgentType Type { get => data.type; private set => data.type = value; }
    private Action<Agent> collideSpawnAction;
    private Action<Agent> collideDestroyAction;
    //private bool hasCollided = true;
    public bool ReadyToCollide { get; set; }

    public void SetActions(Action<Agent> spawnAction, Action<Agent> destroyAction)
    {
        collideSpawnAction = spawnAction;
        collideDestroyAction = destroyAction;
    }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (data.overrideNavMeshAgentValues)
        {
            navAgent.speed = data.speed;
            navAgent.angularSpeed = data.angularSpeed;
            navAgent.acceleration = data.acceleration;
            navAgent.stoppingDistance = data.stoppingDistance;
            navAgent.autoBraking = data.autoBraking;

            navAgent.radius = data.radius;
            navAgent.height = data.height;
            navAgent.obstacleAvoidanceType = data.obstacleAvoidanceType;
            navAgent.avoidancePriority = data.priority;
        }
    }

    private void Start()
    {
        // Initially activate collisions with a delay after instantiation
        Invoke(nameof(SetReadyToCollide), data.startupCollisionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Agent>(out Agent otherAgent))
        {
            // Collision of similar agents
            if (otherAgent.Type == Type)
            {
                if (ReadyToCollide && otherAgent.ReadyToCollide)
                {
                    otherAgent.ReadyToCollide = false;
                    ReadyToCollide = false;

                    // Set new destinations for after collision
                    ResetNavigation();
                    otherAgent.ResetNavigation();

                    // Spawn a new object of the same type
                    collideSpawnAction?.Invoke(this);
                }
            }
            // Collision of different agents
            else
            {
                // Stop movement
                StopNavigation();
                // Trigger death animation
                EventManager.Game.OnDeathCollision?.Invoke(navAgent);
                // Destroy both objects
                collideDestroyAction?.Invoke(this);
            }

            ReadyToCollide = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Activate collisions again with a delay after the last collision
        Invoke(nameof(SetReadyToCollide), data.exitTriggerCollisionDelay);
    }

    private void SetReadyToCollide()
    {
        ReadyToCollide = true;
    }

    public void ResetNavigation()
    {
        navAgent.ResetPath();
    }

    public void StopNavigation()
    {
        navAgent.isStopped = true;
    }

    public void ResumeNavigation()
    {
        navAgent.isStopped = false;
    }
}
