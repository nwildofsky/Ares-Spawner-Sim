using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;

// Class representing the identity of an AgentData object which defines behavior
// that should occur when collisions between agents are triggered
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent (typeof(Collider))]
public class Agent : MonoBehaviour
{
    // References
    [SerializeField]
    private AgentData _data;
    private NavMeshAgent _navAgent;
    private Collider _collider;             // Collider that handles trigger collisions between agents
    [SerializeField]
    private DOTweenAnimation _appearAnim;   // DoTween Editor component which defines how agents fade in to the scene on instantiation

    // Fields
    // Delegates leaving the responsibility of instantiating and destroying agents to another script
    private Action<Agent> _collideSpawnAction;
    private Action<Agent> _collideDestroyAction;
    // Hash IDs for shared Animation properties
    // Defines the names that must be used on animator components for agent animations
    private int _deathTriggerID;
    private int _createTriggerID;

    // Properties
    public AgentType Type { get => _data.type; private set => _data.type = value; }
    public float SpawnRadius { get => _data.spawnRadius; }
    public bool ReadyToCollide { get; set; }    // Only allow collisions between 2 similar agents when this is true
    public DOTweenAnimation AppearAnimation { get => _appearAnim; }

    // Public method which allows another script to set the methods executed when an
    // agent needs to be created or destroyed
    public void SetActions(Action<Agent> spawnAction, Action<Agent> destroyAction)
    {
        _collideSpawnAction = spawnAction;
        _collideDestroyAction = destroyAction;
    }

    private void Awake()
    {
        // Grab required references
        _navAgent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

        // Override 1:1 nav agent values
        if (_data.overrideNavMeshAgentValues)
        {
            _navAgent.speed = _data.speed;
            _navAgent.angularSpeed = _data.angularSpeed;
            _navAgent.acceleration = _data.acceleration;
            _navAgent.stoppingDistance = _data.stoppingDistance;
            _navAgent.autoBraking = _data.autoBraking;

            _navAgent.radius = _data.radius;
            _navAgent.height = _data.height;
            _navAgent.obstacleAvoidanceType = _data.obstacleAvoidanceType;
            _navAgent.avoidancePriority = _data.priority;
        }

        // Defines the names that must be used on animator components for agent animations
        _deathTriggerID = Animator.StringToHash("Death");
        _createTriggerID = Animator.StringToHash("Create");
    }

    private void OnEnable()
    {
        // Ensure new agents and reenabled agents have a delay until they are ready to collide 
        ReadyToCollide = false;
        // Initially allow collisions with a delay after instantiation
        Invoke(nameof(SetReadyToCollide), _data.startupCollisionDelay);
        // Reset actual collider to enabled once spawned or respawned
        _collider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // If agent collided with another agent
        if (other.TryGetComponent<Agent>(out Agent otherAgent))
        {
            // Collision of similar agents
            if (otherAgent.Type == Type)
            {
                // Only handle collision when both agents are ready
                // This collision should only be run once, so one agent
                // out of the 2 will handle both agents logic
                if (ReadyToCollide && otherAgent.ReadyToCollide)
                {
                    // Set both agents no longer ready to collide
                    ReadyToCollide = false;
                    otherAgent.ReadyToCollide = false;

                    // Stop both agent's movement for a short time after collision
                    StopNavigationForSeconds(1f);
                    otherAgent.StopNavigationForSeconds(1f);

                    // Trigger both agent's create animation
                    TriggerAnimationWithID(_createTriggerID);
                    otherAgent.TriggerAnimationWithID(_createTriggerID);

                    // Spawn a new object of the same type
                    _collideSpawnAction?.Invoke(this);
                }
            }
            // Collision of different agents
            // This collision can always happen as long as the collider component is enabled
            else
            {
                // Disable collider to prevent further collisions while being destroyed
                _collider.enabled = false;

                // Stop movement
                StopNavigation();

                // Trigger death animation
                TriggerAnimationWithID(_deathTriggerID);

                // Destroy both objects
                _collideDestroyAction?.Invoke(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Once agent has left the collider of the agent it just collided with
        if (CompareTag(other.tag))
        {
            // Activate collisions again with a delay after the last collision
            Invoke(nameof(SetReadyToCollide), _data.exitTriggerCollisionDelay);
        }
    }

    // Internal function only used for Invoke()
    private void SetReadyToCollide()
    {
        ReadyToCollide = true;
    }

    #region Navigation Control
    // Force a new destination to be calculated by removing the current path
    public void ResetNavigation()
    {
        _navAgent.ResetPath();
    }

    // Halt further navigation and ensure the agent stops to zero speed
    public void StopNavigation()
    {
        _navAgent.isStopped = true;
        _navAgent.velocity = Vector3.zero;
    }

    // Enable the nav agent to be able to move again
    public void ResumeNavigation()
    {
        _navAgent.isStopped = false;
    }

    // Stop navigation and then resume it again after a delay
    public void StopNavigationForSeconds(float seconds)
    {
        StopNavigation();
        Invoke(nameof(ResetNavigation), seconds);
    }
    #endregion

    // Set an animation trigger based on shared Animation property IDs all animated agents have
    // Use the EventManager to de-couple the agent and its animation handler
    public void TriggerAnimationWithID(int id)
    {
        EventManager.AgentEvent.OnAgentAnimatedCollision?.Invoke(_navAgent, id);
    }
}
