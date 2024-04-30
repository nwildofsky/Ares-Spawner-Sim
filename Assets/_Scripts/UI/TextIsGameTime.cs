using System;
using TMPro;
using UnityEngine;

// Simple script which updates the entirety of the TMP text field its attached to every update
// Displays the Time Manager GameTime in the format minutes:seconds:miliseconds
[RequireComponent(typeof(TextMeshProUGUI))]
public class TextIsGameTime : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private char[] _textArray;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _textArray = new char[8];
    }

    void Update()
    {
        // Removes the generation of any garbage by using TMP's SetCharArray
        // function rather than directly setting a string
        // Solution for this issue found in this forum post
        // https://forum.unity.com/threads/ui-timer-without-garbage-collection-possible.663874/
        SecondsToCharArray(TimeManager.GameTime, _textArray);
        _text.SetCharArray(_textArray);
    }

    // Converts a float timer field in seconds (with milliseconds as decimal) to a char array for displaying
    // References code post at end of this forum page
    // https://forum.unity.com/threads/ui-timer-without-garbage-collection-possible.663874/
    private void SecondsToCharArray(float timeInSeconds, char[] array)
    {
        int minutes = (int)(timeInSeconds / 60);
        array[0] = (char)(48 + minutes / 10);
        array[1] = (char)(48 + minutes % 10);
        array[2] = ':';

        int seconds = (int)(timeInSeconds % 60);
        array[3] = (char)(48 + seconds / 10);
        array[4] = (char)(48 + seconds % 10);
        array[5] = ':';

        int milliseconds = (int)((timeInSeconds % 1) * 1000);
        array[6] = (char)(48 + milliseconds / 100);
        array[7] = (char)(48 + (milliseconds % 100) / 10);
    }
}
