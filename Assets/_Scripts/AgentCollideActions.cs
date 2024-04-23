using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentCollideActions : MonoBehaviour
{
    public enum AgentType
    { 
        Agent1,
        Agent2,
        Agent3,
        Agent4,
    }

    //public AgentType Type { get; private set; }
    public AgentType type;
    public GameObject prefab;

    private bool hasCollided = true;
    public bool HasCollided { get => hasCollided; set => hasCollided = value; }

    private void SetHasNotCollided()
    {
        hasCollided = false;
    }

    //private void Awake()
    //{
    //    type = ScriptableData.type;
    //}

    private void Start()
    {
        // Initially activate collisions with a delay after instantiation
        Invoke("SetHasNotCollided", .5f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<AgentCollideActions>(out AgentCollideActions otherAgent))
        {
            // Collision of similar agents
            if (otherAgent.type == type)
            {
                if (!hasCollided && !otherAgent.HasCollided)
                {
                    otherAgent.HasCollided = true;
                    hasCollided = true;

                    // Spawn a new object of the same type
                    Vector3 spawnPos = Random.onUnitSphere;
                    spawnPos.y = transform.position.y;
                    if (spawnPos == Vector3.zero)
                        spawnPos = Vector3.right;
                    spawnPos = spawnPos.normalized * 5;

                    Instantiate(prefab, transform.position + spawnPos, Quaternion.identity);
                }
            }
            // Collision of different agents
            else
            {
                // Destroy both objects
                //Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Activate collisions again with a delay after the last collision
        Invoke("SetHasNotCollided", 2);
    }
}
