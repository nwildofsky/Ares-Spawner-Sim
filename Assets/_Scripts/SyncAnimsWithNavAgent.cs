using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class SyncAnimsWithNavAgent : MonoBehaviour
{
    // References
    public NavMeshAgent agent;
    private Animator animator;

    private int velocityHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        velocityHash = Animator.StringToHash("VelocityMag");
    }

    private void OnEnable()
    {
        EventManager.Game.OnAgentCollision += TriggerWithID;
    }

    private void OnDisable()
    {
        EventManager.Game.OnAgentCollision -= TriggerWithID;
    }

    void Update()
    {
        animator.SetFloat(velocityHash, agent.velocity.magnitude);
    }
    
    private void TriggerWithID(NavMeshAgent navAgent, int triggerID)
    {
        if (agent != navAgent)
            return;

        animator.SetTrigger(triggerID);
    }
}
