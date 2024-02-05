using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DateAndTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timeDateDisplayText;
    [SerializeField] private TextMeshProUGUI _welcomeMessageText;
    private void Start()
    {
        UpdateTime();

        // Invoke the UpdateTime method every second (not in every update)
        InvokeRepeating("UpdateTime", 1f, 1f);
    }

    private void UpdateTime()
    {
        // Current Time
        DateTime currentTime = DateTime.Now;

        // Formatting the time string
        string timeString = currentTime.ToString("HH:mm:ss ddd dd-MMM-yy");

        // Set the TextMeshProUGUI texts
        _timeDateDisplayText.text = timeString;
        _welcomeMessageText.text = GetWelcomeMessage(currentTime);
    }

    private string GetWelcomeMessage(DateTime currentTime)
    {
        // Get the hour of the day
        int hour = currentTime.Hour;

        // Determine the time of day
        string welcomeMessage = "";
        if (hour >= 5 && hour < 12)
            welcomeMessage = "Good morning!";
        else if (hour >= 12 && hour < 18)
            welcomeMessage = "Good afternoon!";
        else
            welcomeMessage = "Good evening!";

        return welcomeMessage;
    }

}
