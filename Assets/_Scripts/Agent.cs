using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent (typeof(Collider))]
public class Agent : MonoBehaviour
{
    [SerializeField]
    private AgentData data;
    public float SpawnRadius { get => data.spawnRadius; }
    private NavMeshAgent navAgent;
    public DOTweenAnimation appearAnim;
    private Collider mainCollider;
    public AgentType Type { get => data.type; private set => data.type = value; }
    private Action<Agent> collideSpawnAction;
    private Action<Agent> collideDestroyAction;
    //private bool hasCollided = true;
    private int deathTriggerID;
    private int createTriggerID;
    public bool ReadyToCollide { get; set; }

    public void SetActions(Action<Agent> spawnAction, Action<Agent> destroyAction)
    {
        collideSpawnAction = spawnAction;
        collideDestroyAction = destroyAction;
    }

    private void Awake()
    {
        navAgent = GetComponent<NavMeshAgent>();
        mainCollider = GetComponent<Collider>();

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

        deathTriggerID = Animator.StringToHash("Death");
        createTriggerID = Animator.StringToHash("Create");
    }

    private void OnEnable()
    {
        // Ensure ready to collide starts as false
        ReadyToCollide = false;
        // Initially activate collisions with a delay after instantiation
        Invoke(nameof(SetReadyToCollide), data.startupCollisionDelay);
        // Reset collider to enabled once spawned or respawned
        mainCollider.enabled = true;
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
                    ReadyToCollide = false;
                    otherAgent.ReadyToCollide = false;

                    // Set new destinations for after collision
                    //ResetNavigation();
                    //otherAgent.ResetNavigation();
                    StopNavigationForSeconds(1f);
                    otherAgent.StopNavigationForSeconds(1f);

                    // Trigger Create animation
                    TriggerAnimationWithID(createTriggerID);
                    otherAgent.TriggerAnimationWithID(createTriggerID);

                    // Spawn a new object of the same type
                    collideSpawnAction?.Invoke(this);
                }
            }
            // Collision of different agents
            else
            {
                // Disable collider to prevent further collisions while being destroyed
                mainCollider.enabled = false;
                // Stop movement
                StopNavigation();
                // Trigger death animation
                TriggerAnimationWithID(deathTriggerID);
                // Destroy both objects
                collideDestroyAction?.Invoke(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (CompareTag(other.tag))
        {
            // Activate collisions again with a delay after the last collision
            Invoke(nameof(SetReadyToCollide), data.exitTriggerCollisionDelay);
        }
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
        navAgent.velocity = Vector3.zero;
    }

    public void StopNavigationForSeconds(float seconds)
    {
        navAgent.isStopped = true;
        navAgent.velocity = Vector3.zero;

        Invoke(nameof(ResetNavigation), seconds);
    }

    public void ResumeNavigation()
    {
        navAgent.isStopped = false;
    }

    public void TriggerAnimationWithID(int id)
    {
        EventManager.Game.OnAgentCollision?.Invoke(navAgent, id);
    }
}
