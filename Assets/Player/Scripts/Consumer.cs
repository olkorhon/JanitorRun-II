using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumer : MonoBehaviour
{
    public Transform mouth;

    public float consumeSpeed = 2.0f;

	// Use this for initialization
	void Start () {
        if (this.mouth == null)
        {
            Debug.Log("No mouth linked to Consumer component, will cause unwanted behaviourn.");
        }
	}
	
    public Transform getMouth()
    {
        return this.mouth;
    }

    public float getConsumeSpeed()
    {
        return this.consumeSpeed;
    }
}
