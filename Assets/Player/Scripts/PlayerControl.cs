
using UnityEngine;
using UnityEngine.Networking;
using Prototype.NetworkLobby;

/// <summary>
/// Class that contains code for player moving.
/// </summary>

/*
    Class that contains code for player moving.
    Player moves only in forward and -forward direction.
    Left and right turns the player object.
    Weather affects moving -> rain decreases turning speed by 15
        which makes it harder to control.

    @Author: Mikael Martinviita
*/

public class PlayerControl : NetworkBehaviour
{
    private Animator animator;

    public LobbyManager lobbyManager;
    public Transform angle_buffer;

    public float baseTorque; // Moving speed factor
    public float torque; // Turning speed factor
    public float max_speed=16f; // Practical max speed

    public float speedBoost;
    public float stamina;

    private Rigidbody rb;
    private Weather getWeather;
    //private PlayerGuide guide;

    private float t = 1.0f;
    private float moveInput; //Keyboard input factor for moving
    private float turnInput; //Keyboard input factor for turning
    public bool kickInput; //Keyboard input factor for kicking speed


    // Runs after object is loaded
    void Start()
    {
        this.animator = GetComponentInChildren<Animator>();
        if (this.animator == null)
        {
            Debug.LogWarning("PlayerControl: Could not find animator in player prefab, cannot link player object to animator");
        }

        // Set not finished and get objects
        rb = this.GetComponent<Rigidbody>();
    }

    // Get WeatherManager object, called by GetWeather.cs
    public void GetWeatherObject()
    {
        if (isLocalPlayer)
        {
            GameObject weatherObject = GameObject.FindWithTag("WeatherManager");
            getWeather = weatherObject.GetComponent<Weather>();
        }
    }
}

