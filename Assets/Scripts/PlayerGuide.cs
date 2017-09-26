using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGuide : MonoBehaviour
{
    private PlayerControl control;
    private ProgressScript progress;

    public Text HungryText;

    // Use this for initialization
    void Start ()
    {
        control = this.GetComponent<PlayerControl>();
        HungryText.text = "";

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

	// Update is called once per frame
	void Update ()
    {
        checkHunger();
    }

    IEnumerator delayedTextReset(float time)
    {
        yield return new WaitForSeconds(time);

        HungryText.text = "";
    }

    void checkHunger()
    {
        if (control.getCurrentStamina() < .25f && control.kickInput)
        {
            Debug.Log("Näläkä!!");
            StartCoroutine(delayedTextReset(2));
            HungryText.text = "Näläkä!";
        }
        // Will always reset the text instantly
        //if(control.getCurrentStamina() > .25f)
        //{
        //    HungryText.text = "";
        //}
    }

    // Callback for the progress script
    void handleRightCheckpoint(CheckpointScript checkpoint)
    {
        HungryText.text = checkpoint.getName();
        StartCoroutine(delayedTextReset(2));
    }

    // Callback for the progress script
    void handleWrongCheckpoint(CheckpointScript checkpoint)
    {
        HungryText.text = "Wrong checkpoint!";
        StartCoroutine(delayedTextReset(2));
    }
}
