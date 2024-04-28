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

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.Game.OnDeathCollision += TriggerDeath;
    }

    private void OnDisable()
    {
        EventManager.Game.OnDeathCollision -= TriggerDeath;
    }

    void Update()
    {
        animator.SetFloat("VelocityMag", agent.velocity.magnitude);
    }
    
    private void TriggerDeath(NavMeshAgent navAgent)
    {
        if (agent != navAgent)
            return;

        animator.SetTrigger("Death");
    }
}
