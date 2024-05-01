using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

// Class representing the identity of a SpawnerData object which defines how agents are
// pooled, state changes, and behavior on touch
public class Spawner : MonoBehaviour, ITouchable
{
    // References
    [SerializeField]
    private SpawnerData _data;
    [SerializeField]
    private Transform _spawnLocation;

    // Object pool
    private ObjectPool<Agent> _pool;
    private bool _isPoolReady;

    // State
    public bool IsSpawning { get; private set; }
    public bool IsAccelerated { get; private set; }
    public bool InCooldown { get; private set; }

    // Timings
    private int _timingIdx;                  // Which time delay to check in a list of timings
    private float _spawnTimer;               // Timer calculating the correct time to spawn agents
    private float _acceleratedDurationTimer; // Timer calculating the correct time to decelerate spawner
    private float _acceleratedCooldownTimer; // Timer calculating the correct time to end the spawner's cooldown

    // Initialize the object pool at the start of the scene
    private void Awake()
    {
        // Use defined methods for behavior when interacting with the pool
        _pool = new ObjectPool<Agent>(CreateAgent, OnGetFromPool, OnReleaseFromPool, OnDestroyPooledObject,
            false,                  // Don't need to check if the same agent has been released into the pool multiple times
            _data.defaultPoolSize,   // On initialization the underlying data structure should have this much space
                                    // By default, this many agents are not instantiated on initialization of the pool
            _data.maxPoolSize);      // Limit of how many agents can be held inside of the underlying data structure after resizing
                                    // By default, more agents than this can be instantiated but will be destroyed on release
    }

    private void Start()
    {
        // Should start spawning at the start of the scene?
        if (_data.spawnOnPlay)
        {
            IsSpawning = true;
            EventManager.SpawnerEvent.OnResetState(this);
        }

        // Initial timing values
        _timingIdx = 0;
        _spawnTimer = 0f;
        _acceleratedDurationTimer = 0f;
        _acceleratedCooldownTimer = 0f;

        // Should pre spawn all default objects in the pool at startup?
        _isPoolReady = false;
        if (_data.instantiateOnStart)
        {
            // References to each object are required for release
            Agent[] startupAgents = new Agent[_data.defaultPoolSize];
            // Create all default objects
            for (int i = 0; i < _data.defaultPoolSize; i++)
            {
                startupAgents[i] = _pool.Get();
            }
            // Release all created objects
            for (int i = 0; i < _data.defaultPoolSize; i++)
            {
                _pool.Release(startupAgents[i]);
            }
        }
        _isPoolReady = true;
    }

    private void Update()
    {
        // Start spawning after a delay, if not already active
        if (!IsSpawning && _data.spawnDelay > 0f)
        {
            // Use the spawn timer to check for the delay, then reset it for spawning
            if (_spawnTimer > _data.spawnDelay)
            {
                IsSpawning = true;
                _spawnTimer = 0f;
                EventManager.SpawnerEvent.OnResetState(this);
            }
            _spawnTimer += Time.deltaTime;
        }

        if (IsSpawning)
        {
            // Spawn objects at the specified intervals
            if (!IsAccelerated)
            {
                SpawnWhenReady(_data.spawnTimings);
            }
            // Spawn objects at sped up intervals
            else if (IsAccelerated)
            {
                SpawnWhenReady(_data.acceleratedSpawnTimings);

                // Stop accelerated spawning after its duration is up
                DecelerateAfterTimer();
            }
        }

        // Prevent accelerating again while in cooldown
        if (InCooldown)
        {
            EndCooldownAfterTimer();
        }
    }

    #region State machine control and timer relevant methods
    // Spawns an agent by getting one from the pool
    private void Spawn(List<float> timings)
    {
        // Leave early on wrong state
        if (!IsSpawning)
            return;

        PoolGetWithinLimit();

        // Reset timer and index for next spawn time
        _spawnTimer = 0f;
        _timingIdx = (_timingIdx + 1) % timings.Count;
    }

    // Prepares the spawner to use faster spawn timings
    public void Accelerate()
    {
        // Leave early on wrong state
        if (!IsSpawning)
            return;

        if (InCooldown)
            return;

        if (IsAccelerated)
            return;

        // Change state to accelerated and reset spawn timer and index
        IsAccelerated = true;
        _spawnTimer = 0f;
        _timingIdx = 0;

        EventManager.SpawnerEvent.OnAccelerate(this);
    }

    // Sets the spawner back to regular spawn timings and begins cooldown
    public void Decelerate()
    {
        // Leave early on wrong state
        if (!IsSpawning)
            return;

        if (InCooldown)
            return;

        if (!IsAccelerated)
            return;

        // Change state back to non-accelerated, start cooldown, and reset spawn timer and index
        IsAccelerated = false;
        InCooldown = true;
        _spawnTimer = 0f;
        _timingIdx = 0;
        // Acceleration guaranteed to be finished, reset timer so it can be used again
        _acceleratedDurationTimer = 0f;

        EventManager.SpawnerEvent.OnBeginCooldown(this);
    }

