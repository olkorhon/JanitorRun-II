using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Class that provides functions for UI bar refreshing and manipulation
/// </summary>

/*
    Class that provides functions for UI bar refreshing and manipulation

    @Author: Jussi Sepponen
*/

public class BarControl : MonoBehaviour
{
    public Image speed_bar;
    public Image boost_bar;
    public Image stamina_bar;

    private PlayerControl player;

	// Use this for initialization
	void Start ()
    {
        this.player = fetchLocalPlayer();
        if (this.player == null)
        {
            Debug.LogWarning("BarControl: Cannot link bars to player, could not find a local player.");
        }
        this.stamina_bar.fillAmount = 1f;
    }

    private PlayerControl fetchLocalPlayer()
    {
        PlayerControl control;
        GameObject[] player_controls = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in player_controls)
        {
            control = obj.GetComponent<PlayerControl>();
            if (control == null)
                continue;

            if (control.isLocalPlayer)
                return control;
        }
        return null;
    }

    // Update is called once per frame
    void Update ()
    {
        if (this.player != null)
        {
      
            if (this.speed_bar != null)
                this.speed_bar.fillAmount = this.player.getNormalizedSpeed();

            if (this.boost_bar != null)
                this.boost_bar.fillAmount = this.player.getNormalizedBoost();

            if (this.stamina_bar != null)
                this.stamina_bar.fillAmount = this.player.getCurrentStamina();
        }
	}
}


