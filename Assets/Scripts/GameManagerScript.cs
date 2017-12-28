
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using com.shephertz.app42.paas.sdk.csharp.storage;
using AssemblyCSharp;
using com.shephertz.app42.paas.sdk.csharp;


/// <summary>
/// Class that contains main game managing script. 
/// </summary>

/* 
    Contains game start function, game ending function, 
    UI time finalize and returning to back to lobby.

    @Author: Mikael Martinviita
*/


public class GameManagerScript : NetworkBehaviour
{
    public int countMax; //seconds before game starts after scene load
    private int countDown; //iteration time variable

    public Text countDownText; //UI text for countdown
    public Countdown timeoutCountdown; //Countdown script that displays a decreasing counter that calls a predefined function of the gamemanager after it finishes
    public Timer timeOutput; //Timer script that displays time with input of seconds passed
    //public Canvas canvas;
    //public Image backgroundImage; //UI image for resultTime
    public ResultPanel resultTimePanel; //UI text after final, shows result time
    public AudioClip audioClip; //Countdown audio
    public AudioClip audioClip2; //Countdown finish audio
    public AudioSource audioSource;

    public GameObject surveyDialog;

    private PlayerObject playersc; //Class that holds playerObject variables
    private GameObject[] players;

    [HideInInspector]
    public float startTime;

    private float endTime;
    public bool gmfinal; //bool variable for game ending
    private bool returning = false; // bool variable for returnToLobby function call check
    private int playersFinished = 0; // number of players that have finished

    // Use this for initialization, runs when object is loaded
    void Start ()
    {
        //PlayerPrefs.DeleteAll();

        gmfinal = false;
        //backgroundImage.enabled = false;
        resultTimePanel.hide();
        timeOutput.gameObject.SetActive(false);
        surveyDialog.SetActive(false);
        audioSource.clip = audioClip;
        StartGameFunction();
    }

    // Runs at every frame
    void Update()
    {
        // Check if game is ended
        if (!gmfinal)
        {
            //Update UI time
            UpdateTime();
        }
        else
        {
            //Check if other player leaves the lobby after local player finishes 
            if (LobbyManager.s_Singleton.numPlayers == 1 && !returning)
            {
                returning = true;
                //End game and return to back to lobby
                StartReturnToLobby();
            }
        }
        
        
    }

    // Update time in UI
    void UpdateTime()
    {
        timeOutput.refreshTime(Time.time - startTime);
    }

    public float getTimeNow()
    {
        return Time.time - startTime;
    }

    // Performs last rites for the players that did not finish and returns to the lobby
    public void startTimeoutProcess()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject obj in players)
        {
            PlayerControl p_control = obj.GetComponent<PlayerControl>();
            PlayerObject p_object = obj.GetComponent<PlayerObject>();

            p_object.setResultTime(Time.time - startTime);
        }

        StartReturnToLobby();
    }

    //Finalize time in UI. Function is called when player finish or when timeout ends.
    void finalizeTime(PlayerObject p_object)
    {
        endTime = Time.time;
        double result_time = endTime - startTime;

        // Set the final time of the player
        p_object.setResultTime(result_time);

        // Do this only for local player
        if (p_object.isLocalPlayer)
        {
            gmfinal = true; //disable UI time update

            // Activate and display the players time
            timeOutput.refreshTime((float)result_time); //UI update
            resultTimePanel.setStatusText("Waiting for others");
            resultTimePanel.setTime("Your time\n{0}", (long)(result_time * 1000)); // UI result time
            resultTimePanel.show();

            Debug.Log("Finalized time was: " + result_time);
        }
        else
            Debug.Log("Finalized remote time");
    }

    // Start GameStart coroutine
    void StartGameFunction()
    {
        StartCoroutine(GameStart());
    }

    // Creates <countMax> seconds UI countdown at the beginnig of the game.
    // Enables driving script and UI time for player after that.
    private IEnumerator GameStart()
    {
        //Get components
        GameObject player = GameObject.FindWithTag("Player");
        PlayerControl drivingScript = player.GetComponent<PlayerControl>();
        playersc = GetComponent<PlayerObject>();

        //Disable driving script
        drivingScript.enabled = false;

        //Create countdown
        for (countDown = countMax; countDown > 0; countDown--)
        {
            countDownText.text = countDown.ToString(); //UI update
            audioSource.Play();
            yield return new WaitForSeconds(1);
        }
        audioSource.clip = audioClip2;
        audioSource.Play();

        //Send number of players to Scoreboard.cs
        ScoreBoard scoreboard = player.GetComponent<ScoreBoard>();
        scoreboard.num_players = LobbyManager.s_Singleton.numPlayers;

        //Disable countdown UI and enable driving script and time UI
        countDownText.enabled = false;
        drivingScript.enabled = true;
        startTime = Time.time;
        timeOutput.gameObject.SetActive(true);

        yield return null;
    }

    // Called when object entered on collision zone of GameObject where this script is attached
    public void playerCrossesFinish(GameObject player_obj)
    {
        // Fetch controller and object from gameObject
        PlayerControl p_control = player_obj.GetComponent<PlayerControl>();
        PlayerObject p_object = player_obj.GetComponent<PlayerObject>();

        // If we are already finished, escape
        if (p_object.hasFinished())
            return;

        //Get all the players on the lobby
        players = GameObject.FindGameObjectsWithTag("Player");

        // Advance finished players counter
        playersFinished++;

        // Log position of the finisher
        switch (playersFinished)
        {
            case 1:
                Debug.Log("Winner: " + p_object.nickname);
                break;
            case 2:
                Debug.Log("Second: " + p_object.nickname);
                break;
            case 3:
                Debug.Log("Third: " + p_object.nickname);
                break;
            default:
                Debug.Log(playersFinished.ToString() + "th: " + p_object.nickname);
                break;
        }

        // Disable finishers controls
        p_control.disableControls();

        // Finalize the time for the player
        finalizeTime(p_object);

        // Start timeout timer if the first player finished
        if (playersFinished == 1 && players.Length != 1)
        {
            timeoutCountdown.gameObject.SetActive(true);
            timeoutCountdown.startCountdown(30000); // Time provided in milliseconds
        }

        //if (p_object.isLocalPlayer && PlayerPrefs.GetInt("answered", 0) == 0 && PlayerPrefs.GetInt("dialog_visible", 0) == 0)
        //{
            PlayerPrefs.SetInt("dialog_visible", 1);
            surveyDialog.SetActive(true);
        //}

        // If everyone has finished, start return to lobby
        if (playersFinished == players.Length)
            StartReturnToLobby();
    }

    // Start ReturnToLobby coroutine
    public void StartReturnToLobby()
    {
        resultTimePanel.setStatusText("Returning to lobby...");
        StartCoroutine(ReturnToLoby());
    }

    // Coroutine for returning back to lobby
    private IEnumerator ReturnToLoby()
    {
        // Wait 15s
        yield return new WaitForSeconds(10.0f);

        // Check number of players so that script calls right function
        if (LobbyManager.s_Singleton.numPlayers == 1)
        {
            // Throws AddPlayer error sometimes, nothing serious because local/host player is only on in the lobby
            try
            {
                // Make server return to lobby
                LobbyManager.s_Singleton.ServerReturnToLobby();
            }
            catch (NullReferenceException ex)
            {
                Debug.LogException(ex, this);
            }
                
        }
        else
        {
            // Send return message to server
            LobbyManager.s_Singleton.SendReturnToLobby();
        }
    }

}