    // Sets the spawner back to default state where it can be accelerated again
    public void EndCooldown()
    {
        // Leave early on wrong state
        if (!IsSpawning)
            return;

        if (IsAccelerated)
            return;

        if (!InCooldown)
            return;

        // Stops cooldown, spawn timer does not need to be reset
        InCooldown = false;
        // Reset cooldown timer so it can be used again
        _acceleratedCooldownTimer = 0f;

        EventManager.SpawnerEvent.OnResetState(this);
    }

    // For use in Update
    // Trigger a spawn when the spawn timer has reached the next spawn delay
    private void SpawnWhenReady(List<float> timings)
    {
        if (_spawnTimer > timings[_timingIdx])
        {
            Spawn(timings);
        }
        _spawnTimer += Time.deltaTime;
    }

    // For use in Update
    // Stop acceleration after the duration timer has reached the duration delay
    private void DecelerateAfterTimer()
    {
        if (_acceleratedDurationTimer > _data.acceleratedDuration)
        {
            Decelerate();
        }
        _acceleratedDurationTimer += Time.deltaTime;
    }

    // For use in Update
    // Stop cooldown after the cooldown timer has reached the duration delay
    private void EndCooldownAfterTimer()
    {
        if (_acceleratedCooldownTimer > _data.acceleratedCooldown)
        {
            EndCooldown();
        }
        _acceleratedCooldownTimer += Time.deltaTime;
    }
    #endregion

    #region Event Handlers
    // Attempt to accelerate this spawner once a touch has been confirmed on it
    public void HandleTouch()
    {
        Accelerate();
    }
    #endregion

    #region Object pool functions
    // Called when an object is needed and there are no inactive objects to get
    private Agent CreateAgent()
    {
        // Creates a new agent at the spawn location
        Agent agent = Instantiate(_data.prefabToSpawn, _spawnLocation.position, _spawnLocation.rotation);
        // Set its pool behavior on collision
        agent.SetActions(CreateAgentFromCollision, DestroyAgent);

        return agent;
    }

    // Called every time an object is asked for from the pool
    private void OnGetFromPool(Agent agent)
    {
        // Set agent active
        agent.gameObject.SetActive(true);

        if (_isPoolReady)
        {
            // Update agent counts after startup
            EventManager.UI.OnAgentCountChanged?.Invoke(_data.prefabToSpawn.Type, _pool.CountActive);

            // Have the agent appear with an animation
            agent.AppearAnimation?.DOPlayForward();
        }
    }

    // Called when an object is being given back to the managed pool and there is space left in the pool
    private void OnReleaseFromPool(Agent agent)
    {
        // Reset the agent's position back to the spawn location
        agent.transform.position = _spawnLocation.position;
        agent.transform.rotation = _spawnLocation.rotation;
        // Disable the agent
        agent.gameObject.SetActive(false);

        // Update agent counts after pool startup
        if (_isPoolReady)
        {
            // At this point pool.CountActive has not accounted for this agent yet
            EventManager.UI.OnAgentCountChanged?.Invoke(_data.prefabToSpawn.Type, _pool.CountActive - 1);

            // If the last active agent is released, end the game
            if (_pool.CountActive - 1 == 0)
            {
                EventManager.Game.OnGameEnd?.Invoke();
            }
        }
    }

    // Called when an object is being given back to the managed pool and there is no space left in the pool
    private void OnDestroyPooledObject(Agent agent)
    {
        // Destroy the agent
        Destroy(agent.gameObject);
    }

    // Additional layer on top of pool.Get preventing additional agents to be instantiated past the max size
    private Agent PoolGetWithinLimit()
    {
        // Only functional when maxSizeIsHardLimit is true
        if (_data.maxSizeIsHardLimit && _pool.CountActive >= _data.maxPoolSize)
            return null;

        return _pool.Get();
    }
    #endregion

    #region Agent specific functions defined in the spawner so the agents don't have any dependency on the pool
    // On collision of 2 similar agents, create a new agent in the general area
    private void CreateAgentFromCollision(Agent agent)
    {
        // Grab a new agent from the pool
        Agent newAgent = PoolGetWithinLimit();
        // Exit if no more agents can be created
        if (newAgent == null)
            return;

        // Get a random position around the area of the collision
        // There is no Random.onUnitCircle, so convert onUnitSphere into the XZ plane
        Vector3 spawnPos = UnityEngine.Random.onUnitSphere;
        spawnPos.y = 0f;
        // .normalized won't work properly on a zero vector, default to right
        if (spawnPos == Vector3.zero)
            spawnPos = Vector3.right;
        // Use spawn radius as the distance from the collision to spawn
        spawnPos = spawnPos.normalized * agent.SpawnRadius;

        // Set the new agent's position
        newAgent.transform.position = agent.transform.position + spawnPos;
    }

    // On collision of 2 different agents, destroy both agents
    private void DestroyAgent(Agent agent)
    {
        // Make the agent disappear by playing the appear animation backwards
        agent.AppearAnimation?.DOPlayBackwards();

        // If agent has an animation, destroy it after the animation finishes
        if (agent.AppearAnimation != null)
        {
            agent.AppearAnimation.GetTweens()[0].OnRewind(() => { _pool.Release(agent); });
        }
        else
        {
            // Give the agent back to the pool
            _pool.Release(agent);
        }
    }
    #endregion
}
