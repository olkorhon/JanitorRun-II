
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

/// <summary>
/// Class that holds player object variables: name, colour, time.
/// </summary>

/*
    Holds main variables player name, colour and time.
    Adds the name to UI and renders the player object with selected colour
    Sets camera to follow local player

    @Author: Mikael Martinviita
*/



public class PlayerObject : NetworkBehaviour
{
    private MinimapControl minimap;
    private ScoreBoard scoreboardScript;
    private SmoothCameraWithBumper camerafollow;

    [Space(10)]
    // Sync variable with other clients
    [SyncVar]
    public string nickname;
    // Sync variable with other clients
    [SyncVar]
    public Color color;

    // Moving speed variables
    [Space(10)]
    public float camera_forward;
    public float camera_up;
    public float camera_right;

    [Space(10)]
    public float look_forward;
    public float look_up;
    public float look_right;

    [Space(10)]
    // Player's time score
    private double result_time;
    private bool finished;

    // Player avatar color
    public Color ava_color;

    // Use this for initialization
    void Start()
    {    
        // Render player objects
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (r.gameObject.layer == 9)
                r.material.color = color;
        }

        // Set names for players
        Text nameText = GetComponentInChildren<Text>();
        Debug.Log("player.name: " + nickname);
        nameText.text = nickname;

        // Set camera to follow local player
        if(isLocalPlayer)
        {
            Camera.main.transform.parent = this.transform;
            camerafollow = Camera.main.gameObject.GetComponent<SmoothCameraWithBumper>();
            camerafollow.target = this.transform;
            camerafollow.gameObject.SetActive(true);

            /*
            Camera.main.transform.position = this.transform.position 
                                           + this.transform.forward * camera_forward
                                           + this.transform.up * camera_up
                                           + this.transform.right * camera_right;

            Camera.main.transform.LookAt(this.transform.position
                                       + this.transform.forward * look_forward
                                       + this.transform.up * look_up
                                       + this.transform.right * look_right);
           
            Camera.main.transform.parent = this.transform;
            */
            
        }

        // Start minimapping, but only if we are the local player
        if (this.isLocalPlayer)
        {
            GameObject minimapObject = GameObject.FindGameObjectWithTag("Minimap");
            if (minimapObject != null)
            {
                minimap = minimapObject.GetComponent<MinimapControl>();
                minimap.startMinimap(this.gameObject.GetComponent<PlayerControl>());
            }
            else
                Debug.LogWarning("PlayerObject: Could not start minimap, no minimap found in the scene.");
        }

        // Initialize scoreboard
        scoreboardScript = this.GetComponent<ScoreBoard>();
    }

    public bool hasFinished()
    {
        return this.finished;
    }
    public void setResultTime(double time)
    {
        this.finished = true;
        this.result_time = time;

        // Save name and time
        Debug.Log("player.m_time: " + result_time);
        scoreboardScript.SaveTimeScore(this.nickname, time);
    }
    public double getResultTime()
    {
        return this.result_time;
    }

    // Update player object color in Avatar Menu
    public void UpdateColor()
    {
        // Render player objects
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (r.gameObject.layer == 9)
                r.material.color = ava_color;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!System.String.Equals(collision.gameObject.tag, "Floor") && !System.String.Equals(collision.gameObject.tag, "Finish"))
        {
            //Don't allow annoying loop sound fx
            if(!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();
        }
    }
}
