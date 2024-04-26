using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextIsGameTime : MonoBehaviour
{
    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = TimeManager.GameTime.ToString();
        text.text = string.Format("{0:00}:{1:00}:{2:00}", (int)TimeManager.GameTime / 60, (int)TimeManager.GameTime, (int)(TimeManager.GameTime * 100f) % 100);
    }
}
