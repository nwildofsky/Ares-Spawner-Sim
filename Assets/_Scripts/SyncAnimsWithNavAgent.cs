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

    void Update()
    {
        animator.SetFloat("VelocityMag", agent.velocity.magnitude);
    }
}
