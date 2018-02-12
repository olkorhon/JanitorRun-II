using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    public Text status_text;
    public Text time_text;

	// Use this for initialization
	void Start ()
    {
        if (status_text == null)
            Debug.LogWarning("ResultPanel: No text element found, cannot display text.");

        if (time_text == null)
            Debug.LogWarning("ResultPanel: No text element found, cannot display text.");
    }

    // Inactivates this gameObject
    public void hide()
    {
        this.gameObject.SetActive(false);
    }

    // Activates this gameObject
    public void show()
    {
        this.gameObject.SetActive(true);
    }

    // Sets the status text
    public void setStatusText(string msg)
    {
        if (status_text != null)
            status_text.text = msg;
        else
            Debug.LogWarning("ResultPanel: Cannot set status text, no text element defined.");
    }

    // Sets the time text
    public void setTimeText(string msg)
    {
        if (time_text != null)
            time_text.text = msg;
        else
            Debug.LogWarning("ResultPanel: Cannot set time text, no text element defined.");
    }
    public void setTime(string formattable_message, long milliseconds)
    {
        if (time_text != null)
        {
            try
            {
                time_text.text = string.Format(formattable_message, convertToTimeStamp(milliseconds));
            }
            catch (Exception e)
            {
                // Most likely you provided a bad formattable message
                Debug.LogError(e.Message);
            }
        }
        else
            Debug.LogWarning("ResultPanel: Cannot dispay time, no text element defined");
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
