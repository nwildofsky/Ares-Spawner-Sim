using DG.Tweening;
using UnityEngine;

// Controlls commands applying to all the DoTween Animation components in the scene
public class DoTweenManager : MonoBehaviour
{
    // Preset the DoTween library to be ready for the maximum number of agents on start
    private void Awake()
    {
        DOTween.SetTweensCapacity(201, 100);
    }

    // Stop and destroy all remaining animations once the scene has been unloaded
    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
