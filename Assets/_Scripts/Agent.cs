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
    public AgentType Type { get => data.type; private set => data.type = value; }
    private Action<Agent> collideSpawnAction;
    private Action<Agent> collideDestroyAction;
    //private bool hasCollided = true;
    public bool HasCollided { get; set; }

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
        }
    }

    private void Start()
    {
        // Initially activate collisions with a delay after instantiation
        Invoke("SetHasNotCollided", data.startupCollisionDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Agent>(out Agent otherAgent))
        {
            // Collision of similar agents
            if (otherAgent.Type == Type)
            {
                if (!HasCollided && !otherAgent.HasCollided)
                {
                    otherAgent.HasCollided = true;
                    HasCollided = true;

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
                // Destroy both objects
                collideDestroyAction?.Invoke(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Activate collisions again with a delay after the last collision
        Invoke("SetHasNotCollided", data.exitTriggerCollisionDelay);
    }

    private void SetHasNotCollided()
    {
        HasCollided = false;
    }

    public void ResetNavigation()
    {
        navAgent.ResetPath();
    }
}
