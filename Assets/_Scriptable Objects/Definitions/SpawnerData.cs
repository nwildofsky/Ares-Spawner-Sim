using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TestSpawnerData", menuName = "ScriptObj/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    [Header("Object Pool")]
    public Agent prefabToSpawn;
    public int defaultPoolSize = 50;
    public int maxPoolSize = 100;
    public bool maxSizeIsHardLimit = true;

    [Header("Spawn Timings")]
    public bool spawnOnPlay = true;
    public float spawnDelay = 0f;
    public List<float> spawnTimings;

    [Header("Accelerated Spawn Properties")]
    public List<float> acceleratedSpawnTimings;
    public float acceleratedDuration = 7f;
    public float acceleratedCooldown = 5f;
}
