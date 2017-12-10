using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that provides functions for refresh UI time
/// </summary>

/*
    Class that provides functions for refresh UI time. Function for converting type long milliseconds for TimeStamp format.

    @Author: Olli Korhonen
*/


public class Timer : MonoBehaviour
{
    public Text output;

    private void OnEnable()
    {
        GameManagerScript.timerUpdated += TimerUpdated;
    }

    private void OnDisable()
    {
        GameManagerScript.timerUpdated -= TimerUpdated;
    }

    /*
    public void refreshTime(float seconds_passed)
    {
        output.text = convertToTimeStamp((long)(seconds_passed * 1000));
    }
    */

    string ConvertToTimestamp(long total_milliseconds)
    {
        int minutes = (int)(total_milliseconds / 60000);
        int seconds = (int)(total_milliseconds / 1000) - minutes * 60;
        int milliseconds = (int)(total_milliseconds - seconds * 1000 - minutes * 60000);

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    void TimerUpdated(float secondsPassed)
    {
        output.text = ConvertToTimestamp((long)(secondsPassed * 1000));
    }
}
