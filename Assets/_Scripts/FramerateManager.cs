using UnityEngine;

// Simple script which enables mobile devices to not be capped at 30 fps
// Learned the technique from this forum
// https://forum.unity.com/threads/set-the-target-frame-rate-to-60-on-android.720584/
public class FramerateManager : MonoBehaviour
{
    [SerializeField]
    private int _targetFramerate = 61;

    // This must be done after Awake and Start to actually take effect
    private void OnEnable()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFramerate;
    }
}
