using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static float GameTime { get; private set; }

    private static bool isGameRunning = false;

    private void Awake()
    {
        GameTime = 0f;
    }

    private void Start()
    {
        isGameRunning = true;
    }

    private void Update()
    {
        if (!isGameRunning)
        {
            GameTime += Time.deltaTime;
        }
    }

    public static void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public static void SetIsGameRunning(bool isRunning)
    {
        isGameRunning = isRunning;
    }
}
