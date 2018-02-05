
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Prototype.NetworkLobby;
using AssemblyCSharp;
using com.shephertz.app42.paas.sdk.csharp;
using com.shephertz.app42.paas.sdk.csharp.storage;
using System.Collections.Generic;

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

    public Constant cons = new Constant();
    public float[] checkpoint_times = new float[7] { 0f, 0f, 0f, 0f, 0f, 0f, 0f};
    public static event TimerUpdated timerUpdated;

    public delegate void TimerUpdated(float time);

    public int countMax; //seconds before game starts after scene load
    private int countDown; //iteration time variable

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

    private UIManager uiManager;

    // Use this for initialization, runs when object is loaded
    void Start ()
    {
        gmfinal = false;
        surveyDialog.SetActive(false);
        
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("Cannot find UI Manager. Make sure GUICanvas is present in the scene.");
        } else
        {
            audioSource.clip = audioClip;
            GetBestScore();
            StartGameFunction();
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
        
        
    }
    public void SetCheckpointTimes(float[] times) { checkpoint_times = times; }
    public class UnityCallBack : App42CallBack
    {
        private float[] checkpoint_times = new float[7] { 0f, 0f, 0f, 0f, 0f, 0f, 0f };
        private GameManagerScript gm_m;
        public UnityCallBack(GameManagerScript gm)
        {
            gm_m = gm;
        }
        public void OnSuccess(object response)
        {
            Storage storage = (Storage)response;
            IList<Storage.JSONDocument> jsonDocList = storage.GetJsonDocList();
            for (int i = 0; i < jsonDocList.Count; i++)
            {
                App42Log.Console("objectId is " + jsonDocList[i].GetDocId());
                App42Log.Console("jsonDoc is " + jsonDocList[i].GetJsonDoc());
                for (int j = 0; j < 7; j++)
                {
                    var data = SimpleJSON.JSON.Parse(jsonDocList[i].GetJsonDoc());
                    var name = "checkpoint" + j + 1;
                    checkpoint_times[j] = data[name].AsFloat;
                }
            }
            gm_m.SetCheckpointTimes(checkpoint_times);
        }
        public void OnException(Exception e)
        {
            App42Log.Console("Exception : " + e);
        }
    }
    private void GetBestScore()
    {
        string dbName = "JANITORRUN";
        string collectionName = "BestTimeCheckpoints";
        String docId = "docId";
        App42Log.SetDebug(true);
        //Print output in your editor console  
        App42API.Initialize(cons.apiKey,cons.secretKey);
        StorageService storageService = App42API.BuildStorageService();
        storageService.FindDocumentById(dbName,collectionName,docId, new UnityCallBack(this));     
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
            Steering p_control = obj.GetComponent<Steering>();
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
            if (timerUpdated != null)
            {
                timerUpdated((float)result_time);
            }
            uiManager.resultTimePanel.setStatusText("Waiting for others");
            uiManager.resultTimePanel.setTime("Your time\n{0}", (long)(result_time * 1000)); // UI result time
            uiManager.resultTimePanel.show();

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
        Steering steering = player.GetComponent<Steering>();
        
        //Disable driving script
        steering.enabled = false;

        playersc = GetComponent<PlayerObject>();

        //Create countdown
        for (countDown = countMax; countDown > 0; countDown--)
        {
            uiManager.countDownText.text = countDown.ToString(); //UI update
            audioSource.Play();
            yield return new WaitForSeconds(1);
        }
        audioSource.clip = audioClip2;
        audioSource.Play();

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
    public void playerCrossesFinish(GameObject player_obj)
    {
        // Fetch controller and object from gameObject
        Steering p_control = player_obj.GetComponent<Steering>();
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
        uiManager.resultTimePanel.setStatusText("Returning to lobby...");
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
