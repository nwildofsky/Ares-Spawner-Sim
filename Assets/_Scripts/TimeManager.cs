using UnityEngine;

// Separate class for keeping track of in-game time related values and
// for handling common Time API calls in other scripts and UnityEvents
//
// Instead of each script needing their own calculations of the time,
// handle it all in this script with static properties other scripts can easily access
public class TimeManager : MonoBehaviour
{
    // Synced with running scene
    public static float GameTime { get; private set; }
    private static bool _isGameRunning = false;

    // Settings
    [SerializeField]
    private bool _startGameTimeOnPlay = true;

    private void Awake()
    {
        GameTime = 0f;
    }

    private void Start()
    {
        _isGameRunning = _startGameTimeOnPlay;
    }

    private void Update()
    {
        if (_isGameRunning)
        {
            GameTime += Time.deltaTime;
        }
    }

    // API Methods
    public static void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    public static void SetIsGameRunning(bool isRunning)
    {
        _isGameRunning = isRunning;
    }
}
