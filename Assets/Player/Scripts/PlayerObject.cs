
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
    @CoAuthor: Olli Korhonen
*/

public class PlayerObject : NetworkBehaviour
{
    private ScoreBoard scoreboardScript;
    private CameraScript cameraScript;

    [Space(10)]
    // Sync variable with other clients
    [SyncVar]
    public string nickname;
    // Sync variable with other clients
    [SyncVar]
    public Color color;

    [Space(10)]
    // Player's time score
    private double resultTime;
    private bool finished;

    // Player avatar color
    public Color avatarColor;

    // Use this for initialization
    void Start()
    { 
        updateColor();                                      // Render player object with correct color
        scoreboardScript = this.GetComponent<ScoreBoard>(); // Initialize scoreboard

        if (isLocalPlayer)
        {
            setupCameraFollow();                            // Set camera to follow local player
        }
    }

    private void setupCameraFollow()
    {
        cameraScript = Camera.main.gameObject.GetComponent<CameraScript>();
        cameraScript.target = this.transform;
        cameraScript.gameObject.SetActive(true);
    }

    public bool hasFinished()
    {
        return this.finished;
    }
    public void setResultTime(double time)
    {
        this.finished = true;
        this.resultTime = time;

        // Save name and time
        Debug.Log("player.m_time: " + resultTime);

        if (scoreboardScript != null)
        {
            scoreboardScript.SaveTimeScore(this.nickname, time);
        }
    }
    public double getResultTime()
    {
        return this.resultTime;
    }

    // Update player object color in Avatar Menu
    public void updateColor()
    {
        // Render player objects
        Renderer[] rends = GetComponentsInChildren<Renderer>();
        foreach (Renderer r in rends)
        {
            if (r.gameObject.layer == 9)
                r.material.color = avatarColor;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!System.String.Equals(collision.gameObject.tag, "Floor") && !System.String.Equals(collision.gameObject.tag, "Finish"))
        {
            //Don't allow annoying loop sound fx
            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();
        }
    }
}
