using UnityEngine;
using UnityEngine.AI;

// When attached to a NavMeshAgent, it will automatically start randomly wandering around
// a baked navmesh and inside of the rect bounds set in the inspector
[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshWander : MonoBehaviour
{
    private NavMeshAgent _agent;

    // Bounds
    public Vector2 xWanderWithin;
    public Vector2 zWanderWithin;

    private void Awake()
    {
        // Get a reference to the agent that must exist
        _agent = GetComponent<NavMeshAgent>();
    }

    // Find a random viable spot on the navmesh and go there
    void Update()
    {
        // Once the agent reaches its first destination it will calculate another one
        // Calculation and setting of destinations is halted while the agent is set to be stopped
        if (!_agent.hasPath && !_agent.isStopped)
        {
            // Only set the destination, once a viable position has been found
            // Even if no position is found, update will run the same process again on the next frame
            if (FindRandomPoint(xWanderWithin.x, xWanderWithin.y, zWanderWithin.x, zWanderWithin.y, out Vector3 result))
            {
                _agent.SetDestination(result);
            }
        }
    }

    // Referenced from an example in the Unity docs:
    // https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
    //
    // Search for a random spot on the baked navmesh and within rect bounds and test it to see if it's accesable
    // If not accesible, the process will just be run again until it's exhausted all of its tries
    // Returns - true | false if a position was found, and a reference to the position if found
    bool FindRandomPoint(float xMin, float xMax, float zMin, float zMax, out Vector3 result)
    {
        // Use 10 iterations to look for a spot
        for (int i = 0; i < 10; i++)
        {
            float randomX = Random.Range(xMin, xMax);
            float randomZ = Random.Range(zMin, zMax);

            // Keep Y position the same as it currently is
            // For a non-flat terrain but smooth terrain, this will still work fine, but
            // if there is enough height difference in the terrain, it should be sampled as
            // well to get the right y-position
            Vector3 randomPoint = new Vector3(randomX, transform.position.y, randomZ);

            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
}
