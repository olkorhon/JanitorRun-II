
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that contains code for snow effect position setting
/// </summary>

/*
    Sets snow effect object's position and updates it based on camera position an direction.
    That way snow follows camera and it's always visible for player if weather is snowy.   

    @Author: Mikael Martinviita
*/



public class Snow : MonoBehaviour {

	// Use this for initialization
    // Initialize snow effect objects position 20 units forward from camera
	void Start ()
    {
        this.transform.position = Camera.main.transform.position + new Vector3(0.0f, 0.0f, 20.0f);
	}

    // Update is called once per frame
    // Update snow effect objects position based on where camera is looking
    void Update ()
    {
        Vector3 scaled = Vector3.Scale(Camera.main.transform.forward, new Vector3(20.0f, 0.0f, 20.0f));
        this.transform.position = Camera.main.transform.position + scaled;
    }
}
