using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshWander : MonoBehaviour
{
    public Vector2 xWanderWithin;
    public Vector2 zWanderWithin;

    NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.hasPath && !agent.isStopped)
        {
            if (FindRandomPoint(xWanderWithin.x, xWanderWithin.y, zWanderWithin.x, zWanderWithin.y, out Vector3 result))
            {
                agent.SetDestination(result);
            }
        }
    }

    // Referenced from an example in the Unity docs:
    // https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
    bool FindRandomPoint(float xMin, float xMax, float zMin, float zMax, out Vector3 result)
    {
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);
            Vector3 randomPoint = new Vector3(randomX, transform.position.y, randomZ);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
            //Debug.Log("Missed finding an available point with random numbers");
        }

        result = Vector3.zero;
        //Debug.Log("Couldn't find any point after 10 iterations");
        return false;
    }
}
