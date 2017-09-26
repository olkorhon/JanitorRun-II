
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.Events;

/// <summary>
/// Class that provides a stopwatch based timer that calls a defined UnityEvent after a defined amount of time has passed.
/// /summary>

/*
    Class that provides a stopwatch based timer that calls a defined UnityEvent after a defined amount of time has passed.
    Timer can be manipulated with the basic methods start, pause, cotinue and end.
    UnityEvent needs to be set in the scene editor explicitly

    @Author: Olli Korhonen
*/


public class Countdown : MonoBehaviour
{
    private Stopwatch stopwatch = new Stopwatch();

    public UnityEvent end_callback;
    public Text output;

    private int target_time = 0;

	// Update is called once per frame
	void Update ()
    {
        if (stopwatch.IsRunning)
        {
            long milliseconds_passed = stopwatch.ElapsedMilliseconds;
 
            if (target_time - milliseconds_passed > 0)
            {
                // Normal case, parse and log the passed time
                output.text = convertToTimeStamp(target_time - milliseconds_passed);
            }
            else
            {
                // The timer has run out, invoke ending event and stop the watch
                output.text = convertToTimeStamp(0);

                stopwatch.Stop();
                end_callback.Invoke();
            }
        }
    }

    public void startCountdown(int time)
    {
        target_time = time;
        stopwatch.Start();
    }
    public void stopCountdown()
    {
        stopwatch.Reset();
    }
    public void pauseCountdown()
    {
        stopwatch.Stop();
    }
    public void continueCountdown()
    {
        stopwatch.Start();
    }

    private string convertToTimeStamp(long total_milliseconds)
    {
        // Determine the components of the timestamp
        int minutes = (int)(total_milliseconds / 60000);
        int seconds = (int)(total_milliseconds / 1000) - minutes * 60;
        int milliseconds = (int)(total_milliseconds - seconds * 1000 - minutes * 60000);

        // Format and return the result
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
