using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour, ITouchable
{
    // References
    [SerializeField]
    private SpawnerData data;
    public Transform spawnLocation;
    //public MeshRenderer debugRenderer;
    //private Material debugDefault;
    //public Material debugAccel;
    //public Material debugCooldown;
    public ParticleSystem touchableSFX;
    public ParticleSystem acceleratedSFX;
    public ParticleSystem cooldownSFX;

    // Object pool
    private ObjectPool<Agent> pool;

    // State
    public bool IsSpawning { get; private set; }
    public bool IsAccelerated { get; private set; }
    public bool InCooldown { get; private set; }

    // Timings
    private int timingIdx;
    private float spawnTimer;
    private float acceleratedDurationTimer;
    private float acceleratedCooldownTimer;

    private void Awake()
    {
        pool = new ObjectPool<Agent>(CreateAgent, OnGetFromPool, OnReleaseFromPool, OnDestroyPooledObject,
            false, data.defaultPoolSize, data.maxPoolSize);

        //debugDefault = debugRenderer.material;
    }

    private void Start()
    {
        if (data.spawnOnPlay)
        {
            IsSpawning = true;
            touchableSFX.Play();
        }

        timingIdx = 0;
        spawnTimer = 0f;
        acceleratedDurationTimer = 0f;
        acceleratedCooldownTimer = 0f;
    }

    private void Update()
    {
        // Start spawning after a delay
        if (!IsSpawning && data.spawnDelay > 0f)
        {
            if (spawnTimer > data.spawnDelay)
            {
                IsSpawning = true;
                spawnTimer = 0f;
                touchableSFX.Play();
            }
            spawnTimer += Time.deltaTime;
        }

        if (IsSpawning)
        {
            // Spawn objects at the specified intervals
            if (!IsAccelerated)
            {
                SpawnWhenReady(data.spawnTimings);
            }
            // Spawn objects at sped up intervals
            else if (IsAccelerated)
            {
                SpawnWhenReady(data.acceleratedSpawnTimings);

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
    private void SpawnWhenReady(List<float> timings)
    {
        if (spawnTimer > timings[timingIdx])
        {
            Spawn(timings);
        }
        spawnTimer += Time.deltaTime;
    }

    private void Spawn(List<float> timings)
    {
        if (!IsSpawning)
            return;

        PoolGetWithinLimit();

        spawnTimer = 0f;
        timingIdx = (timingIdx + 1) % timings.Count;
    }

    public void Accelerate()
    {
        if (InCooldown)
            return;

        IsAccelerated = true;
        spawnTimer = 0f;
        timingIdx = 0;
        acceleratedCooldownTimer = 0f;

        //debugRenderer.material = debugAccel;
        acceleratedSFX.Play();
        touchableSFX.Stop();
    }

    private void DecelerateAfterTimer()
    {
        if (acceleratedDurationTimer > data.acceleratedDuration)
        {
            Decelerate();
        }
        acceleratedDurationTimer += Time.deltaTime;
    }

    public void Decelerate()
    {
        if (!IsAccelerated)
            return;

        IsAccelerated = false;
        InCooldown = true;
        spawnTimer = 0f;
        timingIdx = 0;
        acceleratedDurationTimer = 0f;

        //debugRenderer.material = debugCooldown;
        cooldownSFX.Play();
        acceleratedSFX.Stop();
    }

    private void EndCooldownAfterTimer()
    {
        if (acceleratedCooldownTimer > data.acceleratedCooldown)
        {
            EndCooldown();
        }
        acceleratedCooldownTimer += Time.deltaTime;
    }

    public void EndCooldown()
    {
        if (!InCooldown)
            return;

        InCooldown = false;
        acceleratedCooldownTimer = 0f;

        //debugRenderer.material = debugDefault;
        touchableSFX.Play();
        cooldownSFX.Stop();
    }
    #endregion

    #region Event Handlers
    public void HandleTouch()
    {
        Accelerate();

        Debug.Log($"{this.name} was touched!");

        EventManager.Game.OnTouchSpawner?.Invoke();
    }
    #endregion

    #region Object pool functions
    private Agent PoolGetWithinLimit()
    {
        if (data.maxSizeIsHardLimit && pool.CountActive >= data.maxPoolSize)
            return null;

        return pool.Get();
    }

    private Agent CreateAgent()
    {
        Agent agent = Instantiate(data.prefabToSpawn, spawnLocation.position, spawnLocation.rotation);
        agent.SetActions(CreateAgentFromCollision, DestroyAgent);

        return agent;
    }

    private void OnGetFromPool(Agent agent)
    {
        agent.gameObject.SetActive(true);

        EventManager.UI.OnAgentCountChanged?.Invoke(data.prefabToSpawn.Type, pool.CountActive);
    }

    private void OnReleaseFromPool(Agent agent)
    {
        agent.transform.position = spawnLocation.position;
        agent.transform.rotation = spawnLocation.rotation;
        agent.gameObject.SetActive(false);

        EventManager.UI.OnAgentCountChanged?.Invoke(data.prefabToSpawn.Type, pool.CountActive - 1);
        if (pool.CountActive - 1 == 0)
        {
            EventManager.Game.OnGameEnd?.Invoke();
        }
    }

    private void OnDestroyPooledObject(Agent agent)
    {
        Destroy(agent.gameObject);
    }
    #endregion

    #region Agent specific functions defined in the spawner so the agents don't have any dependency on the pool
    private void CreateAgentFromCollision(Agent agent)
    {
        Agent newAgent = PoolGetWithinLimit();
        if (newAgent == null)
            return;

        // Get a random position around the area of the collision
        Vector3 spawnPos = UnityEngine.Random.onUnitSphere;
        spawnPos.y = transform.position.y;
        if (spawnPos == Vector3.zero)
            spawnPos = Vector3.right;
        spawnPos = spawnPos.normalized * 5;

        newAgent.transform.position = agent.transform.position + spawnPos;
    }

    private void DestroyAgent(Agent agent)
    {
        pool.Release(agent);
    }
    #endregion
}
