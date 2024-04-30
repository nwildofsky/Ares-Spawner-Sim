using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateManager : MonoBehaviour
{
    [SerializeField]
    private int _targetFramerate = 61;

    private void OnEnable()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = _targetFramerate;
    }
}
