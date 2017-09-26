using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that provides collision detecting for pickups
/// </summary>

/*
    Class that provides collision detecting for pickups. Calls playerControl to increase speed by givin speed burst.

    @Author: Jussi Sepponen
*/

public class PickUpmechanics : MonoBehaviour {

    Vector3 temp;
    public string PickupType;

    void Start()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        /*  y-axis movement for pickups. Lets do this when we have time
        Vector3 temp = transform.position;

        if (temp.y > 4.5)
            for (float i = temp.y; i < 5; i = i + 0.02f)
            {
                temp.y = temp.y + 0.02f;
                transform.position = temp;
            }
        for (float j = temp.y; j > 4.5; j = j - 0.2f)
        {
            temp.y = temp.y - j;
            transform.position = temp;
        }
        */
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerControl player = other.gameObject.GetComponent<PlayerControl>();
            if(PickupType=="Coffee")
            {
                player.BoostSpeed(2500f);
            }
            if(PickupType=="Food_Hamburger")
            {
                player.setStamina(1f);
            }
        }
        gameObject.SetActive(false);
    }


}


