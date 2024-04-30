using UnityEngine;
using UnityEngine.AI;

// Separate class for simply handling interactions with an animator component that are shared with all agents
//
// Makes use of its own reference to the NavMeshAgent and the observer pattern so agent animations can be handled
// apart from the agent class, and agents aren't forced to have an animator aspect
[RequireComponent(typeof(Animator))]
public class AgentAnimations : MonoBehaviour
{
    // References
    [SerializeField]
    private NavMeshAgent _agent;
    private Animator _animator;

    // Hash IDs for Animator variables
    private int _velocityHash;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        _velocityHash = Animator.StringToHash("VelocityMag");
    }

    // Subscribe to Event Manager events on enable
    private void OnEnable()
    {
        EventManager.AgentEvent.OnAgentAnimatedCollision += TriggerWithID;
    }

    // Unsubscribe to Event Manager events on disable
    private void OnDisable()
    {
        EventManager.AgentEvent.OnAgentAnimatedCollision -= TriggerWithID;
    }

    // Constantly send the current NavMeshAgent velocity to the animator for accurate animations
    void Update()
    {
        _animator.SetFloat(_velocityHash, _agent.velocity.magnitude);
    }
    
    // Received as an event with a specific NavMeshAgent and an Animator Hash ID
    // If the NavMeshAgent is the same reference stored in this class, trigger the Animator property
    private void TriggerWithID(NavMeshAgent navAgent, int triggerID)
    {
        if (_agent != navAgent)
            return;

        _animator.SetTrigger(triggerID);
    }
}
