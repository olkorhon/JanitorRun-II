
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
    public bool final; // Bool for finishing

    public float speedBoost;
    public float stamina;

    private Rigidbody rb;
    private GetWeather getWeather;
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
        final = false;
        rb = this.GetComponent<Rigidbody>();

        //guide = this.GetComponent<PlayerGuide>();
        setStamina(1f);
    }

    // Get WeatherManager object, called by GetWeather.cs
    public void GetWeatherObject()
    {
        if (isLocalPlayer)
        {
            GameObject weatherObject = GameObject.FindWithTag("WeatherManager");
            getWeather = weatherObject.GetComponent<GetWeather>();
        }

    }

	// Update is called once per frame
	void Update ()
    {
        //Move only local player
        if(!isLocalPlayer)
        {
            return;
        }
        
        //Get keyboard input for not finished local player
        if (!final)
        {  
            turnInput = Input.GetAxis("Horizontal");
            moveInput = Input.GetAxis("Vertical");
            kickInput = Input.GetButtonDown("Fire1");

            Turn();
        }
    }

    //
    void FixedUpdate()
    {
        // Called after Update()
        //Move only not finished local player
        if (!final)
        {
            Move();
            Kick();

            if (angle_buffer != null)
            {
                TiltPlayer();
            }
        }
    }

    //Tilt players
    void TiltPlayer()
    {
        Vector3 projection = Vector3.Project(this.rb.velocity, transform.right);
        Vector3 angle_as_eulers = angle_buffer.localEulerAngles;

        float tilt_amount = projection.magnitude / 1.2f;

        if (projection.x * transform.right.x > 0 && projection.z * transform.right.z > 0)
            angle_as_eulers.x = tilt_amount;
        else
            angle_as_eulers.x = -tilt_amount;

        angle_buffer.localRotation = Quaternion.Euler(angle_as_eulers);
    }

    //Move player
    void Move()
    {
        t += Time.deltaTime * 0.5f;
        float incrForce = Mathf.Lerp(speedBoost, 0, t);

        float torqueCurweValue = -Mathf.Pow(this.rb.velocity.magnitude / this.max_speed, 2) + 1;
        float torqueMultiplier = baseTorque * torqueCurweValue + incrForce;

        // Create a vector in the direction the player is facing with a magnitude based on the input, speed and the time between frames.
        Vector3 movement = transform.forward * moveInput * torqueMultiplier * Time.fixedDeltaTime;

        // Figure mocing direction
        bool is_moving_forwards = Mathf.Sign(movement.x) == Mathf.Sign(transform.forward.x) && Mathf.Sign(movement.y) == Mathf.Sign(transform.forward.y);
        if (!is_moving_forwards)
            movement /= 2f;

        // Apply this movement to the rigidbody's position.
        rb.AddForce(movement);

        // Update Animator
        if (this.animator != null)
        {
            float magnitude = is_moving_forwards ? movement.magnitude : -movement.magnitude;
            this.animator.SetFloat("acceleration", magnitude * moveInput);
        }
    }

    //Turn player
    void Turn()
    {
        if (getWeather != null)
        {
            //Weather effect
            if (getWeather.weather == "Rain")
            {
                //Decrease 15 ->slower turning, harder to change direction on full speed
                torque = 45;
            }
        }
        // Determine the number of degrees to be turned based on the input, speed and time between frames.
        float turn = turnInput * torque * Time.deltaTime;

        // Make this into a rotation in the y axis.
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

        // Apply this rotation to the rigidbody's rotation.
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    public void disableControls()
    {
        this.final = true;
        Debug.Log("set playercontrol final: " + final);
    }
    
    public void Kick()
    {
        if (this.kickInput && stamina < .25f)
        {
            //Debug.LogWarning("Stamina : Not enuff ");
            //guide.gameObject.SetActive(true);
        }
        if (this.kickInput && stamina >= .25f )
        {
            stamina = stamina - .25f;
            setStamina(stamina);
            BoostSpeed(100f);
            //Debug.Log("Current stamina :  " + getCurrentStamina());
        }
    }
    

    public void StopBuffEffect(string pickup_id)
    {
        if (pickup_id == "Coffee")
        {
            //WaitForSeconds (2.5f);
            baseTorque = 1;
        }
    }

    public void BoostSpeed(float boost)
    {
        this.speedBoost = boost;
        this.t = 0.0f;
    }

    public float getNormalizedSpeed()
    {
        return this.rb.velocity.magnitude / this.max_speed;
    }

    public float getNormalizedBoost()
    {
        return 1f - t;
    }

    public float getCurrentStamina()
    {
        return stamina;
    }

    public void setStamina(float stamina)
    {
        this.stamina = stamina;
    }

}

