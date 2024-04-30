using TMPro;
using UnityEngine;

// Simple script which updates the entirety of the TMP text field its attached to every update
// Displays the Time Manager GameTime in the format minutes:seconds:miliseconds
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextIsGameTime : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        _text.text = string.Format("{0:00}:{1:00}:{2:00}",
            (int)(TimeManager.GameTime * 0.0166f),          // Minutes
            (int)TimeManager.GameTime % 60,                 // Seconds
            (int)(TimeManager.GameTime * 100f) % 100);      // Miliseconds
    }
}
