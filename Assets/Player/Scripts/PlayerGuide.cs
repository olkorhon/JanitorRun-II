using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGuide : MonoBehaviour
{
    private Steering control;
    private ProgressScript progress;

    public Text textField;

    // Use this for initialization
    void Start ()
    {
        control = this.GetComponent<Steering>();
        textField.text = "";

        // Delay fetching 
        StartCoroutine(delayedFetchProgress(1));
    }

    IEnumerator delayedFetchProgress(float time)
    {
        yield return new WaitForSeconds(time);

        // Setting up checkpoint event listeners
        progress = fetchProgressScript();
        if (progress != null)
        {
            Debug.Log("PlayerGuide: ProgressScript found and callbacks registered.");
            progress.setListenerRightCheckpoint(handleRightCheckpoint);
            progress.setListenerWrongCheckpoint(handleWrongCheckpoint);
        }
    }

    // Fetches the progress script from the scene
    private ProgressScript fetchProgressScript()
    {
        GameObject component_holder = GameObject.FindGameObjectWithTag("ProgressTracker");
        if (component_holder != null)
        {
            return component_holder.GetComponent<ProgressScript>();
        }
        else
        {
            Debug.LogError("PlayerGuide: Could not find object with < ProgressTracker > tag, cannot listen to checkpoint events.");
            return null;
        }
    }

    IEnumerator delayedTextReset(float time)
    {
        yield return new WaitForSeconds(time);

        textField.text = "";
    }

    // Callback for the progress script
    void handleRightCheckpoint(GameObject player, Checkpoint checkpoint)
    {
        textField.text = checkpoint.getName();
        StartCoroutine(delayedTextReset(2));
    }

    // Callback for the progress script
    void handleWrongCheckpoint(GameObject player, Checkpoint checkpoint)
    {
        textField.text = "Wrong checkpoint!";
        StartCoroutine(delayedTextReset(2));
    }
}
