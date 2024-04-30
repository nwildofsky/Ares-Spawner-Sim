using UnityEngine;

// Handles displaying each of the 4 creature counts as progress bars
//
// Doesn't display percentages accurately, instead this uses the majority of the
// progress bar to display how close to evenly displaced this creature count is
// and the rest of the progress bar simply displays how much over 25% this creature is
public class CreatureCountDisplay : MonoBehaviour
{
    // Reference to the listener with all the calculated values
    [SerializeField]
    private CreatureCountListener _data;

    // Required Settings
    public AgentType type;              // Select the type this class displays
    
    [SerializeField]
    private RectTransform _progressBar;
    [SerializeField]
    private float _minWidth;
    [SerializeField]
    private float _stablePercentWidth;    // Width of the progress bar when creature count is at 25%
    [SerializeField]
    private float _maxWidth;

    private void Awake()
    {
        // Initialize the progress bar at 0%
        _progressBar.sizeDelta = new Vector2(_minWidth, _progressBar.sizeDelta.y);
    }

    // Subscribe to Event Manager events on enable
    private void OnEnable()
    {
        EventManager.UI.OnAgentCountHeard += AdjustWidth;
    }

    // Unsubscribe to Event Manager events on disable
    private void OnDisable()
    {
        EventManager.UI.OnAgentCountHeard -= AdjustWidth;
    }

    // Change the width of the progress bar according to how many of this creature type
    // there are currently active in the scene
    private void AdjustWidth()
    {
        float percentage = 0f;

        // Grab the correct percentage according to this type
        switch (type)
        {
            case AgentType.Bat:
                percentage = _data.BatCountAsPercentage;
                break;
            case AgentType.Crab:
                percentage = _data.CrabCountAsPercentage;
                break;
            case AgentType.Salamander:
                percentage = _data.SalamanderCountAsPercentage;
                break;
            case AgentType.Spider:
                percentage = _data.SpiderCountAsPercentage;
                break;
            default: break;
        }

        // Linearly display the percentage with the progress bar when it's under 25%
        if (percentage <= .25f)
        {
            _progressBar.sizeDelta = new Vector2(Mathf.Lerp(_minWidth, _stablePercentWidth, percentage / 0.25f), _progressBar.sizeDelta.y);
        }
        // Display percentages above 25% with a smooth step function
        else if (percentage > 0.25f)
        {
            _progressBar.sizeDelta = new Vector2(Mathf.SmoothStep(_stablePercentWidth, _maxWidth, percentage), _progressBar.sizeDelta.y);
        }
    }
}
