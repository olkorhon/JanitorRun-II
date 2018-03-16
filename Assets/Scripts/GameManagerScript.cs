
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;

/* 
    Contains game start function, game ending function, 
    UI time finalize and returning to back to lobby.

    @Author: Mikael Martinviita
*/

/// <summary>
/// Class that contains main game managing script. 
/// </summary>
public class GameManagerScript : NetworkBehaviour
{
    public static GameManagerScript Instance;

    public static event TimerUpdated timerUpdated;

    public delegate void TimerUpdated(float time);

    public int countMax; //seconds before game starts after scene load
    private int countDown; //iteration time variable

    public AudioClip audioClip; //Countdown audio
    public AudioClip audioClip2; //Countdown finish audio
    public AudioSource countdownSoundSource;
    public AudioSource finishSoundSource;

    public GameObject surveyDialog;
    public RectTransform resultPanel;

    private GameObject[] players;

    [HideInInspector]
    public float startTime;

    [HideInInspector]
    public double resultTime;

    private float endTime;
    public bool gmfinal; //bool variable for game ending
    private bool returning = false; // bool variable for returnToLobby function call check
    private int playersFinished = 0; // number of players that have finished

    private UIManager uiManager;
    private ProgressScript progress;

    // Use this for initialization, runs when object is loaded
    void Start ()
    {
        Instance = this;

        gmfinal = false;
        surveyDialog.SetActive(false);
        
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("Cannot find UI Manager. Make sure GUICanvas is present in the scene.");
        }
        else
        {
            countdownSoundSource.clip = audioClip;
            StartGameFunction();
        }

        progress = FindObjectOfType<ProgressScript>();
        if (progress == null)
        {
            Debug.LogWarning("Cannot find progress script, game cannot be finished");
        }
        else
        {
            progress.last_checkpoint_hit += playerCrossesFinish;
        }
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

        if (Input.GetButtonDown("Reset"))
        {
            Debug.Log("Resetting!");
            surveyDialog = null;
            StartReturnToLobby();
        } 
    }

    // Update time in UI
    void UpdateTime()
    {
        if (timerUpdated != null)
        {
            timerUpdated(Time.time - startTime);
        }
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
            //Steering p_control = obj.GetComponent<Steering>();
            PlayerObject p_object = obj.GetComponent<PlayerObject>();

            p_object.setResultTime(Time.time - startTime);
        }

        StartReturnToLobby();
    }

    //Finalize time in UI. Function is called when player finish or when timeout ends.
    void finalizeTime(PlayerObject p_object)
    {
        endTime = Time.time;
        resultTime = endTime - startTime;

        // Set the final time of the player
        p_object.setResultTime(resultTime);

        // Do this only for local player
        if (p_object.isLocalPlayer)
        {
            gmfinal = true; //disable UI time update

            // Activate and display the players time
            if (timerUpdated != null)
            {
                timerUpdated((float)resultTime);
            }

            Debug.Log("Finalized time was: " + resultTime);
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
        Steering steering = player.GetComponent<Steering>();
        
        //Disable driving script
        steering.enabled = false;

        //Create countdown
        for (countDown = countMax; countDown > 0; countDown--)
        {
            uiManager.countDownText.text = countDown.ToString(); //UI update
            countdownSoundSource.Play();
            yield return new WaitForSeconds(1);
        }
        countdownSoundSource.clip = audioClip2;
        countdownSoundSource.Play();

        //Send number of players to Scoreboard.cs
        updateScoreboardPlayerCount(player);

        //Disable countdown UI and enable time UI
        uiManager.countDownText.enabled = false;
        startTime = Time.time;
        uiManager.timeOutput.gameObject.SetActive(true);

        // Enable controls
        steering.enabled = true;

        yield return null;
    }

    private void updateScoreboardPlayerCount(GameObject player)
    {
        ScoreBoard scoreboard = player.GetComponent<ScoreBoard>();
        if (scoreboard != null)
            scoreboard.num_players = LobbyManager.s_Singleton.numPlayers;
        else
            Debug.LogWarning("No scoreboard attached to player");
    }

    // Called when object entered on collision zone of GameObject where this script is attached
    public void playerCrossesFinish(GameObject player_obj, Checkpoint checkpoint)
    {
        // Fetch controller and object from gameObject
        //Steering p_control = player_obj.GetComponent<Steering>();
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
        player_obj.GetComponent<Steering>().enabled = false;
        player_obj.GetComponent<AudioSource>().mute = true;
        finishSoundSource.Play();

        // Finalize the time for the player
        finalizeTime(p_object);

        // Start timeout timer if the first player finished
        if (playersFinished == 1 && players.Length != 1)
        {
            uiManager.timeoutCountdown.gameObject.SetActive(true);
            uiManager.timeoutCountdown.startCountdown(30000); // Time provided in milliseconds
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
        StartCoroutine(ReturnToLoby());
    }

    // Coroutine for returning back to lobby
    private IEnumerator ReturnToLoby()
    {
        // Wait 15s
        //yield return new WaitForSeconds(10.0f);

        while (surveyDialog != null)
        {
            yield return new WaitForSeconds(10.0f);
        }

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
