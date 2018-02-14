using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(Animator))]
public class CheckpointPopup : MonoBehaviour {
    Text output;
    Animator currentAnimator;
    public Timer timer;

    // Use this for initialization
    void Start ()
    {
        registerCheckpointListener();

        currentAnimator = GetComponent<Animator>();
        output = gameObject.GetComponent<Text>();

        if (timer == null)
        {
            Debug.LogWarning("Cannot initialise checkpoint popup, no Timer set");
            return;
        }
	}

    private void registerCheckpointListener()
    {
        GameObject progressTracker = GameObject.FindWithTag("ProgressTracker");
        if (progressTracker == null)
        {
            Debug.LogWarning("Cannot initialize checkpoint popups, cannot find GameObject tagged with ProgressTracker");
            return;
        }

        ProgressScript progressScript = progressTracker.GetComponent<ProgressScript>();
        if (progressScript == null)
        {
            Debug.LogWarning("Cannot initialize checkpoint popups, cannot find ProgressScript in progressTracker GameObject");
            return;
        }

        progressScript.correct_checkpoint_hit += showTime;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void showTime(GameObject player, Checkpoint a)
    {
        if (timer != null)
            output.text = timer.output.text;

        currentAnimator.SetTrigger("Show");
    }
}
