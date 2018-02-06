
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Analytics;
using System.Collections.Generic;


/// <summary>
/// Class that listens to trigger events and sends them forwards if a player is detected
/// </summary>

/*
    Class that listens to trigger events and sends them forwards if a player is detected.
    Has an order number that defines the checkpoints position in the checkpoint path.
    Manages a UnityEvent that needs to be set separately in the scene editor
    
    TODO: 
    A system that automatically orders the checkpoints and links them to the central controller

    @Author: Olli Korhonen
*/

public class Checkpoint : MonoBehaviour
{
    [System.Serializable]
    public class myGameObjectEvent : UnityEvent<int, GameObject> { }
    public myGameObjectEvent checkpointHit;

    public string checkpoint_name;
    public GameObject timerObject;

    private int order;

    private string eventName = "";
    private float timerOutput = 0.0f;
    private GameManagerScript timer;


    // Use this for initialization
    void Start()
    {
        // TODO, automatic coupling of the event and a controller here     
        timer = timerObject.GetComponent<GameManagerScript>();

        ProgressScript progressScript = GetComponentInParent<ProgressScript>();
        if (progressScript)
        {
            checkpointHit.AddListener(new UnityAction<int, GameObject>(progressScript.objectHitCheckpoint));
        }

    }

    public int getOrder()
    {
        return order;
    }
    public void setOrder(int order)
    {
        this.order = order;
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject other_obj = other.gameObject;

        // Check if collided object is player
        if (other_obj.CompareTag("Player"))
        {
            eventName = this.gameObject.name;
            checkpointHit.Invoke(order, other_obj);

            Debug.Log("Custom event: " + eventName);
            timerOutput = Time.time - timer.startTime;
            Analytics.CustomEvent(eventName, new Dictionary<string, object> { { "time", timerOutput } });
        }
    }

    public string getName()
    {
        return checkpoint_name;
    }
}
