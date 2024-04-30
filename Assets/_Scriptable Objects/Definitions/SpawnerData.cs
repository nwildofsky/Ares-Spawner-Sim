using System.Collections.Generic;
using UnityEngine;

// Contains all data which defines an spawner's identity and its modifiable parameters
[CreateAssetMenu(fileName = "TestSpawnerData", menuName = "ScriptObj/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    [Header("Object Pool")]
    public Agent prefabToSpawn;
    public int defaultPoolSize = 50;
    public int maxPoolSize = 100;
    public bool maxSizeIsHardLimit = true;  // Prevent any additional objects from being created after the max size

    [Header("Spawn Timings")]
    public bool spawnOnPlay = true;
    public float spawnDelay = 0f;           // Delay is not used when Spawn On Play is checked
    public List<float> spawnTimings;        // List of delays for the spawner to run through in its update cycle

    [Header("Accelerated Spawn Properties")]
    public List<float> acceleratedSpawnTimings; // List of delays to use once a spawner is accelerated
    public float acceleratedDuration = 7f;
    public float acceleratedCooldown = 5f;
}
